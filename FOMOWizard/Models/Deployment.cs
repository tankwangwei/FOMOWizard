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
        public int DeploymentID { get; set; }

        [Display(Name = "Deployment Type")]
        [Required]
        public string DeploymentType { get; set; }

        [Display(Name = "MID")]
        [Required]
        [RegularExpression("^[0-9]{15}$", ErrorMessage = "Please enter only 15 numbers")]
        public string MID { get; set; }

        [Display(Name = "TID")]
        [Required]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "Please enter 8 numbers only")]

        public string TID { get; set; }

        [Display(Name = "Schemes")]
        [Required(ErrorMessage ="Please select at least one of the scheme")]
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

        public virtual IFormFile ImageFile { get; set; }

        public string ImageStorageName { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

    }
}

