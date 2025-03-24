using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LabyrinthGame;

public partial class MainWindow : Window
{
    private Labyrinth labyrinth;
    private Player player;
    private Enemy enemy;
    private int cellSize;
    private int playerMoves;
    private DispatcherTimer enemyTimer;
    private double enemyMoveInterval;
    private int score;

    public MainWindow()
    {
        InitializeComponent();
        labyrinth = new Labyrinth(30, 30);
        ResetGame(); // Initialize the game with default settings
        MapSizeComboBox.IsEnabled = true;
    }

    // Starts the enemy movement timer
    private void StartEnemyTimer()
    {
        enemyTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(enemyMoveInterval)
        };
        enemyTimer.Tick += EnemyMove;
        enemyTimer.Start();
    }

    // Moves the enemy towards the player
    private void EnemyMove(object sender, EventArgs e)
    {
        if (enemy != null)
        {
            enemy.Move(player, labyrinth);  // Enemy chases the player
            DrawMaze();  // Redraw the maze after each enemy move
            CheckIfCaught();  // Check if the enemy caught the player
        }
    }

    // Handle key presses for player movement
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Check if no difficulty has been selected
        if (DifficultyComboBox.SelectedIndex == -1)
        {
            // Show a message box to notify the player to select a difficulty
            MessageBox.Show("Please select a difficulty level before starting!", "No Difficulty Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;  // Stop further execution if no difficulty is selected
        }

        switch (e.Key)
        {
            case Key.Up:
            case Key.W:
                player.Move("up", labyrinth);
                break;
            case Key.Down:
            case Key.S:
                player.Move("down", labyrinth);
                break;
            case Key.Left:
            case Key.A:
                player.Move("left", labyrinth);
                break;
            case Key.Right:
            case Key.D:
                player.Move("right", labyrinth);
                break;
        }

        playerMoves++;  // Count the player's moves

        // After 4 player moves, the enemy appears
        if (playerMoves == 4 && enemy == null)
        {
            enemy = new Enemy(labyrinth.StartX, labyrinth.StartY);
            StartEnemyTimer();  // Start the enemy movement timer
        }

        DrawMaze();  // Redraw the maze after each player move
        CheckIfCaught();  // Check if the enemy caught the player
        CheckIfWon();  // Check if the player has reached the goal
    }

    // Checks if the enemy caught the player
    private void CheckIfCaught()
    {
        if (enemy != null && enemy.X == player.X && enemy.Y == player.Y)
        {
            score = 0;
            ScoreTextBlock.Text = $"Score: {score}";
            ResetGame();
            MessageBox.Show("The enemy caught you! Game Over!");
        }
    }

    private void CheckIfWon()
    {
        if (player.X == labyrinth.GoalX && player.Y == labyrinth.GoalY)
        {
            score++;
            ScoreTextBlock.Text = $"Score: {score}";
            ResetGame();
            MessageBox.Show("Congratulations! You reached the score.");
        }
    }

    // Resets the game by generating a new labyrinth and placing the player at the start
    private void ResetGame()
    {
        if (MapSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string size = selectedItem.Content.ToString();

            int width, height;
            switch (size)
            {
                case "Small":
                    width = height = 20;
                    break;
                case "Medium":
                    width = height = 30;
                    break;
                case "Large":
                    width = height = 50;
                    break;
                default:
                    width = height = 30;
                    break;
            }

            labyrinth = new Labyrinth(width, height);
        }

        player = new Player(labyrinth.StartX, labyrinth.StartY);
        enemy = null;
        playerMoves = 0;

        if (enemyTimer != null)
        {
            enemyTimer.Stop();
        }

        DrawMaze();
    }

    private void DrawMaze()
    {
        if (mazeCanvas == null || mazeCanvas.ActualWidth == 0 || mazeCanvas.ActualHeight == 0)
            return;

        mazeCanvas.Children.Clear();
        cellSize = (int)Math.Min(mazeCanvas.ActualWidth / labyrinth.Width, mazeCanvas.ActualHeight / labyrinth.Height);

        // Draw the labyrinth
        for (int y = 0; y < labyrinth.Height; y++)
        {
            for (int x = 0; x < labyrinth.Width; x++)
            {
                var rect = new Rectangle
                {
                    Width = cellSize,
                    Height = cellSize,
                    Fill = labyrinth.IsWall(x, y) ? Brushes.Black : Brushes.White
                };
                Canvas.SetLeft(rect, x * cellSize);
                Canvas.SetTop(rect, y * cellSize);
                mazeCanvas.Children.Add(rect);
            }
        }

        // Draw the player
        var playerRect = new Rectangle { Width = cellSize, Height = cellSize, Fill = Brushes.Blue };
        Canvas.SetLeft(playerRect, player.X * cellSize);
        Canvas.SetTop(playerRect, player.Y * cellSize);
        mazeCanvas.Children.Add(playerRect);

        // Draw the enemy
        if (enemy != null)
        {
            var enemyRect = new Rectangle { Width = cellSize, Height = cellSize, Fill = Brushes.Red };
            Canvas.SetLeft(enemyRect, enemy.X * cellSize);
            Canvas.SetTop(enemyRect, enemy.Y * cellSize);
            mazeCanvas.Children.Add(enemyRect);
        }

        // Draw the goal (green goal)
        var goalRect = new Rectangle { Width = cellSize, Height = cellSize, Fill = Brushes.Green };
        Canvas.SetLeft(goalRect, labyrinth.GoalX * cellSize);
        Canvas.SetTop(goalRect, labyrinth.GoalY * cellSize);
        mazeCanvas.Children.Add(goalRect);
    }

    // Adjust the maze when the window is resized
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawMaze();
    }

    // Reset button handler
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        ResetGame();
        score = 0;
        ScoreTextBlock.Text = $"Score: {score}";
        DifficultyComboBox.IsEnabled = true;
        MapSizeComboBox.IsEnabled = true;
        DifficultyComboBox.SelectedItem = null;
        MapSizeComboBox.SelectedItem = null;
    }

    // Handle difficulty selection change
    private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DifficultyComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string difficulty = selectedItem.Content.ToString();

            switch (difficulty)
            {
                case "Slow":
                    enemyMoveInterval = 350;
                    break;
                case "Medium":
                    enemyMoveInterval = 200;
                    break;
                case "Fast":
                    enemyMoveInterval = 150;
                    break;
                case "Asian":
                    enemyMoveInterval = 50;
                    break;
            }

            // Restart the enemy movement with the new interval
            if (enemy != null)
            {
                enemyTimer.Interval = TimeSpan.FromMilliseconds(enemyMoveInterval);
            }
            DifficultyComboBox.IsEnabled = false;
        }
    }

    // Handle map size selection change
    private void MapSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MapSizeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string size = selectedItem.Content.ToString();

            int width, height;
            switch (size)
            {
                case "Small":
                    width = height = 20;
                    break;
                case "Medium":
                    width = height = 30;
                    break;
                case "Large":
                    width = height = 50;
                    break;
                default:
                    return;
            }

            labyrinth = new Labyrinth(width, height);
            ResetGame();
            MapSizeComboBox.IsEnabled = false;
        }
    }
}