public class Punctuation : Token
{
    public Punctuation(string value) : base(value) { }

    public override string ToString() => Content;
}
