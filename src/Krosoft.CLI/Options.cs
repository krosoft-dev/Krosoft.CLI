using CommandLine;

namespace Krosoft.CLI;

internal static class Options
{
    [Verb("git-pull", HelpText = "Exécuter un 'git pull' sur le dépôt.")]
    internal class GitPullOptions;

    [Verb("git-clean", HelpText = "Exécuter un 'git clean' sur le dépôt.")]
    internal class GitCleanOptions;
}