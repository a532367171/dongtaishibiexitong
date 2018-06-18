using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using Face.resources;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	public class LoadingForm : SplashScreen
	{
		public enum SplashScreenCommand
		{
			Setinfo
		}
		private IContainer components;
		private LabelControl labelControl1;
		private LabelControl labelControl2;
		private PictureEdit pictureEdit1;
		private PictureEdit pictureEdit2;
		private ProgressBarControl marqueeProgressBarControl1;
		private LabelControl lbl_text;
		public LoadingForm()
		{
			this.InitializeComponent();
			if (LanguageSet.Resource != null)
			{
				this.lbl_text.Text = LanguageSet.Resource.GetString("FaceRecognitionSystem");
			}
		}
		public override void ProcessCommand(Enum cmd, object arg)
		{
			base.ProcessCommand(cmd, arg);
			if ((LoadingForm.SplashScreenCommand)cmd == LoadingForm.SplashScreenCommand.Setinfo)
			{
				Info info = (Info)arg;
				this.marqueeProgressBarControl1.Position = info.Pos;
				this.labelControl2.Text = info.LabelText;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LoadingForm));
			this.labelControl1 = new LabelControl();
			this.labelControl2 = new LabelControl();
			this.marqueeProgressBarControl1 = new ProgressBarControl();
			this.lbl_text = new LabelControl();
			this.pictureEdit2 = new PictureEdit();
			this.pictureEdit1 = new PictureEdit();
			((ISupportInitialize)this.marqueeProgressBarControl1.Properties).BeginInit();
			((ISupportInitialize)this.pictureEdit2.Properties).BeginInit();
			((ISupportInitialize)this.pictureEdit1.Properties).BeginInit();
			base.SuspendLayout();
			this.labelControl1.BorderStyle = BorderStyles.NoBorder;
			this.labelControl1.Location = new Point(23, 419);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new Size(131, 14);
			this.labelControl1.TabIndex = 6;
			this.labelControl1.Text = "Copyright © 2016-2017";
			this.labelControl2.Location = new Point(23, 287);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new Size(55, 14);
			this.labelControl2.TabIndex = 7;
			this.labelControl2.Text = "Starting...";
			this.marqueeProgressBarControl1.Location = new Point(23, 310);
			this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
			this.marqueeProgressBarControl1.Size = new Size(404, 11);
			this.marqueeProgressBarControl1.TabIndex = 5;
			this.lbl_text.Appearance.Font = new Font("幼圆", 15f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_text.Appearance.ForeColor = Color.Gold;
			this.lbl_text.Location = new Point(23, 357);
			this.lbl_text.Name = "lbl_text";
			this.lbl_text.Size = new Size(294, 20);
			this.lbl_text.TabIndex = 11;
			this.lbl_text.Text = "动态人脸识别黑白名单预警系统";
			this.pictureEdit2.EditValue = componentResourceManager.GetObject("pictureEdit2.EditValue");
			this.pictureEdit2.Location = new Point(12, 11);
			this.pictureEdit2.Name = "pictureEdit2";
			this.pictureEdit2.Properties.AllowFocused = false;
			this.pictureEdit2.Properties.Appearance.BackColor = Color.Transparent;
			this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
			this.pictureEdit2.Properties.BorderStyle = BorderStyles.NoBorder;
			this.pictureEdit2.Properties.ShowMenu = false;
			this.pictureEdit2.Properties.SizeMode = PictureSizeMode.Stretch;
			this.pictureEdit2.Size = new Size(426, 270);
			this.pictureEdit2.TabIndex = 9;
			this.pictureEdit1.EditValue = componentResourceManager.GetObject("pictureEdit1.EditValue");
			this.pictureEdit1.Location = new Point(299, 395);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Properties.AllowFocused = false;
			this.pictureEdit1.Properties.Appearance.BackColor = Color.Transparent;
			this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
			this.pictureEdit1.Properties.BorderStyle = BorderStyles.NoBorder;
			this.pictureEdit1.Properties.ShowMenu = false;
			this.pictureEdit1.Properties.SizeMode = PictureSizeMode.Stretch;
			this.pictureEdit1.Size = new Size(139, 50);
			this.pictureEdit1.TabIndex = 8;
			this.pictureEdit1.Visible = false;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(450, 457);
			base.Controls.Add(this.lbl_text);
			base.Controls.Add(this.pictureEdit2);
			base.Controls.Add(this.pictureEdit1);
			base.Controls.Add(this.labelControl2);
			base.Controls.Add(this.labelControl1);
			base.Controls.Add(this.marqueeProgressBarControl1);
			base.Name = "LoadingForm";
			this.Text = "Form1";
			((ISupportInitialize)this.marqueeProgressBarControl1.Properties).EndInit();
			((ISupportInitialize)this.pictureEdit2.Properties).EndInit();
			((ISupportInitialize)this.pictureEdit1.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
