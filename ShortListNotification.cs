using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationService
{
    public class ShortListNotification : DatabaseContext
    {
        #region

        private static int sendNotificationCount = 0;
        private static int status = 0;

        #endregion

        #region Methods

        internal static SiNotificationData SetNotificationData(NotificationData data)
        {
            SiNotificationData notificationData = new SiNotificationData();
            notificationData.Title = ConfigurationManager.AppSettings["ActivityType"];

            notificationData.Body = GetNotificationBody(data);

            notificationData.PushNotificationParameters = data.ActivityTransactionID.ToString() + "|" + data.ActivityTypeId.ToString();

            notificationData.IsGenaralNotification = Convert.ToBoolean(ConfigurationManager.AppSettings["IsGeneral"]);

            return notificationData;
        }

        internal static AndroidNotificationData SetPushNotificationContentForAndroid(NotificationData data, DeviceDetails deviceDatails)
        {
            AndroidNotificationData androidNotificationData = new AndroidNotificationData();
            androidNotificationData.To = deviceDatails.TokenID;
            androidNotificationData.Data = SetNotificationData(data);

            return androidNotificationData;

        }

        internal static IosNotificationData SetPushNotificationContentForiOS(NotificationData data, DeviceDetails deviceDatails)
        {
            try
            {
                IosNotificationData notificationDataForiOS = new IosNotificationData();

                notificationDataForiOS.To = deviceDatails.TokenID;

                notificationDataForiOS.Notification=SetNotificationData(data);               

                return notificationDataForiOS;
            }
            catch (Exception ex)
            {
                LogIt.WriteErrorLog(ex);
                return null;
            }
        }

        private static string GetNotificationBody(NotificationData data)
        {
            string notificationBody = data.CandidateName + " has been shortlisted by " + data.NotificationCreaterUserName + " for " + data.CompanyName + "." + " Job ID " + data.AgreementID + " " + data.JobTitle.Substring(0, 20) + "...";

            return notificationBody;
        }

        private static StringContent GetHttpContent(object notificationModel)
        {
            var httpContent = string.Empty;
            httpContent = JsonConvert.SerializeObject(notificationModel);
            var stringContent = new StringContent(httpContent, Encoding.UTF8, "application/json");
            return stringContent;
        }

        internal static void UpdateNotificationStatus(int agreementID, int candidateID, int status)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    const string query =
                                @"EXEC sp_UpdateNotificationStatus @status,@agreementID,@candidateID";

                    var deviceID = db.Connection.Query<string>(query, new { status = status, agreementID = agreementID, candidateID = candidateID });

                }
            }
            catch (Exception ex)
            {

                LogIt.WriteErrorLog(ex);
            }
        }

        internal static IEnumerable<DeviceDetails> GetTokenID(int accountExecutiveID)
        {
            try
            {
                using (var db = new DatabaseContext())
                {

                    const string query =
                                @"EXEC sp_GetNotificationTokenID @UserID";

                    var deviceDetails = db.Connection.Query<DeviceDetails>(query, new { UserID = accountExecutiveID });

                    return deviceDetails;

                }
            }
            catch (Exception ex)
            {
                LogIt.WriteErrorLog(ex);
                return null;

            }
        }

        internal static void SendShortListNotification()
        {
            try
            {
                if (sendNotificationCount == 0)
                {
                    using (var db = new DatabaseContext())
                    {
                        const string query =
                                    @"EXEC sp_GetPushNotificationEntry_Details";

                        var notificationDetails = db.Connection.Query<NotificationData>(query);
                        int notificationCount = notificationDetails.Count();

                        if (notificationCount > 0)
                        {
                            sendNotificationCount = notificationCount;
                            BindNotification(notificationDetails);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogIt.WriteErrorLog(ex);
            }
        }

        internal async static void BindNotification(IEnumerable<NotificationData> notificationModel)
        {
            try
            {
                if (notificationModel != null)
                {
                    foreach (var item in notificationModel)
                    {
                        NotificationData data = new NotificationData();
                        data.AgreementID = item.AgreementID;
                        data.ActivityTransactionID = item.ActivityTransactionID;
                        data.ActivityTypeId = item.ActivityTypeId;
                        data.JobTitle = item.JobTitle;
                        data.AccountExecutiveID = item.AccountExecutiveID;
                        data.NotificationCreatedUserID = item.NotificationCreatedUserID;
                        data.NotificationCreaterEmail = item.NotificationCreaterEmail;
                        data.BranchName = item.BranchName;
                        data.CandidateID = item.CandidateID;
                        data.CandidateName = item.CandidateName;
                        data.NotificationCreaterUserName = item.NotificationCreaterUserName;
                        var deviceDetails = GetTokenID(item.AccountExecutiveID);

                        StringContent stringContent = null;

                        foreach (var deviceItem in deviceDetails)
                        {
                            if (deviceItem.DeviceType == "iOS")
                            {
                                var iOSNotificationData = SetPushNotificationContentForiOS(data, deviceItem);
                                stringContent = GetHttpContent(iOSNotificationData);
                            }
                            else
                            {
                                var androidNotificationData = SetPushNotificationContentForAndroid(data, deviceItem);
                                stringContent = GetHttpContent(androidNotificationData);
                            }

                           await TriggerNotification(stringContent, item.AgreementID, item.CandidateID, deviceItem.DeviceType);
                        }
                        sendNotificationCount--;
                        UpdateNotificationStatus(item.AgreementID, item.CandidateID, status);
                    }
                }

            }
            catch (Exception ex)
            {
                LogIt.WriteErrorLog(ex);
            }
        }

        private async static Task TriggerNotification(StringContent stringContent, int agreementID, int candidateID, string deviceType)
        {
            try
            {
                var client = new HttpClient();

                var serverKey = string.Format("key={0}", ConfigurationManager.AppSettings["ServerKey"].ToString());

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", serverKey);

                stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                string uri = ConfigurationManager.AppSettings["NotificationURL"].ToString();
                status = 0;
                HttpResponseMessage response = client.PostAsync(uri, stringContent).Result;
                var result = response.Content.ReadAsStringAsync();                
                if (response.IsSuccessStatusCode)
                    status = 1;                
            }
            catch (Exception ex)
            {
                status = 0;
                LogIt.WriteErrorLog(ex);               
            }
        }
    }

    #endregion
}

