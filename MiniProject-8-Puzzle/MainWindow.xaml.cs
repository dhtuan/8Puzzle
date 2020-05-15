using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;
using System.IO;

namespace MiniProject_8_Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadGame();
        }

        bool _isDragging = false;
        Point _firstPos;
        Point _lastPos;
        int _firstI;
        int _firstJ;
        int time = 180;
        DispatcherTimer timer = new DispatcherTimer();
        bool flag = false;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _firstPos = e.GetPosition(this);

            int i = (int)_firstPos.X / 200;
            int j = (int)_firstPos.Y / 200;

            if ((i < 3) && (j < 3))
            {
                _isDragging = true;
                _firstI = i;
                _firstJ = j;
                _lastPos = _firstPos;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPos = e.GetPosition(this);

                //var dx = currentPos.X - _lastPos.X;
                //var dy = currentPos.Y - _lastPos.Y;

                //var oldLeft = Canvas.GetLeft(_images[_firstI, _firstJ]);
                //var oldTop = Canvas.GetTop(_images[_firstI, _firstJ]);

                //Canvas.SetLeft(_images[_firstI, _firstJ], oldLeft + dx);
                //Canvas.SetTop(_images[_firstI, _firstJ], oldTop + dy);

                _lastPos = currentPos;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;

            var currentPos = e.GetPosition(this);

            if (currentPos.X <= 700 && currentPos.Y <= 700)
            {
                int currentI = (int)currentPos.X / 200;
                int currentJ = (int)currentPos.Y / 200;

                if (currentI < 3 && currentJ < 3)
                {
                    var x = (int)currentPos.X / 200 * 200;
                    var y = (int)currentPos.Y / 200 * 200;

                    if(_images[currentI, currentJ].Source == null && (currentI == _firstI || currentJ == _firstJ))
                    {
                        Canvas.SetLeft(_images[_firstI, _firstJ], x);
                        Canvas.SetTop(_images[_firstI, _firstJ], y);

                        Image temp = _images[_firstI, _firstJ];
                        _images[_firstI, _firstJ] = _images[currentI, currentJ];
                        _images[currentI, currentJ] = temp;
                    }
                }
            }

            Check();
        }

        private void UpPadButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var index = _images[i, j].Tag.ToString();

                    if (index == "(2, 2, 8)" && j + 1 <= 2)
                    {
                        var oldtop = Canvas.GetTop(_images[i, j + 1]);
                        Canvas.SetTop(_images[i, j + 1], oldtop - 200);

                        Image temp = _images[i, j];
                        _images[i, j] = _images[i, j + 1];
                        _images[i, j + 1] = temp;
                        Check();
                        return;
                    }
                }
            }
        }

        private void LeftPadButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var index = _images[i, j].Tag.ToString();

                    if (index == "(2, 2, 8)" && i + 1 <= 2)
                    {
                        var oldleft = Canvas.GetLeft(_images[i + 1, j]);
                        Canvas.SetLeft(_images[i + 1, j], oldleft - 200);

                        Image temp = _images[i, j];
                        _images[i, j] = _images[i + 1, j];
                        _images[i + 1, j] = temp;
                        Check();
                        return;
                    }
                }
            }
        }

        private void RightPadButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var index = _images[i, j].Tag.ToString();

                    if (index == "(2, 2, 8)" && i - 1 >= 0)
                    {
                        var oldleft = Canvas.GetLeft(_images[i - 1, j]);
                        Canvas.SetLeft(_images[i - 1, j], oldleft + 200);

                        Image temp = _images[i, j];
                        _images[i, j] = _images[i - 1, j];
                        _images[i - 1, j] = temp;
                        Check();
                        return;
                    }
                }
            }
        }

        private void DownPadButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var index = _images[i, j].Tag.ToString();

                    if (index == "(2, 2, 8)" && j - 1 >= 0)
                    {
                        var oldtop = Canvas.GetTop(_images[i, j - 1]);
                        Canvas.SetTop(_images[i, j - 1], oldtop + 200);

                        Image temp = _images[i, j];
                        _images[i, j] = _images[i, j - 1];
                        _images[i, j - 1] = temp;
                        Check();
                        return;
                    }
                }
            }
        }

        string filename = "";
        Image[,] _images = new Image[3, 3];
        List<Image> _samples = new List<Image>();
        List<string> _result = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (flag == false)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG" + "|All Files (*.*)|*.*";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == true)
                {
                    filename = dialog.FileName;
                    var image = new BitmapImage(new Uri(filename));
                    finalImage.Source = image;

                    var width = image.PixelWidth / 3;
                    var height = image.PixelHeight / 3;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if ((i == 2) && (j == 2))
                            {
                                _samples.Add(new Image()
                                {
                                    Width = 200,
                                    Height = 200,
                                    Tag = new Tuple<int, int, int>(i, j, 8),
                                });

                                _result.Add("(2, 2, 8)");

                            }
                            else
                            {
                                var part = new CroppedBitmap(image, new Int32Rect(i * width, j * height, width, height));
                                var imgPart = new Image()
                                {
                                    Source = part,
                                    Width = 200,
                                    Height = 200,
                                    Tag = new Tuple<int, int, int>(i, j, j * 3 + i)
                                };

                                _samples.Add(imgPart);
                                _result.Add(imgPart.Tag.ToString());
                            }

                        }
                    }

                    Random rng = new Random();
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int pos = rng.Next(_samples.Count);
                            _images[i, j] = _samples[pos];
                            content.Children.Add(_images[i, j]);

                            Canvas.SetLeft(_images[i, j], i * 200);
                            Canvas.SetTop(_images[i, j], j * 200);
                            _samples.RemoveAt(pos);
                        }
                    }

                    //int pos = 0;
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    for (int j = 0; j < 3; j++)
                    //    {
                    //        _images[i, j] = _samples[pos++];
                    //        content.Children.Add(_images[i, j]);
                    //        Canvas.SetLeft(_images[i, j], i * 200);
                    //        Canvas.SetTop(_images[i, j], j * 200);
                    //    }
                    //}

                    timer = new DispatcherTimer();
                    timer.Interval = new TimeSpan(0, 0, 1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var writer = new StreamWriter("save.dat");

            writer.WriteLine(filename);

            writer.WriteLine(time);

            foreach (var img in _images)
            {
                var tag = img.Tag as Tuple<int, int, int>;

                writer.WriteLine(tag.ToString());
            }

            writer.Close();

            MessageBox.Show("Save successfully!");
        }

        private void Check()
        {
            bool check = true;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var index1 = _images[i, j].Tag.ToString();
                    var index2 = _result[i * 3 + j];

                    if (index1 != index2)
                    {
                        check = false;
                        break;
                    }
                }
            }

            if (check == true)
            {
                timer.Stop();
                MessageBox.Show("Bravo! You win!");
                System.Environment.Exit(0);
            }
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            bool check = true;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var index1 = _images[i, j].Tag.ToString();
                    var index2 = _result[i * 3 + j];

                    if (index1 != index2)
                    {
                        check = false;
                        if (time != 0)
                        {
                            MessageBox.Show("You haven't won yet! Keep playing.");
                        }
                        else
                        {
                            MessageBox.Show("You lose!");
                            System.Environment.Exit(0);
                        }
                        return;
                    }
                }
            }

            if (check == true)
            {
                timer.Stop();
                MessageBox.Show("Bravo! You win!");
                System.Environment.Exit(0);
            }
        }

        private void Load()
        {
            var reader = new StreamReader("save.dat");
            filename = reader.ReadLine();
            
            if (filename == null)
            {
                MessageBox.Show("You haven't saved any games yet! There's nothing to load.");
                reader.Close();
                flag = false;
            }
            else
            {
                int newtime = int.Parse(reader.ReadLine());

                string line;
                List<string> temp = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    temp.Add(line);
                }

                reader.Close();

                var image = new BitmapImage(new Uri(filename));
                finalImage.Source = image;

                timer.Stop();
                time = newtime;
                content.Children.Clear();

                var width = image.PixelWidth / 3;
                var height = image.PixelHeight / 3;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if ((i == 2) && (j == 2))
                        {
                            var img = new Image()
                            {
                                Width = 200,
                                Height = 200,
                                Tag = new Tuple<int, int, int>(i, j, 8),
                            };

                            _samples.Add(img);
                            _result.Add(img.Tag.ToString());

                        }
                        else
                        {
                            var part = new CroppedBitmap(image, new Int32Rect(i * width, j * height, width, height));
                            var imgPart = new Image()
                            {
                                Source = part,
                                Width = 200,
                                Height = 200,
                                Tag = new Tuple<int, int, int>(i, j, j * 3 + i)
                            };

                            _samples.Add(imgPart);
                            _result.Add(imgPart.Tag.ToString());
                        }
                    }
                }

                int findIndex(List<Image> l, string x)
                {
                    int index = 0;

                    for (int i = 0; i < l.Count; i++)
                    {
                        if (l[i].Tag.ToString() == x)
                        {
                            index = i;
                        }
                    }

                    return index;
                }

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var tag = temp[i * 3 + j];

                        _images[i, j] = _samples[findIndex(_samples, tag)];
                        content.Children.Add(_images[i, j]);
                        Canvas.SetLeft(_images[i, j], i * 200);
                        Canvas.SetTop(_images[i, j], j * 200);
                    }
                }

                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += Timer_Tick;
                timer.Start();
            }          
        }

        private void LoadGame()
        {
            if (File.Exists("save.dat"))
            {
                System.Windows.Forms.DialogResult dlr = System.Windows.Forms.MessageBox.Show("Do you want to load the saved game?", "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo);

                if (dlr == System.Windows.Forms.DialogResult.Yes)
                {
                    flag = true;
                    try
                    {
                        Load();
                    }
                    catch (FileNotFoundException)
                    {
                        flag = false;
                        MessageBox.Show("File not found: " + filename + "\nUnable to load saved game!", "Error");
                    }
                }
            }
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dlr = System.Windows.Forms.MessageBox.Show("Are you sure you want to load the saved game? Current unsaved progress will be lost.", "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dlr == System.Windows.Forms.DialogResult.Yes)
            {
                if (File.Exists("save.dat"))
                {
                    try
                    {
                        Load();
                    }
                    catch (FileNotFoundException)
                    {
                        flag = false;
                        MessageBox.Show("File not found: " + filename + "\nUnable to load saved game!", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("You haven't saved any games yet! There's nothing to load.");
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (time > 0)
            {
                if (time <= 10)
                {
                    TimerCountdown.Background = Brushes.OrangeRed;
                    TimerCountdown.Foreground = Brushes.White;
                    time--;
                    TimerCountdown.Text = string.Format("   00:0{0}:0{1}   ", time / 60, time % 60);
                }
                else
                {
                    TimerCountdown.Background = Brushes.SeaGreen;
                    TimerCountdown.Foreground = Brushes.White;
                    time--;
                    if (time % 60 < 10)
                    {
                        TimerCountdown.Text = string.Format("   00:0{0}:0{1}   ", time / 60, time % 60);
                    }
                    else
                    {
                        TimerCountdown.Text = string.Format("   00:0{0}:{1}   ", time / 60, time % 60);
                    }
                }
            }
            else
            {
                timer.Stop();
                MessageBox.Show("You lose!");
                System.Environment.Exit(0);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.DialogResult dlr = System.Windows.Forms.MessageBox.Show("Are you sure you want to exit? Current unsaved progress will be lost.", "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dlr == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
