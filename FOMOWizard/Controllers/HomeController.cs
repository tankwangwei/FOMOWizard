using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FOMOWizard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StaffLogin(IFormCollection formData)
        {
            // Read inputs from textboxes // Email address converted to lowercase 
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();

            if (loginID == "abc@gmail.com" && password == "pass1234")
            {
                // Store Login ID in session with the key as “LoginID” 
                HttpContext.Session.SetString("LoginID", loginID);
                // Store user role “Staff” in session with the key as “Role” 
                HttpContext.Session.SetString("Role", "Staff");
                // Redirect user to the "StaffMain" view through an action 
                return RedirectToAction("StaffMain");
            }
            else
            {
                // Store an error message in TempData for display at the index view 
                TempData["Message"] = "Invalid Login Credentials!";
                // Redirect user back to the index view through an action 
                return RedirectToAction("Index");
            }
        }

        public ActionResult LogOut()
        {
            // Clear all key-values pairs stored in session state 
            HttpContext.Session.Clear();
            // Call the Index action of Home controller 
            return RedirectToAction("Index");
        }

        public ActionResult StaffMain()
        {
            return View();
        }
    }
}