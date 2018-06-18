using System;
using System.Runtime.InteropServices;
namespace MCFaceRecognitionVideo.UnitBase
{
	public static class PlaySoundBase
	{
		public const int SND_FILENAME = 131072;
		public const int SND_ASYNC = 1;
		[DllImport("winmm.dll")]
		public static extern bool PlaySound(string pszSound, int hmod, int fdwSound);
		public static void Play(string PlaySoundPath)
		{
			PlaySoundBase.PlaySound(PlaySoundPath, 0, 131073);
		}
	}
}
