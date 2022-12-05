using Microsoft.AspNetCore.Mvc;
using App.Api.Repositories;
using App.Api.Entities;
using App.Api.Dtos;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class MainController : ControllerBase
    {
        private readonly IGroupRepository groupRepository;
        private readonly IMemberRepository memberRepository;

        public MainController(IGroupRepository groupRepository, IMemberRepository memberRepository)
        {
            this.groupRepository = groupRepository;
            this.memberRepository = memberRepository;
        }
        private Guid AddMemberToGroupX(Group group, String name)
        {
            var guid = Guid.NewGuid();
            Member member = new()
            {
                Id = guid,
                Name = name
            };
            memberRepository.CreateMember(member);
            groupRepository.AddMemberToGroup(group, member);
            return guid;
        }
        private void CreateTransaction(Group group, Guid from, Guid to, float amount)
        {
            if (memberRepository.GetMember(from) is Member && memberRepository.GetMember(to) is Member)
            {
                Transaction transaction = new()
                {
                    Id = Guid.NewGuid(),
                    From = from,
                    To = to,
                    Amount = amount,
                };
                groupRepository.AddTransactionToGroup(group, transaction);
            }
        }

        [HttpGet("groups/{groupId}/settleDebts")]
        public ActionResult<IEnumerable<Transaction>> SettleDebts(Guid groupId)
        {
            if (groupRepository.GetGroup(groupId) is Group group)
            {
                return groupRepository.SettleDebts(group);
            }
            return NotFound();
        }
        [HttpGet("groups")]
        public IEnumerable<Group> GetGroups()
        {
            return groupRepository.GetGroups();
        }

        [HttpGet("members")]
        public IEnumerable<Member> GetMembers()
        {
            return memberRepository.GetMembers();
        }
        [HttpGet("groups/{groupId}/transactions")]
        public ActionResult<IEnumerable<Transaction>> GetTransactions(Guid groupId)
        {
            if (groupRepository.GetGroup(groupId) is Group group)
            {
                return Ok(groupRepository.GetTransactions(group));
            }
            return NotFound();
        }

        [HttpGet("groups/{id}")]
        public ActionResult<Group> GetGroup(Guid id)
        {
            var group = groupRepository.GetGroup(id);
            if (group is null)
            {
                return NotFound();
            }
            return Ok(group);
        }

        [HttpPost("members/{name}")]
        public ActionResult<Member> CreateMember(String name)
        {
            Member member = new()
            {
                Id = Guid.NewGuid(),
                Name = name
            };
            memberRepository.CreateMember(member);
            return member;
        }

        [HttpGet("members/{memberId}")]
        public ActionResult<Member> GetMember(Guid memberId)
        {

            if (memberRepository.GetMember(memberId) is Member member)
            {
                return member;
            }
            return NotFound();
        }

        [HttpPost("groups")]
        public ActionResult<Group> CreateGroup()
        {
            Group group = new()
            {
                Id = Guid.NewGuid(),
                Members = new(),
                Transactions = new()
            };
            groupRepository.CreateGroup(group);
            return group;
        }
        [HttpPost("groups/transaction")]
        public ActionResult<Transaction> CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            if (transactionDto is not null && transactionDto.Amount != 0)
            {
                if (groupRepository.GetGroup(transactionDto.GroupId) is Group group && memberRepository.GetMember(transactionDto.From) is Member && memberRepository.GetMember(transactionDto.To) is Member)
                {
                    Transaction transaction = new()
                    {
                        Id = Guid.NewGuid(),
                        From = transactionDto.From,
                        To = transactionDto.To,
                        Amount = (float)System.Math.Round(transactionDto.Amount, 2)
                    };
                    groupRepository.AddTransactionToGroup(group, transaction);
                    return Ok();
                }
                return NotFound();
            }
            Response.StatusCode = 400;
            return Content("Invalid amount");
        }

        [HttpPost("groups/{groupId}/member/{memberId}")]
        public ActionResult<Member> AddMemberToGroup(Guid groupId, Guid memberId)
        {
            if (groupRepository.GetGroup(groupId) is Group group && memberRepository.GetMember(memberId) is Member member)
            {
                groupRepository.AddMemberToGroup(group, member);
                return Ok();
            }
            return NotFound();
        }
    }
}