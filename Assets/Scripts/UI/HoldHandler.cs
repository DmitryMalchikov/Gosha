using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float HoldTime = 0.5f;

    private bool _holding = false;
    private InputField _inputField;

    private void Start()
    {
        _inputField = GetComponent<InputField>();
    }

    public void HoldStarted()
    {
        _holding = true;
        StartCoroutine(Holding());
    }

    public void HoldEnded()
    {
        if (_holding)
        {
            StopCoroutine(Holding());
            _holding = false;
        }
    }

    IEnumerator Holding()
    {
        yield return new WaitForSeconds(HoldTime);

        if (_holding)
        {
            _holding = false;
            _inputField.text = UniClipboard.GetText();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        HoldStarted();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HoldEnded();
    }
}
