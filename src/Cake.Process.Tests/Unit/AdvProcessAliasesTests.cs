using System;
using System.Reflection;
using Cake.Core;
using Cake.Core.IO;
using Cake.Process.Tests.Utils;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Cake.Process.Tests.Unit
{
    public sealed class AdvProcessAliasesTests
    {
        [Theory]
        [ProcessAutoData]
        public void Methods_Should_Throw_If_Any_Args_Are_Null(GuardClauseAssertion assertion)
        {
            assertion.Verify(
                typeof (AdvProcessAliases).GetMethods(BindingFlags.Static | BindingFlags.Public |
                                                      BindingFlags.NonPublic));
        }

        public sealed class TheStartAdvProcessMethod
        {
            public sealed class WithoutSettings
            {
                [Theory]
                [ProcessAutoData]
                public void Should_Use_Environments_Working_Directory(ICakeContext context,
                    IAdvProcessRunner runner)
                {
                    // Arrange
                    context.Environment.WorkingDirectory.Returns(new DirectoryPath("/Working"));

                    Action sut = () => context.StartAdvProcess("hello.exe", runner);

                    // Act
                    sut();

                    // Assert
                    runner.Received(1).Start(Arg.Any<FilePath>(), Arg.Is<AdvProcessSettings>(info =>
                        info.WorkingDirectory.FullPath == "/Working"));
                }

                [Theory]
                [ProcessAutoData]
                public void Should_Throw_If_No_Process_Was_Returned_From_Process_Runner(
                    ICakeContext context, IAdvProcessRunner runner)
                {
                    // Arrange
                    runner.Start(Arg.Any<FilePath>(), Arg.Any<AdvProcessSettings>())
                        .Returns((IAdvProcess) null);

                    Action sut = () => context.StartAdvProcess("hello.exe", runner);

                    // Act / Assert
                    sut.ShouldThrow<CakeException>().WithMessage("Could not start process.");
                }

                [Theory]
                [ProcessAutoData]
                public void Should_Return_Process_Created_By_Runner(ICakeContext context,
                    FilePath filePath, IAdvProcessRunner runner,
                    IAdvProcess expectedResult)
                {
                    runner.Start(filePath, Arg.Any<AdvProcessSettings>()).Returns(expectedResult);

                    Func<IAdvProcess> sut =
                        () => context.StartAdvProcess(filePath, runner);

                    var result = sut();

                    result.Should().BeSameAs(expectedResult);
                }
            }

            public sealed class WithSettings
            {
                [Theory]
                [ProcessAutoData]
                public void Should_Return_Process_Created_By_Runner(ICakeContext context,
                    FilePath filePath, AdvProcessSettings settings, IAdvProcessRunner runner,
                    IAdvProcess expectedResult)
                {
                    runner.Start(filePath, settings).Returns(expectedResult);

                    Func<IAdvProcess> sut =
                        () => context.StartAdvProcess(filePath, settings, runner);

                    var result = sut();

                    result.Should().BeSameAs(expectedResult);
                }

                [Theory]
                [ProcessAutoData]
                public void
                    Should_Use_Environments_Working_Directory_If_Working_Directory_Is_Not_Set(
                    ICakeContext context, IAdvProcessRunner runner)
                {
                    // Arrange
                    context.Environment.WorkingDirectory.Returns(new DirectoryPath("/Working"));

                    Action sut =
                        () => context.StartAdvProcess("hello.exe", new AdvProcessSettings(), runner);

                    // Act
                    sut();

                    // Assert
                    runner.Received(1).Start(Arg.Any<FilePath>(), Arg.Is<AdvProcessSettings>(info =>
                        info.WorkingDirectory.FullPath == "/Working"));
                }

                [Theory]
                [ProcessAutoData]
                public void Should_Use_Provided_Working_Directory_If_Set(ICakeContext context,
                    [NoAutoProperties] AdvProcessSettings settings, IAdvProcessRunner runner)
                {
                    // Arrange
                    settings.WorkingDirectory = "/OtherWorking";

                    Action sut =
                        () => context.StartAdvProcess("hello.exe", settings, runner);

                    // Act
                    sut();

                    // Assert
                    runner.Received(1).Start(
                        Arg.Any<FilePath>(),
                        Arg.Is<AdvProcessSettings>(info =>
                            info.WorkingDirectory.FullPath == "/OtherWorking"));
                }

                [Theory, ProcessAutoData]
                public void Should_Make_Working_Directory_Absolute_If_Set_To_Relative(
                    ICakeContext context,
                    [NoAutoProperties] AdvProcessSettings settings, IAdvProcessRunner runner)
                {
                    // Arrange
                    context.Environment.WorkingDirectory.Returns("/Working");
                    settings.WorkingDirectory = "OtherWorking";

                    Action sut =
                        () => context.StartAdvProcess("hello.exe", settings, runner);

                    // Act
                    sut();

                    // Assert
                    runner.Received(1).Start(
                        Arg.Any<FilePath>(),
                        Arg.Is<AdvProcessSettings>(info =>
                            info.WorkingDirectory.FullPath == "/Working/OtherWorking"));
                }

                [Theory]
                [ProcessAutoData]
                public void Should_Throw_If_No_Process_Was_Returned_From_Process_Runner(
                    ICakeContext context, IAdvProcessRunner runner)
                {
                    // Arrange
                    runner.Start(Arg.Any<FilePath>(), Arg.Any<AdvProcessSettings>())
                        .Returns((IAdvProcess) null);

                    Action sut =
                        () => context.StartAdvProcess("hello.exe", new AdvProcessSettings(), runner);

                    // Act / Assert
                    sut.ShouldThrow<CakeException>().WithMessage("Could not start process.");
                }
            }
        }
    }
}