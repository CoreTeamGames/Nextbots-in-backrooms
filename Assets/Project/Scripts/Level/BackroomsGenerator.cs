using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using UnityEngine;
using System.IO;
using System;

using Graphics = System.Drawing.Graphics;
using Color = System.Drawing.Color;
using Random = System.Random;
using System.Threading.Tasks;

public class BackroomsGenerator : MonoBehaviour
{
    [SerializeField] private string _fileName = "backrooms";
    private List<(int x, int y)> spawnPoints = new List<(int x, int y)>();
    private int _wallThickness = 1;
    private int _cellSize = 1;
    private Random random = new Random();
    private Cell[,] maze;
    private int numMazes, mazeSize, roomsCount, NUM_COLS, NUM_ROWS;
    private float stopCollisionProbality;

    public delegate void OnMapStartsCreates();
    public OnMapStartsCreates OnMapStartsCreatesEvent;

    public delegate void OnMapCreated();
    public OnMapCreated OnMapCreatedEvent;

    public delegate void OnMapCreateStateChanged(string stateName);
    public  OnMapCreateStateChanged OnMapCreateStateChangedEvent;

    public string FileName => _fileName;

    private struct Cell
    {
        public int X;
        public int Y;
        public int Width;
        public bool Visited;

        public Cell(int x, int y, int width, bool visited)
        {
            X = x;
            Y = y;
            Width = width;
            Visited = visited;
        }
    }

    private void CreateBorderWalls()
    {
        int wallThickness = 3; // Толщина стен

        // Верхняя и нижняя стены
        for (int y = 0; y < wallThickness; y++)
        {
            for (int x = 0; x < NUM_COLS; x++)
            {
                // Верхняя стена
                maze[y, x].Visited = false;
                // Нижняя стена
                maze[NUM_ROWS - 1 - y, x].Visited = false;
            }
        }

        // Левая и правая стены
        for (int x = 0; x < wallThickness; x++)
        {
            for (int y = 0; y < NUM_ROWS; y++)
            {
                // Левая стена
                maze[y, x].Visited = false;
                // Правая стена
                maze[y, NUM_COLS - 1 - x].Visited = false;
            }
        }
    }

    private async void GenerateMaze()
    {
        // Инициализация лабиринта стенами
        for (int y = 0; y < NUM_ROWS; y++)
        {
            for (int x = 0; x < NUM_COLS; x++)
            {
                maze[y, x] = new Cell(x * _cellSize, y * _cellSize, _cellSize, false);
            }
        }

        await Task.Run(CreateBorderWalls);

        HashSet<(int x, int y)> visitedCells = new HashSet<(int x, int y)>();
        int pathWidth = 3; // Ширина прохода

        for (int mazeCount = 0; mazeCount < numMazes; mazeCount++)
        {
            // Выбираем случайную стартовую точку
            int startX = random.Next(pathWidth, NUM_COLS - pathWidth);
            int startY = random.Next(pathWidth, NUM_ROWS - pathWidth);

            // Убедимся, что стартовая точка выровнена по сетке
            startX = startX - (startX % pathWidth);
            startY = startY - (startY % pathWidth);

            visitedCells.Add((startX, startY));
            List<(int x, int y)> frontier = new List<(int x, int y)> { (startX, startY) };

            // Создаем начальную комнату
            CreateWidePassage(startX, startY, pathWidth);

            while (frontier.Count > 0)
            {
                int index = random.Next(frontier.Count);
                var current = frontier[index];
                frontier.RemoveAt(index);

                var neighbors = GetUnvisitedNeighbors(current.x, current.y, visitedCells, pathWidth);
                foreach (var neighbor in neighbors)
                {
                    if (random.NextDouble() > stopCollisionProbality)
                    {
                        // Создаем проход до соседа
                        CreateWidePassage(neighbor.x, neighbor.y, pathWidth);

                        // Создаем проход между текущей точкой и соседом
                        int midX = (current.x + neighbor.x) / 2;
                        int midY = (current.y + neighbor.y) / 2;
                        CreateWidePassage(midX, midY, pathWidth);

                        visitedCells.Add(neighbor);
                        frontier.Add(neighbor);
                    }
                }
            }
        }
    }

    private void CreateWidePassage(int centerX, int centerY, int width)
    {
        int halfWidth = width / 2;
        for (int dy = -halfWidth; dy <= halfWidth; dy++)
        {
            for (int dx = -halfWidth; dx <= halfWidth; dx++)
            {
                int newX = centerX + dx;
                int newY = centerY + dy;
                if (newX >= 0 && newX < NUM_COLS && newY >= 0 && newY < NUM_ROWS)
                {
                    maze[newY, newX].Visited = true;
                }
            }
        }
    }

    private List<(int x, int y)> GetUnvisitedNeighbors(int x, int y, HashSet<(int x, int y)> visited, int spacing)
    {
        var neighbors = new List<(int x, int y)>();
        int[][] directions = new int[][]
        {
        new int[] {-spacing * 2, 0},
        new int[] {spacing * 2, 0},
        new int[] {0, -spacing * 2},
        new int[] {0, spacing * 2}
        };

        foreach (var dir in directions)
        {
            int newX = x + dir[0];
            int newY = y + dir[1];

            // Проверяем границы с учетом толщины стен
            if (newX >= _wallThickness + spacing &&
                newX < NUM_COLS - _wallThickness - spacing &&
                newY >= _wallThickness + spacing &&
                newY < NUM_ROWS - _wallThickness - spacing &&
                !visited.Contains((newX, newY)))
            {
                // Проверяем, не пересекается ли новый проход с существующими
                bool canAdd = true;
                for (int checkY = newY - spacing; checkY <= newY + spacing; checkY++)
                {
                    for (int checkX = newX - spacing; checkX <= newX + spacing; checkX++)
                    {
                        if (checkX >= 0 && checkX < NUM_COLS &&
                            checkY >= 0 && checkY < NUM_ROWS &&
                            maze[checkY, checkX].Visited)
                        {
                            canAdd = false;
                            break;
                        }
                    }
                    if (!canAdd) break;
                }

                if (canAdd)
                {
                    neighbors.Add((newX, newY));
                }
            }
        }

        return neighbors;
    }

    private void GenerateRooms()
    {
        List<(int x, int y, int width, int height)> rooms = new List<(int x, int y, int width, int height)>();

        for (int i = 0; i < roomsCount; i++)
        {
            int attempts = 0;
            bool roomPlaced = false;

            while (attempts < 50 && !roomPlaced) // Ограничиваем количество попыток
            {
                int roomWidth = random.Next(15, 25);  // Большие комнаты
                int roomHeight = random.Next(15, 25);
                int x = random.Next(2, NUM_COLS - roomWidth - 2);
                int y = random.Next(2, NUM_ROWS - roomHeight - 2);

                // Проверяем перекрытие с существующими комнатами
                bool overlaps = false;
                foreach (var room in rooms)
                {
                    if (x + roomWidth + 2 > room.x && x - 2 < room.x + room.width &&
                        y + roomHeight + 2 > room.y && y - 2 < room.y + room.height)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    rooms.Add((x, y, roomWidth, roomHeight));

                    // Создаем комнату
                    for (int ry = y; ry < y + roomHeight; ry++)
                    {
                        for (int rx = x; rx < x + roomWidth; rx++)
                        {
                            maze[ry, rx].Visited = true;
                        }
                    }

                    // Добавляем колонны в комнату
                    for (int py = y + 4; py < y + roomHeight - 4; py += 6)
                    {
                        for (int px = x + 4; px < x + roomWidth - 4; px += 6)
                        {
                            // Создаем колонну 2x2
                            for (int cy = 0; cy < 2; cy++)
                            {
                                for (int cx = 0; cx < 2; cx++)
                                {
                                    if (py + cy < NUM_ROWS && px + cx < NUM_COLS)
                                    {
                                        maze[py + cy, px + cx].Visited = false;
                                    }
                                }
                            }
                        }
                    }

                    roomPlaced = true;
                }

                attempts++;
            }
        }
    }

    public void SaveToPng()
    {
        using (Bitmap bmp = new Bitmap(mazeSize, mazeSize))
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(0, 0, 0));

                // Рисуем лабиринт
                for (int y = 0; y < NUM_ROWS; y++)
                {
                    for (int x = 0; x < NUM_COLS; x++)
                    {
                        if (maze[y, x].Visited)
                        {
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
                            {
                                g.FillRectangle(brush,
                                    maze[y, x].X,
                                    maze[y, x].Y,
                                    maze[y, x].Width,
                                    maze[y, x].Width);
                            }
                        }
                    }
                }
            }
            bmp.Save(Application.persistentDataPath + "/" + _fileName + ".png", ImageFormat.Png);
        }
    }

    // Измените метод Generate
    public async Task Generate(int numMazes, float stopCollisionProbality, int mazeSize, int rooms)
    {
        this.numMazes = numMazes;
        this.stopCollisionProbality = stopCollisionProbality;
        this.mazeSize = mazeSize;
        this.roomsCount = rooms;
        await GenerateBackroomsMaze();
    }

    /// <summary>
    /// This method returns a Backrooms maze map image
    /// </summary>
    /// <param name="filename">The name of file without extension</param>
    /// <returns></returns>
    public Texture2D LoadMaze(string filename)
    {
        if (!File.Exists(Application.persistentDataPath +"/" +filename + ".png"))
            return null;

        Texture2D _mazeTexture = new Texture2D(1, 1);
        _mazeTexture.filterMode = FilterMode.Point;
        _mazeTexture.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/" + filename + ".png"));

        return _mazeTexture;
    }

    async Task GenerateBackroomsMaze()
    {  
        NUM_COLS = mazeSize / _cellSize;
        NUM_ROWS = mazeSize / _cellSize;
        maze = new Cell[NUM_ROWS, NUM_COLS];

        OnMapStartsCreatesEvent?.Invoke();

        await Task.Run(GenerateMaze);
        await Task.Run(GenerateRooms);
        SaveToPng();
        OnMapCreatedEvent?.Invoke();
    }
}