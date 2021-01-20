# ProgressBar

`Namespace: ConsoleProgressBar`

A ProgressBar for Console

```csharp
public class ProgressBar
    : IDisposable
```

## Properties

| Type | Name | Summary |
| --- | --- | --- |
| `Int32` | Delay | Delay for repaint and recalculate all ProgressBar  Default = 75 |
| `DescriptionDefinition` | Description | Description of the ProgressBar |
| `String` | ElementName | The Name of the Curent Element |
| `Boolean` | FixedInBottom | True to Print the ProgressBar always in last Console Line  False to Print the ProgressBar fixed in Console (Current position at Starting)  You can Write at Console and ProgressBar will always be below your lines  Default = true |
| `Boolean` | HasProgress | Indicates if the ProgressBar has Progress defined (Maximum defined) |
| `Boolean` | IsDone | True if ProgresBar is Done: when disposing or Progress is finished |
| `Boolean` | IsPaused | True if ProgressBar is Paused |
| `Boolean` | IsStarted | True if ProgressBar is Started |
| `LayoutDefinition` | Layout | Layout of the ProgressBar |
| `Nullable<Int32>` | Maximum | The Maximum value  Default = 100 |
| `Nullable<Int32>` | Percentage | Percentage of progress |
| `Int32` | Step | The amount by which to increment the ProgressBar with each call to the PerformStep() method.  Default = 1 |
| `Object` | Tag | Tag object |
| `TextDefinition` | Text | Text of the ProgressBar |
| `Nullable<TimeSpan>` | TimePerElement | Processing time per element (median) |
| `TimeSpan` | TimeProcessing | Processing time (time paused excluded) |
| `Nullable<TimeSpan>` | TimeRemaining | Estimated time finish (to Value = Maximum) |
| `Int32` | Value | The current Value  If Value is greater than Maximum, then updates Maximum value |

## Methods

| Type | Name | Summary |
| --- | --- | --- |
| `void` | Dispose() | Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. |
| `void` | Pause() | Pauses the ProgressBar |
| `void` | PerformStep(`String` elementName = null, `Object` tag = null) | Advances the current position of the progress bar by the amount of the Step property |
| `void` | Print() | Prints in Console the ProgressBar |
| `void` | Resume() | Resume the ProgresBar |
| `void` | SetValue(`Int32` value, `String` elementName = null, `Object` tag = null) | Assigns the current Value, and optionally current ElementName and Tag  If Value is greater than Maximum, updates Maximum as Value |
| `void` | Start() | Starts the ProgressBar |
| `void` | Unprint() | Unprints (remove) from Console last ProgressBar printed |
| `void` | WriteLine(`String` value, `Boolean` truncateToOneLine = True) |  |
| `void` | WriteLine(`String` value, `Nullable<ConsoleColor>` foregroundColor = null, `Nullable<ConsoleColor>` backgroundColor = null, `Boolean` truncateToOneLine = True) |  |

## Static Fields

| Type | Name | Summary |
| --- | --- | --- |
| `Object` | ConsoleWriterLock | A Lock for Writing to Console |

---

[`< Back`](../)
