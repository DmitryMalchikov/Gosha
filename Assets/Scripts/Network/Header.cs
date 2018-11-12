public class Header
{
    public string Name { get; set; }
    public string Value { get; set; }

    public Header() { }

    public Header(string header, string val)
    {
        Name = header;
        Value = val;
    }
}
