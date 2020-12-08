using System.CommandLine;
using System.CommandLine.Invocation;

namespace BWHazel.Games.ReactionDuel
{
    /// <summary>
    /// Builder of the program command structure.
    /// </summary>
    public class CommandBuilder
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CommandBuilder"/> class with <see cref="CommandValues"/> instance.
        /// </summary>
        /// <param name="commandValues">The command-line argument values store.</param>
        public CommandBuilder(CommandValues commandValues)
        {
            this.CommandValues = commandValues;
        }

        /// <summary>
        /// Gets or initialises the command-line values store.
        /// </summary>
        public CommandValues CommandValues { get; init; }

        /// <summary>
        /// Builds the program command structure.
        /// </summary>
        /// <returns>The root command with program command structure.</returns>
        public RootCommand BuildCommandStructure()
        {
            RootCommand rootCommand = new()
            {
                new Option<string>(
                    new[] { "-d", "--device" },
                    "The USB device ID."
                ),
                new Option<string>(
                    new[] { "-1", "--player-one" },
                    "The name of player 1."
                ),
                new Option<string>(
                    new[] { "-2", "--player-two" },
                    "The name of player 2."
                )
            };

            rootCommand.Description = "Reaction Duel Game";
            rootCommand.Handler = CommandHandler.Create<string, string, string>(
                (device, playerOne, playerTwo) =>
                {
                    this.CommandValues.DeviceId = device;
                    this.CommandValues.Player1Name = playerOne;
                    this.CommandValues.Player2Name = playerTwo;

                    System.Console.WriteLine(device);
                    System.Console.WriteLine(playerOne);
                    System.Console.WriteLine(playerTwo);
                }
            );

            return rootCommand;
        }
    }
}
