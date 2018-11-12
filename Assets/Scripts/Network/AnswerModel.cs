using System.Collections.Generic;
using System.Net;

public class AnswerModel
{
    public string Text { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public Dictionary<string, IList<string>> Errors { get; set; }

    public AnswerModel() { }

    public AnswerModel(string text)
    {
        Text = text;
        StatusCode = HttpStatusCode.OK;
        Errors = null;
    }

    public void SetFields(AnswerModel toCopy)
    {
        Text = toCopy.Text;
        StatusCode = toCopy.StatusCode;
        Errors = toCopy.Errors;
    }
}
