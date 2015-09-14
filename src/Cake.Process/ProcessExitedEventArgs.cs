using System;
using Cake.Core.IO;

namespace Cake.Process
{
    /// <summary>
    ///     Contains args from the Exited event of <see cref="IProcess" />>
    /// </summary>
    public class ProcessExitedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessExitedEventArgs" /> class.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public ProcessExitedEventArgs(int exitCode)
        {
            ExitCode = exitCode;
        }

        /// <summary>
        ///     Gets the exit code of the exited process.
        /// </summary>
        /// <value>
        ///     The exit code.
        /// </value>
        public int ExitCode { get; private set; }
    }
}