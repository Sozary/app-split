namespace App.UnitTests
{
    public record TransactionTest
    {
        public string From { get; init; }
        public string To { get; init; }
        public float Amount { get; init; }
    }
}