namespace App.Api.Entities
{
    public record Group
    {
        public Guid Id { get; init; }
        public List<Member> Members { get; init; }
        public List<Transaction> Transactions { get; init; }
    }
}