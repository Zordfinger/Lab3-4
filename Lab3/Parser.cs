using System.Text.RegularExpressions;

public static class Parser
{
    private static Regex TokenRegex = new(@"[\p{L}\p{M}\p{N}_]+(?:['’\-][\p{L}\p{M}\p{N}_]+)*|[^\s]", RegexOptions.Compiled);
    private static Regex WordRegex = new(@"^[\p{L}\p{M}\p{N}_]+(?:['’\-][\p{L}\p{M}\p{N}_]+)*$", RegexOptions.Compiled);

    public static Text ParseText(string source)
    {
        var result = new Text();
        Sentence current = new();

        foreach (Match match in TokenRegex.Matches(source))
        {
            string value = match.Value;

            bool isWord = WordRegex.IsMatch(value);

            Token token = isWord ? new Word(value) : new Punctuation(value);

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
