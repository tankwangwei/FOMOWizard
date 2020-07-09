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
    public class OperationsStaffController : Controller
    {
        private StaffDAL staffContext = new StaffDAL();
        private OperationsStaffDAL opsStaffContext = new OperationsStaffDAL();

        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Staff"))
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
                (HttpContext.Session.GetString("Role") != "Staff"))
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

                return RedirectToAction("Create", "OperationsStaff");
            }
            else
            {
                return View(deployment);
            }
        }
        
        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") == "Part-timer")
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

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Deployment deployment)
        {
            if (ModelState.IsValid)
            {
                opsStaffContext.Update(deployment);
                return RedirectToAction("Edit");
            }
            
            return View(deployment);
        }

        public ActionResult ViewDeployment()
        {
            if (HttpContext.Session.GetString("Role") == "Part-timer")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["DeploymentType"] = GetDeploymentType();
            ViewData["MerchantType"] = GetMerchantType();

            List<Deployment> deployments = staffContext.GetAllDeployment();

            return View(deployments);
        }

        public ActionResult ViewPayload()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Staff"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Payload> payloads = opsStaffContext.SortPayloadByDesc();
            return View(payloads);
        }

        public ActionResult FindLocation()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Staff"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
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
    }
}