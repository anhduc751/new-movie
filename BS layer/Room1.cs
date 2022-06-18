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
    internal class Room1
    {
        //DB db = new DB();
        public DataTable getroomlist()
        {
            /*SqlCommand command = new SqlCommand("select roomid 'ID',name 'Name', num_of_seats 'Seats Amount' from [Room]", db.GetConnection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;*/
            Movie_ticket_managementDataContext Room = new Movie_ticket_managementDataContext();
            var query =
            from Rooms in Room.Rooms
            select new
            {
                Rooms.roomid,
                Rooms.name,
                Rooms.num_of_seats
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }
        public bool AddRoom(string name, int seat)
        {
            /*SqlCommand command = new SqlCommand("insert into Room(name,num_of_seat) values(@name, @seat)", db.GetConnection);
            command.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            command.Parameters.Add("@seat", SqlDbType.Int).Value = seat;

            db.openConnection();

            if(command.ExecuteNonQuery() == 1)
            {
                db.closeConnection();
                return true;
            }
            else
            {
                db.closeConnection();
                return false;
            }*/
            Movie_ticket_managementDataContext Room = new Movie_ticket_managementDataContext();
            Room iRooms = new Room()
            {
                name = name,
                num_of_seats = Convert.ToInt32(seat)
            };
            Room.Rooms.InsertOnSubmit(iRooms);
            if (name != null && seat.ToString() != null)
            {
                Room.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteRoom(int id)
        {
            /*SqlCommand command = new SqlCommand("Delete from Room where roomid = @id", db.GetConnection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            db.openConnection();

            if (command.ExecuteNonQuery() == 1)
            {
                db.closeConnection();
                return true;
            }
            else
            {
                db.closeConnection();
                return false;
            }*/
            Movie_ticket_managementDataContext Room = new Movie_ticket_managementDataContext();
            var queryRooms =
                from Rooms in Room.Rooms
                where
                    Rooms.roomid == id
                select Rooms;
                foreach (var del in queryRooms)
                {
                Room.Rooms.DeleteOnSubmit(del);
                }
            if (id.ToString() != null)
            {
                Room.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool UpdateRoom(int roomid, string name, int seat)
        {
            /*SqlCommand command = new SqlCommand("update Room set name = @name, num_of_seats = @seat where roomid = @roomid", db.GetConnection);
            command.Parameters.Add("@roomid", SqlDbType.Int).Value = roomid;
            command.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            command.Parameters.Add("@seat", SqlDbType.Int).Value = seat;

            db.openConnection();

            if (command.ExecuteNonQuery() == 1)
            {
                db.closeConnection();
                return true;
            }
            else
            {
                db.closeConnection();
                return false;
            }*/
            Movie_ticket_managementDataContext Room = new Movie_ticket_managementDataContext();
            var queryRooms =
                from Rooms in Room.Rooms
                where
                Rooms.roomid == roomid
                select Rooms;
            foreach (var Rooms in queryRooms)
            {
                Rooms.name = name;
                Rooms.num_of_seats = seat;
            }
            if (queryRooms.Any())
            {
                Room.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }

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
