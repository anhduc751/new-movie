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
    internal class Schedule1
    {
        DB db = new DB();
        
        public DataTable getScheduleList()
        {

            /*SqlCommand cmd = new SqlCommand("Select * from Schedule", db.GetConnection);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;*/
            Movie_ticket_managementDataContext schedule = new Movie_ticket_managementDataContext();
            var query =
            from Schedules in schedule.Schedules
            select new
            {
                scheduleid = Schedules.scheduleid,
                start_time = Schedules.start_time,
                end_time = Schedules.end_time,
                movie_id = Schedules.movie_id,
                room_id = Schedules.room_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }
        
        public DataTable getScheduleById(int id)
        {
            /*SqlCommand cmd = new SqlCommand("Select start_time, end_time, movie_id, room_id from Schedule where scheduleid = @id", db.GetConnection);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;*/
            Movie_ticket_managementDataContext schedule = new Movie_ticket_managementDataContext();
            var query =
            from Schedules in schedule.Schedules
            where
              Schedules.scheduleid == id
            select new
            {
                Schedules.start_time,
                Schedules.end_time,
                Schedules.movie_id,
                Schedules.room_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }
        public void addSchedule(int scheduleid, DateTime start, DateTime end, int movieId, int roomId)
        {
            string a = start.ToString();
            string b = end.ToString();

            /*SqlCommand command = new SqlCommand("INSERT INTO Schedule (scheduleid, start_time, end_time, movie_id, room_id) VALUES (@scheduleid, @start, @end, @movieId, @roomId)", db.GetConnection);
            command.Parameters.Add("@scheduleid", SqlDbType.Int).Value = scheduleid;
            command.Parameters.Add("@start", SqlDbType.DateTime).Value = start;
            command.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
            command.Parameters.Add("@movieId", SqlDbType.Int).Value = movieId;
            command.Parameters.Add("@roomId", SqlDbType.Int).Value = roomId;


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
            Movie_ticket_managementDataContext schedule = new Movie_ticket_managementDataContext();
            Schedule iSchedules = new Schedule
            {
                scheduleid = scheduleid,
                start_time = start,
                end_time = end,
                movie_id = movieId,
                room_id = roomId
            };
            schedule.Schedules.InsertOnSubmit(iSchedules);
            
                schedule.SubmitChanges();
                
            
        }
        public bool updateSchedule(int scheduleid, DateTime start, DateTime end, int movieId, int roomId)
        {
            SqlCommand command = new SqlCommand("UPDATE Schedule SET start_time = @start, end_time = @end, movie_id = @movieId " +
                ", room_id = @roomId WHERE scheduleid = @scheduleid", db.GetConnection);
            command.Parameters.Add("@scheduleid", SqlDbType.Int).Value = scheduleid;
            command.Parameters.Add("@start", SqlDbType.DateTime).Value = start;
            command.Parameters.Add("@end", SqlDbType.DateTime).Value = end;
            command.Parameters.Add("@movieId", SqlDbType.Int).Value = movieId;
            command.Parameters.Add("@roomId", SqlDbType.Int).Value = roomId;

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
            }
        }
        public void deleteSchedule(int id)
        {
            /*SqlCommand command = new SqlCommand("DELETE FROM Schedule WHERE scheduleid = @id", db.GetConnection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
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
            Movie_ticket_managementDataContext schedule = new Movie_ticket_managementDataContext();
            var querySchedules =
                from Schedules in schedule.Schedules
                where
                Schedules.scheduleid == id
                select Schedules;
            foreach (var del in querySchedules)
            {
                schedule.Schedules.DeleteOnSubmit(del);
            }
            schedule.SubmitChanges();


        }

        public DataTable getScheduleListByMovieId(int movie_id)
        {
            /*SqlCommand cmd = new SqlCommand("select scheduleid, start_time, end_time, room_id from Schedule where movie_id = @movie_id", db.GetConnection);
            cmd.Parameters.Add("@movie_id", SqlDbType.Int).Value = movie_id;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;*/
            Movie_ticket_managementDataContext schedule = new Movie_ticket_managementDataContext();
            var query =
            from Schedules in schedule.Schedules
            where
              Schedules.movie_id == movie_id
            select new
            {
                Schedules.scheduleid,
                Schedules.start_time,
                Schedules.end_time,
                Schedules.room_id
            };
            DataTable dt = LINQToDataTable(query);
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
