using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyTasksPanel : MonoBehaviour {

    public Transform Content;
    public GameObject TaskObject;
    public List<PlayerTaskModel> Tasks;

    public void SetTasks(List<PlayerTaskModel> models)
    {
        ClearContent();
        Tasks = models;
        
        for (int i = 0; i < models.Count; i++)
        {
            WeeklyTask newTask = Instantiate(TaskObject, Content).GetComponent<WeeklyTask>();
            newTask.SetTask(i + 1, models[i]);
        }

        gameObject.SetActive(true);       
    }
    public void ClearContent()
    {
        foreach (Transform item in Content)
        {
            Destroy(item.gameObject);
        }
    }
}
