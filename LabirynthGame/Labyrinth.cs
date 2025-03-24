public class Labyrinth
{
    public int Width { get; }
    public int Height { get; }
    private int[,] maze;
    private Random rand = new Random();

    public int StartX { get; private set; } = 1;
    public int StartY { get; private set; } = 1;

    public int GoalX { get; private set; }
    public int GoalY { get; private set; }

    public Labyrinth(int width, int height)
    {
        Width = width;
        Height = height;
        maze = new int[Width, Height];

        GenerateLabyrinth();
        SetGoal();
    }

    private void GenerateLabyrinth()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                maze[x, y] = 1;
            }
        }

        CreatePath(1, 1);
    }

    private void CreatePath(int x, int y)
    {
        maze[x, y] = 0;

        var directions = new (int dx, int dy)[]
        {
            (0, -2), (2, 0), (0, 2), (-2, 0)
        };

        directions = directions.OrderBy(d => rand.Next()).ToArray();

        foreach (var direction in directions)
        {
            int newX = x + direction.dx;
            int newY = y + direction.dy;

            if (newX > 0 && newX < Width - 1 && newY > 0 && newY < Height - 1 && maze[newX, newY] == 1)
            {
                maze[newX, newY] = 0;
                maze[x + direction.dx / 2, y + direction.dy / 2] = 0;
                CreatePath(newX, newY);
            }
        }
    }

    private void SetGoal()
    {
        GoalX = Width - 2;
        GoalY = Height - 2;

        while (maze[GoalX, GoalY] == 1)
        {
            GoalX--;
            GoalY--;
        }
    }

    public bool IsWall(int x, int y) => maze[x, y] == 1;
    public bool CanMove(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height && maze[x, y] == 0;
}