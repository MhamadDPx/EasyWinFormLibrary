using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Validation result for control checking
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Checks if the validation passed
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Gets or sets the list of invalid controls
        /// </summary>
        public List<Control> InvalidControls { get; set; } = new List<Control>();
        /// <summary>
        /// Gets or sets the list of error messages for invalid controls
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }

    /// <summary>
    /// Helper class for checking required controls in Windows Forms
    /// </summary>
    public static class ControlRequiredCheckerExtensions
    {
        private static readonly Color ErrorColor = Color.FromArgb(255, 226, 226);
        private static readonly Color ValidColor = Color.White;

        #region Table Layout Panel Validation

        /// <summary>
        /// Checks if all required controls in a TableLayoutPanel have valid values
        /// </summary>
        /// <param name="tablePanel">The TableLayoutPanel to validate</param>
        /// <returns>True if all required controls are valid</returns>
        public static bool CheckEmptyText(this TableLayoutPanel tablePanel)
        {
            if (tablePanel == null) return true;

            bool isAllValid = true;

            foreach (Control control in tablePanel.Controls)
            {
                bool isControlValid = ValidateRequiredControl(control);
                if (!isControlValid)
                {
                    isAllValid = false;
                }
            }

            return isAllValid;
        }

        /// <summary>
        /// Validates required controls and returns detailed validation result
        /// </summary>
        /// <param name="tablePanel">The TableLayoutPanel to validate</param>
        /// <returns>Detailed validation result</returns>
        public static ValidationResult ValidateRequired(this TableLayoutPanel tablePanel)
        {
            var result = new ValidationResult { IsValid = true };

            if (tablePanel == null) return result;

            foreach (Control control in tablePanel.Controls)
            {
                if (!ValidateRequiredControl(control))
                {
                    result.IsValid = false;
                    result.InvalidControls.Add(control);
                    result.ErrorMessages.Add(GetValidationErrorMessage(control));
                }
            }

            return result;
        }

        #endregion

        #region Control Container Validation

        /// <summary>
        /// Validates all required controls in any container control
        /// </summary>
        /// <param name="container">The container control</param>
        /// <param name="recursive">Whether to validate controls in nested containers</param>
        /// <returns>True if all required controls are valid</returns>
        public static bool ValidateAllRequired(this Control container, bool recursive = false)
        {
            if (container == null) return true;

            bool isAllValid = true;

            foreach (Control control in container.Controls)
            {
                bool isControlValid = ValidateRequiredControl(control);
                if (!isControlValid)
                {
                    isAllValid = false;
                }

                // Recursively validate nested containers
                if (recursive && control.HasChildren)
                {
                    bool nestedValid = control.ValidateAllRequired(recursive);
                    if (!nestedValid)
                    {
                        isAllValid = false;
                    }
                }
            }

            return isAllValid;
        }

        /// <summary>
        /// Validates required controls in container and returns detailed result
        /// </summary>
        /// <param name="container">The container control</param>
        /// <param name="recursive">Whether to validate controls in nested containers</param>
        /// <returns>Detailed validation result</returns>
        public static ValidationResult ValidateContainerRequired(this Control container, bool recursive = false)
        {
            var result = new ValidationResult { IsValid = true };

            if (container == null) return result;

            foreach (Control control in container.Controls)
            {
                if (!ValidateRequiredControl(control))
                {
                    result.IsValid = false;
                    result.InvalidControls.Add(control);
                    result.ErrorMessages.Add(GetValidationErrorMessage(control));
                }

                // Recursively validate nested containers
                if (recursive && control.HasChildren)
                {
                    var nestedResult = control.ValidateContainerRequired(recursive);
                    if (!nestedResult.IsValid)
                    {
                        result.IsValid = false;
                        result.InvalidControls.AddRange(nestedResult.InvalidControls);
                        result.ErrorMessages.AddRange(nestedResult.ErrorMessages);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Individual Control Validation

        /// <summary>
        /// Validates a single required control
        /// </summary>
        /// <param name="control">The control to validate</param>
        /// <returns>True if the control is valid or not required</returns>
        public static bool ValidateRequiredControl(Control control)
        {
            if (control?.Tag == null)
            {
                // Not required, reset to valid color
                SetControlValidationColor(control, true);
                return true;
            }

            string tag = control.Tag.ToString().ToLower();

            if (!tag.Contains("required"))
            {
                // Not required, reset to valid color
                SetControlValidationColor(control, true);
                return true;
            }

            // Control is required, validate based on type
            bool isValid = true;

            if (tag.Contains("number"))
            {
                // Required number field - must not be empty and not zero
                isValid = !string.IsNullOrEmpty(control.Text) && control.Text.TextToDouble() != 0;
            }
            else
            {
                // Required text field - must not be empty
                isValid = !string.IsNullOrEmpty(control.Text?.Trim());
            }

            // Set visual feedback
            SetControlValidationColor(control, isValid);

            return isValid;
        }

        /// <summary>
        /// Checks if a control is marked as required
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <returns>True if the control is required</returns>
        public static bool IsRequired(this Control control)
        {
            if (control?.Tag == null) return false;
            string tag = control.Tag.ToString().ToLower();
            return tag.Contains("required");
        }

        /// <summary>
        /// Checks if a control is a required number field
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <returns>True if the control is a required number field</returns>
        public static bool IsRequiredNumber(this Control control)
        {
            if (control?.Tag == null) return false;
            string tag = control.Tag.ToString().ToLower();
            return tag.Contains("required") && tag.Contains("number");
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Sets the validation color for a control
        /// </summary>
        /// <param name="control">The control to set color for</param>
        /// <param name="isValid">Whether the control is valid</param>
        private static void SetControlValidationColor(Control control, bool isValid)
        {
            if (control == null) return;

            Color color = isValid ? ValidColor : ErrorColor;

            try
            {
                switch (control)
                {
                    case ComboBox cmb:
                    case TextBox txt:
                        control.BackColor = color;
                        break;

                    default:
                        // Handle custom controls using reflection
                        SetCustomControlColor(control, color);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting validation color: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets color for custom controls using reflection
        /// </summary>
        /// <param name="control">The custom control</param>
        /// <param name="color">The color to set</param>
        private static void SetCustomControlColor(Control control, Color color)
        {
            string typeName = control.GetType().Name.ToLower();

            // Try BackColor first
            if (TrySetProperty(control, "BackColor", color))
            {
                return;
            }

            // Try FillColor for Guna2 and custom controls
            if (typeName.Contains("guna") || typeName.Contains("auto"))
            {
                TrySetProperty(control, "FillColor", color);
            }
        }

        /// <summary>
        /// Attempts to set a property value using reflection
        /// </summary>
        /// <param name="obj">The object to set property on</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="value">The value to set</param>
        /// <returns>True if property was set successfully</returns>
        private static bool TrySetProperty(object obj, string propertyName, object value)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, value);
                    return true;
                }
            }
            catch
            {
                // Ignore if property can't be set
            }
            return false;
        }

        #endregion

        #region Error Message Generation

        /// <summary>
        /// Gets a validation error message for a control
        /// </summary>
        /// <param name="control">The invalid control</param>
        /// <returns>Error message describing the validation issue</returns>
        private static string GetValidationErrorMessage(Control control)
        {
            if (control?.Tag == null) return "Unknown validation error";

            string tag = control.Tag.ToString().ToLower();
            string controlName = !string.IsNullOrEmpty(control.Name) ? control.Name : control.GetType().Name;

            if (tag.Contains("number"))
            {
                return $"{controlName}: Required number field cannot be empty or zero";
            }
            else
            {
                return $"{controlName}: Required field cannot be empty";
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Clears all validation colors from controls in a container
        /// </summary>
        /// <param name="container">The container control</param>
        /// <param name="recursive">Whether to clear colors in nested containers</param>
        public static void ClearValidationColors(this Control container, bool recursive = false)
        {
            if (container == null) return;

            foreach (Control control in container.Controls)
            {
                SetControlValidationColor(control, true);

                if (recursive && control.HasChildren)
                {
                    control.ClearValidationColors(recursive);
                }
            }
        }

        /// <summary>
        /// Gets all required controls in a container
        /// </summary>
        /// <param name="container">The container control</param>
        /// <param name="recursive">Whether to search in nested containers</param>
        /// <returns>List of required controls</returns>
        public static List<Control> GetRequiredControls(this Control container, bool recursive = false)
        {
            var requiredControls = new List<Control>();

            if (container == null) return requiredControls;

            foreach (Control control in container.Controls)
            {
                if (control.IsRequired())
                {
                    requiredControls.Add(control);
                }

                if (recursive && control.HasChildren)
                {
                    requiredControls.AddRange(control.GetRequiredControls(recursive));
                }
            }

            return requiredControls;
        }

        #endregion
    }
}