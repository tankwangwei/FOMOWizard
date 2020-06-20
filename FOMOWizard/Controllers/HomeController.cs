using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FOMOWizard.DAL;
using FOMOWizard.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Diagnostics;

namespace FOMOWizard.Controllers
{
    public class HomeController : Controller
    {
        private HomeDAL homeContext = new HomeDAL();
        StaffDAL staffContext = new StaffDAL();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StaffLogin(IFormCollection formData)
        {   
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();

            Staff staff = staffContext.GetStaff(loginID);

            if (homeContext.IsEmailExist(loginID, password))
            {
                if (loginID == staff.Email.ToLower() && password == staff.Password)
                {
                    string name = homeContext.GetName(loginID);
                    string department = homeContext.GetDepartment(loginID);
                    string role = homeContext.GetRole(loginID);
                    
                    HttpContext.Session.SetString("LoginID", loginID);
                    HttpContext.Session.SetString("Name", name);
                    HttpContext.Session.SetString("Department", department);
                    HttpContext.Session.SetString("Role", role);

                    if (department == "Operations")
                    {
                        if (role == "Manager")
                        {
                            return RedirectToAction("Index", "OperationsManager");
                        }
                        else if (role == "Staff")
                        {
                            return RedirectToAction("Index", "OperationsStaff");
                        }
                        else if (role == "Part-timer")
                        {
                            return RedirectToAction("Index", "PartTimeStaff");
                        }
                    }
                }

                else
                {
                    TempData["Message"] = "Your credentials are incorrect!";
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }

            else
            {
                TempData["Message"] = "Invalid Login Credentials!";

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOut()
        {
            // Clear all key-values pairs stored in session state 
            HttpContext.Session.Clear();
            // Call the Index action of Home controller 
            return RedirectToAction("Index", "Home");
        }

        public ActionResult StaffMain()
        {
            return View();
        }
    }
}