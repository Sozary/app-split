using System.ComponentModel.DataAnnotations;

namespace App.Api.Entities
{
    public record Member
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
    }
}