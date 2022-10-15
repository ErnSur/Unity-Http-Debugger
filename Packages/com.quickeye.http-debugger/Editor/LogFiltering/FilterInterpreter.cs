using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    internal class FilterInterpreter
    {
        private static readonly List<TokenDefinition> _TokenDefinitions;

        static FilterInterpreter()
        {
            _TokenDefinitions = new List<TokenDefinition>
            {
                new TokenDefinition(TokenType.ResultStatement, "res:", 1),
                new TokenDefinition(TokenType.MethodStatement, "met:", 1),
                new TokenDefinition(TokenType.IdStatement, "id:", 1),
                new TokenDefinition(TokenType.UrlStatement, "url:", 1),
                // this is troublesome, if we look greedy for everything inside " " then we treat other query statements with quotes as text literal
                // url can be tricki to filter for? why?
                // why do I need quoted text literals?
                //   - I need quoted text literals to look for text with spaces
                //   - Maybe treat space as token
                //   - if previous token was space and the new one is another text literal then join them with spaces
                //   - if not then it is treated as spacer
                //new TokenDefinition(TokenType.QuotedTextLiteral, @"(?:"")(.+?)(?:"")", 1),
                new TokenDefinition(TokenType.TextLiteral, @"(?::)(.+?)(?:,|\z|\n)", 2),
                //new TokenDefinition(TokenType.NumericLiteral, "-?\\d+", 2),
                new TokenDefinition(TokenType.Comma, ",", 1),
            };
        }

        public static IEnumerable<RequestData> Filter(ICollection<RequestData> requests, string query)
        {
            var currentStatement = TokenType.NotDefined;

            var selectors = new List<Func<RequestData, bool>>();
            var lowestTokenIndex = int.MaxValue;
            foreach (var token in Tokenize(query))
            {
                lowestTokenIndex = Math.Min(lowestTokenIndex, token.StartIndex);
                //Debug.Log($"Token {token}");
                switch (token.TokenType)
                {
                    case TokenType.NotDefined:
                        break;
                    case TokenType.ResultStatement:
                    case TokenType.MethodStatement:
                    case TokenType.IdStatement:
                    case TokenType.UrlStatement:
                        currentStatement = token.TokenType;
                        break;
                    case TokenType.QuotedTextLiteral:
                    case TokenType.TextLiteral:
                        switch (currentStatement)
                        {
                            case TokenType.ResultStatement:
                                selectors.Add(r =>
                                    r.lastResponse != null &&
                                    r.lastResponse.statusCode.ToString().StartsWith(token.Value));
                                break;
                            case TokenType.MethodStatement:
                                selectors.Add(r =>
                                    Enum.TryParse<HttpMethodType>(token.Value, true, out var method) &&
                                    r.type == method);
                                break;
                            case TokenType.UrlStatement:
                                selectors.Add(r => r.url.ToLower().Contains(token.Value.ToLower()));
                                break;
                            case TokenType.IdStatement:
                                selectors.Add(r => FilterByName(r,token.Value));
                                break;
                        }

                        break;
                    case TokenType.Comma:
                        break;
                }
            }

            if (currentStatement == TokenType.NotDefined)
                return requests.Where(r => FilterByName(r, query));
            return requests.Where(p => selectors.All(s => s(p)));
        }

        private static bool FilterByName(RequestData request, string name)
        {
            return request.name.ToLower().Contains(name.ToLower());
        }
        
        private static List<TokenMatch> GetTokenMatches(string query)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _TokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(query).ToList());

            return tokenMatches;
        }

        private static IEnumerable<TokenMatch> Tokenize(string query)
        {
            var tokenMatches = GetTokenMatches(query);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            foreach (var tokens in groupedByIndex)
            {
                var bestMatch = tokens.OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return bestMatch; //new Token(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }
        }
    }
}