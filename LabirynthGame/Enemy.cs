namespace LabyrinthGame;

public class Enemy
{
    public int X { get; set; }
    public int Y { get; set; }

    private int[] dx = { 1, -1, 0, 0 };
    private int[] dy = { 0, 0, 1, -1 };

    public Enemy(int startX, int startY)
    {
        X = startX;
        Y = startY;
    }

    public void Move(Player player, Labyrinth labyrinth)
    {
        List<Node> openList = new List<Node>();
        HashSet<string> closedList = new HashSet<string>();

        Node startNode = new Node(X, Y, 0, GetHeuristic(X, Y, player.X, player.Y), null);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.OrderBy(n => n.F).First();

            if (currentNode.X == player.X && currentNode.Y == player.Y)
            {
                TracePath(currentNode);
                return;
            }

            openList.Remove(currentNode);
            closedList.Add($"{currentNode.X},{currentNode.Y}");

            for (int i = 0; i < 4; i++)
            {
                int newX = currentNode.X + dx[i];
                int newY = currentNode.Y + dy[i];

                if (labyrinth.CanMove(newX, newY) && !closedList.Contains($"{newX},{newY}"))
                {
                    int g = currentNode.G + 1;
                    int h = GetHeuristic(newX, newY, player.X, player.Y);
                    Node neighbor = new Node(newX, newY, g, h, currentNode);

                    openList.Add(neighbor);
                }
            }
        }
    }

    private void TracePath(Node node)
    {
        while (node.Parent != null)
        {
            X = node.X;
            Y = node.Y;
            node = node.Parent;
        }
    }

    private int GetHeuristic(int x, int y, int targetX, int targetY)
    {
        return Math.Abs(x - targetX) + Math.Abs(y - targetY);
    }

    private class Node
    {
        public int X { get; }
        public int Y { get; }
        public int G { get; }
        public int H { get; }
        public int F => G + H;
        public Node Parent { get; }

        public Node(int x, int y, int g, int h, Node parent)
        {
            X = x;
            Y = y;
            G = g;
            H = h;
            Parent = parent;
        }
    }
}