using UnityEngine;

public class WeeklyTasksPanel : MonoBehaviour
{
    public Transform Content;
    public GameObject TaskObject;

    public void SetTasks(PlayerTaskModel[] models)
    {
        ClearContent();

        for (int i = 0; i < models.Length; i++)
        {
            WeeklyTask newTask = Instantiate(TaskObject, Content).GetComponent<WeeklyTask>();
            newTask.SetTask(i + 1, models[i]);
        }
    }
    public void ClearContent()
    {
        Content.ClearContent();
    }
}
