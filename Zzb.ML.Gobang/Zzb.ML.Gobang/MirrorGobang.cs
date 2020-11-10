using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Zzb.ML.EF;

namespace Zzb.ML.Gobang
{
    public class MirrorGobang
    {
        private List<EF.Gobang> _gobangs = new List<EF.Gobang>();

        public void AddPoint(int[,] map, Point point, bool isBlack)
        {

            var gobang = new EF.Gobang() { IsBlack = isBlack, X = point.X, Y = point.Y };
            gobang.SetPoint(map);
            _gobangs.Add(gobang);

            //镜像X
            int[,] temp = new int[15, 15];
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    temp[i, j] = map[i, 14 - j];
                    temp[i, 14 - j] = map[i, j];
                }
            }


            gobang = new EF.Gobang() { IsBlack = isBlack, X = 14 - point.X, Y = point.Y };
            gobang.SetPoint(temp);
            _gobangs.Add(gobang);

            //镜像Y
            temp = new int[15, 15];
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    temp[j, i] = map[14 - j, i];
                    temp[14 - j, i] = map[j, i];
                }
            }


            gobang = new EF.Gobang() { IsBlack = isBlack, X = point.X, Y = 14 - point.Y };
            gobang.SetPoint(temp);
            _gobangs.Add(gobang);

            //镜像XY
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int t1 = temp[i, j];
                    temp[i, j] = temp[i, 14 - j];
                    temp[i, 14 - j] = t1;
                }
            }

            gobang = new EF.Gobang() { IsBlack = isBlack, X = 14 - point.X, Y = 14 - point.Y };
            gobang.SetPoint(temp);
            _gobangs.Add(gobang);
        }
    }
}