using System.Drawing;
using System;
using System.Collections.Generic;
using Zzb.ML.AI;
using Zzb.ML.GameComponent;

namespace Zzb.ML.Gobang;

public partial class GameBoard
{
    private List<Point> _blackHistory;

    private List<Point> _whiteHistory;

    public void RandomPlay()
    {
        _blackHistory = new List<Point>();
        _whiteHistory = new List<Point>();
        color = 1;
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

    private readonly GoBangAi goBangAi = new();

    public void Check()
    {
        IsGameEnd(_point);
    }

    public Point RandomNextStep()
    {
        var point = goBangAi.CalculateNextStep(map, _whiteHistory, _blackHistory);
        map[point.Y, point.X] = color;
        if (color == 1)
        {
            _blackHistory.Add(point);
        }
        else
        {
            _whiteHistory.Add(point);
        }
        AddChessman(IndexToScreen(point.X, point.Y), color);
        return point;
    }
}