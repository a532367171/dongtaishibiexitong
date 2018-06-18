using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using log4net.Config;
using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo
{
	internal static class Program
	{
		public static class NativeMethods
		{
			[DllImport("kernel32.dll")]
			public static extern bool AllocConsole();
			[DllImport("kernel32.dll")]
			public static extern bool FreeConsole();
		}
		[STAThread]
		private static void Main()
		{
			SkinManager.EnableFormSkins();
			UserLookAndFeel.Default.SetSkinStyle("Visual Studio 2013 Dark");
			SkinManager.EnableFormSkins();
			AppearanceObject.DefaultFont = new Font("微软雅黑", 10f);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");
			CultureInfo cultureInfo = new CultureInfo("zh-CHS");
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)Thread.CurrentThread.CurrentCulture.DateTimeFormat.Clone();
			dateTimeFormatInfo.DateSeparator = "-";
			dateTimeFormatInfo.ShortDatePattern = "yyyy-MM-dd";
			dateTimeFormatInfo.LongDatePattern = "yyyy'年'M'月'd'日'";
			dateTimeFormatInfo.ShortTimePattern = "H:mm:ss";
			dateTimeFormatInfo.LongTimePattern = "H'时'mm'分'ss'秒'";
			cultureInfo.DateTimeFormat = dateTimeFormatInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			if (ConfigurationManager.AppSettings["AllocConsole"].Equals("true"))
			{
				Program.NativeMethods.AllocConsole();
			}
			XmlConfigurator.Configure();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (ConfigurationManager.AppSettings["MainForm"].Equals("VideoForm"))
			{
				Application.Run(new VideoForm());
				return;
			}
			Application.Run(new VideoForm2());
		}
	}
}
