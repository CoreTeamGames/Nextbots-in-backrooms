using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _spawnPoints = new GameObject[0];
    [SerializeField] private GameObject _player;

    public void Initialize()
    {
        // Инициализация только если массивы пусты
        if (_spawnPoints.Length == 0)
        {
            _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        }

        _player = GameObject.FindGameObjectWithTag("Player");


        // Проверка наличия объектов
        if (_player == null)
        {
            Debug.LogError($"Can{"'"}t find Player on the map!");
        }
        if (_spawnPoints.Length == 0)
        {
            Debug.LogError($"Can{"'"}t find Spawn Points on the map!");
        }
    }

    public void SpawnPlayer(bool canSpawnTogether = true)
    {
        // Инициализация, если массивы пусты
        if (_spawnPoints.Length == 0 || _player == null)
        {
            Initialize();
        }

        // Проверка наличия объектов после инициализации
        if (_spawnPoints.Length == 0 || _player == null)
        {
            Debug.LogError("Spawn points or player are missing!");
            return;
        }
        int spawnIndex = 0;

        // Выбираем случайную точку спавна
        spawnIndex = Random.Range(0, _spawnPoints.Length);


        // Устанавливаем позицию игрока
        _player.GetComponent<CharacterController>().enabled = false;
        _player.transform.position = _spawnPoints[spawnIndex].transform.position;
        Debug.Log($"Player spawned at {_spawnPoints[spawnIndex].transform.position}");
        _player.GetComponent<CharacterController>().enabled = true;
    }

    private void Update()
    {
        if (Keyboard.current.f1Key.isPressed)
        {
            SpawnPlayer();
        }
    }
}
