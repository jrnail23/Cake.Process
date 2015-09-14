using System;
using System.Collections.Generic;
using System.Globalization;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Process
{
    /// <summary>
    /// Contains functionality related to processes.
    /// </summary>
    [CakeAliasCategory("AdvProcess")]
    public static class AdvProcessAliases
    {
        /// <summary>
        /// Starts the process resource that is specified by the filename and settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The newly started process.</returns>
        /// <example>
        /// <code>
        /// using(var process = StartAdvProcess("ping", new AdvProcessSettings{ Arguments = "localhost" }))
        /// {
        ///     process.WaitForExit();
        ///     // This should output 0 as valid arguments supplied
        ///     Information("Exit code: {0}", process.GetExitCode());
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="context"/>, <paramref name="fileName"/>, or <paramref name="settings"/>  is null.</exception>
        [CakeMethodAlias]
        public static IAdvProcess StartAdvProcess(this ICakeContext context, FilePath fileName, AdvProcessSettings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return StartAdvProcess(context, fileName, settings, new AdvProcessRunner(context.Environment, context.Log));
        }

        internal static IAdvProcess StartAdvProcess(this ICakeContext context, FilePath fileName, AdvProcessSettings settings, IAdvProcessRunner processRunner )
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (processRunner == null)
            {
                throw new ArgumentNullException("processRunner");
            }


            // Get the working directory.
            var workingDirectory = settings.WorkingDirectory ?? context.Environment.WorkingDirectory;
            settings.WorkingDirectory = workingDirectory.MakeAbsolute(context.Environment);

            // Start the process.
            var process = processRunner.Start(fileName, settings);
            if (process == null)
            {
                throw new CakeException("Could not start process.");
            }

            return process;
        }

        /// <summary>
        /// Starts the process resource that is specified by the filename.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The newly started process.</returns>
        /// <example>
        /// <code>
        /// using(var process = StartAdvProcess("ping"))
        /// {
        ///     process.WaitForExit();
        ///     // This should output 0 as valid arguments supplied
        ///     Information("Exit code: {0}", process.GetExitCode());
        /// }
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static IAdvProcess StartAdvProcess(this ICakeContext context, FilePath fileName)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return StartAdvProcess(context, fileName,
                new AdvProcessRunner(context.Environment, context.Log));
        }

        internal static IAdvProcess StartAdvProcess(this ICakeContext context, FilePath fileName,
            IAdvProcessRunner processRunner)
        {
            return StartAdvProcess(context, fileName, new AdvProcessSettings(),
                processRunner);
        }
    }
}