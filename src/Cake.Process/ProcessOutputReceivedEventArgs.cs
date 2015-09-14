namespace Cake.Process
{
    /// <summary>
    ///     The event args that contains information about the line of text written to either
    ///     StandardOutputStream or StandardErrorStream on the created process.
    /// </summary>
    public class ProcessOutputReceivedEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessOutputReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="output">The Output.</param>
        public ProcessOutputReceivedEventArgs(string output)
        {
            Output = output;
        }

        /// <summary>
        ///     Gets the line of text written to StandardOutputStream / StandardErrorStream.
        /// </summary>
        /// <value>
        ///     The Output.
        /// </value>
        public string Output { get; private set; }
    }
}