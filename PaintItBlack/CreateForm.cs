using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintItBlack
{
    public partial class CreateForm : Form
    {
        public bool create;
        public CreateForm()
        {
            InitializeComponent();
            create = false;
            numericUpDown1.Maximum = 2000;
            numericUpDown1.Minimum = 1;
            numericUpDown2.Maximum = 2000;
            numericUpDown2.Minimum = 1;
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            create = true;
            Close();
        }
        public Bitmap GetNewBitmap()
        {
            return new Bitmap(Convert.ToInt32(numericUpDown2.Value), Convert.ToInt32(numericUpDown1.Value));
        }

        private void Button2_Click(object sender, EventArgs e)
        {
  
            create = false;
            Close();
        }
    }
}
