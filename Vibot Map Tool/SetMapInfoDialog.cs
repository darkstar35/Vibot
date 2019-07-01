using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cellbot_Map_Tool
{
    public partial class CSetMapInfoDialog : Form
    {
        private VibotMapTool m_MainForm = (VibotMapTool)Application.OpenForms[0];
        public CSetMapInfoDialog()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_MainForm.SetMapSize(int.Parse(comboBox1.Text), int.Parse(comboBox2.Text)); 
            m_MainForm.SetTileSize(int.Parse(comboBox3.Text), int.Parse(comboBox4.Text));
            m_MainForm.LOAD_InitalizeMap();
            m_MainForm.ClearMap();
            this.Close();
        }
    }
}
