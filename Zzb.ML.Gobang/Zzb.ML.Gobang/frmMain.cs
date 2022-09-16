using System;
using System.Drawing;
using System.Windows.Forms;

namespace Zzb.ML.Gobang
{
    public partial class frmMain : Form
    {
        private GameBoard board;

        public frmMain()
        {
            InitializeComponent();
            setGameBoard();
            
        }

        private void setGameBoard()
        {
            board = new GameBoard();
            board.OnGameEnd += board_OnGameEnd;
            this.Controls.Add(board);
            Rectangle rec = this.ClientRectangle;
            int xPos = (rec.Width - board.Width) / 2;
            int yPos = (rec.Height - menu.Height - board.Height) / 2 + menu.Height;
            board.Left = xPos;
            board.Top = yPos;
        }

        private void board_OnGameEnd(object sender, GameComponent.GameEndEventArgs e)
        {
            MessageBox.Show(e.Message, "提示", MessageBoxButtons.OK);
        }

        private void menuItemNewGame_Click(object sender, EventArgs e)
        {
            board.ReStartGame();
        }

        private void menuItemDebug_Click(object sender, EventArgs e)
        {
            frmDubug frm = new frmDubug(this.board);
            frm.Show();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本游戏由火蜥蜴开发^_^，\nQQ：1906747819");
        }

        private void autoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.AutoPlay();
        }

    }
}
