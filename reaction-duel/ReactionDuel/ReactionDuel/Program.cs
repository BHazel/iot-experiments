using System.CommandLine;
using BWHazel.Games.ReactionDuel;

CommandBuilder commandBuilder = new(new CommandValues());
RootCommand rootCommand = commandBuilder.BuildCommandStructure();

return rootCommand.InvokeAsync(args).Result;