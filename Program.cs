using System.Numerics;
using Raylib_cs;
using Snake;

public static class Program
{
    public static Game game = new Game();
    public static Task Main(string[] args)
    {
        Raylib.InitWindow(2140, 1440, "Snake");
        Raylib.SetTargetFPS(60);


        while (!Raylib.WindowShouldClose())
        {
            game.Update();
            game.Draw();
        
        }

        Raylib.CloseWindow();
        return Task.CompletedTask;
    }
}
