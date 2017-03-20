using System;
using System.Web.Mvc;
using AutomatedTellerMachine.Controllers;
using AutomatedTellerMachine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutomatedTellerMachine.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FooActionReturnsAboutView()
        {
            // ARRANGE 
            var homeController = new HomeController();

            // ACT
            var result = homeController.Foo() as ViewResult;

            //ASSERT
            Assert.AreEqual("About", result.ViewName);
        }

        [TestMethod]
        public void ContactFormSaysThanks()
        {
            //ARRANGE 
            var homeController = new HomeController();

            //ACT
            var result = homeController.Contact("I love your bank") as ViewResult;

            //ASSERT
            Assert.IsNotNull(result.ViewBag.TheMessage);
        }

        [TestMethod]
        public void BalanceIsCorrectAfterDeposit()
        {
            //ARRANGE

            //Fake database
            var fakeDb = new FakeApplicationDbContext();
            //Fake CheckingAccounts table
            fakeDb.CheckingAccounts = new FakeDbSet<CheckingAccount>();
            //Fake checkingAccount
            var checkingAccount = new CheckingAccount {AccountNumber = "000123TEST", Id = 1, Balance = 0};
            fakeDb.CheckingAccounts.Add(checkingAccount);
            //Fake Transactions table
            fakeDb.Transactions = new FakeDbSet<Transaction>();
            //Transaction controller
            var transactionController = new TransactionController(fakeDb);
            
            //ACT

            //Makeing a deposit
            transactionController.Deposit(new Transaction {CheckingAccountId = 1, Amount = 25});

            //Turning into GREEN
            //checkingAccount.Balance = 25;

            //ASSERT
            Assert.AreEqual(25, checkingAccount.Balance);
        }

    }
}
