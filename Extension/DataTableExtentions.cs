using System.Data;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Extension methods for DataTable
    /// </summary>
    public static class DataTableExtentions
    {
        /// <summary>
        /// Checks if DataTable has any rows
        /// </summary>
        /// <param name="dataTable">The DataTable to check</param>
        /// <returns>True if DataTable has rows</returns>
        public static bool HasRow(this DataTable dataTable)
        {
            return dataTable?.Rows?.Count > 0;
        }

        /// <summary>
        /// Checks if DataTable is empty (no rows)
        /// </summary>
        /// <param name="dataTable">The DataTable to check</param>
        /// <returns>True if DataTable is empty</returns>
        public static bool IsEmpty(this DataTable dataTable)
        {
            return dataTable?.Rows?.Count == 0;
        }

        /// <summary>
        /// Gets the number of rows safely
        /// </summary>
        /// <param name="dataTable">The DataTable to check</param>
        /// <returns>Number of rows, or 0 if null</returns>
        public static int RowCount(this DataTable dataTable)
        {
            return dataTable?.Rows?.Count ?? 0;
        }

        /// <summary>
        /// Checks if DataTable has more than specified number of rows
        /// </summary>
        /// <param name="dataTable">The DataTable to check</param>
        /// <param name="count">Minimum row count</param>
        /// <returns>True if DataTable has more rows than specified</returns>
        public static bool HasMoreThan(this DataTable dataTable, int count)
        {
            return dataTable?.Rows?.Count > count;
        }
    }
}
