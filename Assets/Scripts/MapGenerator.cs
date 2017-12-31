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
        }

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

            var number = Random.Range(0, _avaliableTiles.Count);

            _avaliableTiles[number].gameObject.SetActive(true);
            //TODO: Turn on all coins and etc

            if (_lastTile)
            {
                _avaliableTiles[number].transform.localPosition = _lastTile.transform.localPosition - Vector3.forward * TileSize * TileScale;
            }

            _lastTile = _avaliableTiles[number];
            _avaliableTiles.RemoveAt(number);

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
        //TODO: Turn on all coins and etc

        StartTile.transform.position = _lastTile.transform.position - Vector3.forward * TileSize * transform.localScale.z;

        _lastTile = tempLast;
    }

    public void NextTile()
    {
        if (_avaliableTiles.Count == 0) return;

        if (_lastTile && _lastTile.CanGoAfter.Count > 0)
        {
            for (int i = 0; i < _lastTile.CanGoAfter.Count; i++)
            {
                if (!_lastTile.CanGoAfter[i].gameObject.activeInHierarchy)
                {
                    _lastTile.CanGoAfter[i].gameObject.SetActive(true);
                    _lastTile.CanGoAfter[i].GenerateBonus();
                    _lastTile.CanGoAfter[i].GenerateBox();
                    _lastTile.CanGoAfter[i].transform.localPosition = _lastTile.transform.localPosition + Vector3.forward * TileSize;
                    _lastTile = _lastTile.CanGoAfter[i];
                    _avaliableTiles.Remove(_lastTile);
                    break;
                }
            }
        }
        else
        {
            var number = Random.Range(0, _avaliableTiles.Count);

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

        tile.transform.position = transform.position;
        tile.gameObject.SetActive(false);
    }
}
