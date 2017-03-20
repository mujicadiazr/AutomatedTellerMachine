using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using AutomatedTellerMachine.Models;
using AutomatedTellerMachine.Sevices;
using Microsoft.AspNet.Identity;
using Transaction = AutomatedTellerMachine.Models.Transaction;

namespace AutomatedTellerMachine.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        
        private IApplicationDbContext db;

        public TransactionController()
        {
            db = new ApplicationDbContext();
        }

        public TransactionController(IApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: Transaction/Deposit
        public ActionResult Deposit(int checkingAccountId)
        {
            ViewBag.CheckingAccountId = checkingAccountId;
            return View();
        }

        // POST: Transaction/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                CheckingAccountService service = new CheckingAccountService(db);
                service.UpdateBalace(transaction.CheckingAccountId);
                return RedirectToAction("Index", "Home");
            }
            return View();

        }

        //GET: Transaction/Withdrawal
        public ActionResult Withdrawal(int checkingAccountId)
        {
            ViewBag.CheckingAccountId = checkingAccountId;
            return View();
        }

        //POST: Transaction/Withdrawal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdrawal(Transaction transaction)
        {
            var checkingAccount = db.CheckingAccounts.Find(transaction.CheckingAccountId);

            if (checkingAccount.Balance < transaction.Amount)
            {
                ModelState.AddModelError("Amount", "You have insufficient funds!");
            }

            if (ModelState.IsValid)
            {
                transaction.Amount = -transaction.Amount;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                CheckingAccountService service = new CheckingAccountService(db);
                service.UpdateBalace(transaction.CheckingAccountId);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        public ActionResult QuickCash(int checkingAccountId, int amount)
        {
            var checkingAccount = db.CheckingAccounts.Find(checkingAccountId);

            //Validating correct amount
            if (amount != 100)
            {
                ViewBag.ErrorMessage = "ERROR: Incorrect amount for this transaction!";
                return View("QuickCash");
            }
            //Validating sufficient funds
            if (checkingAccount.Balance < 100)
            {
                ViewBag.ErrorMessage = "ERROR: Insufficient funds for this transaction!";
                return View("QuickCash");
            }

            var transaction = new Transaction { Amount = -amount, CheckingAccountId = checkingAccountId };
            db.Transactions.Add(transaction);
            db.SaveChanges();

            var service = new CheckingAccountService(db);
            service.UpdateBalace(checkingAccountId);
            
            return RedirectToAction("Index","Home");
        }

        //GET: Transaction/Transfer
        public ActionResult Transfer(int checkingAccountId)
        {
            return View();
        }

        //POST: Transaction/Transfer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(TranferViewModel transfer)
        {
            var sourceCheckingAccount = db.CheckingAccounts.Find(transfer.CheckingAccountId);
            //Check for available funds
            if (sourceCheckingAccount.Balance < transfer.Amount)
            {
                ModelState.AddModelError("Amount", "Insufficient funds for this transfer");
            }

            var destinationCheckingAccount =
                db.CheckingAccounts.Where(c => c.AccountNumber == transfer.DestinationCheckingAccountNumber)
                    .FirstOrDefault();

            //Check for a valid destination account
            if (destinationCheckingAccount == null)
            {
                ModelState.AddModelError("DestinationCheckingAccountNumber", "Invalid destination checking account number");
            }
            
            if (ModelState.IsValid)
            {
                db.Transactions.Add(new Transaction {Amount = -transfer.Amount, CheckingAccountId = transfer.CheckingAccountId});
                db.Transactions.Add(new Transaction
                {
                    Amount = transfer.Amount,
                    CheckingAccountId = destinationCheckingAccount.Id
                });

                db.SaveChanges();

                var service = new CheckingAccountService(db);
                service.UpdateBalace(transfer.CheckingAccountId);
                service.UpdateBalace(destinationCheckingAccount.Id);

                return PartialView("_TranferResult", transfer);
            }

            return PartialView("_TransferForm");
        }
    }
}
