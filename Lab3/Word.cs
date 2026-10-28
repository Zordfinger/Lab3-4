
public class Word : Token
{
    public Word() { }

    public Word(string value) : base(value) { }
    
    public int Length => Content.Length;
}
