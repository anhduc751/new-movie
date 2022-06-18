using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Movie_management.Du_lieu;
using System.Reflection;

namespace Movie_management
{
    internal class ListMovie
    {
        DB db = new DB();

        public DataTable getMovieIdAndNameById(int movieid)
        {
            Movie_ticket_managementDataContext MovieI = new Movie_ticket_managementDataContext();
            var query =
            from Movies in MovieI.Movies
            where
              Movies.movieid == movieid
            select new
            {
                Movies.movieid,
                Movies.name
            };
            DataTable dt = LINQToDataTable(query);
            
            return dt;
        }

        /*public DataTable gettotalmovieamount()
        {

            SqlCommand command = new SqlCommand("select count(movieid) from Movie", db.GetConnection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }*/

        public DataTable getmovielist()
        {
            SqlCommand command = new SqlCommand("select movieid  'ID',name 'Movie',thumbnail 'Thumbnail', rating 'Rating', genre 'Genre', short_description 'Desciption', duration 'Duration' from [Movie]", db.GetConnection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }
        public DataTable GetMovieById(int id)
        {
            Movie_ticket_managementDataContext MovieI = new Movie_ticket_managementDataContext();
            var query =
            from Movies in MovieI.Movies
            where
              Movies.movieid == id
            select new
            {
                ID = Movies.movieid,
                Movie = Movies.name,
                Thumbnail = Movies.thumbnail,
                Rating = Movies.rating,
                Genre = Movies.genre,
                Desciption = Movies.short_description,
                Duration = Movies.duration
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }
        public DataTable GetMovieByName(string name)
        {
            Movie_ticket_managementDataContext MovieI = new Movie_ticket_managementDataContext();
            var query =
            from Movies in MovieI.Movies
            where
              Movies.name.Contains(name)
            select new
            {
                ID = Movies.movieid,
                Movie = Movies.name,
                Thumbnail = Movies.thumbnail,
                Rating = Movies.rating,
                Genre = Movies.genre,
                Desciption = Movies.short_description,
                Duration = Movies.duration
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
