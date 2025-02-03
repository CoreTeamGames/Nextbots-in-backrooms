using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _minSecsToStartGame, _maxSecsToStartGame, _maxSpawnAttempts, _minDistantionFromPlayer;
    [SerializeField] private float _timeToStartGame;
    [SerializeField] private string _filename = "backrooms.png";
    [SerializeField] private GameObject _shotgunPrefab;
    [SerializeField] private GameObject _player;

    [SerializeField] private MazeLoaderOptimized _loader;
    [SerializeField] private ObjectSpawner _objectSpawner;
    [SerializeField] private NextbotPlacer _nextbotPlacer;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private NavmeshCreator _navmeshCreator;

    private bool _isGameStarted = false;
    private bool _isInitialized = false;
    public Texture2D MazeImage { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _timeToStartGame = Time.time + Random.Range(_minSecsToStartGame, _maxSecsToStartGame);

        LoadImage(Path.Combine(Application.persistentDataPath, _filename));

        _loader = FindObjectOfType<MazeLoaderOptimized>();
        _objectSpawner = FindObjectOfType<ObjectSpawner>();
        _nextbotPlacer = FindObjectOfType<NextbotPlacer>();
        _playerSpawner = FindObjectOfType<PlayerSpawner>();
        _navmeshCreator = FindObjectOfType<NavmeshCreator>();

        if (_loader == null)
        {
            Debug.LogError("Can not find MazeLoaderOptimized on Scene!");
            return;
        }

        if (_objectSpawner == null)
        {
            Debug.LogError("Can not find ObjectSpawner on Scene!");
            return;
        }

        if (_nextbotPlacer == null)
        {
            Debug.LogError("Can not find NextbotPlacer on Scene!");
            return;
        }

        if (_playerSpawner == null)
        {
            Debug.LogError("Can not find PlayerSpawner on Scene!");
            return;
        }

        if (_navmeshCreator == null)
        {
            Debug.LogError("Can not find NavmeshCreator on Scene!");
            return;
        }

        _loader.Initialize();
        _playerSpawner.Initialize();
        _navmeshCreator.Initialize();
        _objectSpawner.Initialize();

        _isInitialized = true;
    }

    private void Start()
    {
        if (!_isInitialized)
            Initialize();
        if (!_isInitialized)
            return;

        _objectSpawner.SpawnShotgun();
        _loader.GenerateMap();
        _navmeshCreator.Bake();
        _playerSpawner.SpawnPlayers();
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

    private void LoadImage(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(1, 1);
        if (texture.LoadImage(fileData))
        {
            MazeImage = texture;
        }
        else
        {
            Debug.LogError("Could not to load maze image");
            return;
        }
    }
}
