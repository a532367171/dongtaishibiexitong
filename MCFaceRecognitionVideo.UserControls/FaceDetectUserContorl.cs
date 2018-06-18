using Face.resources;
using FaceCompareBase;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo.UserControls
{
	public class FaceDetectUserContorl : UserControl
	{
		private static string ChannelName = "通道1";
		private IContainer components;
		private PictureBox pictureBox1;
		private Label label1;
		private Bitmap image2
		{
			get;
			set;
		}
		public FaceDetectUserContorl(byte[] image, string Channel)
		{
			if (LanguageSet.Resource != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(Channel);
				if (num <= 119101069u)
				{
					if (num <= 85545831u)
					{
						if (num != 1657736u)
						{
							if (num != 85545831u)
							{
								goto IL_140;
							}
							if (!(Channel == "Channel 1"))
							{
								goto IL_140;
							}
						}
						else
						{
							if (!(Channel == "Channel 4"))
							{
								goto IL_140;
							}
							goto IL_12E;
						}
					}
					else
					{
						if (num != 102323450u)
						{
							if (num != 119101069u)
							{
								goto IL_140;
							}
							if (!(Channel == "Channel 3"))
							{
								goto IL_140;
							}
							goto IL_122;
						}
						else
						{
							if (!(Channel == "Channel 2"))
							{
								goto IL_140;
							}
							goto IL_116;
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
								goto IL_140;
							}
							if (!(Channel == "通道3"))
							{
								goto IL_140;
							}
							goto IL_122;
						}
						else
						{
							if (!(Channel == "通道2"))
							{
								goto IL_140;
							}
							goto IL_116;
						}
					}
					else
					{
						if (num != 1546804881u)
						{
							if (num != 1597137738u)
							{
								goto IL_140;
							}
							if (!(Channel == "通道4"))
							{
								goto IL_140;
							}
							goto IL_12E;
						}
						else
						{
							if (!(Channel == "通道1"))
							{
								goto IL_140;
							}
						}
					}
				}
				FaceDetectUserContorl.ChannelName = "C.1";
				goto IL_140;
				IL_116:
				FaceDetectUserContorl.ChannelName = "C.2";
				goto IL_140;
				IL_122:
				FaceDetectUserContorl.ChannelName = "C.3";
				goto IL_140;
				IL_12E:
				FaceDetectUserContorl.ChannelName = "C.4";
			}
			else
			{
				FaceDetectUserContorl.ChannelName = Channel;
			}
			IL_140:
			this.InitializeComponent();
			this.pictureBox1.Image = FaceImageFormat.ByteToBitmap(image);
			this.label1.Text = string.Format("（{0} {1}）", FaceDetectUserContorl.ChannelName, DateTime.Now.ToString("HH:mm:ss"));
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
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new Font("微软雅黑", 7.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.label1.ForeColor = Color.Gray;
			this.label1.Location = new Point(5, 124);
			this.label1.Name = "label1";
			this.label1.Size = new Size(97, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "（通道1 11:30:25）";
			this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
			this.pictureBox1.Location = new Point(1, 0);
			this.pictureBox1.Margin = new Padding(3, 4, 3, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(104, 120);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(7f, 17f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(45, 45, 48);
			base.BorderStyle = BorderStyle.FixedSingle;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.pictureBox1);
			this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			base.Margin = new Padding(3, 4, 3, 4);
			base.Name = "FaceDetectUserContorl";
			base.Size = new Size(108, 142);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
