using System;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Process.Tests.Utils;
using FluentAssertions;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace Cake.Process.Tests.Unit
{
    public sealed class ProcessSettingsTests
    {
        public sealed class ExtensionMethods
        {
            [Theory]
            [ProcessAutoData]
            public void Methods_Should_Throw_When_Any_Args_Are_Null(GuardClauseAssertion assertion)
            {
                var methods =
                    typeof (AdvProcessSettingsExtensions).GetMethods(BindingFlags.Static |
                                                                     BindingFlags.Public).Where(m=>m.Name!="WithEnvironmentVariable");
                assertion.Verify(methods);
            }

            [Theory]
            [InlineData("Hello World", "Hello World")]
            [InlineData("", "")]
            [InlineData(" \t ", " \t ")]
            [InlineData(null, "")]
            public void Should_Return_Settings_With_Correct_Arguments(string value, string expected)
            {
                var settings = new AdvProcessSettings().WithArguments(args => { args.Append(value); });

                settings.Arguments.Render().Should().Be(expected);
            }

            [Theory]
            [InlineData("C:/Test.zip", "C:/Test.zip")]
            [InlineData("../Test.zip", "../Test.zip")]
            public void Should_Return_Settings_With_Correct_Directory(string value, string expected)
            {
                var settings = new AdvProcessSettings().UseWorkingDirectory(value);

                settings.WorkingDirectory.FullPath.Should().Be(expected);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, false)]
            public void Should_Return_Settings_With_Correct_Output(bool value, bool expected)
            {
                var settings = new AdvProcessSettings().SetRedirectStandardOutput(value);

                settings.RedirectStandardOutput.Should().Be(expected);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(false, false)]
            public void Should_Return_Settings_With_Correct_StandardError(bool value, bool expected)
            {
                var settings = new AdvProcessSettings().SetRedirectStandardError(value);

                settings.RedirectStandardError.Should().Be(expected);
            }

            [Theory]
            [InlineData(0, 0)]
            [InlineData(5000, 5000)]
            [InlineData(15000, 15000)]
            public void Should_Return_Settings_With_Correct_Timeout(int value, int expected)
            {
                var settings = new AdvProcessSettings().SetTimeout(value);

                settings.Timeout.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Settings_With_Correct_EnvironmentVariables()
            {
                var settings =
                    new AdvProcessSettings().WithEnvironmentVariable("Key1", "Value1")
                        .WithEnvironmentVariable("Key2", "Value2");

                settings.EnvironmentVariables["Key1"].Should().Be("Value1");
                settings.EnvironmentVariables["Key2"].Should().Be("Value2");
            }

            [Fact]
            public void EnvironmentVariable_Set_Multiple_Times_Should_Use_The_Last_One()
            {
                var settings =
                    new AdvProcessSettings().WithEnvironmentVariable("Key1", "Value1")
                        .WithEnvironmentVariable("Key1", "Value1-A");

                settings.EnvironmentVariables["Key1"].Should().Be("Value1-A");
            }

            [Fact]
            public void EnvironmentVariable_Key_Should_Not_Be_Null()
            {
                var sut = new AdvProcessSettings();
                sut.Invoking(_ => _.WithEnvironmentVariable(null, "Value1"))
                    .ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void EnvironmentVariables_Should_Be_Empty_By_Default()
            {
                var settings = new AdvProcessSettings();
                settings.EnvironmentVariables.Should().NotBeNull().And.BeEmpty();
            }
        }
    }
}
