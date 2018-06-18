using Face.resources;
using FaceCompareBase;
using FaceCompareThread;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo.UserControls
{
	public class FaceCompareUserControl : UserControl
	{
		private static readonly object _obj = new object();
		private readonly ResultInfo _recognition;
		private static string Similar = "相似度";
		private static string PersonName = "姓名";
		private static string ChannelName = "通道1";
		private static PersonType _personType;
		private IContainer components;
		private Label label1;
		private PictureBox pictureBox1;
		private Label label2;
		private Label label3;
		private PictureBox pictureBox2;
		public FaceCompareUserControl(ResultInfo recognition, string Channel)
		{
			if (LanguageSet.Resource != null)
			{
				FaceCompareUserControl.UpDataMainFormMenuLanguage(LanguageSet.Resource);
				uint num = <PrivateImplementationDetails>.ComputeStringHash(Channel);
				if (num <= 119101069u)
				{
					if (num <= 85545831u)
					{
						if (num != 1657736u)
						{
							if (num != 85545831u)
							{
								goto IL_14A;
							}
							if (!(Channel == "Channel 1"))
							{
								goto IL_14A;
							}
						}
						else
						{
							if (!(Channel == "Channel 4"))
							{
								goto IL_14A;
							}
							goto IL_138;
						}
					}
					else
					{
						if (num != 102323450u)
						{
							if (num != 119101069u)
							{
								goto IL_14A;
							}
							if (!(Channel == "Channel 3"))
							{
								goto IL_14A;
							}
							goto IL_12C;
						}
						else
						{
							if (!(Channel == "Channel 2"))
							{
								goto IL_14A;
							}
							goto IL_120;
						}
					}
				}
				else
				{
					if (num <= 1513249643u)
					{
						if (num != 1496472024u)
						{
							if (num != 1513249643u)
							{
								goto IL_14A;
							}
							if (!(Channel == "通道3"))
							{
								goto IL_14A;
							}
							goto IL_12C;
						}
						else
						{
							if (!(Channel == "通道2"))
							{
								goto IL_14A;
							}
							goto IL_120;
						}
					}
					else
					{
						if (num != 1546804881u)
						{
							if (num != 1597137738u)
							{
								goto IL_14A;
							}
							if (!(Channel == "通道4"))
							{
								goto IL_14A;
							}
							goto IL_138;
						}
						else
						{
							if (!(Channel == "通道1"))
							{
								goto IL_14A;
							}
						}
					}
				}
				FaceCompareUserControl.ChannelName = "C.1";
				goto IL_14A;
				IL_120:
				FaceCompareUserControl.ChannelName = "C.2";
				goto IL_14A;
				IL_12C:
				FaceCompareUserControl.ChannelName = "C.3";
				goto IL_14A;
				IL_138:
				FaceCompareUserControl.ChannelName = "C.4";
			}
			else
			{
				FaceCompareUserControl.ChannelName = Channel;
			}
			IL_14A:
			try
			{
				this.InitializeComponent();
				object obj = FaceCompareUserControl._obj;
				lock (obj)
				{
					this._recognition = recognition;
					FaceCompareUserControl._personType = this._recognition.FaceTemplate.PersonType;
					this.label1.Text = string.Format("{0}：{1}", FaceCompareUserControl.Similar, this._recognition.Score.ToString("P"));
					this.label2.Text = string.Format("{0}：{1}", FaceCompareUserControl.PersonName, this._recognition.FaceTemplate.PersonName);
					this.label3.Text = string.Format("({0} {1})", FaceCompareUserControl.ChannelName, DateTime.Now.ToString("HH:mm"));
					this.pictureBox1.Image = FaceImageFormat.ByteToBitmap(Convert.FromBase64String(this._recognition.FaceTemplate.ImageLocation));
					this.pictureBox2.Image = FaceImageFormat.ByteToBitmap(this._recognition.FaceImage);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		private static void UpDataMainFormMenuLanguage(ResourceManager rm)
		{
			FaceCompareUserControl.Similar = rm.GetString("Similar");
			FaceCompareUserControl.PersonName = rm.GetString("PersonName");
		}
		private void FaceCompareUserControl_Paint(object sender, PaintEventArgs e)
		{
			if (FaceCompareUserControl._personType == PersonType.Black)
			{
				ControlPaint.DrawBorder(e.Graphics, base.ClientRectangle, Color.Red, 1, ButtonBorderStyle.Solid, Color.Red, 1, ButtonBorderStyle.Solid, Color.Red, 1, ButtonBorderStyle.Solid, Color.Red, 1, ButtonBorderStyle.Solid);
				return;
			}
			ControlPaint.DrawBorder(e.Graphics, base.ClientRectangle, Color.Gray, 1, ButtonBorderStyle.Solid, Color.Gray, 1, ButtonBorderStyle.Solid, Color.Gray, 1, ButtonBorderStyle.Solid, Color.Gray, 1, ButtonBorderStyle.Solid);
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
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.pictureBox2 = new PictureBox();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.label1.ForeColor = Color.Lime;
			this.label1.Location = new Point(2, 135);
			this.label1.Name = "label1";
			this.label1.Size = new Size(118, 19);
			this.label1.TabIndex = 3;
			this.label1.Text = "相似度：76.33%";
			this.label2.AutoSize = true;
			this.label2.Font = new Font("幼圆", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.label2.ForeColor = Color.Silver;
			this.label2.Location = new Point(3, 162);
			this.label2.Name = "label2";
			this.label2.Size = new Size(86, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "张三  11：30";
			this.label3.AutoSize = true;
			this.label3.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.label3.ForeColor = Color.DarkGray;
			this.label3.Location = new Point(118, 136);
			this.label3.Name = "label3";
			this.label3.Size = new Size(91, 17);
			this.label3.TabIndex = 4;
			this.label3.Text = "(通道2 18：20)";
			this.pictureBox2.BorderStyle = BorderStyle.FixedSingle;
			this.pictureBox2.Location = new Point(108, 5);
			this.pictureBox2.Margin = new Padding(3, 4, 3, 4);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(100, 120);
			this.pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 5;
			this.pictureBox2.TabStop = false;
			this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
			this.pictureBox1.Location = new Point(4, 6);
			this.pictureBox1.Margin = new Padding(3, 4, 3, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(100, 120);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(7f, 17f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(45, 45, 48);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.pictureBox1);
			this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			base.Margin = new Padding(3, 4, 3, 4);
			base.Name = "FaceCompareUserControl";
			base.Size = new Size(215, 188);
			base.Paint += new PaintEventHandler(this.FaceCompareUserControl_Paint);
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
