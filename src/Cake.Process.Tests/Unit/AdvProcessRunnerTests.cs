using Cake.Process.Tests.Utils;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace Cake.Process.Tests.Unit
{
    public sealed class AdvProcessRunnerTests
    {
        [Theory]
        [ProcessAutoData]
        public void Should_Throw_If_Any_Ctor_Args_Are_Null(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof (AdvProcessRunner).GetConstructors());
        }

        [Theory]
        [ProcessAutoData]
        public void Should_Throw_If_Any_Method_Args_Are_Null(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof (AdvProcessRunner).GetMethods());
        }
    }
}