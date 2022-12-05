using App.Api.Entities;
namespace App.Api.Repositories;
public interface IMemberRepository
{
    Member GetMember(Guid id);
    IEnumerable<Member> GetMembers();

    void CreateMember(Member member);
}