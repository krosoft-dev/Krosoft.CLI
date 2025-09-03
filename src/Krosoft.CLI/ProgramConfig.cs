////namespace Krosoft.CLI;

//using System.Diagnostics;

//internal static class ProgramConfig
//{
////    public static Task<int> Configure(Options.ConfigureOptions opts)
////        => GetManager()
////            .Configure(opts.Token, opts.Url, opts.Agent, opts.Validate);

////    public static Task<int> Status() => GetManager().Status();
////    public static Task<int> Remove() => GetManager().Remove();
//    public static Task<int> Pull() => GetManager().Pull();
//    public static Task<int> Clean() => GetManager().Clean();

//    private static IGitManager GetManager() => new GitManager();

////    private static IConfigManager GetManager() => BuildHost()
////                                                  .Services
////                                                  .GetRequiredService<IConfigManager>();

////    private static IHost BuildHost()
////    {
////        var builder = HostApplicationBuilderHelper.Create(null, false, false, false);
////        builder.Services.AddTransient<IConfigManager, ConfigManager>();
////        var host = builder.Build();
////        return host;
////    }
//}

//internal interface IGitManager
//{
//    Task<int> Pull();
//    Task<int> Clean();
//}

//internal class GitManager : IGitManager
//{
//    public Task<int> Pull()
//    {
//        Console.WriteLine("PULL");
//        return Task.FromResult(1);
//    }

//    public Task<int> Clean()
//    {
//        Console.WriteLine("Clean");
//        return Task.FromResult(1);
//    }
//}

using System.Diagnostics;

namespace Krosoft.CLI;

internal static class ProgramConfig
{
    public static Task<int> Pull() => GetManager().Pull();
    public static Task<int> Clean() => GetManager().Clean();

    private static IGitManager GetManager() => new GitManager();
}

internal interface IGitManager
{
    Task<int> Pull();
    Task<int> Clean();
}

internal class GitManager : IGitManager
{
    public async Task<int> Pull()
    {
        try
        {
            // Affichage du header comme dans le PowerShell
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==========================================");
            Console.WriteLine("Pull Repository");
            Console.WriteLine("==========================================");

            // Affichage du chemin actuel
            var currentPath = Directory.GetCurrentDirectory();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Path : ");
            Console.WriteLine(currentPath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==========================================");

            // Réinitialisation de la couleur
            Console.ResetColor();

            // Exécution de git pull
            var result = await ExecuteGitCommand("pull");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(); // Ligne vide finale comme dans le PowerShell
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Nettoyage du repository...");
            Console.ResetColor();

            var result = await ExecuteGitCommand("clean -fd");
            return result;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erreur lors du clean : {ex.Message}");
            Console.ResetColor();
            return 1;
        }
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