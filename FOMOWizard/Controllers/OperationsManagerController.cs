using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FOMOWizard.DAL;
using FOMOWizard.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FOMOWizard.Controllers
{
    public class OperationsManagerController : Controller
    {
        private OperationsManagerDAL mngContext = new OperationsManagerDAL();
        private OperationsStaffDAL staffContext = new OperationsStaffDAL();

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

        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["DeploymentType"] = GetDeploymentType();
                ViewData["MerchantType"] = GetMerchantType();

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Deployment deployment)
        {
            if (ModelState.IsValid)
            {
                deployment.DeploymentID = staffContext.Add(deployment);

                return RedirectToAction("Create", "OperationsManager");
            }
            else
            {
                return View(deployment);
            }
        }

        public ActionResult Edit(int id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Deployment deployment = staffContext.GetDetails(id);
            if (deployment == null)
            {
                return RedirectToAction("Index");
            }
            return View(deployment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Deployment deployment)
        {
            if (ModelState.IsValid)
            {
                mngContext.Update(deployment);
                return RedirectToAction("Edit");
            }

            return View(deployment);
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

        public ActionResult ViewDeployment()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["DeploymentType"] = GetDeploymentType();
            ViewData["MerchantType"] = GetMerchantType();

            List<Deployment> deployments = staffContext.GetAllDeployment();

            return View(deployments);
        }

        private List<SelectListItem> GetDeploymentType()
        {
            List<SelectListItem> deploymenttype = new List<SelectListItem>();
            deploymenttype.Add(new SelectListItem { Value = "NewDeployment", Text = "New Deployment" });
            deploymenttype.Add(new SelectListItem { Value = "ReDeployment", Text = "Re-Deployment" });
            deploymenttype.Add(new SelectListItem { Value = "ReTraining", Text = "Re-Training" });

            return deploymenttype;
        }

        private List<SelectListItem> GetMerchantType()
        {
            List<SelectListItem> merchanttype = new List<SelectListItem>();
            merchanttype.Add(new SelectListItem { Value = "SGQRNotificationApp", Text = "SGQR Notification App" });
            merchanttype.Add(new SelectListItem { Value = "POSTerminal", Text = "POS Terminal" });
            merchanttype.Add(new SelectListItem { Value = "SGQRPOSTerminal", Text = "SGQR + POS Terminal" });

            return merchanttype;
        }

        //Needs to complete uploading of payload today
        public ActionResult UploadPayload()
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPayload(List<IFormFile> files)
        {
            var size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\FullPayload", formFile.FileName);
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return View();
        }

        public ActionResult ViewPayload()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Manager"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Payload> payloads = staffContext.SortPayloadByDesc();
            return View(payloads);
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

        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}