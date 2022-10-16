namespace QuickEye.WebTools.Editor
{
    internal class TokenMatch
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Precedence { get; set; }
        public override string ToString() => $"[{TokenType}] [{Value}] [{StartIndex}-{EndIndex}] [{Precedence}]";
    }
    
    internal class Token
    {
        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public TokenType TokenType { get; set; }
        public string Value { get; set; }
    }
}