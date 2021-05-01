﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

[ServiceContract]
public interface IService
{
    // GET Method // Get Records
    [OperationContract]
    [WebInvoke(Method = "GET",
        UriTemplate = "/GetAccounts",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    List<Account> GetAccounts();

    // POST Method // Create New Records
    [OperationContract]
    [WebInvoke(Method = "POST",
        UriTemplate = "/AddAccount",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
    int AddAccount(Account account);

    // PUT Method // Updating Records
    [OperationContract]
    [WebInvoke(Method = "PUT",
        UriTemplate = "/UpdateAccountDetails",
        BodyStyle = WebMessageBodyStyle.Wrapped, 
        RequestFormat = WebMessageFormat.Json, 
        ResponseFormat = WebMessageFormat.Json)]
    int UpdateAccountDetails(Account account);

    [OperationContract]
    [WebInvoke(Method = "PUT",
        UriTemplate = "/UpdateAccountPassword",
        BodyStyle = WebMessageBodyStyle.Wrapped,
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    int UpdateAccountPassword(int account_id, string new_password);

    // DELETE Method // Deleting Records
    //[OperationContract]
    //[WebInvoke(Method = "DELETE",
    //    UriTemplate = "/DeleteAccount",
    //    BodyStyle = WebMessageBodyStyle.Wrapped, 
    //    RequestFormat = WebMessageFormat.Json, 
    //    ResponseFormat = WebMessageFormat.Json)]
    //void DeleteAccount(int account_id);
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
