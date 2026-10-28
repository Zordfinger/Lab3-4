using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlInclude(typeof(Word))]
[XmlInclude(typeof(Punctuation))]
public abstract class Token
{
    [XmlText]
    public string Content { set; get; }

    [XmlAttribute]
    public string LeadingWhitespace { get; set; } = "";

    public Token() { }
    public Token(string value) => Content = value;

    public override string ToString() => LeadingWhitespace + Content;
}
