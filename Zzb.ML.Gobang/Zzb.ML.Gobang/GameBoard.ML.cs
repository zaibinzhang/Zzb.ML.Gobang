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

    private FrmMessage _frmMessage;

    private int _i = 1;

    public void AutoPlay()
    {
        _frmMessage = new FrmMessage();
        _frmMessage.Show();
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
            while (!IsGameEnd(point) && whiteHistory.Count + blackHistory.Count < gameSize * gameSize)
            {
                color = 3 - color;
                point = AiNextStep();
            }

            Invoke(() =>
            {
                var (l, a) = goBangAi.Train(whiteHistory, blackHistory, IsGameEnd(point));
                _frmMessage.textBox1.Text = $"第{_i++}次训练，loss是{l},a是{a}\r\n" + _frmMessage.textBox1.Text;
            });
        }
    }

    public Point AiNextStep()
    {
        var list = new List<MapValueItem>();
        Invoke(() =>
        {
            var mapValue = goBangAi.Predict(whiteHistory, blackHistory);
            for (int i = 0; i < gameSize; i++)
            {
                for (int j = 0; j < gameSize; j++)
                {
                    list.Add(new MapValueItem() { X = j, Y = i, Value = mapValue[i, j] });
                }
            }
        });


        var listSort = (from i in list orderby i.Value descending select i).ToList();

        var point = new Point();
        for (int i = 0; i < listSort.Count; i++)
        {
            var one = listSort[i];
            if (one.Value == 0)
            {
                return RandomNextStep();
            }
            point = new Point(one.X, one.Y);
            if (map[one.Y, one.X] == 0)
            {
                if (color == 1)
                {
                    blackHistory.Add(point);
                }
                else
                {
                    whiteHistory.Add(point);
                }
                map[point.Y, point.X] = color;
                Invoke(() => { AddChessman(IndexToScreen(point.X, point.Y), color); });
                return point;
            }
        }
        return point;
    }

    public void RandomPlay()
    {
        var point = RandomNextStep();
        while (!IsGameEnd(point) && whiteHistory.Count + blackHistory.Count < gameSize * gameSize)
        {
            color = 3 - color;
            point = RandomNextStep();
        }

        Invoke(() => { goBangAi.Train(whiteHistory, blackHistory, IsGameEnd(point)); });
        //OnGameEnd(this, new GameEndEventArgs(color));
    }


    private readonly GoBangAi goBangAi = new();

    public Point RandomNextStep()
    {
        var point = goBangAi.CalculateNextStep(map, whiteHistory, blackHistory);
        map[point.Y, point.X] = color;
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