namespace QuickEye.RequestWatcher
{
    internal enum TokenType
    {
        NotDefined,
        ResultStatement,
        MethodStatement,
        IdStatement,
        UrlStatement,
        TextLiteral,
        QuotedTextLiteral,
        Comma
    }
}