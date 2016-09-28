using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyCinema.Models
{
    public class Cinema
    {
        public Cinema()
        {
            seats = new Dictionary<string, Seat>();
            soldTickets = new List<Ticket>();
            schedule = new Schedule();
        }
        /// <summary>
        /// 放映厅座位集合
        /// </summary>
        private Dictionary<string, Seat> seats;
        public Dictionary<string, Seat> Seats
        {
            get { return seats; }
            set { seats = value; }
        }

        private Schedule schedule;
        /// <summary>
        /// 当天的放映计划
        /// </summary>
        public Schedule Schedule
        {
            get { return schedule; }
            set { schedule = value; }
        }

        private List<Ticket> soldTickets;
        public List<Ticket> SoldTickets
        {
            get { return soldTickets; }
            set { soldTickets = value; }
        }

        public void Save()
        {
            FileStream fs = new FileStream("soldTickets.bin", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SoldTickets);
            fs.Close();
        }

        public void Load()
        {
            try
            {
                FileStream fs = new FileStream("soldTickets.bin", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                this.SoldTickets = (List<Ticket>)bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
                soldTickets = new List<Ticket>();
            }
        }
    }
}
