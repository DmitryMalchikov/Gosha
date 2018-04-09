using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Generated = false;
    public bool StartTile = false;

    public Transform IceCreams;

    public Transform Bonus;
    public CarObstacles Obstacles;
    public GameObject Box;

    public List<Tile> CanGoAfter = new List<Tile>();

    byte counter = 0;

    public bool CarsStarted = false;

    public List<Transform> IceCreamTrucks;

	[HideInInspector]
	public List<Collider> DisabledColliders = new List<Collider> ();

	private void Update(){
		if (!GameController.Instance.Started) return;

		transform.Translate(GameController.Instance.Speed * Time.deltaTime);
	}

    private void FixedUpdate()
    {
        if (!GameController.Instance.Started) return;

        counter = (byte)((counter + 1) % 3);

        if (counter != 0) return;

        if (!Generated)
        {
            if (MapGenerator.Instance.transform.position.z - transform.position.z > MapGenerator.Instance.TileSize)
            {
                StartCoroutine(NextTile());
            }
        }

		if(!CarsStarted && transform.position.z < MapGenerator.Instance.TileSize/2)
        {
            Obstacles.StartCars();
            CarsStarted = true;
        }

        if (transform.position.z < -MapGenerator.Instance.TileSize/2 - 10)
        {
            Generated = false;

            for (int i = 0; i < Bonus.childCount; i++)
            {
                Bonus.GetChild(i).gameObject.SetActive(false);
            }            

			//EnableColliders ();
			CarsStarted = false;
			Obstacles.StopCars();
			//EnableObstcles ();
			//Obstacles.gameObject.SetActive(true);

            if (!StartTile)
            {
                Box.SetActive(false);
            }

            MapGenerator.Instance.ResetTile(this, StartTile);
        }
    }

    public void GenerateBonus()
    {
        if (Random.Range(0, 100) <= GameController.Instance.BonusChance)
        {
            int rand = Random.Range(0, 4);

            Bonus.GetChild(rand).gameObject.SetActive(true);
        }
    }

    public void GenerateBox()
    {
        if (Random.Range(0, 100) <= GameController.Instance.BoxChance)
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

	public void DisableCollider(Collider col){
		DisabledColliders.Add (col);
		col.enabled = false;
	}

	public void EnableObstcles(){
		for (int i = 0; i < Obstacles.transform.childCount; i++) {
			Obstacles.transform.GetChild (i).gameObject.SetActive (true);
		}
	}

	public void EnableIceCreams(){
		for (int i = 0; i < IceCreams.childCount; i++)
		{
			IceCreams.GetChild(i).gameObject.SetActive(true);
		}
        for (int i = 0; i < IceCreamTrucks.Count; i++)
        {
            Coin[] TruckCoins = IceCreamTrucks[i].GetComponentsInChildren<Coin>();
            for (int j = 0; j < TruckCoins.Length; j++)
            {
                TruckCoins[j].gameObject.SetActive(true);
            }
        }
	}

	void EnableColliders(){
		for (int i = 0; i < DisabledColliders.Count; i++) {
			DisabledColliders [i].enabled = true;
		}

		DisabledColliders.Clear ();
	}
}

