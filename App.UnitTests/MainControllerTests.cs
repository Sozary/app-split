using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using App.Api.Controllers;
using App.Api.Entities;
using App.Api.Repositories;
using Xunit;

namespace App.UnitTests;

public class MainControllerTests
{
    private readonly Mock<IGroupRepository> groupRepositoryStub = new();
    private readonly Mock<IMemberRepository> memberRepositoryStub = new();
    [Fact]
    public void GetGroup_WithNoGroupId_ReturnsNotFound()
    {
        // Arrange
        groupRepositoryStub.Setup(repo => repo.GetGroup(It.IsAny<Guid>())).Returns((Group)null);
        var controller = new MainController(groupRepositoryStub.Object, memberRepositoryStub.Object);

        // Act
        var result = controller.GetGroup(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    private List<Transaction> CreateSettleCase(MainController controller, List<String> names, List<TransactionTest> transactions)
    {
        List<Transaction> res = new();
        if (controller.CreateGroup().Value is Group group)
        {
            Dictionary<string, Member> members = new();
            foreach (var name in names)
            {
                var member = controller.CreateMember(name).Value;
                members.Add(name, member);
                controller.AddMemberToGroup(group.Id, member.Id);
            }
            foreach (var transaction in transactions)
            {
                controller.CreateTransaction(new()
                {
                    GroupId = group.Id,
                    From = members[transaction.From].Id,
                    To = members[transaction.To].Id,
                    Amount = transaction.Amount
                });
            }
            res = controller.SettleDebts(group.Id).Value.ToList();

            // Sort them so we can test it
            res.Sort(delegate (Transaction t1, Transaction t2)
            {
                return t1.Amount.CompareTo(t2.Amount);
            });

        }
        return res;
    }


    [Fact]
    public void SettleDebts_WithExample_ReturnsRightValues()
    {
        IGroupRepository groupRepo = new GroupRepository();
        IMemberRepository memberRepo = new MemberRepository();

        var controller = new MainController(groupRepo, memberRepo);
        var settleTransactions = CreateSettleCase(controller, new() { "Marie", "John", "Peter" }, new() {
                new (){
                    From="John",To="Marie",Amount=500f/3f
                },
                 new (){
                    From="John",To="Peter",Amount=500f/3f
                },
                 new (){
                    From="Marie",To="John",Amount=150f/3f
                },
                 new (){
                    From="Marie",To="Peter",Amount=150f/3f
                },
                 new (){
                    From="Peter",To="Marie",Amount=100f/3f
                },
                 new (){
                    From="Peter",To="John",Amount=100f/3f
                }
            });
        Assert.Equal(2, settleTransactions.Count);
        Assert.Equal("Marie", controller.GetMember(settleTransactions[0].From).Value.Name);
        Assert.Equal("John", controller.GetMember(settleTransactions[0].To).Value.Name);
        Assert.Equal(100, (int)settleTransactions[0].Amount);

        Assert.Equal("Peter", controller.GetMember(settleTransactions[1].From).Value.Name);
        Assert.Equal("John", controller.GetMember(settleTransactions[0].To).Value.Name);
        Assert.Equal(150, (int)settleTransactions[1].Amount);
    }
    [Fact]
    public void SettleDebts_WithTrivia_ReturnsRightValues()
    {
        IGroupRepository groupRepo = new GroupRepository();
        IMemberRepository memberRepo = new MemberRepository();

        var controller = new MainController(groupRepo, memberRepo);
        var settleTransactions = CreateSettleCase(controller, new() { "A", "B", "C" }, new() {
                new (){
                    From="A",To="B",Amount=10f
                },
                 new (){
                    From="B",To="A",Amount=1f
                },
                 new (){
                    From="B",To="C",Amount=5f
                },
                 new (){
                    From="C",To="A",Amount=5f
                }
            });
        Assert.Equal(1, settleTransactions.Count);
        Assert.Equal("B", controller.GetMember(settleTransactions[0].From).Value.Name);
        Assert.Equal("A", controller.GetMember(settleTransactions[0].To).Value.Name);
        Assert.Equal(4, (int)settleTransactions[0].Amount);
    }

    [Fact]
    public void SettleDebts_WithDifficult_ReturnsRightValues()
    {
        IGroupRepository groupRepo = new GroupRepository();
        IMemberRepository memberRepo = new MemberRepository();

        var controller = new MainController(groupRepo, memberRepo);
        var settleTransactions = CreateSettleCase(
            controller,
            Enumerable.Range(1, 17).ToList().ConvertAll<string>(x => x.ToString()),
             new() {
                new (){
                    From="2",To="17",Amount=27f
                },
                 new (){
                    From="3",To="6",Amount=31f
                },
                 new (){
                    From="4",To="10",Amount=47f
                },
                 new (){
                    From="5",To="15",Amount=57f
                }
                ,
                 new (){
                    From="6",To="13",Amount=33f
                }
                ,
                 new (){
                    From="7",To="9",Amount=5f
                }
                ,
                 new (){
                    From="8",To="1",Amount=41f
                }
                ,
                 new (){
                    From="9",To="1",Amount=4f
                }
                ,
                 new (){
                    From="9",To="13",Amount=23f
                }
                ,
                 new (){
                    From="11",To="13",Amount=75f
                }
                ,
                 new (){
                    From="12",To="7",Amount=28f
                }
                ,
                 new (){
                    From="16",To="7",Amount=3f
                }

            });
        Assert.Equal(13, settleTransactions.Count);
        Assert.Equal("10", controller.GetMember(settleTransactions[0].From).Value.Name);
        Assert.Equal("2", controller.GetMember(settleTransactions[0].To).Value.Name);
        Assert.Equal(1, (int)settleTransactions[0].Amount);

        Assert.Equal("13", controller.GetMember(settleTransactions[12].From).Value.Name);
        Assert.Equal("11", controller.GetMember(settleTransactions[12].To).Value.Name);
        Assert.Equal(75, (int)settleTransactions[12].Amount);



    }

}