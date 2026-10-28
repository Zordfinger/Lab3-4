using System.IO;
using System.Text;
using System.Xml.Serialization;

[XmlRoot("Text")]
public class Text
{
    //[XmlArray("Sentences")]
    [XmlArrayItem("Sentence")]
    public List<Sentence> sentences = new List<Sentence>();

    public Text() { }

    public override string ToString() => string.Concat(sentences.Select(s => s.ToString()));

    public IEnumerable<Sentence> SentencesByWordCount() => sentences.OrderBy(s => s.WordCount);

    public IEnumerable<Sentence> SentencesByLength() => sentences.OrderBy(s => s.ToString().Length);

    public List<string> WordByLengthInQuestion(int wordlength) {
        return sentences
            .Where(s => s.IsQuestion)
            .SelectMany(s => s.Tokens.OfType<Word>())
            .Where(w => w.Content.Length == wordlength)
            .Select(w => w.Content)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void DeleteWordsSWConLetterByLength(int wordlength) {
        foreach (Sentence sentence in sentences)
        {
            for (int i = sentence.Tokens.Count - 1; i >= 0; i--)
            {
                if (sentence.Tokens[i] is Word word && word.Content.Length == wordlength && StartsWithConsonant(word.Content))
                    sentence.Tokens.RemoveAt(i);
            }
        }
    }

    private static bool StartsWithConsonant(string value)
    {
        if (string.IsNullOrEmpty(value) || !char.IsLetter(value[0]))
            return false;

        const string vowels = "aeiouyаеёиоуыэюя";
        return !vowels.Contains(char.ToLowerInvariant(value[0]));
    }

    public bool SwapWordByLengthWithSubstring(int sentenceNumber, int wordlength, string replacement) {
        if (sentenceNumber < 1 || sentenceNumber > sentences.Count)
            return false;

        foreach (Token token in sentences[sentenceNumber - 1].Tokens)
        {
            if (token is Word word && word.Content.Length == wordlength)
                word.Content = replacement;
        }
        return true;
    }

    public void RemoveStopwords(List<string> stopwords)
    {
        HashSet<string> set = new HashSet<string>(stopwords.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()),StringComparer.OrdinalIgnoreCase);

        foreach (Sentence sentence in sentences)
        {
            for (int i = sentence.Tokens.Count - 1; i >= 0; i--)
            {
                if (sentence.Tokens[i] is Word word && set.Contains(word.Content))
                    sentence.Tokens.RemoveAt(i);
            }
        }
    }

    public void XMlExport(string filename) {
        XmlSerializer serializer = new XmlSerializer(typeof(Text));
        using StreamWriter writer = new StreamWriter(filename, false, new UTF8Encoding(false));
        serializer.Serialize(writer, this);
    }
}
