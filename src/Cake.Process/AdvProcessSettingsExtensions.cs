using System;
using Cake.Core.IO;

namespace Cake.Process
{
    /// <summary>
    /// Contains extension methods for <see cref="AdvProcessSettings" />.
    /// </summary>
    public static class AdvProcessSettingsExtensions
    {
        /// <summary>
        /// Sets the arguments for the process
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="arguments">The action used to set arguments.</param>
        /// <returns>The same <see cref="AdvProcessSettings"/> instance so that multiple calls can be chained.</returns>
        public static AdvProcessSettings WithArguments(this AdvProcessSettings settings, Action<ProcessArgumentBuilder> arguments)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("build");
            }

            if (settings.Arguments == null)
            {
                settings.Arguments = new ProcessArgumentBuilder();
            }

            arguments(settings.Arguments);
            return settings;
        }

        /// <summary>
        /// Sets the working directory for the process to be started.
        /// </summary>
        /// <param name="settings">The process settings.</param>
        /// <param name="path">The working directory for the process to be started.</param>
        /// <returns>The same <see cref="AdvProcessSettings"/> instance so that multiple calls can be chained.</returns>
        public static AdvProcessSettings UseWorkingDirectory(this AdvProcessSettings settings, DirectoryPath path)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            settings.WorkingDirectory = path;
            return settings;
        }

        /// <summary>
        /// Sets a value indicating whether the output of an application is written to the <see cref="P:System.Diagnostics.Process.StandardOutput"/> stream.
        /// </summary>
        /// <param name="settings">The process settings.</param>
        /// <param name="redirect">true if output should be written to <see cref="P:System.Diagnostics.Process.StandardOutput"/>; otherwise, false. The default is false.</param>
        /// <returns>The same <see cref="AdvProcessSettings"/> instance so that multiple calls can be chained.</returns>
        public static AdvProcessSettings SetRedirectStandardOutput(this AdvProcessSettings settings, bool redirect)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.RedirectStandardOutput = redirect;
            return settings;
        }

        /// <summary>
        /// Sets a value indicating whether the error output of an application is written to the <see cref="P:System.Diagnostics.Process.StandardError"/> stream.
        /// </summary>
        /// <param name="settings">The process settings.</param>
        /// <param name="redirect">true if error output should be written to <see cref="P:System.Diagnostics.Process.StandardError"/>; otherwise, false. The default is false.</param>
        /// <returns>The same <see cref="AdvProcessSettings"/> instance so that multiple calls can be chained.</returns>
        public static AdvProcessSettings SetRedirectStandardError(this AdvProcessSettings settings, bool redirect)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.RedirectStandardError = redirect;
            return settings;
        }

        /// <summary>
        /// Sets the optional timeout for process execution
        /// </summary>
        /// <param name="settings">The process settings.</param>
        /// <param name="timeout">The timeout duration</param>
        /// <returns>The same <see cref="AdvProcessSettings"/> instance so that multiple calls can be chained.</returns>
        public static AdvProcessSettings SetTimeout(this AdvProcessSettings settings, int timeout)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.Timeout = timeout;
            return settings;
        }

        /// <summary>
        /// Defines and sets the value of an environment variable that applies to this process and child processes 
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        /// <returns>The same <see cref="AdvProcessSettings"/> instance so that multiple calls can be chained.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="settings"/> or <paramref name="name"/> is null.</exception>
        public static AdvProcessSettings WithEnvironmentVariable(this AdvProcessSettings settings, string name, string value)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            settings.EnvironmentVariables[name] = value;

            return settings;
        }
    }
}