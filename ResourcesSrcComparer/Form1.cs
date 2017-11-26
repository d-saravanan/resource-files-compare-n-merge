using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResourcesSrcComparer
{
    public partial class Form1 : Form
    {
        ResourceFileComparer resourceFileComparer = null;

        public Form1()
        {
            InitializeComponent();
            resourceFileComparer = new ResourceFileComparer();
        }

        public string srcFileName, targetFileName;

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            missingNodesInTgt.DataSource = null;

            var result = resourceFileComparer.Compare(srcFileName, targetFileName);

            UpdateGUI(result);
        }

        private void UpdateGUI(Tuple<Dictionary<string, string>, List<string>> result)
        {
            //Node with different values
            if (result.Item1 != null && result.Item1.Count > 0)
                dataGridView1.DataSource = result.Item1.Select(x => new { x.Key, x.Value }).ToList();

            //Non-Existent Nodes in Target
            if (null != result.Item2 && result.Item2.Count > 0)
                missingNodesInTgt.DataSource = result.Item2.Select(x => new { Value = x }).ToList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            targetFileName = openFileDialog2.FileName;
            lblTgtFileName.Text = targetFileName;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            srcFileName = openFileDialog1.FileName;
            lblSrcFileName.Text = srcFileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var result = resourceFileComparer.Compare(srcFileName, targetFileName, true, consoleLog);
            UpdateGUI(result);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
