﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class FullPayloadUploadViewModal
    {
        public IFormFile FullPayload { get; set; }
    }
}
