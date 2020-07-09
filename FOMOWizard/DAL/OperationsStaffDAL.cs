using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using FOMOWizard.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;

namespace FOMOWizard.DAL
{
    public class OperationsStaffDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        //Path of file
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\FullPayload\Full_Payload.csv");

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

        //Gets All name on label
        public List<string> getNameOnLabel()
        {
            List<string> namesOnLabel = new List<string>();
            List<string> lines = File.ReadAllLines(filePath).ToList();

            lines = lines.Skip(1).ToList();

            foreach (var line in lines)
            {
                string[] entries = line.Split(',');

                namesOnLabel.Add(entries[7]);
            }

            return namesOnLabel;
        }

        public List<Payload> GetMonthlyPayloads()
        {
            List<Payload> payloadsByMonth = new List<Payload>();
            if (File.Exists(filePath))
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();

                //Get current year
                string currentYear = DateTime.Now.Year.ToString();
                //Get current month
                string currentMonth = DateTime.Now.Month.ToString("d2");
                //Combines the string
                string currentYearMonth = currentYear + "-" + currentMonth;

                //Skip header
                lines = lines.Skip(1).ToList();

                foreach (var line in lines)
                {
                    string[] entries = line.Split(',');
                    Payload payload = new Payload();

                    if (entries[17].Length >= 10)
                    {
                        string dateExtract = entries[17].Substring(0, 7);

                        if (dateExtract.Contains(currentYearMonth))
                        {
                            payload.ID = Int32.Parse(entries[0]);

                            payload.Location = entries[9] + " " + entries[10] + " " + entries[11];
                            payload.UEN = entries[2];
                            payload.MID = entries[6];
                            payload.TID = entries[8];
                            payload.NameOnLabel = entries[7];
                            if (entries[13] == "")
                            {
                                payload.SGQRID = "Empty SGQR";
                                payload.ContactPerson = entries[22];
                                payload.ContactNo = entries[23];
                            }
                            else
                            {
                                payload.SGQRID = entries[13];
                                payload.ContactPerson = entries[21];
                                payload.ContactNo = entries[22];
                            }

                            payloadsByMonth.Add(payload);
                        }
                    }
                }

                var distinctPayloads = payloadsByMonth.Distinct(new DistinctPayloadComparer()).ToList();

                return distinctPayloads;
            }

            return payloadsByMonth;
            
        }

        public List<Payload> SortPayloadByDesc()
        {
            return GetMonthlyPayloads().OrderByDescending(x => x.ID).ToList();
        }
    }
}
