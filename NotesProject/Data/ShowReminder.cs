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
using Project;
using System.Windows.Threading;
using System.Timers;
using Project.Data;
using System.ComponentModel;

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
            for (int i = 0, j=0; i < list.Count && j < l.Count;)
            {
                if (l[j] == list[i].Date)
                {
                    newlist.Add(list[i]);
                    list.Remove(list[i]);
                    j++;
                    i = 0;
                }
                else i++;
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

        List<Window> openWindow = new List<Window>();
        List<Zadania> openZadania = new List<Zadania>();
        private void CreateReminder(Zadania obj)
        {
            if (!obj.Window)
            {
                Window window = new Window();
                window.MaxHeight = 170;
                window.MaxWidth = 500;
                window.Width = 500;
                window.Height = 170;
                window.Title = "Przypomnienie";
                var desktopWorkingArea = SystemParameters.WorkArea;
                window.Left = desktopWorkingArea.Right - window.Width;
                window.Top = desktopWorkingArea.Bottom - window.Height;
                window.Closing += new CancelEventHandler(Window_Closing);

                openWindow.Add(window);
                openZadania.Add(obj);

                Grid gr = new Grid();
                gr.HorizontalAlignment = HorizontalAlignment.Stretch;
                gr.VerticalAlignment = VerticalAlignment.Stretch;
                gr.Margin = new Thickness(10,5,10,10);

                ColumnDefinition c1 = new ColumnDefinition();
                ColumnDefinition c2 = new ColumnDefinition();
                ColumnDefinition c3 = new ColumnDefinition();
                c1.Width = GridLength.Auto;
                c2.Width = new GridLength(1, GridUnitType.Star);
                c3.Width = GridLength.Auto;
                gr.ColumnDefinitions.Add(c1);
                gr.ColumnDefinitions.Add(c2);
                gr.ColumnDefinitions.Add(c3);

                RowDefinition r1 = new RowDefinition();
                RowDefinition r2 = new RowDefinition();
                r1.Height = new GridLength(4, GridUnitType.Star);
                r2.Height = new GridLength(2, GridUnitType.Star);
                gr.RowDefinitions.Add(r1);
                gr.RowDefinitions.Add(r2);

                Image img = new Image();
                img.Margin = new Thickness(5, 0, 0, 5);
                img.MinHeight = img.MinWidth = 40;
                img.MaxHeight = img.MaxWidth = 100;

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Data\\Images\\image.png", UriKind.Relative);
                bi.EndInit();

                img.Source = bi;
                Grid.SetColumn(img, 0);
                Grid.SetRow(img, 0);
                gr.Children.Add(img);

                TextBox text = new TextBox();
                text.Margin = new Thickness(5, 0, 0, 5);
                text.HorizontalAlignment = HorizontalAlignment.Stretch;
                text.Margin = new Thickness(0, 5, 0, 5);
                text.TextWrapping = TextWrapping.Wrap;
                text.Text = obj.TextZadania;
                Grid.SetColumn(text,1);
                Grid.SetRow(text, 0);
                gr.Children.Add(text);

                Button btnOK = new Button();
                btnOK.Margin = new Thickness(5, 0, 0, 5);
                btnOK.HorizontalAlignment = HorizontalAlignment.Stretch;
                btnOK.Content = "Dobrze";
                btnOK.Click += new RoutedEventHandler(CloseReminder_Click);
                Grid.SetColumn(btnOK, 1);
                Grid.SetRow(btnOK,1);
                gr.Children.Add(btnOK);

                TextBlock txt = new TextBlock();
                txt.Margin = new Thickness(5,0,0,5);
                txt.VerticalAlignment = VerticalAlignment.Center;
                txt.Text = "Data:\t" + obj.Date.ToShortDateString() + "\nCzas:\t" + obj.Date.ToLongTimeString();
                Grid.SetColumn(txt, 2);
                Grid.SetRow(txt, 0);
                gr.Children.Add(txt);

                Button btnDelay = new Button();
                btnDelay.Margin = new Thickness(5, 0, 0, 5);
                btnDelay.HorizontalAlignment = HorizontalAlignment.Stretch;
                btnDelay.Content = "Odłożyć na 5 minut";
                btnDelay.Click += new RoutedEventHandler(CloseAndDelay_Click);
                Grid.SetColumn(btnDelay, 2);
                Grid.SetRow(btnDelay, 1);
                gr.Children.Add(btnDelay);

                window.Content = gr;
                window.Show();
                obj.Window = true;
            }
        }
        private void CloseReminder_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < openWindow.Count; i++)
            {
                if ((Window)((Grid)((Button)sender).Parent).Parent == openWindow[i])
                {
                    openWindow.Remove(openWindow[i]);
                    openZadania[i].Window = false;
                    openZadania.Remove(openZadania[i]);
                    break;
                }
            }
            ((Window)((Grid)((Button)sender).Parent).Parent).Close();
        }

        private void CloseAndDelay_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < openWindow.Count; i++)
            {
                if ((Window)((Grid)((Button)sender).Parent).Parent == openWindow[i])
                {
                    openWindow.Remove(openWindow[i]);
                    openZadania[i].Window = false;
                    openZadania[i].AddTime(new TimeSpan(0,5,0));
                    remList.Add(openZadania[i]);
                    remList = SortAscending(remList);
                    if (remList.Count == 1) timer.Start();
                    openZadania.Remove(openZadania[i]);
                    break;
                }
            }
            ((Window)((Grid)((Button)sender).Parent).Parent).Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            for (int i = 0; i < openWindow.Count; i++)
            {
                if (sender == openWindow[i])
                {
                    openWindow.Remove(openWindow[i]);
                    openZadania[i].Window = false;
                    openZadania.Remove(openZadania[i]);
                    break;
                }
            }
        }
    }
}
