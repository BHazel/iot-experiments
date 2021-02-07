namespace BWHazel.Apps.QuantumTelloport
{
    /// <summary>
    /// Values for the command-line arguments.
    /// </summary>
    public class CommandValues
    {
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