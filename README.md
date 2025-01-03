﻿# SharpTree

**SharpTree** is a tool designed to construct and visualize the filesystem tree structure.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Building SharpTree](#building-sharptree)
- [Using SharpTree in PowerShell](#using-sharptree-in-powershell)
  - [Importing the DLL](#importing-the-dll)
  - [Example Usage](#example-usage)

## Features

- **PowerShell Integration**: Easily importable into PowerShell 5.1 or 7 scripts for automation and scripting tasks.
- **Multi-Targeted**: Compatible with both modern .NET environments (.NET 8) and legacy systems via .NET Framework 4.8 for PowerShell 5.1.

## Prerequisites

- **.NET 8 SDK**: Required for building the project. [Download Here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PowerShell 5.1 or 7**: Available by default on Windows systems.
- **Operating System**: Windows.

## Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/isaiahduarte/SharpTree.git
   cd SharpTree
   ```

2. **Restore Dependencies**

   Ensure all necessary NuGet packages are restored.

   ```bash
   dotnet restore
   ```

3. **Build the Project**

   Follow the [Building SharpTree](#building-sharptree) section to generate the necessary DLLs.

## Building SharpTree

1. **Ensure .NET 8 SDK is Installed**

   Download and install the [latest .NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) if you haven't already.

2. **Navigate to the Project Directory**

   ```bash
   cd SharpTree
   ```

3. **Build the Project**

   Execute the following command to build for both target frameworks:

   ```bash
   dotnet publish
   ```

4. **Locate the Generated DLLs**

   After a successful build, the DLLs will be located in:

   ```
   \bin\SharpTree\publish
   \bin\SharpTree.Core\publish
   ```

## Using SharpTree in PowerShell

Integrate SharpTree into your PowerShell scripts by importing the compatible DLL and utilizing its methods to build and display filesystem trees.

### Importing the DLL

Depending if you are using PowerShell 5.1 or 7, you will need to import the corresponding DLL:

**Powershell 5.1**
```powershell
$dllPath = "C:\Path\To\SharpTree\bin\SharpTree.Core.Powershell\SharpTree.Core.Powershell.dll"
```

**Powershell 7**
```powershell
$dllPath = "C:\Path\To\SharpTree\bin\SharpTree.Core\publish\SharpTree.Core.Powershell.dll"
```

Note: Powershell 7 is more efficient and recommended for use.

**Import the SharpTree DLL**

   Use the `Add-Type` cmdlet to load the assembly:

   ```powershell
   # Import the DLL
   Add-Type -Path $dllPath
   ```

### Example Usage

   ```powershell
   # See which version of PowerShell is running
   if ($PSVersionTable.PSVersion.Major -eq 5) {
      $dllPath = "C:\Path\To\SharpTree\bin\SharpTree.Core.Powershell\SharpTree.Core.Powershell.dll"
      exit
   } elseif ($PSVersionTable.PSVersion.Major -eq 7) {
      $dllPath = "C:\Path\To\SharpTree\bin\SharpTree.Core\SharpTree.Core.dll"
   } else {
      Write-Host "Unsupported PowerShell version"
      exit
   }

   # Load the assembly
   Add-Type -Path $dllPath -PassThru

   # Get the root node                                      #Path #MinSize #MaxDepth
   $Node = [SharpTree.Core.Services.FileSystemReader]::Read($ENV:USERPROFILE, 1024, -1)

   # Save node to JSON
   $Node.SaveToJson(C:\report.json)

   # Display Node
   $Node.Show()

  # Load From JSON
  $Node = [SharpTree.Core.Services.JsonNode]::LoadFromJson(C:\report.json)
   ```

[Test.webm](https://github.com/user-attachments/assets/9383a831-180e-4c6b-b911-e67952039833)

## Contributing

Contributions are welcome! To contribute to SharpTree, follow these steps:

1. **Fork the Repository**

   Click the "Fork" button on the repository page to create a personal copy.

2. **Clone Your Fork**

   ```bash
   git clone https://github.com/IsaiahDuarte/SharpTree.git
   cd SharpTree
   ```

3. **Create a New Branch**

   ```bash
   git checkout -b feature/YourFeatureName
   ```

4. **Make Your Changes**

   Implement your feature or bug fix.

5. **Commit Your Changes**

   ```bash
   git commit -m "Add description of your feature or fix"
   ```

6. **Push to Your Fork**

   ```bash
   git push origin feature/YourFeatureName
   ```

7. **Open a Pull Request**

   Go to the original repository and create a pull request detailing your changes.
