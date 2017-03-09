using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutomatedTellerMachine.Models;

namespace AutomatedTellerMachine.Sevices
{
    public class CheckingAccountService
    {
        private ApplicationDbContext db;

        public CheckingAccountService(ApplicationDbContext dbContext)
        {
            this.db = dbContext;
        }

        public void CreateCheckingAccount(string firstName, string lastName, string userId, decimal initialBalanace)
        {
            var accountNumber = (123456 + db.CheckingAccounts.Count()).ToString().PadLeft(10, '0');
            var checkingAccount = new CheckingAccount
            {
                AccountNumber = accountNumber,
                ApplicationUserId = userId,
                Balance = initialBalanace,
                FirstName = firstName,
                LastName = lastName
            };

            db.CheckingAccounts.Add(checkingAccount);
            db.SaveChanges();
        }

        public void UpdateBalace(int checkingAccountId)
        {
            var checkingAccount = db.CheckingAccounts.First(c => c.Id == checkingAccountId);
            checkingAccount.Balance =
                db.Transactions.Where(t => t.CheckingAccountId == checkingAccountId).Sum(k => k.Amount);
            db.SaveChanges();
        }
    }
}