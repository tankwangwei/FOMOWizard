using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using FOMOWizard.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FOMOWizard.DAL
{
    public class OperationsStaffDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public OperationsStaffDAL()
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

        public int Add(Deployment deployment)
        { 
            SqlCommand cmd = new SqlCommand
                ("INSERT INTO Deployment (DeploymentType, MID, TID, Schemes, MerchantType, SGQRID, SGQRVer, DeploymentPhoto, PhotoBefore, PhotoAfter) " + "OUTPUT INSERTED.DeploymentID " + "VALUES(@deploymenttype, @mid, @tid, @schemes, @merchanttype, @sgqrid, @sgqrver, @deploymentphoto, @photobefore, @photoafter)", conn);
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

            conn.Open();
            deployment.DeploymentID = (int)cmd.ExecuteScalar();
            conn.Close();
            
            return (deployment.DeploymentID);
        }

        // Return number of row updated 
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
            
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();

            return count;
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
                        SGQRVersion = row["SGQRVer"].ToString()

                    }
                );
            }
            return deploymentList;
        }

        public List<Payload> GetPayloads()
        {
            //Path of file
            string filePath = @"C:\Users\LUN\Desktop\Demos\Test.csv";

            List<Payload> payloads = new List<Payload>();

            List<string> lines = File.ReadAllLines(filePath).ToList();

            //Skip header
            lines = lines.Skip(1).ToList();

            foreach (var line in lines)
            {
                string[] entries = line.Split(',');
                Payload newPayload = new Payload();

                //Store date as string from CSV file
                //string monthExtract = entries[17].Substring(5, 2);
                //Get the current year
                string currentMonth = DateTime.Now.ToString("yyyy");

                if (entries[17].Substring(5, 7) == currentMonth)
                {
                    newPayload.ID = Int32.Parse(entries[0]);
                    newPayload.UEN = entries[2];
                    newPayload.RegisteredName = entries[3];
                    newPayload.MID = entries[6];
                    newPayload.TID = entries[8];
                    //newPayload.DateAdded = entries[17];

                    payloads.Add(newPayload);
                }

            }

            var distinctPayloads = payloads.Distinct(new DistinctPayloadComparer()).ToList();

            return distinctPayloads;
        }

        public List<Payload> SortPayloadByDesc()
        {
            return GetPayloads().OrderByDescending(x => x.ID).ToList();
        }

        class DistinctPayloadComparer : IEqualityComparer<Payload>
        {

            public bool Equals(Payload x, Payload y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(Payload payload)
            {
                return payload.ID.GetHashCode();
            }
        }


    }
}
