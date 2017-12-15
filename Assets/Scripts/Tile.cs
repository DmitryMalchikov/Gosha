using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Generated = false;
    public bool StartTile = false;

    public Transform IceCreams;

    public Transform Bonus;
    public GameObject Obstacles;
    public GameObject Box;

    byte counter = 0;

    private void Update()
    {
        if (!GameController.Instance.Started) return;

        transform.Translate(GameController.Instance.Speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!GameController.Instance.Started) return;

        counter = (byte)((counter + 1) % 3);

        if (counter == 0) return;

        if (!Generated)
        {
            if (MapGenerator.Instance.transform.position.z - transform.position.z > MapGenerator.Instance.TileSize)
            {
                MapGenerator.Instance.NextTile();
                Generated = true;
            }
        }

        if (transform.position.z < -20)
        {
            Generated = false;
            for (int i = 0; i < IceCreams.childCount; i++)
            {
                IceCreams.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < Bonus.childCount; i++)
            {
                Bonus.GetChild(i).gameObject.SetActive(false);
            }            

            if (!StartTile)
            {
                Obstacles.SetActive(true);
                Box.SetActive(false);
            }
            MapGenerator.Instance.ResetTile(this, StartTile);

        }
    }

    public void GenerateBonus()
    {
        if (Random.Range(1, 11) <= GameController.Instance.BonusChance)
        {
            int rand = Random.Range(0, 4);

            Bonus.GetChild(rand).gameObject.SetActive(true);
        }
    }

    public void GenerateBox()
    {
        if (Random.Range(1, 101) <= GameController.Instance.BoxChance)
        {
            Box.SetActive(true);
        }
    }

    public void ClearObstacles()
    {
        Obstacles.SetActive(false);
    }
}

