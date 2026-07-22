# EasyWinFormLibrary

<p align="center">
  <img src="screenshots/logo.png" alt="EasyWinFormLibrary Logo" width="180"/>
</p>

<h3 align="center">
Modern reusable controls and components for C# Windows Forms applications
</h3>

<p align="center">
  <a href="https://github.com/MhamadDPx/EasyWinFormLibrary/stargazers">
    <img src="https://img.shields.io/github/stars/MhamadDPx/EasyWinFormLibrary?style=for-the-badge" />
  </a>
  <a href="https://github.com/MhamadDPx/EasyWinFormLibrary/network/members">
    <img src="https://img.shields.io/github/forks/MhamadDPx/EasyWinFormLibrary?style=for-the-badge" />
  </a>
  <a href="https://github.com/MhamadDPx/EasyWinFormLibrary/blob/master/LICENSE">
    <img src="https://img.shields.io/github/license/MhamadDPx/EasyWinFormLibrary?style=for-the-badge" />
  </a>
  <a href="https://www.nuget.org/">
    <img src="https://img.shields.io/nuget/v/EasyWinFormLibrary?style=for-the-badge" />
  </a>
  <a href="https://www.nuget.org/">
    <img src="https://img.shields.io/nuget/dt/EasyWinFormLibrary?style=for-the-badge" />
  </a>
</p>

---

## About

**EasyWinFormLibrary** is an open-source C# library that provides modern reusable controls, UI components, validation tools, and helper utilities for Windows Forms developers.

The goal of this project is to make WinForms development faster and easier by reducing repetitive code and providing ready-to-use components.

Built for developers who want cleaner, more professional, and more productive Windows desktop applications.

---

# Features

## Advanced Controls

✅ AdvancedTextBox

* Input validation
* Numeric support
* Negative number control
* Placeholder support
* Custom formatting
* Developer-friendly properties

✅ AdvancedButton

* Custom styling
* Improved appearance
* Easy configuration

✅ AdvancedDataGridView

* Better table management
* Improved formatting
* Productivity features

---

## Additional Features

✔ Reusable WinForms components
✔ Validation helpers
✔ UI utilities
✔ Developer productivity tools
✔ Easy integration
✔ Lightweight design
✔ .NET Framework compatibility

---

# Screenshots

## AdvancedTextBox

<p align="center">
  <img src="screenshots/advanced-textbox.png" width="700"/>
</p>

---

## AdvancedDataGridView

<p align="center">
  <img src="screenshots/advanced-datagridview.png" width="700"/>
</p>

---

## AdvancedButton

<p align="center">
  <img src="screenshots/advanced-button.png" width="700"/>
</p>

---

# Installation

## NuGet Package

Install using NuGet Package Manager:

```powershell
Install-Package EasyWinFormLibrary
```

or using .NET CLI:

```powershell
dotnet add package EasyWinFormLibrary
```

---

# Quick Example

### AdvancedTextBox Example

```csharp
using EasyWinFormLibrary.Controls;

AdvancedTextBox textBox = new AdvancedTextBox();

textBox.Required = true;
textBox.IsNumeric = true;
textBox.AllowNegative = false;
textBox.PlaceholderText = "Enter value";

this.Controls.Add(textBox);
```

---

# Requirements

* Windows
* Visual Studio
* .NET Framework 4.8

---

# Why EasyWinFormLibrary?

Developing WinForms applications often requires creating the same controls and validation logic repeatedly.

EasyWinFormLibrary provides:

* Faster development
* Cleaner code
* Reusable components
* Consistent user interfaces
* Less repetitive programming

---

# Documentation

Documentation and examples are being improved continuously.

Available resources:

* Getting Started Guide
* Control Documentation
* Examples
* API Reference

---

# Roadmap

## Version 2.x

Future improvements:

* [ ] Modern theme system
* [ ] Dark mode support
* [ ] Fluent UI inspired controls
* [ ] More validation components
* [ ] More examples
* [ ] Better designer support
* [ ] .NET 8 compatibility

---

# Contributing

Contributions are welcome!

If you would like to improve EasyWinFormLibrary:

1. Fork the repository
2. Create a feature branch

```bash
git checkout -b feature/MyFeature
```

3. Commit your changes

```bash
git commit -m "Add new feature"
```

4. Push your branch

```bash
git push origin feature/MyFeature
```

5. Open a Pull Request

---

# Bug Reports and Feature Requests

Found a bug or have an idea?

Please create an issue:

https://github.com/MhamadDPx/EasyWinFormLibrary/issues

---

# License

EasyWinFormLibrary is open-source software licensed under the MIT License.

---

# Author

Created and maintained by:

**Mhamad Tahir**

GitHub:
https://github.com/MhamadDPx

---

<p align="center">
⭐ If you find this project useful, consider giving it a star!
</p>
