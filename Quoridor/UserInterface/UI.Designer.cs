namespace Quoridor.UserInterface
{
	partial class UI
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
			this.Placeholder = new System.Windows.Forms.Panel();
			this.UpButton = new System.Windows.Forms.Button();
			this.LeftButton = new System.Windows.Forms.Button();
			this.RightButton = new System.Windows.Forms.Button();
			this.DownButton = new System.Windows.Forms.Button();
			this.SetFenceButton = new System.Windows.Forms.Button();
			this.RotateFenceButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Placeholder
			// 
			this.Placeholder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Placeholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Placeholder.Location = new System.Drawing.Point(12, 7);
			this.Placeholder.Name = "Placeholder";
			this.Placeholder.Size = new System.Drawing.Size(500, 500);
			this.Placeholder.TabIndex = 0;
			this.Placeholder.Visible = false;
			// 
			// UpButton
			// 
			this.UpButton.Enabled = false;
			this.UpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.UpButton.Location = new System.Drawing.Point(273, 513);
			this.UpButton.Name = "UpButton";
			this.UpButton.Size = new System.Drawing.Size(61, 23);
			this.UpButton.TabIndex = 1;
			this.UpButton.TabStop = false;
			this.UpButton.Text = "^ UP ^";
			this.UpButton.UseVisualStyleBackColor = true;
			this.UpButton.Click += new System.EventHandler(this.CursorButtonClick);
			// 
			// LeftButton
			// 
			this.LeftButton.Enabled = false;
			this.LeftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LeftButton.Location = new System.Drawing.Point(204, 542);
			this.LeftButton.Name = "LeftButton";
			this.LeftButton.Size = new System.Drawing.Size(60, 23);
			this.LeftButton.TabIndex = 1;
			this.LeftButton.TabStop = false;
			this.LeftButton.Text = "< Left <";
			this.LeftButton.UseVisualStyleBackColor = true;
			this.LeftButton.Click += new System.EventHandler(this.CursorButtonClick);
			// 
			// RightButton
			// 
			this.RightButton.Enabled = false;
			this.RightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RightButton.Location = new System.Drawing.Point(340, 542);
			this.RightButton.Name = "RightButton";
			this.RightButton.Size = new System.Drawing.Size(62, 23);
			this.RightButton.TabIndex = 1;
			this.RightButton.TabStop = false;
			this.RightButton.Text = "> Right >";
			this.RightButton.UseVisualStyleBackColor = true;
			this.RightButton.Click += new System.EventHandler(this.CursorButtonClick);
			// 
			// DownButton
			// 
			this.DownButton.Enabled = false;
			this.DownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DownButton.Location = new System.Drawing.Point(272, 571);
			this.DownButton.Name = "DownButton";
			this.DownButton.Size = new System.Drawing.Size(62, 23);
			this.DownButton.TabIndex = 1;
			this.DownButton.TabStop = false;
			this.DownButton.Text = "v Down v";
			this.DownButton.UseVisualStyleBackColor = true;
			this.DownButton.Click += new System.EventHandler(this.CursorButtonClick);
			// 
			// SetFenceButton
			// 
			this.SetFenceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SetFenceButton.Location = new System.Drawing.Point(12, 514);
			this.SetFenceButton.Name = "SetFenceButton";
			this.SetFenceButton.Size = new System.Drawing.Size(86, 23);
			this.SetFenceButton.TabIndex = 1;
			this.SetFenceButton.TabStop = false;
			this.SetFenceButton.Text = "Set Fence";
			this.SetFenceButton.UseVisualStyleBackColor = true;
			this.SetFenceButton.Click += new System.EventHandler(this.SetFenceButton_Click);
			// 
			// RotateFenceButton
			// 
			this.RotateFenceButton.Enabled = false;
			this.RotateFenceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RotateFenceButton.Location = new System.Drawing.Point(273, 542);
			this.RotateFenceButton.Name = "RotateFenceButton";
			this.RotateFenceButton.Size = new System.Drawing.Size(61, 23);
			this.RotateFenceButton.TabIndex = 1;
			this.RotateFenceButton.TabStop = false;
			this.RotateFenceButton.Text = "Rotate Fence";
			this.RotateFenceButton.UseVisualStyleBackColor = true;
			this.RotateFenceButton.Click += new System.EventHandler(this.RotateFenceButton_Click);
			// 
			// UI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(526, 602);
			this.Controls.Add(this.DownButton);
			this.Controls.Add(this.RightButton);
			this.Controls.Add(this.RotateFenceButton);
			this.Controls.Add(this.SetFenceButton);
			this.Controls.Add(this.LeftButton);
			this.Controls.Add(this.UpButton);
			this.Controls.Add(this.Placeholder);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.KeyPreview = true;
			this.Name = "UI";
			this.ShowIcon = false;
			this.Text = "Quoridor";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UI_Paint);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UI_KeyUp);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UI_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel Placeholder;
		private System.Windows.Forms.Button UpButton;
		private System.Windows.Forms.Button LeftButton;
		private System.Windows.Forms.Button RightButton;
		private System.Windows.Forms.Button DownButton;
		private System.Windows.Forms.Button SetFenceButton;
		private System.Windows.Forms.Button RotateFenceButton;
	}
}

