using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _players = new GameObject[0];
    [SerializeField] private LevelManager _manager;
    [SerializeField] private int _maxSpawnAttempts = 20;

    public void Initialize()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        _manager = FindObjectOfType<LevelManager>();

        if (_players.Length == 0)
        {
            Debug.LogError($"Can{"'"}t find Players on the map!");
        }

        if (_manager == null)
        {
            Debug.LogError($"Can{"'"}t find Level Manager on the map!");
        }
    }

    public void SpawnPlayer(bool canSpawnTogether = true)
    {
        if (_players.Length == 0 || _manager == null)
        {
            Initialize();
        }

        if (_players.Length == 0 || _manager == null)
        {
            Debug.LogError("Spawn points or player are missing!");
            return;
        }


        for (int i = 0; i < _players.Length; i++)
        {
            bool isSpawned = false;

            for (int t = 0; t < _maxSpawnAttempts; t++)
            {
                int x = Random.Range(0, _manager.MazeImage.width);
                int y = Random.Range(0, _manager.MazeImage.height);

                if (_manager.MazeImage.GetPixel(x, y) == Color.black)
                    continue;

                _players[i].GetComponent<CharacterController>().enabled = false;
                _players[i].transform.position = new Vector3(x, 0.1f, y);
                Debug.Log($"Player {i} spawned at {x},0,{y}");
                _players[i].GetComponent<CharacterController>().enabled = true;

                isSpawned = true;
            }

            if (isSpawned)
                continue;

            // Create player at first free place what found if _maxSpawnAttempts was out
            for (int y = 0; y < _manager.MazeImage.height; y++)
            {
                for (int x = 0; x < _manager.MazeImage.width; x++)
                {
                    if (_manager.MazeImage.GetPixel(x, y) == Color.black)
                        continue;

                    _players[i].GetComponent<CharacterController>().enabled = false;
                    _players[i].transform.position = new Vector3(x, 0.1f, y);
                    Debug.Log($"Player {i} spawned at {x},0,{y}. _maxSpawnAttempts was out");
                    _players[i].GetComponent<CharacterController>().enabled = true;
                }
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.f1Key.isPressed)
        {
            SpawnPlayer();
        }
    }
}
