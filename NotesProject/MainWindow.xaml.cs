using Microsoft.Win32;
using NotesProject.Data;
using Project.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

namespace Project
{
    /// <summary>
    /// Основне вікно програми, яке наслідуване від класу Window
    /// </summary>
    public partial class MainWindow : Window
    {
        // Підсказка
        ToolTip toolTipBtnX;
        // Змінна типу mYFile для роботи з збереженням, шифруванням, дешифруванням
        mYFile file;
        // Ліста для збереження всіх завдань користувача
        public List<Zadania> all_zadania = new List<Zadania>();
        // Для роботи з треєм
        private System.Windows.Forms.NotifyIcon TrayIcon = null;
        private ContextMenu TrayMenu = null;
        // Флаг, який визначає чи можна вийти з програми
        private bool fCanClose = false;

        /// <summary>
        /// Конструктор вікна, в якому ініціалізуються його елементи
        /// а також створюються інші запрограмовані елементи
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Створення об'єкту mYFile для подальшої роботи з ним
            file = new mYFile();
            // Функція, яка визначає значення для меню - start with windows
            InitMainMenu();
            // Створення трей іконки
            createTrayIcon();

            InitialReminder();

            // Підсказка для нагадувань
            toolTipBtnX = new ToolTip();
            toolTipBtnX.Background = Brushes.White;
            toolTipBtnX.Content = "Usuń zadanie";
        }

        #region Робота з блокнотом
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            file.SaveAsToFile(text.Text);
            string file_name = file.ShortFileName;
            if (file_name != null)
                MainFile.Header = file_name;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            file.SaveToFile(text.Text);
            string file_name = file.ShortFileName;
            if (file_name != null)
                MainFile.Header = file_name;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string my_text = file.OpenFromFile();
            string file_name = file.ShortFileName;
            if (my_text != null)
                text.Text = my_text;
            if (file_name != null)
                MainFile.Header = file_name;
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            file.RemoveFileName();
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
        int number = 0;
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
            txt.TextWrapping = TextWrapping.Wrap;
            txt.DataContext = all_zadania[count];
            var binding = new Binding("TextZadania");
            binding.Mode = BindingMode.TwoWay;
            txt.SetBinding(TextBox.TextProperty, binding);
            txt.LostFocus += new RoutedEventHandler(Text_LostFocus);

            ToolTip toolTip = new ToolTip();
            toolTip.Background = Brushes.White;
            txt.ToolTip = toolTip;

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
            ch.DataContext = all_zadania[count];
            sp.Children.Add(ch);

            ToolTip toolTip3 = new ToolTip();
            toolTip3.Background = Brushes.White;

            if (all_zadania[count].Remind)
            {
                ch.Content = "Tak";
                ch.IsChecked = true;

                toolTip3.Content = "Wyłączyć przypomnienie";
                ch.ToolTip = toolTip3;

                toolTip.Content = "Tekst zadania i przypomnienie";

                DateTimePicker dtpick = new DateTimePicker();
                dtpick.Name = "time" + count;
                dtpick.Format = DateTimeFormat.LongTime;
                dtpick.ShowDropDownButton = false;
                dtpick.Text = all_zadania[count].Time.ToString();
                dtpick.FontSize = 20;
                dtpick.LostFocus += new RoutedEventHandler(TimePicker_LostFocus);
                sp.Children.Add(dtpick);
            }
            else
            {
                ch.Content = "Nie";
                ch.IsChecked = false;

                toolTip3.Content = "Włączyć przypomnienie";
                ch.ToolTip = toolTip3;
                toolTip.Content = "Tekst zadania";
            }
            ch.Checked += new RoutedEventHandler(CheckedChanged);
            ch.Unchecked += new RoutedEventHandler(UnCheckedChanged);

            Grid.SetColumn(sp, 3);
            gr.Children.Add(sp);

            Button btn_X = new Button();
            btn_X.Name = "btn_X" + count;
            btn_X.Foreground = Brushes.Red;
            btn_X.Content = "X";
            btn_X.Margin = new Thickness(0, 0, 5, 0);
            btn_X.Click += new RoutedEventHandler(DeleteTask_Click);
            btn_X.ToolTip = toolTipBtnX;

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
            if (numer_zadania == 0)
            {
                (sender as Button).Foreground = Brushes.Blue;
                (sender as Button).Content = " ... ";
            }
            else if (numer_zadania == 1)
            {
                (sender as Button).Foreground = Brushes.Green;
                (sender as Button).Content = " V ";
            }
            else if (numer_zadania == 2)
            {
                (sender as Button).Foreground = Brushes.Red;
                (sender as Button).Content = " X ";
            }
            SaveToFile();
        }

        private void CheckedChanged(object sender, RoutedEventArgs e)
        {
            int numer_zadania = Convert.ToInt32((sender as CheckBox).Name.Remove(0, 4));
            all_zadania[numer_zadania].Remind = true;
            SaveToFile();
            Redraw();
        }
        private void UnCheckedChanged(object sender, RoutedEventArgs e)
        {
            int numer_zadania = Convert.ToInt32((sender as CheckBox).Name.Remove(0, 4));
            all_zadania[numer_zadania].Remind = false;
            all_zadania[numer_zadania].Time = new TimeSpan(0,0,0);
            SaveToFile();
            Redraw();
        }
        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        ShowReminder shrem;
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
                    write_text.WriteLine(all_zadania[j].Time);
                    write_text.WriteLine(all_zadania[j].CurrentStatus);
                    write_text.WriteLine();
                }
                write_text.Close();
                shrem = new ShowReminder(this);
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
                all_zadania[number].Time = TimeSpan.Parse(streamReader.ReadLine());
                all_zadania[number].CurrentStatus = Convert.ToByte(streamReader.ReadLine());
                streamReader.ReadLine();
                number++;
            }
            streamReader.Close();
            shrem = new ShowReminder(this);
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
        private void TimePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            int numer_zadania = Convert.ToInt32((sender as DateTimePicker).Name.Remove(0, 4));
            all_zadania[numer_zadania].Time = TimeSpan.Parse((sender as DateTimePicker).Text);
            SaveToFile();
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

        #region Робота з нагадуваннями
        private void InitialReminder()
        {
            string datetimenow = "zadania" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" +
                DateTime.Now.Year + ".txt";
            FileInfo file = new FileInfo(datetimenow);
            if (file.Exists)
            {
                ReadForFile(file.Name.ToString());
            }
        }

        private MediaPlayer player = new MediaPlayer();
        private string mp3_filename;
        DispatcherTimer dispatcherTimer;

        private void TestMP3_Click(object sender, RoutedEventArgs e)
        {
            if (MusicLocationText.Text != "")
            {
                if (TestMP3btn.Content.ToString() == "Test MP3")
                {
                    player.Open(new Uri(mp3_filename, UriKind.Relative));
                    player.Play();
                    TestMP3btn.Content = " ■ ";
                }
                else if (TestMP3btn.Content.ToString() == " ■ ")
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (mp3_filename != null)
            {
                if (DateTime.Now.ToLongTimeString() == TimePicker.Text.ToString())
                {
                    player.Play();
                }
            }
        }

        #endregion

        #region Робота з треєм
        private bool createTrayIcon()
        {
            bool result = false;
            if (TrayIcon == null)
            {
                TrayIcon = new System.Windows.Forms.NotifyIcon();
                TrayIcon.Icon = NotesProject.Properties.Resources.TreyIcon;
                TrayIcon.Text = "Ku-ku";
                // а здесь уже ресурсы окна и тот самый x:Key
                TrayMenu = Resources["TrayMenu"] as ContextMenu; 

                // сразу же опишем поведение при щелчке мыши, о котором мы говорили ранее
                // это будет просто анонимная функция, незачем выносить ее в класс окна
                TrayIcon.Click += delegate (object sender, EventArgs e)
                {
                    if ((e as System.Windows.Forms.MouseEventArgs).Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        // по левой кнопке показываем или прячем окно
                        ShowHideMainWindow(sender, null);
                    }
                    else {
                        // по правой кнопке (и всем остальным) показываем меню
                        TrayMenu.IsOpen = true;
                        // нужно отдать окну фокус
                        Activate(); 
                    }
                };
                result = true;
            }
            else result = true;
            // делаем иконку видимой в трее
            TrayIcon.Visible = true; 
            return result;
        }

        private void ShowHideMainWindow(object sender, RoutedEventArgs e)
        {
            // спрячем менюшку, если она вдруг видима
            TrayMenu.IsOpen = false; 
            if (IsVisible)
            {
                Hide();
                (TrayMenu.Items[0] as MenuItem).Header = "Pokaż";
            }
            else
            { 
                Show();
                (TrayMenu.Items[0] as MenuItem).Header = "Ukryj";
                WindowState = CurrentWindowState;
                // обязательно нужно отдать фокус окну,
                // иначе пользователь сильно удивится, когда увидит окно
                // но не сможет в него ничего ввести с клавиатуры
                Activate(); 
            }
        }

        private System.Windows.WindowState fCurrentWindowState = System.Windows.WindowState.Normal;
        public System.Windows.WindowState CurrentWindowState
        {
            get { return fCurrentWindowState; }
            set { fCurrentWindowState = value; }
        }

        // переопределяем встроенную реакцию на изменение состояния сознания окна
        protected override void OnStateChanged(EventArgs e)
        {
            // системная обработка
            base.OnStateChanged(e); 
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                // если окно минимизировали, просто спрячем
                Hide();
                // и поменяем надпись на менюшке
                (TrayMenu.Items[0] as MenuItem).Header = "Pokaż";
            }
            else {
                // в противном случае запомним текущее состояние
                CurrentWindowState = WindowState;
            }
        }

        public bool CanClose
        { 
            get { return fCanClose; }
            set { fCanClose = value; }
        }

        // переопределяем обработчик запроса выхода из приложения
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // встроенная обработка
            base.OnClosing(e); 
            if (!CanClose)
            {   
                // если нельзя закрывать
                e.Cancel = true;
                // выставляем флаг отмены закрытия
                // запоминаем текущее состояние окна
                CurrentWindowState = this.WindowState;
                // меняем надпись в менюшке
                (TrayMenu.Items[0] as MenuItem).Header = "Pokaż";
                // прячем окно
                Hide();
            }
            else
            { 
                // убираем иконку из трея
                TrayIcon.Visible = false;
            }
        }

        private void MenuExitClick(object sender, RoutedEventArgs e)
        {
            CanClose = true;
            Close();
        }
        #endregion

    }
}
