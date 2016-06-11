using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MibbleBrowser
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            mibTreeBuilder = new MibTreeBuilder(treeMibs);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fIleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void loadMIBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialogMain.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialogMain.FileName;
                mibTreeBuilder.LoadMibFile(file);
            }
        }
    }
}
