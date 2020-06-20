using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class Payload
    {
        public int ID { get; set; }
        public string UEN { get; set; }
        public string RegisteredName { get; set; }
        public string MID { get; set; }
        public string TID { get; set; }
        public string DateAdded { get; set; }
    }
}
