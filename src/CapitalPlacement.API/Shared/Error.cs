namespace CapitalPlacement.API.Shared
{
    public sealed record Error(string Code, string Description, IEnumerable<string> Messages = null!)
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public IEnumerable<string> Messages { get; init; } = Messages ?? new List<string>();
    }
}
