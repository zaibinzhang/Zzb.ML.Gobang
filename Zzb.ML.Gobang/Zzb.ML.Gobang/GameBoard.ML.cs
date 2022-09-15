using System.Drawing;
using System;
using Zzb.ML.GameComponent;

namespace Zzb.ML.Gobang;

public partial class GameBoard
{

    public void RandomPlay()
    {
        Createbackgroudimage();
        map = new int[gameSize + 1, gameSize + 1];
        _point = RandomNextStep();
        while (!IsGameEnd(_point))
        {
            color = 3 - color;
            _point = RandomNextStep();
        }
        OnGameEnd(this, new GameEndEventArgs(color));
    }

    private Point _point;

    private readonly Random rand = new();

    public void Check()
    {
        IsGameEnd(_point);
    }

    public Point RandomNextStep()
    {
        while (true)
        {
            var point = new Point(rand.Next(gameSize), rand.Next(gameSize));
            if (map[point.Y, point.X] == 0)
            {
                map[point.Y, point.X] = color;
                AddChessman(IndexToScreen(point.X, point.Y), color);
                return point;
            }
        }
    }
}