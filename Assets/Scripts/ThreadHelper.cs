using System;
using System.Collections;
using UnityEngine;

public class ThreadHelper
{

    public static void RunNewThread(Action method)
    {
        System.Threading.ThreadStart pts = new System.Threading.ThreadStart(() =>
        {
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        });
        System.Threading.Thread workerForOneRow = new System.Threading.Thread(pts);
        workerForOneRow.Start();
    }
}
