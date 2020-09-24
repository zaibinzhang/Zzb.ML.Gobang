using System.Drawing;
using System.Windows.Forms;

namespace Zzb.ML.Gobang
{
    class Chessman//棋子类
    {
        static Bitmap blackchess = new Bitmap(Resource.black);
        static Bitmap whitechess = new Bitmap(Resource.white);
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }

        public void AddedToChessBoard(PictureBox[,] Chesspics)
        {
            Chesspics[X, Y].Visible = true;
            if (Color == 0)
            {
                Chesspics[X, Y].Image = blackchess;
            }
            else
            {
                Chesspics[X, Y].Image = whitechess;
            }
        }
    }
}
