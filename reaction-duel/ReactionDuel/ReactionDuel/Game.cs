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
        /// Gets or sets a value indicating if a handshake has been successfully
        /// established with the USB device.
        /// </summary>
        public bool IsHandshakeEstablished { get; set; }

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
                    ? this.RequestInput("", "Device USB ID")
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

            WriteLine(receivedData);
        }

        /// <summary>
        /// Handles a successful handshake with the USB device.
        /// </summary>
        private void HandleHandshakeEstablished()
        {
            WriteLine("Handshake successful.");
            this.IsHandshakeEstablished = true;
        }
    }
}
