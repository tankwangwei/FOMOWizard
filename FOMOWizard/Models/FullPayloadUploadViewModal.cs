using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class FullPayloadUploadViewModal
    {
        [Display(Name = "Full Payload")]
        [NotMapped]
        public IFormFile FullPayload { get; set; }

        public string FullPayloadStorageName { get; set; }
    }
}
