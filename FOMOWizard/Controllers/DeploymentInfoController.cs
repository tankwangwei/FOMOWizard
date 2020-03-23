//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using System.Data.SqlClient;
//using System.Configuration;
//using FOMOWizard.Models;
//using FOMOWizard.DAL;
//using Microsoft.AspNetCore.Http;

//namespace FOMOWizard.Controllers
//{
//    public class DeploymentInfoController : Controller
//    {
//        private StaffDAL staffcontext = new StaffDAL();
//        public ActionResult Index()
//        {
//            if ((HttpContext.Session.GetString("Department") == null) ||
//                (HttpContext.Session.GetString("Department") != "Operation"))
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            List<Deployment> deploymentList = staffcontext.GetAllStaff();
//            return View(staffList);
//        }
//    }
//}