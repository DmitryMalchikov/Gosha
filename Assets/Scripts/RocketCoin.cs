using UnityEngine;

public class RocketCoin : Coin
{
    void Update()
    {
        if (!GameController.Instance.Started) return;

        transform.Translate(GameController.Instance.Speed * Time.deltaTime, Space.World);

        if (transform.position.z < -18)
        {
            CoinGenerator.Instance.ResetCoin(this);
            gameObject.SetActive(false);
        }
    }

    public override void PickUp()
    {
        CoinGenerator.Instance.ResetCoin(this);
        base.PickUp();
    }
}
