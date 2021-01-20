[***Readme under construction***]

# ConsoleProgressBar
A versatile and really easy to use ProgressBar for Console applications, written in C#. 

Is **.Net Core** ready (cross-platform), but only tested on Windows.

If you want to use it in your projects, simply add the file [`ProgressBar.cs`](ConsoleProgressBar\ProgressBar.cs)

## Examples in images
Default ProgressBar:
![Output of Ussage](docs/img/ProgressBarConsole-Example_Usage1.gif)

Writing on Console:
![Screencapture ConsoleProgressBar Demo3](docs/img/ProgressBarConsole-Demo3.gif)
![Screencapture ConsoleProgressBar Demo4](docs/img/ProgressBarConsole-Demo4.gif)

Styling ProgressBar:
![Screencapture ConsoleProgressBar Demo](docs/img/ProgressBarConsole-Demo.gif)
![Screencapture ConsoleProgressBar Demo2](docs/img/ProgressBarConsole-Demo2.gif)


## Install
All the code of the ProjectBar is included in only one file: [`ProgressBar.cs`](ConsoleProgressBar\ProgressBar.cs) 

You can download this file and use it in your own projects. Simple, without dependencies.

## Usage
You can configure a lot of things, but usage is very simple:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar{ Maximum = max })
{
    for (int i = 0; i < max; i++)
    {
        Task.Delay(50).Wait(); //Do thinks
        pb.PerformStep(); //Step in ProgressBar (Default is 1)
    }
}
```
And produces:
![Output of Ussage](docs/img/ProgressBarConsole-Example_Usage1.gif)
