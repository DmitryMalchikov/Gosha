using System.Collections.Generic;

public class SubmitScoreModel : SubmitModel
{
    public int IceCreamCount { get; set; }
    public int CasesCount { get; set; }
    public int Distance { get; set; }
    public bool NotContinued { get; set; }
    public Dictionary<int, byte> Uses { get; set; }
}
