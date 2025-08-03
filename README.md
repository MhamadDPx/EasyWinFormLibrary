# EasyWinFormLibrary

Easy Windows Form library to simplify development: includes custom controls, SQL Server helpers, and useful extensions.

## Features

- **Custom Controls**: Modern, customizable Windows Forms controls
- **SQL Server Helpers**: Simplified database operations and connection management
- **Useful Extensions**: Extension methods for common WinForms tasks
- **Built-in Logging**: Integrated Sentry and Serilog support

## Installation

Install via NuGet Package Manager:

```
Install-Package EasyWinFormLibrary
```

Or via .NET CLI:

```
dotnet add package EasyWinFormLibrary
```

## Quick Start

```csharp
using EasyWinFormLibrary;
using EasyWinFormLibrary.CustomControls;

// Use custom controls
var advancedButton = new AdvancedButton();
var advancedTextBox = new AdvancedTextBox();

// Use SQL helpers
var connectionConfig = new SqlDatabaseConnectionConfig
{
    Server = "your-server",
    Database = "your-database"
};

var dbActions = new SqlDatabaseActions(connectionConfig);
```

## Requirements

- .NET Framework 4.8
- Windows Forms Application

## Documentation

For detailed documentation and examples, visit our [GitHub repository](https://github.com/MhamadDPx/EasyWinFormLibrary).

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and feature requests, please visit our [GitHub Issues](https://github.com/MhamadDPx/EasyWinFormLibrary/issues) page.