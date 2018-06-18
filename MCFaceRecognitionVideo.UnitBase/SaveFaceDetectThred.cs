using Emgu.CV;
using Emgu.CV.Structure;
using FaceCompareBase;
using log4net;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
namespace MCFaceRecognitionVideo.UnitBase
{
	public class SaveFaceDetectThred
	{
		private Image<Bgr, byte> _image;
		private string _channel;
		private volatile bool _IsStop;
		private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
		private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly object _obj = new object();
		private readonly bool _isCapturePhotos;
		private readonly string _photoType;
		private readonly string _photoSavePath;
		public SaveFaceDetectThred()
		{
			this._isCapturePhotos = XMLHelper.getXmlValue("PhotoSetting", "IsCapturePhotos").Equals("是");
			this._photoType = XMLHelper.getXmlValue("PhotoSetting", "PhotoType");
			this._photoSavePath = XMLHelper.getXmlValue("PhotoSetting", "PhotoSavePath");
			if (!Directory.Exists(this._photoSavePath))
			{
				Directory.CreateDirectory(this._photoSavePath);
			}
		}
		public void Execute()
		{
			while (!this._IsStop)
			{
				this._autoResetEvent.WaitOne();
				if (!this._IsStop)
				{
					try
					{
						if (this._image != null)
						{
							Image<Bgr, byte> image = this._image.Clone();
							if (this._isCapturePhotos)
							{
								string text = string.Concat(new string[]
								{
									this._photoSavePath,
									"\\",
									DateTime.Now.ToString("yyyyMMdd"),
									"\\",
									this._channel
								});
								if (!Directory.Exists(text))
								{
									Directory.CreateDirectory(text);
								}
								if (this._photoType.Equals("cut"))
								{
									image.Bitmap.Save(text + "\\" + DateTime.Now.ToString("yyyyMMdd_HHssmmfff") + ".jpg", ImageFormat.Jpeg);
								}
								else
								{
									image.Bitmap.Save(text + "\\" + DateTime.Now.ToString("yyyyMMdd_HHssmmff") + ".jpg", ImageFormat.Jpeg);
								}
							}
						}
					}
					catch (Exception message)
					{
						this._log.Error(message);
					}
				}
			}
		}
		public void Start(byte[] image, string channel)
		{
			this._image = new Image<Bgr, byte>(FaceImageFormat.ByteToBitmap(image));
			this._channel = channel;
			this._autoResetEvent.Set();
		}
		public void Stop()
		{
			this._IsStop = true;
			this._autoResetEvent.Set();
		}
	}
}
