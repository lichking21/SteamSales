using Domain;
using Microsoft.Extensions.Logging;
using Application;
using Infrastructure;

public class Program
{
    static async Task Main()
    {
        await ProgramRun.Execute();
    }
}