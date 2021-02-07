using System;
using static System.Console;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Connections;
using BWHazel.Apps.QuantumTelloport.Control;

namespace BWHazel.Apps.QuantumTelloport
{
    /// <summary>
    /// Main logic to control and fly the Ryze Tello drone.
    /// </summary>
    public class DronePilot
    {
        private static string TakeoffCommand = "takeoff";
        private static string LandCommand = "land";
        private static string ForwardCommand = "forward";
        private static string BackCommand = "back";
        private static string RightCommand = "right";
        private static string LeftCommand = "left";
        private static string UnknownCommand = "unknown";
        private static int MinimumDroneDistance = 20;

        /// <summary>
        /// Initialises a new instance of the <see cref="DronePilot"/> class with <see cref="CommandValues"/> instance.
        /// </summary>
        /// <param name="commandValues">The command-line argument values store.</param>
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
        /// Gets or sets the flight start time.
        /// </summary>
        public DateTime FlightStartTime { get; set; }

        /// <summary>
        /// Gets or sets the quantum simulator.
        /// </summary>
        public QuantumSimulator QuantumSimulator { get; set; } = new();

        /// <summary>
        /// Starts and runs the drone flight.
        /// </summary>
        public async Task Start()
        {
            WriteLine("*** QUANTUM TELLOPORT ***");
            WriteLine("Initialising drone commander...");
            this.DroneCommander = this.InitialiseDroneCommander();
            CancelKeyPress += this.OnConsoleCancelKeyPress;
            
            WriteLine("Starting flight...");
            this.FlightStartTime = DateTime.Now;
            this.StartFlight();

            int droneMove = 1;
            double totalElapsedTime = 0;
            while (totalElapsedTime <= this.CommandValues.TotalRunTime)
            {
                WriteLine($"Starting drone move {droneMove}...");
                long axisQuantumResult = await DetermineAxis.Run(this.QuantumSimulator);
                long directionQuantumResult = await DetermineDirection.Run(this.QuantumSimulator);
                long distanceQuantumResult = await DetermineDistance.Run(this.QuantumSimulator);
                
                long droneDistance = MinimumDroneDistance + distanceQuantumResult;
                string droneCommand = (axisQuantumResult, directionQuantumResult) switch
                {
                    (1, 1) => $"{ForwardCommand} {droneDistance}",
                    (1, 0) => $"{BackCommand} {droneDistance}",
                    (0, 1) => $"{RightCommand} {droneDistance}",
                    (0, 0) => $"{LeftCommand} {droneDistance}",
                    (_, _) => $"{UnknownCommand}"
                };

                if (droneCommand == UnknownCommand)
                {
                    break;
                }

                this.DroneCommander.RunCommand(droneCommand);
                WriteLine($"Executing command: {droneCommand}...");
                Thread.Sleep(this.CommandValues.PauseTime * 1000);

                totalElapsedTime = (DateTime.Now - this.FlightStartTime).TotalSeconds;
                droneMove++;
                WriteLine($"Total elapsed flight time: {(int)totalElapsedTime}s of {this.CommandValues.TotalRunTime}s");
            }

            this.TerminateFlight();
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
                WriteLine("Creating connection to drone simulator...");
                return new(IPAddress.Loopback.ToString(),
                    TelloConnection.DefaultTelloPort,
                    ConnectionType.Simulator
                );
            }
            else
            {
                WriteLine("Creating connection to drone...");
                return new();
            }
        }

        /// <summary>
        /// Starts the flight by connecting the drone and taking off.
        /// </summary>
        private void StartFlight()
        {
            WriteLine("Establishing connection...");
            this.DroneCommander.Connect();

            WriteLine("Taking off...");
            this.DroneCommander.RunCommand(TakeoffCommand);
        }

        /// <summary>
        /// Terminates the flight by landing and disconnecting the drone.
        /// </summary>
        private void TerminateFlight()
        {
            WriteLine("Landing...");
            this.DroneCommander.RunCommand(LandCommand);

            WriteLine("Disconnecting...");
            this.DroneCommander.Disconnect();
        }
    }
}