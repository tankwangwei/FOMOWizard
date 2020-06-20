using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FOMOWizard.DAL;
using FOMOWizard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FOMOWizard.Controllers
{
    public class OperationsManagerController : Controller
    {
        private OperationsManagerDAL mngContext = new OperationsManagerDAL();

        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ViewStaff()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                List<Staff> staffList = mngContext.GetAllStaff();
                return View(staffList);
            }
        }

        public ActionResult CreateStaff()
        {
            if ((HttpContext.Session.GetString("Role") == null) || 
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["TypesOfRoles"] = SetRoles();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStaff(Staff staff)
        {
            if (ValidateStaff(staff))
            {
                if (ModelState.IsValid)
                {
                    staff.StaffID = mngContext.CreateNewStaff(staff);
                    return RedirectToAction("CreateStaff", "OperationsManager");
                }
                else
                {
                    return View(staff);
                }
            }
            else
            {
                TempData["CreateStaffErrorMessage"] = "Existing email entered!";
                return RedirectToAction("CreateStaff");
            }
        }

        public ActionResult EditStaff(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Staff staff = mngContext.GetStaff(id.Value);
                if (staff == null)
                {
                    return RedirectToAction("Index");
                }
                ViewData["TypesOfRoles"] = SetRoles();
                TempData["StaffID"] = staff.StaffID;
                return View(staff);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStaff(Staff staff)
        {
            staff.StaffID = Convert.ToInt32(TempData["StaffID"]);
            mngContext.EditStaffDetails(staff);

            return RedirectToAction("Index");
        }

        public bool ValidateStaff(Staff staff)
        {
            List<Staff> staffList = mngContext.ViewStaff();
            foreach (var item in staffList)
            {
                if (staff.Email == item.Email)
                {
                    return false;
                }
            }
            return true;
        }

        private List<SelectListItem> SetRoles()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            roles.Add(new SelectListItem { Value = "Part-timer", Text = "Part-Timer" });
            roles.Add(new SelectListItem { Value = "Staff", Text = "Staff" });
            roles.Add(new SelectListItem { Value = "Manager", Text = "Manager" });

            return roles;
        }
    }
}