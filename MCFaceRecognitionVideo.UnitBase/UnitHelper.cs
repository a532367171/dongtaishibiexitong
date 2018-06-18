using Face.resources;
using FaceCompareThread;
using log4net;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
namespace MCFaceRecognitionVideo.UnitBase
{
	public static class UnitHelper
	{
		private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static string Post_Data = ConfigurationManager.AppSettings["PostData"];
		private static string Post_Url = ConfigurationManager.AppSettings["PostUrl"];
		private static Stopwatch _stopwatch = new Stopwatch();
		public static CameraType GetCameraTypeByName(string cameraName)
		{
			CameraType result;
			if (cameraName.Equals(UnitField.USBCamera))
			{
				result = CameraType.USBCamera;
			}
			else
			{
				if (cameraName.Equals(UnitField.Hikvision))
				{
					result = CameraType.IPCamera;
				}
				else
				{
					if (cameraName.Equals(UnitField.Dahuatech))
					{
						result = CameraType.DaHuaCamera;
					}
					else
					{
						if (cameraName.Equals(UnitField.ZTE))
						{
							result = CameraType.ZhongXingCamera;
						}
						else
						{
							if (cameraName.Equals(UnitField.Uniview))
							{
								result = CameraType.YuShiCamera;
							}
							else
							{
								result = CameraType.RTSP;
							}
						}
					}
				}
			}
			return result;
		}
		public static void PostUrl(string PostUrl, string PostData, string ID)
		{
			try
			{
				if (!PostUrl.Equals(string.Empty) && !PostData.Equals(string.Empty))
				{
					WebRequest expr_20 = WebRequest.Create(PostUrl);
					expr_20.Method = "POST";
					byte[] bytes = new UTF8Encoding().GetBytes(PostData + string.Format("&LogID={0}", ID));
					expr_20.ContentType = "application/x-www-form-urlencoded";
					expr_20.ContentLength = (long)bytes.Length;
					Stream expr_61 = expr_20.GetRequestStream();
					expr_61.Write(bytes, 0, bytes.Length);
					expr_61.Close();
				}
			}
			catch (Exception message)
			{
				UnitHelper._log.Error(message);
			}
		}
		public static void PostUrl2(string Name)
		{
			try
			{
				if (!UnitHelper.Post_Url.Equals(string.Empty))
				{
					UnitHelper._stopwatch.Restart();
					UnitHelper._stopwatch.Start();
					WebRequest expr_32 = WebRequest.Create(UnitHelper.Post_Url);
					expr_32.Method = "POST";
					byte[] bytes = new UTF8Encoding().GetBytes(string.Format("Name={0}", Name));
					expr_32.ContentType = "application/x-www-form-urlencoded";
					expr_32.ContentLength = (long)bytes.Length;
					Stream expr_6D = expr_32.GetRequestStream();
					expr_6D.Write(bytes, 0, bytes.Length);
					expr_6D.Close();
					UnitHelper._stopwatch.Stop();
					Console.WriteLine(string.Format("{0} POST Name={1}, 耗时: {2} ms", DateTime.Now.ToString("HH:mm:ss ddd"), Name, UnitHelper._stopwatch.ElapsedMilliseconds));
				}
			}
			catch (Exception arg_BA_0)
			{
				Console.WriteLine(arg_BA_0.Message);
			}
		}
	}
}
