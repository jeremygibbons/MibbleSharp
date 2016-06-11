namespace MibbleBrowser
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private MibTreeBuilder mibTreeBuilder;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fIleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMIBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeMibs = new System.Windows.Forms.TreeView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colOID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openFileDialogMain = new System.Windows.Forms.OpenFileDialog();
            this.txtNodeInfo = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fIleToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(799, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fIleToolStripMenuItem
            // 
            this.fIleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMIBToolStripMenuItem});
            this.fIleToolStripMenuItem.Name = "fIleToolStripMenuItem";
            this.fIleToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fIleToolStripMenuItem.Text = "File";
            this.fIleToolStripMenuItem.Click += new System.EventHandler(this.fIleToolStripMenuItem_Click);
            // 
            // loadMIBToolStripMenuItem
            // 
            this.loadMIBToolStripMenuItem.Name = "loadMIBToolStripMenuItem";
            this.loadMIBToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.loadMIBToolStripMenuItem.Text = "Load MIB";
            this.loadMIBToolStripMenuItem.Click += new System.EventHandler(this.loadMIBToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtNodeInfo);
            this.splitContainer1.Panel1.Controls.Add(this.treeMibs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(799, 425);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeMibs
            // 
            this.treeMibs.Location = new System.Drawing.Point(3, 3);
            this.treeMibs.Name = "treeMibs";
            this.treeMibs.Size = new System.Drawing.Size(262, 284);
            this.treeMibs.TabIndex = 0;
            this.treeMibs.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeMibs_NodeMouseClick);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOID,
            this.colValue});
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(523, 349);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // colOID
            // 
            this.colOID.HeaderText = "OID";
            this.colOID.Name = "colOID";
            // 
            // colValue
            // 
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            // 
            // openFileDialogMain
            // 
            this.openFileDialogMain.FileName = "openFileDialogMain";
            // 
            // txtNodeInfo
            // 
            this.txtNodeInfo.Location = new System.Drawing.Point(2, 293);
            this.txtNodeInfo.Multiline = true;
            this.txtNodeInfo.Name = "txtNodeInfo";
            this.txtNodeInfo.ReadOnly = true;
            this.txtNodeInfo.Size = new System.Drawing.Size(262, 129);
            this.txtNodeInfo.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 449);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMain";
            this.Text = "Mibble Sharp";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fIleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMIBToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeMibs;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.OpenFileDialog openFileDialogMain;
        private System.Windows.Forms.TextBox txtNodeInfo;
    }
}

