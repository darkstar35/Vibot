using System.Drawing;
using System.Windows.Forms;
namespace Cellbot_Map_Tool
{
    partial class VibotMapTool
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("BackGround1", 1, 1);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("BackGround2", 2, 2);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("BackGround3", 3, 3);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Stage0_4", 4, 4);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Terrain", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("SpawnPoint", 6, 6);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Enemy", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Vibot", 4, 4);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("RedBlood", 5, 5);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Goal_Zone", 10, 10);
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("WhiteCell", 7, 7);
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Main", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Cole", 8, 8);
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("N1", 9, 9);
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Object", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode14});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VibotMapTool));
            this.파일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setSpawnInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.redBloodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.이동경로ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enemyDesignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setBGobjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // 파일ToolStripMenuItem
            // 
            this.파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            this.파일ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.파일ToolStripMenuItem.Text = "파일";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 578);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(736, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(121, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer1.Panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer1_Panel2_Scroll);
            this.splitContainer1.Panel2.ClientSizeChanged += new System.EventHandler(this.splitContainer1_Panel2_ClientSizeChanged);
            this.splitContainer1.Size = new System.Drawing.Size(736, 554);
            this.splitContainer1.SplitterDistance = 137;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(3, 134);
            this.treeView1.Name = "treeView1";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "노드1";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Text = "BackGround1";
            treeNode2.ImageIndex = 2;
            treeNode2.Name = "노드3";
            treeNode2.SelectedImageIndex = 2;
            treeNode2.Text = "BackGround2";
            treeNode3.ImageIndex = 3;
            treeNode3.Name = "노드4";
            treeNode3.SelectedImageIndex = 3;
            treeNode3.Text = "BackGround3";
            treeNode4.ImageIndex = 4;
            treeNode4.Name = "노드5";
            treeNode4.SelectedImageIndex = 4;
            treeNode4.Text = "Stage0_4";
            treeNode5.ImageIndex = 0;
            treeNode5.Name = "노드0";
            treeNode5.SelectedImageIndex = 0;
            treeNode5.Text = "Terrain";
            treeNode6.ImageIndex = 6;
            treeNode6.Name = "노드0";
            treeNode6.SelectedImageIndex = 6;
            treeNode6.Text = "SpawnPoint";
            treeNode7.ImageIndex = 0;
            treeNode7.Name = "노드0";
            treeNode7.SelectedImageIndex = 0;
            treeNode7.Text = "Enemy";
            treeNode8.ImageIndex = 4;
            treeNode8.Name = "노드0";
            treeNode8.SelectedImageIndex = 4;
            treeNode8.Text = "Vibot";
            treeNode9.ImageIndex = 5;
            treeNode9.Name = "노드1";
            treeNode9.SelectedImageIndex = 5;
            treeNode9.Text = "RedBlood";
            treeNode10.ImageIndex = 10;
            treeNode10.Name = "노드4";
            treeNode10.SelectedImageIndex = 10;
            treeNode10.Text = "Goal_Zone";
            treeNode11.ImageIndex = 7;
            treeNode11.Name = "WhiteCell";
            treeNode11.SelectedImageIndex = 7;
            treeNode11.Text = "WhiteCell";
            treeNode12.ImageIndex = 0;
            treeNode12.Name = "노드1";
            treeNode12.SelectedImageIndex = 0;
            treeNode12.Text = "Main";
            treeNode13.ImageIndex = 8;
            treeNode13.Name = "노드1";
            treeNode13.SelectedImageIndex = 8;
            treeNode13.Text = "Cole";
            treeNode14.ImageIndex = 9;
            treeNode14.Name = "노드3";
            treeNode14.SelectedImageIndex = 9;
            treeNode14.Text = "N1";
            treeNode15.ImageIndex = 0;
            treeNode15.Name = "노드0";
            treeNode15.SelectedImageIndex = 0;
            treeNode15.Text = "Object";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode7,
            treeNode12,
            treeNode15});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(131, 415);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CellbotMapTool_KeyUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.ico");
            this.imageList1.Images.SetKeyName(1, "BackGround1.png");
            this.imageList1.Images.SetKeyName(2, "BackGround2.png");
            this.imageList1.Images.SetKeyName(3, "BackGround3.png");
            this.imageList1.Images.SetKeyName(4, "Vibot.jpg");
            this.imageList1.Images.SetKeyName(5, "RedBlood.png");
            this.imageList1.Images.SetKeyName(6, "SpawnPoint.png");
            this.imageList1.Images.SetKeyName(7, "WhiteCell.png");
            this.imageList1.Images.SetKeyName(8, "Cole.png");
            this.imageList1.Images.SetKeyName(9, "N1.png");
            this.imageList1.Images.SetKeyName(10, "Goal_Zone.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.moveToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(105, 48);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.moveToolStripMenuItem.Text = "Move";
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click_1);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setSpawnInfoToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 48);
            // 
            // setSpawnInfoToolStripMenuItem
            // 
            this.setSpawnInfoToolStripMenuItem.Name = "setSpawnInfoToolStripMenuItem";
            this.setSpawnInfoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.setSpawnInfoToolStripMenuItem.Text = "Set Spawn Info";
            this.setSpawnInfoToolStripMenuItem.Click += new System.EventHandler(this.setSpawnInfoToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removeToolStripMenuItem.Text = "Move";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.MoveToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(736, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click_1);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolToolStripMenuItem1
            // 
            this.toolToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.redBloodToolStripMenuItem,
            this.enemyDesignToolStripMenuItem});
            this.toolToolStripMenuItem1.Name = "toolToolStripMenuItem1";
            this.toolToolStripMenuItem1.Size = new System.Drawing.Size(43, 20);
            this.toolToolStripMenuItem1.Text = "Tool";
            // 
            // redBloodToolStripMenuItem
            // 
            this.redBloodToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.이동경로ToolStripMenuItem});
            this.redBloodToolStripMenuItem.Name = "redBloodToolStripMenuItem";
            this.redBloodToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.redBloodToolStripMenuItem.Text = "Red Blood";
            // 
            // 이동경로ToolStripMenuItem
            // 
            this.이동경로ToolStripMenuItem.Name = "이동경로ToolStripMenuItem";
            this.이동경로ToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.이동경로ToolStripMenuItem.Text = "이동 경로";
            this.이동경로ToolStripMenuItem.Click += new System.EventHandler(this.이동경로ToolStripMenuItem_Click);
            // 
            // enemyDesignToolStripMenuItem
            // 
            this.enemyDesignToolStripMenuItem.Name = "enemyDesignToolStripMenuItem";
            this.enemyDesignToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.enemyDesignToolStripMenuItem.Text = "Enemy Design";
            this.enemyDesignToolStripMenuItem.Click += new System.EventHandler(this.enemyDesignToolStripMenuItem_Click);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setBGobjectToolStripMenuItem,
            this.moveToolStripMenuItem1});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(142, 48);
            // 
            // setBGobjectToolStripMenuItem
            // 
            this.setBGobjectToolStripMenuItem.Name = "setBGobjectToolStripMenuItem";
            this.setBGobjectToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.setBGobjectToolStripMenuItem.Text = "Set BGobject";
            // 
            // moveToolStripMenuItem1
            // 
            this.moveToolStripMenuItem1.Name = "moveToolStripMenuItem1";
            this.moveToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
            this.moveToolStripMenuItem1.Text = "Move";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(97, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Menu;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox2.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox2.Location = new System.Drawing.Point(1, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(571, 531);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
            // 
            // VibotMapTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 600);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "VibotMapTool";
            this.Text = "Vibot Map Tool";
            this.SizeChanged += new System.EventHandler(this.CellbotMapTool_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VibotMapTool_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CellbotMapTool_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ImageList imageList1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TreeView treeView1;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem setSpawnInfoToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem toolToolStripMenuItem1;
        private ToolStripMenuItem redBloodToolStripMenuItem;
        private ToolStripMenuItem 이동경로ToolStripMenuItem;
        private ToolStripMenuItem enemyDesignToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem moveToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip3;
        private ToolStripMenuItem setBGobjectToolStripMenuItem;
        private ToolStripMenuItem moveToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
    }

}

