using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _minSecsToStartGame, _maxSecsToStartGame, _maxSpawnAttempts, _minDistantionFromPlayer;
    [SerializeField] private float _timeToStartGame;
    [SerializeField] private NextbotPlacer _placer;
    [SerializeField] private GameObject _shotgunPrefab;
    [SerializeField] private MazeLoaderOptimized _loader;
    [SerializeField] private GameObject _player;
    private bool _isGameStarted = false;
    private Texture2D _mazeImage;

    public void Initialize()
    {
        _timeToStartGame = Time.time + Random.Range(_minSecsToStartGame, _maxSecsToStartGame);


        _mazeImage = _loader.MazeImage;

        // Получаем пиксели изображения
        Color[] pixels = _mazeImage.GetPixels();
        Color[,] map = new Color[_mazeImage.width, _mazeImage.height];

        // Проходим по пикселям и создаем объекты
        for (int _y = 0; _y < _mazeImage.height; _y++)
        {
            for (int _x = 0; _x < _mazeImage.width; _x++)
            {
                map[_x, _y] = pixels[_y * _mazeImage.width + _x];
            }
        }

        int x = 0;
        int y = 0;

        for (int z = 0; z < _maxSpawnAttempts; z++)
        {
            x = Random.Range(0, map.GetLength(0));
            y = Random.Range(0, map.GetLength(1));

            if (map[x, y] != Color.black && Vector3.Distance(_player.transform.position, new Vector3(x, 0, y)) >= _minDistantionFromPlayer)
                break;
        }

        Instantiate(_shotgunPrefab, new Vector3(x, 1, y), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _timeToStartGame && !_isGameStarted)
        {
            _isGameStarted = true;
            _placer.CreateNextbots();
        }    
    }
}
