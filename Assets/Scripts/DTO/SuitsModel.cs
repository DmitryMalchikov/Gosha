using System.Collections.Generic;

public class SuitsModel
{
    public List<Costume> Costumes { get; set; }
    public string SuitsHash { get; set; }

    public SuitsModel()
    {
        Costumes = new List<Costume>();
    }
}
