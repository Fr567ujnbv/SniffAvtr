
namespace SniffAvtr
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.components = new System.ComponentModel.Container();
			this.Button_StartStop = new System.Windows.Forms.Button();
			this.ComboBox_Interface = new System.Windows.Forms.ComboBox();
			this.Label_Interface = new System.Windows.Forms.Label();
			this.Button_Clear = new System.Windows.Forms.Button();
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.CheckBox_AutoScroll = new System.Windows.Forms.CheckBox();
			this.CheckBox_SkipDuplicates = new System.Windows.Forms.CheckBox();
			this.ContextMenuStrip_ListView = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyUserDisplaynameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyUserIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.copyAvatarNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAvatarDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAvatarIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.copyAuthorDisplaynameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAuthorIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.copyAssetURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MainListView = new ListViewEx();
			this.ColumnHeaderUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderUserId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderPlatform = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarReleaseStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarUploaded = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarAuthorId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnHeaderAvatarUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.CheckBox_LogRawPackets = new System.Windows.Forms.CheckBox();
			this.ContextMenuStrip_ListView.SuspendLayout();
			this.SuspendLayout();
			// 
			// Button_StartStop
			// 
			this.Button_StartStop.Location = new System.Drawing.Point(12, 12);
			this.Button_StartStop.Name = "Button_StartStop";
			this.Button_StartStop.Size = new System.Drawing.Size(100, 55);
			this.Button_StartStop.TabIndex = 1;
			this.Button_StartStop.Text = "&Start";
			this.Button_StartStop.UseVisualStyleBackColor = true;
			this.Button_StartStop.Click += new System.EventHandler(this.Button_StartStop_Click);
			// 
			// ComboBox_Interface
			// 
			this.ComboBox_Interface.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ComboBox_Interface.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.ComboBox_Interface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBox_Interface.FormattingEnabled = true;
			this.ComboBox_Interface.Location = new System.Drawing.Point(499, 12);
			this.ComboBox_Interface.Name = "ComboBox_Interface";
			this.ComboBox_Interface.Size = new System.Drawing.Size(289, 21);
			this.ComboBox_Interface.Sorted = true;
			this.ComboBox_Interface.TabIndex = 2;
			// 
			// Label_Interface
			// 
			this.Label_Interface.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Label_Interface.AutoSize = true;
			this.Label_Interface.Location = new System.Drawing.Point(441, 15);
			this.Label_Interface.Name = "Label_Interface";
			this.Label_Interface.Size = new System.Drawing.Size(52, 13);
			this.Label_Interface.TabIndex = 3;
			this.Label_Interface.Text = "Interface:";
			// 
			// Button_Clear
			// 
			this.Button_Clear.Location = new System.Drawing.Point(118, 12);
			this.Button_Clear.Name = "Button_Clear";
			this.Button_Clear.Size = new System.Drawing.Size(100, 55);
			this.Button_Clear.TabIndex = 4;
			this.Button_Clear.Text = "&Clear";
			this.Button_Clear.UseVisualStyleBackColor = true;
			this.Button_Clear.Click += new System.EventHandler(this.Button_Clear_Click);
			// 
			// UpdateTimer
			// 
			this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
			// 
			// CheckBox_AutoScroll
			// 
			this.CheckBox_AutoScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CheckBox_AutoScroll.AutoSize = true;
			this.CheckBox_AutoScroll.Checked = true;
			this.CheckBox_AutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBox_AutoScroll.Location = new System.Drawing.Point(711, 50);
			this.CheckBox_AutoScroll.Name = "CheckBox_AutoScroll";
			this.CheckBox_AutoScroll.Size = new System.Drawing.Size(77, 17);
			this.CheckBox_AutoScroll.TabIndex = 5;
			this.CheckBox_AutoScroll.Text = "Auto Scroll";
			this.CheckBox_AutoScroll.UseVisualStyleBackColor = true;
			// 
			// CheckBox_SkipDuplicates
			// 
			this.CheckBox_SkipDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CheckBox_SkipDuplicates.AutoSize = true;
			this.CheckBox_SkipDuplicates.Checked = true;
			this.CheckBox_SkipDuplicates.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBox_SkipDuplicates.Location = new System.Drawing.Point(605, 50);
			this.CheckBox_SkipDuplicates.Name = "CheckBox_SkipDuplicates";
			this.CheckBox_SkipDuplicates.Size = new System.Drawing.Size(100, 17);
			this.CheckBox_SkipDuplicates.TabIndex = 6;
			this.CheckBox_SkipDuplicates.Text = "Skip Duplicates";
			this.CheckBox_SkipDuplicates.UseVisualStyleBackColor = true;
			// 
			// ContextMenuStrip_ListView
			// 
			this.ContextMenuStrip_ListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyUserDisplaynameToolStripMenuItem,
            this.copyUserIDToolStripMenuItem,
            this.toolStripSeparator1,
            this.copyAvatarNameToolStripMenuItem,
            this.copyAvatarDescriptionToolStripMenuItem,
            this.copyAvatarIDToolStripMenuItem,
            this.toolStripSeparator2,
            this.copyAuthorDisplaynameToolStripMenuItem,
            this.copyAuthorIDToolStripMenuItem,
            this.toolStripSeparator3,
            this.copyAssetURLToolStripMenuItem});
			this.ContextMenuStrip_ListView.Name = "ContextMenuStrip_ListView";
			this.ContextMenuStrip_ListView.ShowImageMargin = false;
			this.ContextMenuStrip_ListView.Size = new System.Drawing.Size(196, 198);
			this.ContextMenuStrip_ListView.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_ListView_Opening);
			// 
			// copyUserDisplaynameToolStripMenuItem
			// 
			this.copyUserDisplaynameToolStripMenuItem.Name = "copyUserDisplaynameToolStripMenuItem";
			this.copyUserDisplaynameToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyUserDisplaynameToolStripMenuItem.Text = "Copy User\'s displayname";
			this.copyUserDisplaynameToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// copyUserIDToolStripMenuItem
			// 
			this.copyUserIDToolStripMenuItem.Name = "copyUserIDToolStripMenuItem";
			this.copyUserIDToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyUserIDToolStripMenuItem.Text = "Copy User\'s ID";
			this.copyUserIDToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			// 
			// copyAvatarNameToolStripMenuItem
			// 
			this.copyAvatarNameToolStripMenuItem.Name = "copyAvatarNameToolStripMenuItem";
			this.copyAvatarNameToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyAvatarNameToolStripMenuItem.Text = "Copy Avatar name";
			this.copyAvatarNameToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// copyAvatarDescriptionToolStripMenuItem
			// 
			this.copyAvatarDescriptionToolStripMenuItem.Name = "copyAvatarDescriptionToolStripMenuItem";
			this.copyAvatarDescriptionToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyAvatarDescriptionToolStripMenuItem.Text = "Copy Avatar description";
			this.copyAvatarDescriptionToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// copyAvatarIDToolStripMenuItem
			// 
			this.copyAvatarIDToolStripMenuItem.Name = "copyAvatarIDToolStripMenuItem";
			this.copyAvatarIDToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyAvatarIDToolStripMenuItem.Text = "Copy Avatar ID";
			this.copyAvatarIDToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
			// 
			// copyAuthorDisplaynameToolStripMenuItem
			// 
			this.copyAuthorDisplaynameToolStripMenuItem.Name = "copyAuthorDisplaynameToolStripMenuItem";
			this.copyAuthorDisplaynameToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyAuthorDisplaynameToolStripMenuItem.Text = "Copy Author\'s displayname";
			this.copyAuthorDisplaynameToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// copyAuthorIDToolStripMenuItem
			// 
			this.copyAuthorIDToolStripMenuItem.Name = "copyAuthorIDToolStripMenuItem";
			this.copyAuthorIDToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyAuthorIDToolStripMenuItem.Text = "Copy Author\'s ID";
			this.copyAuthorIDToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(192, 6);
			// 
			// copyAssetURLToolStripMenuItem
			// 
			this.copyAssetURLToolStripMenuItem.Name = "copyAssetURLToolStripMenuItem";
			this.copyAssetURLToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.copyAssetURLToolStripMenuItem.Text = "Copy Asset URL";
			// 
			// MainListView
			// 
			this.MainListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderUser,
            this.ColumnHeaderUserId,
            this.ColumnHeaderPlatform,
            this.ColumnHeaderAvatarName,
            this.ColumnHeaderAvatarDescription,
            this.ColumnHeaderAvatarId,
            this.ColumnHeaderAvatarReleaseStatus,
            this.ColumnHeaderAvatarUploaded,
            this.ColumnHeaderAvatarAuthor,
            this.ColumnHeaderAvatarAuthorId,
            this.ColumnHeaderAvatarUrl});
			this.MainListView.ContextMenuStrip = this.ContextMenuStrip_ListView;
			this.MainListView.FullRowSelect = true;
			this.MainListView.HideSelection = false;
			this.MainListView.Location = new System.Drawing.Point(12, 73);
			this.MainListView.MultiSelect = false;
			this.MainListView.Name = "MainListView";
			this.MainListView.Size = new System.Drawing.Size(776, 365);
			this.MainListView.TabIndex = 0;
			this.MainListView.UseCompatibleStateImageBehavior = false;
			this.MainListView.View = System.Windows.Forms.View.Details;
			// 
			// ColumnHeaderUser
			// 
			this.ColumnHeaderUser.Text = "User";
			this.ColumnHeaderUser.Width = 180;
			// 
			// ColumnHeaderUserId
			// 
			this.ColumnHeaderUserId.Text = "ID";
			this.ColumnHeaderUserId.Width = 250;
			// 
			// ColumnHeaderAvatarName
			// 
			this.ColumnHeaderAvatarName.Text = "Avatar";
			this.ColumnHeaderAvatarName.Width = 180;
			// 
			// ColumnHeaderAvatarDescription
			// 
			this.ColumnHeaderAvatarDescription.Text = "Description";
			this.ColumnHeaderAvatarDescription.Width = 180;
			// 
			// ColumnHeaderAvatarId
			// 
			this.ColumnHeaderAvatarId.Text = "ID";
			this.ColumnHeaderAvatarId.Width = 250;
			// 
			// ColumnHeaderAvatarReleaseStatus
			// 
			this.ColumnHeaderAvatarReleaseStatus.Text = "Release Status";
			// 
			// ColumnHeaderAvatarUploaded
			// 
			this.ColumnHeaderAvatarUploaded.Text = "Uploaded";
			this.ColumnHeaderAvatarUploaded.Width = 150;
			// 
			// ColumnHeaderAvatarAuthor
			// 
			this.ColumnHeaderAvatarAuthor.Text = "Author";
			this.ColumnHeaderAvatarAuthor.Width = 180;
			// 
			// ColumnHeaderAvatarAuthorId
			// 
			this.ColumnHeaderAvatarAuthorId.Text = "ID";
			this.ColumnHeaderAvatarAuthorId.Width = 250;
			// 
			// ColumnHeaderAvatarUrl
			// 
			this.ColumnHeaderAvatarUrl.Text = "URL";
			// 
			// CheckBox_LogRawPackets
			// 
			this.CheckBox_LogRawPackets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CheckBox_LogRawPackets.AutoSize = true;
			this.CheckBox_LogRawPackets.Location = new System.Drawing.Point(488, 50);
			this.CheckBox_LogRawPackets.Name = "CheckBox_LogRawPackets";
			this.CheckBox_LogRawPackets.Size = new System.Drawing.Size(111, 17);
			this.CheckBox_LogRawPackets.TabIndex = 7;
			this.CheckBox_LogRawPackets.Text = "Log Raw Packets";
			this.CheckBox_LogRawPackets.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.CheckBox_LogRawPackets);
			this.Controls.Add(this.CheckBox_SkipDuplicates);
			this.Controls.Add(this.CheckBox_AutoScroll);
			this.Controls.Add(this.Button_Clear);
			this.Controls.Add(this.Label_Interface);
			this.Controls.Add(this.ComboBox_Interface);
			this.Controls.Add(this.Button_StartStop);
			this.Controls.Add(this.MainListView);
			this.DoubleBuffered = true;
			this.Name = "MainForm";
			this.Text = "SniffAvtr";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.ContextMenuStrip_ListView.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ListViewEx MainListView;
		private System.Windows.Forms.Button Button_StartStop;
		private System.Windows.Forms.ComboBox ComboBox_Interface;
		private System.Windows.Forms.Label Label_Interface;
		private System.Windows.Forms.ColumnHeader ColumnHeaderUser;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarName;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarDescription;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarId;
		private System.Windows.Forms.ColumnHeader ColumnHeaderUserId;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarAuthor;
		private System.Windows.Forms.Button Button_Clear;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarAuthorId;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarUrl;
		private System.Windows.Forms.Timer UpdateTimer;
		private System.Windows.Forms.CheckBox CheckBox_AutoScroll;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarUploaded;
		private System.Windows.Forms.CheckBox CheckBox_SkipDuplicates;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_ListView;
		private System.Windows.Forms.ToolStripMenuItem copyUserDisplaynameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyUserIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem copyAvatarNameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyAvatarDescriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyAvatarIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem copyAuthorDisplaynameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyAuthorIDToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem copyAssetURLToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBox_LogRawPackets;
		private System.Windows.Forms.ColumnHeader ColumnHeaderAvatarReleaseStatus;
		private System.Windows.Forms.ColumnHeader ColumnHeaderPlatform;
	}
}

