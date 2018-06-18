using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using Face.resources;
using FaceCompareBase;
using log4net;
using MC_DAL;
using MC_DAL.Entity;
using MCFaceRecognitionVideo.UnitBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	public class FaceCompareLogger : XtraForm
	{
		private List<FaceCompareLog> _faceCompareLogs = new List<FaceCompareLog>();
		private readonly FaceCompareLogService _faceCompareLogService = new FaceCompareLogService();
		private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private DataTable DT;
		private int no = 1;
		private int _pageIndex = 1;
		private int _faceCompareLogCount;
		private int _pageCount = 1;
		private int PageSize = 50;
		private IContainer components;
		private Panel panel1;
		private ImageList imageList1;
		private Label lbl_number;
		private Label lbl_date;
		private Label lbl_paging;
		private SimpleButton btn_TopPage;
		private SimpleButton btn_EndPage;
		private SimpleButton btn_Previous;
		private SimpleButton btn_Next;
		private DateEdit dtp_date;
		private GridControl gridControl1;
		private GridView gridView1;
		private GridColumn PersonID;
		private GridColumn PersonType;
		private GridColumn PersonNumber;
		private GridColumn PersonName;
		private GridColumn FaceDetcetDate;
		private GridColumn FaceTempateImage;
		private GridColumn FaceDetcetImage;
		private GridColumn Similarity;
		private GridColumn Chanl;
		private TextEdit txt_Number;
		private SimpleButton btn_Search;
		private SimpleButton btn_extport;
		public FaceCompareLogger()
		{
			this.InitializeComponent();
			if (LanguageSet.Resource != null)
			{
				this.UpDataMainFormMenuLanguage();
			}
			this.dtp_date.DateTime = DateTime.Now;
		}
		private void tsb_close_Click(object sender, EventArgs e)
		{
			base.Close();
		}
		private void CreateTable()
		{
			this.DT = new DataTable();
			this.no = (this._pageIndex - 1) * this.PageSize;
			this.no = ((this.no == 0) ? 1 : this.no);
			this.DT.Columns.Add("No");
			this.DT.Columns.Add("PersonID");
			this.DT.Columns.Add("PersonType");
			this.DT.Columns.Add("PersonNumber");
			this.DT.Columns.Add("PersonName");
			this.DT.Columns.Add("FaceDetcetDate");
			this.DT.Columns.Add("FaceTempateImage", typeof(Image));
			this.DT.Columns.Add("FaceDetcetImage", typeof(Image));
			this.DT.Columns.Add("Similarity");
			this.DT.Columns.Add("Chanl");
			foreach (FaceCompareLog current in this._faceCompareLogs)
			{
				DataRow dataRow = this.DT.NewRow();
				dataRow["No"] = this.no;
				dataRow["PersonID"] = current.PersonID;
				dataRow["PersonType"] = current.tmp2;
				dataRow["PersonNumber"] = current.PersonNumber;
				dataRow["PersonName"] = current.PersonName;
				dataRow["FaceDetcetDate"] = current.FaceDetcetDate;
				if (current.FaceTempateImage.Length > 0)
				{
					dataRow["FaceTempateImage"] = FaceImageFormat.ByteToBitmap(Convert.FromBase64String(current.FaceTempateImage));
				}
				if (current.FaceDetcetImage.Length > 0)
				{
					dataRow["FaceDetcetImage"] = FaceImageFormat.ByteToBitmap(Convert.FromBase64String(current.FaceDetcetImage));
				}
				dataRow["Similarity"] = current.Similarity;
				dataRow["Chanl"] = current.tmp1;
				this.DT.Rows.Add(dataRow);
				this.no++;
			}
		}
		private void btn_Select_Click(object sender, EventArgs e)
		{
			this._pageIndex = 1;
			this.FaceCompareLogSelect();
		}
		private void FaceCompareLogSelect()
		{
			if (!this.txt_Number.Text.Equals(string.Empty))
			{
				this._faceCompareLogs = this._faceCompareLogService.GetList(out this._faceCompareLogCount, this.dtp_date.DateTime, this.txt_Number.Text, this.PageSize, this._pageIndex);
			}
			else
			{
				this._faceCompareLogs = this._faceCompareLogService.GetList(out this._faceCompareLogCount, this.dtp_date.DateTime, this.PageSize, this._pageIndex);
			}
			this._pageCount = this._faceCompareLogCount / this.PageSize + ((this._faceCompareLogCount % this.PageSize > 0) ? 1 : 0);
			if (LanguageSet.Resource != null)
			{
				this.lbl_paging.Text = string.Format("{0}，{1}/{2}", this._faceCompareLogCount, this._pageIndex, this._pageCount);
			}
			else
			{
				this.lbl_paging.Text = string.Format("共{0}条数据，当前{1}/{2}页，每页显示{3}条。", new object[]
				{
					this._faceCompareLogCount,
					this._pageIndex,
					this._pageCount,
					this.PageSize
				});
			}
			this.CreateTable();
			this.gridControl1.DataSource = this.DT;
		}
		private void btn_Up_Click(object sender, EventArgs e)
		{
			this._pageIndex--;
			if (this._pageIndex < 1)
			{
				this._pageIndex = 1;
				return;
			}
			this.FaceCompareLogSelect();
		}
		private void btn_head_Click(object sender, EventArgs e)
		{
			this._pageIndex = 1;
			this.FaceCompareLogSelect();
		}
		private void btn_foot_Click(object sender, EventArgs e)
		{
			this._pageIndex = this._pageCount;
			this.FaceCompareLogSelect();
		}
		private void btn_Down_Click(object sender, EventArgs e)
		{
			this._pageIndex++;
			if (this._pageIndex > this._pageCount)
			{
				this._pageIndex = this._pageCount;
				return;
			}
			this.FaceCompareLogSelect();
		}
		private void FaceCompareLogger_FormClosing(object sender, FormClosingEventArgs e)
		{
			GC.Collect();
		}
		private void btn_extport_Click(object sender, EventArgs e)
		{
			ExportToExcelHelper.ExportToExcel(string.Format("{0}_{1}", UnitField.LogList, this.dtp_date.DateTime.ToString("yyyyMMdd")), new IPrintable[]
			{
				this.gridControl1
			});
		}
		private void UpDataMainFormMenuLanguage()
		{
			this.btn_TopPage.Text = UnitField.TopPage;
			this.btn_Next.Text = UnitField.Next;
			this.btn_Previous.Text = UnitField.Previous;
			this.btn_EndPage.Text = UnitField.EndPage;
			this.btn_Search.Text = UnitField.Search;
			this.Text = UnitField.LogList;
			this.btn_extport.Text = UnitField.Export;
			this.lbl_number.Text = UnitField.PersonNumber;
			this.lbl_date.Text = UnitField.Date;
			this.PersonID.Caption = UnitField.PersonID;
			this.PersonType.Caption = UnitField.PersonType;
			this.PersonNumber.Caption = UnitField.PersonNumber;
			this.PersonName.Caption = UnitField.PersonName;
			this.FaceDetcetDate.Caption = UnitField.CreateTime;
			this.FaceTempateImage.Caption = UnitField.FaceTempateImage;
			this.FaceDetcetImage.Caption = UnitField.FaceDetcetImage;
			this.Similarity.Caption = UnitField.Similarity;
			this.Chanl.Caption = UnitField.MonitoringChannel;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FaceCompareLogger));
			this.panel1 = new Panel();
			this.btn_extport = new SimpleButton();
			this.btn_Search = new SimpleButton();
			this.txt_Number = new TextEdit();
			this.dtp_date = new DateEdit();
			this.gridControl1 = new GridControl();
			this.gridView1 = new GridView();
			this.PersonID = new GridColumn();
			this.PersonType = new GridColumn();
			this.PersonNumber = new GridColumn();
			this.PersonName = new GridColumn();
			this.FaceDetcetDate = new GridColumn();
			this.FaceTempateImage = new GridColumn();
			this.FaceDetcetImage = new GridColumn();
			this.Similarity = new GridColumn();
			this.Chanl = new GridColumn();
			this.btn_TopPage = new SimpleButton();
			this.btn_EndPage = new SimpleButton();
			this.btn_Previous = new SimpleButton();
			this.btn_Next = new SimpleButton();
			this.lbl_paging = new Label();
			this.lbl_number = new Label();
			this.lbl_date = new Label();
			this.imageList1 = new ImageList();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.txt_Number.Properties).BeginInit();
			((ISupportInitialize)this.dtp_date.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtp_date.Properties).BeginInit();
			((ISupportInitialize)this.gridControl1).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			base.SuspendLayout();
			this.panel1.Controls.Add(this.btn_extport);
			this.panel1.Controls.Add(this.btn_Search);
			this.panel1.Controls.Add(this.txt_Number);
			this.panel1.Controls.Add(this.dtp_date);
			this.panel1.Controls.Add(this.gridControl1);
			this.panel1.Controls.Add(this.btn_TopPage);
			this.panel1.Controls.Add(this.btn_EndPage);
			this.panel1.Controls.Add(this.btn_Previous);
			this.panel1.Controls.Add(this.btn_Next);
			this.panel1.Controls.Add(this.lbl_paging);
			this.panel1.Controls.Add(this.lbl_number);
			this.panel1.Controls.Add(this.lbl_date);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(1, 1);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(898, 659);
			this.panel1.TabIndex = 3;
			this.btn_extport.Image = (Image)componentResourceManager.GetObject("btn_extport.Image");
			this.btn_extport.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_extport.Location = new Point(586, 12);
			this.btn_extport.Name = "btn_extport";
			this.btn_extport.Size = new Size(75, 23);
			this.btn_extport.TabIndex = 15;
			this.btn_extport.Text = "导出";
			this.btn_extport.Click += new EventHandler(this.btn_extport_Click);
			this.btn_Search.Image = (Image)componentResourceManager.GetObject("btn_Search.Image");
			this.btn_Search.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_Search.Location = new Point(493, 12);
			this.btn_Search.Name = "btn_Search";
			this.btn_Search.Size = new Size(73, 23);
			this.btn_Search.TabIndex = 15;
			this.btn_Search.Text = "查 询";
			this.btn_Search.Click += new EventHandler(this.btn_Select_Click);
			this.txt_Number.Location = new Point(308, 13);
			this.txt_Number.Name = "txt_Number";
			this.txt_Number.Size = new Size(173, 20);
			this.txt_Number.TabIndex = 14;
			this.dtp_date.EditValue = null;
			this.dtp_date.Location = new Point(62, 13);
			this.dtp_date.Name = "dtp_date";
			this.dtp_date.Properties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dtp_date.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dtp_date.Size = new Size(173, 20);
			this.dtp_date.TabIndex = 13;
			this.gridControl1.Location = new Point(3, 47);
			this.gridControl1.LookAndFeel.SkinName = "Office 2010 Black";
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new Size(892, 574);
			this.gridControl1.TabIndex = 12;
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
				this.FaceDetcetDate,
				this.FaceTempateImage,
				this.FaceDetcetImage,
				this.Similarity,
				this.Chanl
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
			this.FaceDetcetDate.Caption = "识别时间";
			this.FaceDetcetDate.FieldName = "FaceDetcetDate";
			this.FaceDetcetDate.Name = "FaceDetcetDate";
			this.FaceDetcetDate.Visible = true;
			this.FaceDetcetDate.VisibleIndex = 4;
			this.FaceTempateImage.Caption = "注册照片";
			this.FaceTempateImage.FieldName = "FaceTempateImage";
			this.FaceTempateImage.Name = "FaceTempateImage";
			this.FaceTempateImage.Visible = true;
			this.FaceTempateImage.VisibleIndex = 5;
			this.FaceDetcetImage.Caption = "识别照片";
			this.FaceDetcetImage.FieldName = "FaceDetcetImage";
			this.FaceDetcetImage.Name = "FaceDetcetImage";
			this.FaceDetcetImage.Visible = true;
			this.FaceDetcetImage.VisibleIndex = 6;
			this.Similarity.Caption = "相似度";
			this.Similarity.FieldName = "Similarity";
			this.Similarity.Name = "Similarity";
			this.Similarity.Visible = true;
			this.Similarity.VisibleIndex = 7;
			this.Chanl.Caption = "识别通道";
			this.Chanl.FieldName = "Chanl";
			this.Chanl.Name = "Chanl";
			this.Chanl.Visible = true;
			this.Chanl.VisibleIndex = 8;
			this.btn_TopPage.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_TopPage.Location = new Point(553, 626);
			this.btn_TopPage.Name = "btn_TopPage";
			this.btn_TopPage.RightToLeft = RightToLeft.No;
			this.btn_TopPage.Size = new Size(75, 23);
			this.btn_TopPage.TabIndex = 8;
			this.btn_TopPage.Text = "首页";
			this.btn_TopPage.Click += new EventHandler(this.btn_head_Click);
			this.btn_EndPage.ImageToTextAlignment = ImageAlignToText.RightCenter;
			this.btn_EndPage.Location = new Point(808, 626);
			this.btn_EndPage.Name = "btn_EndPage";
			this.btn_EndPage.Size = new Size(75, 23);
			this.btn_EndPage.TabIndex = 7;
			this.btn_EndPage.Text = "尾页";
			this.btn_EndPage.Click += new EventHandler(this.btn_foot_Click);
			this.btn_Previous.ImageToTextAlignment = ImageAlignToText.LeftCenter;
			this.btn_Previous.Location = new Point(638, 626);
			this.btn_Previous.Name = "btn_Previous";
			this.btn_Previous.Size = new Size(75, 23);
			this.btn_Previous.TabIndex = 7;
			this.btn_Previous.Text = "上一页";
			this.btn_Previous.Click += new EventHandler(this.btn_Up_Click);
			this.btn_Next.ImageToTextAlignment = ImageAlignToText.RightCenter;
			this.btn_Next.Location = new Point(723, 626);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(75, 23);
			this.btn_Next.TabIndex = 7;
			this.btn_Next.Text = "下一页";
			this.btn_Next.Click += new EventHandler(this.btn_Down_Click);
			this.lbl_paging.AutoSize = true;
			this.lbl_paging.ForeColor = Color.White;
			this.lbl_paging.Location = new Point(14, 629);
			this.lbl_paging.Name = "lbl_paging";
			this.lbl_paging.Size = new Size(0, 17);
			this.lbl_paging.TabIndex = 6;
			this.lbl_number.AutoSize = true;
			this.lbl_number.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.lbl_number.ForeColor = Color.White;
			this.lbl_number.Location = new Point(254, 15);
			this.lbl_number.Name = "lbl_number";
			this.lbl_number.Size = new Size(44, 17);
			this.lbl_number.TabIndex = 1;
			this.lbl_number.Text = "编号：";
			this.lbl_date.AutoSize = true;
			this.lbl_date.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.lbl_date.ForeColor = Color.White;
			this.lbl_date.Location = new Point(11, 15);
			this.lbl_date.Name = "lbl_date";
			this.lbl_date.Size = new Size(44, 17);
			this.lbl_date.TabIndex = 1;
			this.lbl_date.Text = "日期：";
			this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
			this.imageList1.TransparentColor = Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "video.png");
			base.Appearance.Options.UseFont = true;
			base.AutoScaleMode = AutoScaleMode.None;
			base.ClientSize = new Size(900, 661);
			base.Controls.Add(this.panel1);
			this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			base.MaximizeBox = false;
			this.MaximumSize = new Size(916, 697);
			base.MinimizeBox = false;
			this.MinimumSize = new Size(916, 697);
			base.Name = "FaceCompareLogger";
			base.Padding = new Padding(1);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "识别记录查看";
			base.FormClosing += new FormClosingEventHandler(this.FaceCompareLogger_FormClosing);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((ISupportInitialize)this.txt_Number.Properties).EndInit();
			((ISupportInitialize)this.dtp_date.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.dtp_date.Properties).EndInit();
			((ISupportInitialize)this.gridControl1).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
