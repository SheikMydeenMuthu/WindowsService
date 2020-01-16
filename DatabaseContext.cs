using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationService
{
    public class DatabaseContext : IDisposable
    {
        public DatabaseContext()
        {

        }
        private IDbConnection _connection;
        public IDbConnection Connection { get { return _connection ?? (_connection = CreateConnection()); } }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"].ToString());
        }

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        #endregion

    }
}
