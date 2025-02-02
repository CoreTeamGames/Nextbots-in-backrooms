using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(BackroomsGenerator))]
public class BackroomsGeneratorUI : MonoBehaviour
{
    private BackroomsGenerator _generator;
    [SerializeField] private Image _maze;
    [SerializeField] private TMP_Text _mazeSizeText;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private float _stopCollisionProbality = 0.6f;

    [SerializeField] private Slider _mazeSizeSlider;
    private void Awake()
    {
        _generator = GetComponent<BackroomsGenerator>();
        _generator.OnMapStartsCreatesEvent += OnMapStartsCreates;
        _generator.OnMapCreateStateChangedEvent += OnMapCreateStateChanged;
        _generator.OnMapCreatedEvent += OnMapCreated;

        _mazeSizeSlider.onValueChanged.AddListener(ChangeMazeSizeText);
        LoadMaze();
    }

    public void OnDestroy()
    {
        _generator.OnMapStartsCreatesEvent -= OnMapStartsCreates;
        _generator.OnMapCreateStateChangedEvent -= OnMapCreateStateChanged;
        _generator.OnMapCreatedEvent -= OnMapCreated;
        _mazeSizeSlider.onValueChanged.RemoveListener(ChangeMazeSizeText);
    }

    public async void CreateMaze()
    {
        if (_mazeSizeSlider == null)
            return;

        int mazeSize = (int)_mazeSizeSlider.value * 100;
        int numMazes = mazeSize * 10;
        int rooms = mazeSize / 10;

        await _generator.Generate(numMazes, _stopCollisionProbality, mazeSize, rooms);

        LoadMaze();
    }

    public void LoadMaze()
    {
        if (_generator == null)
            return;

        Texture2D t = new Texture2D(1, 1);

        t = _generator.LoadMaze(_generator.FileName);

        Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one * 0.5f);
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