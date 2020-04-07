using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FOMOWizard.DAL;
using FOMOWizard.Models;

namespace FOMOWizard.Controllers
{
    public class HomeController : Controller
    {
        private HomeDAL homeContext = new HomeDAL();
        StaffDAL staffContext = new StaffDAL();
        Staff staff = new Staff();

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

            Staff staff = staffContext.GetStaff(loginID);
            if (homeContext.IsEmailExist(loginID, password))
            {
                if (loginID == staff.Email.ToLower() && password == staff.Password)
                {
                    string name = homeContext.GetName(loginID);
                    string department = homeContext.GetDepartment(loginID);

                    // Store Login ID in session with the key as “LoginID” 
                    HttpContext.Session.SetString("LoginID", loginID);

                    // Store user role “Staff” in session with the key as “Role” 
                    HttpContext.Session.SetString("Role", "Staff");

                    //Store the Name of Staff in session
                    HttpContext.Session.SetString("Name", name);

                    //Store Staff's Department
                    HttpContext.Session.SetString("Department", department);

                    // Redirect user to the "StaffMain" view through an action 
                    return RedirectToAction("StaffMain");
                }

                else
                {
                    TempData["Message"] = "Invalid Login";
                    return RedirectToAction("Index");
                }

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