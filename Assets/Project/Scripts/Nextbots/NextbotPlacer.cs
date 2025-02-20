using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class NextbotPlacer : MonoBehaviour
{
    [SerializeField] private Nextbot[] _bots;
    [SerializeField] private GameObject _nextbotPrefab;
    [SerializeField] private int _minDistantionFromPlayer = 20;
    [SerializeField] private int _maxSpawnAttempts = 40;

    public void CreateNextBot(Nextbot bot, Vector3 position)
    {
        GameObject nextbot = Instantiate(_nextbotPrefab, position, Quaternion.identity);

        nextbot.GetComponentInChildren<MeshRenderer>().material.mainTexture = bot.Texture;

        nextbot.GetComponentInChildren<LookAtConstraint>().AddSource(new ConstraintSource() { sourceTransform = GameObject.FindGameObjectWithTag("Player").transform, weight = 1f });
    }

    public void CreateNextbots(int count)
    {
        Nextbot[] bots = _bots;
        Nextbot[] customBots = CustomNextbotCreator.LoadAllCustomNextbots();
        bots = bots.Concat(customBots).ToArray();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Получаем пиксели изображения
        Color[] pixels = GameManager.CurrentMazeTexture.GetPixels();
        Color[,] map = new Color[GameManager.CurrentMazeTexture.width, GameManager.CurrentMazeTexture.height];

        // Проходим по пикселям и создаем объекты
        for (int y = 0; y < GameManager.CurrentMazeTexture.height; y++)
        {
            for (int x = 0; x < GameManager.CurrentMazeTexture.width; x++)
            {
                map[x, y] = pixels[y * GameManager.CurrentMazeTexture.width + x];
            }
        }

        for (int i = 0; i < count; i++)
        {
            int botIndex = Random.Range(0, bots.Length);
            int x = 0;
            int y = 0;

            for (int z = 0; z < _maxSpawnAttempts; z++)
            {
                x = Random.Range(0, map.GetLength(0));
                y = Random.Range(0, map.GetLength(1));

                if (map[x, y] != Color.black && Vector3.Distance(player.transform.position, new Vector3(x, 0, y)) >= _minDistantionFromPlayer)
                    break;
            }

            CreateNextBot(bots[botIndex], new Vector3(x, 0, y));
        }
    }
}
