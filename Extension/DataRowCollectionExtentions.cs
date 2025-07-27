using System.Data;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Extension methods for DataRowCollection
    /// </summary>
    public static class DataRowCollectionExtentions
    {
        /// <summary>
        /// Checks if DataRowCollection has any rows
        /// </summary>
        /// <param name="rows">The DataRowCollection to check</param>
        /// <returns>True if collection has rows</returns>
        public static bool HasRow(this DataRowCollection rows)
        {
            return rows?.Count > 0;
        }

        /// <summary>
        /// Checks if DataRowCollection is empty
        /// </summary>
        /// <param name="rows">The DataRowCollection to check</param>
        /// <returns>True if collection is empty</returns>
        public static bool IsEmpty(this DataRowCollection rows)
        {
            return rows?.Count == 0;
        }
    }
}
