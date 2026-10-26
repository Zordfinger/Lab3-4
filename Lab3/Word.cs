
public class Word : Token
{
    public Word(string value) : base(value) { }
    
    public int Lenght => Content.Length;

    public override string ToString() => " " + Content;
}
