# JASON Compiler Project

A compiler implementation written in C# for the JASON language, featuring lexical analysis (scanning), syntax analysis (parsing), and a graphical user interface for code compilation and error visualization.

## Overview

This project is a complete compiler frontend implementation that includes:
- **Scanner (Lexical Analyzer)**: Tokenizes source code
- **Parser (Syntax Analyzer)**: Builds parse trees and validates syntax
- **GUI Application**: Windows Forms-based interface for interactive compilation
- **Error Handling**: Comprehensive error detection and reporting

## Project Structure

```
JASON_Compiler/
├── Scanner.cs              # Lexical analysis - tokenization
├── Parser.cs               # Syntax analysis - parsing and AST generation
├── Errors.cs               # Error handling and error definitions
├── JASON_Compiler.cs       # Main compiler class
├── Program.cs              # Application entry point
├── Form1.cs                # Primary GUI form
├── Form1.Designer.cs       # GUI designer-generated code
├── Form1.resx              # GUI resources
├── Form2.cs                # Secondary GUI form
├── Form2.Designer.cs       # GUI designer-generated code
├── Form2.resx              # GUI resources
├── NourParser              # Parser documentation/reference
├── JASON_Compiler.sln      # Visual Studio solution file
├── JASON_Compiler.csproj   # C# project file
├── App.config              # Application configuration
└── README.md               # This file
```

## Key Components

### Scanner.cs
Implements lexical analysis to tokenize input source code. Identifies keywords, identifiers, literals, operators, and special symbols according to the JASON language specification.

### Parser.cs
Implements syntax analysis using parsing algorithms to:
- Validate grammatical correctness of source code
- Build abstract syntax trees (AST)
- Generate detailed error messages with line and column information

### Errors.cs
Defines error types and error handling mechanisms for reporting compilation errors back to the user.

### GUI Application (Form1, Form2)
Windows Forms application providing:
- Source code editor
- Compilation controls
- Error message display
- Output visualization

## Technologies Used

- **Language**: C#
- **Framework**: .NET Framework (Windows Forms)
- **IDE**: Visual Studio
- **Platform**: Windows

## Getting Started

### Prerequisites
- Visual Studio (2019 or later)
- .NET Framework 4.7.2 or higher

### Building the Project
1. Open `JASON_Compiler.sln` in Visual Studio
2. Build the solution (Build → Build Solution)
3. Run the application (Debug → Start Debugging)

## Usage

1. **Launch the application** - Run the compiled executable
2. **Enter source code** - Type or paste JASON language code into the editor
3. **Compile** - Click the compile button to analyze the code
4. **View results** - Check the output panel for:
   - Successful compilation messages
   - Syntax errors with location details
   - Semantic errors if applicable

## Features

- ✅ Lexical analysis with comprehensive token recognition
- ✅ Syntax analysis with detailed error reporting
- ✅ Interactive GUI for user-friendly compilation
- ✅ Error visualization with line and column numbers
- ✅ Support for the JASON programming language syntax

## Future Enhancements

- Semantic analysis and type checking
- Code generation to intermediate representation
- Optimization passes
- Support for code completion and syntax highlighting in editor

## License

This project is created as an educational compiler implementation.

## Author

**nourMelegy** - Compiler Project Implementation

---

**Created**: March 3, 2026  
**Last Updated**: May 12, 2026
