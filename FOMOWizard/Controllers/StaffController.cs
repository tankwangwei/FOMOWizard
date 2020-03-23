using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FOMOWizard.DAL;
using FOMOWizard.Models;
using System.IO;

namespace FOMOWizard.Controllers
{
    public class StaffController : Controller
    {
        private StaffDAL staffContext = new StaffDAL();

        // GET: Staff
        public ActionResult Index()
        {
            // Stop accessing the action if not logged in 
            // or account not in the "Staff" role 
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Staff"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Staff> staffList = staffContext.GetAllStaff();
            return View(staffList);
        }

        // GET: Staff/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Staff/Create
        public ActionResult Create()
        {
            // Stop accessing the action if not logged in 
            // or account not in the "Staff" role 
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Staff"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["DeploymentType"] = GetDeploymentType();
            ViewData["MerchantType"] = GetMerchantType();

            return View();
        }
        public ActionResult ViewDeployment()
        {
            // Stop accessing the action if not logged in 
            // or account not in the "Staff" role 
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Staff"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["DeploymentType"] = GetDeploymentType();
            ViewData["MerchantType"] = GetMerchantType();

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



        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Deployment deployment)
        {
            if (ModelState.IsValid)
            {
                //Add staff record to database 
                deployment.DeploymentID = staffContext.Add(deployment);
                //Redirect user to Staff/Index view 
                return RedirectToAction("Create");
            }
            else
            {
                //Input validation fails, return to the Create view //to display error message
                return View(deployment);
            }
        }

        // GET: Staff/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Staff/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Staff/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //public ActionResult UploadPhoto (int id)
        //{
        //    // Stop accessing the action if not logged in // or account not in the "Staff" role 
        //    if ((HttpContext.Session.GetString("Role") == null) || 
        //        (HttpContext.Session.GetString("Role") != "Staff"))
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    Staff staff = staffContext.GetStaff;
            
        //    return View();
        //}
        //[HttpPost] [ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadPhoto(StaffViewModel staffVM) {
        //    if (staffVM.fileToUpload != null && staffVM.fileToUpload.Length > 0)
        //    {
        //        try
        //        { // Find the filename extension of the file to be uploaded. 
        //            string fileExt = Path.GetExtension( staffVM.fileToUpload.FileName); // Rename the uploaded file with the staff’s name. 
        //            string uploadedFile = staffVM.Name + fileExt; // Get the complete path to the images folder in server 
        //            string savePath = Path.Combine( Directory.GetCurrentDirectory(), "wwwroot\\images", uploadedFile); // Upload the file to server 
        //            using (var fileSteam = new FileStream(savePath, FileMode.Create))
        //            {
        //                await staffVM.fileToUpload.CopyToAsync(fileSteam);
        //            }
        //            staffVM.Photo = uploadedFile; ViewData["Message"] = "File uploaded successfully.";
        //        }
        //        catch (IOException)
        //        { //File IO error, could be due to access rights denied 
        //            ViewData["Message"] = "File uploading fail!";
        //        }
        //        catch (Exception ex) //Other type of error 
        //        {
        //            ViewData["Message"] = ex.Message;
        //        }
        //    } return View(staffVM);
        //}
    }

}

