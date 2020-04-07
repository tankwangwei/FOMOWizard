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

        public string GetName(string LoginID)
        {
            SqlCommand cmd = new SqlCommand("SELECT Name FROM Staff WHERE Email = @LoginID", conn);
            cmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter nameDA = new SqlDataAdapter(cmd);
            DataSet nameDS = new DataSet();
            conn.Open();
            nameDA.Fill(nameDS, "Name");
            conn.Close();
            Staff staff = new Staff();
            foreach (DataRow row in nameDS.Tables["Name"].Rows)
            {
                staff.Name = row["Name"].ToString();
            }

            return staff.Name.ToString();
        }

        public string GetDepartment(string LoginID)
        {
            SqlCommand cmd = new SqlCommand("SELECT Department FROM Staff WHERE Email = @LoginID", conn);
            cmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter departmentDA = new SqlDataAdapter(cmd);
            DataSet departmentDS = new DataSet();
            conn.Open();
            departmentDA.Fill(departmentDS, "Department");
            conn.Close();
            Staff staff = new Staff();
            foreach (DataRow row in departmentDS.Tables["Department"].Rows)
            {
                staff.Department = row["Department"].ToString();
            }

            return staff.Department.ToString();
            
        }
    }
}
