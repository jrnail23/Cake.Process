using System;
using System.Threading;
using CommandLine;

namespace Cake.Process.MockProcess
{
    /// <summary>
    ///     This console app acts as a mock for integration testing Cake's Process features.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Entry point for the console app.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static int Main(string[] args)
        {
            var options = new Options();

            if (!Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine(options.GetUsage());
                return 0;
            }

            if (options.EchoArgs)
            {
                Console.Out.WriteLine("Args received: {0}", string.Join(" ", args));
            }

            if (!string.IsNullOrWhiteSpace(options.ThrowException))
            {
                throw new ApplicationException(options.ThrowException);
            }

            Thread.Sleep(options.Delay);

            if (options.LoadedMessage != null)
            {
                Console.Out.WriteLine(options.LoadedMessage);
            }

            foreach (var key in options.EnvironmentVariables)
            {
                Console.Out.WriteLine("{0}: '{1}'", key, Environment.GetEnvironmentVariable(key));
            }

            foreach (var outContent in options.StandardOutputToWrite)
            {
                Console.Out.WriteLine(outContent);
            }

            foreach (var errContent in options.StandardErrorToWrite)
            {
                Console.Error.WriteLine(errContent);
            }

            Thread.Sleep(options.Sleep);

            if (options.Pause)
            {
                Console.ReadKey();
            }

            return options.ExitCode;
        }
    }
}