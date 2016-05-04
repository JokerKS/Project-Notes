using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project.Data
{
    class Zadania
    {

        string text_zadania;
        DateTime date;
        Image img;

        enum Status:byte {InProgress=0, Completed=1, Wasted=2 };
        byte current_status;

        string time;

        bool remind;

        public Zadania()
        {
            text_zadania = "";
            current_status = (byte)Status.InProgress;
            remind = false;
        }

        public string TextZadania
        {
            get { return text_zadania; }
            set { text_zadania = value; }
        }
        public byte CurrentStatus
        {
            get { return current_status; }
            set { current_status = value; }
        }
        public void ChangeStatus()
        {
            if (current_status == (byte)Status.InProgress)
                current_status = (byte)Status.Completed;
            else if(current_status == (byte)Status.Completed)
                current_status = (byte)Status.Wasted;
            else current_status = (byte)Status.InProgress;
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        public bool Remind
        {
            get { return remind; }
            set
            {
                remind = value;
            }
        }
    }
}
