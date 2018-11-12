using System.Collections.Generic;

public class ErrorAnswer
{
    public string Message { get; set; }
    public Dictionary<string, IList<string>> ModelState { get; set; }
}
