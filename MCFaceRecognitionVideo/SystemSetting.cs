using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Face.resources;
using MCFaceRecognitionVideo.UnitBase;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	public class SystemSetting : XtraForm
	{
		private Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		private IContainer components;
		private Panel panel1;
		private ImageList imageList1;
		private Label lbl_IsSaveImage;
		private Label lbl_PhotoSavePath;
		private ComboBoxEdit cbx_IsCapturePhotos;
		private SimpleButton btn_Path;
		private TextEdit txt_PhotSavePath;
		private Label lbl_HomePageStyle;
		private PictureEdit pictureEdit2;
		private PictureEdit pictureEdit1;
		private RadioButton radio_sytle2;
		private RadioButton radio_Style;
		private ComboBoxEdit cbx_IsPlaySound;
		private Label lbl_IsRunSound;
		private SimpleButton btn_save;
		private TextEdit txt_systemName;
		private Label lbl_SoftwareName;
		public SystemSetting()
		{
			this.InitializeComponent();
			if (LanguageSet.Resource != null)
			{
				this.UpDataMainFormMenuLanguage(LanguageSet.Resource);
			}
			this.cbx_IsCapturePhotos.Text = XMLHelper.getXmlValue("PhotoSetting", "IsCapturePhotos");
			this.txt_PhotSavePath.Text = XMLHelper.getXmlValue("PhotoSetting", "PhotoSavePath");
			this.txt_systemName.Text = ConfigurationManager.AppSettings["MainTitle"];
			this.cbx_IsPlaySound.Text = (ConfigurationManager.AppSettings["IsPlaySound"].Equals("true") ? UnitField.Yes : UnitField.No);
			if (ConfigurationManager.AppSettings["MainForm"].Equals("VideoForm"))
			{
				this.radio_Style.Checked = true;
				this.radio_sytle2.Checked = false;
				return;
			}
			this.radio_sytle2.Checked = true;
			this.radio_Style.Checked = false;
		}
		private void tsb_close_Click(object sender, EventArgs e)
		{
			base.Close();
		}
		private void btn_Path_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.ShowDialog();
			this.txt_PhotSavePath.Text = folderBrowserDialog.SelectedPath;
		}
		private void btn_save_Click(object sender, EventArgs e)
		{
			XMLHelper.setXmlValue("PhotoSetting", "IsCapturePhotos", this.cbx_IsCapturePhotos.Text);
			XMLHelper.setXmlValue("PhotoSetting", "PhotoSavePath", this.txt_PhotSavePath.Text);
			this.cfa.AppSettings.Settings["MainTitle"].Value = this.txt_systemName.Text;
			this.cfa.AppSettings.Settings["IsPlaySound"].Value = (this.cbx_IsPlaySound.Text.Equals(UnitField.Yes) ? "true" : "false");
			this.cfa.AppSettings.Settings["MainForm"].Value = (this.radio_Style.Checked ? "VideoForm" : "VideoForm2");
			this.cfa.Save();
			if (XtraMessageBox.Show(UnitField.SystemSave, UnitField.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Process.Start(Assembly.GetExecutingAssembly().Location);
				base.Close();
				Process.GetCurrentProcess().Kill();
			}
		}
		private void UpDataMainFormMenuLanguage(ResourceManager rm)
		{
			this.cbx_IsCapturePhotos.Properties.Items[0] = UnitField.Yes;
			this.cbx_IsCapturePhotos.Properties.Items[1] = UnitField.No;
			this.lbl_IsSaveImage.Text = UnitField.SaveImage;
			this.lbl_PhotoSavePath.Text = UnitField.PhotoSavePath;
			this.lbl_IsRunSound.Text = UnitField.IsRunSound;
			this.lbl_SoftwareName.Text = UnitField.SoftwareName;
			this.lbl_HomePageStyle.Text = UnitField.HomePageStyle;
			this.radio_Style.Text = UnitField.Style1;
			this.radio_sytle2.Text = UnitField.Style2;
			this.btn_save.Text = UnitField.Save;
			this.btn_Path.Text = UnitField.FolderPath;
			this.cbx_IsPlaySound.Properties.Items[0] = UnitField.Yes;
			this.cbx_IsPlaySound.Properties.Items[1] = UnitField.No;
			this.Text = UnitField.SystemSettings;
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SystemSetting));
			this.imageList1 = new ImageList(this.components);
			this.panel1 = new Panel();
			this.txt_systemName = new TextEdit();
			this.lbl_SoftwareName = new Label();
			this.btn_save = new SimpleButton();
			this.cbx_IsPlaySound = new ComboBoxEdit();
			this.lbl_IsRunSound = new Label();
			this.radio_sytle2 = new RadioButton();
			this.radio_Style = new RadioButton();
			this.pictureEdit2 = new PictureEdit();
			this.pictureEdit1 = new PictureEdit();
			this.lbl_HomePageStyle = new Label();
			this.btn_Path = new SimpleButton();
			this.txt_PhotSavePath = new TextEdit();
			this.cbx_IsCapturePhotos = new ComboBoxEdit();
			this.lbl_IsSaveImage = new Label();
			this.lbl_PhotoSavePath = new Label();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.txt_systemName.Properties).BeginInit();
			((ISupportInitialize)this.cbx_IsPlaySound.Properties).BeginInit();
			((ISupportInitialize)this.pictureEdit2.Properties).BeginInit();
			((ISupportInitialize)this.pictureEdit1.Properties).BeginInit();
			((ISupportInitialize)this.txt_PhotSavePath.Properties).BeginInit();
			((ISupportInitialize)this.cbx_IsCapturePhotos.Properties).BeginInit();
			base.SuspendLayout();
			this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
			this.imageList1.TransparentColor = Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "video.png");
			this.panel1.Controls.Add(this.txt_systemName);
			this.panel1.Controls.Add(this.lbl_SoftwareName);
			this.panel1.Controls.Add(this.btn_save);
			this.panel1.Controls.Add(this.cbx_IsPlaySound);
			this.panel1.Controls.Add(this.lbl_IsRunSound);
			this.panel1.Controls.Add(this.radio_sytle2);
			this.panel1.Controls.Add(this.radio_Style);
			this.panel1.Controls.Add(this.pictureEdit2);
			this.panel1.Controls.Add(this.pictureEdit1);
			this.panel1.Controls.Add(this.lbl_HomePageStyle);
			this.panel1.Controls.Add(this.btn_Path);
			this.panel1.Controls.Add(this.txt_PhotSavePath);
			this.panel1.Controls.Add(this.cbx_IsCapturePhotos);
			this.panel1.Controls.Add(this.lbl_IsSaveImage);
			this.panel1.Controls.Add(this.lbl_PhotoSavePath);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(1, 1);
			this.panel1.Margin = new Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(798, 598);
			this.panel1.TabIndex = 3;
			this.txt_systemName.EditValue = "人脸识别监控预警系统 服务电话：400 825 3771";
			this.txt_systemName.Location = new Point(164, 165);
			this.txt_systemName.Name = "txt_systemName";
			this.txt_systemName.Size = new Size(438, 20);
			this.txt_systemName.TabIndex = 28;
			this.lbl_SoftwareName.AutoSize = true;
			this.lbl_SoftwareName.ForeColor = Color.White;
			this.lbl_SoftwareName.Location = new Point(22, 164);
			this.lbl_SoftwareName.Name = "lbl_SoftwareName";
			this.lbl_SoftwareName.Size = new Size(67, 14);
			this.lbl_SoftwareName.TabIndex = 27;
			this.lbl_SoftwareName.Text = "软件名称：";
			this.btn_save.Image = (Image)componentResourceManager.GetObject("btn_save.Image");
			this.btn_save.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_save.Location = new Point(319, 528);
			this.btn_save.Name = "btn_save";
			this.btn_save.Size = new Size(127, 48);
			this.btn_save.TabIndex = 26;
			this.btn_save.Text = "保 存";
			this.btn_save.Click += new EventHandler(this.btn_save_Click);
			this.cbx_IsPlaySound.EditValue = "是";
			this.cbx_IsPlaySound.Location = new Point(164, 233);
			this.cbx_IsPlaySound.Name = "cbx_IsPlaySound";
			this.cbx_IsPlaySound.Properties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.cbx_IsPlaySound.Properties.Items.AddRange(new object[]
			{
				"是",
				"否"
			});
			this.cbx_IsPlaySound.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			this.cbx_IsPlaySound.Size = new Size(149, 20);
			this.cbx_IsPlaySound.TabIndex = 25;
			this.lbl_IsRunSound.AutoSize = true;
			this.lbl_IsRunSound.ForeColor = Color.White;
			this.lbl_IsRunSound.Location = new Point(22, 232);
			this.lbl_IsRunSound.Name = "lbl_IsRunSound";
			this.lbl_IsRunSound.Size = new Size(91, 14);
			this.lbl_IsRunSound.TabIndex = 24;
			this.lbl_IsRunSound.Text = "开启报警声音：";
			this.radio_sytle2.AutoSize = true;
			this.radio_sytle2.Location = new Point(433, 303);
			this.radio_sytle2.Name = "radio_sytle2";
			this.radio_sytle2.Size = new Size(56, 18);
			this.radio_sytle2.TabIndex = 23;
			this.radio_sytle2.Text = "样式2";
			this.radio_sytle2.UseVisualStyleBackColor = true;
			this.radio_Style.AutoSize = true;
			this.radio_Style.Checked = true;
			this.radio_Style.Location = new Point(167, 305);
			this.radio_Style.Name = "radio_Style";
			this.radio_Style.Size = new Size(56, 18);
			this.radio_Style.TabIndex = 22;
			this.radio_Style.TabStop = true;
			this.radio_Style.Text = "样式1";
			this.radio_Style.UseVisualStyleBackColor = true;
			this.pictureEdit2.EditValue = componentResourceManager.GetObject("pictureEdit2.EditValue");
			this.pictureEdit2.Location = new Point(433, 331);
			this.pictureEdit2.Name = "pictureEdit2";
			this.pictureEdit2.Properties.BorderStyle = BorderStyles.Simple;
			this.pictureEdit2.Properties.ReadOnly = true;
			this.pictureEdit2.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
			this.pictureEdit2.Properties.ShowMenu = false;
			this.pictureEdit2.Properties.SizeMode = PictureSizeMode.Stretch;
			this.pictureEdit2.Size = new Size(234, 160);
			this.pictureEdit2.TabIndex = 21;
			this.pictureEdit1.EditValue = componentResourceManager.GetObject("pictureEdit1.EditValue");
			this.pictureEdit1.Location = new Point(167, 331);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Properties.BorderStyle = BorderStyles.Simple;
			this.pictureEdit1.Properties.InitialImage = (Image)componentResourceManager.GetObject("pictureEdit1.Properties.InitialImage");
			this.pictureEdit1.Properties.ReadOnly = true;
			this.pictureEdit1.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
			this.pictureEdit1.Properties.ShowMenu = false;
			this.pictureEdit1.Properties.SizeMode = PictureSizeMode.Stretch;
			this.pictureEdit1.Size = new Size(234, 160);
			this.pictureEdit1.TabIndex = 20;
			this.lbl_HomePageStyle.AutoSize = true;
			this.lbl_HomePageStyle.ForeColor = Color.White;
			this.lbl_HomePageStyle.Location = new Point(22, 303);
			this.lbl_HomePageStyle.Name = "lbl_HomePageStyle";
			this.lbl_HomePageStyle.Size = new Size(91, 14);
			this.lbl_HomePageStyle.TabIndex = 18;
			this.lbl_HomePageStyle.Text = "首页显示样式：";
			this.btn_Path.Location = new Point(608, 94);
			this.btn_Path.Name = "btn_Path";
			this.btn_Path.Size = new Size(85, 26);
			this.btn_Path.TabIndex = 17;
			this.btn_Path.Text = "选择路径";
			this.btn_Path.Click += new EventHandler(this.btn_Path_Click);
			this.txt_PhotSavePath.EditValue = "D:\\CapturePhotos";
			this.txt_PhotSavePath.Location = new Point(164, 96);
			this.txt_PhotSavePath.Name = "txt_PhotSavePath";
			this.txt_PhotSavePath.Size = new Size(438, 20);
			this.txt_PhotSavePath.TabIndex = 16;
			this.cbx_IsCapturePhotos.EditValue = "是";
			this.cbx_IsCapturePhotos.Location = new Point(164, 32);
			this.cbx_IsCapturePhotos.Name = "cbx_IsCapturePhotos";
			this.cbx_IsCapturePhotos.Properties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.cbx_IsCapturePhotos.Properties.Items.AddRange(new object[]
			{
				"是",
				"否"
			});
			this.cbx_IsCapturePhotos.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			this.cbx_IsCapturePhotos.Size = new Size(149, 20);
			this.cbx_IsCapturePhotos.TabIndex = 15;
			this.lbl_IsSaveImage.AutoSize = true;
			this.lbl_IsSaveImage.ForeColor = Color.White;
			this.lbl_IsSaveImage.Location = new Point(22, 31);
			this.lbl_IsSaveImage.Name = "lbl_IsSaveImage";
			this.lbl_IsSaveImage.Size = new Size(115, 14);
			this.lbl_IsSaveImage.TabIndex = 11;
			this.lbl_IsSaveImage.Text = "是否保存抓拍照片：";
			this.lbl_PhotoSavePath.AutoSize = true;
			this.lbl_PhotoSavePath.ForeColor = Color.White;
			this.lbl_PhotoSavePath.Location = new Point(22, 97);
			this.lbl_PhotoSavePath.Name = "lbl_PhotoSavePath";
			this.lbl_PhotoSavePath.Size = new Size(115, 14);
			this.lbl_PhotoSavePath.TabIndex = 8;
			this.lbl_PhotoSavePath.Text = "抓拍照片存放路径：";
			base.AutoScaleMode = AutoScaleMode.None;
			base.ClientSize = new Size(800, 600);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			base.MaximizeBox = false;
			this.MaximumSize = new Size(816, 636);
			base.MinimizeBox = false;
			this.MinimumSize = new Size(816, 636);
			base.Name = "SystemSetting";
			base.Padding = new Padding(1);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "系统设置";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((ISupportInitialize)this.txt_systemName.Properties).EndInit();
			((ISupportInitialize)this.cbx_IsPlaySound.Properties).EndInit();
			((ISupportInitialize)this.pictureEdit2.Properties).EndInit();
			((ISupportInitialize)this.pictureEdit1.Properties).EndInit();
			((ISupportInitialize)this.txt_PhotSavePath.Properties).EndInit();
			((ISupportInitialize)this.cbx_IsCapturePhotos.Properties).EndInit();
			base.ResumeLayout(false);
		}
	}
}
