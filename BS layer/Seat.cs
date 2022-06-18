using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movie_management.Du_lieu;
using System.Reflection;


namespace Movie_management
{
    class Seat
    {
        DB db = new DB();

        public DataTable getSeatList()
        {
            Movie_ticket_managementDataContext Seat = new Movie_ticket_managementDataContext();
            var query =
            from Seats in Seat.Seats
            select new
            {
                seatid = Seats.seatid,
                code = Seats.code,
                kind = Seats.kind,
                status = Seats.status,
                room_id = Seats.room_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getSeatById(int seatid)
        {
            Movie_ticket_managementDataContext Seat = new Movie_ticket_managementDataContext();
            var query =
            from Seats in Seat.Seats
            where
              Seats.seatid == seatid
            select new
            {
                Seats.code,
                Seats.kind,
                Seats.status,
                Seats.room_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public bool updateSeat(int seatid, int code, string kind, string status, int room_id)
        {
            /*SqlCommand command = new SqlCommand("UPDATE Seat SET code = @code, kind = @kind, status = @status " +
                ", room_id = @room_id WHERE seatid = @seatid", db.GetConnection);
            command.Parameters.Add("@seatid", SqlDbType.Int).Value = seatid;
            command.Parameters.Add("@code", SqlDbType.Int).Value = code;
            command.Parameters.Add("@kind", SqlDbType.VarChar).Value = kind;
            command.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
            command.Parameters.Add("@room_id", SqlDbType.Int).Value = room_id;

            db.openConnection();

            if ((command.ExecuteNonQuery() == 1))
            {
                db.closeConnection();
                return true;
            }
            else
            {
                db.closeConnection();
                return false;
            }*/
            Movie_ticket_managementDataContext Seat = new Movie_ticket_managementDataContext();
            var querySeats =
                from Seats in Seat.Seats
                where
                Seats.seatid == seatid
                select Seats;
            foreach (var Seats in querySeats)
            {
                Seats.code = code.ToString();
                Seats.kind = kind;
                Seats.status = bool.Parse(status);
                Seats.room_id = room_id;
            }
            if (querySeats.Any())
            {
                Seat.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable getSeatListByRoomID(int roomid)
        {
            /*SqlCommand command = new SqlCommand("select seatid, kind from Seat where room_id = @roomid and status = 'False'", db.GetConnection);
            command.Parameters.Add("@roomid", SqlDbType.Int).Value = roomid;
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;*/
            Movie_ticket_managementDataContext Seat = new Movie_ticket_managementDataContext();
            var query =
            from Seats in Seat.Seats
            where
              Seats.room_id == roomid &&
              Convert.ToString(Seats.status) == "False"
            select new
            {
                Seats.seatid,
                Seats.kind
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getSeatKind(int seatid)
        {
            /*SqlCommand command = new SqlCommand("select kind from Seat where seatid = @seatid", db.GetConnection);
            command.Parameters.Add("@seatid", SqlDbType.Int).Value = seatid;
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;*/
            Movie_ticket_managementDataContext Seat = new Movie_ticket_managementDataContext();
            var query =
            from Seats in Seat.Seats
            where
              Seats.seatid == seatid
            select new
            {
                Seats.kind
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public bool bookSeat(int seatid)
        {
            /*SqlCommand command = new SqlCommand("UPDATE Seat SET status = 'True' WHERE seatid = @seatid", db.GetConnection);
            command.Parameters.Add("@seatid", SqlDbType.Int).Value = seatid;
            db.openConnection();

            if ((command.ExecuteNonQuery() == 1))
            {
                db.closeConnection();
                return true;
            }
            else
            {
                db.closeConnection();
                return false;
            }*/
            Movie_ticket_managementDataContext Seat = new Movie_ticket_managementDataContext();
            var querySeats =
                from Seats in Seat.Seats
                where
                Seats.seatid == seatid
                select Seats;
            foreach (var Seats in querySeats)
            {
                Seats.status = bool.Parse("True");
            }
            if (querySeats.Any())
            {
                Seat.SubmitChanges();
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
