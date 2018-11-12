public class NameLocalization
{
    public string Name { get; set; }
    public string NameRu { get; set; }

    public NameLocalization() { }

    public NameLocalization(string name, string nameRu)
    {
        Name = name;
        NameRu = nameRu;
    }

    public void SetNames(string name, string nameRu)
    {
        Name = name;
        NameRu = NameRu;
    }
}
