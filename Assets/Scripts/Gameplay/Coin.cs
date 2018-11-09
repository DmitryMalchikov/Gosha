using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour, IPickable
{
    private bool _reset = false;
    private Vector3 _defaultPosition;

    public virtual void PickUp()
    {
        GameController.Instance.AddCoin();
        gameObject.SetActive(false);
        AudioManager.PlayIceCreamPickup();
    }

    void Start()
    {
        _defaultPosition = transform.localPosition;
    }

    void OnEnable()
    {
        if (_reset)
        {
            _reset = false;
            transform.localPosition = _defaultPosition;
        }
    }

    public void OnMagnet()
    {
        _reset = true;
        StartCoroutine(MoveToPlayer());
    }

    IEnumerator MoveToPlayer()
    {
        while (true)
        {
            yield return CoroutineManager.Frame;
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position, GameController.Instance.CoinSpeed * SpeedController.SpeedMultiplier * Time.deltaTime);
        }
    }
}
