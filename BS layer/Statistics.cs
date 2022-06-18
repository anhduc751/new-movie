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

    class Statistics
    {
        DB db = new DB();

        public DataTable getStatisticsList()
        {

            Movie_ticket_managementDataContext Stat = new Movie_ticket_managementDataContext();
            var query =
            from Statistics in Stat.Statistics
            select new
            {
                statisticsid = Statistics.statisticsid,
                movie = Statistics.movie,
                revenue = Statistics.revenue
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getStatisticsTotalId()
        {
            Movie_ticket_managementDataContext Stat = new Movie_ticket_managementDataContext();
            var query =
            from Statistics in
                (from Statistics in Stat.Statistics
                select new
                 {
                Statistics.statisticsid,
                    Dummy = "x"
                  })
            group Statistics by new { Statistics.Dummy } into g
            select new
            {
                Column1 = g.Count(p => p.statisticsid.ToString() != null)
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getTotalRevenueByMovieId(int movie_id)
        {
            SqlCommand command = new SqlCommand("select sum(price) from Ticket where movie_id = @movie_id", db.GetConnection);
            command.Parameters.Add("@movie_id", SqlDbType.Int).Value = movie_id;
            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }

        public bool updateTotalRevenueByMovieId(int statisticsid, int revenue)
        {
            Movie_ticket_managementDataContext Stat = new Movie_ticket_managementDataContext();
            var queryStatistics =
                from Statistics in Stat.Statistics
                where
                Statistics.statisticsid == statisticsid
                 select Statistics;
            foreach (var Statistics in queryStatistics)
            {
                Statistics.revenue = revenue;
            };
            Stat.SubmitChanges();


            if (queryStatistics.Any())
            {
                
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
