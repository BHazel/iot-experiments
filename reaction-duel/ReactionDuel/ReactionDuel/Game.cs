using System;
using static System.Console;
using System.IO.Ports;

namespace BWHazel.Games.ReactionDuel
{
    /// <summary>
    /// Main game logic.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class with <see cref="CommandValues"/> instance.
        /// </summary>
        /// <param name="commandValues">The command-line argument values store.</param>
        public Game(CommandValues commandValues)
        {
            this.CommandValues = commandValues;
        }

        /// <summary>
        /// Gets or sets the command-line argument values store.
        /// </summary>
        public CommandValues CommandValues { get; init; }

        /// <summary>
        /// Gets or sets the name of Player 1.
        /// </summary>
        public string Player1Name { get; set; }

        /// <summary>
        /// Gets or sets the name of Player 2.
        /// </summary>
        public string Player2Name { get; set; }

        /// <summary>
        /// Gets or sets the device USB ID.
        /// </summary>
        public string DeviceUsbId { get; set; }

        /// <summary>
        /// Gets or sets the device USB port.
        /// </summary>
        public SerialPort DeviceUsbPort { get; set; }

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        public Random RandomNumberGenerator { get; set; } =
            new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Gets or sets a value indicating if a handshake has been successfully
        /// established with the USB device.
        /// </summary>
        public bool IsHandshakeEstablished { get; set; }

        /// <summary>
        /// Gets or sets the duel length.
        /// </summary>
        public double DuelLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if a duel is running.
        /// </summary>
        public bool IsDuelRunning { get; set; }

        /// <summary>
        /// Gets or sets a vale indicating if a duel has been won.
        /// </summary>
        public bool IsDuelWon { get; set; }

        /// <summary>
        /// Gets or sets the duel winner.
        /// </summary>
        public string DuelWinner { get; set; }

        /// <summary>
        /// Starts and runs the main game loop.
        /// </summary>
        public void Start()
        {
            this.SetPlayerNames();
            this.SetDeviceUsbId();
            try
            {
                this.ConnectUsb();
                this.InitiateHandshakeUsb();

                while (this.IsHandshakeEstablished is not true) { };

                while (this.RequestGameStart())
                {
                    WriteLine("Starting Game...");
                    this.DeviceUsbPort.WriteLine("rxn-duel:ready");
                    while (this.IsDuelRunning is not true) { };

                    this.DuelLength = this.CreateDuelLength();
                    DateTime duelStartTime = DateTime.Now;
                    while (this.GetDuelRunTime(duelStartTime) <= this.DuelLength) { };
                    this.DeviceUsbPort.WriteLine("rxn-duel:go");

                    while (this.IsDuelWon is not true) { };
                    WriteLine($"{this.DuelWinner} wins with a reaction of " +
                        $"{this.GetDuelWinTime(duelStartTime):0.##} seconds!");

                    this.ResetGame();
                }
            }
            finally
            {
                this.DisconnectUsb();
            }
        }

        /// <summary>
        /// Sets the player names.
        /// </summary>
        private void SetPlayerNames()
        {
            this.Player1Name =
                string.IsNullOrWhiteSpace(this.CommandValues.Player1Name)
                ? this.RequestInput("P1", "Player 1 Name")
                : this.CommandValues.Player1Name;

            this.Player2Name =
                string.IsNullOrWhiteSpace(this.CommandValues.Player2Name)
                ? this.RequestInput("P2", "Player 2 Name")
                : this.CommandValues.Player2Name;
        }

        /// <summary>
        /// Sets the device USB ID.
        /// </summary>
        private void SetDeviceUsbId()
        {
            while (string.IsNullOrWhiteSpace(this.DeviceUsbId))
            {
                this.DeviceUsbId =
                    string.IsNullOrWhiteSpace(this.CommandValues.DeviceId)
                    ? this.RequestInput(string.Empty, "Device USB ID")
                    : this.CommandValues.DeviceId;
            }
        }

        /// <summary>
        /// Configure and connect to the device via USB.
        /// </summary>
        private void ConnectUsb()
        {
            WriteLine($"Connecting to USB device {this.DeviceUsbId}...");
            this.DeviceUsbPort = new(this.DeviceUsbId, 115200);
            this.DeviceUsbPort.DataReceived += this.OnSerialDataReceived;

            if (this.DeviceUsbPort.IsOpen is not true)
            {
                this.DeviceUsbPort.Open();
                WriteLine("Connection established.");
            }
        }

        /// <summary>
        /// Initiates a handshake with the USB device for use with Reaction Duel.
        /// </summary>
        private void InitiateHandshakeUsb()
        {
            WriteLine($"Initiating handshake with USB device {this.DeviceUsbId}...");
            this.DeviceUsbPort.WriteLine("rxn-duel:handshake");
        }

        /// <summary>
        /// Request input from the user on whether to start a new game.
        /// </summary>
        /// <returns><c>true</c> if a new game should start, otherwise <c>false</c>.</returns>
        private bool RequestGameStart()
        {
            string playGameRequest = string.Empty;

            do
            {
                playGameRequest = this.RequestInput("Y", "Ready to play? (Y/N)");
            }
            while (playGameRequest.Trim().ToUpper() != "Y" &&
                playGameRequest.Trim().ToUpper() != "N");

            return playGameRequest.Trim().ToUpper() == "Y" ? true : false;
        }

        /// <summary>
        /// Create the duel length.
        /// </summary>
        /// <returns>The duel length in seconds.</returns>
        private double CreateDuelLength()
        {
            return Convert.ToDouble(this.RandomNumberGenerator.Next(1, 10));
        }

        /// <summary>
        /// Gets the current run time of the duel.
        /// </summary>
        /// <param name="duelStartTime">The duel start time.</param>
        /// <returns>The current run time of the duel in seconds.</returns>
        private double GetDuelRunTime(DateTime duelStartTime)
        {
            TimeSpan timeSinceStart = DateTime.Now - duelStartTime;
            return timeSinceStart.TotalSeconds;
        }

        /// <summary>
        /// Gets the winning time of the duel.
        /// </summary>
        /// <param name="duelStartTime">The duel start time.</param>
        /// <returns>The winning time of the duel in seconds.</returns>
        private double GetDuelWinTime(DateTime duelStartTime)
        {
            TimeSpan timeSinceStart = DateTime.Now - duelStartTime;
            return timeSinceStart.TotalSeconds - this.DuelLength;
        }

        /// <summary>
        /// Resets the game environment.
        /// </summary>
        private void ResetGame()
        {
            this.IsDuelRunning = false;
            this.IsDuelWon = false;
            this.DuelLength = default;
            this.DuelWinner = string.Empty;
        }

        /// <summary>
        /// Close USB connection.
        /// </summary>
        private void DisconnectUsb()
        {
            this.DeviceUsbPort.Close();
            this.DeviceUsbPort.Dispose();
        }

        /// <summary>
        /// Request input from the user or set a default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="prompt">The user prompt.</param>
        /// <returns>The input or default value.</returns>
        private string RequestInput(string defaultValue, string prompt)
        {
            Write($"{prompt} [{defaultValue}]: ");
            string input = ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }

            return input;
        }

        /// <summary>
        /// Handles received data via the USB port.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">Information about the event.</param>
        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            string receivedData = serialPort.ReadLine().Trim();

            if (receivedData == "rxn-duel:ack")
            {
                this.HandleHandshakeEstablished();
            }
            else if (receivedData == "rxn-duel:start")
            {
                this.HandleDuelStart();
            }
            else if (receivedData.StartsWith("rxn-duel:early-"))
            {
                string player = receivedData.Substring(15);
                this.HandleDuelEarlyPlayer(player);
            }
            else if (receivedData.StartsWith("rxn-duel:winner-"))
            {
                string player = receivedData.Substring(16);
                this.HandleDuelWinner(player);
            }
        }

        /// <summary>
        /// Handles a successful handshake with the USB device.
        /// </summary>
        private void HandleHandshakeEstablished()
        {
            WriteLine("Handshake successful.");
            this.IsHandshakeEstablished = true;
        }

        /// <summary>
        /// Handles the start of a duel.
        /// </summary>
        private void HandleDuelStart()
        {
            WriteLine("Duel started.");
            this.IsDuelRunning = true;
        }

        /// <summary>
        /// Handles a player declaring too early.
        /// </summary>
        /// <param name="player">The player.</param>
        private void HandleDuelEarlyPlayer(string player)
        {
            WriteLine($"Too early {player}, keep going!");
        }

        /// <summary>
        /// Handles a player winning.
        /// </summary>
        /// <param name="player">The player.</param>
        private void HandleDuelWinner(string player)
        {
            this.DuelWinner = player;
            this.IsDuelWon = true;
        }
    }
}
