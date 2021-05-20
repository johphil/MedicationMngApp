using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    //Database Conenction String
    protected string conStr = ConfigurationManager.ConnectionStrings["MEDMNG_DBF"].ConnectionString;

    /// <summary>
    /// Used to get the account information of the user
    /// </summary>
    /// <param name="account_id">ID assigned to the user's account</param>
    /// <returns>Account object which contains the information</returns>
    public Account GetAccountDetails(string account_id)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetAccountDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("id", SqlDbType.Int).Value = DBConvert.From(int.Parse(account_id));
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Account
                            {
                                Account_ID = DBConvert.To<int>(reader[0]),
                                FirstName = DBConvert.To<string>(reader[1]),
                                LastName = DBConvert.To<string>(reader[2]),
                                Birthday = DBConvert.To<string>(reader[3]),
                                Email = DBConvert.To<string>(reader[4]),
                                Username = DBConvert.To<string>(reader[5]),
                                Date_Registered = DBConvert.To<string>(reader[6])
                            };
                        }
                    }
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Used to add a new user account after successfull registration
    /// </summary>
    /// <param name="account">Account object which contains the supplied information by the user</param>
    /// <returns>Returns integer value -69 if username exists, -70 if email exists, -1 if failed, and 1 if success</returns>
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

                    return (int)command.ExecuteScalar();
                }
            }
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Used to updated the user's account information to the database
    /// </summary>
    /// <param name="account">Account object which contains the supplied information by the user</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
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

    /// <summary>
    /// Used to activate or deactivate a medication
    /// </summary>
    /// <param name="med_take_id">ID assigned to the selected medication</param>
    /// <param name="enabled">1 if true, 0 if false</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int UpdateMedTakeEnable(string med_take_id, string enabled)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spUpdateMedTakeStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(med_take_id));
                    command.Parameters.Add("isactive", SqlDbType.Bit).Value = DBConvert.From(int.Parse(enabled));
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

    /// <summary>
    /// Used to update the user's account password
    /// </summary>
    /// <param name="account_id">ID assigned to the user's account</param>
    /// <param name="old_password">Current password of the user</param>
    /// <param name="new_password">New password supplied by the user</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int UpdateAccountPassword(int account_id, string old_password, string new_password)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spUpdateAccountPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(account_id);
                    command.Parameters.Add("oldPw", SqlDbType.VarChar, 99).Value = DBConvert.From(PassHash.MD5Hash(old_password));
                    command.Parameters.Add("newPw", SqlDbType.VarChar, 99).Value = DBConvert.From(PassHash.MD5Hash(new_password));
                    connection.Open();

                    return (int)command.ExecuteScalar();
                }
            }
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Used to authenticate the user credentials before accessing the main interface of the application
    /// </summary>
    /// <param name="username">Username of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>returns the account id of the authenticated user</returns>
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

    /// <summary>
    /// Used to add the user's ratings and recommendation
    /// </summary>
    /// <param name="ratings">Ratings_Recommendation object which contains the ratings and feedback by the user</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int AddRatingsRecommendation(Ratings_Recommendation ratings)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spAddRatings", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(ratings.Account_ID);
                    command.Parameters.Add("ratings", SqlDbType.Int).Value = DBConvert.From(ratings.Ratings);
                    command.Parameters.Add("recommendation", SqlDbType.VarChar).Value = DBConvert.From(ratings.Recommendation);
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

    /// <summary>
    /// Used to add a new medication from the user
    /// </summary>
    /// <param name="medtake">MedTake object</param>
    /// <param name="medtakeschedules">Collection of MedTakeSchedule object</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int AddMedTake(MedTake medtake, List<MedTakeSchedule> medtakeschedules)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command = new SqlCommand("spAddMedTake", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(medtake.Account_ID);
                            command.Parameters.Add("med_name", SqlDbType.VarChar, 20).Value = DBConvert.From(medtake.Med_Name);
                            command.Parameters.Add("med_count", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Count);
                            command.Parameters.Add("med_count_critical", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Count_Critical);
                            command.Parameters.Add("med_type_id", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Type_ID);

                            medtake.Med_Take_ID = (int)command.ExecuteScalar();

                            if (medtake.Med_Take_ID > 0)
                            {
                                command.Parameters.Clear();
                                command.CommandText = "spAddMedTakeSchedule";

                                foreach (var schedule in medtakeschedules)
                                {
                                    command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Take_ID);
                                    command.Parameters.Add("day_of_week", SqlDbType.Int).Value = DBConvert.From(schedule.Day_Of_Week);
                                    command.Parameters.Add("dosage_count", SqlDbType.Int).Value = DBConvert.From(schedule.Dosage_Count);
                                    command.Parameters.Add("time", SqlDbType.Time, 7).Value = DBConvert.From(schedule.Time);

                                    command.ExecuteNonQuery();
                                    command.Parameters.Clear();
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
            return 1;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Used to get all medications added by the user
    /// </summary>
    /// <param name="account_id">ID assigned to the user</param>
    /// <returns>returns a collection of medications</returns>
    public List<MedTake> GetMedTakes(string account_id)
    {
        try
        {
            List<MedTake> collection = new List<MedTake>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetMedTakes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(account_id));
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new MedTake
                            {
                                Med_Take_ID = DBConvert.To<int>(reader[0]),
                                Account_ID = DBConvert.To<int>(reader[1]),
                                Med_Name = DBConvert.To<string>(reader[2]),
                                Med_Count = DBConvert.To<int>(reader[3]),
                                Med_Count_Critical = DBConvert.To<int>(reader[4]),
                                Med_Type_ID = DBConvert.To<int>(reader[5]),
                                Med_Type_Name = DBConvert.To<string>(reader[6]),
                                IsCount = DBConvert.To<bool>(reader[7]),
                                Image = DBConvert.To<string>(reader[8]),
                                IsActive = DBConvert.To<bool>(reader[9])
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

    /// <summary>
    /// Used to update the information of the selected medication
    /// </summary>
    /// <param name="medtake">MedTake object</param>
    /// <param name="deletemedtakeschedules">List of MedTakes that will be deleted</param>
    /// <param name="updatemedtakeschedules">List of MedTakes that will be updated</param>
    /// <param name="createmedtakeschedules">List of MedTakes that will be created</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int UpdateMedTake(MedTake medtake, List<MedTakeSchedule> deletemedtakeschedules, List<MedTakeSchedule> updatemedtakeschedules, List<MedTakeSchedule> createmedtakeschedules)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command = new SqlCommand("spUpdateMedTake", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Take_ID);
                            command.Parameters.Add("med_name", SqlDbType.VarChar, 20).Value = DBConvert.From(medtake.Med_Name);
                            command.Parameters.Add("med_count", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Count);
                            command.Parameters.Add("med_count_critical", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Count_Critical);
                            command.Parameters.Add("med_type_id", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Type_ID);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                //Create MedTake Schedule
                                if (createmedtakeschedules != null && createmedtakeschedules.Count > 0)
                                {
                                    command.Parameters.Clear();
                                    command.CommandText = "spAddMedTakeSchedule";

                                    foreach (var schedule in createmedtakeschedules)
                                    {
                                        command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(medtake.Med_Take_ID);
                                        command.Parameters.Add("day_of_week", SqlDbType.Int).Value = DBConvert.From(schedule.Day_Of_Week);
                                        command.Parameters.Add("dosage_count", SqlDbType.Int).Value = DBConvert.From(schedule.Dosage_Count);
                                        command.Parameters.Add("time", SqlDbType.Time, 7).Value = DBConvert.From(schedule.Time);

                                        command.ExecuteNonQuery();
                                        command.Parameters.Clear();
                                    }
                                }

                                //Update MedTake Schedule
                                if (updatemedtakeschedules != null && updatemedtakeschedules.Count > 0)
                                {
                                    command.Parameters.Clear();
                                    command.CommandText = "spUpdateMedTakeSchedule";

                                    foreach (var schedule in updatemedtakeschedules)
                                    {
                                        command.Parameters.Add("med_take_schedule_id", SqlDbType.Int).Value = DBConvert.From(schedule.Med_Take_Schedule_ID);
                                        command.Parameters.Add("day_of_week", SqlDbType.Int).Value = DBConvert.From(schedule.Day_Of_Week);
                                        command.Parameters.Add("dosage_count", SqlDbType.Int).Value = DBConvert.From(schedule.Dosage_Count);
                                        command.Parameters.Add("time", SqlDbType.Time, 7).Value = DBConvert.From(schedule.Time);

                                        command.ExecuteNonQuery();
                                        command.Parameters.Clear();
                                    }
                                }

                                //Delete MedTake Schedule
                                if (deletemedtakeschedules != null && deletemedtakeschedules.Count > 0)
                                {
                                    command.Parameters.Clear();
                                    command.CommandText = "spDeleteMedTakeSchedule";

                                    foreach (var schedule in deletemedtakeschedules)
                                    {
                                        command.Parameters.Add("med_take_schedule_id", SqlDbType.Int).Value = DBConvert.From(schedule.Med_Take_Schedule_ID);

                                        command.ExecuteNonQuery();
                                        command.Parameters.Clear();
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
            return 1;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Used to delete a medication
    /// </summary>
    /// <param name="med_take_id">ID assigned to the selected medication</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int DeleteMedTake(string med_take_id)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spDeleteMedTake", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(med_take_id));
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

    /// <summary>
    /// Used to get all available types of medication from the database
    /// </summary>
    /// <returns>returns the list of medtypes</returns>
    public List<MedType> GetMedTypes()
    {
        try
        {
            List<MedType> collection = new List<MedType>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetMedTypes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new MedType
                            {
                                Med_Type_ID = DBConvert.To<int>(reader[0]),
                                Med_Type_Name = DBConvert.To<string>(reader[1]),
                                IsCount = DBConvert.To<bool>(reader[2]),
                                Image = DBConvert.To<string>(reader[3])
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

    /// <summary>
    /// Used to get the schedules of a medication
    /// </summary>
    /// <param name="med_take_id">ID of the selected medication</param>
    /// <returns>returns a collection of MedTakeSchedule</returns>
    public List<MedTakeSchedule> GetMedTakeSchedules(string med_take_id)
    {
        try
        {
            List<MedTakeSchedule> collection = new List<MedTakeSchedule>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetMedTakeSchedules", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(med_take_id));
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new MedTakeSchedule
                            {
                                Med_Take_Schedule_ID = DBConvert.To<int>(reader[0]),
                                Med_Take_ID = DBConvert.To<int>(reader[1]),
                                Day_Of_Week = DBConvert.To<int>(reader[2]),
                                Dosage_Count = DBConvert.To<int>(reader[3]),
                                Time = DBConvert.To<TimeSpan>(reader[4]).ToString(),
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

    /// <summary>
    /// Used to get the medications scheduled for the day
    /// </summary>
    /// <param name="account_id">ID assigned to the user</param>
    /// <param name="day_of_week">Day of week in integer</param>
    /// <returns>returns the list of medications to be taken for the day</returns>
    public List<MedTakeToday> GetMedTakeToday(string account_id, string day_of_week)
    {
        try
        {
            List<MedTakeToday> collection = new List<MedTakeToday>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetMedTakeToday", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(account_id));
                    command.Parameters.Add("day_of_week", SqlDbType.Int).Value = DBConvert.From(int.Parse(day_of_week));
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new MedTakeToday
                            {
                                Med_Take_ID = DBConvert.To<int>(reader[0]),
                                Med_Take_Schedule_ID = DBConvert.To<int>(reader[1]),
                                Time = DBConvert.To<TimeSpan>(reader[2]).ToString(),
                                Day_Of_Week = DBConvert.To<int>(reader[3]),
                                Med_Name = DBConvert.To<string>(reader[4]),
                                Dosage_Count = DBConvert.To<int>(reader[5]),
                                Image = DBConvert.To<string>(reader[6]),
                                Last_Take = DBConvert.To<DateTime?>(reader[7]),
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

    /// <summary>
    /// Used to get the password of a user
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <returns>returns the decrypted password</returns>
    public string GetAccountPassword(string email)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetAccountPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("email", SqlDbType.VarChar, 99).Value = DBConvert.From(email);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return PassHash.MD5HashDecrypt(DBConvert.To<string>(reader[0]));
                        }
                    }
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Used to get the logs information of an account
    /// </summary>
    /// <param name="account_id">ID assigned to the user</param>
    /// <returns>returns the list of AccountLog</returns>
    public List<AccountLog> GetAccountLogs(string account_id)
    {
        try
        {
            List<AccountLog> collection = new List<AccountLog>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetAccountLogs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(account_id));

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new AccountLog
                            {
                                Account_Log_ID = DBConvert.To<int>(reader[0]),
                                Account_ID = DBConvert.To<int>(reader[1]),
                                Date = DBConvert.To<DateTime>(reader[2]),
                                Tag = DBConvert.To<string>(reader[3]),
                                Description = DBConvert.To<string>(reader[4])
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

    /// <summary>
    /// Used to set the status of the selected medication schedule as taken
    /// </summary>
    /// <param name="med_take_schedule_id">ID assigned to the medication schedule</param>
    /// <param name="med_take_id">ID assigned to the medication</param>
    /// <returns>returns positive integer if success otherwise failed</returns>
    public int TakeMedicine(string med_take_schedule_id, string med_take_id)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spTakeMedicine", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("med_take_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(med_take_id));
                    command.Parameters.Add("med_take_schedule_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(med_take_schedule_id));
                    connection.Open();

                    return (int)command.ExecuteScalar();
                }
            }
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Used to get the intake logs of a user
    /// </summary>
    /// <param name="account_id">ID assigned to the user</param>
    /// <returns>returns the list of intake logs of a user</returns>
    public List<IntakeLog> GetIntakeLogs(string account_id)
    {
        try
        {
            List<IntakeLog> collection = new List<IntakeLog>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetIntakeLogs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(account_id));

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new IntakeLog
                            {
                                Intake_Log_ID = DBConvert.To<int>(reader[0]),
                                Account_ID = DBConvert.To<int>(reader[1]),
                                Med_Name = DBConvert.To<string>(reader[2]),
                                Dosage_Count = DBConvert.To<int>(reader[3]),
                                Med_Type_Name = DBConvert.To<string>(reader[4]),
                                Image = DBConvert.To<string>(reader[5]),
                                Taken = DBConvert.To<string>(reader.GetDateTime(6).ToDateWithTime())
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
}
