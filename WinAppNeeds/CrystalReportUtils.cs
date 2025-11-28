using CrystalDecisions.CrystalReports.Engine;
using EasyWinFormLibrary.CustomControls;
using EasyWinFormLibrary.Data;
using System.Drawing;

namespace EasyWinFormLibrary.WinAppNeeds
{
    public static class CrystalReportUtils
    {
        public enum ReportSize { A4, A3 }
        public enum ReportOrientation { Portrait, Landscape }
        public enum PrintToPrinterType { ShowDialog, PrintDirect }

        public static void PrintDialog(this ReportDocument report, PrintToPrinterType printToPrinterType)
        {
            if (printToPrinterType == PrintToPrinterType.ShowDialog)
            {
                using (frm_CrtViewer frm = new frm_CrtViewer())
                {
                    frm.crt_Viewer.ReportSource = report;
                    frm.ShowDialog();
                }
            }
            else if (printToPrinterType == PrintToPrinterType.PrintDirect)
            {
                report.PrintToPrinter(1, false, 0, 0);
            }
        }

        public async static void FillReportCaption(this ReportDocument report, string ReportTitleKu, string ReportTitleAr, string ReportTitleEn, ReportOrientation Orientation, ReportLanguage language, bool useLandscapeLogo, bool useHeaderImage, bool useFooterImage, ReportSize reportSize = ReportSize.A4, bool DoubleCaption = false)
        {
            ReportDocument SubHeader1 = null;
            ReportDocument SubFooter1 = null;
            ReportDocument SubHeader2 = null;
            ReportDocument SubFooter2 = null;

            if (reportSize == ReportSize.A4)
            {
                SubHeader1 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionHeader.rpt" : "cr_CaptionHeaderLandscape.rpt"];
                SubFooter1 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionFooter.rpt" : "cr_CaptionFooterLandscape.rpt"];
            }
            else if (reportSize == ReportSize.A3)
            {
                SubHeader1 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionHeaderA3.rpt" : "cr_CaptionHeaderLandscapeA3.rpt"];
                SubFooter1 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionFooterA3.rpt" : "cr_CaptionFooterLandscapeA3.rpt"];
            }
            if (useLandscapeLogo)
                SubHeader1.ReportDefinition.ReportObjects["cologo1"].Width = SubHeader1.ReportDefinition.ReportObjects["cologo2"].Width = SubHeader1.ReportDefinition.ReportObjects["cologo3"].Width = 4000;

            if (useLandscapeLogo)
                SubHeader1.ReportDefinition.ReportObjects["cologo3"].Left = Orientation == ReportOrientation.Portrait ? 7080 : 12120;

            if (DoubleCaption)
            {
                if (reportSize == ReportSize.A4)
                {
                    SubHeader2 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionHeader.rpt2" : "cr_CaptionHeaderLandscape.rpt2"];
                    SubFooter2 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionFooter.rpt2" : "cr_CaptionFooterLandscape.rpt2"];
                }
                else if (reportSize == ReportSize.A3)
                {
                    SubHeader2 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionHeaderA3.rpt2" : "cr_CaptionHeaderLandscapeA3.rpt2"];
                    SubFooter2 = report.Subreports[Orientation == ReportOrientation.Portrait ? "cr_CaptionFooterA3.rpt2" : "cr_CaptionFooterLandscapeA3.rpt2"];
                }

                if (useLandscapeLogo)
                    SubHeader2.ReportDefinition.ReportObjects["cologo1"].Width = SubHeader1.ReportDefinition.ReportObjects["cologo2"].Width = SubHeader1.ReportDefinition.ReportObjects["cologo3"].Width = 4000;

                if (useLandscapeLogo)
                    SubHeader2.ReportDefinition.ReportObjects["cologo3"].Left = Orientation == ReportOrientation.Portrait ? 7080 : 12120;
            }

            Color ReportColor = ColorTranslator.FromHtml((await SqlDatabaseActions.GetSingleValueAsync("SELECT company_color FROM tbl_system_settings")).Value);

            var companyData = await SqlDatabaseActions.GetDataAsync($"SELECT co_name,co_job,co_address,co_email,co_website,co_phone1,co_phone2,co_note,{(useLandscapeLogo ? "co_logo_landscape" : "co_logo")} AS co_logo,co_color,N'{(language == ReportLanguage.Ku ? ReportTitleKu : language == ReportLanguage.Ar ? ReportTitleAr : ReportTitleEn)}' AS report_title,N'{AuthUserInfo.Fullname}' AS print_by,N'{language}' AS report_language,{ReportColor.R} AS co_color_red,{ReportColor.G} AS co_color_green,{ReportColor.B} AS co_color_blue,{(useHeaderImage ? 1 : 0)} AS use_image_header,co_header_image_portrait{(reportSize == ReportSize.A3 ? "_a3" : "")}_{language} AS co_header_image_portrait,co_header_image_landscape{(reportSize == ReportSize.A3 ? "_a3" : "")}_{language} AS co_header_image_landscape,{(useFooterImage ? 1 : 0)} AS use_image_footer,co_footer_image_portrait{(reportSize == ReportSize.A3 ? "_a3" : "")}_{language} AS co_footer_image_portrait,co_footer_image_landscape{(reportSize == ReportSize.A3 ? "_a3" : "")}_{language} AS co_footer_image_landscape FROM func_caption('{language}');");
            SubHeader1.SetDataSource(companyData.Data);
            SubFooter1.SetDataSource(companyData.Data);
            if (DoubleCaption)
            {
                SubHeader2.SetDataSource(companyData.Data);
                SubFooter2.SetDataSource(companyData.Data);
            }
        }

    }
}
