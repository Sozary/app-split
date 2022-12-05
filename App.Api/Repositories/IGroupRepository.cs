using App.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Repositories;
public interface IGroupRepository
{
    Group GetGroup(Guid id);
    IEnumerable<Group> GetGroups();
    IEnumerable<Transaction> GetTransactions(Group group);

    void CreateGroup(Group group);

    void AddMemberToGroup(Group group, Member member);
    void AddTransactionToGroup(Group group, Transaction transaction);
    ActionResult<IEnumerable<Transaction>> SettleDebts(Group group);
}