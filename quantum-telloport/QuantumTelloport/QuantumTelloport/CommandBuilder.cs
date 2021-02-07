using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace BWHazel.Apps.QuantumTelloport
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
                    new[] { "-a", "--api" },
                    () => CommandValues.DefaultTelloApiVersion,
                    "The Tello API version."
                ),
                new Option<int>(
                    new[] { "-r", "--run-time" },
                    () => CommandValues.DefaultTotalRunTime,
                    "The total run-time in seconds."
                ),
                new Option<int>(
                    new[] { "-p", "--pause-time" },
                    () => CommandValues.DefaultPauseTime,
                    "The pause time between drone commands in seconds."
                ),
                new Option<bool>(
                    new[] { "-s", "--simulate" },
                    "Use the drone simulator."
                )
            };

            rootCommand.Description = "Quantum Telloport";
            rootCommand.Handler = CommandHandler.Create<string, int, int, bool>(
                async (api, runTime, pauseTime, simulate) =>
                {
                    this.CommandValues.TelloApiVersion = api;
                    this.CommandValues.TotalRunTime = runTime;
                    this.CommandValues.PauseTime = pauseTime;
                    this.CommandValues.UseSimulator = simulate;

                    DronePilot dronePilot = new(this.CommandValues);
                    await dronePilot.Start();
                }
            );

            return rootCommand;
        }
    }
}
