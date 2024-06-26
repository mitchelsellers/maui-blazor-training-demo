The initial goal is to create the new project using the .NET Core Template and ensure that you are able to run/debug all aspects of the application.

* Create a new folder to store the project, such as C:\Repos\MyDemo
* Within Visual Studio "File" -> "New Project"
* Select MAUI Blazor Hybrid App
  * Project Name: DemoApplication.UI
  * Location: Your Folder
  * Solution: Create New
  * Solution name: src
* Select Framework: .NET 8.0 (Long Term Support)
* After creation right click on the solution and rename "Demo Application"

## Validate Functionality & VS Setup

* Build -> Build Solution - Should busines successfully
* Debug -> Start Debugging - Should launch Windows project
* Change target to Android Emulator
  * Debug should then launch and see the app on Android