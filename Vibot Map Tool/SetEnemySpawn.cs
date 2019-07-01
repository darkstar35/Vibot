using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VibotCommon;

namespace Cellbot_Map_Tool
{
    public partial class SetEnemySpawn : Form
    {
        private VibotMapTool m_MainForm = (VibotMapTool)Application.OpenForms[0];
        public SetEnemySpawn()
        {
            InitializeComponent();
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void SetEnemySpawn_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void SetEnemySpawn_Load(object sender, EventArgs e)
        {
            CEnemySpawnInfo EnemySpawnInfo = m_MainForm.GetCurrentVirusSpwanInfo();
            if (EnemySpawnInfo != null)
            {
                textBox1.Text = string.Format("{0}", EnemySpawnInfo.m_Interval);
                textBox2.Text = string.Format("{0}", EnemySpawnInfo.m_AliveTime);
                foreach (CEnemySpawnInfo._SpawnInfo SpawnInfo in EnemySpawnInfo.m_SpwanInfo)
                {
                    int iRowNum = dataGridView1.Rows.Add();
                    dataGridView1.Rows[iRowNum].Cells[0].Value = SpawnInfo.m_EnemyName;
                    dataGridView1.Rows[iRowNum].Cells[1].Value = string.Format("{0}", SpawnInfo.m_Probability);
                }
            }
        }

        private void SetEnemySpawn_FormClosing(object sender, FormClosingEventArgs e)
        {
            CEnemySpawnInfo EnemySpawnInfo = new CEnemySpawnInfo();

            EnemySpawnInfo.m_Interval = int.Parse(textBox1.Text);
            EnemySpawnInfo.m_AliveTime = int.Parse(textBox2.Text);
            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null) continue;
                string EnemyName = (string)dataGridView1.Rows[i].Cells[0].Value;
                int Probability = dataGridView1.Rows[i].Cells[1].Value == null ? 0 :int.Parse((string)dataGridView1.Rows[i].Cells[1].Value);
                EnemySpawnInfo.m_SpwanInfo.Add(new CEnemySpawnInfo._SpawnInfo(EnemyName, Probability));
            }
            m_MainForm.SaveVirusSpwanInfo(EnemySpawnInfo);
        }
    }
}
