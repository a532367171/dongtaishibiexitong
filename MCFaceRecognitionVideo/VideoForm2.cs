using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Emgu.CV;
using Emgu.CV.Structure;
using Face.resources;
using FaceCompareBase;
using FaceCompareThread;
using FaceDetectiveCtl;
using log4net;
using MC_DAL;
using MC_DAL.Entity;
using MCFaceRecognitionVideo.Properties;
using MCFaceRecognitionVideo.UnitBase;
using MCFaceRecognitionVideo.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	public class VideoForm2 : XtraForm
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly VideoForm2.<>c <>9 = new VideoForm2.<>c();
			public static Func<Person, long> <>9__38_0;
			public static Func<Person, FaceTemplate> <>9__38_1;
			public static Action <>9__43_0;
			public static Action <>9__44_0;
			public static Action <>9__46_0;
			internal long <GetTemplate>b__38_0(Person p)
			{
				return p.ID;
			}
			internal FaceTemplate <GetTemplate>b__38_1(Person person)
			{
				PersonType personType;
				if (person.tmp1.Equals(UnitField.Black))
				{
					personType = PersonType.Black;
				}
				else
				{
					if (person.tmp1.Equals(UnitField.White))
					{
						personType = PersonType.White;
					}
					else
					{
						personType = PersonType.VIP;
					}
				}
				return new FaceTemplate
				{
					FaceFeature = Convert.FromBase64String(person.Feature),
					ImageLocation = person.Image,
					PersonId = person.ID.ToString(),
					PersonName = person.Name,
					PersonNumber = person.Number,
					PersonType = personType
				};
			}
			internal void <toolStripButton2_Click>b__43_0()
			{
				using (PersonList personList = new PersonList())
				{
					personList.ShowDialog();
				}
			}
			internal void <tsb_camerSet_Click>b__44_0()
			{
				new CamerSetForm().Show();
			}
			internal void <tsb_FaceCompareLog_Click>b__46_0()
			{
				using (FaceCompareLogger faceCompareLogger = new FaceCompareLogger())
				{
					faceCompareLogger.ShowDialog();
				}
			}
		}
		private List<CamerInfo> _camerInfos;
		private readonly CamerInfoService _camerInfoService = new CamerInfoService();
		private List<Person> _persons = new List<Person>();
		private readonly PersonService _personService = new PersonService();
		private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private bool IsCapturePhotos;
		private string PhotoType;
		private int FrameRate;
		private FaceCompareType _FaceCompareType;
		private string PhotoSavePath;
		private float Conf;
		private long SoundTime = 1200L;
		private bool IsPlaySound = true;
		private string PlaySoundPath = "bj.wav";
		private int UpdateTime = 1;
		private List<FaceTemplate> _faceTemplates;
		public const int WM_SYSCOMMAND = 274;
		public const int SC_MOVE = 61456;
		public const int HTCAPTION = 2;
		private static readonly object _obj2 = new object();
		private Stopwatch _stopwatch = new Stopwatch();
		private volatile int _faceCompareCtlMaxCount;
		private long _maxId;
		private readonly object _obj = new object();
		private SaveFaceDetectThred _saveFaceDetectThred;
		private Thread _saveFaceDetectCaller;
		private bool IsMax;
		private IContainer components;
		private TableLayoutPanel tableLayoutPanel1;
		private FaceDetectiveControl faceDetectiveControl1;
		private FaceDetectiveControl faceDetectiveControl4;
		private FaceDetectiveControl faceDetectiveControl3;
		private FaceDetectiveControl faceDetectiveControl2;
		private ToolStrip toolStrip1;
		private ToolStripButton tsb_camerSet;
		private ToolStripButton tsb_PersonList;
		private ToolStripButton tsb_Logger;
		private ToolStripButton tsb_SystemSet;
		private ToolStripButton tsb_close;
		private ToolStripButton tsb_windows;
		private ToolStripButton tsb_restart;
		private Label lbl_mainTitle;
		private System.Windows.Forms.Timer timer1;
		private Panel panel1;
		private GroupControl grb_video;
		private GroupControl grp_CompareSuccess;
		public FlowLayoutPanel flowLayoutPanel2;
		public VideoForm2()
		{
			this.InitializeComponent();
			if (LanguageSet.Resource != null)
			{
				this.UpDataMainFormMenuLanguage();
			}
			SplashScreenManager.ShowForm(this, typeof(LoadingForm), true, true, false);
			this.SetInfo(UnitField.LoadFaceEngine, 0, 30, 30);
			try
			{
				this.lbl_mainTitle.Text = ConfigurationManager.AppSettings["MainTitle"];
				this.Text = ConfigurationManager.AppSettings["SystemName"];
				this.SoundTime = long.Parse(ConfigurationManager.AppSettings["SoundTime"]);
				this.PlaySoundPath = ConfigurationManager.AppSettings["PlaySoundPath"];
				this.IsPlaySound = ConfigurationManager.AppSettings["IsPlaySound"].Equals("true");
				if (!int.TryParse(ConfigurationManager.AppSettings["UpdateTime"], out this.UpdateTime))
				{
					this.UpdateTime = 1;
				}
			}
			catch (Exception)
			{
				this._log.Error(UnitField.LoadConfigurationError);
			}
			this.PageLoad();
			this._stopwatch.Start();
			this.SetInfo(UnitField.LoadConfiguration, 31, 60, 30);
			this.SetInfo(UnitField.LoadForm, 61, 100, 40);
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SplashScreenManager.CloseForm(false);
		}
		public void SetInfo(string labeltext, int formpos, int topos, int sleeptime)
		{
			for (int i = formpos; i < topos; i++)
			{
				SplashScreenManager.Default.SendCommand(LoadingForm.SplashScreenCommand.Setinfo, new Info
				{
					LabelText = labeltext,
					Pos = i
				});
				Thread.Sleep(sleeptime);
			}
		}
		private void PageLoad()
		{
			try
			{
				this._faceTemplates = this.GetTemplate();
				this.GetFaceCompareCount();
				this.IsCapturePhotos = XMLHelper.getXmlValue("PhotoSetting", "IsCapturePhotos").Equals("是");
				this.PhotoType = XMLHelper.getXmlValue("PhotoSetting", "PhotoType");
				this.PhotoSavePath = XMLHelper.getXmlValue("PhotoSetting", "PhotoSavePath");
				this.FrameRate = int.Parse(XMLHelper.getXmlValue("FaceCompreaSet", "FrameRate"));
				this.Conf = float.Parse(XMLHelper.getXmlValue("FaceCompreaSet", "conf").ToString());
				string xmlValue = XMLHelper.getXmlValue("FaceCompreaSet", "FaceCompareType");
				if (!(xmlValue == "FaceCompareBase"))
				{
					if (!(xmlValue == "FaceComparePro"))
					{
						if (!(xmlValue == "FaceCompareV2"))
						{
							if (xmlValue == "FaceCompareV4")
							{
								this._FaceCompareType = FaceCompareType.FaceCompareV4;
							}
						}
						else
						{
							this._FaceCompareType = FaceCompareType.FaceCompareV2;
						}
					}
					else
					{
						this._FaceCompareType = FaceCompareType.FaceComparePro;
					}
				}
				else
				{
					this._FaceCompareType = FaceCompareType.FaceCompareBase;
				}
				Console.WriteLine(this._FaceCompareType);
				if (!Directory.Exists(this.PhotoSavePath))
				{
					Directory.CreateDirectory(this.PhotoSavePath);
				}
				this._camerInfos = this._camerInfoService.GetList();
				if (this._camerInfos.Count > 0)
				{
					this.Init(this.faceDetectiveControl1, this._camerInfos[0]);
					this.Init(this.faceDetectiveControl2, this._camerInfos[1]);
					this.Init(this.faceDetectiveControl3, this._camerInfos[2]);
					this.Init(this.faceDetectiveControl4, this._camerInfos[3]);
				}
				this._saveFaceDetectThred = new SaveFaceDetectThred();
				this._saveFaceDetectCaller = new Thread(new ThreadStart(this._saveFaceDetectThred.Execute));
				this._saveFaceDetectCaller.Start();
				this.timer1.Interval = this.UpdateTime * 1000 * 60;
				this.timer1.Start();
			}
			catch (Exception message)
			{
				this._log.Error(message);
			}
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			this.faceDetectiveControl1_DoubleClick(sender, e);
		}
		private void Init(FaceDetectiveControl faceDetectiveControl, CamerInfo camerInfo)
		{
			try
			{
				if (camerInfo.IsTure != 1L)
				{
					faceDetectiveControl.Image = new Image<Bgr, byte>(Resources.weiqiyong);
				}
				else
				{
					faceDetectiveControl.Name = camerInfo.ID.ToString();
					faceDetectiveControl.CaptureType = UnitHelper.GetCameraTypeByName(camerInfo.CamerType);
					faceDetectiveControl.UsbIndex = (camerInfo.CamerType.Equals(UnitField.USBCamera) ? int.Parse(camerInfo.CamerAddress) : 0);
					faceDetectiveControl.IP = camerInfo.CamerAddress;
					faceDetectiveControl.CaptureSize = new Size((int)Convert.ToInt16(camerInfo.CamerWeight), (int)Convert.ToInt16(camerInfo.CamerHeight));
					faceDetectiveControl.Port = Convert.ToUInt16(camerInfo.CamerPort);
					faceDetectiveControl.UserName = camerInfo.CamerUser;
					faceDetectiveControl.PassWord = camerInfo.CamerPassword;
					faceDetectiveControl.Threshold = float.Parse(camerInfo.tmp1);
					faceDetectiveControl.Between2Eyes = int.Parse(camerInfo.tmp2);
					faceDetectiveControl.IsShowFaceRectangle = camerInfo.tmp3.Equals(UnitField.OnShow);
					faceDetectiveControl.FrameRate = this.FrameRate;
					faceDetectiveControl.SetFaceCompareType = this._FaceCompareType;
					faceDetectiveControl.Conf = this.Conf;
					faceDetectiveControl.ShowFaceDeteiveImageEventHandler += new ShowFaceDeteiveImageHandler(this.OnShowFaceDeteiveImage);
					faceDetectiveControl.CompareSuccessEventHander += new CompareSuccessHandler(this.OnCompareSuccessHander);
					faceDetectiveControl.Init(this._faceTemplates);
				}
			}
			catch (Exception message)
			{
				this._log.Error(message);
			}
		}
		private void Start(FaceDetectiveControl faceDetectiveControl, CamerInfo camerInfo)
		{
			if (camerInfo.IsTure == 1L)
			{
				faceDetectiveControl.Start();
			}
		}
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		[DllImport("user32.dll")]
		public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
		private void pnlTop_MouseDown(object sender, MouseEventArgs e)
		{
			VideoForm2.ReleaseCapture();
			VideoForm2.SendMessage(base.Handle, 274, 61458, 0);
		}
		private void OnCompareSuccessHander(List<ResultInfo> recognitions, string controlName)
		{
			Func<CamerInfo, bool> <>9__1;
			this.flowLayoutPanel2.BeginInvoke(delegate
			{
				try
				{
					object obj = VideoForm2._obj2;
					lock (obj)
					{
						VideoForm2.<>c__DisplayClass31_2 <>c__DisplayClass31_2 = new VideoForm2.<>c__DisplayClass31_2();
						VideoForm2.<>c__DisplayClass31_2 arg_46_0 = <>c__DisplayClass31_2;
						IEnumerable<CamerInfo> arg_41_0 = this._camerInfos;
						Func<CamerInfo, bool> arg_41_1;
						if ((arg_41_1 = <>9__1) == null)
						{
							arg_41_1 = (<>9__1 = ((CamerInfo p) => p.ID == (long)int.Parse(controlName)));
						}
						arg_46_0.camerInfo = arg_41_0.FirstOrDefault(arg_41_1);
						if (<>c__DisplayClass31_2.camerInfo != null)
						{
							string channel = <>c__DisplayClass31_2.camerInfo.Channel;
							ResultInfo recognition = recognitions[0];
							byte[] faceImage = recognitions[0].FaceImage;
							if (recognition.FaceTemplate.PersonType == PersonType.Black || recognition.FaceTemplate.PersonType == PersonType.VIP)
							{
								FaceCompareUserControl2 faceCompareUserControl = new FaceCompareUserControl2(recognition, channel);
								this.flowLayoutPanel2.Controls.Add(faceCompareUserControl);
								this.flowLayoutPanel2.Controls.SetChildIndex(faceCompareUserControl, 0);
								int count = this.flowLayoutPanel2.Controls.Count;
								if (count > this._faceCompareCtlMaxCount)
								{
									for (int i = this._faceCompareCtlMaxCount; i < count; i++)
									{
										this.flowLayoutPanel2.Controls.Remove(this.flowLayoutPanel2.Controls[this._faceCompareCtlMaxCount]);
									}
								}
								if (this.IsPlaySound && this._stopwatch.ElapsedMilliseconds > this.SoundTime)
								{
									PlaySoundBase.Play(this.PlaySoundPath);
									this._stopwatch.Restart();
								}
							}
							new Thread(delegate
							{
								SaveFaceCompareThred.AddFaceCompare(recognition, channel, faceImage, <>c__DisplayClass31_2.camerInfo.CamerAddress);
							}).Start();
						}
					}
				}
				catch (Exception message)
				{
					this._log.Error(message);
				}
			});
		}
		private void OnShowFaceDeteiveImage(byte[] imageData, FaceModel faceModels, string controlName)
		{
			Func<CamerInfo, bool> <>9__1;
			this.faceDetectiveControl1.BeginInvoke(delegate
			{
				try
				{
					IEnumerable<CamerInfo> arg_2A_0 = this._camerInfos;
					Func<CamerInfo, bool> arg_2A_1;
					if ((arg_2A_1 = <>9__1) == null)
					{
						arg_2A_1 = (<>9__1 = ((CamerInfo p) => p.ID == (long)int.Parse(controlName)));
					}
					CamerInfo camerInfo = arg_2A_0.FirstOrDefault(arg_2A_1);
					if (camerInfo != null && this._saveFaceDetectThred != null)
					{
						this._saveFaceDetectThred.Start(imageData, camerInfo.Channel);
					}
				}
				catch (Exception message)
				{
					this._log.Error(message);
				}
			});
		}
		private void GetFaceCompareCount()
		{
			int num = this.flowLayoutPanel2.Width * this.flowLayoutPanel2.Height;
			int num2 = 51600;
			this._faceCompareCtlMaxCount = num / num2;
		}
		private void UnLoadVideo(FaceDetectiveControl faceDetectiveControl, CamerInfo camerInfo)
		{
			try
			{
				if (camerInfo.IsTure == 1L)
				{
					faceDetectiveControl.ShowFaceDeteiveImageEventHandler -= new ShowFaceDeteiveImageHandler(this.OnShowFaceDeteiveImage);
					faceDetectiveControl.CompareSuccessEventHander -= new CompareSuccessHandler(this.OnCompareSuccessHander);
					faceDetectiveControl.Exit();
					faceDetectiveControl.Image = new Image<Bgr, byte>(Resources.weiqiyong);
				}
			}
			catch (Exception message)
			{
				this._log.Error(message);
			}
		}
		private void tsb_close_Click(object sender, EventArgs e)
		{
			base.Close();
		}
		private List<FaceTemplate> GetTemplate()
		{
			this._persons = this._personService.GetList(this._maxId);
			IEnumerable<Person> arg_3D_0 = this._persons;
			Func<Person, long> arg_3D_1;
			if ((arg_3D_1 = VideoForm2.<>c.<>9__38_0) == null)
			{
				arg_3D_1 = (VideoForm2.<>c.<>9__38_0 = new Func<Person, long>(VideoForm2.<>c.<>9.<GetTemplate>b__38_0));
			}
			Person expr_47 = arg_3D_0.OrderByDescending(arg_3D_1).FirstOrDefault<Person>();
			this._maxId = ((expr_47 != null) ? expr_47.ID : this._maxId);
			IEnumerable<Person> arg_82_0 = this._persons;
			Func<Person, FaceTemplate> arg_82_1;
			if ((arg_82_1 = VideoForm2.<>c.<>9__38_1) == null)
			{
				arg_82_1 = (VideoForm2.<>c.<>9__38_1 = new Func<Person, FaceTemplate>(VideoForm2.<>c.<>9.<GetTemplate>b__38_1));
			}
			return arg_82_0.Select(arg_82_1).ToList<FaceTemplate>();
		}
		private void tsb_windows_Click(object sender, EventArgs e)
		{
			if (base.WindowState == FormWindowState.Maximized)
			{
				base.WindowState = FormWindowState.Normal;
				this.tsb_windows.Text = UnitField.Maximized;
			}
			else
			{
				base.WindowState = FormWindowState.Maximized;
				this.tsb_windows.Text = UnitField.ExitFullScreen;
			}
			this.GetFaceCompareCount();
		}
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			Action arg_20_1;
			if ((arg_20_1 = VideoForm2.<>c.<>9__43_0) == null)
			{
				arg_20_1 = (VideoForm2.<>c.<>9__43_0 = new Action(VideoForm2.<>c.<>9.<toolStripButton2_Click>b__43_0));
			}
			base.BeginInvoke(arg_20_1);
		}
		private void tsb_camerSet_Click(object sender, EventArgs e)
		{
			Action arg_20_1;
			if ((arg_20_1 = VideoForm2.<>c.<>9__44_0) == null)
			{
				arg_20_1 = (VideoForm2.<>c.<>9__44_0 = new Action(VideoForm2.<>c.<>9.<tsb_camerSet_Click>b__44_0));
			}
			base.BeginInvoke(arg_20_1);
		}
		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			new SystemSetting().ShowDialog();
		}
		private void tsb_FaceCompareLog_Click(object sender, EventArgs e)
		{
			Action arg_20_1;
			if ((arg_20_1 = VideoForm2.<>c.<>9__46_0) == null)
			{
				arg_20_1 = (VideoForm2.<>c.<>9__46_0 = new Action(VideoForm2.<>c.<>9.<tsb_FaceCompareLog_Click>b__46_0));
			}
			base.BeginInvoke(arg_20_1);
		}
		private void VideoForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this._saveFaceDetectThred != null)
			{
				this._saveFaceDetectThred.Stop();
				this._saveFaceDetectCaller.Join();
			}
			if (this._camerInfos.Count > 0)
			{
				this.UnLoadVideo(this.faceDetectiveControl1, this._camerInfos[0]);
				this.UnLoadVideo(this.faceDetectiveControl2, this._camerInfos[1]);
				this.UnLoadVideo(this.faceDetectiveControl3, this._camerInfos[2]);
				this.UnLoadVideo(this.faceDetectiveControl4, this._camerInfos[3]);
			}
		}
		private void VideoForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}
		private void tsb_restart_Click(object sender, EventArgs e)
		{
			if (XtraMessageBox.Show(UnitField.RestartMessage, UnitField.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Process.Start(Assembly.GetExecutingAssembly().Location);
				base.Close();
				Process.GetCurrentProcess().Kill();
			}
		}
		private void faceDetectiveControl1_DoubleClick(object sender, EventArgs e)
		{
			if (!this.IsMax)
			{
				this.faceDetectiveControl2.Visible = false;
				this.faceDetectiveControl3.Visible = false;
				this.faceDetectiveControl4.Visible = false;
				this.tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0f);
				this.tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Percent, 0f);
				this.IsMax = true;
				return;
			}
			this.TableLayoutPanelRest();
		}
		private void faceDetectiveControl2_DoubleClick(object sender, EventArgs e)
		{
			if (!this.IsMax)
			{
				this.faceDetectiveControl1.Visible = false;
				this.faceDetectiveControl3.Visible = false;
				this.faceDetectiveControl4.Visible = false;
				this.tableLayoutPanel1.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 0f);
				this.tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Percent, 0f);
				this.IsMax = true;
				return;
			}
			this.TableLayoutPanelRest();
		}
		private void faceDetectiveControl3_DoubleClick(object sender, EventArgs e)
		{
			if (!this.IsMax)
			{
				this.faceDetectiveControl1.Visible = false;
				this.faceDetectiveControl2.Visible = false;
				this.faceDetectiveControl4.Visible = false;
				this.tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0f);
				this.tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, 0f);
				this.IsMax = true;
				return;
			}
			this.TableLayoutPanelRest();
		}
		private void faceDetectiveControl4_DoubleClick(object sender, EventArgs e)
		{
			if (!this.IsMax)
			{
				this.faceDetectiveControl1.Visible = false;
				this.faceDetectiveControl2.Visible = false;
				this.faceDetectiveControl3.Visible = false;
				this.tableLayoutPanel1.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 0f);
				this.tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Percent, 100f);
				this.tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, 0f);
				this.IsMax = true;
				return;
			}
			this.TableLayoutPanelRest();
		}
		private void TableLayoutPanelRest()
		{
			this.faceDetectiveControl1.Visible = true;
			this.faceDetectiveControl2.Visible = true;
			this.faceDetectiveControl3.Visible = true;
			this.faceDetectiveControl4.Visible = true;
			this.tableLayoutPanel1.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 50f);
			this.tableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, 50f);
			this.tableLayoutPanel1.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 50f);
			this.tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Percent, 50f);
			this.IsMax = false;
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			using (List<FaceTemplate>.Enumerator enumerator = (this.GetTemplate() ?? new List<FaceTemplate>()).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FaceTemplate faceTemplate = enumerator.Current;
					this.faceDetectiveControl1.BeginInvoke(delegate
					{
						this.faceDetectiveControl1.IndsertFaceTemplate(faceTemplate);
					});
				}
			}
		}
		private void VideoForm2_Shown(object sender, EventArgs e)
		{
			this._camerInfos = this._camerInfoService.GetList();
			if (this._camerInfos.Count > 0)
			{
				this.Start(this.faceDetectiveControl1, this._camerInfos[0]);
				this.Start(this.faceDetectiveControl2, this._camerInfos[1]);
				this.Start(this.faceDetectiveControl3, this._camerInfos[2]);
				this.Start(this.faceDetectiveControl4, this._camerInfos[3]);
			}
		}
		private void UpDataMainFormMenuLanguage()
		{
			this.tsb_windows.Text = UnitField.Maximized;
			this.tsb_close.Text = UnitField.ExitSystem;
			this.tsb_camerSet.Text = UnitField.ProtectionSettings;
			this.tsb_PersonList.Text = UnitField.PersonList;
			this.tsb_Logger.Text = UnitField.LogList;
			this.tsb_SystemSet.Text = UnitField.SystemSettings;
			this.tsb_restart.Text = UnitField.Restart;
			this.grb_video.Text = UnitField.Video;
			this.grp_CompareSuccess.Text = UnitField.CompareSuccess;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(VideoForm2));
			this.timer1 = new System.Windows.Forms.Timer();
			this.toolStrip1 = new ToolStrip();
			this.tsb_camerSet = new ToolStripButton();
			this.tsb_PersonList = new ToolStripButton();
			this.tsb_Logger = new ToolStripButton();
			this.tsb_SystemSet = new ToolStripButton();
			this.tsb_close = new ToolStripButton();
			this.tsb_windows = new ToolStripButton();
			this.tsb_restart = new ToolStripButton();
			this.panel1 = new Panel();
			this.grp_CompareSuccess = new GroupControl();
			this.flowLayoutPanel2 = new FlowLayoutPanel();
			this.grb_video = new GroupControl();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.faceDetectiveControl4 = new FaceDetectiveControl();
			this.faceDetectiveControl3 = new FaceDetectiveControl();
			this.faceDetectiveControl2 = new FaceDetectiveControl();
			this.faceDetectiveControl1 = new FaceDetectiveControl();
			this.lbl_mainTitle = new Label();
			this.toolStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.grp_CompareSuccess).BeginInit();
			this.grp_CompareSuccess.SuspendLayout();
			((ISupportInitialize)this.grb_video).BeginInit();
			this.grb_video.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((ISupportInitialize)this.faceDetectiveControl4).BeginInit();
			((ISupportInitialize)this.faceDetectiveControl3).BeginInit();
			((ISupportInitialize)this.faceDetectiveControl2).BeginInit();
			((ISupportInitialize)this.faceDetectiveControl1).BeginInit();
			base.SuspendLayout();
			this.timer1.Tick += new EventHandler(this.timer1_Tick);
			this.toolStrip1.BackColor = Color.FromArgb(45, 45, 48);
			this.toolStrip1.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.tsb_camerSet,
				this.tsb_PersonList,
				this.tsb_Logger,
				this.tsb_SystemSet,
				this.tsb_close,
				this.tsb_windows,
				this.tsb_restart
			});
			this.toolStrip1.Location = new Point(1, 1);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = ToolStripRenderMode.System;
			this.toolStrip1.Size = new Size(1022, 46);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "系统设置";
			this.toolStrip1.MouseDown += new MouseEventHandler(this.pnlTop_MouseDown);
			this.tsb_camerSet.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_camerSet.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_camerSet.ForeColor = Color.White;
			this.tsb_camerSet.Image = (Image)componentResourceManager.GetObject("tsb_camerSet.Image");
			this.tsb_camerSet.ImageTransparentColor = Color.Magenta;
			this.tsb_camerSet.Margin = new Padding(5, 2, 0, 2);
			this.tsb_camerSet.Name = "tsb_camerSet";
			this.tsb_camerSet.Padding = new Padding(13, 13, 0, 13);
			this.tsb_camerSet.Size = new Size(74, 42);
			this.tsb_camerSet.Text = "布防设置";
			this.tsb_camerSet.Click += new EventHandler(this.tsb_camerSet_Click);
			this.tsb_PersonList.BackColor = Color.Transparent;
			this.tsb_PersonList.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_PersonList.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_PersonList.ForeColor = Color.White;
			this.tsb_PersonList.Image = (Image)componentResourceManager.GetObject("tsb_PersonList.Image");
			this.tsb_PersonList.ImageTransparentColor = Color.Magenta;
			this.tsb_PersonList.Margin = new Padding(2);
			this.tsb_PersonList.Name = "tsb_PersonList";
			this.tsb_PersonList.Padding = new Padding(13, 13, 0, 13);
			this.tsb_PersonList.Size = new Size(100, 42);
			this.tsb_PersonList.Text = "人员名单查询";
			this.tsb_PersonList.Click += new EventHandler(this.toolStripButton2_Click);
			this.tsb_Logger.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_Logger.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_Logger.ForeColor = Color.White;
			this.tsb_Logger.Image = (Image)componentResourceManager.GetObject("tsb_Logger.Image");
			this.tsb_Logger.ImageTransparentColor = Color.Magenta;
			this.tsb_Logger.Margin = new Padding(2);
			this.tsb_Logger.Name = "tsb_Logger";
			this.tsb_Logger.Padding = new Padding(13, 13, 0, 13);
			this.tsb_Logger.Size = new Size(74, 42);
			this.tsb_Logger.Text = "记录查看";
			this.tsb_Logger.Click += new EventHandler(this.tsb_FaceCompareLog_Click);
			this.tsb_SystemSet.BackColor = Color.Transparent;
			this.tsb_SystemSet.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_SystemSet.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_SystemSet.ForeColor = Color.White;
			this.tsb_SystemSet.Image = (Image)componentResourceManager.GetObject("tsb_SystemSet.Image");
			this.tsb_SystemSet.ImageTransparentColor = Color.Magenta;
			this.tsb_SystemSet.Margin = new Padding(2);
			this.tsb_SystemSet.Name = "tsb_SystemSet";
			this.tsb_SystemSet.Padding = new Padding(13, 13, 0, 13);
			this.tsb_SystemSet.Size = new Size(74, 42);
			this.tsb_SystemSet.Text = "系统设置";
			this.tsb_SystemSet.Click += new EventHandler(this.toolStripButton4_Click);
			this.tsb_close.Alignment = ToolStripItemAlignment.Right;
			this.tsb_close.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_close.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_close.ForeColor = Color.White;
			this.tsb_close.Image = (Image)componentResourceManager.GetObject("tsb_close.Image");
			this.tsb_close.ImageTransparentColor = Color.Magenta;
			this.tsb_close.Margin = new Padding(2, 2, 12, 2);
			this.tsb_close.Name = "tsb_close";
			this.tsb_close.Padding = new Padding(0, 13, 13, 13);
			this.tsb_close.Size = new Size(88, 42);
			this.tsb_close.Text = "退出系统  ";
			this.tsb_close.TextAlign = ContentAlignment.MiddleRight;
			this.tsb_close.Click += new EventHandler(this.tsb_close_Click);
			this.tsb_windows.Alignment = ToolStripItemAlignment.Right;
			this.tsb_windows.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_windows.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_windows.ForeColor = Color.White;
			this.tsb_windows.Image = (Image)componentResourceManager.GetObject("tsb_windows.Image");
			this.tsb_windows.ImageTransparentColor = Color.Magenta;
			this.tsb_windows.Margin = new Padding(2);
			this.tsb_windows.Name = "tsb_windows";
			this.tsb_windows.Padding = new Padding(0, 13, 13, 13);
			this.tsb_windows.Size = new Size(48, 42);
			this.tsb_windows.Text = "全屏";
			this.tsb_windows.Click += new EventHandler(this.tsb_windows_Click);
			this.tsb_restart.BackColor = Color.Transparent;
			this.tsb_restart.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_restart.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsb_restart.ForeColor = Color.White;
			this.tsb_restart.Image = (Image)componentResourceManager.GetObject("tsb_restart.Image");
			this.tsb_restart.ImageTransparentColor = Color.Magenta;
			this.tsb_restart.Margin = new Padding(2);
			this.tsb_restart.Name = "tsb_restart";
			this.tsb_restart.Padding = new Padding(13, 13, 0, 13);
			this.tsb_restart.Size = new Size(74, 42);
			this.tsb_restart.Text = "重启系统";
			this.tsb_restart.ToolTipText = "重启系统";
			this.tsb_restart.Click += new EventHandler(this.tsb_restart_Click);
			this.panel1.BackColor = Color.FromArgb(45, 45, 48);
			this.panel1.Controls.Add(this.grp_CompareSuccess);
			this.panel1.Controls.Add(this.grb_video);
			this.panel1.Controls.Add(this.lbl_mainTitle);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(1, 1);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new Padding(2);
			this.panel1.Size = new Size(1022, 766);
			this.panel1.TabIndex = 6;
			this.grp_CompareSuccess.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right);
			this.grp_CompareSuccess.Appearance.Font = new Font("Tahoma", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.grp_CompareSuccess.Appearance.ForeColor = Color.Yellow;
			this.grp_CompareSuccess.Appearance.Options.UseFont = true;
			this.grp_CompareSuccess.Appearance.Options.UseForeColor = true;
			this.grp_CompareSuccess.AppearanceCaption.Font = new Font("Tahoma", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.grp_CompareSuccess.AppearanceCaption.ForeColor = Color.Yellow;
			this.grp_CompareSuccess.AppearanceCaption.Options.UseFont = true;
			this.grp_CompareSuccess.AppearanceCaption.Options.UseForeColor = true;
			this.grp_CompareSuccess.Controls.Add(this.flowLayoutPanel2);
			this.grp_CompareSuccess.Location = new Point(598, 54);
			this.grp_CompareSuccess.Name = "grp_CompareSuccess";
			this.grp_CompareSuccess.Size = new Size(419, 707);
			this.grp_CompareSuccess.TabIndex = 7;
			this.grp_CompareSuccess.Text = "识别记录";
			this.flowLayoutPanel2.Dock = DockStyle.Fill;
			this.flowLayoutPanel2.Location = new Point(2, 21);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new Size(415, 684);
			this.flowLayoutPanel2.TabIndex = 0;
			this.grb_video.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.grb_video.AppearanceCaption.Font = new Font("Tahoma", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.grb_video.AppearanceCaption.ForeColor = Color.Yellow;
			this.grb_video.AppearanceCaption.Options.UseFont = true;
			this.grb_video.AppearanceCaption.Options.UseForeColor = true;
			this.grb_video.Controls.Add(this.tableLayoutPanel1);
			this.grb_video.Location = new Point(5, 54);
			this.grb_video.Name = "grb_video";
			this.grb_video.Size = new Size(587, 576);
			this.grb_video.TabIndex = 6;
			this.grb_video.Text = "实时视频";
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl4, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl3, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl1, 0, 0);
			this.tableLayoutPanel1.Dock = DockStyle.Fill;
			this.tableLayoutPanel1.Location = new Point(2, 21);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.Size = new Size(583, 553);
			this.tableLayoutPanel1.TabIndex = 0;
			this.faceDetectiveControl4.BackColor = Color.FromArgb(64, 64, 64);
			this.faceDetectiveControl4.BackgroundImage = Resources.weiqiyong;
			this.faceDetectiveControl4.BackgroundImageLayout = ImageLayout.Stretch;
			this.faceDetectiveControl4.Between2Eyes = 60;
			this.faceDetectiveControl4.BorderStyle = BorderStyle.FixedSingle;
			this.faceDetectiveControl4.CaptureSize = new Size(640, 480);
			this.faceDetectiveControl4.CaptureType = CameraType.USBCamera;
			this.faceDetectiveControl4.CompareSuccessCount = 5;
			this.faceDetectiveControl4.Conf = 0.7f;
			this.faceDetectiveControl4.Dock = DockStyle.Fill;
			this.faceDetectiveControl4.FrameRate = 1;
			this.faceDetectiveControl4.IP = "192.168.0.64";
			this.faceDetectiveControl4.IsMaxFace = false;
			this.faceDetectiveControl4.IsShowFaceRectangle = false;
			this.faceDetectiveControl4.Location = new Point(291, 276);
			this.faceDetectiveControl4.Margin = new Padding(0);
			this.faceDetectiveControl4.Name = "faceDetectiveControl4";
			this.faceDetectiveControl4.PassWord = "admin123";
			this.faceDetectiveControl4.Port = 8000;
			this.faceDetectiveControl4.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl4.Size = new Size(292, 277);
			this.faceDetectiveControl4.SizeMode = PictureBoxSizeMode.StretchImage;
			this.faceDetectiveControl4.TabIndex = 3;
			this.faceDetectiveControl4.TabStop = false;
			this.faceDetectiveControl4.Threshold = 0.6f;
			this.faceDetectiveControl4.UsbIndex = 0;
			this.faceDetectiveControl4.UserName = "admin";
			this.faceDetectiveControl4.DoubleClick += new EventHandler(this.faceDetectiveControl4_DoubleClick);
			this.faceDetectiveControl3.BackColor = Color.FromArgb(64, 64, 64);
			this.faceDetectiveControl3.BackgroundImage = Resources.weiqiyong;
			this.faceDetectiveControl3.BackgroundImageLayout = ImageLayout.Stretch;
			this.faceDetectiveControl3.Between2Eyes = 60;
			this.faceDetectiveControl3.BorderStyle = BorderStyle.FixedSingle;
			this.faceDetectiveControl3.CaptureSize = new Size(640, 480);
			this.faceDetectiveControl3.CaptureType = CameraType.USBCamera;
			this.faceDetectiveControl3.CompareSuccessCount = 5;
			this.faceDetectiveControl3.Conf = 0.7f;
			this.faceDetectiveControl3.Dock = DockStyle.Fill;
			this.faceDetectiveControl3.FrameRate = 1;
			this.faceDetectiveControl3.IP = "192.168.0.64";
			this.faceDetectiveControl3.IsMaxFace = false;
			this.faceDetectiveControl3.IsShowFaceRectangle = false;
			this.faceDetectiveControl3.Location = new Point(0, 276);
			this.faceDetectiveControl3.Margin = new Padding(0);
			this.faceDetectiveControl3.Name = "faceDetectiveControl3";
			this.faceDetectiveControl3.PassWord = "admin123";
			this.faceDetectiveControl3.Port = 8000;
			this.faceDetectiveControl3.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl3.Size = new Size(291, 277);
			this.faceDetectiveControl3.SizeMode = PictureBoxSizeMode.StretchImage;
			this.faceDetectiveControl3.TabIndex = 2;
			this.faceDetectiveControl3.TabStop = false;
			this.faceDetectiveControl3.Threshold = 0.6f;
			this.faceDetectiveControl3.UsbIndex = 0;
			this.faceDetectiveControl3.UserName = "admin";
			this.faceDetectiveControl3.DoubleClick += new EventHandler(this.faceDetectiveControl3_DoubleClick);
			this.faceDetectiveControl2.BackColor = Color.FromArgb(64, 64, 64);
			this.faceDetectiveControl2.BackgroundImage = Resources.weiqiyong;
			this.faceDetectiveControl2.BackgroundImageLayout = ImageLayout.Stretch;
			this.faceDetectiveControl2.Between2Eyes = 60;
			this.faceDetectiveControl2.BorderStyle = BorderStyle.FixedSingle;
			this.faceDetectiveControl2.CaptureSize = new Size(640, 480);
			this.faceDetectiveControl2.CaptureType = CameraType.USBCamera;
			this.faceDetectiveControl2.CompareSuccessCount = 5;
			this.faceDetectiveControl2.Conf = 0.7f;
			this.faceDetectiveControl2.Dock = DockStyle.Fill;
			this.faceDetectiveControl2.FrameRate = 1;
			this.faceDetectiveControl2.IP = "192.168.0.64";
			this.faceDetectiveControl2.IsMaxFace = false;
			this.faceDetectiveControl2.IsShowFaceRectangle = false;
			this.faceDetectiveControl2.Location = new Point(291, 0);
			this.faceDetectiveControl2.Margin = new Padding(0);
			this.faceDetectiveControl2.Name = "faceDetectiveControl2";
			this.faceDetectiveControl2.PassWord = "admin123";
			this.faceDetectiveControl2.Port = 8000;
			this.faceDetectiveControl2.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl2.Size = new Size(292, 276);
			this.faceDetectiveControl2.SizeMode = PictureBoxSizeMode.StretchImage;
			this.faceDetectiveControl2.TabIndex = 1;
			this.faceDetectiveControl2.TabStop = false;
			this.faceDetectiveControl2.Threshold = 0.55f;
			this.faceDetectiveControl2.UsbIndex = 0;
			this.faceDetectiveControl2.UserName = "admin";
			this.faceDetectiveControl2.DoubleClick += new EventHandler(this.faceDetectiveControl2_DoubleClick);
			this.faceDetectiveControl1.BackColor = Color.FromArgb(64, 64, 64);
			this.faceDetectiveControl1.BackgroundImage = Resources.weiqiyong;
			this.faceDetectiveControl1.BackgroundImageLayout = ImageLayout.Stretch;
			this.faceDetectiveControl1.Between2Eyes = 60;
			this.faceDetectiveControl1.BorderStyle = BorderStyle.FixedSingle;
			this.faceDetectiveControl1.CaptureSize = new Size(640, 480);
			this.faceDetectiveControl1.CaptureType = CameraType.USBCamera;
			this.faceDetectiveControl1.CompareSuccessCount = 5;
			this.faceDetectiveControl1.Conf = 0.7f;
			this.faceDetectiveControl1.Dock = DockStyle.Fill;
			this.faceDetectiveControl1.FrameRate = 1;
			this.faceDetectiveControl1.IP = "192.168.0.64";
			this.faceDetectiveControl1.IsMaxFace = false;
			this.faceDetectiveControl1.IsShowFaceRectangle = false;
			this.faceDetectiveControl1.Location = new Point(0, 0);
			this.faceDetectiveControl1.Margin = new Padding(0);
			this.faceDetectiveControl1.Name = "faceDetectiveControl1";
			this.faceDetectiveControl1.PassWord = "admin123";
			this.faceDetectiveControl1.Port = 8000;
			this.faceDetectiveControl1.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl1.Size = new Size(291, 276);
			this.faceDetectiveControl1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.faceDetectiveControl1.TabIndex = 0;
			this.faceDetectiveControl1.TabStop = false;
			this.faceDetectiveControl1.Threshold = 0.6f;
			this.faceDetectiveControl1.UsbIndex = 0;
			this.faceDetectiveControl1.UserName = "admin";
			this.faceDetectiveControl1.DoubleClick += new EventHandler(this.faceDetectiveControl1_DoubleClick);
			this.lbl_mainTitle.Anchor = AnchorStyles.Bottom;
			this.lbl_mainTitle.AutoSize = true;
			this.lbl_mainTitle.Font = new Font("幼圆", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.lbl_mainTitle.ForeColor = Color.White;
			this.lbl_mainTitle.Location = new Point(36, 674);
			this.lbl_mainTitle.Name = "lbl_mainTitle";
			this.lbl_mainTitle.Size = new Size(427, 19);
			this.lbl_mainTitle.TabIndex = 5;
			this.lbl_mainTitle.Text = "人脸识别系统   服务电话：0318 - 0347883";
			base.Appearance.BackColor = Color.DodgerBlue;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleMode = AutoScaleMode.None;
			base.ClientSize = new Size(1024, 768);
			base.Controls.Add(this.toolStrip1);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.LookAndFeel.SkinName = "Visual Studio 2013 Dark";
			base.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MinimumSize = new Size(1024, 768);
			base.Name = "VideoForm2";
			base.Padding = new Padding(1);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "视频监控预警平台";
			base.FormClosing += new FormClosingEventHandler(this.VideoForm_FormClosing);
			base.FormClosed += new FormClosedEventHandler(this.VideoForm_FormClosed);
			base.Load += new EventHandler(this.Form1_Load);
			base.Shown += new EventHandler(this.VideoForm2_Shown);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((ISupportInitialize)this.grp_CompareSuccess).EndInit();
			this.grp_CompareSuccess.ResumeLayout(false);
			((ISupportInitialize)this.grb_video).EndInit();
			this.grb_video.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((ISupportInitialize)this.faceDetectiveControl4).EndInit();
			((ISupportInitialize)this.faceDetectiveControl3).EndInit();
			((ISupportInitialize)this.faceDetectiveControl2).EndInit();
			((ISupportInitialize)this.faceDetectiveControl1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
