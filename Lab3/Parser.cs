using System.Text.RegularExpressions;

public static class Parser
{
    private static readonly Regex TokenRegex = new(@"(?<space>\s*)(?<token>[\p{L}\p{M}\p{N}_]+(?:['’\-][\p{L}\p{M}\p{N}_]+)*|[^\s])", RegexOptions.Compiled);
    private static readonly Regex WordRegex = new(@"^[\p{L}\p{M}\p{N}_]+(?:['’\-][\p{L}\p{M}\p{N}_]+)*$", RegexOptions.Compiled);

    public static Text ParseText(string source)
    {
        Text result = new Text();
        Sentence current = new();

        foreach (Match match in TokenRegex.Matches(source))
        {
            string value = match.Groups["token"].Value;
            string whitespace = match.Groups["space"].Value;

            bool isWord = WordRegex.IsMatch(value);

            Token token = isWord
                ? new Word(value)
                : new Punctuation(value);

            token.LeadingWhitespace = whitespace;
            current.Tokens.Add(token);

            if (token is Punctuation p &&
                (p.Content == "." || p.Content == "!" || p.Content == "?"))
            {
                result.sentences.Add(current);
                current = new Sentence();
            }
        }

        if (current.Tokens.Count > 0)
            result.sentences.Add(current);

        return result;
    }

}
