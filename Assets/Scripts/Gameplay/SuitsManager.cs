using UnityEngine;

public class SuitsManager : Singleton<SuitsManager>
{
    private static SuitInfo[] _suitsItems;

    private void Start()
    {
        _suitsItems = GetComponentsInChildren<SuitInfo>();
        PutOnSuit(PlayerPrefs.GetString("CurrentSuit"));
    }

    public static void PutOnSuit(string suitName)
    {
        for (int i = 0; i < _suitsItems.Length; i++)
        {
            _suitsItems[i].gameObject.SetActive(_suitsItems[i].SuitName == suitName);
        }
    }

    public static void TakeOffSuits()
    {
        for (int i = 0; i < _suitsItems.Length; i++)
        {
            _suitsItems[i].gameObject.SetActive(false);
        }
    }
}
