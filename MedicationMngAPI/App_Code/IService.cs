using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

[ServiceContract]
public interface IService
{
    #region METHOD: GET
    [OperationContract]
    [WebInvoke(Method = "GET",
        UriTemplate = "/GetAccountDetails/{id}",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "GetAccountDetailsResult")]
    Account GetAccountDetails(string id);

    [OperationContract]
    [WebInvoke(Method = "GET",
        UriTemplate = "/LoginAccount/{username}/{password}",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "LoginAccountResult")]
    int LoginAccount(string username, string password);

    [OperationContract]
    [WebInvoke(Method = "GET",
        UriTemplate = "/GetMedTypes",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "GetMedTypesResult")]
    List<MedType> GetMedTypes();

    [OperationContract]
    [WebInvoke(Method = "GET",
        UriTemplate = "/GetRatingsRecommendation/{account_id}",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "GetRatingsRecommendationResult")]
    Ratings_Recommendation GetRatingsRecommendation(string account_id);

    [OperationContract]
    [WebInvoke(Method = "GET",
        UriTemplate = "/GetMedTakes/{account_id}",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "GetMedTakesResult")]
    List<MedTake> GetMedTakes(string account_id);
    #endregion //METHOD: GET

    #region METHOD: POST
    [OperationContract]
    [WebInvoke(Method = "POST",
        UriTemplate = "/AddAccount",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "AddAccountResult")]
    int AddAccount(Account account);

    [OperationContract]
    [WebInvoke(Method = "POST",
        UriTemplate = "/AddMedTake",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "AddMedTakeResult")]
    int AddMedTake(MedTake medtake, List<MedTakeSchedule> medtakeschedules);

    [OperationContract]
    [WebInvoke(Method = "POST",
        UriTemplate = "/AddRatingsRecommendation",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "AddRatingsRecommendationResult")]
    int AddRatingsRecommendation(Ratings_Recommendation ratingsRecommendation);
    #endregion //METHOD: POST

    #region METHOD: PUT
    [OperationContract]
    [WebInvoke(Method = "PUT",
        UriTemplate = "/UpdateAccountDetails",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "UpdateAccountDetailsResult")]
    int UpdateAccountDetails(Account account);

    [OperationContract]
    [WebInvoke(Method = "PUT",
        UriTemplate = "/UpdateAccountPassword",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "UpdateAccountPasswordResult")]
    int UpdateAccountPassword(int account_id, string old_password, string new_password);

    [OperationContract]
    [WebInvoke(Method = "PUT",
        UriTemplate = "/UpdateMedTake",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "UpdateMedTakeResult")]
    int UpdateMedTake(MedTake medtake);
    #endregion //METHOD: PUT

    #region METHOD: DELETE
    [OperationContract]
    [WebInvoke(Method = "DELETE",
        UriTemplate = "/DeleteMedTake",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    [return: MessageParameter(Name = "DeleteMedTakeResult")]
    int DeleteMedTake(MedTake medtake);
    #endregion //METHOD: DELETE
}

[DataContract]
public class Account
{
    private int account_id = -1;
    private string firstname = string.Empty;
    private string lastname = string.Empty;
    private DateTime birthday = DateTime.MinValue;
    private string email = string.Empty;
    private string username = string.Empty;
    private string password = string.Empty;
    private DateTime date_registered = DateTime.MinValue;

    [DataMember]
    public int Account_ID
    {
        get { return account_id; }
        set { account_id = value; }
    }

    [DataMember]
    public string FirstName
    {
        get { return firstname; }
        set { firstname = value; }
    }

    [DataMember]
    public string LastName
    {
        get { return lastname; }
        set { lastname = value; }
    }

    [DataMember]
    public string Birthday
    {
        get { return birthday.ToDateOnly(); }
        set { birthday = value.ToDateTime(); }
    }

    [DataMember]
    public string Email
    {
        get { return email; }
        set { email = value; }
    }

    [DataMember]
    public string Username
    {
        get { return username; }
        set { username = value; }
    }

    [DataMember]
    public string Password
    {
        get { return password; }
        set { password = value; }
    }

    [DataMember]
    public string Date_Registered
    {
        get { return date_registered.ToDateWithTime(); }
        set { date_registered = value.ToDateTime(); }
    }
}

[DataContract]
public class MedTake
{
    private int med_take_id = -1;
    private string med_name = string.Empty;
    private int? med_count = null;
    private int account_id = -1;
    private int med_type_id = -1;

    [DataMember]
    public int Med_Take_ID
    {
        get { return med_take_id; }
        set { med_take_id = value; }
    }

    [DataMember]
    public int Account_ID
    {
        get { return account_id; }
        set { account_id = value; }
    }

    [DataMember]
    public string Med_Name
    {
        get { return med_name; }
        set { med_name = value; }
    }

    [DataMember]
    public int? Med_Count
    {
        get { return med_count; }
        set { med_count = value; }
    }

    [DataMember]
    public int Med_Type_ID
    {
        get { return med_type_id; }
        set { med_type_id = value; }
    }
}

[DataContract]
public class MedType
{
    private int med_type_id = -1;
    private string med_type_name = string.Empty;
    private bool iscount = false;
    private string image = string.Empty;

    [DataMember]
    public int Med_Type_ID
    {
        get { return med_type_id; }
        set { med_type_id = value; }
    }

    [DataMember]
    public string Med_Type_Name
    {
        get { return med_type_name; }
        set { med_type_name = value; }
    }

    [DataMember]
    public bool IsCount
    {
        get { return iscount; }
        set { iscount = value; }
    }

    [DataMember]
    public string Image
    {
        get { return image; }
        set { image = value; }
    }
}

[DataContract]
public class MedTakeSchedule
{
    private int med_take_schedule_id = -1;
    private int day_of_week = 0;
    private int dosage_count = 0;
    private int med_take_id = -1;
    private TimeSpan time = TimeSpan.Zero;

    [DataMember]
    public int Med_Take_Schedule_ID 
    {
        get { return med_take_schedule_id; }
        set { med_take_schedule_id = value; }
    }

    [DataMember]
    public int Med_Take_ID
    {
        get { return med_take_id; }
        set { med_take_id = value; }
    }

    [DataMember]
    public int Day_Of_Week
    {
        get { return day_of_week; }
        set { day_of_week = value; }
    }

    [DataMember]
    public int Dosage_Count
    {
        get { return dosage_count; }
        set { dosage_count = value; }
    }

    [DataMember]
    public TimeSpan Time
    {
        get { return time; }
        set { time = value; }
    }
}

[DataContract]
public class Ratings_Recommendation: Account
{
    private int ratings_recommendation_id = -1;
    private int ratings = 0;
    private string recommendations = string.Empty;
    private DateTime date = DateTime.MinValue;

    [DataMember]
    public int Ratings_Recommendation_ID
    {
        get { return ratings_recommendation_id; }
        set { ratings_recommendation_id = value; }
    }

    [DataMember]
    public int Ratings
    {
        get { return ratings; }
        set { ratings = value; }
    }

    [DataMember]
    public string Recommendations
    {
        get { return recommendations; }
        set { recommendations = value; }
    }


    [DataMember]
    public string Date
    {
        get { return date.ToDateWithTime(); }
        set { date = value.ToDateTime(); }
    }
}