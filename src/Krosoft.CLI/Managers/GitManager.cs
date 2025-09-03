using Krosoft.CLI.Interfaces;
using System.Diagnostics;

namespace Krosoft.CLI.Managers;

internal class GitManager : IGitManager
{
    public async Task<int> Pull()
    {
        try
        {
            DisplayHeader("Pull repository");

            var result = await ExecuteGitCommand("pull");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.ResetColor();

            return result;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erreur lors du pull : {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    public async Task<int> Clean()
    {
        try
        {
            DisplayHeader("Clean up all old branches");

            var pullResult = await ExecuteGitCommand("pull");
            if (pullResult != 0)
            {
                return pullResult;
            }

            var pruneResult = await ExecuteGitCommand("fetch origin --prune");
            if (pruneResult != 0)
            {
                return pruneResult;
            }

            var cleanGoneBranchesResult = await ExecuteGitCommand("branch -vv | where { $_ -match '\\[origin/.*: gone\\]' } | foreach { git branch -D ($_.split(\" \", [StringSplitOptions]'RemoveEmptyEntries')[0])");
            if (cleanGoneBranchesResult != 0)
            {
                return cleanGoneBranchesResult;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erreur lors du clean : {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static void DisplayHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("==========================================");
        Console.WriteLine(title);
        Console.WriteLine("==========================================");

        var currentPath = Directory.GetCurrentDirectory();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("Path : ");
        Console.WriteLine(currentPath);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("==========================================");

        Console.ResetColor();
    }

    private async Task<int> ExecuteGitCommand(string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        if (process == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Impossible de démarrer le processus git");
            Console.ResetColor();
            return 1;
        }

        // Affichage de la sortie en temps réel
        var outputTask = Task.Run(async () =>
        {
            while (!process.StandardOutput.EndOfStream)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    Console.WriteLine(line);
                }
            }
        });

        var errorTask = Task.Run(async () =>
        {
            while (!process.StandardError.EndOfStream)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(line);
                    Console.ResetColor();
                }
            }
        });

        await process.WaitForExitAsync();
        await Task.WhenAll(outputTask, errorTask);

        return process.ExitCode;
    }
}