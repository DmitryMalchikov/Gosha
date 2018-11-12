using System.Text;

public class PlayerTasks
{
    public int Id { get; set; }
    public int PlayerProgress { get; set; }
    public int ActionsCount { get; set; }
    public string Type { get; set; }
    public bool InOneRun { get; set; }
    public int PlayerStartProgress { get; set; }
    /// <summary>
    /// achievements or weekly task id
    /// </summary>
    public int TaskId { get; set; }

    public string GenerateDescription()
    {
        StringBuilder description = new StringBuilder();

        switch (Type)
        {
            case "Run":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("runtask"), ActionsCount);
                break;
            case "Jump":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("jumptask"), ActionsCount);
                break;
            case "CollectIceCream":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("collecticecreamtask"), ActionsCount);
                break;
            case "Buy":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("buytask"), ActionsCount);
                break;
            case "Loose":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("loosetask"), ActionsCount);
                break;
            case "Play":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("playtask"), ActionsCount);
                break;
            case "CollectBonus":
                description.AppendFormat(LocalizationManager.GetLocalizedValue("collectbonustask"), ActionsCount);
                break;
            case "ShareVK":
                description.Append(LocalizationManager.GetLocalizedValue("sharetaskvk"));
                break;
            case "ShareFB":
                description.Append(LocalizationManager.GetLocalizedValue("sharetaskfb"));
                break;
            case "ShareOK":
                description.Append(LocalizationManager.GetLocalizedValue("sharetaskok"));
                break;
        }

        if (InOneRun)
        {
            description.Append(LocalizationManager.GetLocalizedValue("inonerun"));
        }

        return description.ToString();
    }
}
