using System.Drawing;
using System.Numerics;
using Raylib_cs;

namespace Snake
{
    public class Game
    {
        Vector2 position = new Vector2(10, 10);
        Queue<Point> Snake = new Queue<Point>();
        Point head;
        bool SnakeEat;
        float SnakeSpeed;

        Point apple;
        int Score;

        private const int GRIDH = 25;
        private const int GRIDW = 25;
        private const int CELLH = 20;
        private const int CELLW = 20;
        int[,] Grid = new int[GRIDH, GRIDW];

        float timer = 0;

        enum dir
        {
            up,
            right,
            down,
            left
        }

        enum egameState
        {
            pause,
            play,
            gameover
        }

        egameState gameState;
        dir SnakeDir;
        dir NextDir;
        int SnakeDistance;

        private void Init()
        {
            for (int h = 0; h < GRIDH; h++)
            {
                for (int l = 0; l < GRIDW; l++)
                {
                    Grid[h, l] = 0;
                }
            }

            Snake.Clear();
            head = new Point((int)GRIDW/2, (int)GRIDH/2);
            Snake.Enqueue(head);
            timer = 0;
            SnakeDir = dir.right;
            NextDir = SnakeDir;
            gameState = egameState.pause;
            SnakeDistance = 0;
            NewApple();
            SnakeEat = false;
            Score = 0;
            SnakeSpeed = 0.5f;

        }

        public Game()
        {
            Init(); 
        }

        private void SnakeMove(int pOX, int pOY)
        {
            Point newHead = new Point(head.X + pOX, head.Y + pOY);
            head = newHead;
            SnakeDistance++;

            if (head.X < 0 || head.X == GRIDW || head.Y < 0 || head.Y == GRIDH)
            {
                gameState = egameState.gameover;
            }

            if (SnakeAt(head.Y, head.X))
            {
                gameState = egameState.gameover;
            }
            else
            {
                Snake.Enqueue(newHead);
                if (!SnakeEat && Snake.Count >= 3)
                    Snake.Dequeue();
                SnakeEat = false;
            }
        }

        private bool SnakeAt(int pl, int ph)
        {
            bool result = false;
            foreach (Point p in Snake)
            {
                if (p.X == ph && p.Y == pl)
                    result = true;
            }
            return result;
        }

        private void NewApple()
        {
            int l;
            int h;
            Random rnd = new Random();
            do
            {
                l = rnd.Next(0, GRIDW);
                h = rnd.Next(0, GRIDH);
            } while (SnakeAt(h, l) == true);

            apple = new Point(l, h);
            Score++;
            if (Score % 5 == 0)
                SnakeSpeed -= 0.05f;
        }

        private void Play()
        {
            timer += Raylib.GetFrameTime();

            if (Raylib.IsKeyDown(KeyboardKey.Up) && (SnakeDir == dir.right || SnakeDir == dir.left))
            {
                NextDir = dir.up;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Right) && (SnakeDir == dir.up || SnakeDir == dir.down))
            {
                NextDir = dir.right;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Down) && (SnakeDir == dir.right || SnakeDir == dir.left))
            {
                NextDir = dir.down;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Left) && (SnakeDir == dir.up || SnakeDir == dir.down))
            {
                NextDir = dir.left;
            }

            if (timer >= SnakeSpeed)
            {
                timer = 0;

                SnakeDir = NextDir;

                switch (SnakeDir)
                {
                    case dir.up:
                        SnakeMove(0, -1);
                        break;
                    case dir.right:
                        SnakeMove(+1, 0);
                        break;
                    case dir.down:
                        SnakeMove(0, +1);
                        break;
                    case dir.left:
                        SnakeMove(-1, 0);
                        break;
                    default:
                        break;
                }
            }

            if (head == apple)
            {
                NewApple();
                SnakeEat = true;
            }

        }

        public void Update()
        {
            switch (gameState)
            {
                case egameState.pause:
                    if (Raylib.IsKeyDown(KeyboardKey.Space))
                        gameState = egameState.play;
                    break;
                case egameState.play:
                    Play();
                    break;
                case egameState.gameover:
                    if (Raylib.IsKeyDown(KeyboardKey.Space))
                    {
                        Init();
                        gameState = egameState.pause;
                    }
                        
                    break;
            }
        }

        public void Draw()
        {
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Raylib_cs.Color.White);

            Raylib.DrawText("Mon Snake", 1, 1, 40, Raylib_cs.Color.Gray);

            if (gameState == egameState.pause)
                Raylib.DrawText("Appuie sur ESPACE ", 10, 500 + 60, 20, Raylib_cs.Color.DarkBlue);

            else
                Raylib.DrawText("Score : " + Score, 10, 500 + 60, 20, Raylib_cs.Color.DarkBlue);

            if (gameState == egameState.gameover)
                Raylib.DrawText("Tu as perdu ! ", 10, 600 + 60, 50, Raylib_cs.Color.Red);



            for (int h = 0; h < GRIDH; h++)
            {
                for (int l = 0; l < GRIDW; l++)
                {
                    int x = h * CELLH;
                    int y = l * CELLW;

                    Raylib.DrawRectangleLines(x + 30, y + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.Brown);
                }
            }

            foreach (Point p in Snake)
            {
                int x = p.X * CELLW;
                int y = p.Y * CELLH;

                Raylib.DrawRectangle(x + 30, y + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.Green);
            }

            int xa = apple.X * CELLW;
            int ya = apple.Y * CELLH;

            Raylib.DrawRectangle(xa + 30, ya + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.Red);

            Raylib.EndDrawing();
        }
    }

}
