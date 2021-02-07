using System.CommandLine;
using BWHazel.Apps.QuantumTelloport;

CommandBuilder commandBuilder = new(new CommandValues());
RootCommand rootCommand = commandBuilder.BuildCommandStructure();

return rootCommand.InvokeAsync(args).Result;