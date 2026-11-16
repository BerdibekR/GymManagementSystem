\# Gym Management System



\## Project Overview



Gym Management System is a console-based application developed in C# using functional programming principles.



The application manages gym members, memberships, trainers, and workout sessions. It also provides analytical reports and uses a native C library to calculate workout load.



\## Technologies



\- C#

\- .NET 10

\- LINQ

\- Functional programming

\- xUnit

\- JSON

\- Native C

\- P/Invoke

\- GCC (MSYS2 UCRT64)



\## Requirements



\- Windows 10 or Windows 11

\- .NET 10 SDK

\- GCC compiler (MSYS2 UCRT64)

\- Visual Studio 2026 or another compatible C# development environment



\## Building the Project



Open Developer PowerShell in the solution directory and run:



powershell

dotnet restore

dotnet build



\## Running the Application



From the solution root directory, execute:



powershell

dotnet run --project src/GymManagementSystem/GymManagementSystem.csproj



The application provides a console menu for managing members, memberships, trainers, and workout sessions.



\## Running Tests



Execute all automated tests using:



powershell

dotnet test



The test suite currently contains 40 tests covering domain validation, functional operations, workflows, reports, persistence, the workout analysis pipeline, and native C integration.



\## Native C Library



The project uses a native C library through Platform Invocation Services (P/Invoke).



The native source file is:



text

native/workout\_calculator.c



The compiled library must be named:



text

workout\_calculator.dll



\### Compiling the Native Library



Open the MSYS2 UCRT64 terminal, navigate to the project directory, and execute:



bash

gcc -shared -o native/workout\_calculator.dll native/workout\_calculator.c



The DLL is configured to be copied into the application's output directory during the .NET build.



Make sure the native library architecture is compatible with the application architecture.



\## Data Persistence



The application uses JSON files to store:



\- Members

\- Trainers

\- Memberships

\- Workout sessions



The persistence layer supports loading and saving data and handles relevant file and JSON errors.



\## Main Features



\- Member registration and validation

\- Membership creation and pricing policies

\- Workout booking

\- Workout completion and cancellation

\- Trainer capacity validation

\- Membership revenue report

\- Trainer performance report

\- Functional programming with Result and Maybe types

\- Map and Bind operations

\- Currying and partial application

\- Deferred execution with IEnumerable and yield return

\- JSON data persistence

\- Native C workout load calculation

\- Workout analysis pipeline



\## Repository



GitHub: https://github.com/BerdibekR/GymManagementSystem



Branch: `master`



\## Author



Individual project developed as part of the Functional Programming course.

