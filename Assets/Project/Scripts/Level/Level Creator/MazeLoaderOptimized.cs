using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.AI;
using System.Threading.Tasks;
using UnityEngine.Events;

public class MazeLoaderOptimized : MonoBehaviour
{
    [SerializeField] private Texture2D _mazeImage;
    [SerializeField] private Material _floorMaterial;
    [SerializeField] private Material _ceilingMaterial;
    [SerializeField] private Material _wallMaterial;
    [SerializeField] private GameObject wallsRoot;
    [SerializeField] private GameObject spawnPointsRoot;
    [SerializeField] private GameObject _lampPrefab;
    [SerializeField] private float wallHeight = 1f;
    [SerializeField] private string filename = "backrooms.png";
    [SerializeField] private int _countOfMeshesInCombinedMesh = 100;
    [SerializeField] private PlayerSpawner _spawner;
    [SerializeField] private int _lampOffset = 10;
    [SerializeField] private NavMeshSurface _surface;
    [SerializeField] private UnityEvent _onMazeCreated;

    public Texture2D MazeImage => _mazeImage;

    private void Start()
    {
        LoadImage(Application.persistentDataPath + "/" + filename);
        wallsRoot = new GameObject("Walls Root");
        spawnPointsRoot = new GameObject("Spawn Points Root");
        GenerateMaze();
    }

    private async void GenerateMaze()
    {
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

        _wallMaterial.mainTextureScale = new Vector2(1, wallHeight);

        for (int mapY = 0; mapY < map.GetLength(1); mapY++)
        {
            for (int mapX = 0; mapX < map.GetLength(0); mapX++)
            {
                if (map[mapX, mapY] == Color.green)
                {
                    CreatePlayerStart(mapX, mapY);
                    continue;
                }
                else if (map[mapX, mapY] != Color.black)
                    continue;

                bool generateN = false;
                bool generateW = false;
                bool generateE = false;
                bool generateS = false;

                if (mapX + 1 < map.GetLength(0))
                {
                    if (map[mapX + 1, mapY] != Color.black)
                        generateE = true;
                }
                if (mapX - 1 >= 0)
                {
                    if (map[mapX - 1, mapY] != Color.black)
                        generateW = true;
                }
                if (mapY + 1 < map.GetLength(1))
                {
                    if (map[mapX, mapY + 1] != Color.black)
                        generateN = true;
                }
                if (mapY - 1 >= 0)
                {
                    if (map[mapX, mapY - 1] != Color.black)
                        generateS = true;
                }

                if (generateN || generateE || generateW || generateS)
                    await CreateWall(mapX, mapY, generateN, generateE, generateW, generateS);
            }
        }

        CreateFloor();
        CreateCeiling();
        PlaceLamps();

        await CombineWalls();

        _spawner.Initialize();
        _spawner.SpawnPlayer();
        _surface.BuildNavMesh();
        _onMazeCreated?.Invoke();
    }

    private void CreateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.position = new Vector3(_mazeImage.width / 2, 0, _mazeImage.height / 2);
        floor.transform.localScale = new Vector3(0.1f * _mazeImage.width, 1, 0.1f * _mazeImage.height);
        _floorMaterial.mainTextureScale = new Vector2(_mazeImage.width + 1f, _mazeImage.height + 1f);
        floor.GetComponent<MeshRenderer>().material = _floorMaterial;
    }

    private void CreateCeiling()
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ceiling.name = "Ceiling";
        ceiling.transform.position = new Vector3(_mazeImage.width / 2, wallHeight, _mazeImage.height / 2);
        ceiling.transform.eulerAngles = Vector3.left * 180;
        ceiling.transform.localScale = new Vector3(0.1f * _mazeImage.width, 1, 0.1f * _mazeImage.height);
        _ceilingMaterial.mainTextureScale = new Vector2(_mazeImage.width + 1f, _mazeImage.height + 1f);
        ceiling.GetComponent<MeshRenderer>().material = _ceilingMaterial;
    }

    private async Task CreateWall(int x, int y, bool generateN, bool generateE, bool generateW, bool generateS)
    {
        Vector3 position = new Vector3(x, wallHeight / 2, y);
        GameObject wallRoot = new GameObject($"Wall {x} {y}");
        wallRoot.transform.position = position;

        if (generateN)
        {
            GameObject wallN = GameObject.CreatePrimitive(PrimitiveType.Plane);
            wallN.transform.localScale = new Vector3(0.1f, 0.1f, wallHeight * 0.1f);
            wallN.transform.eulerAngles = new Vector3(90, 90, 90);
            wallN.transform.parent = wallRoot.transform;
            wallN.transform.localPosition = new Vector3(0, 0, 0.5f);
            wallN.GetComponent<MeshRenderer>().material = _wallMaterial;
            wallN.name = "N";
        }

        if (generateE)
        {
            GameObject wallE = GameObject.CreatePrimitive(PrimitiveType.Plane);
            wallE.transform.localScale = new Vector3(0.1f, 0.1f, wallHeight * 0.1f);
            wallE.transform.eulerAngles = new Vector3(90, 180, 90);
            wallE.transform.parent = wallRoot.transform;
            wallE.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallE.GetComponent<MeshRenderer>().material = _wallMaterial;
            wallE.name = "E";
        }

        if (generateW)
        {
            GameObject wallW = GameObject.CreatePrimitive(PrimitiveType.Plane);
            wallW.transform.localScale = new Vector3(0.1f, 0.1f, wallHeight * 0.1f);
            wallW.transform.eulerAngles = new Vector3(90, 0, 90);
            wallW.transform.parent = wallRoot.transform;
            wallW.transform.localPosition = new Vector3(-0.5f, 0, 0);
            wallW.GetComponent<MeshRenderer>().material = _wallMaterial;
            wallW.name = "W";
        }

        if (generateS)
        {
            GameObject wallS = GameObject.CreatePrimitive(PrimitiveType.Plane);
            wallS.transform.localScale = new Vector3(0.1f, 0.1f, wallHeight * 0.1f);
            wallS.transform.eulerAngles = new Vector3(90, -90, 90);
            wallS.transform.parent = wallRoot.transform;
            wallS.transform.localPosition = new Vector3(0, 0, -0.5f);
            wallS.GetComponent<MeshRenderer>().material = _wallMaterial;
            wallS.name = "S";
        }

        wallRoot.transform.parent = wallsRoot.transform;
    }

    private void CreatePlayerStart(int x, int y)
    {
        Vector3 position = new Vector3(x, 0.5f, y);
        GameObject spawnPoint = new GameObject();
        spawnPoint.transform.position = position;
        spawnPoint.name = $"SpawnPoint {x} {y}";
        spawnPoint.tag = "SpawnPoint";
        spawnPoint.transform.parent = spawnPointsRoot.transform;
    }

    private void LoadImage(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(1, 1);
        if (texture.LoadImage(fileData))  // Загружаем изображение в текстуру
        {
            _mazeImage = texture;
        }
        else
        {
            Debug.LogError("Не удалось загрузить изображение.");
            return;
        }
    }

    private async Task CombineWalls()
    {
        // Получаем все MeshFilter в дочерних объектах wallsRoot
        MeshFilter[] meshFilters = wallsRoot.GetComponentsInChildren<MeshFilter>();

        // Если мешей нет, выходим из метода
        if (meshFilters.Length == 0)
        {
            Debug.LogWarning("Нет стен для объединения.");
            return;
        }

        // Вычисляем количество групп для объединения
        int groupCount = Mathf.CeilToInt((float)meshFilters.Length / _countOfMeshesInCombinedMesh);

        // Проходим по каждой группе
        for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
        {
            // Вычисляем количество мешей в текущей группе
            int meshesInGroup = Mathf.Min(_countOfMeshesInCombinedMesh, meshFilters.Length - groupIndex * _countOfMeshesInCombinedMesh);

            // Создаем массив CombineInstance для текущей группы
            CombineInstance[] combineInstances = new CombineInstance[meshesInGroup];

            // Заполняем массив CombineInstance
            for (int i = 0; i < meshesInGroup; i++)
            {
                int meshIndex = groupIndex * _countOfMeshesInCombinedMesh + i;
                combineInstances[i].mesh = meshFilters[meshIndex].sharedMesh;
                combineInstances[i].transform = meshFilters[meshIndex].transform.localToWorldMatrix;

                // Деактивируем объект стены
                meshFilters[meshIndex].gameObject.SetActive(false);
            }

            // Создаем новый меш для группы
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances);

            // Создаем новый объект для объединенного меша
            GameObject combinedObject = new GameObject($"Combined Walls {groupIndex}");
            combinedObject.transform.parent = wallsRoot.transform;

            // Добавляем компоненты MeshFilter и MeshRenderer
            MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = combinedMesh;

            MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();
            meshRenderer.material = _wallMaterial;

            // Добавляем MeshCollider (опционально)
            combinedObject.AddComponent<MeshCollider>().sharedMesh = combinedMesh;
            combinedObject.isStatic = true;
        }

        // Удаляем все деактивированные объекты стен
        foreach (var meshFilter in meshFilters)
        {
            if (!meshFilter.gameObject.activeSelf)
            {
                Destroy(meshFilter.gameObject.transform.parent.gameObject);
            }
        }
    }

    private void PlaceLamps()
    {
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

        for (int y = 0; y < _mazeImage.height; y++)
        {
            if (y % _lampOffset != 0)
                continue;

            for (int x = 0; x < _mazeImage.width; x++)
            {
                if (x % _lampOffset != 0)
                    continue;

                if (map[x, y] == Color.black)
                    continue;

                GameObject lamp = Instantiate(_lampPrefab, new Vector3(x, wallHeight, y), Quaternion.identity);

                lamp.isStatic = true;
            }
        }


    }
}