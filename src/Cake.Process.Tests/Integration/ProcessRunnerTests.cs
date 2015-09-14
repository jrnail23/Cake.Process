using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Process.MockProcess;
using Cake.Process.Tests.Fakes;
using FluentAssertions;
using Xunit;
using Path = System.IO.Path;

namespace Cake.Process.Tests.Integration
{
    public class ProcessRunnerTests
    {
        // this is one of the ugly parts -- where should we actually expect the MockProcess.exe to go, and should it be customizable?
        private static readonly FilePath _appExe =
            new FilePath(Path.Combine(Environment.CurrentDirectory, new FileInfo(typeof(Options).Assembly.Location).Name));

        [Fact, Trait("Category", "integration")]
        public void Process_Should_Return_Correct_Exit_Code()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings().WithArguments(args => args.Append("--exitcode 3"))
                    .UseWorkingDirectory(Environment.CurrentDirectory);


            using (var process = runner.Start(_appExe, settings))
            {
                process.WaitForExit();
                var exitCode = process.GetExitCode();
                Console.WriteLine(string.Join("\r\n", log.Messages));
                exitCode.Should().Be(3);
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Process_Should_Return_Correct_StandardOutput()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings().WithArguments(
                    args => args.Append("--out line1 line2 line3"))
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .SetRedirectStandardOutput(true);

            using (var process = runner.Start(_appExe, settings))
            {
                process.WaitForExit();
                var output = process.GetStandardOutput().ToArray();

                output.Should().BeEquivalentTo("line1", "line2", "line3");
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Process_Should_Return_Correct_StandardError()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings().WithArguments(
                    args => args.Append("--err \"error line1\" \"error line2\" \"error line3\""))
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .SetRedirectStandardError(true);

            using (var process = runner.Start(_appExe, settings))
            {
                process.WaitForExit();
                var output = process.GetStandardError().ToArray();

                output.Should().BeEquivalentTo("error line1", "error line2", "error line3");
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Process_Can_Be_Killed()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings()
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .WithArguments(args => args.Append("--sleep 5000"));

            using (var process = runner.Start(_appExe, settings))
            {
                process.Kill();

                process.HasExited.Should().BeTrue();
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Kill_Process_Returns_Minus1_ExitCode()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings()
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .WithArguments(args => args.Append("--sleep 5000 --exitcode 3"));

            using (var process = runner.Start(_appExe, settings))
            {
                process.Kill();
                process.GetExitCode().Should().Be(-1);
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Dispose_Does_Not_Kill_Underlying_Process_If_Still_Running()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings()
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .WithArguments(args => args.Append("--sleep 5000"));

            int processId;
            using (var process = runner.Start(_appExe, settings))
            {
                processId = process.ProcessId;
                process.HasExited.Should().BeFalse();
            }

            using (var p2 = System.Diagnostics.Process.GetProcessById(processId))
            {
                p2.HasExited.Should().BeFalse();
                p2.Kill();
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Process_Should_Use_Provided_EnvironmentVariables()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings()
                    .WithArguments(args => args.Append("--environmentVariables EnvVar1 EnvVar2"))
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .WithEnvironmentVariable("EnvVar1", "Value1")
                    .WithEnvironmentVariable("EnvVar2", "Value2")
                    .SetRedirectStandardOutput(true);

            using (var process = runner.Start(_appExe, settings))
            {
                process.WaitForExit();
                var output = process.GetStandardOutput().ToArray();

                output.Should().BeEquivalentTo("EnvVar1: 'Value1'", "EnvVar2: 'Value2'");
            }
        }

        [Fact, Trait("Category", "integration")]
        public void Process_WillNotMissOutputViaEvents()
        {
            var environment = FakeEnvironment.CreateWindowsEnvironment();
            var log = new FakeLog();
            var runner = new AdvProcessRunner(environment, log);

            var settings =
                new AdvProcessSettings()
                    .WithArguments(args => args.Append("--out line1 line2 --echoArgs --delay 100"))
                    .UseWorkingDirectory(Environment.CurrentDirectory)
                    .SetRedirectStandardOutput(true);

            var outputDataReceived = new List<string>();

            using (var process = runner.Start(_appExe, settings))
            {
                process.OutputDataReceived +=
                    (sender, args) => { outputDataReceived.Add(args.Output); };

                //process.Start();

                process.WaitForExit();

                outputDataReceived.Should()
                    .BeEquivalentTo("Args received: --out line1 line2 --echoArgs --delay 100",
                        "line1", "line2");
            }
        }

        public class TheExitedEvent
        {
            [Fact, Trait("Category", "integration")]
            public void Process_Raises_Exited_Event_With_ExitCode()
            {
                var environment = FakeEnvironment.CreateWindowsEnvironment();
                var log = new FakeLog();
                var runner = new AdvProcessRunner(environment, log);

                var settings =
                    new AdvProcessSettings().WithArguments(args => args.Append("--exitcode 3"))
                        .UseWorkingDirectory(Environment.CurrentDirectory);

                var exitCodeUponExit = 0;

                using (var process = runner.Start(_appExe, settings))
                {
                    process.Exited += (sender, args) => { exitCodeUponExit = args.ExitCode; };

                    process.WaitForExit();

                    exitCodeUponExit.Should().Be(3);
                }
            }

            [Fact, Trait("Category", "integration")]
            public void Process_Should_Capture_Exited_Event_Added_Before_Start()
            {
                var environment = FakeEnvironment.CreateWindowsEnvironment();
                var log = new FakeLog();
                var runner = new AdvProcessRunner(environment, log);

                var settings =
                    new AdvProcessSettings()
                        .WithArguments(args => args.Append("--exitCode 5"))
                        .UseWorkingDirectory(Environment.CurrentDirectory);

                using (var process = runner.Start(_appExe, settings))
                {
                    var exitedEventWasCaptured = false;

                    process.Exited += (sender, args) => { exitedEventWasCaptured = true; };

                    // process.Start();

                    process.WaitForExit();

                    exitedEventWasCaptured.Should().BeTrue();
                }
            }
        }

        public class TheErrorDataReceivedEvent
        {
            [Fact, Trait("Category", "integration")]
            public void Process_With_Error_Not_Redirected_Will_Throw()
            {
                var environment = FakeEnvironment.CreateWindowsEnvironment();
                var log = new FakeLog();
                var runner = new AdvProcessRunner(environment, log);

                var settings =
                    new AdvProcessSettings()
                        .WithArguments(args => args.Append("--err errorLine1"))
                        .UseWorkingDirectory(Environment.CurrentDirectory)
                        .SetRedirectStandardError(false);

                using (var process = runner.Start(_appExe, settings))
                {
                    process.Invoking(sut => sut.ErrorDataReceived += (sender, args) => { })
                        .ShouldThrow<InvalidOperationException>();
                }
            }

            [Fact, Trait("Category", "integration")]
            public void Process_Raises_ErrorDataReceived_Event()
            {
                var environment = FakeEnvironment.CreateWindowsEnvironment();
                var log = new FakeLog();
                var runner = new AdvProcessRunner(environment, log);

                var settings =
                    new AdvProcessSettings()
                        .WithArguments(args => args.Append("--err errorLine1"))
                        .UseWorkingDirectory(Environment.CurrentDirectory)
                        .SetRedirectStandardError(true);

                string errorDataReceived = null;

                using (var process = runner.Start(_appExe, settings))
                {
                    process.ErrorDataReceived +=
                        (sender, args) => { errorDataReceived = args.Output; };

                    process.WaitForExit();

                    errorDataReceived.Should().Be("errorLine1");
                }
            }
        }

        public class TheOutputDataReceivedEvent
        {
            [Fact, Trait("Category", "integration")]
            public void Process_With_Output_Not_Redirected_Will_Throw()
            {
                var environment = FakeEnvironment.CreateWindowsEnvironment();
                var log = new FakeLog();
                var runner = new AdvProcessRunner(environment, log);

                var settings =
                    new AdvProcessSettings()
                        .WithArguments(args => args.Append("--out OutputLine1"))
                        .UseWorkingDirectory(Environment.CurrentDirectory)
                        .SetRedirectStandardOutput(false);

                using (var process = runner.Start(_appExe, settings))
                {
                    process.Invoking(sut => sut.OutputDataReceived += (sender, args) => { })
                        .ShouldThrow<InvalidOperationException>();
                }
            }

            [Fact, Trait("Category", "integration")]
            public void Process_Raises_OutputDataReceived_Event()
            {
                var environment = FakeEnvironment.CreateWindowsEnvironment();
                var log = new FakeLog();
                var runner = new AdvProcessRunner(environment, log);

                var settings =
                    new AdvProcessSettings()
                        .WithArguments(args => args.Append("--out OutputLine1"))
                        .UseWorkingDirectory(Environment.CurrentDirectory)
                        .SetRedirectStandardOutput(true);

                string outputDataReceived = null;

                using (var process = runner.Start(_appExe, settings))
                {
                    process.OutputDataReceived +=
                        (sender, args) => { outputDataReceived = args.Output; };

                    process.WaitForExit();

                    outputDataReceived.Should().Be("OutputLine1");
                }
            }
        }
    }
}