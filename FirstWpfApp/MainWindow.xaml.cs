using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FirstWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection db;
        DateTime date;
        private int count = 0;
        private KeyValuePair<int, int> _emptyPosition = new KeyValuePair<int, int>(3, 3);
        DispatcherTimer timer = new DispatcherTimer();
        private bool newGame = false;
        public MainWindow()
        {
            InitializeComponent();
            lblTime.Content = "00:00:00";
            lblCounter.Content = "0";
        }

        void timer_Tick(object sender, EventArgs e)
        {
            long tick = DateTime.Now.Ticks - date.Ticks;
            DateTime stopWatch = new DateTime();


            stopWatch = stopWatch.AddTicks(tick);
            lblTime.Content = String.Format("{0:mm:ss:ff}", stopWatch);
        }

        private void MainButton_Click(object sender, RoutedEventArgs eve)
        {         
            var btn = sender as Button;
            Move(btn);
        }

        private void MenuNewGameButton_Click(object sender, RoutedEventArgs eve)
        {
            newGame = true;
            NewGame();
            newGame = false;
            date = DateTime.Now;
            timer.Interval = new TimeSpan(10);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        private void MenuExitButton_Click(object sender, RoutedEventArgs eve)
        {
            this.Close();
            db.Close();
        }

        private void Move(Button btn)
        {
            if ((int)btn.GetValue(Grid.RowProperty) == (int)_emptyPosition.Key)
            {
                if ((int)btn.GetValue(Grid.ColumnProperty) + 1 == _emptyPosition.Value ||
                    (int)btn.GetValue(Grid.ColumnProperty) - 1 == _emptyPosition.Value)
                {
                    var position = new KeyValuePair<int, int>((int)btn.GetValue(Grid.RowProperty), (int)btn.GetValue(Grid.ColumnProperty));
                    btn.SetValue(Grid.ColumnProperty, _emptyPosition.Value);
                    btn.SetValue(Grid.RowProperty, _emptyPosition.Key);
                    _emptyPosition = position;
                    count++;
                    lblCounter.Content = count;
                }
            }
            if ((int)btn.GetValue(Grid.ColumnProperty) == (int)_emptyPosition.Value)
            {
                if ((int)btn.GetValue(Grid.RowProperty) + 1 == _emptyPosition.Key ||
                    (int)btn.GetValue(Grid.RowProperty) - 1 == _emptyPosition.Key)
                {
                    var position = new KeyValuePair<int, int>((int)btn.GetValue(Grid.RowProperty), (int)btn.GetValue(Grid.ColumnProperty));
                    btn.SetValue(Grid.ColumnProperty, _emptyPosition.Value);
                    btn.SetValue(Grid.RowProperty, _emptyPosition.Key);
                    _emptyPosition = position;
                    count++;
                    lblCounter.Content = count;
                }
            }
            if (!newGame)
            {
                if (EndGame())
                {
                    timer.Stop();
                    using (db = new SQLiteConnection("Data Source = Records.db; Version = 3"))
                    {
                        db.Open();
                        SQLiteCommand command = db.CreateCommand();
                        command.CommandText = $"select BestTime, BestMoves from Records;";

                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var rec = new Record(reader["BestTime"].ToString(), reader["BestMoves"].ToString());
                                if (TimeSpan.Compare(TimeSpan.Parse(lblTime.Content.ToString()), TimeSpan.Parse(rec.BestTime)) == -1)
                                {
                                    reader.Close();
                                    command.CommandText = $"update Records set BestTime = '{lblTime.Content}' where BestTime = '{rec.BestTime}';";
                                    command.ExecuteNonQuery();
                                }
                                if (int.Parse(rec.BestMoves) > count)
                                {
                                    reader.Close();
                                    command.CommandText = $"update Records set BestMoves = '{count}' where BestMoves = '{rec.BestMoves}';";
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            reader.Close();
                            command.CommandText = $"INSERT INTO Records (BestTime, BestMoves) values ('{lblTime.Content.ToString()}', '{count}')";
                            command.ExecuteNonQuery();
                        }
                        db.Close();
                    }
                     MessageBox.Show($"You Win!\n Your time:{lblTime.Content.ToString()}\n Your moves:{count.ToString()}");
                }
            }
        }


        public struct Record
        {
            public Record(string best_time, string best_moves)
            {
                BestTime = best_time;
                BestMoves = best_moves;
            }
            public string BestTime { get; set; }
            public string BestMoves { get; set; }
        }

        private void NewGame()
        {
            List<Button> list_button = new List<Button>();
            list_button.Add(btn1);
            list_button.Add(btn2);
            list_button.Add(btn3);
            list_button.Add(btn4);
            list_button.Add(btn5);
            list_button.Add(btn6);
            list_button.Add(btn7);
            list_button.Add(btn8);
            list_button.Add(btn9);
            list_button.Add(btn10);
            list_button.Add(btn11);
            list_button.Add(btn12);
            list_button.Add(btn13);
            list_button.Add(btn14);
            list_button.Add(btn15);
            Random rnd = new Random();
            for(int i = 0; i < 500; i++)
            {
                MainButton_Click((list_button[rnd.Next(14)] as object), null);
                count = 0;
                lblCounter.Content = count;
            }          
        }

        private bool EndGame()
        {
            if ((int)btn1.GetValue(Grid.RowProperty) == 0 && (int)btn1.GetValue(Grid.ColumnProperty) == 0 &&
                (int)btn2.GetValue(Grid.RowProperty) == 0 && (int)btn2.GetValue(Grid.ColumnProperty) == 1 &&
                (int)btn3.GetValue(Grid.RowProperty) == 0 && (int)btn3.GetValue(Grid.ColumnProperty) == 2 &&
                (int)btn4.GetValue(Grid.RowProperty) == 0 && (int)btn4.GetValue(Grid.ColumnProperty) == 3 &&

                (int)btn5.GetValue(Grid.RowProperty) == 1 && (int)btn5.GetValue(Grid.ColumnProperty) == 0 &&
                (int)btn6.GetValue(Grid.RowProperty) == 1 && (int)btn6.GetValue(Grid.ColumnProperty) == 1 &&
                (int)btn7.GetValue(Grid.RowProperty) == 1 && (int)btn7.GetValue(Grid.ColumnProperty) == 2 &&
                (int)btn8.GetValue(Grid.RowProperty) == 1 && (int)btn8.GetValue(Grid.ColumnProperty) == 3 &&

                (int)btn9.GetValue(Grid.RowProperty) == 2 && (int)btn9.GetValue(Grid.ColumnProperty) == 0 &&
                (int)btn10.GetValue(Grid.RowProperty) == 2 && (int)btn10.GetValue(Grid.ColumnProperty) == 1 &&
                (int)btn11.GetValue(Grid.RowProperty) == 2 && (int)btn11.GetValue(Grid.ColumnProperty) == 2 &&
                (int)btn12.GetValue(Grid.RowProperty) == 2 && (int)btn12.GetValue(Grid.ColumnProperty) == 3 &&

                (int)btn13.GetValue(Grid.RowProperty) == 3 && (int)btn13.GetValue(Grid.ColumnProperty) == 0 &&
                (int)btn14.GetValue(Grid.RowProperty) == 3 && (int)btn14.GetValue(Grid.ColumnProperty) == 1 &&
                (int)btn15.GetValue(Grid.RowProperty) == 3 && (int)btn15.GetValue(Grid.ColumnProperty) == 2
               )
                return true;
            else
                return false;
        }

        private void Page_load(object sender, RoutedEventArgs e)
        {
            using (db = new SQLiteConnection("Data Source = Records.db; Version = 3"))
            {
                db.Open();
                SQLiteCommand command = db.CreateCommand();
                command.CommandText = $"select BestTime, BestMoves from Records;";

                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {               
                        var rec = new Record(reader["BestTime"].ToString(), reader["BestMoves"].ToString());
                        lblBestTime.Content = rec.BestTime;
                        lblBestMoves.Content = rec.BestMoves;
                    }
                }
            }
        }
    }
}
