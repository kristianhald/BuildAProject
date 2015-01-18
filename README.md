# BuildAProject

If you have ever had projects being shared among several other solutions and had to recompile each of the other solutions to determine if the change in the shared projects broke the code, then this is the tool for you. The application compiles all solutions in a specified directory and puts the compiled dlls into a compilation directory. At the moment the tool requires projects using shared dlls to reference them in the compilation directory.

The application will during the execution retrieve nuget packages specified in 'packages.config' files, compile the C# projects it finds and execute NUnit tests automatically.

The application will automatically determine the dependencies between the projects based on the references in the project files and the nuget packages.config files.

## Building the application
This section assumes that you have cloned or copied the source code onto you local computer and that you have Powershell installed.
```Powershell
   $ cd $BuildAProject
   $ .\build.ps1
```

If everything goes well, the last lines shown in Powershell will be a build report with a status on each step in the build and status: ok.

The compiled files necessary to execute the build tool are located in:
```
   $BuildAProject\compile\build
```

## Executing the application
After building the application it is now possible to execute the application and build all projects in the provided path.
The application has the following parameters in your favorite shell:
```
   .\BuildAProjectCmd.exe -b <root directory of projects> -c <root folder of compiled files> -l <loglevel>
```

**root directory of projects** tells the application that all C# projects found at the provided directory and all subdirectories are to be compiled, unless an 'ignorefile' states a folder should be ignored by the application. The application will determine the dependencies between the projects based on the references in the .csproject files and compile each project in accordance with the dependencies.

**root folder of compiled files** tells the application where the output of the compiled projects must be placed. Projects generating a .dll file will be placed in *libraries\\{name of project}*, while projects generating .exe file will be placed in *executable\\{name of project}* along with the library files the executable file requires.

**loglevel** is best set with the value 5, as it will give information that it is running, but not provide information on each project compiled, nuget package downloaded or assembly tested. If this information is relevant for you, then raise the loglevel. Maximum is 9.

## Build environment example
The way I use the application is that I have a single directory named *projects* where all my solutions are located. Several of the solutions are support tools, that I reuse between solutions and therefore I want to make certain that when I change one of these, the other still work as expected.

I open a Powershell and do the following commands:
```Powershell
   cd c:\projects
   .\buildaprojectcmd -b . -l 5 -c compiled
```

The above compiles all my solutions in 'projects' and puts the files in a folder named 'compiled', where the dll files are placed in 'libraries' and exe files are placed in 'executables'.

## Acknowledgments
NUnit - Portions Copyright © 2002-2009 Charlie Poole or Copyright © 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov or Copyright © 2000-2002 Philip A. Craig 

Fluent Command Line Parser - Copyright (c) 2012 - 2013, Simon Williams - All rights reserved.
