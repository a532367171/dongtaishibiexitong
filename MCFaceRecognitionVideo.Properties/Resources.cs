using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace MCFaceRecognitionVideo.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;
		private static CultureInfo resourceCulture;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("MCFaceRecognitionVideo.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}
		internal static Bitmap top_bg
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("top_bg", Resources.resourceCulture);
			}
		}
		internal static Bitmap weiqiyong
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("weiqiyong", Resources.resourceCulture);
			}
		}
		internal static Bitmap zanwuzhaopian
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("zanwuzhaopian", Resources.resourceCulture);
			}
		}
		internal static Bitmap 分页_上一页
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("分页_上一页", Resources.resourceCulture);
			}
		}
		internal static Bitmap 分页_下一页
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("分页_下一页", Resources.resourceCulture);
			}
		}
		internal static Bitmap 分页_最前页
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("分页_最前页", Resources.resourceCulture);
			}
		}
		internal static Bitmap 分页_最后页
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("分页_最后页", Resources.resourceCulture);
			}
		}
		internal Resources()
		{
		}
	}
}
