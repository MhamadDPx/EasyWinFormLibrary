namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Implemented by optional export modules (e.g. EasyWinFormLibrary.Excel) and assigned to
    /// <see cref="AdvancedDataGridView.ExportProvider"/> to enable <c>ExportDataAsync</c>.
    /// This lets the core library stay free of any specific export dependency (Office, CSV libs, etc.).
    /// </summary>
    public interface IGridExportProvider
    {
        /// <summary>
        /// Exports the grid's contents.
        /// </summary>
        /// <param name="grid">The grid to export.</param>
        /// <param name="filePath">Suggested output file path.</param>
        /// <param name="showResult">Whether to open/display the result after export.</param>
        void Export(AdvancedDataGridView grid, string filePath, bool showResult);
    }
}
