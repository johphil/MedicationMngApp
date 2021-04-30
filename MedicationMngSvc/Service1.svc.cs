using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MedicationMngSvc.Utils;

namespace MedicationMngSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private string conStr = ConfigurationManager.ConnectionStrings["MEDMNG_DBF"].ConnectionString;

        public List<Account> GetAccounts()
        {
            try
            {
                List<Account> collection = new List<Account>();
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    using (SqlCommand command = new SqlCommand("spGetAccounts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //command.Parameters.Add("employeenumber", SqlDbType.VarChar, 20).Value = EmployeeNumber;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                collection.Add(new Account
                                {
                                    Account_ID = DBConvert.To<int>(reader[0]),
                                    FirstName = DBConvert.To<string>(reader[1]),
                                    LastName = DBConvert.To<string>(reader[2]),
                                    Birthday = DBConvert.To<DateTime>(reader[3]),
                                    Email = DBConvert.To<string>(reader[4]),
                                    Username = DBConvert.To<string>(reader[5]),
                                    Password = DBConvert.To<string>(reader[6]),
                                    Date_Registered = DBConvert.To<DateTime>(reader[7])
                                });
                            }
                        }
                    }
                }
                return collection;
            }
            catch
            {
                return null;
            }
        }

        public int AddAccount(Account account)
        {
            try
            {
                List<Account> collection = new List<Account>();
                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    using (SqlCommand command = new SqlCommand("spAddAccount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("firstname", SqlDbType.Char, 99).Value = account.FirstName;
                        command.Parameters.Add("lastname", SqlDbType.Char, 99).Value = account.FirstName;
                        command.Parameters.Add("birthday", SqlDbType.Date).Value = account.FirstName;
                        command.Parameters.Add("email", SqlDbType.Char, 99).Value = account.FirstName;
                        command.Parameters.Add("username", SqlDbType.Char, 99).Value = account.FirstName;
                        command.Parameters.Add("password", SqlDbType.Char, 99).Value = account.FirstName;
                        connection.Open();

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                return -1;
            }
        }
    }
}
