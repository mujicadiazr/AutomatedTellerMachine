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
        
        private ApplicationDbContext db = new ApplicationDbContext();

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
                //service.UpdateBalace(ViewBag.CheckingAccountId);
                return RedirectToAction("Index", "Home");
            }
            return View();

        }
    }
}
