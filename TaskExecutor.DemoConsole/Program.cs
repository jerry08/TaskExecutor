using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaskExecutor.DemoConsole;

internal static class Program
{
    public static int HelloCount;
    public static int HiCount;

    static async Task Main()
    {
        Console.WriteLine("TaskExecutor Demo");

        Console.WriteLine();
        Console.WriteLine(@"Running 1 ""Hello"" task at a time (from a total of 3)...");

        await TaskExecutor.Run(new[] { Action1, Action1, Action1 });

        Console.WriteLine();
        Console.WriteLine(@"Running 10 ""Hi"" tasks at a time (from a total of 50)...");

        //var myAction = Action2;
        //var actions = Enumerable.Range(0, 50).Select(_ => myAction);
        // OR cast in expression
        var actions = Enumerable.Range(0, 50).Select(_ => (Func<Task<int>>)Action2);

        await TaskExecutor.Run(actions, 10);

        Console.WriteLine();
        Console.WriteLine("Running 4 tasks at a time (from a total of 17)...");

        var getSchoolAsync = GetSchoolAsync;
        var schoolActions = Enumerable.Range(0, 17).Select(_ => getSchoolAsync);

        var schools = await TaskExecutor.Run(schoolActions, 4);

        Console.WriteLine();
        Console.WriteLine($"General Students Total: {schools.Sum(x => x.TotalStudents)}");

        Console.WriteLine();

        var func = () => Action3("John");
        await TaskExecutor.Run(new[] { func });

        // Run asynchronous task synchronously
        Console.WriteLine();
        Console.WriteLine("Running asynchronous task synchronously...");

        TaskExecutor.RunSync(Action1);
        var result = TaskExecutor.RunSync(Action2);
    }

    static async Task Action1()
    {
        await Task.Delay(1000);

        HiCount++;
        Console.WriteLine($"Hi ({HiCount})");
    }

    static async Task<int> Action2()
    {
        await Task.Delay(1000);

        HelloCount++;
        Console.WriteLine($"Hello ({HelloCount})");

        return 0;
    }

    static async Task<int> Action3(string message)
    {
        await Task.Delay(1000);

        HelloCount++;
        Console.WriteLine($"Hello - {message} ({HelloCount})");

        await Task.Delay(1000);

        return 0;
    }

    static async Task<School> GetSchoolAsync()
    {
        await Task.Delay(1000);

        var school = new School()
        {
            TotalStudents = new Random().Next(100, 1000)
        };

        Console.WriteLine($"Total Students: {school.TotalStudents}");

        return school;
    }
}