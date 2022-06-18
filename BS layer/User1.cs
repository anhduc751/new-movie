using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Movie_management.Du_lieu;
using System.Reflection;


namespace Movie_management
{
	class User1
	{
        DB db = new DB();

        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table = new DataTable();

        public bool login(string username, string password)
        {
            /* SqlCommand command = new SqlCommand("SELECT username, password FROM [User] WHERE username = @username and password = @password", db.GetConnection);
             command.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
             command.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
             SqlDataAdapter sda = new SqlDataAdapter(command);
             DataTable dt = new DataTable();
             sda.Fill(dt);
             if (dt.Rows.Count > 0)
             {
                 db.closeConnection();
                 return true;
             }
             else
             {
                 db.closeConnection();
                 return false;
             }*/
            Movie_ticket_managementDataContext loginMV = new Movie_ticket_managementDataContext();
            var loginQr = from user in loginMV.Users
                          where user.username == username
                          && user.password == password
                          select user;
            if (loginQr.Any())
            {
                return true;
            }

            else 
            { 
                return false; 
            }
            
        }
        public void auth_register(string username, string password, string fullname,string address, DateTime year,string phonenumber,string email )
        {
            Movie_ticket_managementDataContext register = new Movie_ticket_managementDataContext();
            User iUsers = new User
            {
                username = username,
                password = password,
                fullname = fullname,
                address = address,
                phone = phonenumber,
                birthday = Convert.ToDateTime(year),
                email = email,
                balance = 0,
                role_code = "USER"
            };
            register.Users.InsertOnSubmit(iUsers);
            register.SubmitChanges();
        }

        public string getrole(string username)
        {
            /*string rolename = "";
            SqlCommand command = new SqlCommand("SELECT role_code FROM [User] WHERE username = @username", db.GetConnection);
            command.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
            db.openConnection();
            SqlDataReader sda = command.ExecuteReader();
            while (sda.Read())
            {
                rolename = sda.GetString(0);
            }
            return rolename;*/
            Movie_ticket_managementDataContext loginMV = new Movie_ticket_managementDataContext();
            var role = (from Users in loginMV.Users
                       where
                         Users.username == username
                       select new
                       {
                           Users.role_code
                       }).Single();
            string a = role.role_code;

            return a;
            
        }

        public bool updateUserInfo(string username, string password, string fullname, string address, int phone, DateTime birthday, string email)
        {
            Movie_ticket_managementDataContext update = new Movie_ticket_managementDataContext();
            var queryUsers =
                from Users in update.Users
                where
                Users.username == username
                select Users;
            foreach (var Users in queryUsers)
            {
                Users.password = password;
                Users.fullname = fullname;
                Users.address = address;
                Users.phone = phone.ToString();
                Users.birthday = birthday;
                Users.email = email;
            }
            if (queryUsers != null)
            {
                update.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public DataTable getUserInfoByName(string username)
        {
            Movie_ticket_managementDataContext info = new Movie_ticket_managementDataContext();
            var queryInfo =
                from Users in info.Users
                where
                  Users.username == username
                select new
                {
                    Users.password,
                    Users.fullname,
                    Users.address,
                    Users.phone,
                    Users.birthday,
                    Users.email
                };
            
            DataTable dt = LINQToDataTable(queryInfo);
            
            return dt;
        }

        public DataTable LINQToDataTable<t>(IEnumerable<t> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (t rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

    }
}
