using System.Linq;

using App.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly List<Group> groups = new();
        private readonly List<Transaction> transactions = new();

        void IGroupRepository.AddMemberToGroup(Group group, Member member)
        {
            group.Members.Add(member);
        }

        void IGroupRepository.AddTransactionToGroup(Group group, Transaction transaction)
        {
            group.Transactions.Add(transaction);
        }

        void IGroupRepository.CreateGroup(Group group)
        {
            groups.Add(group);
        }

        Group IGroupRepository.GetGroup(Guid id)
        {
            return groups.Where(i => i.Id == id).SingleOrDefault();
        }

        IEnumerable<Group> IGroupRepository.GetGroups()
        {
            return groups;
        }
        IEnumerable<Transaction> IGroupRepository.GetTransactions(Group group)
        {
            return group.Transactions;
        }
        private Guid GetMax(Dictionary<Guid, float> scores)
        {
            var selectedMember = scores.Keys.First();
            foreach (KeyValuePair<Guid, float> pair in scores)
            {
                if (pair.Key != selectedMember)
                {
                    if (scores[pair.Key] > scores[selectedMember])
                    {
                        selectedMember = pair.Key;
                    }
                }
            }
            return selectedMember;
        }
        private Guid GetMin(Dictionary<Guid, float> scores)
        {
            var selectedMember = scores.Keys.First();
            foreach (KeyValuePair<Guid, float> pair in scores)
            {
                if (pair.Key != selectedMember)
                {
                    if (scores[pair.Key] < scores[selectedMember])
                    {
                        selectedMember = pair.Key;
                    }
                }
            }
            return selectedMember;
        }
        private void GreedySettleDebts(ref Dictionary<Guid, float> scores, ref List<Transaction> track)
        {
            Guid memberMaxCredit = GetMax(scores);
            Guid memberMaxDebit = GetMin(scores);

            if (scores[memberMaxDebit] == 0.0 && scores[memberMaxCredit] == 0.0)
            {
                return;
            }
            float minAmount = ((Func<float, float, float>)((x, y) => x < y ? x : y))(-scores[memberMaxDebit], scores[memberMaxCredit]);

            scores[memberMaxCredit] -= minAmount;
            scores[memberMaxDebit] += minAmount;

            track.Add(new()
            {
                Id = Guid.NewGuid(),
                From = memberMaxCredit,
                To = memberMaxDebit,
                Amount = minAmount
            });

            GreedySettleDebts(ref scores, ref track);

        }
        ActionResult<IEnumerable<Transaction>> IGroupRepository.SettleDebts(Group group)
        {
            Dictionary<Guid, float> scores = new();
            List<Transaction> track = new();

            foreach (Transaction transaction in group.Transactions)
            {
                if (!scores.ContainsKey(transaction.From))
                {
                    scores.Add(transaction.From, 0);
                }
                if (!scores.ContainsKey(transaction.To))
                {
                    scores.Add(transaction.To, 0);
                }
                scores[transaction.From] -= transaction.Amount;
                scores[transaction.To] += transaction.Amount;
            }

            GreedySettleDebts(ref scores, ref track);

            return track;
        }
    }
}