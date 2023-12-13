//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using InsuranceCore.Models;
//using InsuranceInfrastructure.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace InsuranceManagement.Controllers
//{
//    [TypeFilter(typeof(AuditFilterAttribute))]
//    public class UsersController : Controller
//    {
//        // GET: Users
//        public ActionResult Index()
//        {
//            return View();
//        }
//        [EncryptionAction]
//        // GET: Users/Details/5
//        public ActionResult DetailsUser(int id)
//        {
//            return View();
//        }

//        // GET: Users/Create
//        public ActionResult CreateUser()
//        {
//            return View();
//        }

//        // POST: Users/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult CreateUser(Users collection)
//        {
//            try
//            {
//                // TODO: Add insert logic here

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }
//        [EncryptionAction]
//        // GET: Users/Edit/5
//        public ActionResult EditUser(int id)
//        {
//            return View();
//        }

//        // POST: Users/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult EditUser(int id, Users collection)
//        {
//            try
//            {
//                // TODO: Add update logic here

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }
//        [EncryptionAction]
//        // GET: Users/Delete/5
//        public ActionResult DeleteUser(int id)
//        {
//            return View();
//        }

//        // POST: Users/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteUser(int id, Users collection)
//        {
//            try
//            {
//                // TODO: Add delete logic here

//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }
//    }
//}