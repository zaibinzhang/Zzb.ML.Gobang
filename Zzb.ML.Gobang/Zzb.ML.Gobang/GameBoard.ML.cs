using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zzb.ML.AI;
using Zzb.ML.GameComponent;

namespace Zzb.ML.Gobang;

public partial class GameBoard
{
    private List<Point> blackHistory;

    private List<Point> whiteHistory;

    private bool hasData = false;

    public void AutoPlay()
    {
        new Task(() =>
        {
            while (true)
            {
                AutoPlayTask();
            }
        }).Start();
    }

    private void AutoPlayTask()
    {
        blackHistory = new List<Point>();
        whiteHistory = new List<Point>();
        color = 1;
        Invoke(Createbackgroudimage);
        map = new int[gameSize + 1, gameSize + 1];
        if (!hasData)
        {
            RandomPlay();
            hasData = true;
        }
        else
        {
            var point = AiNextStep();
            while (!IsGameEnd(point))
            {
                color = 3 - color;
                point = AiNextStep();
            }

            Invoke(() => { goBangAi.Train(whiteHistory, blackHistory); });
        }
    }

    public Point AiNextStep()
    {
        var list = new List<MapValueItem>();
        Invoke(() =>
        {
            var mapValue = goBangAi.Predict(whiteHistory, blackHistory);
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    list.Add(new MapValueItem() { X = i, Y = j, Value = mapValue[i, j] });
                }
            }
        });


        var listSort = (from i in list orderby i.Value select i).ToList();

        var point = new Point();
        for (int i = 0; i < listSort.Count; i++)
        {
            var one = listSort[i];
            point = new Point(one.X, one.Y);
            if (map[one.X, one.Y] == 0)
            {
                if (color == 1)
                {
                    blackHistory.Add(point);
                }
                else
                {
                    whiteHistory.Add(point);
                }
                map[point.X, point.Y] = color;
                Invoke(() => { AddChessman(IndexToScreen(point.X, point.Y), color); });
                return point;
            }
        }
        return point;
    }

    public void RandomPlay()
    {
        var point = RandomNextStep();
        while (!IsGameEnd(point))
        {
            color = 3 - color;
            point = RandomNextStep();
        }

        Invoke(() => { goBangAi.Train(whiteHistory, blackHistory); });
        //OnGameEnd(this, new GameEndEventArgs(color));
    }


    private readonly GoBangAi goBangAi = new();

    public Point RandomNextStep()
    {
        var point = goBangAi.CalculateNextStep(map, whiteHistory, blackHistory);
        map[point.X, point.Y] = color;
        if (color == 1)
        {
            blackHistory.Add(point);
        }
        else
        {
            whiteHistory.Add(point);
        }

        Invoke(() => { AddChessman(IndexToScreen(point.X, point.Y), color); });

        return point;
    }

    private class MapValueItem
    {
        public int X { get; set; }

        public int Y { get; set; }

        public float Value { get; set; }
    }
}