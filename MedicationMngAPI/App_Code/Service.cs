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
    private string conStr = ConfigurationManager.ConnectionStrings["MEDMNG_DBF"].ConnectionString;

    public Account GetAccountDetails(string id)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetAccountDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("id", SqlDbType.Int).Value = DBConvert.From(int.Parse(id));
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

    public int AddRatingsRecommendation(Ratings_Recommendation ratingsRecommendation) { return -1; }

    public Ratings_Recommendation GetRatingsRecommendation(string account_id) { return null; }

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
                                Med_Type_ID = DBConvert.To<int>(reader[4]),
                                Med_Type_Name = DBConvert.To<string>(reader[5]),
                                IsCount = DBConvert.To<bool>(reader[6]),
                                Image = DBConvert.To<string>(reader[7]),
                                IsActive = DBConvert.To<bool>(reader[8])
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
                                Time = DBConvert.To<TimeSpan>(reader[4]).ToString()
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

    public List<MedTakeUpcoming> GetMedTakeUpcoming(string account_id, string day_of_week)
    {
        try
        {
            List<MedTakeUpcoming> collection = new List<MedTakeUpcoming>();
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand("spGetMedTakeUpcoming", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("account_id", SqlDbType.Int).Value = DBConvert.From(int.Parse(account_id));
                    command.Parameters.Add("day_of_week", SqlDbType.Int).Value = DBConvert.From(int.Parse(day_of_week));
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            collection.Add(new MedTakeUpcoming
                            {
                                Time = DateTime.Today.Add(DBConvert.To<TimeSpan>(reader[0])).ToString("hh:mm tt").ToUpper(),
                                Day_Of_Week = DBConvert.To<int>(reader[1]),
                                Med_Name = DBConvert.To<string>(reader[2]),
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
}
