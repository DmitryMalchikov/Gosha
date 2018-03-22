﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnapScrolling : MonoBehaviour
{

    public int panCount;
    public float snapSpeed;
    public float scaleOffset;
    public float scaleSpeed;
    public ScrollRect scrollRect;
    public float minScrollVelocity;
    public float panOffset;
    public GameObject panPref;

    private GameObject[] instPans;
    private Vector2[] pansPos;
    private Vector2[] pansScale;
    private List<Image> pansImage;

    public List<SuitIcon> SuitIcons;

    public List<Costume> Costumes = new List<Costume>();

    public Color blocked;
    public Color normal;

    private RectTransform contentRect;
    private Vector2 contentVector;

    public int PrevPanID = -1;
    public int selectedPanID;
    private bool isScrolling;
    

    public List<InventoryItem> Cases;
    public List<int> CasesIds;

    public float offsetScale = Screen.width / 1080f;

    public ToggleGroup IconGroup;

    void Start()
    {
        contentRect = GetComponent<RectTransform>();
    }

    public void SetCostumes(List<Costume> suits)
    {
        CleanContent(transform);
        Costumes = suits;
        panCount = Costumes.Count;

        instPans = new GameObject[panCount];
        pansPos = new Vector2[panCount];
        pansScale = new Vector2[panCount];
        pansImage = new List<Image>();
        SuitIcons = new List<SuitIcon>();

        for (int i = 0; i < panCount; i++)
        {
            instPans[i] = Instantiate(panPref, transform, false);
            SuitIcon newIcon = instPans[i].GetComponent<SuitIcon>();
            //Image img = instPans[i].GetComponent<Image>();
            //img.sprite = Resources.Load<Sprite>(suits[i].Name);
            newIcon.Icon.sprite = Resources.Load<Sprite>(suits[i].Name);
            //pansImage.Add(img);
            newIcon.IsOn.group = IconGroup;
            if (Costumes[i].CostumeAmount == 0)
            {
                newIcon.IsOn.gameObject.SetActive(false);
                newIcon.Icon.color = blocked;
                Debug.Log("NoSuit");
                //pansImage[i].color = blocked;
            }
            else
            {
                newIcon.IsOn.isOn = (PlayerPrefs.GetString("CurrentSuit") == suits[i].Name);
            }
            SuitIcons.Add(newIcon);
            if (i == 0)
                continue;
            Debug.Log(instPans[i - 1].transform.localPosition.x);
            Debug.Log(instPans[i - 1].GetComponent<RectTransform>().sizeDelta.x);
            Debug.Log(panOffset);
            instPans[i].transform.localPosition = new Vector2(instPans[i - 1].transform.localPosition.x +
                instPans[i - 1].GetComponent<RectTransform>().sizeDelta.x + panOffset* offsetScale,
                instPans[i].transform.localPosition.y);
            pansPos[i] = -instPans[i].transform.localPosition;

        }
        Canvaser.Instance.Suits.SetCurrentCostume(Costumes[selectedPanID].Name, Costumes[selectedPanID].CostumeAmount > 0);
    }

    public void SetCases(List<InventoryItem> cases)
    {
        CleanContent(transform);
        Cases = cases;
        panCount = Cases.Sum(c => c.Amount)/*Cases.Count*/;

        instPans = new GameObject[panCount];
        pansPos = new Vector2[panCount];
        pansScale = new Vector2[panCount];
        pansImage = new List<Image>();
		CasesIds.Clear ();

        for (int j = 0; j < cases.Count; j++)
        {
			CasesIds.Add(cases[j].Id);

            for (int i = 0; i < cases[j].Amount/*panCount*/; i++)
            {                
                instPans[i] = Instantiate(panPref, transform, false);
                pansImage.Add(instPans[i].GetComponent<Image>());

                if (i == 0)
                    continue;
                instPans[i].transform.localPosition = new Vector2(instPans[i - 1].transform.localPosition.x +
                    panPref.GetComponent<RectTransform>().sizeDelta.x + panOffset * offsetScale,
                    instPans[i].transform.localPosition.y);
                pansPos[i] = -instPans[i].transform.localPosition;

            }
        }
    }
    public void PutOnSuit()
    {
        SuitIcons[selectedPanID].IsOn.isOn = true;
    }
    void FixedUpdate()
    {
        if (panCount > 0)
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
                pansScale[i].x = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale, scaleSpeed * Time.fixedDeltaTime);
                pansScale[i].y = Mathf.SmoothStep(instPans[i].transform.localScale.y, scale, scaleSpeed * Time.fixedDeltaTime);
                normal.a = scale;
                //pansImage[i].color = normal;
                instPans[i].transform.localScale = pansScale[i];
            }
            if (selectedPanID != PrevPanID)
            {
                if (Canvaser.Instance.CasesPanel.gameObject.activeInHierarchy)
                {
                    Canvaser.Instance.CasesPanel.SetCurrentCase(pansImage[selectedPanID]);
                }
                else
                {
                    Canvaser.Instance.Suits.SetCurrentCostume(Costumes[selectedPanID].Name, Costumes[selectedPanID].CostumeAmount > 0);
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
                snapSpeed * Time.fixedDeltaTime);
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
