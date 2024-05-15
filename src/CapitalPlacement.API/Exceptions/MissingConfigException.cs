namespace CapitalPlacement.API.Exceptions
{
    public class MissingConfigException : Exception
    {
        public MissingConfigException(string nameOfConfig) : base($"The config: {nameOfConfig} is missing.")
        {

        }
    }
}
