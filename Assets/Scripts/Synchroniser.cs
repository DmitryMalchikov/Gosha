using System.Linq;

public class Synchroniser
{
    public static event ResultCallback OnActionsReady;
    private static bool[] ActionsDone;

    public static void SetReady(int number)
    {
        ActionsDone[number] = true;
        if (!ActionsDone.Any(b => !b) && OnActionsReady != null)
        {
            OnActionsReady();
            OnActionsReady = null;
        }
    }

    public static void NewSync(int count)
    {
        ActionsDone = new bool[count];
    }
}
