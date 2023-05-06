
<h1 align="center">
    TaskExecutor
</h1>

<p align="center">
   <a href="https://nuget.org/packages/TaskExecutor"><img src="https://img.shields.io/nuget/dt/TaskExecutor.svg?label=Downloads&color=%233DDC84&logo=nuget&logoColor=%23fff&style=for-the-badge"></a>
</p>

**TaskExecutor** is a library that makes it easier to run multiple tasks simultaneously by using of Semaphore.

## Install

- 📦 [NuGet](https://nuget.org/packages/TaskExecutor): `dotnet add package TaskExecutor`

## Usage

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskExecutor;

var myAction = MyActionAsync;
var actions = Enumerable.Range(0, 11).Select(_ => myAction);

// This runs 2 tasks at a time and wait unitl all tasks are completed.
var result = await TaskEx.Run(actions, 2);

static int HelloCount;
async Task<int> MyActionAsync()
{
    await Task.Delay(1000);

    HelloCount++;
    Console.WriteLine($"Hello ({HelloCount})");

    return 0;
}
```
