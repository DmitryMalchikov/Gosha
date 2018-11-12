using UnityEngine;
using UnityEngine.UI;

public class WeeklyTask : TimeCheck
{
    public Text Task;
    public GameObject IsLocked;

    private PlayerTaskModel _info;

    public override IExpirable Info
    {
        get
        {
            return _info;
        }
    }

    public void SetTask(int number, PlayerTaskModel model)
    {
        _info = model;
        Task.text = model.GenerateDescription();

        if (_info.PlayerProgress >= _info.ActionsCount)
        {
            IsLocked.SetActive(false);
        }
    }
}
