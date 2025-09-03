using CommandLine;

namespace Krosoft.CLI;

internal static class Program
{
    private static async Task<int> Main(params string[] args)
    {
        PrintBanner();
        return await Parser.Default.ParseArguments<Options.GitPullOptions, Options.GitCleanOptions
                           >(args)
                           .MapResult(
                                      (Options.GitPullOptions _) => ProgramConfig.Pull(),
                                      (Options.GitCleanOptions _) => ProgramConfig.Clean(),

                                      //(Options.RunOptions opts) => ProgramRun.Run(opts),
                                      _ => Task.FromResult(-1));
    }

    private static void PrintBanner()
    {
        const string banner = """

                                _  __                     __ _   
                               | |/ /                    / _| |  
                               | ' / _ __ ___  ___  ___ | |_| |_ 
                               |  < | '__/ _ \/ __|/ _ \|  _| __|
                               | . \| | | (_) \__ \ (_) | | | |_ 
                               |_|\_\_|  \___/|___/\___/|_|  \__|
                                    
                               
                              """;
        Console.WriteLine(banner);
    }
}