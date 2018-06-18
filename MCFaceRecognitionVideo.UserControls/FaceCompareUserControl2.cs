using Face.resources;
using FaceCompareBase;
using FaceCompareThread;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo.UserControls
{
	public class FaceCompareUserControl2 : UserControl
	{
		private readonly string StartupPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\UserPicture\\";
		private static readonly object _obj = new object();
		private readonly ResultInfo recognition;
		private IContainer components;
		private PictureBox pictureBox1;
		private Label label2;
		private Label label3;
		public FaceCompareUserControl2(ResultInfo _recognition, string Channel)
		{
			try
			{
				this.InitializeComponent();
				object obj = FaceCompareUserControl2._obj;
				lock (obj)
				{
					this.recognition = _recognition;
					this.label3.Text = string.Format("{0}：{1}", UnitField.PersonName, this.recognition.FaceTemplate.PersonName);
					this.label2.Text = string.Format("{0}： {1}", UnitField.Date, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
					this.pictureBox1.Image = FaceImageFormat.ByteToBitmap(Convert.FromBase64String(this.recognition.FaceTemplate.ImageLocation));
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.label2 = new Label();
			this.label3 = new Label();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label2.AutoSize = true;
			this.label2.Font = new Font("微软雅黑", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.label2.ForeColor = Color.Yellow;
			this.label2.Location = new Point(144, 95);
			this.label2.Name = "label2";
			this.label2.Size = new Size(179, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "时  间：2017年08月04日 17:55";
			this.label3.AutoSize = true;
			this.label3.Font = new Font("微软雅黑", 15f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.label3.ForeColor = Color.Yellow;
			this.label3.Location = new Point(144, 45);
			this.label3.Name = "label3";
			this.label3.Size = new Size(130, 27);
			this.label3.TabIndex = 4;
			this.label3.Text = "姓   名：张三";
			this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
			this.pictureBox1.Location = new Point(4, 6);
			this.pictureBox1.Margin = new Padding(3, 4, 3, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(130, 149);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(7f, 17f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(64, 64, 64);
			base.BorderStyle = BorderStyle.FixedSingle;
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.pictureBox1);
			this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			base.Margin = new Padding(3, 4, 3, 4);
			base.Name = "FaceCompareUserControl2";
			base.Size = new Size(406, 162);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
