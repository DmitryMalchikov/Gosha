using System;
using System.Collections;
using System.Net;
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
            catch (WebException ex)
            {
                //Debug.LogError(ex.Message);
                Canvaser.Errors.Enqueue(ex);                
                //check connection bla bla
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex.ToString());
                Canvaser.Errors.Enqueue(ex);
            }
            finally
            {
                Canvaser.ErrorChecked = true;
            }
        });
        System.Threading.Thread workerForOneRow = new System.Threading.Thread(pts);
        workerForOneRow.Start();
    }
}
