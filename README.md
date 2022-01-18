[***Readme under construction***]

# ConsoleProgressBar
A versatile and easy to use ProgressBar for Console applications, written in C#. 

Is **.Net Standard 2.1** (cross-platform ready), but only tested on Windows.

## Features
* Simple to use with many configuration options
* Can show or hide a *Marquee*: a char that moves around the ProgressBar
* Maximum is optional: If `Maximum` is `null`, no progress will be shown (but you can show Marquee)
* Automatically calculates `Percentage` and *Estimated Remaining Time* (`TimeRemaining`)
* Optional `Text` in the same line as ProgressBar 
* Optional multiple `Descriptions` under ProgressBar
* Colors in all components: in ProgressBar `Layout` elements, in `Text` and `Descriptions`

You can define dynamic content or values, with lambda expressions for:
* Background and Foreground Colors of `Layout` elements, `Text` and `Descriptions`
* Content of `Text` and `Descriptions`
* Characters used in `Layout` to represent ProgressBar

### How it works
ProgressBar creates an internal Thread. In this thread the component updates its representation in Console every few time.
This time is configurable, modifying the ``Delay`` property (default: 75ms)



## Examples with images
### Default ProgressBar:

![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example01.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar { Maximum = max })
{
  for (int i = 0; i < max; i++)
  {
    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(); //Step in ProgressBar (Default is 1)
  }
}
```
### With params:

![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example02.gif)
#### Code:
```csharp
const int max = 1000;

//Create the ProgressBar
// initialPosition: Console Line to put the ProgressBar (optional, last Console line is assumed)
// autoStart: Optional, default true
// Maximum: The Max value in ProgressBar (Default is 100)
// Step: The increment when performStep (Default is 1)
using (var pb = new ProgressBar(initialPosition: 3, autoStart: false) { Maximum = max, Step = 2})
{
  pb.Start(); // autoStart=false -> we need start manually
  for (int i = 0; i < max; i+=pb.Step)
  {
    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(); //Step in ProgressBar
  }
}
```
### Without Maximum:
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example03.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
// Maximum: The Max value in ProgressBar (Default is 100)
using (var pb = new ProgressBar() { Maximum = null })
{
  for (int i = 0; i < max; i++)
  {
    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(); //Step in ProgressBar (Default is 1)
  }
}
```
### Setting text:
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example05.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar() { Maximum = max })
{
  //Setting fixed "Processing Text" 
  pb.Text.Body.Processing.SetValue("Processing, please wait...");

  //Setting "Done Text"
  pb.Text.Body.Done.SetValue("Well done!!");

  //Clear "Description Text"
  pb.Text.Description.Clear();

  for (int i = 0; i < max; i++)
  {
    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(); //Step in ProgressBar (Default is 1)
  }
}
```
### Setting contextual text:
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example06.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar() { Maximum = max })
{
  //Setting "Processing Text" with context 
  pb.Text.Body.Processing.SetValue(pb => ($"Processing {pb.ElementName}, please wait..."));

  //Setting "Done Text" with context
  pb.Text.Body.Done.SetValue(pb => $"Processed {pb.Maximum} in {pb.TimeProcessing.TotalSeconds}s.");

  //Clear "Description Text"
  pb.Text.Description.Clear();

  for (int i = 0; i < max; i++)
  {
    string elementName = Guid.NewGuid().ToString();

    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
  }
}
```
### Setting descriptions:
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example07.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar() { Maximum = max })
{
  //Clear "Description Text"
  pb.Text.Description.Clear();

  //Setting "Description Text" when "Processing"
  pb.Text.Description.Processing.AddNew().SetValue(pb => $"Element: {pb.ElementName}");
  pb.Text.Description.Processing.AddNew().SetValue(pb => $"Count: {pb.Value}");
  pb.Text.Description.Processing.AddNew().SetValue(pb => $"Processing time: {pb.TimeProcessing.TotalSeconds}s.");
  pb.Text.Description.Processing.AddNew().SetValue(pb => $"Estimated remaining time: {pb.TimeRemaining?.TotalSeconds}s.");

  //Setting "Description Text" when "Done"
  pb.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} elements in {pb.TimeProcessing.TotalSeconds}s.");

  for (int i = 0; i < max; i++)
  {
    string elementName = Guid.NewGuid().ToString();

    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
  }
}
```
### Writing on Console:
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example08.gif)
#### Code:
```csharp
const int max = 150;

//Create the ProgressBar
using (var pb = new ProgressBar() { Maximum = max, FixedInBottom = true })
{
  //Clear "Description Text"
  pb.Text.Description.Clear();
  
  for (int i = 0; i < max; i++)
  {
    string elementName = Guid.NewGuid().ToString();
    
    Task.Delay(50).Wait(); //Do something
    pb.PerformStep(); //Step in ProgressBar (Default is 1)
    
    //Writing on Console with ProgressBar
    pb.WriteLine($"> [{DateTime.Now:HH:mm:ss.fff}]: Processed {i}: {elementName}");
  }
}
```
### Styling (1):
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example09.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar(autoStart: false) { Maximum = max })
{
  // Hide Text
  pb.Text.Body.SetVisible(false);
  
  // Clear "Description Text"
  pb.Text.Description.Clear();

  // Setting "Description Indentation", with color
  pb.Text.Description.Indentation.SetValue("└───> ")
                                 .SetForegroundColor(ConsoleColor.Cyan);

  // Setting "Description Text" when "Done", with color
  pb.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} elements in {pb.TimeProcessing.TotalSeconds}s.")
                                   .SetForegroundColor(ConsoleColor.DarkBlue);
  
  // Hide "Margins"
  pb.Layout.Margins.SetVisible(false);

  // Setting "Body" layout
  pb.Layout.Body.SetValue('─');
  pb.Layout.Body.Progress.SetForegroundColor(ConsoleColor.DarkGreen);
  pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.DarkRed);
  
  // Setting "Marquee" layout
  pb.Layout.Marquee.SetValue('─');
  pb.Layout.Marquee.OverProgress.SetForegroundColor(ConsoleColor.Yellow);
  pb.Layout.Marquee.OverPending.SetForegroundColor(ConsoleColor.DarkYellow);

  // Setting ProgressBar width
  pb.Layout.ProgressBarWidth = Console.BufferWidth;

  pb.Start();

  for (int i = 0; i < max; i++)
  {
    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(); //Step in ProgressBar (Default is 1)
  }
}
```
### Styling (2):
![Output of Ussage](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Example10.gif)
#### Code:
```csharp
const int max = 500;

//Create the ProgressBar
using (var pb = new ProgressBar(autoStart: false) { Maximum = max })
{
  // Hide Text
  pb.Text.Body.SetVisible(false);

  // Clear "Description Text"
  pb.Text.Description.Clear();

  // Hide "Margins"
  pb.Layout.Margins.SetVisible(false);

  // Hide "Marquee"
  pb.Layout.Marquee.SetVisible(false);

  // Setting Body Colors
  pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.White)
                        .SetBackgroundColor(ConsoleColor.DarkRed);
  pb.Layout.Body.Progress.SetForegroundColor(ConsoleColor.Black)
                         .SetBackgroundColor(ConsoleColor.DarkGreen);

  // Setting Body Text (internal text), from Layout
  pb.Layout.Body.Text.SetVisible(true).SetValue(pb =>
  {
    if (pb.IsDone)
      return $"{pb.Value} elements processed in {pb.TimeProcessing.TotalSeconds}s.";
    else
      return $"{pb.Percentage}%... Remaining: {pb.TimeRemaining?.TotalSeconds}s. - Current: {pb.ElementName}";
  });

  // Setting ProgressBar width
  pb.Layout.ProgressBarWidth = Console.BufferWidth;

  pb.Start();

  for (int i = 0; i < max; i++)
  {
    string elementName = Guid.NewGuid().ToString();

    Task.Delay(10).Wait(); //Do something
    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
  }
}
```

Styling ProgressBar:

![Screencapture ConsoleProgressBar Demo](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Demo.gif)

![Screencapture ConsoleProgressBar Demo2](https://raw.githubusercontent.com/iluvadev/ConsoleProgressBar/main/docs/img/ProgressBarConsole-Demo2.gif)


## Install
[![Nuget](https://img.shields.io/nuget/v/iluvadev.ConsoleProgressBar?style=plastic)](https://www.nuget.org/packages/iluvadev.ConsoleProgressBar/)

Go to [Nuget project page](https://www.nuget.org/packages/iluvadev.ConsoleProgressBar/) to see options


