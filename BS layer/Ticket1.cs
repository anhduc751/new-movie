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
    class Ticket1
    {
        DB db = new DB();

        public DataTable getTicketList()
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Tickets in Tick.Tickets
            select new
            {
                ticketid = Tickets.ticketid,
                price = Tickets.price,
                booking_date = Tickets.booking_date,
                schedule_id = Tickets.schedule_id,
                username = Tickets.username,
                seat_id = Tickets.seat_id,
                room_id = Tickets.room_id,
                movie_id = Tickets.movie_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getTicketListByUsername(string username)
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Tickets in Tick.Tickets
            where
              Tickets.username == username
            select new
            {
                Tickets.ticketid,
                Tickets.price,
                Tickets.booking_date,
                Tickets.schedule_id,
                Tickets.seat_id,
                Tickets.room_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getUsernameList()
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Users in Tick.Users
            select new
            {
                Users.username
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getSeatList()
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Seats in Tick.Seats
            select new
            {
                Seats.seatid
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getScheduleList()
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Schedules in Tick.Schedules
            select new
            {
                Schedules.scheduleid
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }


        public DataTable getTicketById(int tid)
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Tickets in Tick.Tickets
            where
              Tickets.ticketid == tid
            select new
            {
                Tickets.price,
                Tickets.booking_date,
                Tickets.schedule_id,
                Tickets.username,
                Tickets.seat_id,
                Tickets.room_id
            };
            DataTable dt = LINQToDataTable(query);
            return dt;
        }

        public DataTable getAvailableTicketIdWithPrice(int price)
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            (from Tickets in Tick.Tickets
             where
               Tickets.username == null &&
               Tickets.price == price
             select new
             {
                 ticketid = Tickets.ticketid,
                 price = Tickets.price,
                 booking_date = Tickets.booking_date,
                 schedule_id = Tickets.schedule_id,
                 username = Tickets.username,
                 seat_id = Tickets.seat_id,
                 room_id = Tickets.room_id,
                 movie_id = Tickets.movie_id
             }).Take(1);
            DataTable dt = LINQToDataTable(query);
            return dt;
        }


        public bool addTicket(int Tprice)
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            Ticket iTickets = new Ticket
            {
                price = Tprice
            };
            Tick.Tickets.InsertOnSubmit(iTickets);
            Tick.SubmitChanges();
            

            if (Tprice.ToString() != null)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool updateTicket(int ticketid, int price, DateTime booking_date, int schedule_id, string username, int seat_id, int room_id)
        {
            Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
            var query =
            from Tickets in Tick.Tickets
            where
              Tickets.ticketid == ticketid
            select Tickets;
            foreach (var Tickets in query)
            {
                Tickets.price = price;
                Tickets.booking_date = booking_date;
                Tickets.schedule_id = schedule_id;
                Tickets.username = username;
                Tickets.seat_id = seat_id;
                Tickets.room_id = room_id;
            }
            Tick.SubmitChanges();

            if (query.Any())
            {
                
                return true;
            }
            else
            {
                
                return false;
            }
        }

        /*public bool resetIncrement()
        {
            SqlCommand command = new SqlCommand("DECLARE @number INT; select @number = max(ticketid) from Ticket; DBCC CHECKIDENT ('Ticket', RESEED, @number)", db.GetConnection);
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
        }*/

        public bool bookTicket(int ticketid, int price, DateTime booking_date, int schedule_id, string username, int seat_id, int room_id, int movie_id)
        {
            {
                Movie_ticket_managementDataContext Tick = new Movie_ticket_managementDataContext();
                var queryTickets =
                from Tickets in Tick.Tickets
                where
                Tickets.ticketid == ticketid &&
                Tickets.price == price
                select Tickets;
                foreach (var Tickets in queryTickets)
                {
                    Tickets.booking_date = booking_date;
                    Tickets.schedule_id = schedule_id;
                    Tickets.username = username;
                    Tickets.seat_id = seat_id;
                    Tickets.room_id = room_id;
                    Tickets.movie_id = movie_id;
                }
                Tick.SubmitChanges();


                if (queryTickets.Any())
                {

                    return true;
                }
                else
                {

                    return false;
                }
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
