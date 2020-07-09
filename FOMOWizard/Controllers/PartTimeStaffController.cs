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
    public class PartTimeStaffController : Controller
    {
        private StaffDAL staffContext = new StaffDAL();

        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Part-timer"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
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

        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Part-timer"))
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

                return RedirectToAction("Create", "PartTimeStaff");
            }
            else
            {
                return View(deployment);
            }
        }
    }
}