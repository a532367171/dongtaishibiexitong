using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using Face.resources;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
namespace MCFaceRecognitionVideo.UnitBase
{
	public static class ExportToExcelHelper
	{
		private static PrintableComponentLink CreatePrintableLink(IPrintable printable)
		{
			ChartControl chartControl = printable as ChartControl;
			if (chartControl != null)
			{
				chartControl.OptionsPrint.SizeMode = PrintSizeMode.Stretch;
			}
			return new PrintableComponentLink
			{
				Component = printable
			};
		}
		public static void ExportToExcel(string title, params IPrintable[] panels)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = title;
			saveFileDialog.Title = string.Format("{0} Excel", UnitField.Export);
			saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
			if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
			{
				return;
			}
			string text = saveFileDialog.FileName;
			PrintingSystem expr_43 = new PrintingSystem();
			CompositeLink compositeLink = new CompositeLink(expr_43);
			expr_43.Links.Add(compositeLink);
			for (int i = 0; i < panels.Length; i++)
			{
				IPrintable printable = panels[i];
				compositeLink.Links.Add(ExportToExcelHelper.CreatePrintableLink(printable));
			}
			compositeLink.Landscape = true;
			try
			{
				int num = 1;
				while (File.Exists(text))
				{
					if (text.Contains(")."))
					{
						int startIndex = text.LastIndexOf("(", StringComparison.Ordinal);
						int length = text.LastIndexOf(").", StringComparison.Ordinal) - text.LastIndexOf("(", StringComparison.Ordinal) + 2;
						text = text.Replace(text.Substring(startIndex, length), string.Format("({0}).", num));
					}
					else
					{
						text = text.Replace(".", string.Format("({0}).", num));
					}
					num++;
				}
				if (text.LastIndexOf(".xlsx", StringComparison.Ordinal) >= text.Length - 5)
				{
					XlsxExportOptions options = new XlsxExportOptions();
					compositeLink.ExportToXlsx(text, options);
				}
				else
				{
					XlsExportOptions options2 = new XlsExportOptions();
					compositeLink.ExportToXls(text, options2);
				}
				if (XtraMessageBox.Show(UnitField.ExportOK, UnitField.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
				{
					Process.Start(text);
				}
			}
			catch (Exception arg_16F_0)
			{
				XtraMessageBox.Show(arg_16F_0.Message);
			}
		}
	}
}
