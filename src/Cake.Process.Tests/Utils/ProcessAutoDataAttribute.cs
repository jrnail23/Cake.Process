using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;

namespace Cake.Process.Tests.Utils
{
    public class ProcessAutoDataAttribute : AutoDataAttribute
    {
        public ProcessAutoDataAttribute()
            : base(new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
        {
        }
    }
}