using System.ComponentModel.DataAnnotations;
namespace App.Api.Dtos
{
    public record TransactionDto
    {
        [Required]
        public Guid GroupId { get; init; }
        [Required]
        public Guid From { get; init; }
        [Required]
        public Guid To { get; init; }
        [Required]
        public float Amount { get; init; }
    }
}