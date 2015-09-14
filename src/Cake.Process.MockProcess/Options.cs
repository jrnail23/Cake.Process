using CommandLine;
using CommandLine.Text;

namespace Cake.Process.MockProcess
{
    public class Options
    {
        [Option('s', "sleep", DefaultValue = 0, HelpText = "Number of milliseconds to sleep for before exiting.")]
        public int Sleep { get; set; }

        [Option("echoArgs", DefaultValue = false, HelpText = "Immediately write args received to stdout.")]
        public bool EchoArgs { get; set; }

        [Option('l', "loadedMsg", DefaultValue = null, HelpText = "Message to write to stdout when processing begins.")]
        public string LoadedMessage { get; set; }

        [Option("throw", DefaultValue = null, HelpText = "Immediately throw an exception with the given error message.")]
        public string ThrowException { get; set; }

        [Option('e', "exitCode", DefaultValue = 0, HelpText = "Exit code to return upon program completion.")]
        public int ExitCode { get; set; }

        [Option('p', "pause", DefaultValue = false, HelpText = "Pause to wait for user input before exiting.")]
        public bool Pause { get; set; }

        [OptionArray('v', "environmentVariables", DefaultValue = new string[0],
            HelpText = "Keys of environment variables to be written to stdout.")]
        public string[] EnvironmentVariables { get; set; }

        [Option('d', "delay", DefaultValue = 0, HelpText = "Number of milliseconds to delay before executing (simulates startup time).")]
        public int Delay { get; set; }

        [OptionArray("out", DefaultValue = new string[0], HelpText = "Lines of content to write to stdout.")]
        public string[] StandardOutputToWrite { get; set; }

        [OptionArray("err", DefaultValue = new string[0], HelpText = "Lines of content to write to stderr.")]
        public string[] StandardErrorToWrite { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                AddDashesToOption = true,
                AdditionalNewLineAfterOption = true,
                Heading = new HeadingInfo(GetType().Assembly.GetName().Name)
            };

            help.AddOptions(this);
            return help;
        }
    }
}