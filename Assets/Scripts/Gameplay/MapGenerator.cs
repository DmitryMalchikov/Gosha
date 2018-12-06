using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class MapGenerator : Singleton<MapGenerator>
    {
        public Tile StartTile;
        public float TileSize = 10;
        public byte StartTilesNumber = 10;

        private Tile[] _tiles;
        private List<Tile> _availableTiles;
        private Tile _lastTile;

        public float TileScale = 2;

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
            _availableTiles = new List<Tile>(_tiles);
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
                Tile currentNewTile = null;

                if (_lastTile)
                {
                    var inactiveTiles = _lastTile.InactiveTiles;
                    if (inactiveTiles.Length > 0)
                    {
                        var number = Random.Range(0, inactiveTiles.Length);
                        currentNewTile = inactiveTiles[number];
                    }
                }

                if (!currentNewTile)
                {
                    var number = Random.Range(0, _availableTiles.Count);
                    currentNewTile = _availableTiles[number];
                }

                currentNewTile.SetUpStartTile();

                if (_lastTile)
                {
                    currentNewTile.transform.localPosition = _lastTile.transform.localPosition - Vector3.forward * TileSize * TileScale;
                }

                _lastTile = currentNewTile;
                _availableTiles.Remove(currentNewTile);

                if (i != 0)
                {
                    _lastTile.Generated = true;
                }
                else
                {
                    tempLast = _lastTile;
                }
            }
        
            StartTile.transform.position = _lastTile.transform.position - Vector3.forward * TileSize * transform.localScale.z;
            StartTile.SetUpStartTile();

            _lastTile = tempLast;

            IceCreamRotator.SetRotator(true);
        }

        public void NextTile()
        {
            if (_availableTiles.Count == 0) return;

            if (_lastTile && _lastTile.CanGoAfter.Length > 0 && _lastTile.InactiveTiles.Length > 0)
            {
                var inactiveTiles = _lastTile.InactiveTiles;
                int number = Random.Range(0, inactiveTiles.Length);
                SetUpTile(inactiveTiles[number]);
            }
            else
            {
                var number = Random.Range(0, _availableTiles.Count);
                SetUpTile(_availableTiles[number]);
            }
        }

        public void SetUpTile(Tile tile)
        {
            tile.SetUp();

            if (_lastTile)
            {
                tile.transform.localPosition = _lastTile.transform.localPosition + Vector3.forward * TileSize;
            }

            _lastTile = tile;
            _availableTiles.Remove(tile);
        }

        public void ResetTile(Tile tile, bool StartTile = false)
        {
            if (!StartTile)
            {
                _availableTiles.Add(tile);
            }

            tile.gameObject.SetActive(false);
        }
    }
}
