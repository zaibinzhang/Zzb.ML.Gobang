using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zzb.ML.AI;
using Zzb.ML.GameComponent;
using Tensorflow.Keras.Engine;
using System.Diagnostics;

namespace Zzb.ML.Gobang;

public partial class GameBoard
{
    private List<Point> blackHistory;

    private List<Point> whiteHistory;

    private bool hasData = false;

    private FrmMessage _frmMessage;

    private int _i = 1;

    private Stopwatch _stopwatch = new Stopwatch();

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
        _stopwatch.Start();
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

            Point first;
            Point last;
            var aa = _winPoints;
            if (aa[0].X == aa[1].X)
            {
                var linq = from i in aa orderby i.Y select i;
                first = linq.First();
                last = linq.Last();

            }
            else
            {
                var linq = from i in aa orderby i.X select i;
                first = linq.First();
                last = linq.Last();
            }
            Invoke(() =>
            {
                var firstPoint = IndexToScreen(first.X, first.Y);
                var lastPoint = IndexToScreen(last.X, last.Y);
                using Graphics g = Graphics.FromImage(this.BackgroundImage);
                using (Pen pen = new Pen(Color.Red, 3))
                {
                    // 画一条红线，起始点 (10, 10)，结束点 (100, 100)
                    g.DrawLine(pen, firstPoint.X, firstPoint.Y, lastPoint.X, lastPoint.Y);
                }
                this.Invalidate();
            });
            Invoke(() => { });
            var (bLoss, bAccuracy, wLoss, wAccuracy) = GoBangAi.Train(whiteHistory, blackHistory);
            Invoke(() =>
            {
                string tip = color == 1 ? "黑" : "白";
                _frmMessage.textBox1.Text = $"第{_i++}次对局训练，{tip}胜，黑棋：loss是{bLoss:0.0000},a是{bAccuracy:0.0000}。白棋：loss是{wLoss:0.0000},a是{wAccuracy:0.0000}。\r\n" + _frmMessage.textBox1.Text;
                TimeSpan ts = _stopwatch.Elapsed;
                _frmMessage.SetTitle($"训练时间：{String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Hours, ts.Minutes, ts.Seconds)}");
            });
        }
    }

    public Point AiNextStep()
    {
        var list = new List<MapValueItem>();

        var mapValue = GoBangAi.Predict(whiteHistory, blackHistory);
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                list.Add(new MapValueItem() { X = j, Y = i, Value = mapValue[i, j] });
            }
        }

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
        while (!IsGameEnd(point))
        {
            color = 3 - color;
            point = RandomNextStep();
        }

        Invoke(() => { GoBangAi.Train(whiteHistory, blackHistory); });
        //OnGameEnd(this, new GameEndEventArgs(color));
    }


    private GoBangAi GoBangAi
    {
        get { return _goBangAi ??= new GoBangAi(); }

    }

    private GoBangAi _goBangAi;

    public Point RandomNextStep()
    {
        var point = GoBangAi.CalculateNextStep(map, whiteHistory, blackHistory);
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