namespace LabyrinthGame;

public class Player
{
    public int X { get; set; }
    public int Y { get; set; }

    public Player(int startX, int startY)
    {
        X = startX;
        Y = startY;
    }

    public void Move(string direction, Labyrinth labyrinth)
    {
        int newX = X, newY = Y;

        switch (direction)
        {
            case "up": newY--; break;
            case "down": newY++; break;
            case "left": newX--; break;
            case "right": newX++; break;
        }

        if (labyrinth.CanMove(newX, newY))
        {
            X = newX;
            Y = newY;
        }
    }
}