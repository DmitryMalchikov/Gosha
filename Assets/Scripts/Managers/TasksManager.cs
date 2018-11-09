using System.Collections.Generic;

public class TasksManager : APIManager<TasksManager>
{
    public List<PlayerTasks> RunTasks = new List<PlayerTasks>();
    public List<PlayerTasks> JumpTasks = new List<PlayerTasks>();
    public List<PlayerTasks> PlayTasks = new List<PlayerTasks>();
    public List<PlayerTasks> CollectBonusTasks = new List<PlayerTasks>();

    public string SubmitTaskUrl = "/api/tasks/submittask";
    public string GetTasksUrl = "/api/tasks/WeeklyTasksInfo";

    public override void SetUrls()
    {
        ServerInfo.SetUrl(ref SubmitTaskUrl);
        ServerInfo.SetUrl(ref GetTasksUrl);
    }

    public void LoadTasks(List<PlayerTasks> tasks)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].PlayerStartProgress = tasks[i].PlayerProgress;

            switch (tasks[i].Type)
            {
                case "Run":
                    RunTasks.Add(tasks[i]);
                    break;
                case "Jump":
                    JumpTasks.Add(tasks[i]);
                    break;
                case "Play":
                    PlayTasks.Add(tasks[i]);
                    break;
                case "CollectBonus":
                    CollectBonusTasks.Add(tasks[i]);
                    break;
            }
        }
    }

    public void CheckTasks(TasksTypes type, int completeAmount = 1)
    {
        List<PlayerTasks> tasks = new List<PlayerTasks>();
        bool send = false;

        switch (type)
        {
            case TasksTypes.Run:
                tasks = RunTasks;
                break;
            case TasksTypes.Jump:
                tasks = JumpTasks;
                break;
            case TasksTypes.Play:
                tasks = PlayTasks;
                send = true;
                break;
            case TasksTypes.CollectBonus:
                tasks = CollectBonusTasks;
                break;
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].PlayerProgress += completeAmount;

            if (tasks[i].PlayerProgress >= tasks[i].ActionsCount || send)
            {
                SubmitTaskAsync(tasks[i]);

                if (tasks[i].PlayerProgress >= tasks[i].ActionsCount)
                {
                    tasks.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && GameController.Started)
        {
            SubmitAllTasks(false);
        }
    }

    public void SubmitAllTasks(bool reset)
    {
        List<PlayerTasks> tasks = new List<PlayerTasks>();

        tasks.AddRange(RunTasks);
        tasks.AddRange(JumpTasks);
        tasks.AddRange(PlayTasks);
        tasks.AddRange(CollectBonusTasks);

        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].InOneRun)
            {
                SubmitTaskAsync(tasks[i]);
            }
            else if (reset)
            {
                tasks[i].PlayerProgress = 0;
            }
        }
    }

    void SubmitTaskAsync(PlayerTasks model)
    {
        SubmitTaskModel value = new SubmitTaskModel() { Id = model.Id, PlayerProgress = model.PlayerProgress - model.PlayerStartProgress, TaskId = model.TaskId };
        CoroutineManager.SendRequest(SubmitTaskUrl, value, (PlayerTasksAnswer newModel) =>
       {
           model.Id = newModel.Id;
           model.PlayerStartProgress = model.PlayerProgress;

           if (model.PlayerProgress >= model.ActionsCount)
           {
               Canvaser.Instance.PopUpPanel.ShowTask(model.GenerateDescription());
           }
       });
    }

    public void GetAllTasksAsync()
    {
        CoroutineManager.SendRequest(GetTasksUrl, null, (List<PlayerTaskModel> tasks) =>
        {
            Canvaser.Instance.WeeklyTasks.SetTasks(tasks);
        }, loadingPanelsKey: "weeklytasks");
    }

}
