
using App.Api.Entities;
namespace App.Api.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly List<Member> Members = new();

        void IMemberRepository.CreateMember(Member member)
        {
            Members.Add(member);
        }

        Member IMemberRepository.GetMember(Guid id)
        {
            return Members.Where(i => i.Id == id).SingleOrDefault();
        }

        IEnumerable<Member> IMemberRepository.GetMembers()
        {
            return Members;
        }
    }

}