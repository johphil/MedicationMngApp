using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MedicationMngSvc
{
   [ServiceContract]
   public interface IService1
    {
        // GET Method //
        [OperationContract]
        [WebGet(UriTemplate = "GetAccount")]
        List<Account> GetAccounts();

        // POST Method //
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AddAccount",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        int AddAccount(Account account);
    }

    [DataContract]
    public class Account
    {
        private int account_id { get; set; }
        private string firstname { get; set; }
        private string lastname { get; set; }
        private DateTime birthday { get; set; }
        private string email { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private DateTime date_registered { get; set; }

        [DataMember]
        public int Account_ID
        {
            get => account_id;
            set => account_id = value;
        }

        [DataMember]
        public string FirstName
        {
            get => firstname;
            set => firstname = value;
        }

        [DataMember]
        public string LastName
        {
            get => lastname;
            set => lastname = value;
        }

        [DataMember]
        public DateTime Birthday
        {
            get => birthday;
            set => birthday = value;
        }

        [DataMember]
        public string Email
        {
            get => email;
            set => email = value;
        }

        [DataMember]
        public string Username
        {
            get => username;
            set => username = value;
        }

        [DataMember]
        public string Password
        {
            get => password;
            set => password = value;
        }

        [DataMember]
        public DateTime Date_Registered
        {
            get => date_registered;
            set => date_registered = value;
        }
    }
}
