# EasyWinFormLibrary

A comprehensive, ready-to-use .NET Framework 4.8 class library designed to **simplify Windows Forms application development** with **custom UI controls**, **database helpers**, and **productivity-focused extensions**.

---

## ? Features

### ?? Custom Controls (`CustomControls`)
Modern, reusable Windows Forms controls with enhanced behavior and appearance:
- `AdvancedButton`, `AdvancedActionButton` – stylized buttons
- `AdvancedComboBox`, `AdvancedTextBox` – improved input controls
- `AdvancedDataGridView` – extended DataGridView with features
- `AdvancedDashboardCard` – card-style dashboard components
- `AdvancedAlertForm`, `AdvancedMessageBoxForm` – custom modal dialogs
- `AdvancedFormTopMenuBar` – top-aligned form menu bar
- `AdvancedPanel` – UI containers with visual tweaks
- `AdvancedAddDialogForm` – consistent data input dialogs
- `AdvancedUserMetaLog` – structured user log display

### ??? Data Access Helpers (`Data`)
Simplify SQL Server integration:
- `SqlDatabaseConnectionConfigBuilder` – fluent config builder
- `SqlDatabaseActions` – CRUD operations
- `SqlQueryBuilder` – dynamic query builder
- `AuthUserInfo` – structure to store authenticated user metadata

### ?? Extensions (`Extension`)
Productivity extensions for WinForms controls and common types:
- `ControlCleanerExtensions` – reset/clear form controls
- `ControlRequiredCheckerExtensions` – validate required fields
- `ControlPerformanceExtensions`, `ControlEventExtensions` – better control handling
- `FormExtensions`, `DataGridViewExtensions`, `DataTableExtensions`
- `StringExtensions`, `DataRowCollectionExtensions`

### ?? Utility Tools (`WinAppNeeds`)
Handy modules and utilities:
- `AdvancedAlert`, `AdvancedMessageBox` – easy-to-use global alert APIs
- `BarcodeGenerator`, `ImageUtils`, `NumberToWordsConverter`
- `LanguageManager` – basic multilingual support
- `HardwareIdentifier` – machine-specific ID generation
- `TaskDelayUtils`, `NumberInputUtils` – form automation helpers

---

## ?? Installation

Install via NuGet:

```bash
Install-Package EasyWinFormLibrary
