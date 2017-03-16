using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutomatedTellerMachine.Models;
using Microsoft.AspNet.Identity;

namespace AutomatedTellerMachine.Controllers
{
    [Authorize]
    public class CheckingAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

       // GET: CheckingAccount/Details/
        public ActionResult Details()
        {
            var userId = User.Identity.GetUserId();
            var checkingAccount = db.CheckingAccounts.First(q => q.ApplicationUserId == userId);

            return View(checkingAccount);
        }

        // GET: CheckingAccount/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsForAdmin(int id)
        {
            var checkingAccount = db.CheckingAccounts.Find(id);
            return View("Details", checkingAccount);
        }

        // GET: CheckingAccount/List 
        public ActionResult List()
        {
            return View(db.CheckingAccounts.ToList());
        }

        // GET: CheckingAccount/Create
        public ActionResult Create()
        {
            return View();
        }
        
    }
}

