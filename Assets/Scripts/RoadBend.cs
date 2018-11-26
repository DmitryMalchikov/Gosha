using System.Collections;
using UnityEngine;
using VacuumShaders.CurvedWorld;

public class RoadBend : Singleton<RoadBend>
{
    [SerializeField]
    private CurvedWorld_Controller _curvController;

    public void StartBending()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeDirection());
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            byte rand = (byte)Random.Range(10, 21);
            yield return new WaitForSeconds(rand);
            float direction = (rand - 15) / 2;

            while (Mathf.Abs(_curvController._V_CW_Bend_Y - direction) > 0.01f && Time.timeScale > 0 && GameController.Started)
            {
                yield return null;
                _curvController._V_CW_Bend_Y -= Mathf.Sign(_curvController._V_CW_Bend_Y - direction) * 0.005f;
            }
        }
    }
}
