using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class Deployment
    {
        [Display(Name = "ID")]
        public int StaffID { get; set; }

        [Display(Name = "Deployment Type")]
        [Required]
        public string DeploymentType { get; set; }

        [Display(Name = "MID")]
        [Required]
        [StringLength(15, ErrorMessage = "Cannot exceed 15 characters")]
        public int MID { get; set; }

        [Display(Name = "TID")]
        [Required]
        [StringLength(8, ErrorMessage = "Cannot exceed 8 characters")]
        public int TID { get; set; }

        [Display(Name = "Schemes")]
        [Required]
        public string Schemes { get; set; }

        [Display(Name = "Merchant Type")]
        [Required]
        public string MerchantType { get; set; }

        [Display(Name = "SGQR ID")]
        [StringLength(12, ErrorMessage ="Cannot exceed 12 characters")]
        public string SGQRID { get; set; }

        [Display(Name = "SGQR Version")]
        [StringLength(7, ErrorMessage = "Cannot exceed 7 characters")]
        public string SGQRVersion { get; set; }

        public string DeploymentPhoto { get; set; }
        public string PhotoBefore { get; set; }
        public string PhotoAfter { get; set; }

        public IFormFile fileToUpload { get; set; }

        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

    }
}

