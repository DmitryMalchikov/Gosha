using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager : MonoBehaviour
{
    public static TasksManager Instance { get; private set; }

    public List<PlayerTasks> RunTasks = new List<PlayerTasks>();
    public List<PlayerTasks> JumpTasks = new List<PlayerTasks>();
    public List<PlayerTasks> PlayTasks = new List<PlayerTasks>();
    public List<PlayerTasks> CollectBonusTasks = new List<PlayerTasks>();

    public string SubmitTaskUrl = "/api/tasks/submittask";
    public string GetTasksUrl = "/api/tasks/WeeklyTasksInfo";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetUrls();
    }
    public void SetUrls()
    {
        SubmitTaskUrl = ServerInfo.GetUrl(SubmitTaskUrl);
        GetTasksUrl = ServerInfo.GetUrl(GetTasksUrl);
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

    public void CheckTasks(string type, int completeAmount = 1)
    {
        List<PlayerTasks> tasks = new List<PlayerTasks>();
        bool send = false;

        switch (type)
        {
            case "Run":
                tasks = RunTasks;
                break;
            case "Jump":
                tasks = JumpTasks;
                break;
            case "Play":
                tasks = PlayTasks;
                send = true;
                break;
            case "CollectBonus":
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
        if (pause && GameController.Instance.Started)
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
        StartCoroutine(NetworkHelper.SendRequest(SubmitTaskUrl, JsonConvert.SerializeObject(value), "application/json", (response) =>
        {
            Debug.Log("OK");
            PlayerTasksAnswer newModel = JsonConvert.DeserializeObject<PlayerTasksAnswer>(response.Text);

            model.Id = newModel.Id;
            model.PlayerStartProgress = model.PlayerProgress;

            Canvaser.Instance.PopUpPanel.ShowTask(model.GenerateDescription());
        }));
    }

    public void GetAllTasksAsync()
    {
        Canvaser.ShowLoading(true);
        StartCoroutine(NetworkHelper.SendRequest(GetTasksUrl, "", "application/x-www-form-urlencoded", (response) =>
        {
            Debug.Log("OK");
            List<PlayerTaskModel> tasks = JsonConvert.DeserializeObject<List<PlayerTaskModel>>(response.Text);
            Canvaser.Instance.WeeklyTasks.SetTasks(tasks);
            Canvaser.ShowLoading(false);
        }));
    }

}
