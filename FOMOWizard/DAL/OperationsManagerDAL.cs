using FOMOWizard.Models;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.DAL
{
    public class OperationsManagerDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public OperationsManagerDAL()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            //Read ConnectionString from appsettings.json file
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("FOMOConnectionString");

            //Instantiate a SqlConnection object with the Connection String read. 
            conn = new SqlConnection(strConn);

        }

        //Upload Full Payload to Google Cloud Storage
        public void UploadFile(string bucketName, string localPath, string objectName = null)
        {
            var storage = StorageClient.Create();
            using (var f = File.OpenRead(localPath))
            {
                objectName = objectName ?? Path.GetFileName(localPath);
                storage.UploadObject(bucketName, objectName, null, f);
            }
        }

        public List<Staff> GetAllStaff()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Staff WHERE Department = 'Operations' ORDER BY StaffID ", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();

            conn.Open();
            da.Fill(result, "StaffDetails");
            conn.Close();

            List<Staff> staffList = new List<Staff>();
            foreach (DataRow row in result.Tables["StaffDetails"].Rows)
            {
                staffList.Add(
                    new Staff
                    {
                        StaffID = Convert.ToInt32(row["StaffID"]),
                        Name = row["Name"].ToString(),
                        Email = row["Email"].ToString(),
                        Password = row["Password"].ToString(),
                        Department = row["Department"].ToString(),
                        Role = row["Role"].ToString()
                    });
            }
            return staffList;
        }

        public int CreateNewStaff(Staff staff)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Staff(Name, Email, Password, Department, Role)" +
                "OUTPUT INSERTED.StaffID VALUES" +
                "(@name, @email, @password, @department, @role)", conn);

            cmd.Parameters.AddWithValue("@name", staff.Name);
            cmd.Parameters.AddWithValue("@email", staff.Email);
            cmd.Parameters.AddWithValue("@password", staff.Password);
            cmd.Parameters.AddWithValue("@department", staff.Department);
            cmd.Parameters.AddWithValue("@role", staff.Role);

            conn.Open();
            staff.StaffID = (int)cmd.ExecuteScalar();
            conn.Close();

            return (staff.StaffID);
        }

        //Email method needs to change
        public bool CheckEmailExists(string LoginID)
        {
            SqlCommand cmd = new SqlCommand("SELECT StaffID FROM Staff WHERE Email=@LoginID", conn);
            cmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();

            conn.Open();
            da.Fill(result, "ExistingEmail");
            conn.Close();
            if (result.Tables["ExistingEmail"].Rows.Count > 0)
                return true;
            else
                return false;
        }


        public List<Staff> ViewStaff()
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Staff ORDER BY StaffID", conn);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "staffDetails");
            conn.Close();

            List<Staff> staffList = new List<Staff>();
            foreach (DataRow row in result.Tables["staffDetails"].Rows)
            {
                Staff staff = new Staff
                {
                    StaffID = Convert.ToInt32(row["StaffID"]),
                    Name = row["Name"].ToString(),
                    Email = row["Email"].ToString(),
                    Password = row["Password"].ToString(),
                    Department = row["Department"].ToString(),
                    Role = row["Role"].ToString()
                };

                staffList.Add(staff);
            }
            return staffList;
        }

        public int EditStaffDetails(Staff staff)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE Staff SET Name=@name, Email=@email, Password=@password, Role=@role " +
                "WHERE StaffID=@selectedStaffID", conn);
            cmd.Parameters.AddWithValue("@name", staff.Name);
            cmd.Parameters.AddWithValue("@email", staff.Email);
            cmd.Parameters.AddWithValue("@password", staff.Password);
            cmd.Parameters.AddWithValue("@role", staff.Role);
            cmd.Parameters.AddWithValue("@selectedStaffID", staff.StaffID);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();

            return count;
        }

        public Staff GetStaff(int staffID)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Staff WHERE StaffID = @staffID", conn);
            cmd.Parameters.AddWithValue("@staffID", staffID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();

            conn.Open();
            da.Fill(result, "SpecificStaffDetails");
            conn.Close();

            Staff staff = new Staff();
            if(result.Tables["SpecificStaffDetails"].Rows.Count > 0)
            {
                staff.StaffID = staffID;

                DataTable table = result.Tables["SpecificStaffDetails"];
                if (!DBNull.Value.Equals(table.Rows[0]["Name"]))
                {
                    staff.Name = table.Rows[0]["Name"].ToString();
                }
                if (!DBNull.Value.Equals(table.Rows[0]["Email"]))
                {
                    staff.Email = table.Rows[0]["Email"].ToString();
                }
                if (!DBNull.Value.Equals(table.Rows[0]["Password"]))
                {
                    staff.Password = table.Rows[0]["Password"].ToString();
                }
                if (!DBNull.Value.Equals(table.Rows[0]["Role"]))
                {
                    staff.Role = table.Rows[0]["Role"].ToString();
                }
                return staff;
            }
            else
            {
                return null;
            }
        }

        public int Update(Deployment deployment)
        {
            //Instantiate a SqlCommand object, supply it with SQL statement UPDATE //and the connection object for connecting to the database. 
            SqlCommand cmd = new SqlCommand("UPDATE Deployment SET MID=@mid, TID=@tid, SGQRID=@sgqrid, SGQRVer=@sgqrver" + " WHERE DeploymentID = @selectedDeploymentID", conn);

            //Define the parameters used in SQL statement, value for each parameter //is retrieved from the respective property of “staff” object. 
            cmd.Parameters.AddWithValue("@mid", deployment.MID);
            cmd.Parameters.AddWithValue("@tid", deployment.TID);
            cmd.Parameters.AddWithValue("@sgqrid", deployment.SGQRID);
            cmd.Parameters.AddWithValue("@sgqrver", deployment.SGQRVersion);

            cmd.Parameters.AddWithValue("@selectedDeploymentID", deployment.DeploymentID);

            //Open a database connection. 
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE 
            int count = cmd.ExecuteNonQuery();
            //Close the database connection. 
            conn.Close();

            return count;
        }

        public Deployment GetDetails(int deploymentId)
        {
            //Instantiate a SqlCommand object, supply it with a 
            //SELECT SQL statement that operates against the database, 
            //and the connection object for connecting to the database. 
            SqlCommand cmd = new SqlCommand("SELECT * FROM Deployment ORDER BY DeploymentID", conn);
            //Instantiate a DataAdapter object and pass the     
            //SqlCommand object created as parameter. 
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //Create a DataSet object to contain records get from database 
            DataSet result = new DataSet();

            //Open a database connection 
            conn.Open();
            //Use DataAdapter, which execute the SELECT SQL through its 
            //SqlCommand object to fetch data to a table "StaffDetails" 
            //in DataSet "result". 
            da.Fill(result, "DeploymentDetails");
            //Close the database connection 
            conn.Close();

            Deployment deployment = new Deployment(); if (result.Tables["DeploymentDetails"].Rows.Count > 0)
            {
                deployment.DeploymentID = deploymentId;
                // Fill staff object with values from the DataSet 
                DataTable table = result.Tables["DeploymentDetails"];
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["DeploymentType"]))
                    deployment.DeploymentType = table.Rows[deploymentId - 1]["DeploymentType"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["MID"]))
                    deployment.MID = table.Rows[deploymentId - 1]["MID"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["TID"]))
                    deployment.TID = table.Rows[deploymentId - 1]["TID"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["Schemes"]))
                    deployment.Schemes = table.Rows[deploymentId - 1]["Schemes"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["MerchantType"]))
                    deployment.MerchantType = table.Rows[deploymentId - 1]["MerchantType"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["SGQRID"]))
                    deployment.SGQRID = table.Rows[deploymentId - 1]["SGQRID"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["SGQRVer"]))
                    deployment.SGQRVersion = table.Rows[deploymentId - 1]["SGQRVer"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["DeploymentPhoto"]))
                    deployment.DeploymentPhoto = table.Rows[deploymentId - 1]["DeploymentPhoto"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["PhotoBefore"]))
                    deployment.PhotoBefore = table.Rows[deploymentId - 1]["PhotoBefore"].ToString();
                if (!DBNull.Value.Equals(table.Rows[deploymentId - 1]["PhotoAfter"]))
                    deployment.PhotoAfter = table.Rows[deploymentId - 1]["PhotoAfter"].ToString();
                return deployment;  // No error occurs 
            }
            else
            {
                return null; // Record not found 
            }
        }

        public List<Deployment> GetAllDeployment()
        {
            //Instantiate a SqlCommand object, supply it with a SELECT SQL statement that operates against the database, and the connection object for connecting to the database
            SqlCommand cmd = new SqlCommand("SELECT * FROM Deployment ORDER BY DeploymentID", conn);
            //Instantiate a DataAdapter object and pass the SqlCommand object created as parameter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //Create a DataSet object to contain records get from database 
            DataSet result = new DataSet();
            //Open a database connection 
            conn.Open();
            //Use DataAdapter, which execute the SELECT SQL through its SqlCommand object to fetch data to a table "StaffDetails" in DataSet "result" 
            da.Fill(result, "DeploymentDetails");
            //Close the database connection 
            conn.Close();
            //Transferring rows of data in DataSet’s table to “Staff” objects 
            List<Deployment> deploymentList = new List<Deployment>();
            foreach (DataRow row in result.Tables["DeploymentDetails"].Rows)
            {
                int? DeploymentID;
                //DeploymentID column not null
                if (!DBNull.Value.Equals(row["DeploymentID"]))
                    DeploymentID = Convert.ToInt32(row["DeploymentID"]);
                else
                    DeploymentID = null;

                deploymentList.Add(
                    new Deployment
                    {
                        DeploymentID = Convert.ToInt32(row["DeploymentID"]),
                        DeploymentType = row["DeploymentType"].ToString(),
                        MID = row["MID"].ToString(),
                        TID = row["TID"].ToString(),
                        Schemes = row["Schemes"].ToString(),
                        MerchantType = row["MerchantType"].ToString(),
                        SGQRID = row["SGQRID"].ToString(),
                        SGQRVersion = row["SGQRVer"].ToString(),
                        DeploymentPhoto = row["DeploymentPhoto"].ToString(),
                        PhotoBefore = row["PhotoBefore"].ToString(),
                        PhotoAfter = row["PhotoAfter"].ToString(),

                    }
                );
            }
            return deploymentList;
        }
    }
}
