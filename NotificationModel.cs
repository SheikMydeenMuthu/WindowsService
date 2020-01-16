using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationService
{
    public class IosNotificationData
    {
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("notification")]
        public SiNotificationData Notification { get; set; }
    }
    public class AndroidNotificationData
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("data")]
        public SiNotificationData Data { get; set; }
    }
    public class SiNotificationData
    {
        //Notification Title
        [JsonProperty("title")]
        public string Title { get; set; }

        //Notification Body
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        ///This property contains parameter of notification in mobile receiving side.
        ///As of now we send multiple parameter(ActivityTransaction ID,Activity Type ID) with single string with help of pipe symble        
        /// </summary>
        [JsonProperty("body_loc_key")]
        public string PushNotificationParameters { get; set; }

        // <summary>
        /// this Methods tells which type of Notification Like General or Activity
        /// </summary>

        [JsonProperty("title_loc_key")]
        public bool IsGenaralNotification { get; set; }
    }

    public class DeviceDetails
    {
        public string DeviceType { get; set; }
        public string TokenID { get; set; }

    }

    public class NotificationData
    {
        public int AgreementID { get; set; }

        public int ActivityTransactionID { get; set; }

        public int ActivityTypeId { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }

        public int AccountExecutiveID { get; set; }

        public int NotificationCreatedUserID { get; set; }

        public string NotificationCreaterEmail { get; set; }

        public string BranchName { get; set; }

        public string NotificationCreaterUserName { get; set; }

        public int CandidateID { get; set; }

        public string CandidateName { get; set; }
    }
}
