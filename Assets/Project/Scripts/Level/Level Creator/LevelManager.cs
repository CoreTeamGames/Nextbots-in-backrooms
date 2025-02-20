using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _minSecsToStartGame, _maxSecsToStartGame, _maxSpawnAttempts, _minDistantionFromPlayer;
    [SerializeField] private float _timeToStartGame;
    [SerializeField] private GameObject _shotgunPrefab;
    [SerializeField] private GameObject _player;

    [SerializeField] private MazeLoaderOptimized _loader;
    [SerializeField] private ObjectSpawner _objectSpawner;
    [SerializeField] private NextbotPlacer _nextbotPlacer;
    [SerializeField] private NextbotChaseManager _chaseManager;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private NavmeshCreator _navmeshCreator;

    private bool _isInitialized = false;

    public static bool IsGameStarted { get; private set; } = false;
    public Texture2D MazeImage { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        LoadImage();

        _loader = FindObjectOfType<MazeLoaderOptimized>();
        _objectSpawner = FindObjectOfType<ObjectSpawner>();
        _nextbotPlacer = FindObjectOfType<NextbotPlacer>();
        _chaseManager = FindObjectOfType<NextbotChaseManager>();
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

        if (_chaseManager == null)
        {
            Debug.LogError("Can not find NextbotChaseManager on Scene!");
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

        _loader.GenerateMap();
        _playerSpawner.SpawnPlayers();

        Debug.Log($"Loading game in mode: {GameManager.CurrentGameMode}");

        switch (GameManager.CurrentGameMode)
        {
            case EGameModes.Backrooms:
                break;
            case EGameModes.Nextbots:
                SetupGameNextbots();
                break;
            case EGameModes.FreeMode:
                SetupGameFreeMode();
                break;
            default:
                break;
        }
    }

    private void LoadImage()
    {
        if (GameManager.CurrentMazeTexture == null)
        {
            Debug.LogError("Could not to load maze image");
            return;
        }

        MazeImage = GameManager.CurrentMazeTexture;
    }

    private void SetupGameFreeMode()
    {

    }

    private void SetupGameNextbots()
    {
        _navmeshCreator.Bake();
        ChaseManager.canChase = true;
        _objectSpawner.SpawnShotgun();

        _timeToStartGame = Time.time + Random.Range(_minSecsToStartGame, _maxSecsToStartGame);

        StartCoroutine(StartGameNextbots(_timeToStartGame));
    }

    private IEnumerator StartGameNextbots(float timeToStartGame)
    {
        while (Time.time < timeToStartGame)
        {
            yield return null;
        }
        _nextbotPlacer.CreateNextbots(MazeImage.width/10);
        _chaseManager.Initialize();
        IsGameStarted = true;
        FindObjectOfType<StopwatchUI>().StartStopwatch();
    }
}