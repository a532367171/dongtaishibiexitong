using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using Face.resources;
using FaceCompareBase;
using MC_DAL;
using MC_DAL.Entity;
using MCFaceRecognitionVideo.UnitBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	public class PersonList : XtraForm
	{
		private List<Person> _persons = new List<Person>();
		private readonly PersonService _personService = new PersonService();
		private DataTable DT;
		private int no = 1;
		private IContainer components;
		private Panel panel1;
		private ImageList imageList1;
		private Label label2;
		private Label label1;
		private GridColumn gridColumn1;
		private GridControl gridControl1;
		private GridView gridView1;
		private GridColumn PersonID;
		private GridColumn PersonType;
		private GridColumn PersonNumber;
		private GridColumn PersonName;
		private GridColumn PersonImage;
		private GridColumn PersonAge;
		private GridColumn PersonSex;
		private SimpleButton btn_select;
		private TextEdit txt_name;
		private TextEdit txt_Nunber;
		private SimpleButton btn_export;
		public PersonList()
		{
			this.InitializeComponent();
			if (LanguageSet.Resource != null)
			{
				this.UpDataMainFormMenuLanguage(LanguageSet.Resource);
			}
			this.CreateTable();
			this.gridControl1.DataSource = this.DT;
		}
		private void tsb_close_Click(object sender, EventArgs e)
		{
			base.Close();
		}
		private void btn_Select_Click(object sender, EventArgs e)
		{
			this._persons = this._personService.GetList(this.txt_Nunber.Text, this.txt_name.Text);
			this.CreateTable();
			this.gridControl1.DataSource = this.DT;
		}
		private void CreateTable()
		{
			this.DT = new DataTable();
			this.DT.Columns.Add("No");
			this.DT.Columns.Add("PersonID");
			this.DT.Columns.Add("PersonType");
			this.DT.Columns.Add("PersonNumber");
			this.DT.Columns.Add("PersonName");
			this.DT.Columns.Add("PersonImage", typeof(Image));
			this.DT.Columns.Add("PersonAge");
			this.DT.Columns.Add("PersonSex");
			foreach (Person current in this._persons)
			{
				try
				{
					DataRow dataRow = this.DT.NewRow();
					dataRow["No"] = this.no;
					dataRow["PersonID"] = current.ID;
					dataRow["PersonType"] = current.tmp1;
					dataRow["PersonNumber"] = current.Number;
					dataRow["PersonName"] = current.Name;
					dataRow["PersonImage"] = FaceImageFormat.ByteToBitmap(Convert.FromBase64String(current.Image));
					dataRow["PersonAge"] = current.Age;
					dataRow["PersonSex"] = ((current.Sex == 0L) ? UnitField.Female : UnitField.Male);
					this.DT.Rows.Add(dataRow);
					this.no++;
				}
				catch
				{
				}
			}
		}
		private void btn_export_Click(object sender, EventArgs e)
		{
			ExportToExcelHelper.ExportToExcel(string.Format("{0}_{1}", UnitField.PersonList, DateTime.Now.ToString("yyyyMMdd")), new IPrintable[]
			{
				this.gridControl1
			});
		}
		private void UpDataMainFormMenuLanguage(ResourceManager rm)
		{
			this.btn_select.Text = UnitField.Search;
			this.PersonID.Caption = UnitField.PersonID;
			this.PersonAge.Caption = UnitField.PersonAge;
			this.PersonImage.Caption = UnitField.FaceTempateImage;
			this.PersonName.Caption = UnitField.PersonName;
			this.PersonNumber.Caption = UnitField.PersonNumber;
			this.PersonSex.Caption = UnitField.PersonSex;
			this.PersonType.Caption = UnitField.PersonType;
			this.label1.Text = UnitField.PersonNumber;
			this.label2.Text = UnitField.PersonName;
			this.Text = UnitField.PersonList;
			this.btn_export.Text = UnitField.Export;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PersonList));
			this.imageList1 = new ImageList();
			this.panel1 = new Panel();
			this.btn_export = new SimpleButton();
			this.txt_name = new TextEdit();
			this.txt_Nunber = new TextEdit();
			this.btn_select = new SimpleButton();
			this.gridControl1 = new GridControl();
			this.gridView1 = new GridView();
			this.PersonID = new GridColumn();
			this.PersonType = new GridColumn();
			this.PersonNumber = new GridColumn();
			this.PersonName = new GridColumn();
			this.PersonImage = new GridColumn();
			this.PersonAge = new GridColumn();
			this.PersonSex = new GridColumn();
			this.label2 = new Label();
			this.label1 = new Label();
			this.gridColumn1 = new GridColumn();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.txt_name.Properties).BeginInit();
			((ISupportInitialize)this.txt_Nunber.Properties).BeginInit();
			((ISupportInitialize)this.gridControl1).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			base.SuspendLayout();
			this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
			this.imageList1.TransparentColor = Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "video.png");
			this.panel1.Controls.Add(this.btn_export);
			this.panel1.Controls.Add(this.txt_name);
			this.panel1.Controls.Add(this.txt_Nunber);
			this.panel1.Controls.Add(this.btn_select);
			this.panel1.Controls.Add(this.gridControl1);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(1, 1);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new Padding(10);
			this.panel1.Size = new Size(898, 659);
			this.panel1.TabIndex = 3;
			this.btn_export.Image = (Image)componentResourceManager.GetObject("btn_export.Image");
			this.btn_export.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_export.Location = new Point(681, 17);
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(82, 23);
			this.btn_export.TabIndex = 15;
			this.btn_export.Text = "导出";
			this.btn_export.Click += new EventHandler(this.btn_export_Click);
			this.txt_name.Location = new Point(381, 18);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(162, 20);
			this.txt_name.TabIndex = 14;
			this.txt_Nunber.Location = new Point(104, 18);
			this.txt_Nunber.Name = "txt_Nunber";
			this.txt_Nunber.Size = new Size(162, 20);
			this.txt_Nunber.TabIndex = 13;
			this.btn_select.Image = (Image)componentResourceManager.GetObject("btn_select.Image");
			this.btn_select.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_select.Location = new Point(582, 17);
			this.btn_select.Name = "btn_select";
			this.btn_select.Size = new Size(82, 23);
			this.btn_select.TabIndex = 12;
			this.btn_select.Text = "查 询";
			this.btn_select.Click += new EventHandler(this.btn_Select_Click);
			this.gridControl1.Location = new Point(3, 58);
			this.gridControl1.LookAndFeel.SkinName = "Office 2010 Black";
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new Size(892, 598);
			this.gridControl1.TabIndex = 11;
			this.gridControl1.ViewCollection.AddRange(new BaseView[]
			{
				this.gridView1
			});
			this.gridView1.Columns.AddRange(new GridColumn[]
			{
				this.PersonID,
				this.PersonType,
				this.PersonNumber,
				this.PersonName,
				this.PersonImage,
				this.PersonAge,
				this.PersonSex
			});
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.RowHeight = 60;
			this.PersonID.Caption = "人员ID";
			this.PersonID.FieldName = "PersonID";
			this.PersonID.Name = "PersonID";
			this.PersonID.Visible = true;
			this.PersonID.VisibleIndex = 0;
			this.PersonType.Caption = "人员类别";
			this.PersonType.FieldName = "PersonType";
			this.PersonType.Name = "PersonType";
			this.PersonType.Visible = true;
			this.PersonType.VisibleIndex = 1;
			this.PersonNumber.Caption = "人员编号";
			this.PersonNumber.FieldName = "PersonNumber";
			this.PersonNumber.Name = "PersonNumber";
			this.PersonNumber.Visible = true;
			this.PersonNumber.VisibleIndex = 2;
			this.PersonName.Caption = "人员名称";
			this.PersonName.FieldName = "PersonName";
			this.PersonName.Name = "PersonName";
			this.PersonName.Visible = true;
			this.PersonName.VisibleIndex = 3;
			this.PersonImage.Caption = "照片";
			this.PersonImage.FieldName = "PersonImage";
			this.PersonImage.Name = "PersonImage";
			this.PersonImage.Visible = true;
			this.PersonImage.VisibleIndex = 4;
			this.PersonAge.Caption = "年龄";
			this.PersonAge.FieldName = "PersonAge";
			this.PersonAge.Name = "PersonAge";
			this.PersonAge.Visible = true;
			this.PersonAge.VisibleIndex = 5;
			this.PersonSex.Caption = "人员性别";
			this.PersonSex.FieldName = "PersonSex";
			this.PersonSex.Name = "PersonSex";
			this.PersonSex.Visible = true;
			this.PersonSex.VisibleIndex = 6;
			this.label2.AutoSize = true;
			this.label2.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.label2.ForeColor = Color.White;
			this.label2.Location = new Point(25, 20);
			this.label2.Name = "label2";
			this.label2.Size = new Size(68, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "人员编号：";
			this.label1.AutoSize = true;
			this.label1.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.label1.ForeColor = Color.White;
			this.label1.Location = new Point(292, 20);
			this.label1.Name = "label1";
			this.label1.Size = new Size(68, 17);
			this.label1.TabIndex = 7;
			this.label1.Text = "人员名称：";
			this.gridColumn1.Name = "gridColumn1";
			this.gridColumn1.Visible = true;
			this.gridColumn1.VisibleIndex = 0;
			base.Appearance.Options.UseFont = true;
			base.AutoScaleMode = AutoScaleMode.None;
			base.ClientSize = new Size(900, 661);
			base.Controls.Add(this.panel1);
			this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			base.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PersonList";
			base.Padding = new Padding(1);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "人员名单查询";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((ISupportInitialize)this.txt_name.Properties).EndInit();
			((ISupportInitialize)this.txt_Nunber.Properties).EndInit();
			((ISupportInitialize)this.gridControl1).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
