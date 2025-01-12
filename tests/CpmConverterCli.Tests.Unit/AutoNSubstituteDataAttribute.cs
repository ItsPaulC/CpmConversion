using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;

public class AutoNSubstituteDataAttribute : AutoDataAttribute
{
    internal class AutoPopulatedNSubstitutePropertiesCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.ResidueCollectors.Add(
                new Postprocessor(
                    new NSubstituteBuilder(
                        new AutoFixture.Kernel.MethodInvoker(
                            new NSubstituteMethodQuery())),
                    new AutoPropertiesCommand(
                        new PropertiesOnlySpecification())));
        }
    }

    private class PropertiesOnlySpecification : IRequestSpecification
    {
        public bool IsSatisfiedBy(object request)
        {
            return request is PropertyInfo;
        }
    }
}