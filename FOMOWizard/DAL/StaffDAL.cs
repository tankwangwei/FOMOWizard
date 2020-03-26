using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FOMOWizard.Models;
using System.Data;

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
            string strConn = Configuration.GetConnectionString("FOMOConnectionString");

            //Instantiate a SqlConnection object with the Connection String read. 
            conn = new SqlConnection(strConn);
            
        }

        public List<Staff> GetAllStaff()
        {
            //Instantiate a SqlCommand object, supply it with a 
            //SELECT SQL statement that operates against the database, 
            //and the connection object for connecting to the database. 
            SqlCommand cmd = new SqlCommand("SELECT * FROM Staff ORDER BY StaffID", conn);
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
            da.Fill(result, "StaffDetails");
            //Close the database connection 
            conn.Close();
            //Transferring rows of data in DataSet’s table to “Staff” objects 
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
                        Approved = Convert.ToBoolean(row["Approved"])
                    }
            );
            }
            return staffList;
        }

        public Staff GetStaff(string loginID)
        {
            //Instantiate a SqlCommand object, supply it with a 
            //SELECT SQL statement that operates against the database, 
            //and the connection object for connecting to the database. 
            SqlCommand cmd = new SqlCommand("SELECT * FROM Staff WHERE Email = '" + loginID + "'", conn);
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
            da.Fill(result, "StaffDetails");
            //Close the database connection 
            conn.Close();
            Staff staff = new Staff() { };
            foreach (DataRow row in result.Tables["StaffDetails"].Rows)
            {
                staff = new Staff
                {
                    StaffID = Convert.ToInt32(row["StaffID"]),
                    Name = row["Name"].ToString(),
                    Password = Convert.ToString(row["Password"]),
                    Email = row["Email"].ToString(),
                    Department = Convert.ToString(row["Department"]),
                    Approved = Convert.ToBoolean(row["Approved"])
                };
            }
            return staff;
        }

        public int Add(Deployment deployment)
        {
            //Instantiate a SqlCommand object,supply it with an INSERT SQL statement //which will return the auto-generated StaffID after insertion, //and the connection object for connecting to the database. 
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO Deployment (DeploymentType, MID, TID, Schemes, MerchantType, SGQRID, SGQRVer, DeploymentPhoto, PhotoBefore, PhotoAfter) " + "OUTPUT INSERTED.DeploymentID " + "VALUES(@deploymenttype, @mid, @tid, @schemes, @merchanttype, @sgqrid, @sgqrver, @deploymentphoto, @photobefore, @photoafter)", conn);
            //Define the parameters used in SQL statement, value for each parameter //is retrieved from respective class's property. 
            cmd.Parameters.AddWithValue("@deploymenttype", deployment.DeploymentType);
            cmd.Parameters.AddWithValue("@mid", deployment.MID);
            cmd.Parameters.AddWithValue("@tid", deployment.TID);
            cmd.Parameters.AddWithValue("@schemes", deployment.Schemes);
            cmd.Parameters.AddWithValue("@merchanttype", deployment.MerchantType);
            cmd.Parameters.AddWithValue("@sgqrid", deployment.SGQRID);
            cmd.Parameters.AddWithValue("@sgqrver", deployment.SGQRVersion);
            cmd.Parameters.AddWithValue("@deploymentphoto", deployment.DeploymentPhoto);
            cmd.Parameters.AddWithValue("@photobefore", deployment.PhotoBefore);
            cmd.Parameters.AddWithValue("@photoafter", deployment.PhotoAfter);

            //A connection to database must be opened before any operations made.
            conn.Open();

            //ExecuteScalar is used to retrieve the auto-generated //StaffID after executing the INSERT SQL statement
            deployment.DeploymentID = (int)cmd.ExecuteScalar();

            //A connection should be closed after operations. 
            conn.Close();

            //Return id when no error occurs. 
            return (deployment.DeploymentID);
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
                //BranchNo column not null 
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
                        SGQRVersion = row["SGQRVersion"].ToString(),
                        
                    }
                );
            }
            return deploymentList;
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
            Deployment deployment = new Deployment() { };
            foreach (DataRow row in result.Tables["DeploymentDetails"].Rows)
            {
                deployment = new Deployment
                {
                    DeploymentID = Convert.ToInt32(row["DeploymentID"]),
                    DeploymentType = row["DeploymentType"].ToString(),
                    MID = row["MID"].ToString(),
                    TID = row["TID"].ToString(),
                    Schemes = Convert.ToString(row["Schemes"]),
                    MerchantType = Convert.ToString(row["MerchantType"]),
                    SGQRID = row["MID"].ToString(),
                    SGQRVersion = row["SGQRVer"].ToString(),
                };
            }
            return deployment;
        }
    }
}
