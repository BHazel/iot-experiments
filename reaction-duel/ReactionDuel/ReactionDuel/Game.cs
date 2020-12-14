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
        /// Starts and runs the main game loop.
        /// </summary>
        public void Start()
        {
            this.SetPlayerNames();
            this.SetDeviceUsbId();
            try
            {
                this.ConnectUsb();
                ReadKey();
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
            if (string.IsNullOrWhiteSpace(this.CommandValues.Player1Name))
            {
                this.Player1Name =
                    this.RequestInput("P1", "Player 1 Name");
            }

            if (string.IsNullOrWhiteSpace(this.CommandValues.Player2Name))
            {
                this.Player2Name =
                    this.RequestInput("P2", "Player 2 Name");
            }
        }

        /// <summary>
        /// Sets the device USB ID.
        /// </summary>
        private void SetDeviceUsbId()
        {
            while (string.IsNullOrWhiteSpace(this.CommandValues.DeviceId) &&
                string.IsNullOrWhiteSpace(this.DeviceUsbId))
            {
                this.DeviceUsbId =
                    this.RequestInput("", "Device USB ID");
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
            string receivedData = serialPort.ReadLine();
            WriteLine(receivedData);
        }
    }
}
