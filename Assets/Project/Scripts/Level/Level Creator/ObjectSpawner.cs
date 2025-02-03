using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _shotgunPrefab;
    [SerializeField] private int _maxSpawnAttempts = 40;
    [SerializeField] private int _minDistantionFromPlayer = 30;

    private LevelManager _manager;
    private bool _isInitialized = false;

    public void Initialize()
    {
        _manager = FindObjectOfType<LevelManager>();

        if (_manager == null)
        {
            Debug.LogError($"Can{"'"}t find Level Manager on the map!");
            return;
        }

        _isInitialized = true;
    }

    public void SpawnShotgun()
    {
        if (!_isInitialized && _manager == null)
        {
            Initialize();
        }
        else
            return;

        bool isSpawned = false;

        for (int t = 0; t < _maxSpawnAttempts; t++)
        {
            int x = Random.Range(0, _manager.MazeImage.width);
            int y = Random.Range(0, _manager.MazeImage.height);

            if (_manager.MazeImage.GetPixel(x, y) == Color.black | GetDistanceToNearestPlayer(new Vector3(x, 0, y)) < _minDistantionFromPlayer)
                continue;

            Instantiate(_shotgunPrefab, new Vector3(x, .1f, y), Quaternion.identity);
            Debug.Log($"Shotgun spawned at {x},0,{y}");

            isSpawned = true;
            break;
        }

        if (isSpawned)
            return;

        // Create shotgun at first free place what found if _maxSpawnAttempts was out
        for (int y = 0; y < _manager.MazeImage.height; y++)
        {
            for (int x = 0; x < _manager.MazeImage.width; x++)
            {
                if (_manager.MazeImage.GetPixel(x, y) == Color.black)
                    continue;

                Instantiate(_shotgunPrefab, new Vector3(x, .1f, y), Quaternion.identity);
                Debug.Log($"Shotgun spawned at {x},0,{y}. _maxSpawnAttempts was out");
            }
        }
    }

    private float GetDistanceToNearestPlayer(Vector3 position)
    {
        Player[] players = FindObjectsOfType<Player>();

        if (players.Length == 0)
        {
            Debug.LogError($"Can{"'"}t find Players on the map!");
            return 0;
        }

        float nearestDistance = 0f;

        foreach (var player in players)
        {
            if (Vector3.Distance(player.transform.position, position) < nearestDistance)
                nearestDistance = Vector3.Distance(player.transform.position, position);
        }

        return nearestDistance;
    }
}
