using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.Configuration;

namespace ProyectoDSWI.DataBase
{
    public class DBAccess
    {
        public static SqlConnection getConecta()
        {
            SqlConnection cn = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["Business1"].ConnectionString);

            return cn;
        }
    }
}