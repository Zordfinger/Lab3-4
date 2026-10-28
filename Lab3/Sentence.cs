using System.Xml.Serialization;

public class Sentence
{
    //[XmlArray("Tokens")]
    [XmlArrayItem("Word", typeof(Word))]
    [XmlArrayItem("Punctuation", typeof(Punctuation))]
    public List<Token> Tokens { get; set; } = new();

    [XmlIgnore]
    public int WordCount => Tokens.OfType<Word>().Count();

    [XmlIgnore]
    public bool IsQuestion => Tokens.Any(t => t is Punctuation p && p.Content == "?");

    public Sentence() { }

    public override string ToString() => string.Concat(Tokens.Select(t => t.LeadingWhitespace + t.Content));
}
