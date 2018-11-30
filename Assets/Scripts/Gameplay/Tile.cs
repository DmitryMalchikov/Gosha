using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Generated = false;
    public bool StartTile = false;

    public Transform IceCreams;
    public Transform Bonus;
    public CarObstacles Obstacles;
    public GameObject Box;

    public Tile[] CanGoAfter;

    public Tile[] InactiveTiles { get { return CanGoAfter.Where(t => t.gameObject.activeInHierarchy == false).ToArray(); } }

    byte counter = 0;

    public bool CarsStarted = false;

    public GameObject[] TruckIceCreams;
    private List<Collider> _disabledColliders = new List<Collider>();
    private static bool _bonusLastTile;

    private void Update()
    {
        if (!GameController.Started) return;

        transform.Translate(SpeedController.Speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!GameController.Started) return;

        counter = (byte)((counter + 1) % 3);

        if (counter != 0) return;

        if (!Generated)
        {
            if (MapGenerator.Instance.transform.position.z - transform.position.z > MapGenerator.Instance.TileSize * 0.85f)
            {
                StartCoroutine(NextTile());
            }
        }

        if (!CarsStarted && transform.position.z < MapGenerator.Instance.TileSize / 2)
        {
            Obstacles.StartCars();
            CarsStarted = true;
        }

        if (transform.position.z < -MapGenerator.Instance.TileSize / 2 - 10)
        {
            Generated = StartTile;

            for (int i = 0; i < Bonus.childCount; i++)
            {
                Bonus.GetChild(i).gameObject.SetActive(false);
            }
            CarsStarted = false;
            Obstacles.StopCars();

            if (!StartTile)
            {
                Box.SetActive(false);
            }

            MapGenerator.Instance.ResetTile(this, StartTile);
        }
    }

    public void SetUpStartTile()
    {
        EnableObstcles();
        EnableIceCreams();
        gameObject.SetActive(true);
    }

    public void SetUp()
    {
        SetUpStartTile();
        GenerateBonus();
        GenerateBox();
    }

    public void GenerateBonus()
    {
        if (_bonusLastTile)
        {
            _bonusLastTile = false;
            return;
        }

        for (int i = 0; i < Bonus.childCount; i++)
        {
            Bonus.GetChild(i).gameObject.SetActive(false);
        }

        if (BonusChanceGenerator.GenerateBonus())
        {
            int rand = Random.Range(0, Bonus.childCount);

            Bonus.gameObject.SetActive(true);
            Bonus.GetChild(rand).gameObject.SetActive(true);
            _bonusLastTile = true;
        }
    }

    public void GenerateBox()
    {
        if (BonusChanceGenerator.GenerateBox())
        {
            Box.SetActive(true);
        }
    }

    IEnumerator NextTile()
    {
        yield return new WaitForEndOfFrame();

        MapGenerator.Instance.NextTile();
        Generated = true;
    }

    public void DisableCollider(Collider col)
    {
        _disabledColliders.Add(col);
        col.enabled = false;
    }

    public void EnableObstcles()
    {
        for (int i = 0; i < Obstacles.transform.childCount; i++)
        {
            Obstacles.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void EnableIceCreams()
    {
        IceCreams.gameObject.SetActive(true);

        for (int i = 0; i < IceCreams.childCount; i++)
        {
            IceCreams.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < TruckIceCreams.Length; i++)
        {
            TruckIceCreams[i].SetActive(true);
        }
    }

    void EnableColliders()
    {
        for (int i = 0; i < _disabledColliders.Count; i++)
        {
            _disabledColliders[i].enabled = true;
        }

        _disabledColliders.Clear();
    }
}

