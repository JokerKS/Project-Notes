﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Project;
using System.Windows.Threading;
using System.Timers;
using Project.Data;

namespace NotesProject.Data
{
    class ShowReminder
    {
        MainWindow form;

        List<Zadania> remList;
        DispatcherTimer timer;

        public ShowReminder(MainWindow f)
        {
            remList = new List<Zadania>();
            form = f;

            for (int i = 0; i < f.all_zadania.Count; i++)
            {
                if (f.all_zadania[i].Remind && f.all_zadania[i].Date > DateTime.Now)
                {
                    remList.Add(f.all_zadania[i]);
                }
            }
            if (remList.Count > 0)
            {
                remList = SortAscending(remList);
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
        }

        static List<Zadania> SortAscending(List<Zadania> list)
        {
            List<DateTime> l = new List<DateTime>();
            for (int i = 0; i < list.Count; i++)
                l.Add(list[i].Date);
            l.Sort((a, b) => a.CompareTo(b));


            List<Zadania> newlist = new List<Zadania>();
            for (int i = 0, j=0; i < list.Count && j < list.Count; i++)
            {
                if (l[j] == list[i].Date)
                {
                    newlist.Add(list[i]);
                    list.Remove(list[i]);
                    j++;
                    i = 0;
                }
            }
            return newlist;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (remList.Count > 0)
            {
                if (now.ToString() == remList[0].Date.ToString())
                {
                    CreateReminder(remList[0]);
                    remList.Remove(remList[0]);
                }
            }
            else timer.Stop();
        }

        private void CreateReminder(Zadania obj)
        {
            if (!obj.Window)
            {
                Window window = new Window();
                window.Content = obj.TextZadania;
                //window.Topmost = true;
                window.MaxHeight = 300;
                window.MaxWidth = 300;
                window.Show();
                obj.Window = true;
            }
        }
    }
}
