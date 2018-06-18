using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
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
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	public class VideoForm : XtraForm
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly VideoForm.<>c <>9 = new VideoForm.<>c();
			public static Func<Person, long> <>9__43_0;
			public static Func<Person, FaceTemplate> <>9__43_1;
			public static Action <>9__48_0;
			public static Action <>9__49_0;
			public static Action <>9__51_0;
			internal long <GetTemplate>b__43_0(Person p)
			{
				return p.ID;
			}
			internal FaceTemplate <GetTemplate>b__43_1(Person person)
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
			internal void <toolStripButton2_Click>b__48_0()
			{
				using (PersonList personList = new PersonList())
				{
					personList.ShowDialog();
				}
			}
			internal void <tsb_camerSet_Click>b__49_0()
			{
				new CamerSetForm().Show();
			}
			internal void <tsb_FaceCompareLog_Click>b__51_0()
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
		private volatile int _faceDetectCtlMaxCount;
		private long MaxID;
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
		private GroupBox grb_video;
		private GroupBox grp_FaceCaptureDisplay;
		public FlowLayoutPanel flowLayoutPanel1;
		private GroupBox grp_CompareSuccess;
		public FlowLayoutPanel flowLayoutPanel2;
		private ToolStripButton tsb_close;
		private ToolStripButton tsb_windows;
		private ToolStripButton tsb_restart;
		private System.Windows.Forms.Timer timer1;
		private Panel panel1;
		public sealed override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
		public VideoForm()
		{
			this.InitializeComponent();
			if (LanguageSet.Resource != null)
			{
				this.UpDataMainFormMenuLanguage(LanguageSet.Resource);
			}
			SplashScreenManager.ShowForm(this, typeof(LoadingForm), true, true, false);
			this.SetInfo(UnitField.LoadFaceEngine, 0, 30, 30);
			try
			{
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
				this.GetFaceDetectCount();
				this.GetFaceCompareCount();
				this.IsCapturePhotos = XMLHelper.getXmlValue("PhotoSetting", "IsCapturePhotos").Equals(UnitField.Yes);
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
			VideoForm.ReleaseCapture();
			VideoForm.SendMessage(base.Handle, 274, 61458, 0);
		}
		private void OnCompareSuccessHander(List<ResultInfo> recognitions, string controlName)
		{
			Func<CamerInfo, bool> <>9__1;
			this.flowLayoutPanel2.BeginInvoke(delegate
			{
				try
				{
					object obj = VideoForm._obj2;
					lock (obj)
					{
						VideoForm.<>c__DisplayClass34_2 <>c__DisplayClass34_2 = new VideoForm.<>c__DisplayClass34_2();
						VideoForm.<>c__DisplayClass34_2 arg_46_0 = <>c__DisplayClass34_2;
						IEnumerable<CamerInfo> arg_41_0 = this._camerInfos;
						Func<CamerInfo, bool> arg_41_1;
						if ((arg_41_1 = <>9__1) == null)
						{
							arg_41_1 = (<>9__1 = ((CamerInfo p) => p.ID == (long)int.Parse(controlName)));
						}
						arg_46_0.camerInfo = arg_41_0.FirstOrDefault(arg_41_1);
						if (<>c__DisplayClass34_2.camerInfo != null)
						{
							string channel = <>c__DisplayClass34_2.camerInfo.Channel;
							ResultInfo recognition = recognitions[0];
							byte[] faceImage = recognitions[0].FaceImage;
							if (recognition.FaceTemplate.PersonType == PersonType.Black || recognition.FaceTemplate.PersonType == PersonType.VIP)
							{
								FaceCompareUserControl faceCompareUserControl = new FaceCompareUserControl(recognition, channel);
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
								SaveFaceCompareThred.AddFaceCompare(recognition, channel, faceImage, <>c__DisplayClass34_2.camerInfo.CamerAddress);
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
		private void GetFaceCompareCount()
		{
			int num = this.flowLayoutPanel2.Width * this.flowLayoutPanel2.Height;
			int num2 = 39618;
			this._faceCompareCtlMaxCount = num / num2;
		}
		private void OnShowFaceDeteiveImage(byte[] imageData, FaceModel faceModels, string controlName)
		{
			Func<CamerInfo, bool> <>9__1;
			this.flowLayoutPanel1.BeginInvoke(delegate
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
						FaceDetectUserContorl faceDetectUserContorl = new FaceDetectUserContorl(imageData, camerInfo.Channel);
						this.flowLayoutPanel1.Controls.Add(faceDetectUserContorl);
						this.flowLayoutPanel1.Controls.SetChildIndex(faceDetectUserContorl, 0);
						int count = this.flowLayoutPanel1.Controls.Count;
						if (count > this._faceDetectCtlMaxCount)
						{
							for (int i = this._faceDetectCtlMaxCount; i < count; i++)
							{
								this.flowLayoutPanel1.Controls[this._faceDetectCtlMaxCount].Dispose();
							}
						}
						this._saveFaceDetectThred.Start(imageData, camerInfo.Channel);
					}
				}
				catch (Exception message)
				{
					this._log.Error(message);
				}
			});
		}
		private void GetFaceDetectCount()
		{
			int height = this.flowLayoutPanel1.Height;
			int num = 140;
			this._faceDetectCtlMaxCount = height / num * 4;
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
			this._persons = this._personService.GetList(this.MaxID);
			IEnumerable<Person> arg_3D_0 = this._persons;
			Func<Person, long> arg_3D_1;
			if ((arg_3D_1 = VideoForm.<>c.<>9__43_0) == null)
			{
				arg_3D_1 = (VideoForm.<>c.<>9__43_0 = new Func<Person, long>(VideoForm.<>c.<>9.<GetTemplate>b__43_0));
			}
			Person expr_47 = arg_3D_0.OrderByDescending(arg_3D_1).FirstOrDefault<Person>();
			this.MaxID = ((expr_47 != null) ? expr_47.ID : this.MaxID);
			IEnumerable<Person> arg_82_0 = this._persons;
			Func<Person, FaceTemplate> arg_82_1;
			if ((arg_82_1 = VideoForm.<>c.<>9__43_1) == null)
			{
				arg_82_1 = (VideoForm.<>c.<>9__43_1 = new Func<Person, FaceTemplate>(VideoForm.<>c.<>9.<GetTemplate>b__43_1));
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
			this.GetFaceDetectCount();
		}
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			Action arg_20_1;
			if ((arg_20_1 = VideoForm.<>c.<>9__48_0) == null)
			{
				arg_20_1 = (VideoForm.<>c.<>9__48_0 = new Action(VideoForm.<>c.<>9.<toolStripButton2_Click>b__48_0));
			}
			base.BeginInvoke(arg_20_1);
		}
		private void tsb_camerSet_Click(object sender, EventArgs e)
		{
			Action arg_20_1;
			if ((arg_20_1 = VideoForm.<>c.<>9__49_0) == null)
			{
				arg_20_1 = (VideoForm.<>c.<>9__49_0 = new Action(VideoForm.<>c.<>9.<tsb_camerSet_Click>b__49_0));
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
			if ((arg_20_1 = VideoForm.<>c.<>9__51_0) == null)
			{
				arg_20_1 = (VideoForm.<>c.<>9__51_0 = new Action(VideoForm.<>c.<>9.<tsb_FaceCompareLog_Click>b__51_0));
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
				this.StopVideo();
			}
		}
		private void StartVideo()
		{
			this.Start(this.faceDetectiveControl1, this._camerInfos[0]);
			this.Start(this.faceDetectiveControl2, this._camerInfos[1]);
			this.Start(this.faceDetectiveControl3, this._camerInfos[2]);
			this.Start(this.faceDetectiveControl4, this._camerInfos[3]);
		}
		private void StopVideo()
		{
			this.UnLoadVideo(this.faceDetectiveControl1, this._camerInfos[0]);
			this.UnLoadVideo(this.faceDetectiveControl2, this._camerInfos[1]);
			this.UnLoadVideo(this.faceDetectiveControl3, this._camerInfos[2]);
			this.UnLoadVideo(this.faceDetectiveControl4, this._camerInfos[3]);
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
				this.faceDetectiveControl1.Refresh();
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
				this.faceDetectiveControl2.Refresh();
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
				this.faceDetectiveControl3.Refresh();
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
				this.faceDetectiveControl4.Refresh();
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
		private void VideoForm_Shown(object sender, EventArgs e)
		{
			this._camerInfos = this._camerInfoService.GetList();
			if (this._camerInfos.Count > 0)
			{
				this.StartVideo();
			}
			this.faceDetectiveControl1_DoubleClick(sender, e);
		}
		private void UpDataMainFormMenuLanguage(ResourceManager rm)
		{
			this.tsb_windows.Text = UnitField.Maximized;
			this.tsb_close.Text = UnitField.ExitSystem;
			this.tsb_camerSet.Text = UnitField.ProtectionSettings;
			this.tsb_PersonList.Text = UnitField.PersonList;
			this.tsb_Logger.Text = UnitField.LogList;
			this.tsb_SystemSet.Text = UnitField.SystemSettings;
			this.tsb_restart.Text = UnitField.Restart;
			this.tsb_restart.ToolTipText = UnitField.Restart;
			this.grb_video.Text = UnitField.Video;
			this.grp_CompareSuccess.Text = UnitField.CompareSuccess;
			this.grp_FaceCaptureDisplay.Text = UnitField.FaceCaptureDisplay;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(VideoForm));
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.faceDetectiveControl4 = new FaceDetectiveControl();
			this.faceDetectiveControl3 = new FaceDetectiveControl();
			this.faceDetectiveControl2 = new FaceDetectiveControl();
			this.faceDetectiveControl1 = new FaceDetectiveControl();
			this.grb_video = new GroupBox();
			this.grp_FaceCaptureDisplay = new GroupBox();
			this.flowLayoutPanel1 = new FlowLayoutPanel();
			this.grp_CompareSuccess = new GroupBox();
			this.flowLayoutPanel2 = new FlowLayoutPanel();
			this.toolStrip1 = new ToolStrip();
			this.tsb_camerSet = new ToolStripButton();
			this.tsb_PersonList = new ToolStripButton();
			this.tsb_Logger = new ToolStripButton();
			this.tsb_SystemSet = new ToolStripButton();
			this.tsb_close = new ToolStripButton();
			this.tsb_windows = new ToolStripButton();
			this.tsb_restart = new ToolStripButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.panel1 = new Panel();
			this.tableLayoutPanel1.SuspendLayout();
			((ISupportInitialize)this.faceDetectiveControl4).BeginInit();
			((ISupportInitialize)this.faceDetectiveControl3).BeginInit();
			((ISupportInitialize)this.faceDetectiveControl2).BeginInit();
			((ISupportInitialize)this.faceDetectiveControl1).BeginInit();
			this.grb_video.SuspendLayout();
			this.grp_FaceCaptureDisplay.SuspendLayout();
			this.grp_CompareSuccess.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl4, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl3, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.faceDetectiveControl1, 0, 0);
			this.tableLayoutPanel1.Location = new Point(5, 20);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel1.Size = new Size(529, 443);
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
			this.faceDetectiveControl4.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
			this.faceDetectiveControl4.IP = "192.168.0.64";
			this.faceDetectiveControl4.IsMaxFace = false;
			this.faceDetectiveControl4.IsShowFaceRectangle = false;
			this.faceDetectiveControl4.Location = new Point(264, 221);
			this.faceDetectiveControl4.Margin = new Padding(0);
			this.faceDetectiveControl4.Name = "faceDetectiveControl4";
			this.faceDetectiveControl4.PassWord = "admin123";
			this.faceDetectiveControl4.Port = 8000;
			this.faceDetectiveControl4.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl4.Size = new Size(265, 222);
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
			this.faceDetectiveControl3.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
			this.faceDetectiveControl3.IP = "192.168.0.64";
			this.faceDetectiveControl3.IsMaxFace = false;
			this.faceDetectiveControl3.IsShowFaceRectangle = false;
			this.faceDetectiveControl3.Location = new Point(0, 221);
			this.faceDetectiveControl3.Margin = new Padding(0);
			this.faceDetectiveControl3.Name = "faceDetectiveControl3";
			this.faceDetectiveControl3.PassWord = "admin123";
			this.faceDetectiveControl3.Port = 8000;
			this.faceDetectiveControl3.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl3.Size = new Size(264, 222);
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
			this.faceDetectiveControl2.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
			this.faceDetectiveControl2.IP = "192.168.0.64";
			this.faceDetectiveControl2.IsMaxFace = false;
			this.faceDetectiveControl2.IsShowFaceRectangle = false;
			this.faceDetectiveControl2.Location = new Point(264, 0);
			this.faceDetectiveControl2.Margin = new Padding(0);
			this.faceDetectiveControl2.Name = "faceDetectiveControl2";
			this.faceDetectiveControl2.PassWord = "admin123";
			this.faceDetectiveControl2.Port = 8000;
			this.faceDetectiveControl2.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl2.Size = new Size(265, 221);
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
			this.faceDetectiveControl1.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
			this.faceDetectiveControl1.IP = "192.168.0.64";
			this.faceDetectiveControl1.IsMaxFace = false;
			this.faceDetectiveControl1.IsShowFaceRectangle = false;
			this.faceDetectiveControl1.Location = new Point(0, 0);
			this.faceDetectiveControl1.Margin = new Padding(0);
			this.faceDetectiveControl1.Name = "faceDetectiveControl1";
			this.faceDetectiveControl1.PassWord = "admin123";
			this.faceDetectiveControl1.Port = 8000;
			this.faceDetectiveControl1.SetFaceCompareType = FaceCompareType.FaceComparePro;
			this.faceDetectiveControl1.Size = new Size(264, 221);
			this.faceDetectiveControl1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.faceDetectiveControl1.TabIndex = 0;
			this.faceDetectiveControl1.TabStop = false;
			this.faceDetectiveControl1.Threshold = 0.6f;
			this.faceDetectiveControl1.UsbIndex = 0;
			this.faceDetectiveControl1.UserName = "admin";
			this.faceDetectiveControl1.DoubleClick += new EventHandler(this.faceDetectiveControl1_DoubleClick);
			this.grb_video.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.grb_video.Controls.Add(this.tableLayoutPanel1);
			this.grb_video.Font = new Font("微软雅黑", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.grb_video.ForeColor = Color.FromArgb(223, 255, 43);
			this.grb_video.Location = new Point(5, 59);
			this.grb_video.Name = "grb_video";
			this.grb_video.Size = new Size(540, 470);
			this.grb_video.TabIndex = 4;
			this.grb_video.TabStop = false;
			this.grb_video.Text = "实时视频流";
			this.grp_FaceCaptureDisplay.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right);
			this.grp_FaceCaptureDisplay.Controls.Add(this.flowLayoutPanel1);
			this.grp_FaceCaptureDisplay.Font = new Font("微软雅黑", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.grp_FaceCaptureDisplay.ForeColor = Color.FromArgb(223, 255, 43);
			this.grp_FaceCaptureDisplay.Location = new Point(551, 59);
			this.grp_FaceCaptureDisplay.Name = "grp_FaceCaptureDisplay";
			this.grp_FaceCaptureDisplay.Size = new Size(467, 470);
			this.grp_FaceCaptureDisplay.TabIndex = 2;
			this.grp_FaceCaptureDisplay.TabStop = false;
			this.grp_FaceCaptureDisplay.Text = "实时人脸抓拍显示";
			this.flowLayoutPanel1.Dock = DockStyle.Fill;
			this.flowLayoutPanel1.Location = new Point(3, 19);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new Size(461, 448);
			this.flowLayoutPanel1.TabIndex = 0;
			this.grp_CompareSuccess.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.grp_CompareSuccess.Controls.Add(this.flowLayoutPanel2);
			this.grp_CompareSuccess.Font = new Font("微软雅黑", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.grp_CompareSuccess.ForeColor = Color.FromArgb(223, 255, 43);
			this.grp_CompareSuccess.Location = new Point(4, 538);
			this.grp_CompareSuccess.Name = "grp_CompareSuccess";
			this.grp_CompareSuccess.Size = new Size(1015, 220);
			this.grp_CompareSuccess.TabIndex = 2;
			this.grp_CompareSuccess.TabStop = false;
			this.grp_CompareSuccess.Text = "比对成功信息";
			this.flowLayoutPanel2.Dock = DockStyle.Fill;
			this.flowLayoutPanel2.Location = new Point(3, 19);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new Size(1009, 198);
			this.flowLayoutPanel2.TabIndex = 0;
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
			this.toolStrip1.Margin = new Padding(23);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = ToolStripRenderMode.System;
			this.toolStrip1.Size = new Size(1022, 46);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "系统设置";
			this.toolStrip1.MouseDown += new MouseEventHandler(this.pnlTop_MouseDown);
			this.tsb_camerSet.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_camerSet.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.tsb_camerSet.ForeColor = Color.White;
			this.tsb_camerSet.Image = (Image)componentResourceManager.GetObject("tsb_camerSet.Image");
			this.tsb_camerSet.ImageTransparentColor = Color.Magenta;
			this.tsb_camerSet.Margin = new Padding(5, 2, 0, 2);
			this.tsb_camerSet.Name = "tsb_camerSet";
			this.tsb_camerSet.Padding = new Padding(13, 13, 0, 13);
			this.tsb_camerSet.Size = new Size(74, 42);
			this.tsb_camerSet.Text = "布防设置";
			this.tsb_camerSet.Click += new EventHandler(this.tsb_camerSet_Click);
			this.tsb_PersonList.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_PersonList.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.tsb_PersonList.ForeColor = Color.White;
			this.tsb_PersonList.ImageTransparentColor = Color.Magenta;
			this.tsb_PersonList.Margin = new Padding(2);
			this.tsb_PersonList.Name = "tsb_PersonList";
			this.tsb_PersonList.Padding = new Padding(13, 13, 0, 13);
			this.tsb_PersonList.Size = new Size(74, 42);
			this.tsb_PersonList.Text = "人员名单";
			this.tsb_PersonList.Click += new EventHandler(this.toolStripButton2_Click);
			this.tsb_Logger.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_Logger.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.tsb_Logger.ForeColor = Color.White;
			this.tsb_Logger.ImageTransparentColor = Color.Magenta;
			this.tsb_Logger.Margin = new Padding(2);
			this.tsb_Logger.Name = "tsb_Logger";
			this.tsb_Logger.Padding = new Padding(13, 13, 0, 13);
			this.tsb_Logger.Size = new Size(74, 42);
			this.tsb_Logger.Text = "记录查看";
			this.tsb_Logger.Click += new EventHandler(this.tsb_FaceCompareLog_Click);
			this.tsb_SystemSet.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_SystemSet.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.tsb_SystemSet.ForeColor = Color.White;
			this.tsb_SystemSet.ImageTransparentColor = Color.Magenta;
			this.tsb_SystemSet.Margin = new Padding(2);
			this.tsb_SystemSet.Name = "tsb_SystemSet";
			this.tsb_SystemSet.Padding = new Padding(13, 13, 0, 13);
			this.tsb_SystemSet.Size = new Size(74, 42);
			this.tsb_SystemSet.Text = "系统设置";
			this.tsb_SystemSet.Click += new EventHandler(this.toolStripButton4_Click);
			this.tsb_close.Alignment = ToolStripItemAlignment.Right;
			this.tsb_close.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_close.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
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
			this.tsb_windows.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.tsb_windows.ForeColor = Color.White;
			this.tsb_windows.Image = (Image)componentResourceManager.GetObject("tsb_windows.Image");
			this.tsb_windows.ImageTransparentColor = Color.Magenta;
			this.tsb_windows.Margin = new Padding(2);
			this.tsb_windows.Name = "tsb_windows";
			this.tsb_windows.Padding = new Padding(0, 13, 13, 13);
			this.tsb_windows.Size = new Size(48, 42);
			this.tsb_windows.Text = "全屏";
			this.tsb_windows.Click += new EventHandler(this.tsb_windows_Click);
			this.tsb_restart.DisplayStyle = ToolStripItemDisplayStyle.Text;
			this.tsb_restart.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.tsb_restart.ForeColor = Color.White;
			this.tsb_restart.Image = (Image)componentResourceManager.GetObject("tsb_restart.Image");
			this.tsb_restart.ImageTransparentColor = Color.Magenta;
			this.tsb_restart.Margin = new Padding(2);
			this.tsb_restart.Name = "tsb_restart";
			this.tsb_restart.Padding = new Padding(13, 13, 0, 13);
			this.tsb_restart.Size = new Size(74, 42);
			this.tsb_restart.Text = "重启系统";
			this.tsb_restart.Click += new EventHandler(this.tsb_restart_Click);
			this.timer1.Tick += new EventHandler(this.timer1_Tick);
			this.panel1.BackColor = Color.FromArgb(45, 45, 48);
			this.panel1.Controls.Add(this.grp_FaceCaptureDisplay);
			this.panel1.Controls.Add(this.grp_CompareSuccess);
			this.panel1.Controls.Add(this.grb_video);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(1, 1);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(1022, 766);
			this.panel1.TabIndex = 5;
			base.Appearance.BackColor = Color.DodgerBlue;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleMode = AutoScaleMode.None;
			base.ClientSize = new Size(1024, 768);
			base.Controls.Add(this.toolStrip1);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			this.MinimumSize = new Size(1024, 768);
			base.Name = "VideoForm";
			base.Padding = new Padding(1);
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "视频监控预警平台";
			base.FormClosing += new FormClosingEventHandler(this.VideoForm_FormClosing);
			base.FormClosed += new FormClosedEventHandler(this.VideoForm_FormClosed);
			base.Load += new EventHandler(this.Form1_Load);
			base.Shown += new EventHandler(this.VideoForm_Shown);
			this.tableLayoutPanel1.ResumeLayout(false);
			((ISupportInitialize)this.faceDetectiveControl4).EndInit();
			((ISupportInitialize)this.faceDetectiveControl3).EndInit();
			((ISupportInitialize)this.faceDetectiveControl2).EndInit();
			((ISupportInitialize)this.faceDetectiveControl1).EndInit();
			this.grb_video.ResumeLayout(false);
			this.grp_FaceCaptureDisplay.ResumeLayout(false);
			this.grp_CompareSuccess.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
