using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QuickEye.RequestWatcher
{
    internal class TokenDefinition
    {
        private Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly int _precedence;

        public TokenDefinition(TokenType returnsToken, string regexPattern, int precedence)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var group = match.Groups.Count > 1 ? match.Groups[1] : match.Groups[0];

//                Debug.Log($"Match {match.Success} | {match} | {_returnsToken}");
                yield return new TokenMatch()
                {
                    StartIndex = group.Index,
                    EndIndex = group.Index + group.Length,
                    TokenType = _returnsToken,
                    Value = group.Value,
                    Precedence = _precedence
                };
            }
        }
    }
}