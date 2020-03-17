using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FOMOWizard.Models;
using Microsoft.Extensions.Configuration;

namespace FOMOWizard.DAL
{
    public class HomeDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public HomeDAL()
        {
            //Locate the appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            //Read ConnectionString from appsettings.json file
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "FOMOConnectionString");

            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public bool IsEmailExist(string LoginID, string password)
        {
            if (password != null)
            {
                SqlCommand cmd = new SqlCommand("SELECT StaffID FROM Staff WHERE Email=@LoginID", conn);
                cmd.Parameters.AddWithValue("@LoginID", LoginID);
                SqlDataAdapter daEmail = new SqlDataAdapter(cmd);
                DataSet result = new DataSet();
                conn.Open();
                daEmail.Fill(result, "StaffDetails");
                conn.Close();
                if (result.Tables["StaffDetails"].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }
    }
}
