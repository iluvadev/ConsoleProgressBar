# ConsoleProgressBar
A ProgressBar for Console, in C# 
It needs only a file: ConsoleProgressBar.cs
(Readme under construction)

![Screencapture ConsoleProgressBar with DefaultConfig: LongRunning](Images/ProgressBarConsole-DefaultConfig-LongRunning.gif)
Code:
```csharp
using (var pg = new ProgressBar() { Maximum = 5 }) //Create the ProgressBar
{
	for (int i = 0; i < 5; i++) //Iterate over elements
	{
		//Assign Current element Name
		pg.CurrentElementName = elementName[i];
		
		Task.Delay(2000).Wait();	//Do long running task
		
		//PerformStep in ProgressBar
		pg.PerformStep();
	}
}
```
 
 ![Screencapture ConsoleProgressBar without Progress (only Marquee)](ProgressBarConsole-DefaultConfig-NoProgress.gif)
Code:
```csharp
using (var pg = new ProgressBar() { ShowProgress = false }) //Create the ProgressBar
{
	for (int i = 0; i < 500; i++) //Iterate over elements 
	{
		//Assign Current element Name
		pg.CurrentElementName = elementName[i];
		
		Task.Delay(10).Wait();	//Do something
		
		//PerformStep in ProgressBar
		pg.PerformStep();
	}
}
```