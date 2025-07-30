using EasyWinFormLibrary.CustomControls;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EasyWinFormLibrary.WinAppNeeds
{
    public static class AdvancedMessageBox
    {
        /// <summary>
        /// Defines the type of message box to display.
        /// </summary>
        public enum MessageBoxType
        {
            Information = 0, Warning = 1, YesNo = 2
        }

        /// <summary>
        /// Shows a message box with multilingual support for Kurdish, Arabic and English languages.
        /// </summary>
        /// <param name="messageKu">The message text in Kurdish</param>
        /// <param name="messageAr">The message text in Arabic</param>
        /// <param name="messageEn">The message text in English</param>
        /// <param name="titleKu">The title text in Kurdish</param>
        /// <param name="titleAr">The title text in Arabic</param>
        /// <param name="titleEn">The title text in English</param>
        /// <param name="messageBoxType">The type of message box to display</param>
        /// <returns>Returns true if Yes was selected for YesNo type, otherwise false</returns>
        public static bool ShowMessageBox(string messageKu, string messageAr, string messageEn, string titleKu, string titleAr, string titleEn, MessageBoxType messageBoxType)
        {
            MessageBoxButtons messageBoxButton;
            MessageBoxIcon messageBoxIcon;
            string captionText = LanguageManager.SelectedLanguage == FormLanguage.Kurdish ? titleKu : LanguageManager.SelectedLanguage == FormLanguage.Arabic ? titleAr : titleEn;
            string messageText = LanguageManager.SelectedLanguage == FormLanguage.Kurdish ? messageKu : LanguageManager.SelectedLanguage == FormLanguage.Arabic ? messageAr : messageEn;

            switch (messageBoxType)
            {
                case MessageBoxType.Warning:
                    messageBoxButton = MessageBoxButtons.OK;
                    messageBoxIcon = MessageBoxIcon.Warning;
                    break;
                case MessageBoxType.Information:
                    messageBoxButton = MessageBoxButtons.OK;
                    messageBoxIcon = MessageBoxIcon.Information;
                    break;
                case MessageBoxType.YesNo:
                    messageBoxButton = MessageBoxButtons.YesNo;
                    messageBoxIcon = MessageBoxIcon.Question;
                    break;
                default:
                    messageBoxButton = MessageBoxButtons.OK;
                    messageBoxIcon = MessageBoxIcon.Information;
                    break;
            }
           ;
            return ShowMessage(messageText, captionText, messageBoxButton, messageBoxIcon) == DialogResult.Yes;
        }

        /// <summary>
        /// Internal helper method that creates and shows the actual message box form.
        /// </summary>
        /// <param name="message">The message text to display</param>
        /// <param name="caption">The caption/title text to display</param>
        /// <param name="button">The buttons to show on the message box</param>
        /// <param name="icon">The icon to display in the message box</param>
        /// <returns>The dialog result from the message box</returns>
        private static DialogResult ShowMessage(string message, string caption, MessageBoxButtons button, MessageBoxIcon icon)
        {
            using (AdvancedMessageBoxForm msgForm = new AdvancedMessageBoxForm())
            {
                msgForm.Caption = caption;
                msgForm.Message = message;
                msgForm.ShowYesOrNo = icon == MessageBoxIcon.Question;

                switch (icon)
                {
                    case MessageBoxIcon.Information:
                        msgForm.MessageIcon = GetImage(MessageBoxType.Information);
                        break;
                    case MessageBoxIcon.Warning:
                        msgForm.MessageIcon = GetImage(MessageBoxType.Warning);
                        break;
                    case MessageBoxIcon.Question:
                        msgForm.MessageIcon = GetImage(MessageBoxType.YesNo);
                        break;
                }
                return msgForm.ShowDialog();
            }
        }

        #region Base64 Images

        private const string InformationImageBase64 = "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";
        private const string WarningImageBase64 = "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";
        private const string QuestionImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAA3NCSVQICAjb4U/gAAAACXBIWXMAAAHJAAAByQFhD1RcAAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAcVQTFRF////AKqqGrPMK7/VJrPQIbXWJrfVJbnTJLfVJLbTJrjUJbjTJLbUJbbTJbfTJrfSJbfTJrfUJLjUJrfTJbfTJLfTJbfTJbfTJbfSJbfTJbfTJbfTJbfTJbfTJbfTJrfTJ7jTKLjUKbjUK7nULbrVLrrVL7rVMLvVMbvVNLzWN73XOb3XO77XPL/YPb/YPr/YP7/YQ8HZRMHZRcHZSsPaS8PbTMTbTcTbT8XbUMXcUcXcUsbcVMfcWMjdWcjeWsneW8neXMneXcneXcreXsrfX8rfYMrfYcvfY8zgZ83gac3has7ha87hbs/icNDicdDic9HjddHjddLjdtLje9TkfNTlfdTlftTlgNXlhdfmidjni9nojNnojdnokdvpldzql93qnt/sn9/soeDsouDspuLtqeLuquPuq+PurOTur+XvseXvsubvuOjxvuryv+rywerywevzw+vzx+z0x+30y+70ze71z+/10fD20vD21PH21/L32PL32vP42/P43PP43fT44/b55Pb55Pb65vf66fj76vj77Pn77fn77/r88Pr88vv89Pv99fz99/z9+P3++f3++v3++/7+/P7+/f7//v//////L+Q+dAAAAB50Uk5TAAMKDBsfPExOYmRocHSKjqSwtr7Ay9XY5ery+Pn+bfME6gAAAvZJREFUWMOtV+dfE0EQvRDSKykEkrBEEsWo2AsWLNgVLICKYgMLFuyoUVA0VkBQiLm/1+Ru97ybLXf5Je9bZt+b7M3OzM5KEgd2tz8YjiaSyUQ0HPS77VJNaPZFUsiAVMTXbFXdFIh3IAY64oEmC3Kbtw1x0ea1meldrUiIVpdQ7oghU8QcfL2zHVlAu5On96SRJaQ97OiFkGWEGLG0taAa0EJ7YP1/bsfR4avnDm3rYuyB+n6KsnZouiyrWHx0OguXQRycMH65iSVZj58XYCQNZ+GA59f7SYZ4sQWcpj4fYP4Mrcg0Pm8EGaXLX6A/siqz8L7bSNOy2gbyf/sCkfwqTN19/kXzMAnqgpylF2zgGRE8zCu/L/7Av1c3G4leXP+gfvcQ/RixbCIexkF1q/0hADbwGLOnM5rpJDZ9BNSA4iAOrEXM3q2zfVNNy51Galzpf6B/Zf+q5NessPSALlftkz6wgR7MHdQb32IjSAXkqziIwCz/rlAL+t2uL6n6UgZwI5X+n4IOzlS/oXxAb7qFN/AOclN2yU2Xat/L4uRBveEYDot8meK6Jb959zhBCvNDjlrzS0FT/XlSGcs76cWgFDbTj5K2Il9irIalqIn+mlZK46zlqJQQ629q+hvM9YSUFOrHiLw0wiYkxQ52kc70ZwDxHIg+IVPA+qV+HiUhDOIw0fdxKVHhMd5T9eVTfEpYmEhvVAcPBJSgMJUXVQf7BRQ/q5gINuCLLSNw4GaUs4Y16iE+Eegr5Uw3lP94qoSwV+AgwmhpOmydleXfE6JM8zGaquEjDg90i/RKU6Xaeg2IMy8WAzqFDgLMq02nHplZKd5Zx9Xjq426XDXcV45xNstb93Kud4J+XEvXOXrteqcGDIzb2MErjgMXf8QxFKM8w9bHREOWgivYwRRTbxiy6DGvivxXdS7Zx9KnnWaDZgV75yr6+UHmBjxWRl3UdXz0bJ6pDzV+2K573K//wdGAJ0/9j676n30NeHjW//RtwOO7tuf/PxInRcIOjW3pAAAAAElFTkSuQmCC";

        public static string[] ImagesList = {
            InformationImageBase64,
            WarningImageBase64,
            QuestionImageBase64,
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

        private static Image GetImage(MessageBoxType messageBoxType)
        {
            try
            {
                int index = (int)messageBoxType;
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
