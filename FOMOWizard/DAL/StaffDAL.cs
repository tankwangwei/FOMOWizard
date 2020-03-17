using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.DAL
{
    public class StaffDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public StaffDAL()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            //Read ConnectionString from appsettings.json file 
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("NPBookConnectionString");
            //Instantiate a SqlConnection object with the Connection String read. 
            conn = new SqlConnection(strConn);
        }
    }
}
