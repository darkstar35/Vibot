using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Cellbot_Map_Tool
{
    public partial class SetEnemyDialog : Form
    {
        public SetEnemyDialog()
        {
            InitializeComponent();
        }

        public void SaveEnemyInfo(string FullPath)
        {
            XmlTextWriter MapFile = new XmlTextWriter(FullPath, Encoding.UTF8);

            MapFile.Formatting = Formatting.Indented;

            MapFile.WriteStartDocument();

            MapFile.WriteStartElement("ENEMY");
            for(int i = 0; i < dataGridView1.RowCount; ++i)
            {
                MapFile.WriteStartElement("INFO");
                for(int j = 0; j < dataGridView1.ColumnCount; ++j)
                {
                    MapFile.WriteStartElement(dataGridView1.Columns[j].HeaderText);
                    MapFile.WriteString((string)dataGridView1[j, i].Value);
                    MapFile.WriteEndElement();
                }
                MapFile.WriteEndElement();
            }

            MapFile.WriteEndElement();
 
            MapFile.WriteEndDocument();

            MapFile.Close();
        }

        public void LoadEnemyInfo(string FullPath)
        {
            dataGridView1.Rows.Clear();
            XmlDocument MapFile = new XmlDocument();

            MapFile.Load(FullPath);
            XmlElement Root = MapFile.DocumentElement;
            XmlNodeList ChildNodes = Root.ChildNodes;
            foreach (XmlNode Node in ChildNodes)
            {
                XmlNodeList ChildNodes2 = Node.ChildNodes;
                int iRowNum = dataGridView1.Rows.Add();
                int iColumn = 0;
                foreach (XmlNode Node2 in ChildNodes2)
                {
                    dataGridView1.Rows[iRowNum].Cells[iColumn].Value = Node2.InnerText;
                    iColumn++;
                }
            }
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void 로드ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.InitialDirectory = Directory.GetCurrentDirectory();
            Dialog.DefaultExt = "xml";
            Dialog.Filter = "xml files (*.xml)|*.xml";
            Dialog.FilterIndex = 2;
            Dialog.RestoreDirectory = true;
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                LoadEnemyInfo(Dialog.FileName);
            }
        }

        private void 저장ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dialog = new SaveFileDialog();
            Dialog.InitialDirectory = Directory.GetCurrentDirectory();
            Dialog.DefaultExt = "xml";
            Dialog.Filter = "xml files (*.xml)|*.xml";
            Dialog.FilterIndex = 2;
            Dialog.RestoreDirectory = true;

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                SaveEnemyInfo(Dialog.FileName);
            }
        }
    }
}
