namespace App.Api.Entities
{
    public record Transaction
    {
        public Guid Id { get; init; }
        public Guid From { get; init; }
        public Guid To { get; init; }
        public float Amount { get; init; }
    }
}