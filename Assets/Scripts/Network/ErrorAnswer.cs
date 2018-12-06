using System.Collections.Generic;

namespace Assets.Scripts.Network
{
    public class ErrorAnswer
    {
        public string Message { get; set; }
        public Dictionary<string, IList<string>> ModelState { get; set; }
    }
}
