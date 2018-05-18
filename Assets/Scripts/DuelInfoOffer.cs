using UnityEngine;

public class DuelInfoOffer : DuelInfo
{

    public GameObject WaitText;
    public GameObject AcceptButton;
    public GameObject ResultButton;

    public override void SetDuelPanel(DuelModel model)
    {
        base.SetDuelPanel(model);
        SetStatus(model);
    }


    void SetStatus(int status)
    {
        if (status == 0)
        {
            WaitText.SetActive(true);
            AcceptButton.SetActive(false);
        }
        else
        {
            WaitText.SetActive(false);
            AcceptButton.SetActive(true);
        }
    }

    void SetStatus(DuelModel model)
    {
        AcceptButton.SetActive(false);
        WaitText.SetActive(false);
        ResultButton.SetActive(false);

        if (model.Status == 3 || model.Status == 4)
        {
            ResultButton.SetActive(true);
        }
		else if (model.Result != null || model.Status == -1)
        {
            WaitText.SetActive(true);
        }
        else
        {
            AcceptButton.SetActive(true);
        }
    }

    public void StartRun()
    {
        DuelManager.Instance.StartRunAsync(info.Id);
        Run();
    }

    public void GetResults()
    {
        DuelManager.Instance.GetDuelResultAsync(info.Id);
    }
}
