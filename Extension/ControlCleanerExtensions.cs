using EasyWinFormLibrary.CustomControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Helper class for working with Windows Forms controls
    /// </summary>
    public static class ControlCleanerExtensions
    {
        #region Enums
        /// <summary>
        /// Control types for UI helper operations
        /// </summary>
        public enum ControlType
        {
            /// <summary>Default text controls for string input</summary>
            Text,

            /// <summary>Numeric input controls for integer and decimal values</summary>
            Number,

            /// <summary>Checkbox controls for boolean values</summary>
            Check,

            /// <summary>DateTime picker controls for date and time selection</summary>
            DateTime,

            /// <summary>Date-only picker controls for date selection without time</summary>
            Date
        }
        #endregion

        #region Table Layout Panel Operations

        /// <summary>
        /// Cleans all controls in a TableLayoutPanel, resetting them to default values based on their type
        /// </summary>
        /// <param name="tableLayoutPanel">The TableLayoutPanel to clean</param>
        public static void CleanControls(this TableLayoutPanel tableLayoutPanel)
        {
            if (tableLayoutPanel == null) return;

            foreach (Control control in tableLayoutPanel.Controls)
            {
                var controlType = GetControlType(control);
                ResetControlToDefault(control, controlType);
            }
        }

        /// <summary>
        /// Cleans all controls in a TableLayoutPanel with custom default values
        /// </summary>
        /// <param name="tableLayoutPanel">The TableLayoutPanel to clean</param>
        /// <param name="defaultValues">Custom default values for each control type</param>
        public static void CleanControls(this TableLayoutPanel tableLayoutPanel, Dictionary<ControlType, object> defaultValues)
        {
            if (tableLayoutPanel == null) return;

            foreach (Control control in tableLayoutPanel.Controls)
            {
                var controlType = GetControlType(control);

                object defaultValue;
                if (defaultValues != null && defaultValues.TryGetValue(controlType, out defaultValue))
                {
                    // Use provided default value
                }
                else
                {
                    // Use built-in default value
                    defaultValue = GetBuiltInDefault(controlType);
                }

                ApplyDefaultValue(control, controlType, defaultValue);
            }
        }

        /// <summary>
        /// Gets the built-in default value for a control type
        /// </summary>
        /// <param name="controlType">The control type</param>
        /// <returns>Default value for the control type</returns>
        private static object GetBuiltInDefault(ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Text: return "";
                case ControlType.Number: return "0";
                case ControlType.Check: return false;
                case ControlType.Date: return DateTime.Now.Date;
                case ControlType.DateTime: return DateTime.Now;
                default: return "";
            }
        }

        #endregion

        #region Control Container Operations

        /// <summary>
        /// Cleans all controls in any container control (Panel, GroupBox, Form, etc.)
        /// </summary>
        /// <param name="container">The container control</param>
        /// <param name="recursive">Whether to clean controls in nested containers</param>
        public static void CleanAllControls(this Control container, bool recursive = false)
        {
            if (container == null) return;

            foreach (Control control in container.Controls)
            {
                var controlType = GetControlType(control);
                ResetControlToDefault(control, controlType);

                // Recursively clean nested containers
                if (recursive && control.HasChildren)
                {
                    control.CleanAllControls(recursive);
                }
            }
        }

        #endregion

        #region Control Type Detection

        /// <summary>
        /// Determines the control type based on Tag property or control type
        /// </summary>
        /// <param name="control">The control to analyze</param>
        /// <returns>The determined ControlType</returns>
        public static ControlType GetControlType(Control control)
        {
            if (control == null) return ControlType.Text;

            string tag = control.Tag?.ToString()?.ToLower() ?? "";

            // Check tag for explicit type
            if (tag.Contains("number")) return ControlType.Number;
            if (tag.Contains("check")) return ControlType.Check;
            if (tag.Contains("datetime")) return ControlType.DateTime;
            if (tag.Contains("date")) return ControlType.Date;

            // Fallback to control type inference
            string controlTypeName = control.GetType().Name.ToLower();

            if (controlTypeName.Contains("checkbox")) return ControlType.Check;
            if (controlTypeName.Contains("datetimepicker")) return ControlType.Date;

            return ControlType.Text;
        }

        #endregion

        #region Default Value Application

        /// <summary>
        /// Resets a control to its default value based on its type
        /// </summary>
        /// <param name="control">The control to reset</param>
        /// <param name="controlType">The type of control being reset</param>
        private static void ResetControlToDefault(Control control, ControlType controlType)
        {
            if (control == null) return;

            switch (control)
            {
                // ComboBox variants
                case ComboBox cmb:
                    ResetComboBox(cmb, controlType);
                    SetControlBackColor(cmb, Color.White);
                    break;
                // ComboBox variants
                case AdvancedTextBox txt:
                    ResetTextControl(txt, controlType);
                    SetControlBackColor(txt, Color.White);
                    break;

                // TextBox variants  
                case TextBox txt:
                    ResetTextControl(txt, controlType);
                    SetControlBackColor(txt, Color.White);
                    break;

                // CheckBox variants
                case CheckBox chk:
                    ResetCheckBox(chk, controlType);
                    break;

                // DateTimePicker variants
                case DateTimePicker dtp:
                    ResetDateTimePicker(dtp, controlType);
                    break;

                // Handle custom controls by type name (for third-party controls)
                default:
                    ResetCustomControl(control, controlType);
                    break;
            }
        }

        /// <summary>
        /// Applies a default value to a control based on its type
        /// </summary>
        /// <param name="control">The control to update</param>
        /// <param name="controlType">The type of the control</param>
        /// <param name="defaultValue">The default value to apply</param>
        private static void ApplyDefaultValue(Control control, ControlType controlType, object defaultValue)
        {
            if (control == null) return;

            try
            {
                switch (controlType)
                {
                    case ControlType.Text:
                        SetControlText(control, defaultValue?.ToString() ?? "");
                        break;

                    case ControlType.Number:
                        SetControlText(control, defaultValue?.ToString() ?? "0");
                        break;

                    case ControlType.Check:
                        // For checkbox controls, check if the tag contains "check"
                        if (control is CheckBox || control.GetType().Name.ToLower().Contains("checkbox"))
                        {
                            string tag = control.Tag?.ToString()?.ToLower() ?? "";
                            bool shouldBeChecked = tag.Contains("check");
                            SetControlChecked(control, shouldBeChecked);
                        }
                        else
                        {
                            SetControlChecked(control, defaultValue is bool b ? b : false);
                        }
                        break;

                    case ControlType.Date:
                    case ControlType.DateTime:
                        SetControlDateTime(control, defaultValue is DateTime dt ? dt : DateTime.Now);
                        break;
                }

                // Set background color
                SetControlBackColor(control, Color.White);
            }
            catch (Exception ex)
            {
                // Log error if logging is available
                System.Diagnostics.Debug.WriteLine($"Error applying default value to {control.GetType().Name}: {ex.Message}");
            }
        }

        #endregion

        #region Control-Specific Reset Methods

        /// <summary>
        /// Resets a ComboBox control to its default state based on its control type
        /// </summary>
        /// <param name="cmb">The ComboBox control to reset</param>
        /// <param name="controlType">The type of control, affecting default value behavior</param>
        private static void ResetComboBox(ComboBox cmb, ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Number:
                    if (cmb.DropDownStyle == ComboBoxStyle.DropDownList)
                        cmb.SelectedIndex = -1;
                    else
                        cmb.Text = "0";
                    break;

                default:
                    cmb.SelectedIndex = -1;
                    if (cmb.DropDownStyle != ComboBoxStyle.DropDownList)
                        cmb.Text = "";
                    break;
            }
        }

        /// <summary>
        /// Resets a text-based control to its default value based on control type
        /// </summary>
        /// <param name="txt">The text control to reset</param>
        /// <param name="controlType">The type of control, determining the default format and value</param>
        private static void ResetTextControl(Control txt, ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Number:
                    txt.Text = "0";
                    break;

                case ControlType.Date:
                    txt.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    break;

                case ControlType.DateTime:
                    txt.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;

                default:
                    txt.Text = "";
                    break;
            }
        }

        /// <summary>
        /// Resets a CheckBox control based on its tag property
        /// </summary>
        /// <param name="chk">The CheckBox control to reset</param>
        /// <param name="controlType">The type of control (not used in current implementation)</param>
        /// <remarks>
        /// The checkbox will be checked if its tag contains the word "check" (case-insensitive)
        /// </remarks>
        private static void ResetCheckBox(CheckBox chk, ControlType controlType)
        {
            // Check if the tag contains "check" to determine if it should be checked
            string tag = chk.Tag?.ToString()?.ToLower() ?? "";
            chk.Checked = tag.Contains("check");
        }

        /// <summary>
        /// Resets a DateTimePicker control to its default state and format
        /// </summary>
        /// <param name="dtp">The DateTimePicker to reset</param>
        /// <param name="controlType">The type of control being reset</param>
        private static void ResetDateTimePicker(DateTimePicker dtp, ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.DateTime:
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    dtp.Value = DateTime.Now;
                    break;

                case ControlType.Date:
                default:
                    dtp.Format = DateTimePickerFormat.Short;
                    dtp.Value = DateTime.Now.Date;
                    break;
            }
            dtp.Checked = false; // Uncheck the control by default
        }

        /// <summary>
        /// Resets third-party custom controls to their default state based on control type and name pattern
        /// </summary>
        /// <param name="control">The custom control to reset</param>
        /// <param name="controlType">The type of control, determining default values and behavior</param>
        /// <remarks>
        /// Handles controls whose type names contain "guna" or "auto". Supports:
        /// - ComboBox-like controls: Resets selected index and text
        /// - TextBox-like controls: Sets default text based on control type
        /// - CheckBox-like controls: Sets checked state based on tag
        /// - DateTimePicker-like controls: Sets default date/time value
        /// Uses reflection to set common properties like FillColor.
        /// </remarks>
        private static void ResetCustomControl(Control control, ControlType controlType)
        {
            string typeName = control.GetType().Name.ToLower();

            // Handle third-party controls by name pattern
            if (typeName.Contains("guna") || typeName.Contains("auto"))
            {
                // Try to set common properties using reflection
                TrySetProperty(control, "FillColor", Color.White);

                if (typeName.Contains("combobox"))
                {
                    TrySetProperty(control, "SelectedIndex", -1);
                    if (controlType == ControlType.Number)
                        TrySetProperty(control, "Text", "0");
                    else
                        TrySetProperty(control, "Text", "");
                }
                else if (typeName.Contains("textbox"))
                {
                    string defaultText = controlType == ControlType.Number ? "0" : "";
                    TrySetProperty(control, "Text", defaultText);
                }
                else if (typeName.Contains("checkbox"))
                {
                    string tag = control.Tag?.ToString()?.ToLower() ?? "";
                    TrySetProperty(control, "Checked", tag.Contains("check"));
                }
                else if (typeName.Contains("datetimepicker"))
                {
                    DateTime defaultDate = controlType == ControlType.DateTime ? DateTime.Now : DateTime.Now.Date;
                    TrySetProperty(control, "Value", defaultDate);
                    TrySetProperty(control, "Checked", false);
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets the text property of a control with proper thread synchronization
        /// </summary>
        /// <param name="control">The control to update</param>
        /// <param name="text">The text value to set</param>
        private static void SetControlText(Control control, string text)
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => control.Text = text));
            else
                control.Text = text;
        }

        /// <summary>
        /// Sets the checked state of a control with proper thread synchronization
        /// </summary>
        /// <param name="control">The control to update</param>
        /// <param name="isChecked">The checked state to set</param>
        private static void SetControlChecked(Control control, bool isChecked)
        {
            if (control is CheckBox chk)
            {
                if (control.InvokeRequired)
                    control.Invoke(new Action(() => chk.Checked = isChecked));
                else
                    chk.Checked = isChecked;
            }
            else
            {
                TrySetProperty(control, "Checked", isChecked);
            }
        }

        /// <summary>
        /// Sets the date/time value of a control with proper thread synchronization
        /// </summary>
        /// <param name="control">The control to update</param>
        /// <param name="dateTime">The date/time value to set</param>
        private static void SetControlDateTime(Control control, DateTime dateTime)
        {
            if (control is DateTimePicker dtp)
            {
                if (control.InvokeRequired)
                    control.Invoke(new Action(() => dtp.Value = dateTime));
                else
                    dtp.Value = dateTime;
            }
            else
            {
                TrySetProperty(control, "Value", dateTime);
            }
        }

        /// <summary>
        /// Sets the background color of a control with proper thread synchronization
        /// </summary>
        /// <param name="control">The control to update</param>
        /// <param name="color">The color to set</param>
        private static void SetControlBackColor(Control control, Color color)
        {
            try
            {
                // Try BackColor first
                if (control.GetType().GetProperty("BackColor") != null)
                {
                    if (control.InvokeRequired)
                        control.Invoke(new Action(() => control.BackColor = color));
                    else
                        control.BackColor = color;
                }
                // Try FillColor for custom controls
                else
                {
                    TrySetProperty(control, "FillColor", color);
                }
            }
            catch
            {
                // Ignore if property doesn't exist
            }
        }

        /// <summary>
        /// Attempts to set a property value on an object using reflection
        /// </summary>
        /// <param name="obj">The object to update</param>
        /// <param name="propertyName">The name of the property to set</param>
        /// <param name="value">The value to set</param>
        private static void TrySetProperty(object obj, string propertyName, object value)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
            }
            catch
            {
                // Ignore if property can't be set
            }
        }

        #endregion

        #region Validation Helpers

        /// <summary>
        /// Validates all controls in a container and highlights invalid ones
        /// </summary>
        /// <param name="container">The container to validate</param>
        /// <param name="errorColor">Color for invalid controls</param>
        /// <returns>True if all controls are valid</returns>
        public static bool ValidateControls(this Control container, Color? errorColor = null)
        {
            Color invalidColor = errorColor ?? Color.LightCoral;
            bool isValid = true;

            foreach (Control control in container.Controls)
            {
                var controlType = GetControlType(control);
                bool controlValid = ValidateControl(control, controlType);

                if (!controlValid)
                {
                    SetControlBackColor(control, invalidColor);
                    isValid = false;
                }
                else
                {
                    SetControlBackColor(control, Color.White);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validates a single control based on its type
        /// </summary>
        /// <param name="control">The control to validate</param>
        /// <param name="controlType">The type of control being validated</param>
        /// <returns>True if the control's value is valid for its type</returns>
        private static bool ValidateControl(Control control, ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Number:
                    return double.TryParse(control.Text, out _);

                case ControlType.Date:
                case ControlType.DateTime:
                    return DateTime.TryParse(control.Text, out _);

                default:
                    return true; // Text and Check controls are always valid
            }
        }

        #endregion
    }
}