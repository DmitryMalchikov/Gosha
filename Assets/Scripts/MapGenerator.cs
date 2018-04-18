using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public static MapGenerator Instance { get; private set; }

    public Tile StartTile;
    public float TileSize = 10;
    public int StartTilesNumber = 10;

    Tile[] _tiles;
    List<Tile> _avaliableTiles;
    Tile _lastTile;

    public float TileScale = 2;

    private void Awake()
    {
        Instance = this;
    }

    private void ResetTiles()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].gameObject.SetActive(false);
            _tiles[i].transform.localPosition = new Vector3(0, -1.4f, 0);
            _tiles[i].Generated = false;
            _tiles[i].Obstacles.StopCars();
            _tiles[i].CarsStarted = false;
        }
        StartTile.Obstacles.StopCars();
        StartTile.CarsStarted = false;
        _lastTile = null;
        _avaliableTiles = new List<Tile>(_tiles);
    }

    private void Start()
    {
        _tiles = GetComponentsInChildren<Tile>();

        StartGeneration();
    }

    public void StartGeneration()
    {
        ResetTiles();

        Tile tempLast = null;

        for (int i = 0; i < StartTilesNumber; i++)
        {
            Tile _currentNewTile = null;

            if (_lastTile)
            {
                var inactiveTiles = _lastTile.InactiveTiles;
                if (inactiveTiles.Count > 0)
                {
                    var number = Random.Range(0, inactiveTiles.Count);
                    _currentNewTile = inactiveTiles[number];
                }
            }

            if (!_currentNewTile)
            {
                var number = Random.Range(0, _avaliableTiles.Count);
                _currentNewTile = _avaliableTiles[number];
            }

            _currentNewTile.gameObject.SetActive(true);
            _currentNewTile.EnableObstcles();
            _currentNewTile.EnableIceCreams();

            if (_lastTile)
            {
                _currentNewTile.transform.localPosition = _lastTile.transform.localPosition - Vector3.forward * TileSize * TileScale;
            }

            _lastTile = _currentNewTile;
            _avaliableTiles.Remove(_currentNewTile);

            if (i != 0)
            {
                _lastTile.Generated = true;
            }
            else
            {
                tempLast = _lastTile;
            }
        }

        StartTile.gameObject.SetActive(true);
        StartTile.transform.position = _lastTile.transform.position - Vector3.forward * TileSize * transform.localScale.z;
        StartTile.EnableObstcles();

        _lastTile = tempLast;

        IceCreamRotator.SetRotator(true);
    }

    public void NextTile()
    {
        if (_avaliableTiles.Count == 0) return;

        if (_lastTile && _lastTile.CanGoAfter.Count > 0 && _lastTile.InactiveTiles.Count > 0)
        {
            var inactiveTiles = _lastTile.InactiveTiles;
            int number = Random.Range(0, inactiveTiles.Count);

            inactiveTiles[number].EnableObstcles();
            inactiveTiles[number].EnableIceCreams();
            inactiveTiles[number].gameObject.SetActive(true);
            inactiveTiles[number].GenerateBonus();
            inactiveTiles[number].GenerateBox();
            inactiveTiles[number].transform.localPosition = _lastTile.transform.localPosition + Vector3.forward * TileSize;
            _lastTile = inactiveTiles[number];
            _avaliableTiles.Remove(_lastTile);
        }
        else
        {
            var number = Random.Range(0, _avaliableTiles.Count);

            _avaliableTiles[number].EnableObstcles();
            _avaliableTiles[number].EnableIceCreams();
            _avaliableTiles[number].gameObject.SetActive(true);
            _avaliableTiles[number].GenerateBonus();
            _avaliableTiles[number].GenerateBox();

            if (_lastTile)
            {
                _avaliableTiles[number].transform.localPosition = _lastTile.transform.localPosition + Vector3.forward * TileSize;
            }

            _lastTile = _avaliableTiles[number];
            _avaliableTiles.RemoveAt(number);
        }
    }

    public void ResetTile(Tile tile, bool StartTile = false)
    {
        if (!StartTile)
        {
            _avaliableTiles.Add(tile);
        }

        tile.gameObject.SetActive(false);
    }
}
