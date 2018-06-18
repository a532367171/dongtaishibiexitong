using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using FaceCompareBase;
using FaceCompareThread;
using MC_DAL;
using MC_DAL.Entity;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo.UserControls
{
	public class FaceCompareUserControl3 : UserControl
	{
		private PersonService _personService;
		private IContainer components;
		private PictureBox pictureBox1;
		private PictureBox pictureBox2;
		private LabelControl labelControl1;
		private LabelControl labelControl2;
		private LabelControl labelControl3;
		private LabelControl labelControl4;
		private LabelControl labelControl5;
		private LabelControl lbl_Name;
		private LabelControl lbl_Number;
		private LabelControl lbl_sex;
		private LabelControl lbl_age;
		private LabelControl lbl_persontype;
		private LabelControl lbl_channl;
		private LabelControl labelControl7;
		private PanelControl panelControl1;
		private Label lbl_time;
		private GaugeControl gaugeControl1;
		private CircularGauge circularGauge1;
		private LabelComponent labelComponent1;
		private ArcScaleRangeBarComponent arcScaleRangeBarComponent1;
		private ArcScaleComponent arcScaleComponent1;
		private LabelComponent labelComponent2;
		public FaceCompareUserControl3()
		{
			this.InitializeComponent();
			this._personService = new PersonService();
			this.Clear();
		}
		private void labelControl2_Click(object sender, EventArgs e)
		{
		}
		private void lbl_Number_Click(object sender, EventArgs e)
		{
		}
		public new void Load(ResultInfo _recognition, string Channel)
		{
			Person person = this._personService.GetPerson(int.Parse(_recognition.FaceTemplate.PersonId));
			if (person != null)
			{
				this.lbl_Name.Text = person.Name;
				this.lbl_Number.Text = person.Number;
				this.lbl_age.Text = person.Age.ToString();
				this.lbl_channl.Text = Channel;
				this.lbl_sex.Text = ((person.Sex == 0L) ? "女" : "男");
				this.lbl_persontype.Text = person.tmp1;
				this.pictureBox1.Image = FaceImageFormat.ByteToBitmap(Convert.FromBase64String(_recognition.FaceTemplate.ImageLocation));
				this.pictureBox2.Image = FaceImageFormat.ByteToBitmap(_recognition.FaceImage);
				this.labelComponent1.set_Text(_recognition.Score.ToString("p"));
				this.arcScaleRangeBarComponent1.set_Value(new float?(_recognition.Score * 100f));
				this.lbl_time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			}
		}
		public void Clear()
		{
			this.lbl_Name.Text = string.Empty;
			this.lbl_Number.Text = string.Empty;
			this.lbl_age.Text = string.Empty;
			this.lbl_channl.Text = string.Empty;
			this.lbl_sex.Text = string.Empty;
			this.lbl_persontype.Text = string.Empty;
			this.pictureBox1.Image = null;
			this.pictureBox2.Image = null;
			this.labelComponent1.set_Text("0.00%");
			this.lbl_time.Text = string.Empty;
			this.arcScaleRangeBarComponent1.set_Value(new float?(0f));
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
			this.pictureBox2 = new PictureBox();
			this.pictureBox1 = new PictureBox();
			this.labelControl1 = new LabelControl();
			this.labelControl2 = new LabelControl();
			this.labelControl3 = new LabelControl();
			this.labelControl4 = new LabelControl();
			this.labelControl5 = new LabelControl();
			this.lbl_Name = new LabelControl();
			this.lbl_Number = new LabelControl();
			this.lbl_sex = new LabelControl();
			this.lbl_age = new LabelControl();
			this.lbl_persontype = new LabelControl();
			this.lbl_channl = new LabelControl();
			this.labelControl7 = new LabelControl();
			this.panelControl1 = new PanelControl();
			this.lbl_time = new Label();
			this.gaugeControl1 = new GaugeControl();
			this.circularGauge1 = new CircularGauge();
			this.labelComponent1 = new LabelComponent();
			this.labelComponent2 = new LabelComponent();
			this.arcScaleRangeBarComponent1 = new ArcScaleRangeBarComponent();
			this.arcScaleComponent1 = new ArcScaleComponent();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.panelControl1).BeginInit();
			this.panelControl1.SuspendLayout();
			this.circularGauge1.BeginInit();
			this.labelComponent1.BeginInit();
			this.labelComponent2.BeginInit();
			this.arcScaleRangeBarComponent1.BeginInit();
			this.arcScaleComponent1.BeginInit();
			base.SuspendLayout();
			this.pictureBox2.BackColor = Color.Black;
			this.pictureBox2.BorderStyle = BorderStyle.FixedSingle;
			this.pictureBox2.Location = new Point(217, 2);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(211, 226);
			this.pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 0;
			this.pictureBox2.TabStop = false;
			this.pictureBox1.BackColor = Color.Black;
			this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
			this.pictureBox1.Location = new Point(3, 2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(211, 226);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.labelControl1.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.labelControl1.Appearance.ForeColor = Color.Silver;
			this.labelControl1.Location = new Point(217, 16);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new Size(50, 19);
			this.labelControl1.TabIndex = 3;
			this.labelControl1.Text = "姓  名：";
			this.labelControl2.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.labelControl2.Appearance.ForeColor = Color.Silver;
			this.labelControl2.Location = new Point(217, 121);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new Size(50, 19);
			this.labelControl2.TabIndex = 3;
			this.labelControl2.Text = "编  号：";
			this.labelControl2.Click += new EventHandler(this.labelControl2_Click);
			this.labelControl3.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.labelControl3.Appearance.ForeColor = Color.Silver;
			this.labelControl3.Location = new Point(217, 51);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new Size(50, 19);
			this.labelControl3.TabIndex = 3;
			this.labelControl3.Text = "性  别：";
			this.labelControl4.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.labelControl4.Appearance.ForeColor = Color.Silver;
			this.labelControl4.Location = new Point(217, 156);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Size = new Size(50, 19);
			this.labelControl4.TabIndex = 3;
			this.labelControl4.Text = "年  龄：";
			this.labelControl5.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.labelControl5.Appearance.ForeColor = Color.Silver;
			this.labelControl5.Location = new Point(217, 86);
			this.labelControl5.Name = "labelControl5";
			this.labelControl5.Size = new Size(50, 19);
			this.labelControl5.TabIndex = 3;
			this.labelControl5.Text = "类  别：";
			this.lbl_Name.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_Name.Appearance.ForeColor = Color.Silver;
			this.lbl_Name.Location = new Point(286, 16);
			this.lbl_Name.Name = "lbl_Name";
			this.lbl_Name.Size = new Size(28, 19);
			this.lbl_Name.TabIndex = 4;
			this.lbl_Name.Text = "张三";
			this.lbl_Number.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_Number.Appearance.ForeColor = Color.Silver;
			this.lbl_Number.Location = new Point(286, 121);
			this.lbl_Number.Name = "lbl_Number";
			this.lbl_Number.Size = new Size(117, 19);
			this.lbl_Number.TabIndex = 4;
			this.lbl_Number.Text = "2220124555555";
			this.lbl_Number.Click += new EventHandler(this.lbl_Number_Click);
			this.lbl_sex.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_sex.Appearance.ForeColor = Color.Silver;
			this.lbl_sex.Location = new Point(286, 51);
			this.lbl_sex.Name = "lbl_sex";
			this.lbl_sex.Size = new Size(14, 19);
			this.lbl_sex.TabIndex = 4;
			this.lbl_sex.Text = "男";
			this.lbl_age.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_age.Appearance.ForeColor = Color.Silver;
			this.lbl_age.Location = new Point(286, 156);
			this.lbl_age.Name = "lbl_age";
			this.lbl_age.Size = new Size(18, 19);
			this.lbl_age.TabIndex = 4;
			this.lbl_age.Text = "19";
			this.lbl_persontype.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_persontype.Appearance.ForeColor = Color.Silver;
			this.lbl_persontype.Location = new Point(286, 86);
			this.lbl_persontype.Name = "lbl_persontype";
			this.lbl_persontype.Size = new Size(52, 19);
			this.lbl_persontype.TabIndex = 4;
			this.lbl_persontype.Text = "VIP客户";
			this.lbl_channl.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_channl.Appearance.ForeColor = Color.Silver;
			this.lbl_channl.Location = new Point(286, 191);
			this.lbl_channl.Name = "lbl_channl";
			this.lbl_channl.Size = new Size(37, 19);
			this.lbl_channl.TabIndex = 3;
			this.lbl_channl.Text = "通道1";
			this.labelControl7.Appearance.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.labelControl7.Appearance.ForeColor = Color.Silver;
			this.labelControl7.Location = new Point(217, 191);
			this.labelControl7.Name = "labelControl7";
			this.labelControl7.Size = new Size(70, 19);
			this.labelControl7.TabIndex = 4;
			this.labelControl7.Text = "识别通道：";
			this.panelControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.panelControl1.Appearance.BackColor = Color.Transparent;
			this.panelControl1.Appearance.Options.UseBackColor = true;
			this.panelControl1.BorderStyle = BorderStyles.Simple;
			this.panelControl1.Controls.Add(this.lbl_time);
			this.panelControl1.Controls.Add(this.gaugeControl1);
			this.panelControl1.Controls.Add(this.labelControl4);
			this.panelControl1.Controls.Add(this.labelControl7);
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.lbl_persontype);
			this.panelControl1.Controls.Add(this.labelControl2);
			this.panelControl1.Controls.Add(this.lbl_age);
			this.panelControl1.Controls.Add(this.labelControl3);
			this.panelControl1.Controls.Add(this.lbl_sex);
			this.panelControl1.Controls.Add(this.labelControl5);
			this.panelControl1.Controls.Add(this.lbl_Number);
			this.panelControl1.Controls.Add(this.lbl_Name);
			this.panelControl1.Controls.Add(this.lbl_channl);
			this.panelControl1.Location = new Point(4, 233);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new Size(424, 239);
			this.panelControl1.TabIndex = 5;
			this.lbl_time.AutoSize = true;
			this.lbl_time.Font = new Font("微软雅黑", 10.5f, FontStyle.Bold);
			this.lbl_time.ForeColor = Color.Silver;
			this.lbl_time.Location = new Point(15, 215);
			this.lbl_time.Name = "lbl_time";
			this.lbl_time.Size = new Size(159, 19);
			this.lbl_time.TabIndex = 6;
			this.lbl_time.Text = "2017-12-25 13:18:85";
			this.gaugeControl1.set_AutoLayout(false);
			this.gaugeControl1.BackColor = Color.Transparent;
			this.gaugeControl1.set_BorderStyle(BorderStyles.NoBorder);
			this.gaugeControl1.get_Gauges().AddRange(new IGauge[]
			{
				this.circularGauge1
			});
			this.gaugeControl1.Location = new Point(4, 16);
			this.gaugeControl1.Name = "gaugeControl1";
			this.gaugeControl1.Size = new Size(207, 196);
			this.gaugeControl1.TabIndex = 7;
			this.circularGauge1.set_Bounds(new Rectangle(2, 3, 195, 184));
			this.circularGauge1.get_Labels().AddRange(new LabelComponent[]
			{
				this.labelComponent1,
				this.labelComponent2
			});
			this.circularGauge1.set_Name("circularGauge1");
			this.circularGauge1.get_RangeBars().AddRange(new ArcScaleRangeBarComponent[]
			{
				this.arcScaleRangeBarComponent1
			});
			this.circularGauge1.get_Scales().AddRange(new ArcScaleComponent[]
			{
				this.arcScaleComponent1
			});
			this.labelComponent1.get_AppearanceText().set_Font(new Font("Segoe UI", 27.75f));
			this.labelComponent1.set_Name("circularGauge1_Label1");
			this.labelComponent1.set_Shader(new StyleShader("Colors[Style1:OrangeRed;Style2:OrangeRed]"));
			this.labelComponent1.set_Size(new SizeF(140f, 60f));
			this.labelComponent1.set_Text("910");
			this.labelComponent1.set_UseColorScheme(false);
			this.labelComponent1.set_ZOrder(-1001);
			this.labelComponent2.get_AppearanceText().set_Font(new Font("Tahoma", 15f));
			this.labelComponent2.get_AppearanceText().set_TextBrush(new SolidBrushObject("Color:Black"));
			this.labelComponent2.set_Name("circularGauge1_Label2");
			this.labelComponent2.set_Position(new PointF2D(125f, 175f));
			this.labelComponent2.set_Shader(new StyleShader("Colors[Style1:White;Style2:White]"));
			this.labelComponent2.set_Text("相似度");
			this.labelComponent2.set_ZOrder(-1001);
			this.arcScaleRangeBarComponent1.set_ArcScale(this.arcScaleComponent1);
			this.arcScaleRangeBarComponent1.set_Name("circularGauge1_RangeBar2");
			this.arcScaleRangeBarComponent1.set_RoundedCaps(true);
			this.arcScaleRangeBarComponent1.set_Shader(new StyleShader("Colors[Style1:OrangeRed;Style2:OrangeRed]"));
			this.arcScaleRangeBarComponent1.set_ShowBackground(true);
			this.arcScaleRangeBarComponent1.set_StartOffset(80f);
			this.arcScaleRangeBarComponent1.set_ZOrder(-10);
			this.arcScaleComponent1.get_AppearanceMajorTickmark().set_BorderBrush(new SolidBrushObject("Color:White"));
			this.arcScaleComponent1.get_AppearanceMajorTickmark().set_ContentBrush(new SolidBrushObject("Color:White"));
			this.arcScaleComponent1.get_AppearanceMinorTickmark().set_BorderBrush(new SolidBrushObject("Color:White"));
			this.arcScaleComponent1.get_AppearanceMinorTickmark().set_ContentBrush(new SolidBrushObject("Color:White"));
			this.arcScaleComponent1.get_AppearanceTickmarkText().set_Font(new Font("Tahoma", 8.5f));
			this.arcScaleComponent1.get_AppearanceTickmarkText().set_TextBrush(new SolidBrushObject("Color:#484E5A"));
			this.arcScaleComponent1.set_Center(new PointF2D(125f, 125f));
			this.arcScaleComponent1.set_EndAngle(90f);
			this.arcScaleComponent1.set_MajorTickCount(0);
			this.arcScaleComponent1.get_MajorTickmark().set_FormatString("{0:F0}");
			this.arcScaleComponent1.get_MajorTickmark().set_ShapeOffset(-14f);
			this.arcScaleComponent1.get_MajorTickmark().set_ShapeType(67);
			this.arcScaleComponent1.get_MajorTickmark().set_TextOrientation(3);
			this.arcScaleComponent1.set_MaxValue(100f);
			this.arcScaleComponent1.set_MinorTickCount(0);
			this.arcScaleComponent1.get_MinorTickmark().set_ShapeOffset(-7f);
			this.arcScaleComponent1.get_MinorTickmark().set_ShapeType(68);
			this.arcScaleComponent1.set_Name("scale1");
			this.arcScaleComponent1.set_StartAngle(-270f);
			this.arcScaleComponent1.set_Value(20f);
			base.AutoScaleMode = AutoScaleMode.None;
			this.BackColor = Color.FromArgb(45, 45, 48);
			base.Controls.Add(this.panelControl1);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.pictureBox1);
			this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			base.Name = "FaceCompareUserControl3";
			base.Size = new Size(431, 475);
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.panelControl1).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			this.circularGauge1.EndInit();
			this.labelComponent1.EndInit();
			this.labelComponent2.EndInit();
			this.arcScaleRangeBarComponent1.EndInit();
			this.arcScaleComponent1.EndInit();
			base.ResumeLayout(false);
		}
	}
}
