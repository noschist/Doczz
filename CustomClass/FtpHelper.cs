using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;

namespace Doczz.CustomClass
{
    class FtpHelper
    {

        SqlSetup sql = new SqlSetup();

        public List<string> GetDetails()
        {
            List<string> vs = new List<string>();
            DataTable dt = new DataTable();
            sql.LoadTable($"select * from Users, Hosts where Users.Email = Hosts.Email and Users.Email = '{Properties.Settings.Default.CurrentUser}';", ref dt);
            vs.Add(dt.Rows[0][0].ToString());
            vs.Add(dt.Rows[0][4].ToString());
            vs.Add(dt.Rows[0][5].ToString());
            vs.Add(dt.Rows[0][6].ToString());
            return vs;
        }
    }
}
