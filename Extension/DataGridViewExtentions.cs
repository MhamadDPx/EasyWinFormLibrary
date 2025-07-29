using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Button types for DataGridView image columns
    /// </summary>
    public enum ButtonImage
    {
        Print = 0,
        Update = 1,
        Delete = 2,
        Add = 3,
        View = 4,
        Download = 5,
        Upload = 6,
        Settings = 7
    }

    /// <summary>
    /// Extension methods for DataGridView
    /// </summary>
    public static class DataGridViewExtentions
    {
        /// <summary>
        /// Checks if DataGridView has any rows
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>True if DataGridView has rows</returns>
        public static bool HasRow(this DataGridView dataGridView)
        {
            return dataGridView?.Rows?.Count > 0;
        }

        /// <summary>
        /// Checks if DataGridView is empty (no rows)
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>True if DataGridView is empty</returns>
        public static bool IsEmpty(this DataGridView dataGridView)
        {
            return dataGridView?.Rows?.Count == 0;
        }

        /// <summary>
        /// Gets the number of rows safely (excluding new row if present)
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>Number of rows, or 0 if null</returns>
        public static int RowCount(this DataGridView dataGridView)
        {
            if (dataGridView?.Rows == null) return 0;

            int count = dataGridView.Rows.Count;

            // Exclude the "new row" if AllowUserToAddRows is true
            if (dataGridView.AllowUserToAddRows && count > 0)
                count--;

            return count;
        }

        /// <summary>
        /// Checks if DataGridView has any selected rows
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>True if DataGridView has selected rows</returns>
        public static bool HasSelectedRow(this DataGridView dataGridView)
        {
            return dataGridView?.SelectedRows?.Count > 0;
        }

        /// <summary>
        /// Checks if DataGridView has more than specified number of rows
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <param name="count">Minimum row count</param>
        /// <returns>True if DataGridView has more rows than specified</returns>
        public static bool HasMoreThan(this DataGridView dataGridView, int count)
        {
            return dataGridView.RowCount() > count;
        }

        /// <summary>
        /// Gets the selected row safely
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>First selected row or null</returns>
        public static DataGridViewRow GetSelectedRow(this DataGridView dataGridView)
        {
            return dataGridView?.SelectedRows?.Count > 0 ? dataGridView.SelectedRows[0] : null;
        }

        /// <summary>
        /// Hides specified columns in DataGridView
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="hideIgnore">Whether to tag columns as "HideIgnore"</param>
        /// <param name="columns">Column names to hide</param>
        public static void HideColumns(this DataGridView dataGridView, bool hideIgnore, params string[] columns)
        {
            if (dataGridView?.Columns == null || columns == null) return;

            foreach (string columnName in columns)
            {
                if (string.IsNullOrEmpty(columnName)) continue;

                try
                {
                    if (dataGridView.Columns.Contains(columnName))
                    {
                        dataGridView.Columns[columnName].Visible = false;

                        if (hideIgnore)
                            dataGridView.Columns[columnName].Tag = "HideIgnore";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Column '{columnName}' not found in DataGridView");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding column '{columnName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Hides columns by index
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="hideIgnore">Whether to tag columns as "HideIgnore"</param>
        /// <param name="columnIndexes">Column indexes to hide</param>
        public static void HideColumns(this DataGridView dataGridView, bool hideIgnore, params int[] columnIndexes)
        {
            if (dataGridView?.Columns == null || columnIndexes == null) return;

            foreach (int index in columnIndexes)
            {
                try
                {
                    if (index >= 0 && index < dataGridView.Columns.Count)
                    {
                        dataGridView.Columns[index].Visible = false;

                        if (hideIgnore)
                            dataGridView.Columns[index].Tag = "HideIgnore";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Column index '{index}' is out of range");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error hiding column at index '{index}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Shows specified columns in DataGridView
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="columns">Column names to show</param>
        public static void ShowColumns(this DataGridView dataGridView, params string[] columns)
        {
            if (dataGridView?.Columns == null || columns == null) return;

            foreach (string columnName in columns)
            {
                if (string.IsNullOrEmpty(columnName)) continue;

                try
                {
                    if (dataGridView.Columns.Contains(columnName))
                    {
                        dataGridView.Columns[columnName].Visible = true;
                        dataGridView.Columns[columnName].Tag = null; // Clear tag
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error showing column '{columnName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Shows all columns except those tagged as "HideIgnore"
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        public static void ShowAllColumns(this DataGridView dataGridView)
        {
            if (dataGridView?.Columns == null) return;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.Tag?.ToString() != "HideIgnore")
                {
                    column.Visible = true;
                }
            }
        }

        /// <summary>
        /// Hides all columns except the specified ones
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="visibleColumns">Column names to keep visible</param>
        public static void HideAllExcept(this DataGridView dataGridView, params string[] visibleColumns)
        {
            if (dataGridView?.Columns == null) return;

            var visibleSet = new HashSet<string>(visibleColumns ?? new string[0]);

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.Visible = visibleSet.Contains(column.Name);
            }
        }

        /// <summary>
        /// Gets all hidden column names
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>List of hidden column names</returns>
        public static List<string> GetHiddenColumns(this DataGridView dataGridView)
        {
            var hiddenColumns = new List<string>();

            if (dataGridView?.Columns == null) return hiddenColumns;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (!column.Visible)
                {
                    hiddenColumns.Add(column.Name);
                }
            }

            return hiddenColumns;
        }

        /// <summary>
        /// Gets all visible column names
        /// </summary>
        /// <param name="dataGridView">The DataGridView to check</param>
        /// <returns>List of visible column names</returns>
        public static List<string> GetVisibleColumns(this DataGridView dataGridView)
        {
            var visibleColumns = new List<string>();

            if (dataGridView?.Columns == null) return visibleColumns;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.Visible)
                {
                    visibleColumns.Add(column.Name);
                }
            }

            return visibleColumns;
        }

        /// <summary>
        /// Toggles visibility of specified columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="columns">Column names to toggle</param>
        public static void ToggleColumns(this DataGridView dataGridView, params string[] columns)
        {
            if (dataGridView?.Columns == null || columns == null) return;

            foreach (string columnName in columns)
            {
                if (string.IsNullOrEmpty(columnName)) continue;

                try
                {
                    if (dataGridView.Columns.Contains(columnName))
                    {
                        dataGridView.Columns[columnName].Visible = !dataGridView.Columns[columnName].Visible;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error toggling column '{columnName}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sets column widths for specified columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="columnWidths">Dictionary of column names and their widths</param>
        public static void SetColumnWidths(this DataGridView dataGridView, Dictionary<string, int> columnWidths)
        {
            if (dataGridView?.Columns == null || columnWidths == null) return;

            foreach (var kvp in columnWidths)
            {
                try
                {
                    if (dataGridView.Columns.Contains(kvp.Key))
                    {
                        dataGridView.Columns[kvp.Key].Width = kvp.Value;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting width for column '{kvp.Key}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Auto-sizes all visible columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to modify</param>
        /// <param name="mode">The auto size mode</param>
        public static void AutoSizeVisibleColumns(this DataGridView dataGridView, DataGridViewAutoSizeColumnsMode mode = DataGridViewAutoSizeColumnsMode.AllCells)
        {
            if (dataGridView?.Columns == null) return;

            try
            {
                dataGridView.AutoSizeColumnsMode = mode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error auto-sizing columns: {ex.Message}");
            }
        }


        #region Base64 Images

        private const string PrintImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAABAlJREFUeJztm0924kYQxr9qYTK7aD/hRTnBkBMMPoHhQbIdzcJkduEGcU4QZ+cHC8vbCX4iJwicYJgTjBwm68BuwtBdWSASjBG0hETz77eCpqtV+qxuVVW3CQnwHdeW6uwVGGWASknGSA/ugtCp/dn6NYk1xTVoF+plALcA7CQXzJBAKa58/1erH8colgD3hUuXQbfx/NoqQ6X4PI4I2gL4zhtHSvUOu/eXXySwrPG3lcAb6nQWuqNKqa6w+zcPAI5SeVe3cy7GwBdL2u4sa9zQVTttfMe1lcx3GHg5387AKwDXOmNoTYG3zy+LQtC7R42Mh9rHpqPpa2b4jmtLmf97sb02aGrdm9YUILKePPpECHRssyZ8+t4ntddeA3YZAhJPwYMQYBNOAph2wDQnAUw7YJqTAKYdMM1JANMOmOboBViaMPiOa3+enL2YfRcCRTA9zq4IfaW4kbF/WghB12AU59sUc2n++1nu8/tlWesjAcKs7ycA5Swc3QE6SvHP8xWj/wTYg3JXahD4dXXQ8qafcVw3P2MmAoUFhQ/Yj3JXmgwta/xNTk6+uALxYdw84wHEfQLZDBQBfLmitz1R+YaA4JcrOu0Td1ZuXKwNWuXqoFmyrLED8O+rDAi4oHahzltyMDMI6FUHzdJiu++4tpzk+yB8HWV7IIEQe8tap+996qyyPAgBFFtB1G8seGW98CAEgFClqJ+I4aw0TdsXExDjR99xn7zJ3j6/LGK6SRJtewiLYEjAoEbO+qcHPLOlkhdgusKa+CbO1tiu4xC4I2UegILuvu9BTIFNOAlg2gHTHL0AiRfBqPDTBPeFenfxjIAuR/8EnAQw7YBpUg2EFqvJWRBV3U1KqgJMJs+KgtQfaY655BrnALppjSewwfmafYeAniCCZ9oRYxA6QoixB2Bk2hcDjIQYe7lK4A3bhboLwN90RGY5FES9zX2LRrFMawF0K4E3zAFAbdDs3BcuXzPoGqtLySsJt5xKKTmYFSMCN6qDVgeYiwOqg5ZnWaII4A6HOSVGAO4sSxRn22LAwmuwEtwEAFxgWk6anRAVQhWZ8cu6K0w3V9f3W0dt0DrfdAxmcf7/ZzmMOkIfGQfMG/z21RsQqbUXnQoWXaDcJt99vOnq9Dv6UPgkgGkHTJNqLpDLfeqHsfrekKoAYZbWTXPMrDmIKcCUPHjTEoCXhJ+M6C3nbeI7rr14QiwOWgKEMcFidOi0Cz/cLtuT2xbT4z1ny3IY7RQ/zhrQwZONRnalzLvtQj3GMOkh5fL2OCm+9hpgWeIK+5AjMB7CFF8LbQEqwU1A2I2ToSsYKeZynJphrLdAmEVVsItPAuNBKS5l+s/TM3zHtZXKu2CUk+7IpAUBPYC9+RQ3Dv8CfDBn+eF40uIAAAAASUVORK5CYII=";
        private const string EditImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAuNJREFUeJztmj122kAUhe8bnDqUOZYLZQd0KRNvIIcdBOEmDaAd5HgFUXANAzvgZAORy1SJVxAKk5MSp8Z6KcBOMBIzGv0wnMxXopnHu3f+RwIcDofD4XA4HI6jwgtl66w3fl00jigjmbrxBvIjEv7GhNgbSFkkFpWVVF14AynB3Nn6kWiy+BQEJvGOrwcw7zYac8frjUKTcEdjwFko3wDAYtjtAJjuFCBqmcQ9CgO8gZSc8BevN+oAGSYIikxiW2/A1pgnkqkmMAeLKPhuEt/qSTB1wgPWgq8uJsB6aNxGQWz6H9YakCn+gX9MKIKVQ0ApvkSs6wFa4ktqfcAyA+oWD1hkwCHEAwYG+KFsrhK8Y+Y2CE0A+TYgKSJMxW/qtcBYEtHsRGA6j4JlnnRyGXDalx8IHAJo5qn3SPnin9ZbMij6OQwudVPSNqDwzFy9+L/kOBw1dAp5/XEE4L1O2VTqFL+m9fzVW/r99XOsSk3ZA16E0m8k/ENVLpP6xT9yL+jlryiY7yuj3Ag1EjY6ZgI4qHhAL3ednaDZtdOBxW9QrlAnRYIQ+BJCxE9/Z2C5iLpbpzNT8X4omytgCqLdOwAAnHAbwCAjorLxlHOA1x9z1rPFsKu1ilS9ySmSY+WHoUPt8HSp1ACvNwptFg9U3QOI2nufH1g8UP0QyJ6ELBAPVGjAwy1uKpaIByo0YLM8pTywRzygtw8wggB/szZdEziGEHGRy8uqqMyA22F3/wRoCVZeitaJM6BIZT+UZjdDJVI0h0JzwOo+kad9afRKqixW90kLlLndv1PV1zHgGhkbGiZqEzKWu5rgbPEAoGwc9RBgnuXIxy40clca8KwhJtDoShZyt8l9L0oD5lGwBBe4FjsUzKHOOwL9a/HeqAOiQh8k1UaO7bb2Mri4upiQoHMAN6Z51cANCTrPc9YwejfohbJFSdJmkA/AN4lRInMCz1mImelXIg6Hw+FwOBwOh8Px3/EHRMWyhUpvZbwAAAAASUVORK5CYII=";
        private const string DeleteImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAsNJREFUeJztm01y2kAQRr8epD03CNk6JgU7F4ayOEHICcwVuAE3iI9AThByAouS8JZUHJItvgFsDZ7Ohrj4ke2RGKmpMK/KmynRfvMhqY1RE3JiEgRlb7W6JuYOgCBjmZCJhivP+1oPw7lFvWcoj6LTRqPGRLcAypZKzom5/eHu7oeles9YD+DPxUXlqVSawN7m/zFf+v5722eCslkMAJ5KpT7sbx4Ayv5yeWO7qPUAAFznUDO32lYvgftmMyDm2911pdTnsygapqn1u9XqaK2/7a4zUbsax+EBmttutgq9BDGP0m4eAM6iaEjMozycNnnzDJg2GjUAn0yKMVGA/ZY3I+ZBWrF1vS6AytYa81ABpt3g+1ud49UA1u1sYvjLjhJirr8WQmIAP1utKwBQWncBdHMxK46BVmoAAB+jaO+S8pJeobQO83UqlO76jQQS3vDcb4LHjgtAWkCaxHtAEf3X4TgOjD4LrP8g+pK3jE2IuWfy/4PEe8AuWqkyMQcHWxWIVsroI/nJdwEXgLSANC4AaQFpjLpACh6YaLC5oLQOmOhqc42YR1qpMMtxxNwF8M6WsNUAiHl2Ph73N9fum80+MW9tTCsVVuM403HTRiNgImsBnPwl4AKQFpDGBSAtII0LQFpAGheAtIA0LgBpAWlcANIC0rgApAWkcQFIC0jjApAWkMYFIC0gjQtAWkAaF4C0gDQuAGkBaU4+AKOHpF4ahEhgjv1H2SvYeeQdwGz9k+W4GgxGckwHK2x/PV6G2YhcBfubPeS4zPy3l4DS2mi6zHhm6NflJWfXKZ7z8dhob2nOgIeMLhIYuxoHsPvoyzGTxtU4gJXn3RCwyGRUIAQsVp5nPGBpHEA9DOeaqJNNqzg0USfNeG2qLlCN45CJ2sd4JhCwIOZ62qHK1G2wGsfho+9XAPSOYbBi7dB79P1Klunyv7vz35+tuznWAAAAAElFTkSuQmCC";

        // Additional icons
        private const string AddImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAPNJREFUeJzt21EOADEMA9Bx/53Y/9xYAp5Iaj8yMzMzMzMzMzMzMzMzMzMzMzMzM79gZq1k1/7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM5Xs2h2YqWTX7sBMJbt2B2Yq2bU7MFPJrt2BmUp27Q7MVLJrd2Cmkl27AzOV7NodmKlk1+7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM/UD/gDSqhX9iOUAAAAASUVORK5CYII=";
        private const string ViewImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAPNJREFUeJzt21EOADEMA9Bx/53Y/9xYAp5Iaj8yMzMzMzMzMzMzMzMzMzMzMzMzM79gZq1k1/7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM5Xs2h2YqWTX7sBMJbt2B2Yq2bU7MFPJrt2BmUp27Q7MVLJrd2Cmkl27AzOV7NodmKlk1+7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM/UD/gDSqhX9iOUAAAAASUVORK5CYII=";
        private const string DownloadImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAPNJREFUeJzt21EOADEMA9Bx/53Y/9xYAp5Iaj8yMzMzMzMzMzMzMzMzMzMzMzMzM79gZq1k1/7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM5Xs2h2YqWTX7sBMJbt2B2Yq2bU7MFPJrt2BmUp27Q7MVLJrd2Cmkl27AzOV7NodmKlk1+7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM/UD/gDSqhX9iOUAAAAASUVORK5CYII=";
        private const string UploadImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAPNJREFUeJzt21EOADEMA9Bx/53Y/9xYAp5Iaj8yMzMzMzMzMzMzMzMzMzMzMzMzM79gZq1k1/7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM5Xs2h2YqWTX7sBMJbt2B2Yq2bU7MFPJrt2BmUp27Q7MVLJrd2Cmkl27AzOV7NodmKlk1+7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM/UD/gDSqhX9iOUAAAAASUVORK5CYII=";
        private const string SettingsImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAPNJREFUeJzt21EOADEMA9Bx/53Y/9xYAp5Iaj8yMzMzMzMzMzMzMzMzMzMzMzMzM79gZq1k1/7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM5Xs2h2YqWTX7sBMJbt2B2Yq2bU7MFPJrt2BmUp27Q7MVLJrd2Cmkl27AzOV7NodmKlk1+7ATCW7dgdmKtm1OzBTya7dgZlKdu0OzFSya3dgppJduwMzlezaHZipZNfuwEwlu3YHZirZtTswU8mu3YGZSnbtDsxUsmt3YKaSXbsDM/UD/gDSqhX9iOUAAAAASUVORK5CYII=";

        public static string[] ImagesList = {
            PrintImageBase64,
            EditImageBase64,
            DeleteImageBase64,
            AddImageBase64,
            ViewImageBase64,
            DownloadImageBase64,
            UploadImageBase64,
            SettingsImageBase64
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

        private static Image GetButtonImage(ButtonImage buttonImage)
        {
            try
            {
                int index = (int)buttonImage;
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

        #region Column Creation Methods

        /// <summary>
        /// Creates a DataGridView text box column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateTextColumn(
            string columnName = "txtColumn",
            string headerText = null,
            bool readOnly = false,
            Type valueType = null,
            DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.Fill)
        {
            return new DataGridViewTextBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                AutoSizeMode = autoSizeMode,
                ValueType = valueType ?? typeof(string)
            };
        }

        /// <summary>
        /// Creates a DataGridView checkbox column
        /// </summary>
        public static DataGridViewCheckBoxColumn CreateCheckBoxColumn(
            string columnName = "checkColumn",
            string headerText = null,
            bool readOnly = false)
        {
            return new DataGridViewCheckBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };
        }

        /// <summary>
        /// Creates a DataGridView image button column
        /// </summary>
        public static DataGridViewImageColumn CreateButtonColumn(
            string columnName = "btn",
            Image backImage = null,
            int width = 30)
        {
            var column = new DataGridViewImageColumn()
            {
                Name = columnName,
                HeaderText = "",
                Image = backImage,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = width,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };

            // Add padding to the cell style for spacing
            column.DefaultCellStyle.Padding = new Padding(2, 2, 2, 2);
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            return column;
        }

        /// <summary>
        /// Creates a DataGridView combobox column
        /// </summary>
        public static DataGridViewComboBoxColumn CreateComboBoxColumn(
            string columnName = "comboColumn",
            string headerText = null,
            DataTable dataSource = null,
            string displayMember = "",
            string valueMember = "",
            bool readOnly = false)
        {
            return new DataGridViewComboBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                FlatStyle = FlatStyle.Flat,
                DataSource = dataSource,
                DisplayMember = displayMember,
                ValueMember = valueMember,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
        }

        /// <summary>
        /// Creates a DataGridView number column with formatting
        /// </summary>
        public static DataGridViewTextBoxColumn CreateNumberColumn(
            string columnName = "numberColumn",
            string headerText = null,
            bool readOnly = false,
            int? roundNumber = null,
            bool isPercentage = false)
        {
            var column = new DataGridViewTextBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                ValueType = isPercentage ? typeof(decimal) : typeof(double),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            // Set number formatting
            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            column.DefaultCellStyle.Format = isPercentage ? $"p{digits}" : $"n{digits}";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            return column;
        }

        /// <summary>
        /// Creates a DataGridView date column with formatting
        /// </summary>
        public static DataGridViewTextBoxColumn CreateDateColumn(
            string columnName = "dateColumn",
            string headerText = null,
            bool readOnly = false,
            string dateFormat = "yyyy-MM-dd")
        {
            return new DataGridViewTextBoxColumn()
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                ReadOnly = readOnly,
                ValueType = typeof(DateTime),
                DefaultCellStyle = { Format = dateFormat },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };
        }

        #endregion

        #region Extension Methods for Adding Columns

        /// <summary>
        /// Adds an edit button column to DataGridView
        /// </summary>
        public static void AddEditColumn(this DataGridView dgv, string columnName = "editBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Update), width));
        }

        /// <summary>
        /// Adds a delete button column to DataGridView
        /// </summary>
        public static void AddDeleteColumn(this DataGridView dgv, string columnName = "deleteBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Delete), width));
        }

        /// <summary>
        /// Adds a print button column to DataGridView
        /// </summary>
        public static void AddPrintColumn(this DataGridView dgv, string columnName = "printBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Print), width));
        }

        /// <summary>
        /// Adds an add button column to DataGridView
        /// </summary>
        public static void AddAddColumn(this DataGridView dgv, string columnName = "addBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.Add), width));
        }

        /// <summary>
        /// Adds a view button column to DataGridView
        /// </summary>
        public static void AddViewColumn(this DataGridView dgv, string columnName = "viewBtn", int width = 40)
        {
            if (dgv == null) return;
            dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(ButtonImage.View), width));
        }

        /// <summary>
        /// Adds multiple action buttons at once
        /// </summary>
        public static void AddActionColumns(this DataGridView dgv, params ButtonImage[] buttons)
        {
            if (dgv == null || buttons == null) return;

            foreach (var button in buttons)
            {
                string columnName = $"{button.ToString().ToLower()}Btn";
                dgv.Columns.Add(CreateButtonColumn(columnName, GetButtonImage(button), 30));
            }
        }

        #endregion

        #region Formatting Methods

        /// <summary>
        /// Formats a column as number
        /// </summary>
        public static void FormatAsNumber(this DataGridView dgv, string columnName, int? roundNumber = null)
        {
            if (dgv?.Columns[columnName] == null) return;

            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            dgv.Columns[columnName].DefaultCellStyle.Format = $"n{digits}";
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        /// <summary>
        /// Formats a column as percentage
        /// </summary>
        public static void FormatAsPercentage(this DataGridView dgv, string columnName, int? roundNumber = null)
        {
            if (dgv?.Columns[columnName] == null) return;

            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            dgv.Columns[columnName].DefaultCellStyle.Format = $"p{digits}";
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        /// <summary>
        /// Formats a column as currency
        /// </summary>
        public static void FormatAsCurrency(this DataGridView dgv, string columnName, int? roundNumber = null)
        {
            if (dgv?.Columns[columnName] == null) return;

            int digits = roundNumber ?? LibrarySettings.NumberDefaultRound;
            dgv.Columns[columnName].DefaultCellStyle.Format = $"c{digits}";
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        /// <summary>
        /// Formats a column as date
        /// </summary>
        public static void FormatAsDate(this DataGridView dgv, string columnName, string dateFormat = "yyyy-MM-dd")
        {
            if (dgv?.Columns[columnName] == null) return;

            dgv.Columns[columnName].DefaultCellStyle.Format = dateFormat;
            dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        #endregion

        #region Calculation Methods

        /// <summary>
        /// Performs calculations on DataGridView data
        /// </summary>
        public static double Compute(this DataGridView dgv, string expression, string filter = "")
        {
            if (dgv?.DataSource == null || dgv.Rows.Count <= 0)
                return 0;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable == null) return 0;

                var result = dataTable.Compute(expression, filter);
                if (double.TryParse(result?.ToString(), out double value))
                    return value;

                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Compute: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Calculates sum of a column
        /// </summary>
        public static double Sum(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"SUM({columnName})", filter);
        }

        /// <summary>
        /// Calculates average of a column
        /// </summary>
        public static double Average(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"AVG({columnName})", filter);
        }

        /// <summary>
        /// Calculates count of rows
        /// </summary>
        public static int Count(this DataGridView dgv, string filter = "")
        {
            return (int)dgv.Compute("COUNT(*)", filter);
        }

        /// <summary>
        /// Gets minimum value of a column
        /// </summary>
        public static double Min(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"MIN({columnName})", filter);
        }

        /// <summary>
        /// Gets maximum value of a column
        /// </summary>
        public static double Max(this DataGridView dgv, string columnName, string filter = "")
        {
            return dgv.Compute($"MAX({columnName})", filter);
        }

        #endregion

        #region Styling Methods

        /// <summary>
        /// Sets alternating row colors
        /// </summary>
        public static void SetAlternatingRowColors(this DataGridView dgv,
            Color evenRowColor, Color oddRowColor)
        {
            if (dgv == null) return;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = oddRowColor;
            dgv.RowsDefaultCellStyle.BackColor = evenRowColor;
        }

        /// <summary>
        /// Applies a modern style to DataGridView
        /// </summary>
        public static void ApplyModernStyle(this DataGridView dgv)
        {
            if (dgv == null) return;

            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;

            // Row style
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Alternating rows
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            // Grid appearance
            dgv.GridColor = Color.FromArgb(224, 224, 224);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowTemplate.Height = 30;

            // Enable double buffering for smoother scrolling
            dgv.DoubleBuffered(true);
        }

        /// <summary>
        /// Sets header text alignment for all columns
        /// </summary>
        public static void SetHeaderAlignment(this DataGridView dgv, DataGridViewContentAlignment alignment)
        {
            if (dgv == null) return;

            dgv.ColumnHeadersDefaultCellStyle.Alignment = alignment;
        }

        /// <summary>
        /// Sets font for all cells
        /// </summary>
        public static void SetFont(this DataGridView dgv, Font font)
        {
            if (dgv == null || font == null) return;

            dgv.Font = font;
            dgv.RowsDefaultCellStyle.Font = font;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(font, FontStyle.Bold);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Exports DataGridView to CSV
        /// </summary>
        public static bool ExportToCsv(this DataGridView dgv, string filePath)
        {
            if (dgv?.DataSource == null) return false;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable == null) return false;

                using (var writer = new StreamWriter(filePath))
                {
                    // Write headers
                    var headers = dataTable.Columns.Cast<DataColumn>()
                        .Select(column => $"\"{column.ColumnName}\"");
                    writer.WriteLine(string.Join(",", headers));

                    // Write data
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var fields = row.ItemArray.Select(field =>
                            $"\"{field?.ToString().Replace("\"", "\"\"")}\"");
                        writer.WriteLine(string.Join(",", fields));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to CSV: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets selected row data as dictionary
        /// </summary>
        public static Dictionary<string, object> GetSelectedRowData(this DataGridView dgv)
        {
            if (dgv?.SelectedRows.Count == 0) return null;

            try
            {
                var selectedRow = dgv.SelectedRows[0];
                var data = new Dictionary<string, object>();

                foreach (DataGridViewCell cell in selectedRow.Cells)
                {
                    data[cell.OwningColumn.Name] = cell.Value;
                }

                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting selected row data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Filters DataGridView based on column value
        /// </summary>
        public static void Filter(this DataGridView dgv, string columnName, object value, string operation = "=")
        {
            if (dgv?.DataSource == null) return;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable == null) return;

                string filter = value == null
                    ? $"{columnName} IS NULL"
                    : $"{columnName} {operation} '{value}'";

                dataTable.DefaultView.RowFilter = filter;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all filters from DataGridView
        /// </summary>
        public static void ClearFilter(this DataGridView dgv)
        {
            if (dgv?.DataSource == null) return;

            try
            {
                var dataTable = dgv.DataSource as DataTable;
                if (dataTable != null)
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing filter: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes DataGridView data
        /// </summary>
        public static void RefreshData(this DataGridView dgv)
        {
            if (dgv == null) return;

            try
            {
                dgv.Refresh();
                dgv.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Selects row by column value
        /// </summary>
        public static bool SelectRowByValue(this DataGridView dgv, string columnName, object value)
        {
            if (dgv?.Rows == null) return false;

            try
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[columnName]?.Value?.Equals(value) == true)
                    {
                        row.Selected = true;
                        dgv.FirstDisplayedScrollingRowIndex = row.Index;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting row: {ex.Message}");
            }

            return false;
        }

        #endregion

    }
}
