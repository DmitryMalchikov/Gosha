using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DTO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
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

        private GameObject[] _instPans;
        private Vector2[] _pansPos;
        private Vector2[] _pansScale;
        private bool _initialized;

        public List<SuitIcon> SuitIcons;
        public Costume[] Costumes;

        public Color blocked;
        public Color normal;

        private RectTransform _contentRect;
        private Vector2 _contentVector;

        public int PrevPanID = -1;
        public int selectedPanID;
        private bool _isScrolling;

        public float offsetScale = Screen.width / 1080f;

        public ToggleGroup IconGroup;

        void Start()
        {
            _contentRect = GetComponent<RectTransform>();
        }

        private IEnumerator SetIconsPosition()
        {
            yield return null;

            if (_initialized) yield break;
            for (int i = 1; i < _instPans.Length; i++)
            {
                _instPans[i].transform.localPosition = new Vector2(_instPans[i - 1].transform.localPosition.x +
                                                                   _instPans[i - 1].GetComponent<RectTransform>().sizeDelta.x + panOffset * offsetScale,
                    _instPans[i].transform.localPosition.y);
                _pansPos[i] = -_instPans[i].transform.localPosition;
            }

            yield return null;
            yield return new WaitForEndOfFrame();

            _initialized = true;
            Canvaser.Instance.Suits.SetCurrentCostume(Costumes[selectedPanID]);
            @group.alpha = 1;
        }

        public void SetCostumes(Costume[] suits)
        {
            CleanContent(transform);
            Costumes = suits;
            panCount = Costumes.Length;

            _initialized = false;
            _instPans = new GameObject[panCount];
            _pansPos = new Vector2[panCount];
            _pansScale = new Vector2[panCount];
            SuitIcons = new List<SuitIcon>();
            group.alpha = 0;

            for (int i = 0; i < panCount; i++)
            {
                _instPans[i] = Instantiate(panPref, transform, false);
                SuitIcon newIcon = _instPans[i].GetComponent<SuitIcon>();
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
            if (!_initialized || panCount <= 0) return;
            if (_contentRect.anchoredPosition.x >= _pansPos[0].x && !_isScrolling || _contentRect.anchoredPosition.x <= _pansPos[_pansPos.Length - 1].x && !_isScrolling)
            {
                scrollRect.inertia = false;
            }

            float nearestPos = float.MaxValue;
            for (int i = 0; i < panCount; i++)
            {
                float distance = Mathf.Abs(_contentRect.anchoredPosition.x - _pansPos[i].x);
                if (distance < nearestPos)
                {
                    nearestPos = distance;
                    selectedPanID = i;
                }
                float scale = Mathf.Clamp(1 / (distance /*/ (panOffset * offsetScale)*/) * scaleOffset, 0.5f, 1);
                _pansScale[i].x = Mathf.SmoothStep(_instPans[i].transform.localScale.x, scale, scaleSpeed * Time.deltaTime);
                _pansScale[i].y = Mathf.SmoothStep(_instPans[i].transform.localScale.y, scale, scaleSpeed * Time.deltaTime);
                normal.a = scale;
                //pansImage[i].color = normal;
                _instPans[i].transform.localScale = _pansScale[i];
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
            if (scrollVelocity < minScrollVelocity && !_isScrolling)
                scrollRect.inertia = false;
            if (scrollVelocity > minScrollVelocity || _isScrolling)
                return;
            if (_isScrolling)
                return;
            _contentVector.x = Mathf.SmoothStep(_contentRect.anchoredPosition.x, _pansPos[selectedPanID].x,
                snapSpeed * Time.deltaTime);
            _contentRect.anchoredPosition = _contentVector;
        }

        public void Scrolling(bool scroll)
        {
            _isScrolling = scroll;
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
}
