using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Zzb.ML.Gobang
{
    public partial class frmText : Form
    {
        public void Log(string text)
        {
            Invoke(new EventHandler(delegate { textBox1.Text += text + "\r\n"; }));
        }

        public frmText()
        {
            InitializeComponent();
        }
    }
}
