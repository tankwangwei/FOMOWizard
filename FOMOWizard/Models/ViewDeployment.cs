using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.Models
{
    public class ViewDeployment
    {
        public string DeploymentID { get; set; }
        public string DeploymentType { get; set; }
        public string MID { get; set; }
        public string TID { get; set; }
        public string Schemes { get; set; }
        public string MerchantType { get; set; }
        public string SGQRID { get; set; }
        public string SGQRVer { get; set; }
        public string DeploymentPhoto { get; set; }
        public string PhotoBefore { get; set; }
        public string PhotoAfter { get; set; }

        public List<ViewDeployment> deploymentinfo { get; set; }
    }
}
