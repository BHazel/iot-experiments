namespace BWHazel.Apps.QuantumTelloport
{
    /// <summary>
    /// Values for the command-line arguments.
    /// </summary>
    public class CommandValues
    {
        /// <summary>
        /// The default total run-time.
        /// </summary>
        public static int DefaultTotalRunTime = 120;
        
        /// <summary>
        /// The default pause time.
        /// </summary>
        public static int DefaultPauseTime = 10;
        
        /// <summary>
        /// The default Tello API version.
        /// </summary>
        public static string DefaultTelloApiVersion = "1.0.0.3";

        /// <summary>
        /// Gets or sets the total run-time in seconds.
        /// </summary>
        public int TotalRunTime { get; set; }

        /// <summary>
        /// Gets or sets the pause time between commands in seconds.
        /// </summary>
        public int PauseTime { get; set; }

        /// <summary>
        /// Gets or sets the Tello API version.
        /// </summary>
        public string TelloApiVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the drone simulator should be used.
        /// </summary>
        /// <remarks>The drone simulator should be run separately.</remarks>
        public bool UseSimulator { get; set; }
    }
}