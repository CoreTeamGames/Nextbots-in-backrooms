using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _minSecsToStartGame, _maxSecsToStartGame, _maxSpawnAttempts, _minDistantionFromPlayer;
    [SerializeField] private float _timeToStartGame;
    [SerializeField] private GameObject _shotgunPrefab;
    [SerializeField] private GameObject _player;

    [SerializeField] private MazeLoaderOptimized _loader;
    [SerializeField] private NextbotPlacer _nextbotPlacer;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private NavmeshCreator _navmeshCreator;

    private bool _isGameStarted = false;
    public Texture2D MazeImage { get; private set; }

    public void Initialize()
    {
        _timeToStartGame = Time.time + Random.Range(_minSecsToStartGame, _maxSecsToStartGame);

        MazeImage = _loader.MazeImage;

        // Получаем пиксели изображения
        Color[] pixels = MazeImage.GetPixels();
        Color[,] map = new Color[MazeImage.width, MazeImage.height];

        // Проходим по пикселям и создаем объекты
        for (int _y = 0; _y < MazeImage.height; _y++)
        {
            for (int _x = 0; _x < MazeImage.width; _x++)
            {
                map[_x, _y] = pixels[_y * MazeImage.width + _x];
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
            _nextbotPlacer.CreateNextbots();
        }    
    }
}
