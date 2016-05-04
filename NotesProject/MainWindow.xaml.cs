using Microsoft.Win32;
using Project.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitMainMenu();
            InitializeNotatnik();
        }



        #region Робота з блокнотом

        mYFile f;

        private void InitializeNotatnik()
        {
            f = new mYFile();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            f.SaveAsToFile(text.Text);
            string file_name = f.ShortFileName;
            if (file_name!=null)
                MainFile.Header = file_name;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            f.SaveToFile(text.Text);
            string file_name = f.ShortFileName;
            if (file_name != null)
                MainFile.Header = file_name;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string my_text = f.OpenFromFile();
            string file_name = f.ShortFileName;
            if (my_text != null)
                text.Text = my_text;
            if (file_name != null)
                MainFile.Header = file_name;
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            f.RemoveFileName();
            MainFile.Header = "JKSnot.jks";
            text.Clear();
        }
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            text.SelectAll();
        }
        private void SelectCurrentLine_Click(object sender, RoutedEventArgs e)
        {
            int lineIndex = text.GetLineIndexFromCharacterIndex(text.CaretIndex);
            int lineStartingCharIndex = text.GetCharacterIndexFromLineIndex(lineIndex);
            int lineLength = text.GetLineLength(lineIndex);
            text.Select(lineStartingCharIndex, lineLength);
        }
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            text.Paste();
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            text.Copy();
        }
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            text.Cut();
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            text.Redo();
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            text.Undo();
        }
        #endregion

        #region Робота з списком завдань
        int number=0;
        List<Zadania> all_zadania = new List<Zadania>();
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            all_zadania.Add(new Zadania());
            all_zadania[number].Date = Convert.ToDateTime(DateZadania.SelectedDate);

            CreateTask((byte)number);

            number++;

            SaveToFile();
        }
        private void DeleteTask_Click(object sender, EventArgs e)
        {
            //znajdujemy numer zadania
            int numer_zadania = Convert.ToInt32((sender as Button).Name.Remove(0, 5));
            all_zadania.RemoveAt(numer_zadania);
            number--;
            Redraw();
            SaveToFile();
        }
        private void Redraw()
        {
            GridForZadania.Children.Clear();
            GridForZadania.RowDefinitions.Clear();
            for (int i = 0; i < all_zadania.Count; i++)
                CreateTask((byte)i);
        }
        private void CreateTask(byte count)
        {
            RowDefinition row = new RowDefinition();
            row.Height = GridLength.Auto;
            GridForZadania.RowDefinitions.Add(row);

            Grid gr = new Grid();
            gr.Name = "grid" + count;
            gr.Margin = new Thickness(5, 5, 5, 5);
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            ColumnDefinition c3 = new ColumnDefinition();
            ColumnDefinition c4 = new ColumnDefinition();
            ColumnDefinition c5 = new ColumnDefinition();
            c1.Width = GridLength.Auto;
            c2.Width = GridLength.Auto;
            c3.Width = new GridLength(1, GridUnitType.Star);
            c4.Width = GridLength.Auto;
            c5.Width = GridLength.Auto;
            gr.ColumnDefinitions.Add(c1);
            gr.ColumnDefinitions.Add(c2);
            gr.ColumnDefinitions.Add(c3);
            gr.ColumnDefinitions.Add(c4);
            gr.ColumnDefinitions.Add(c5);

            Button btn = new Button();
            btn.Name = "button" + count;
            int status = all_zadania[count].CurrentStatus;
            if (status == 0)
            {
                btn.Foreground = Brushes.Blue;
                btn.Content = " ... ";
            }
            else if (status == 1)
            {
                btn.Foreground = Brushes.Green;
                btn.Content = " V ";
            }
            else if (status == 2)
            {
                btn.Foreground = Brushes.Red;
                btn.Content = " X ";
            }
            btn.MinWidth = 35;
            btn.Margin = new Thickness(0, 0, 5, 0);
            btn.Click += new RoutedEventHandler(Button_Status_Click);
            Grid.SetColumn(btn, 0);
            gr.Children.Add(btn);

            Image img = new Image();
            img.Name = "img" + count;
            img.MinHeight = img.MinWidth = 40;
            img.MaxHeight = img.MaxWidth = 100;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("Data\\Images\\image.png", UriKind.Relative);
            bi.EndInit();

            img.Source = bi;
            Grid.SetColumn(img, 1);
            gr.Children.Add(img);

            TextBox txt = new TextBox();
            txt.Name = "txt" + count;
            txt.Margin = new Thickness(5, 0, 5, 0);
            txt.DataContext = all_zadania[count];
            var binding = new Binding("TextZadania");
            binding.Mode = BindingMode.TwoWay;
            txt.SetBinding(TextBox.TextProperty, binding);
            txt.LostFocus += new RoutedEventHandler(Text_LostFocus);

            Grid.SetColumn(txt, 2);
            gr.Children.Add(txt);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.Margin = new Thickness(5, 0, 5, 0);

            TextBlock d = new TextBlock();
            d.Text = "Data: " + all_zadania[count].Date.ToShortDateString();
            sp.Children.Add(d);

            TextBlock d2 = new TextBlock();
            d2.Text = "Przypominać: ";
            sp.Children.Add(d2);

            CheckBox ch = new CheckBox();
            ch.Name = "chek" + count;
            ch.Checked += new RoutedEventHandler(CheckedChanged);
            ch.Unchecked += new RoutedEventHandler(UnCheckedChanged);
            if (all_zadania[count].Remind)
                ch.Content = "Tak";
            else ch.Content = "Nie";
            ch.DataContext = all_zadania[count];
            binding = new Binding("Remind");
            ch.SetBinding(CheckBox.IsCheckedProperty, binding);
            sp.Children.Add(ch);

            Grid.SetColumn(sp, 3);
            gr.Children.Add(sp);

            Button btn_X = new Button();
            btn_X.Name = "btn_X" + count;
            btn_X.Foreground = Brushes.Red;
            btn_X.Content = "X";
            btn_X.Margin = new Thickness(0, 0, 5, 0);
            btn_X.Click += new RoutedEventHandler(DeleteTask_Click);
            Grid.SetColumn(btn_X, 4);
            gr.Children.Add(btn_X);

            Grid.SetRow(gr, count);
            GridForZadania.Children.Add(gr);
        }

        private void Button_Status_Click(object sender, EventArgs e)
        {
            int numer_zadania = Convert.ToInt32((sender as Button).Name.Remove(0, 6));
            all_zadania[numer_zadania].ChangeStatus();
            numer_zadania = all_zadania[numer_zadania].CurrentStatus;
            if(numer_zadania == 0)
            {
                (sender as Button).Foreground = Brushes.Blue;
                (sender as Button).Content = " ... ";
            }
            else if(numer_zadania == 1)
            {
                (sender as Button).Foreground = Brushes.Green;
                (sender as Button).Content = " V ";
            }
            else if(numer_zadania == 2)
            {
                (sender as Button).Foreground = Brushes.Red;
                (sender as Button).Content = " X ";
            }
            SaveToFile();
        }

        private void CheckedChanged(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).Content = "Tak";
        }
        private void UnCheckedChanged(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).Content = "Nie";
        }
        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        private void SaveToFile()
        {
            FileInfo file = new FileInfo("zadania" + DateZadania.SelectedDate.Value.Day + "_"
                + DateZadania.SelectedDate.Value.Month + "_" + DateZadania.SelectedDate.Value.Year + ".txt");
            if (file.Exists)
                file.Delete();
            if (all_zadania.Count > 0)
            {
                StreamWriter write_text = file.AppendText();
                for (int j = 0; j < all_zadania.Count; j++)
                {
                    write_text.WriteLine(all_zadania[j].TextZadania);
                    write_text.WriteLine(all_zadania[j].Date);
                    write_text.WriteLine(all_zadania[j].Remind);
                    write_text.WriteLine(all_zadania[j].CurrentStatus);
                    write_text.WriteLine();
                }
                write_text.Close();
            }
        }
        private void ReadForFile(string name)
        {
            number = 0;
            all_zadania.Clear();

            StreamReader streamReader = new StreamReader(name);

            while (!streamReader.EndOfStream)
            {
                all_zadania.Add(new Zadania());
                all_zadania[number].TextZadania = streamReader.ReadLine();
                all_zadania[number].Date = Convert.ToDateTime(streamReader.ReadLine());
                all_zadania[number].Remind = Convert.ToBoolean(streamReader.ReadLine());
                all_zadania[number].CurrentStatus = Convert.ToByte(streamReader.ReadLine());
                streamReader.ReadLine();
                number++;
            }
            streamReader.Close();

            Redraw();
        }

        private void DateZadania_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllPagesTabControl.SelectedIndex == 1)
            {
                FileInfo file = new FileInfo("zadania" + DateZadania.SelectedDate.Value.Day + "_"
                    + DateZadania.SelectedDate.Value.Month + "_" + DateZadania.SelectedDate.Value.Year + ".txt");
                if (!file.Exists)
                {
                    GridForZadania.Children.Clear();
                    GridForZadania.RowDefinitions.Clear();
                    all_zadania.Clear();
                    number = 0;
                }
                else ReadForFile(file.Name.ToString());
            }
        }
        private void AllPagesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllPagesTabControl.SelectedIndex == 1)
            {
                FileInfo file = new FileInfo("zadania" + DateZadania.SelectedDate.Value.Day + "_"
                    + DateZadania.SelectedDate.Value.Month + "_" + DateZadania.SelectedDate.Value.Year + ".txt");
                if (!file.Exists)
                {
                    GridForZadania.Children.Clear();
                    GridForZadania.RowDefinitions.Clear();
                    all_zadania.Clear();
                    number = 0;
                }
                else ReadForFile(file.Name.ToString());
            }
        }
        #endregion

        #region Start with Windows
        RegistryKey reg = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private void InitMainMenu()
        {
            if (reg.GetValue("Notes.exe") != null) YesBtn.IsChecked = true;
            else NoBtn.IsChecked = true;
        }
        private void StartWithWindows_YesClick(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("Notes.exe") == null)
            {
                reg.SetValue("Notes.exe", System.Reflection.Assembly.GetExecutingAssembly().Location);
                YesBtn.IsChecked = true;
                NoBtn.IsChecked = false;
            }
        }
        private void StartWithWindows_NoClick(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("Notes.exe") != null)
            {
                reg.DeleteValue("Notes.exe");
                YesBtn.IsChecked = false;
                NoBtn.IsChecked = true;
            }
        }
        #endregion


        private MediaPlayer player = new MediaPlayer();
        private string mp3_filename;

        private void TestMP3_Click(object sender, RoutedEventArgs e)
        {
            if (MusicLocationText.Text!="")
            {
                if (TestMP3btn.Content.ToString() == "Test MP3")
                {
                    player.Open(new Uri(mp3_filename, UriKind.Relative));
                    player.Play();
                    TestMP3btn.Content = " ■ ";
                }
                else if(TestMP3btn.Content.ToString() == " ■ ")
                {
                    player.Stop();
                    TestMP3btn.Content = "Test MP3";
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Najpierw wybierz plik MP3!");
            }
        }

        private void ChooseMP3_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".mp3";
            ofd.Filter = "mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*"; 
            if (ofd.ShowDialog() == true)
            {
                mp3_filename = ofd.FileName;
                MusicLocationText.Text = ofd.SafeFileName;
            }
        }
    }
}
