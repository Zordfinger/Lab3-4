using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlInclude(typeof(Word))]
[XmlInclude(typeof(Punctuation))]
public abstract class Token
{
    [XmlText]
    public string Content { set; get; }

    protected Token(string value) => Content = value;
 }
