using EasyWinFormLibrary.CustomControls;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EasyWinFormLibrary.WinAppNeeds
{
    public static class AdvancedAlert
    {
        /// <summary>
        /// Shows an alert message with multi-language support and customizable appearance.
        /// </summary>
        /// <param name="messageKu">The message text in Kurdish</param>
        /// <param name="messageAr">The message text in Arabic</param>
        /// <param name="messageEn">The message text in English</param>
        /// <param name="alertType">The type of alert to display (Success, Info, Error, Warning)</param>
        /// <param name="visableSeconds">Number of seconds to display the alert before auto-closing (default: 3)</param>
        public static void ShowAlert(string messageKu, string messageAr, string messageEn, AlertType alertType, int visableSeconds = 3)
        {
            AdvancedAlertForm frm = new AdvancedAlertForm();
            frm.CountDown = visableSeconds;
            frm.MessageIcon = GetImage(alertType);

            switch (alertType)
            {
                case AlertType.Success:
                    frm.BackColor = frm.lblMessage.BackColor = Color.MediumSeaGreen;
                    break;
                case AlertType.Info:
                    frm.BackColor = frm.lblMessage.BackColor = Color.FromArgb(0, 122, 204);
                    break;
                case AlertType.Error:
                    frm.BackColor = frm.lblMessage.BackColor = Color.FromArgb(190, 75, 72);
                    break;
                case AlertType.Warning:
                    frm.BackColor = frm.lblMessage.BackColor = Color.FromArgb(255, 139, 45);
                    break;
            }
            switch (LanguageManager.SelectedLanguage)
            {
                case FormLanguage.English:
                    frm.Message = messageEn;
                    break;
                case FormLanguage.Kurdish:
                    frm.Message = messageKu;
                    break;
                case FormLanguage.Arabic:
                    frm.Message = messageAr;
                    break;
            }
            frm.RightToLeft = LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
            frm.Show();
        }

        /// <summary>
        /// Defines the available types of alerts that can be displayed.
        /// </summary>
        public enum AlertType 
        { 
            /// <summary>Success alert type with green color scheme</summary>
            Success = 0,
            /// <summary>Information alert type with blue color scheme</summary>
            Info = 1, 
            /// <summary>Error alert type with red color scheme</summary>
            Error = 2,
            /// <summary>Warning alert type with orange color scheme</summary>
            Warning = 3 
        }

        #region Base64 Images

        private const string SuccessImageBase64 = "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";
        private const string InfoImageBase64 = "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";
        private const string ErrorImageBase64 = "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";
        private const string WarningImageBase64 = "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";

        public static string[] ImagesList = {
            SuccessImageBase64,
            InfoImageBase64,
            ErrorImageBase64,
            WarningImageBase64,
        };

        #endregion

        #region Image Helper Methods

        private static Image GetIcon(string imageBase64)
        {
            try
            {
                if (string.IsNullOrEmpty(imageBase64)) return null;

                byte[] imageBytes = Convert.FromBase64String(imageBase64);
                using (var ms = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon: {ex.Message}");
                return null;
            }
        }

        private static Image GetImage(AlertType alertType)
        {
            try
            {
                int index = (int)alertType;
                if (index >= 0 && index < ImagesList.Length)
                {
                    return GetIcon(ImagesList[index]);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting button image: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}
