using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapScrolling : MonoBehaviour
{
    public int panCount;
    public float snapSpeed;
    public float scaleOffset;
    public float scaleSpeed;
    public ScrollRect scrollRect;
    public CanvasGroup group;
    public float minScrollVelocity;
    public float panOffset;
    public GameObject panPref;

    private GameObject[] instPans;
    private Vector2[] pansPos;
    private Vector2[] pansScale;
    private bool initialized = false;

    public List<SuitIcon> SuitIcons;
    public Costume[] Costumes;

    public Color blocked;
    public Color normal;

    private RectTransform contentRect;
    private Vector2 contentVector;

    public int PrevPanID = -1;
    public int selectedPanID;
    private bool isScrolling;

    public float offsetScale = Screen.width / 1080f;

    public ToggleGroup IconGroup;

    void Start()
    {
        contentRect = GetComponent<RectTransform>();
    }

    private IEnumerator SetIconsPosition()
    {
        yield return null;

        if (!initialized)
        {
            for (int i = 1; i < instPans.Length; i++)
            {
                instPans[i].transform.localPosition = new Vector2(instPans[i - 1].transform.localPosition.x +
instPans[i - 1].GetComponent<RectTransform>().sizeDelta.x + panOffset * offsetScale,
instPans[i].transform.localPosition.y);
                pansPos[i] = -instPans[i].transform.localPosition;
            }

            yield return null;
            yield return new WaitForEndOfFrame();

            initialized = true;
            Canvaser.Instance.Suits.SetCurrentCostume(Costumes[selectedPanID]);
            group.alpha = 1;
        }
    }

    public void SetCostumes(Costume[] suits)
    {
        CleanContent(transform);
        Costumes = suits;
        panCount = Costumes.Length;

        initialized = false;
        instPans = new GameObject[panCount];
        pansPos = new Vector2[panCount];
        pansScale = new Vector2[panCount];
        SuitIcons = new List<SuitIcon>();
        group.alpha = 0;

        for (int i = 0; i < panCount; i++)
        {
            instPans[i] = Instantiate(panPref, transform, false);
            SuitIcon newIcon = instPans[i].GetComponent<SuitIcon>();
            newIcon.Icon.sprite = Resources.Load<Sprite>(suits[i].Name);
            newIcon.IsOn.group = IconGroup;
            if (Costumes[i].CostumeAmount == 0)
            {
                newIcon.IsOn.gameObject.SetActive(false);
                newIcon.Icon.color = blocked;
            }
            else
            {
                newIcon.IsOn.isOn = (PlayerPrefs.GetString("CurrentSuit") == suits[i].Name);
            }

            SuitIcons.Add(newIcon);
        }

        StartCoroutine(SetIconsPosition());
    }

    public void PutOnSuit(bool toSwitchOn = true)
    {
        if (!AnySuits())
        {
            return;
        }

        SuitIcons[selectedPanID].IsOn.isOn = toSwitchOn;
        Canvaser.Instance.Suits.TakeOffSuitBtn.gameObject.SetActive(toSwitchOn);
    }

    public bool AnySuits()
    {
        return SuitIcons != null && SuitIcons.Count > 0;
    }

    void Update()
    {
        if (initialized && panCount > 0)
        {
            if (contentRect.anchoredPosition.x >= pansPos[0].x && !isScrolling || contentRect.anchoredPosition.x <= pansPos[pansPos.Length - 1].x && !isScrolling)
            {
                scrollRect.inertia = false;
            }

            float nearestPos = float.MaxValue;
            for (int i = 0; i < panCount; i++)
            {
                float distance = Mathf.Abs(contentRect.anchoredPosition.x - pansPos[i].x);
                if (distance < nearestPos)
                {
                    nearestPos = distance;
                    selectedPanID = i;
                }
                float scale = Mathf.Clamp(1 / (distance /*/ (panOffset * offsetScale)*/) * scaleOffset, 0.5f, 1);
                pansScale[i].x = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale, scaleSpeed * Time.deltaTime);
                pansScale[i].y = Mathf.SmoothStep(instPans[i].transform.localScale.y, scale, scaleSpeed * Time.deltaTime);
                normal.a = scale;
                //pansImage[i].color = normal;
                instPans[i].transform.localScale = pansScale[i];
            }
            if (selectedPanID != PrevPanID)
            {
                if (!Canvaser.Instance.CasesPanel.gameObject.activeInHierarchy)
                {
                    Canvaser.Instance.Suits.SetCurrentCostume(Costumes[selectedPanID]);
                }
                PrevPanID = selectedPanID;
            }
            float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);
            if (scrollVelocity < minScrollVelocity && !isScrolling)
                scrollRect.inertia = false;
            if (scrollVelocity > minScrollVelocity || isScrolling)
                return;
            if (isScrolling)
                return;
            contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, pansPos[selectedPanID].x,
                snapSpeed * Time.deltaTime);
            contentRect.anchoredPosition = contentVector;
        }
    }

    public void Scrolling(bool scroll)
    {
        isScrolling = scroll;
        if (scroll)
            scrollRect.inertia = true;
    }

    public void CleanContent(Transform content)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
    }
}
