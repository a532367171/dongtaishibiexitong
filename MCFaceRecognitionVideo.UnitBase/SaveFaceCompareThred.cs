using FaceCompareThread;
using log4net;
using MC_DAL;
using MC_DAL.Entity;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
namespace MCFaceRecognitionVideo.UnitBase
{
	public class SaveFaceCompareThred
	{
		private string _channel;
		private volatile bool _IsStop;
		private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
		private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly FaceCompareLogService _faceCompareLogService = new FaceCompareLogService();
		private ResultInfo _recognition;
		private FaceCompareLog _faceCompareLog;
		private byte[] _FaceImage;
		private static readonly object _obj = new object();
		public void Execute()
		{
			while (!this._IsStop)
			{
				this._autoResetEvent.WaitOne();
				if (!this._IsStop)
				{
					try
					{
						ResultInfo recognition = this._recognition;
						string channel = this._channel;
						byte[] faceImage = this._FaceImage;
						this._faceCompareLog = new FaceCompareLog
						{
							FaceDetcetDate = DateTime.Now.ToString(),
							FaceDetcetImage = Convert.ToBase64String(faceImage),
							FaceTempateImage = recognition.FaceTemplate.ImageLocation,
							PersonID = (long)Convert.ToInt16(recognition.FaceTemplate.PersonId),
							PersonName = recognition.FaceTemplate.PersonName,
							PersonNumber = recognition.FaceTemplate.PersonNumber,
							Similarity = recognition.Score.ToString("P"),
							tmp1 = channel,
							tmp2 = (recognition.FaceTemplate.PersonType == PersonType.Black) ? "黑名单" : "白名单"
						};
						this._faceCompareLogService.Add(this._faceCompareLog);
					}
					catch (Exception message)
					{
						SaveFaceCompareThred._log.Error(message);
					}
				}
			}
		}
		public void Start(ResultInfo recognition, string channel, byte[] image)
		{
			this._recognition = recognition;
			this._channel = channel;
			this._FaceImage = image;
			this._autoResetEvent.Set();
		}
		public void Stop()
		{
			this._IsStop = true;
			this._autoResetEvent.Set();
		}
		private string ImgToBase64String(Bitmap bitmap)
		{
			MemoryStream memoryStream = null;
			byte[] array = new byte[0];
			try
			{
				memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Jpeg);
				array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, (int)memoryStream.Length);
			}
			catch (Exception message)
			{
				SaveFaceCompareThred._log.Error(message);
				return string.Empty;
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
			}
			return Convert.ToBase64String(array);
		}
		private string ReadImageFile(string path)
		{
			FileStream fileStream = null;
			byte[] inArray = new byte[0];
			try
			{
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
				BinaryReader expr_18 = new BinaryReader(fileStream);
				expr_18.BaseStream.Seek(0L, SeekOrigin.Begin);
				inArray = expr_18.ReadBytes((int)expr_18.BaseStream.Length);
			}
			catch
			{
				return string.Empty;
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
			return Convert.ToBase64String(inArray);
		}
		public static void AddFaceCompare(ResultInfo recognition, string channel, byte[] faceImage, string cameraIP)
		{
			FaceCompareLogService faceCompareLogService = new FaceCompareLogService();
			string tmp = "VIP";
			switch (recognition.FaceTemplate.PersonType)
			{
			case PersonType.Black:
				tmp = "黑名单";
				break;
			case PersonType.White:
				tmp = "白名单";
				break;
			case PersonType.VIP:
				tmp = "VIP";
				break;
			}
			try
			{
				FaceCompareLog faceCompareLog = new FaceCompareLog
				{
					FaceDetcetDate = DateTime.Now.ToString("yyyy\\/M\\/d HH:mm:ss"),
					FaceDetcetImage = Convert.ToBase64String(faceImage),
					FaceTempateImage = recognition.FaceTemplate.ImageLocation,
					PersonID = (long)Convert.ToInt16(recognition.FaceTemplate.PersonId),
					PersonName = recognition.FaceTemplate.PersonName,
					PersonNumber = recognition.FaceTemplate.PersonNumber,
					Similarity = recognition.Score.ToString("P"),
					tmp1 = channel,
					tmp2 = tmp,
					tmp3 = cameraIP
				};
				object obj = SaveFaceCompareThred._obj;
				lock (obj)
				{
					faceCompareLogService.Add(faceCompareLog);
				}
				UnitHelper.PostUrl2(recognition.FaceTemplate.PersonName);
			}
			catch (Exception message)
			{
				SaveFaceCompareThred._log.Error(message);
			}
		}
	}
}
