using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Service : IService
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
                                Birthday = DBConvert.To<string>(reader[3]),
                                Email = DBConvert.To<string>(reader[4]),
                                Username = DBConvert.To<string>(reader[5]),
                                Password = DBConvert.To<string>(reader[6]),
                                Date_Registered = DBConvert.To<string>(reader[7])
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
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spAddAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("firstname", SqlDbType.VarChar, 99).Value = DBConvert.From(account.FirstName);
                    command.Parameters.Add("lastname", SqlDbType.VarChar, 99).Value = DBConvert.From(account.LastName);
                    command.Parameters.Add("birthday", SqlDbType.Date).Value = DBConvert.From(account.Birthday);
                    command.Parameters.Add("email", SqlDbType.VarChar, 99).Value = DBConvert.From(account.Email);
                    command.Parameters.Add("username", SqlDbType.VarChar, 99).Value = DBConvert.From(account.Username);
                    command.Parameters.Add("password", SqlDbType.VarChar, 99).Value = DBConvert.From(PassHash.MD5Hash(account.Password));
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

    public int UpdateAccountDetails(Account account)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spUpdateAccountDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(account.Account_ID);
                    command.Parameters.Add("firstname", SqlDbType.VarChar, 99).Value = DBConvert.From(account.FirstName);
                    command.Parameters.Add("lastname", SqlDbType.VarChar, 99).Value = DBConvert.From(account.LastName);
                    command.Parameters.Add("birthday", SqlDbType.Date).Value = DBConvert.From(account.Birthday);
                    command.Parameters.Add("email", SqlDbType.VarChar, 99).Value = DBConvert.From(account.Email);
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

    public int UpdateAccountPassword(int account_id, string new_password)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spUpdateAccountPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(account_id);
                    command.Parameters.Add("newPw", SqlDbType.VarChar, 99).Value = DBConvert.From(PassHash.MD5Hash(new_password));
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

    public int LoginAccount(string username, string password)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spLoginAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("username", SqlDbType.VarChar, 99).Value = DBConvert.From(username);
                    command.Parameters.Add("password", SqlDbType.VarChar, 99).Value = DBConvert.From(PassHash.MD5Hash(password));
                    connection.Open();

                    return (int)command.ExecuteScalar(); //returns id of user
                }
            }
        }
        catch
        {
            return -1;
        }
    }
}
