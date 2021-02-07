using System;
using static System.Console;
using System.Net;
using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Connections;

namespace BWHazel.Apps.QuantumTelloport
{
    /// <summary>
    /// Main logic to control and fly the Ryze Tello drone.
    /// </summary>
    public class DronePilot
    {
        private static string TakeoffCommand = "takeoff";
        private static string LandCommand = "land";

        public DronePilot(CommandValues commandValues)
        {
            this.CommandValues = commandValues;
        }

        /// <summary>
        /// Gets or sets the command-line argument values store.
        /// </summary>
        public CommandValues CommandValues { get; init; }

        /// <summary>
        /// Gets or sets the drone commander.
        /// </summary>
        public DroneCommander DroneCommander { get; set; }

        /// <summary>
        /// Starts and runs the drone flight.
        /// </summary>
        public void Start()
        {
            this.DroneCommander = this.InitialiseDroneCommander();
            this.DroneCommander.Connect();

            this.DroneCommander.Disconnect();
        }

        /// <summary>
        /// Initialises a commander to send commands to the Tello drone.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">Information about the event.</param>
        private void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs eventArgs)
        {
            this.TerminateFlight();   
        }

        /// <summary>
        /// Initialises a commander to send commands to the Tello drone.
        /// </summary>
        private DroneCommander InitialiseDroneCommander()
        {
            CommandDictionary commandDictionary =
                CommandDictionary.ReadStandardDictionary(this.CommandValues.TelloApiVersion);
            
            TelloConnection telloConnection = this.CreateTelloConnection();
            return new(telloConnection, commandDictionary);
        }

        /// <summary>
        /// Creates a connection to the Tello drone or simulator.
        /// </summary>
        private TelloConnection CreateTelloConnection()
        {
            if (this.CommandValues.UseSimulator is true)
            {
                return new(IPAddress.Loopback.ToString(),
                    TelloConnection.DefaultTelloPort,
                    ConnectionType.Simulator
                );
            }
            else
            {
                return new();
            }
        }

        /// <summary>
        /// Terminates the flight by landing and disconnecting the drone.
        /// </summary>
        private void TerminateFlight()
        {
            this.DroneCommander.RunCommand(LandCommand);
            this.DroneCommander.Disconnect();
        }
    }
}