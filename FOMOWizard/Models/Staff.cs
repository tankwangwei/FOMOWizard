using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class Staff
    {
        [Display(Name = "ID")]
        public int StaffID { get; set; }

        [Display(Name = "Name")]
        [Required]
        [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
        public String Name { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        public String Email { get; set; }
        [Required(ErrorMessage = "Please input a valid email address")]

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Department")]
        [Required]
        public string Department { get; set; }

        [Display(Name = "Role")]
        [Required]
        public string Role { get; set; }


    }
}
