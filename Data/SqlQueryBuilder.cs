using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Data
{
    /// <summary>
    /// Provides comprehensive SQL query building utilities with support for dynamic filtering,
    /// search operations, and parameterized queries. Includes SQL injection protection and
    /// support for various WinForms controls. Optimized for .NET Framework 4.8.
    /// </summary>
    public static class SqlQueryBuilder
    {
        #region Enum

        /// <summary>
        /// Enumeration for different search modes in multi-term searches.
        /// </summary>
        public enum SearchMode
        {
            /// <summary>All search terms must be found</summary>
            AllTerms,
            /// <summary>Any of the search terms can be found</summary>
            AnyTerm,
            /// <summary>Search for the exact phrase</summary>
            ExactPhrase
        }

        /// <summary>
        /// Enumeration for different comparison operators.
        /// </summary>
        public enum ComparisonOperator
        {
            /// <summary>Equal to (=)</summary>
            Equal,
            /// <summary>Not equal to (&lt;&gt;)</summary>
            NotEqual,
            /// <summary>Greater than (&gt;)</summary>
            GreaterThan,
            /// <summary>Greater than or equal to (&gt;=)</summary>
            GreaterThanOrEqual,
            /// <summary>Less than (&lt;)</summary>
            LessThan,
            /// <summary>Less than or equal to (&lt;=)</summary>
            LessThanOrEqual,
            /// <summary>Like (LIKE)</summary>
            Like,
            /// <summary>Not like (NOT LIKE)</summary>
            NotLike,
            /// <summary>In (IN)</summary>
            In,
            /// <summary>Not in (NOT IN)</summary>
            NotIn
        }
        #endregion

        #region Filter and Query Building

        /// <summary>
        /// Creates a dynamic SQL SELECT query with filtering based on control values and their tags.
        /// Enhanced version with SQL injection protection and improved parameter handling.
        /// </summary>
        /// <param name="tableName">Name of the database table</param>
        /// <param name="selectColumns">Columns to select (use "*" for all columns)</param>
        /// <param name="controlColumnMappings">Dictionary mapping controls to their corresponding database columns</param>
        /// <param name="includeDateFilter">Whether to include date-based filtering</param>
        /// <param name="dateCondition">Additional date condition to append</param>
        /// <param name="additionalCondition">Additional WHERE condition to append</param>
        /// <param name="reverseSearch">Whether to use NOT LIKE/NOT EQUAL operators</param>
        /// <param name="useParameterizedQueries">Whether to use parameterized queries for SQL injection protection</param>
        /// <returns>Generated SQL query string</returns>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
        /// <exception cref="ArgumentException">Thrown when table name or select columns are empty</exception>
        public static string CreateSelectQuery(string tableName, string selectColumns,
            Dictionary<Control, string> controlColumnMappings, bool includeDateFilter = false,
            string dateCondition = "", string additionalCondition = "", bool reverseSearch = false,
            bool useParameterizedQueries = true)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
            if (string.IsNullOrWhiteSpace(selectColumns))
                throw new ArgumentException("Select columns cannot be null or empty", nameof(selectColumns));
            if (controlColumnMappings == null)
                throw new ArgumentNullException(nameof(controlColumnMappings), "Control column mappings cannot be null");

            try
            {
                var conditions = new List<string>();
                var queryBuilder = new StringBuilder();

                // Process each control and build conditions
                foreach (var mapping in controlColumnMappings)
                {
                    var control = mapping.Key;
                    var columnName = mapping.Value;

                    if (string.IsNullOrWhiteSpace(columnName))
                        continue;

                    var condition = ProcessControlForCondition(control, columnName, reverseSearch, useParameterizedQueries);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        conditions.Add(condition);
                    }
                }

                // Build the base query
                queryBuilder.Append($"SELECT {SanitizeIdentifier(selectColumns)} FROM {SanitizeIdentifier(tableName)}");

                // Build WHERE clause
                var whereConditions = new List<string>();

                if (conditions.Any())
                {
                    whereConditions.AddRange(conditions);
                }

                if (includeDateFilter && !string.IsNullOrWhiteSpace(dateCondition))
                {
                    whereConditions.Add(dateCondition);
                }

                if (!string.IsNullOrWhiteSpace(additionalCondition))
                {
                    whereConditions.Add(additionalCondition);
                }

                if (whereConditions.Any())
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(string.Join(" AND ", whereConditions));
                }

                return queryBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create filter query: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a comprehensive search query that searches across multiple columns with multiple search terms.
        /// Enhanced version with better type handling and SQL injection protection.
        /// </summary>
        /// <param name="columnTypes">Dictionary mapping column names to their data types</param>
        /// <param name="searchTerms">Array of search terms to look for</param>
        /// <param name="searchMode">Search mode (All terms, Any term, Exact phrase)</param>
        /// <param name="caseSensitive">Whether the search should be case sensitive</param>
        /// <returns>Generated search condition string</returns>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
        public static string CreateSearchCondition(Dictionary<string, string> columnTypes, string[] searchTerms,
            SearchMode searchMode = SearchMode.AllTerms, bool caseSensitive = false)
        {
            if (columnTypes == null)
                throw new ArgumentNullException(nameof(columnTypes), "Column types cannot be null");
            if (searchTerms == null || searchTerms.Length == 0)
                throw new ArgumentNullException(nameof(searchTerms), "Search terms cannot be null or empty");

            try
            {
                var searchConditions = new List<string>();

                foreach (var term in searchTerms.Where(t => !string.IsNullOrWhiteSpace(t)))
                {
                    var termConditions = new List<string>();
                    var sanitizedTerm = SanitizeSearchTerm(term);

                    foreach (var column in columnTypes)
                    {
                        var columnName = SanitizeIdentifier(column.Key);
                        var columnType = column.Value.ToLower();

                        string columnCondition = BuildColumnSearchCondition(columnName, columnType, sanitizedTerm, caseSensitive);
                        if (!string.IsNullOrEmpty(columnCondition))
                        {
                            termConditions.Add(columnCondition);
                        }
                    }

                    if (termConditions.Any())
                    {
                        searchConditions.Add($"({string.Join(" OR ", termConditions)})");
                    }
                }

                if (!searchConditions.Any())
                    return string.Empty;

                string joinOperator = searchMode == SearchMode.AnyTerm ? " OR " : " AND ";
                return string.Join(joinOperator, searchConditions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create search condition: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Query Building

        /// <summary>
        /// Creates a parameterized SELECT query with advanced filtering options.
        /// </summary>
        /// <param name="tableName">Name of the table to query</param>
        /// <param name="selectColumns">Columns to select</param>
        /// <param name="whereConditions">Dictionary of column-value pairs for WHERE conditions</param>
        /// <param name="orderBy">ORDER BY clause (optional)</param>
        /// <param name="groupBy">GROUP BY clause (optional)</param>
        /// <param name="having">HAVING clause (optional)</param>
        /// <param name="limit">Number of records to limit (0 for no limit)</param>
        /// <returns>Parameterized SQL query</returns>
        public static string CreateAdvancedSelectQuery(string tableName, string selectColumns,
            Dictionary<string, object> whereConditions = null, string orderBy = null,
            string groupBy = null, string having = null, int limit = 0)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));

            var queryBuilder = new StringBuilder();

            // SELECT clause
            if (limit > 0)
                queryBuilder.Append($"SELECT TOP {limit} ");
            else
                queryBuilder.Append("SELECT ");

            queryBuilder.Append(string.IsNullOrWhiteSpace(selectColumns) ? "*" : SanitizeIdentifier(selectColumns));
            queryBuilder.Append($" FROM {SanitizeIdentifier(tableName)}");

            // WHERE clause
            if (whereConditions != null && whereConditions.Any())
            {
                var conditions = whereConditions.Select(kvp =>
                    $"{SanitizeIdentifier(kvp.Key)} = @{SanitizeParameterName(kvp.Key)}");
                queryBuilder.Append($" WHERE {string.Join(" AND ", conditions)}");
            }

            // GROUP BY clause
            if (!string.IsNullOrWhiteSpace(groupBy))
                queryBuilder.Append($" GROUP BY {groupBy}");

            // HAVING clause
            if (!string.IsNullOrWhiteSpace(having))
                queryBuilder.Append($" HAVING {having}");

            // ORDER BY clause
            if (!string.IsNullOrWhiteSpace(orderBy))
                queryBuilder.Append($" ORDER BY {orderBy}");

            return queryBuilder.ToString();
        }

        /// <summary>
        /// Creates an INSERT query with parameterized values.
        /// </summary>
        /// <param name="tableName">Name of the table to insert into</param>
        /// <param name="columnValues">Dictionary of column-value pairs to insert</param>
        /// <returns>Parameterized INSERT query</returns>
        public static string CreateInsertQuery(string tableName, Dictionary<string, object> columnValues)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
            if (columnValues == null || !columnValues.Any())
                throw new ArgumentException("Column values cannot be null or empty", nameof(columnValues));

            var columns = string.Join(", ", columnValues.Keys.Select(SanitizeIdentifier));
            var parameters = string.Join(", ", columnValues.Keys.Select(k => $"@{SanitizeParameterName(k)}"));

            return $"INSERT INTO {SanitizeIdentifier(tableName)} ({columns}) VALUES ({parameters})";
        }

        /// <summary>
        /// Creates an UPDATE query with parameterized values.
        /// </summary>
        /// <param name="tableName">Name of the table to update</param>
        /// <param name="columnValues">Dictionary of column-value pairs to update</param>
        /// <param name="whereConditions">Dictionary of column-value pairs for WHERE conditions</param>
        /// <returns>Parameterized UPDATE query</returns>
        public static string CreateUpdateQuery(string tableName, Dictionary<string, object> columnValues,
            Dictionary<string, object> whereConditions)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
            if (columnValues == null || !columnValues.Any())
                throw new ArgumentException("Column values cannot be null or empty", nameof(columnValues));
            if (whereConditions == null || !whereConditions.Any())
                throw new ArgumentException("WHERE conditions cannot be null or empty", nameof(whereConditions));

            var setClause = string.Join(", ", columnValues.Keys.Select(k =>
                $"{SanitizeIdentifier(k)} = @{SanitizeParameterName(k)}"));

            var whereClause = string.Join(" AND ", whereConditions.Keys.Select(k =>
                $"{SanitizeIdentifier(k)} = @where_{SanitizeParameterName(k)}"));

            return $"UPDATE {SanitizeIdentifier(tableName)} SET {setClause} WHERE {whereClause}";
        }

        /// <summary>
        /// Creates a DELETE query with parameterized WHERE conditions.
        /// </summary>
        /// <param name="tableName">Name of the table to delete from</param>
        /// <param name="whereConditions">Dictionary of column-value pairs for WHERE conditions</param>
        /// <returns>Parameterized DELETE query</returns>
        public static string CreateDeleteQuery(string tableName, Dictionary<string, object> whereConditions)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));
            if (whereConditions == null || !whereConditions.Any())
                throw new ArgumentException("WHERE conditions cannot be null or empty", nameof(whereConditions));

            var whereClause = string.Join(" AND ", whereConditions.Keys.Select(k =>
                $"{SanitizeIdentifier(k)} = @{SanitizeParameterName(k)}"));

            return $"DELETE FROM {SanitizeIdentifier(tableName)} WHERE {whereClause}";
        }

        #endregion

        #region Control Processing Methods

        /// <summary>
        /// Processes a control and generates the appropriate SQL condition based on its type and tag.
        /// </summary>
        /// <param name="control">The control to process</param>
        /// <param name="columnName">The database column name</param>
        /// <param name="reverseSearch">Whether to reverse the search logic</param>
        /// <param name="useParameterized">Whether to use parameterized queries</param>
        /// <returns>SQL condition string or empty if no condition should be generated</returns>
        private static string ProcessControlForCondition(Control control, string columnName,
            bool reverseSearch, bool useParameterized)
        {
            if (control == null || string.IsNullOrWhiteSpace(columnName))
                return string.Empty;

            var sanitizedColumn = SanitizeIdentifier(columnName);
            var tag = control.Tag?.ToString() ?? string.Empty;

            // Process TextBox controls
            if (control is TextBox textBox)
            {
                return ProcessTextBoxCondition(textBox, sanitizedColumn, tag, reverseSearch, useParameterized);
            }

            // Process ComboBox controls
            if (control is ComboBox comboBox)
            {
                return ProcessComboBoxCondition(comboBox, sanitizedColumn, tag, reverseSearch, useParameterized);
            }

            return string.Empty;
        }

        /// <summary>
        /// Processes TextBox control for SQL condition generation.
        /// </summary>
        private static string ProcessTextBoxCondition(TextBox textBox, string columnName, string tag,
            bool reverseSearch, bool useParameterized)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                return string.Empty;

            var value = textBox.Text.Trim();
            return GenerateConditionByTag(columnName, tag, value, reverseSearch, useParameterized);
        }

        /// <summary>
        /// Processes ComboBox control for SQL condition generation.
        /// </summary>
        private static string ProcessComboBoxCondition(ComboBox comboBox, string columnName, string tag,
            bool reverseSearch, bool useParameterized)
        {
            if (string.IsNullOrWhiteSpace(comboBox.Text))
                return string.Empty;

            var value = tag.Contains("text") ? comboBox.Text : comboBox.SelectedValue?.ToString();
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return GenerateConditionByTag(columnName, tag, value, reverseSearch, useParameterized);
        }


        /// <summary>
        /// Generates SQL condition based on control tag.
        /// </summary>
        private static string GenerateConditionByTag(string columnName, string tag, string value,
            bool reverseSearch, bool useParameterized)
        {
            var lowerTag = tag.ToLower();

            if (lowerTag.Contains("searchstr"))
            {
                var operator_op = reverseSearch ? "NOT LIKE" : "LIKE";
                var likeValue = useParameterized
                    ? $"@param_{SanitizeParameterName(columnName)}"
                    : $"N'%{SanitizeValue(value)}%'";
                return $"{columnName} {operator_op} {likeValue}";
            }

            if (lowerTag.Contains("searchint") || lowerTag.Contains("equal"))
            {
                var operator_op = reverseSearch ? "<>" : "=";
                var paramValue = useParameterized
                    ? $"@param_{SanitizeParameterName(columnName)}"
                    : SanitizeValue(value);
                return $"{columnName} {operator_op} {paramValue}";
            }

            if (lowerTag.Contains("greater"))
            {
                var paramValue = useParameterized
                    ? $"@param_{SanitizeParameterName(columnName)}"
                    : SanitizeValue(value);
                return $"{columnName} > {paramValue}";
            }

            if (lowerTag.Contains("smaller"))
            {
                var paramValue = useParameterized
                    ? $"@param_{SanitizeParameterName(columnName)}"
                    : SanitizeValue(value);
                return $"{columnName} < {paramValue}";
            }

            return string.Empty;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds a search condition for a specific column based on its data type.
        /// </summary>
        private static string BuildColumnSearchCondition(string columnName, string columnType, string searchTerm, bool caseSensitive)
        {
            switch (columnType)
            {
                case "string":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext":
                    var stringCondition = caseSensitive
                        ? $"ISNULL({columnName}, '')"
                        : $"UPPER(ISNULL({columnName}, ''))";
                    var stringTerm = caseSensitive ? searchTerm : searchTerm.ToUpper();
                    return $"{stringCondition} LIKE N'%{SanitizeValue(stringTerm)}%'";

                case "int":
                case "integer":
                case "bigint":
                case "smallint":
                case "tinyint":
                    return $"CONVERT(NVARCHAR, ISNULL({columnName}, 0)) LIKE N'%{SanitizeValue(searchTerm)}%'";

                case "decimal":
                case "numeric":
                case "float":
                case "real":
                case "money":
                case "smallmoney":
                    return $"CONVERT(NVARCHAR, ISNULL({columnName}, 0)) LIKE N'%{SanitizeValue(searchTerm)}%'";

                case "datetime":
                case "datetime2":
                case "date":
                case "time":
                case "smalldatetime":
                    return $"CONVERT(NVARCHAR, {columnName}, 121) LIKE N'%{SanitizeValue(searchTerm)}%'";

                case "bit":
                    return $"CASE WHEN {columnName} = 1 THEN 'True' ELSE 'False' END LIKE N'%{SanitizeValue(searchTerm)}%'";

                default:
                    return $"CONVERT(NVARCHAR, ISNULL({columnName}, '')) LIKE N'%{SanitizeValue(searchTerm)}%'";
            }
        }

        /// <summary>
        /// Sanitizes database identifiers (table names, column names) to prevent SQL injection.
        /// </summary>
        private static string SanitizeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return identifier;

            // Remove or replace dangerous characters
            var sanitized = Regex.Replace(identifier, @"[^\w\s,.*]", "");

            // Handle comma-separated identifiers
            if (sanitized.Contains(","))
            {
                var parts = sanitized.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p));
                return string.Join(", ", parts);
            }

            return sanitized.Trim();
        }

        /// <summary>
        /// Sanitizes parameter names for SQL parameters.
        /// </summary>
        private static string SanitizeParameterName(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                return "param";

            return Regex.Replace(parameterName, @"[^\w]", "_").Trim('_');
        }

        /// <summary>
        /// Sanitizes string values to prevent SQL injection in non-parameterized queries.
        /// </summary>
        private static string SanitizeValue(string value)
        {
            if (value == null)
                return "NULL";

            // Escape single quotes by doubling them
            return value.Replace("'", "''");
        }

        /// <summary>
        /// Sanitizes search terms for LIKE operations.
        /// </summary>
        private static string SanitizeSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return string.Empty;

            // Escape SQL wildcards and special characters
            return searchTerm
                .Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]")
                .Trim();
        }

        /// <summary>
        /// Validates and formats table and column names.
        /// </summary>
        /// <param name="names">Array of names to validate</param>
        /// <returns>True if all names are valid</returns>
        public static bool ValidateIdentifiers(params string[] names)
        {
            if (names == null || names.Length == 0)
                return false;

            var identifierPattern = @"^[a-zA-Z_][a-zA-Z0-9_]*$";
            return names.All(name => !string.IsNullOrWhiteSpace(name) &&
                                   Regex.IsMatch(name, identifierPattern));
        }

        /// <summary>
        /// Escapes a string value for safe inclusion in SQL queries.
        /// </summary>
        /// <param name="value">Value to escape</param>
        /// <param name="addQuotes">Whether to add single quotes around the value</param>
        /// <returns>Escaped string value</returns>
        public static string EscapeStringValue(string value, bool addQuotes = true)
        {
            if (value == null)
                return "NULL";

            var escaped = SanitizeValue(value);
            return addQuotes ? $"N'{escaped}'" : escaped;
        }

        #endregion
    }

}