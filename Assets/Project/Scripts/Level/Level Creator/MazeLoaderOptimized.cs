using System.Collections.Generic;
using UnityEngine;

public class MazeLoaderOptimized : MonoBehaviour
{
    [Header("Materials Set-Up")]
    [SerializeField] private Material _floorMaterial;
    [SerializeField] private Material _ceilingMaterial;
    [SerializeField] private Material _wallMaterial;
    [Header("Walls Set-Up")]
    [SerializeField] private float wallHeight = 1f;
    [SerializeField] private int _countOfMeshesInCombinedMesh = 100;
    [Header("Lamp Set-Up")]
    [SerializeField] private GameObject _lampPrefab;
    [SerializeField] private int _lampOffset = 10;

    private GameObject wallsRoot;
    private Texture2D _mazeImage;
    private LevelManager _manager;
    private bool _isInitialized = false;

    public delegate void OnInitializationComplete();
    public OnInitializationComplete OnInitializationCompleteEvent;

    public delegate void OnMazeCreationStart();
    public OnMazeCreationStart OnMazeCreationStartEvent;

    public delegate void OnMazeCreationEnd();
    public OnMazeCreationEnd OnMazeCreationEndEvent;

    public Texture2D MazeImage => _mazeImage;

    private void Start() => Initialize();

    public void Initialize()
    {
        _manager = FindObjectOfType<LevelManager>();

        if (_manager == null)
        {
            Debug.LogError($"Can{"'"}t find Level Manager on the map!");
            return;
        }

        _isInitialized = true;

        _mazeImage = _manager.MazeImage;
    }

    private void GenerateMaze()
    {
        if (_mazeImage == null)
        {
            Debug.LogError("Maze Image was not assigned!");
            return;
        }

        // Создаём корневой объект для стен
        wallsRoot = new GameObject("Walls Root");

        int texWidth = _mazeImage.width;
        int texHeight = _mazeImage.height;

        // Считываем данные о стенах: true – клетка является стеной
        bool[,] isWall = new bool[texWidth, texHeight];
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                Color pixelColor = _mazeImage.GetPixel(x, y);
                isWall[x, y] = (pixelColor.grayscale <= 0.7f);
            }
        }

        float cellSize = 1f;
        // Определяем размер чанка (количество клеток по одной стороне)
        int chunkSide = Mathf.RoundToInt(Mathf.Sqrt(_countOfMeshesInCombinedMesh));
        // Если значение меньше 1, задаём 1
        chunkSide = Mathf.Max(1, chunkSide);

        // Разбиваем лабиринт на квадратные чанки
        for (int chunkY = 0; chunkY < texHeight; chunkY += chunkSide)
        {
            for (int chunkX = 0; chunkX < texWidth; chunkX += chunkSide)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();
                List<Vector2> uvs = new List<Vector2>();

                // Проходим по клеткам в пределах текущего чанка
                for (int y = chunkY; y < Mathf.Min(chunkY + chunkSide, texHeight); y++)
                {
                    for (int x = chunkX; x < Mathf.Min(chunkX + chunkSide, texWidth); x++)
                    {
                        if (!isWall[x, y])
                            continue; // пропускаем, если клетка не стена

                        // Вычисляем позицию клетки в мировых координатах с учётом смещения
                        float worldX = x * cellSize;
                        float worldZ = y * cellSize;
                        Vector3 cellOrigin = new Vector3(worldX, 0, worldZ);

                        // Верхняя грань (крыша) стены – нормаль смотрит вверх (не инвертируем порядок)
                        Vector3 bl = cellOrigin + new Vector3(0, wallHeight, 0);
                        Vector3 br = cellOrigin + new Vector3(cellSize, wallHeight, 0);
                        Vector3 tr = cellOrigin + new Vector3(cellSize, wallHeight, cellSize);
                        Vector3 tl = cellOrigin + new Vector3(0, wallHeight, cellSize);
                        AddQuad(vertices, triangles, uvs, bl, br, tr, tl, false);

                        // Вертикальные грани – если соседней клетки со стеной нет, добавляем грань с инвертированным порядком

                        // Север (y+1)
                        if (y + 1 >= texHeight || !isWall[x, y + 1])
                        {
                            Vector3 v0 = cellOrigin + new Vector3(0, 0, cellSize);
                            Vector3 v1 = cellOrigin + new Vector3(cellSize, 0, cellSize);
                            Vector3 v2 = cellOrigin + new Vector3(cellSize, wallHeight, cellSize);
                            Vector3 v3 = cellOrigin + new Vector3(0, wallHeight, cellSize);
                            AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                        }
                        // Юг (y-1)
                        if (y - 1 < 0 || !isWall[x, y - 1])
                        {
                            Vector3 v0 = cellOrigin + new Vector3(cellSize, 0, 0);
                            Vector3 v1 = cellOrigin + new Vector3(0, 0, 0);
                            Vector3 v2 = cellOrigin + new Vector3(0, wallHeight, 0);
                            Vector3 v3 = cellOrigin + new Vector3(cellSize, wallHeight, 0);
                            AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                        }
                        // Восток (x+1)
                        if (x + 1 >= texWidth || !isWall[x + 1, y])
                        {
                            Vector3 v0 = cellOrigin + new Vector3(cellSize, 0, cellSize);
                            Vector3 v1 = cellOrigin + new Vector3(cellSize, 0, 0);
                            Vector3 v2 = cellOrigin + new Vector3(cellSize, wallHeight, 0);
                            Vector3 v3 = cellOrigin + new Vector3(cellSize, wallHeight, cellSize);
                            AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                        }
                        // Запад (x-1)
                        if (x - 1 < 0 || !isWall[x - 1, y])
                        {
                            Vector3 v0 = cellOrigin + new Vector3(0, 0, 0);
                            Vector3 v1 = cellOrigin + new Vector3(0, 0, cellSize);
                            Vector3 v2 = cellOrigin + new Vector3(0, wallHeight, cellSize);
                            Vector3 v3 = cellOrigin + new Vector3(0, wallHeight, 0);
                            AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                        }
                    }
                }

                // Если в этом чанке есть данные, создаём комбинированный меш
                if (vertices.Count > 0)
                {
                    CreateMazeMeshSegment(vertices, triangles, uvs, $"Chunk_{chunkX}_{chunkY}");
                }
            }
        }
    }

    private void CreateMazeMeshSegment(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, string segmentName)
    {
        Mesh segmentMesh = new Mesh();
        segmentMesh.vertices = vertices.ToArray();
        segmentMesh.triangles = triangles.ToArray();
        segmentMesh.uv = uvs.ToArray();
        segmentMesh.RecalculateNormals();

        // Приводим bounding box к квадрату для удобства выбора:
        Bounds b = segmentMesh.bounds;
        float maxSize = Mathf.Max(b.size.x, b.size.z);
        Vector3 center = b.center;
        segmentMesh.bounds = new Bounds(center, new Vector3(maxSize, b.size.y, maxSize));

        GameObject segmentGO = new GameObject($"Maze Mesh Segment {segmentName}");
        segmentGO.transform.parent = wallsRoot.transform;
        MeshFilter mf = segmentGO.AddComponent<MeshFilter>();
        mf.mesh = segmentMesh;
        MeshRenderer mr = segmentGO.AddComponent<MeshRenderer>();
        mr.material = _wallMaterial;
        // Добавляем коллайдер для стен
        MeshCollider mc = segmentGO.AddComponent<MeshCollider>();
        mc.sharedMesh = segmentMesh;
        segmentGO.isStatic = true;
    }

    private void AddQuad(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs,
                           Vector3 bl, Vector3 br, Vector3 tr, Vector3 tl, bool invert)
    {
        int startIndex = vertices.Count;
        vertices.Add(bl);
        vertices.Add(br);
        vertices.Add(tr);
        vertices.Add(tl);

        if (!invert)
        {
            // Обычный порядок: нормаль по умолчанию
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 3);
            triangles.Add(startIndex + 2);
        }
        else
        {
            // Инвертированный порядок: нормаль в противоположную сторону
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        // Простейшее UV-развертывание
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));
    }

    private void CreateFloor()
    {
        GameObject root = new GameObject("Floor Root");

        int iterationsX = _mazeImage.width / 10 + 1;
        int iterationsY = _mazeImage.height / 10 + 1;

        for (int y = 0; y < iterationsY; y++)
        {
            for (int x = 0; x < iterationsX; x++)
            {
                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.transform.SetPositionAndRotation(new Vector3(x * 10, 0, y * 10), Quaternion.Euler(0, 0, 0));
                floor.GetComponent<MeshRenderer>().material = _floorMaterial;
                floor.transform.parent = root.transform;
            }
        }

        root.isStatic = true;
    }

    private void CreateCeiling()
    {
        GameObject root = new GameObject("Ceiling Root");

        int iterationsX = _mazeImage.width / 10 + 1;
        int iterationsY = _mazeImage.height / 10 + 1;

        for (int y = 0; y < iterationsY; y++)
        {
            for (int x = 0; x < iterationsX; x++)
            {
                GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ceiling.transform.SetPositionAndRotation(new Vector3(x * 10, wallHeight, y * 10), Quaternion.Euler(180, 0, 0));
                ceiling.GetComponent<MeshRenderer>().material = _ceilingMaterial;
                ceiling.transform.parent = root.transform;
            }
        }

        root.isStatic = true;
    }

    private void PlaceLamps()
    {
        GameObject root = new GameObject("Lamps Root");

        int width = _mazeImage.width;
        int height = _mazeImage.height;

        for (int y = 0; y < height; y++)
        {
            if (y % _lampOffset != 0)
                continue;

            for (int x = 0; x < width; x++)
            {
                if (x % _lampOffset != 0)
                    continue;

                Color pixel = _mazeImage.GetPixel(x, y);
                if (pixel.grayscale <= 0.7f)
                    continue;

                // Центр клетки
                Vector3 lampPos = new Vector3(x + 0.5f, wallHeight, y + 0.5f);
                GameObject lamp = Instantiate(_lampPrefab, lampPos, Quaternion.identity);
                lamp.transform.parent = root.transform;
            }
        }

        root.isStatic = true;
    }

    public void GenerateMap()
    {
        if (!_isInitialized)
            Initialize();

        if (!_isInitialized)
            return;

        GenerateMaze();
        CreateFloor();
        CreateCeiling();
        PlaceLamps();
    }
}