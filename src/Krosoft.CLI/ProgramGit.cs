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

using Krosoft.CLI.Interfaces;
using Krosoft.CLI.Managers;

namespace Krosoft.CLI;

internal static class ProgramGit
{
    public static Task<int> Pull() => GetManager().Pull();
    public static Task<int> Clean() => GetManager().Clean();

    private static IGitManager GetManager() => new GitManager();
}

 

//    internal class GitManager : IGitManager
//    {
//        public async Task<int> Pull()
//        {
//            DisplayHeader("Pull Repository");
//            return await ExecuteGitCommand("pull");
//        }

//        public async Task<int> Clean()
//        {
//            DisplayHeader("Clean branch of Repository");

//            var pullResult = await ExecuteGitCommand("pull");
//            if (pullResult != 0) return pullResult;

//            var pruneResult = await ExecuteGitCommand("fetch origin --prune");
//            if (pruneResult != 0) return pruneResult;

//            await CleanGoneBranches();

//            DisplayFooter();
//            return 0;
//        }

//        private void DisplayHeader(string title)
//        {
//            WriteColoredLine(ConsoleColor.Green, "==========================================");
//            WriteColoredLine(ConsoleColor.Green, title);
//            WriteColoredLine(ConsoleColor.Green, "==========================================");

//            WriteColored(ConsoleColor.Blue, "Path : ");
//            Console.WriteLine(Directory.GetCurrentDirectory());

//            WriteColoredLine(ConsoleColor.Green, "==========================================");
//            Console.ResetColor();
//        }

//        private void DisplayFooter()
//        {
//            WriteColoredLine(ConsoleColor.Green, "");
//        }

//        private void WriteColoredLine(ConsoleColor color, string text)
//        {
//            Console.ForegroundColor = color;
//            Console.WriteLine(text);
//            Console.ResetColor();
//        }

//        private void WriteColored(ConsoleColor color, string text)
//        {
//            Console.ForegroundColor = color;
//            Console.Write(text);
//            Console.ResetColor();
//        }

//        private async Task<int> ExecuteGitCommand(string arguments)
//        {
//            try
//            {
//                var processInfo = new ProcessStartInfo
//                {
//                    FileName = "git",
//                    Arguments = arguments,
//                    UseShellExecute = false,
//                    RedirectStandardOutput = true,
//                    RedirectStandardError = true,
//                    CreateNoWindow = true
//                };

//                using var process = Process.Start(processInfo);
//                if (process == null) return HandleError("Impossible de démarrer git");

//                await Task.WhenAll(
//                    RedirectOutput(process.StandardOutput),
//                    RedirectOutput(process.StandardError, ConsoleColor.Red)
//                );

//                await process.WaitForExitAsync();
//                return process.ExitCode;
//            }
//            catch (Exception ex)
//            {
//                return HandleError($"Erreur lors de l'exécution de 'git {arguments}' : {ex.Message}");
//            }
//        }

//        private async Task RedirectOutput(StreamReader reader, ConsoleColor? color = null)
//        {
//            while (!reader.EndOfStream)
//            {
//                var line = await reader.ReadLineAsync();
//                if (!string.IsNullOrEmpty(line))
//                {
//                    if (color.HasValue)
//                    {
//                        WriteColoredLine(color.Value, line);
//                    }
//                    else
//                    {
//                        Console.WriteLine(line);
//                    }
//                }
//            }
//        }

//        private async Task CleanGoneBranches()
//        {
//            try
//            {
//                var output = await GetGitBranchOutput();
//                if (string.IsNullOrEmpty(output)) return;

//                var goneBranches = ParseGoneBranches(output);
//                await DeleteBranches(goneBranches);
//            }
//            catch (Exception ex)
//            {
//                HandleError($"Erreur lors du nettoyage des branches : {ex.Message}");
//            }
//        }

//        private async Task<string> GetGitBranchOutput()
//        {
//            var processInfo = new ProcessStartInfo
//            {
//                FileName = "git",
//                Arguments = "branch -vv",
//                UseShellExecute = false,
//                RedirectStandardOutput = true,
//                CreateNoWindow = true
//            };

//            using var process = Process.Start(processInfo);
//            if (process == null) return string.Empty;

//            var output = await process.StandardOutput.ReadToEndAsync();
//            await process.WaitForExitAsync();

//            return process.ExitCode == 0 ? output : string.Empty;
//        }

//        private string[] ParseGoneBranches(string branchOutput)
//        {
//            var goneBranches = new List<string>();
//            var lines = branchOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);

//            foreach (var line in lines)
//            {
//                if (IsGoneBranch(line))
//                {
//                    var branchName = ExtractBranchName(line);
//                    if (!string.IsNullOrEmpty(branchName) && !branchName.StartsWith("*"))
//                    {
//                        goneBranches.Add(branchName);
//                    }
//                }
//            }

//            return goneBranches.ToArray();
//        }

//        private bool IsGoneBranch(string line) => 
//            line.Contains("[origin/") && line.Contains(": gone]");

//        private string ExtractBranchName(string line)
//        {
//            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
//            return parts.Length > 0 ? parts[0] : string.Empty;
//        }

//        private async Task DeleteBranches(string[] branches)
//        {
//            foreach (var branch in branches)
//            {
//                Console.WriteLine($"Suppression de la branche locale : {branch}");
//                await ExecuteGitCommand($"branch -D {branch}");
//            }
//        }

//        private int HandleError(string message)
//        {
//            WriteColoredLine(ConsoleColor.Red, message);
//            return 1;
//        }
//    }

//    internal class Program
//    {
//        public static async Task<int> Main(string[] args)
//        {
//            try
//            {
//                return args.Length == 0 ? ShowUsage() : await ExecuteCommand(args[0]);
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"Erreur : {ex.Message}");
//                Console.ResetColor();
//                return 1;
//            }
//        }

//        private static int ShowUsage()
//        {
//            Console.WriteLine("Usage: git-cli [pull|clean]");
//            return 1;
//        }

//        private static Task<int> ExecuteCommand(string command) => command.ToLower() switch
//        {
//            "pull" => ProgramConfig.Pull(),
//            "clean" => ProgramConfig.Clean(),
//            _ => Task.FromResult(HandleInvalidCommand(command))
//        };

//        private static int HandleInvalidCommand(string command)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine($"Commande inconnue: {command}");
//            Console.ResetColor();
//            ShowUsage();
//            return 1;
//        }
//    }
//}