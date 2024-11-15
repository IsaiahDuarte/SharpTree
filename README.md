# SharpTree

**SharpTree** is a versatile .NET library designed to construct and visualize the filesystem tree structure. It seamlessly integrates with PowerShell 5.1 and 7, allowing users to import the generated DLL and interact with filesystem data directly within PowerShell scripts. SharpTree supports various filesystem traversal behaviors, including single-volume traversal and symbolic link handling.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Building SharpTree](#building-sharptree)
- [Using SharpTree in PowerShell 5.1](#using-sharptree-in-powershell-51)
  - [Importing the DLL](#importing-the-dll)
  - [Example Usage](#example-usage)
- [Project Structure](#project-structure)
- [License](#license)

## Features

- **Recursive Filesystem Traversal**: Navigate through directories and files starting from a specified root path.
- **Traversal Behaviors**: Supports multiple behaviors like single-volume traversal and the ability to follow or ignore symbolic links.
- **PowerShell Integration**: Easily importable into PowerShell 5.1 or 7 scripts for automation and scripting tasks.
- **Multi-Targeted**: Compatible with both modern .NET environments (.NET 8) and legacy systems via .NET 4.8 for PowerShell 5.1.
- **Exception Handling**: Robust error handling to manage I/O operations gracefully.

## Prerequisites

- **.NET 8 SDK**: Required for building the project. [Download Here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PowerShell 5.1 or 7**: Available by default on Windows systems.
- **Operating System**: Windows.

## Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/yourusername/SharpTree.git
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

SharpTree is configured to multi-target both `.NET 8` and `.NET 4.8` to ensure compatibility with PowerShell 5.1.

### Steps to Build

1. **Ensure .NET 8 SDK is Installed**

   Download and install the [latest .NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) if you haven't already.

2. **Navigate to the Project Directory**

   ```bash
   cd SharpTree
   ```

3. **Build the Project**

   Execute the following command to build for both target frameworks:

   ```bash
   dotnet build -c Release
   ```

4. **Locate the Generated DLLs**

   After a successful build, the DLLs will be located in:

   ```
    SharpTree/
    ├─ bin/
    │  ├─ SharpTree/
    │  │  ├─ SharpTree.exe
    │  ├─ SharpTree.Core/
    │  │  ├─ SharpTree.Core.dll
    │  ├─ SharpTree.Core.Powershell/
    │  │  ├─ SharpTree.Core.Powershell.dll

   ```

   - **For PowerShell 5.1**: Use the `SharpTree.Core.Powershell/SharpTree.Core.Powershell.dll` as it ensures compatibility.

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
$dllPath = "C:\Path\To\SharpTree\bin\SharpTree.Core.\SharpTree.Core.Powershell.dll"
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

   # Create a new FilesystemBehaviorType
   $FSBehavior = [SharpTree.Core.Behaviors.FilesystemBehaviorType]::SingleVolume

   # Create a new FileSystemBehavior
   $FSBehaviors = [SharpTree.Core.Behaviors.FilesystemBehaviorsFactory]::Create($FSBehavior, "C:\")

   # Get the root node
   $Node = [SharpTree.Core.Services.FileSystemReader]::ReadRecursive($ENV:USERPROFILE, $false, $FSBehaviors)

   # Save node to JSON
   $Node | ConvertTo-Json -Depth 100 | Out-File -FilePath "Node.JSON"

   # Display Node
   function Show-Node($node, $indent = 0) {
      Write-Output "$(' ' * ($indent * 2)) - $($node.Name) ($($node.Size)) bytes"
      if ($node.IsDirectory -and $node.Children) {
         foreach ($child in $node.Children) {
               Show-Node $child ($indent + 1)
         }
      }
   }
   Show-Node $Node
   ```

   **Sample Output:**

   ```
   - Users (1234567) bytes
     - Documents (234567) bytes
       - Report.docx (34567) bytes
       - Notes.txt (4567) bytes
     - Pictures (123456) bytes
       - Vacation.jpg (23456) bytes
       - Family.png (34567) bytes
   ```

## Project Structure

- **Models/**: Contains interfaces and classes representing filesystem nodes (`INode`, `DirectoryNode`, `FileNode`).
- **Behaviors/**: Encapsulates different filesystem traversal behaviors and related factories.
- **Services/**: Houses services like `FileSystemReader` responsible for reading and constructing the filesystem tree.

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
