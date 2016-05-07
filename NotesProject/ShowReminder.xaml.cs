using System;
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
using Project.Data;
using Project;
using System.Windows.Threading;
using System.Timers;

namespace NotesProject
{
    public partial class ShowReminder : Window
    {
        MainWindow form;

        List<Zadania> remList;
        DispatcherTimer timer;

        public ShowReminder(MainWindow f)
        {
            InitializeComponent();
            remList = new List<Zadania>();
            form = f;

            for (int i = 0; i < f.all_zadania.Count; i++)
            {
                if (f.all_zadania[i].Remind && f.all_zadania[i].Time != null)
                {
                    remList.Add(f.all_zadania[i]);
                }
            }
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string datetime;
            string datenow = DateTime.Now.ToString();
            for (int i = 0; i < remList.Count; i++)
            {
                datetime = remList[i].Date.ToShortDateString() + " " + remList[i].Time;
                if (datenow == datetime)
                {
                    CreateReminder(remList[i]);
                }
            }
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
