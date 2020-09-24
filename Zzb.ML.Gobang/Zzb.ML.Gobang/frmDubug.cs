using System;
using System.Windows.Forms;

namespace Zzb.ML.Gobang
{
    public partial class frmDubug : Form
    {
        public frmDubug(GameBoard board)
        {
            InitializeComponent();
            this.board = board;
        }

        private GameBoard board = null;

        private void btnJudge_Click(object sender, EventArgs e)
        {
            int x = (int)numX.Value - 1;
            int y = (int)numY.Value - 1;
            int dir = CboDir.SelectedIndex;
            int color = CboColor.SelectedIndex + 1;
            Object obj = board.GetSituationType(x, y, dir, color);
            MessageBox.Show(obj.ToString());
        }
    }
}
