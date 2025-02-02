using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class NextbotPlacer : MonoBehaviour
{
    [SerializeField] private Nextbot[] _bots;
    [SerializeField] private GameObject _nextbotPrefab;
    [SerializeField] private int _minDistantionFromPlayer = 20;
    [SerializeField] private int _botsCount = 20;
    [SerializeField] private int _maxSpawnAttempts = 40;
    [SerializeField] private MazeLoaderOptimized _loader;
    private Texture2D _mazeImage;

    public void CreateNextBot(Nextbot bot, Vector3 position)
    {
        GameObject nextbot = Instantiate(_nextbotPrefab, position, Quaternion.identity);

        nextbot.GetComponentInChildren<MeshRenderer>().material.mainTexture = bot.Sprite.texture;

        nextbot.GetComponentInChildren<LookAtConstraint>().AddSource(new ConstraintSource() { sourceTransform = GameObject.FindGameObjectWithTag("Player").transform, weight = 1f });
    }

    public void CreateNextbots()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        _mazeImage = _loader.MazeImage;

        // Получаем пиксели изображения
        Color[] pixels = _mazeImage.GetPixels();
        Color[,] map = new Color[_mazeImage.width, _mazeImage.height];

        // Проходим по пикселям и создаем объекты
        for (int y = 0; y < _mazeImage.height; y++)
        {
            for (int x = 0; x < _mazeImage.width; x++)
            {
                map[x, y] = pixels[y * _mazeImage.width + x];
            }
        }

        for (int i = 0; i < _botsCount; i++)
        {
            int botIndex = Random.Range(0, _bots.Length);
            int x = 0;
            int y = 0;

            for (int z = 0; z < _maxSpawnAttempts; z++)
            {
                x = Random.Range(0, map.GetLength(0));
                y = Random.Range(0, map.GetLength(1));

                if (map[x, y] != Color.black && Vector3.Distance(player.transform.position, new Vector3(x, 0, y)) >= _minDistantionFromPlayer)
                    break;
            }

            CreateNextBot(_bots[botIndex], new Vector3(x, 0, y));
        }
    }
}
