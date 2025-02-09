using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationUI : MonoBehaviour
{
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Toggle[] _gameModesToggles;
    [SerializeField] private Image _maze;
    [SerializeField] private TMPro.TMP_Text _mazeSizeText;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private float _stopCollisionProbality = 0.6f;
    [SerializeField] private Slider _mazeSizeSlider;

    private BackroomsGenerator _generator;

    public void Awake()
    {
        _generator = FindObjectOfType<BackroomsGenerator>();
        _generator.OnMapStartsCreatesEvent += OnMapStartsCreates;
        _generator.OnMapCreateStateChangedEvent += OnMapCreateStateChanged;
        _generator.OnMapCreatedEvent += OnMapCreated;

        _mazeSizeSlider.onValueChanged.AddListener(ChangeMazeSizeText);
        LoadMaze();

        if (_gameModesToggles.Length == 0)
            return;

        for (int i = 0; i < _gameModesToggles.Length; i++)
        {
            _toggleGroup.RegisterToggle(_gameModesToggles[i]);
            _gameModesToggles[i].onValueChanged.AddListener(SetGameMode);
            _gameModesToggles[i].SetIsOnWithoutNotify(i == (int)GameManager.CurrentGameMode ? true : false);
            _gameModesToggles[i].group = _toggleGroup;
        }
    }

    private void OnDestroy()
    {
        _generator.OnMapStartsCreatesEvent -= OnMapStartsCreates;
        _generator.OnMapCreateStateChangedEvent -= OnMapCreateStateChanged;
        _generator.OnMapCreatedEvent -= OnMapCreated;
        _mazeSizeSlider.onValueChanged.RemoveListener(ChangeMazeSizeText);

        if (_gameModesToggles.Length == 0)
            return;

        foreach (var item in _gameModesToggles)
        {
            item.onValueChanged.RemoveListener(SetGameMode);
        }
    }

    private void SetGameMode(bool b)
    {
        if (b == false)
            return;

        int maxIndex = Enum.GetNames(typeof(EGameModes)).Length;

        for (int i = 0; i < _gameModesToggles.Length; i++)
        {
            if (i > maxIndex)
                return;

            if (_gameModesToggles[i].isOn)
            {
                GameManager.CurrentGameMode = (EGameModes)i;
                return;
            }
        }
    }

    public async void CreateMaze()
    {
        if (_mazeSizeSlider == null)
            return;

        int mazeSize = (int)_mazeSizeSlider.value * 100;
        int numMazes = mazeSize * (int)_mazeSizeSlider.value * 5;
        int rooms = mazeSize / 20;

        await _generator.Generate(numMazes, _stopCollisionProbality, mazeSize, rooms);

        LoadMaze();
    }

    public void LoadMaze()
    {
        if (_generator == null)
            return;

        Texture2D t = new Texture2D(1, 1);

        t = _generator.LoadMaze(_generator.FileName);

        if (t == null)
        {
            CreateMaze();
            return;
        }

        Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one * 0.5f);
        GameManager.CurrentMazeTexture = t;
        _maze.sprite = s;
    }


    void OnMapStartsCreates()
    {
        _group.interactable = false;
    }

    void OnMapCreated()
    {
        _group.interactable = true;
    }

    void OnMapCreateStateChanged(string stateName)
    {
        switch (stateName)
        {
            case "generateMaze":
                Debug.Log("generateMaze");
                break;

            case "generateRooms":
                Debug.Log("generateRooms");
                break;

            case "generateSpawnpoints":
                Debug.Log("generateSpawnpoints");
                break;

            case "saveFile":
                Debug.Log("saveFile");
                break;
        }
    }

    public void ChangeMazeSizeText(float size)
    {
        _mazeSizeText.text = (size * 100).ToString();
    }
}