using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Newmapmaker : MonoBehaviour
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

        wallsRoot = new GameObject("Walls Root");

        int texWidth = _mazeImage.width;
        int texHeight = _mazeImage.height;

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

        // Временные списки для вершин, треугольников и UV текущего сегмента.
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Счётчик клеток (блоков), добавленных в текущий сегмент.
        int blockCounter = 0;
        int segmentIndex = 0;

        // Перебираем все клетки лабиринта.
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                if (!isWall[x, y])
                    continue; // пропускаем, если клетка не стена

                // Вычисляем позицию клетки в мировых координатах.
                // Размещаем лабиринт на плоскости XZ, ось Y – вертикаль.
                float worldX = x * cellSize;
                float worldZ = y * cellSize;

                // Добавляем верхнюю грань («крышу») стены.
                // Для крыши нормаль должна смотреть вверх, поэтому порядок не инвертируем.
                Vector3 bl = new Vector3(worldX, wallHeight, worldZ);
                Vector3 br = new Vector3(worldX + cellSize, wallHeight, worldZ);
                Vector3 tr = new Vector3(worldX + cellSize, wallHeight, worldZ + cellSize);
                Vector3 tl = new Vector3(worldX, wallHeight, worldZ + cellSize);
                AddQuad(vertices, triangles, uvs, bl, br, tr, tl, false);

                // Проверяем соседей по 4 направлениям и добавляем вертикальные грани там, где отсутствует стена.
                // Для вертикальных граней инвертируем порядок, чтобы нормали смотрели наружу.
                // Север (сосед: (x, y+1))
                if (y + 1 >= texHeight || !isWall[x, y + 1])
                {
                    Vector3 v0 = new Vector3(worldX, 0, worldZ + cellSize);
                    Vector3 v1 = new Vector3(worldX + cellSize, 0, worldZ + cellSize);
                    Vector3 v2 = new Vector3(worldX + cellSize, wallHeight, worldZ + cellSize);
                    Vector3 v3 = new Vector3(worldX, wallHeight, worldZ + cellSize);
                    AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                }
                // Юг (сосед: (x, y-1))
                if (y - 1 < 0 || !isWall[x, y - 1])
                {
                    Vector3 v0 = new Vector3(worldX + cellSize, 0, worldZ);
                    Vector3 v1 = new Vector3(worldX, 0, worldZ);
                    Vector3 v2 = new Vector3(worldX, wallHeight, worldZ);
                    Vector3 v3 = new Vector3(worldX + cellSize, wallHeight, worldZ);
                    AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                }
                // Восток (сосед: (x+1, y))
                if (x + 1 >= texWidth || !isWall[x + 1, y])
                {
                    Vector3 v0 = new Vector3(worldX + cellSize, 0, worldZ + cellSize);
                    Vector3 v1 = new Vector3(worldX + cellSize, 0, worldZ);
                    Vector3 v2 = new Vector3(worldX + cellSize, wallHeight, worldZ);
                    Vector3 v3 = new Vector3(worldX + cellSize, wallHeight, worldZ + cellSize);
                    AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                }
                // Запад (сосед: (x-1, y))
                if (x - 1 < 0 || !isWall[x - 1, y])
                {
                    Vector3 v0 = new Vector3(worldX, 0, worldZ);
                    Vector3 v1 = new Vector3(worldX, 0, worldZ + cellSize);
                    Vector3 v2 = new Vector3(worldX, wallHeight, worldZ + cellSize);
                    Vector3 v3 = new Vector3(worldX, wallHeight, worldZ);
                    AddQuad(vertices, triangles, uvs, v0, v1, v2, v3, true);
                }

                blockCounter++;

                // Если достигли лимита клеток для сегмента, создаём меш для текущего сегмента
                if (blockCounter >= _countOfMeshesInCombinedMesh)
                {
                    CreateMazeMeshSegment(vertices, triangles, uvs, segmentIndex);
                    segmentIndex++;
                    blockCounter = 0;
                    vertices.Clear();
                    triangles.Clear();
                    uvs.Clear();
                }
            }
        }

        // Если остались данные для неполного сегмента, создаём для них меш
        if (vertices.Count > 0)
        {
            CreateMazeMeshSegment(vertices, triangles, uvs, segmentIndex);
        }
    }

    private void CreateMazeMeshSegment(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, int segmentIndex)
    {
        Mesh segmentMesh = new Mesh();
        segmentMesh.vertices = vertices.ToArray();
        segmentMesh.triangles = triangles.ToArray();
        segmentMesh.uv = uvs.ToArray();
        segmentMesh.RecalculateNormals();

        GameObject segmentGO = new GameObject($"Maze Mesh Segment {segmentIndex}");
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
            // Обычный порядок: нормаль направлена по умолчанию
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 3);
            triangles.Add(startIndex + 2);
        }
        else
        {
            // Инвертированный порядок: нормаль направлена в противоположную сторону
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
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        // Центрируем пол относительно лабиринта; если 1 клетка = 1 метр, то размеры равны _mazeImage.width и _mazeImage.height.
        floor.transform.position = new Vector3(_mazeImage.width * 0.5f, 0, _mazeImage.height * 0.5f);
        // Стандартный plane имеет размер 10, поэтому масштаб = размер лабиринта / 10.
        floor.transform.localScale = new Vector3(_mazeImage.width / 10f, 1, _mazeImage.height / 10f);
        _floorMaterial.mainTextureScale = new Vector2(_mazeImage.width, _mazeImage.height);
        floor.GetComponent<MeshRenderer>().material = _floorMaterial;
    }

    private void CreateCeiling()
    {
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ceiling.name = "Ceiling";
        ceiling.transform.eulerAngles = Vector3.left * 180;
        ceiling.transform.position = new Vector3(_mazeImage.width * 0.5f, wallHeight, _mazeImage.height * 0.5f);
        ceiling.transform.localScale = new Vector3(_mazeImage.width / 10f, 1, _mazeImage.height / 10f);
        _floorMaterial.mainTextureScale = new Vector2(_mazeImage.width, _mazeImage.height);
        ceiling.GetComponent<MeshRenderer>().material = _ceilingMaterial;

    }

    private void PlaceLamps()
    {
        // Получаем пиксели изображения
        Color[] pixels = _mazeImage.GetPixels();
        Color[,] map = new Color[_mazeImage.width, _mazeImage.height];

        // Заполняем двумерный массив цветов
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