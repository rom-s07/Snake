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

        Point redApple;
        Point goldApple;
        Point brownApple;
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

        Sound sndredApple;
        Sound sndgoldApple;
        Sound sndBrownApple;

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
            sndredApple = Raylib.LoadSound("sounds/redapple.wav");
            sndgoldApple = Raylib.LoadSound("sounds/goldenapple.wav");
            sndBrownApple = Raylib.LoadSound("sounds/brownapple.wav");
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
            int lr;
            int hr;
            int lg;
            int hg;
            int lb;
            int hb;
            Random rnd = new Random();

            do
            {
                lr = rnd.Next(0, GRIDW);
                hr = rnd.Next(0, GRIDH);
            } while (SnakeAt(hr, lr) == true);
            redApple = new Point(lr, hr);


            do
            {
                lg = rnd.Next(0, GRIDW);
                hg = rnd.Next(0, GRIDH);
            } while (SnakeAt(lg, hg) == true);
            goldApple = new Point(lg, hg);
            

            do
            {
                lb = rnd.Next(0, GRIDW);
                hb = rnd.Next(0, GRIDH);
            }while(SnakeAt(hb, lb) == true);

            brownApple = new Point(lb, hb);

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

            if (head == redApple)
            {
                Raylib.PlaySound(sndredApple);
                NewApple();
                SnakeEat = true;
                Score++;
                if (Score % 5 == 0)
                    SnakeSpeed -= 0.03f;
            }

            if (head == goldApple)
            {
                Raylib.PlaySound(sndgoldApple);
                NewApple();
                SnakeEat = true;
                Score += 10;
                if (SnakeEat = true)
                    SnakeSpeed -= 0.05f;
            }

            if (head == brownApple)
            {
                Raylib.PlaySound(sndBrownApple);
                gameState = egameState.gameover;
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

            Raylib.DrawText("Mon Snake finalement pas du tout originale", 1, 1, 40, Raylib_cs.Color.Gray);

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

                    Raylib.DrawRectangleLines(x + 30, y + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.DarkGray);
                }
            }

            foreach (Point p in Snake)
            {
                int x = p.X * CELLW;
                int y = p.Y * CELLH;

                Raylib.DrawRectangle(x + 30, y + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.Green);
            }

            int xar = redApple.X * CELLW;
            int yar = redApple.Y * CELLH;

            Raylib.DrawRectangle(xar + 30, yar + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.Red);

            int xag = goldApple.X * CELLW;
            int yag = goldApple.Y * CELLH;

            Raylib.DrawRectangle(xag + 30, yag + 30, CELLW - 1, CELLH - 1, Raylib_cs.Color.Gold);

            int xab = brownApple.X * CELLW;
            int yab = brownApple.Y * CELLH;

            Raylib.DrawRectangle(xab + 30, yab + 30, CELLH - 1, CELLH - 1, Raylib_cs.Color.Brown);

            Raylib.EndDrawing();
        }
    }

}
