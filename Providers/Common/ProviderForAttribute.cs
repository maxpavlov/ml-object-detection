namespace waoeml.Providers.Common
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ProviderForAttribute : System.Attribute
    {
        private readonly string name;

        public ProviderForAttribute(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }
    }
}