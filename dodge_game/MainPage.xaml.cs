using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;
using Windows.UI.Xaml.Shapes;
using Windows.UI.ViewManagement;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Threading;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.ApplicationModel;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace dodge_game
{
    
    public sealed partial class MainPage : Page
    {
        Board gameboard;
        Ellipse jerry;
        Ellipse [] toms;
        Ellipse[] cheese;
        DispatcherTimer timer;
        DispatcherTimer timerCheese;
        int Deadtoms = 0;
        bool[] isAlive;
        const int JerrySpeed = 10;
        const int TomSpeed = 10;
        int count = 0;
        int on;
        public MediaPlayer mediaPlayer;
        public MediaPlayer mediaYouLose;

        public MainPage()  
        {
            this.InitializeComponent();
            myGrid.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/startScreen.jpeg"))
            };
        }
        public void startGame()
        {
            start_game.Visibility = Visibility.Collapsed;
            mainThemeSong();
            

            double WindowRect = ApplicationView.GetForCurrentView().VisibleBounds.Width;
            double Windowrecy = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            gameboard = new Board(WindowRect, Windowrecy);
            myGrid.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/backgroundCheese3.jpg"))
            };

            jerry = AddNewElipsse(gameboard.jerry);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Tick += Timer_tick;
            timer.Start();
            on = 0;
            timerCheese = new DispatcherTimer();
            timerCheese.Interval = new TimeSpan(0, 0, 0, 5, 0);
            timerCheese.Tick += TimerCheese_tick;
            timerCheese.Start();

            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            cheese = new Ellipse[15];
            isAlive = new bool[10];
            toms = new Ellipse[10];
            for (int i = 0; i < toms.Length; i++)
            {
                toms[i] = AddNewElipsse(gameboard.toms[i]);
                isAlive[i] = true;
            }
        }
        public void Timer_tick(object sender, object e)
        {
            
            moveToms();
            colissionTomJerry();
            collisionToms();
            colissionJerryCheese();
            score_txtBlock.Text = "points:" + count.ToString();
        }
        public void TimerCheese_tick(object sender, object e)
        {
            if(on < cheese.Length)
            {
                cheese[on] = AddNewElipsse(gameboard.cheese[on]);
                on++;
            }
        }
        public void moveToms()
        {
            for(int i = 0; i < toms.Length; i++)
            {
                if (gameboard.toms[i]._x > gameboard.jerry._x )
                {
                    Canvas.SetLeft(toms[i],Canvas.GetLeft(toms[i])-TomSpeed);
                    gameboard.toms[i]._x = Canvas.GetLeft(toms[i])-TomSpeed;
                }
                if(gameboard.toms[i]._x < gameboard.jerry._x)
                {
                    Canvas.SetLeft(toms[i], Canvas.GetLeft(toms[i]) + TomSpeed);
                    gameboard.toms[i]._x = Canvas.GetLeft(toms[i]) + TomSpeed;
                }
                if (gameboard.toms[i]._y > gameboard.jerry._y)
                {
                    Canvas.SetTop(toms[i], Canvas.GetTop(toms[i]) - TomSpeed);
                    gameboard.toms[i]._y = Canvas.GetTop(toms[i]) - TomSpeed;
                }
                if (gameboard.toms[i]._y < gameboard.jerry._y)
                {
                    Canvas.SetTop(toms[i], Canvas.GetTop(toms[i]) + TomSpeed);
                    gameboard.toms[i]._y = Canvas.GetTop(toms[i]) + TomSpeed;
                }

            }
        }
        public async void collisionToms()
        {
            int t = 5;
            for (int i = 0; i < gameboard.toms.Length; i++)
            {
                if (isAlive[i])
                {
                    for (int j = 0; j < gameboard.toms.Length; j++)
                    {
                        if (i != j && isAlive[j])
                        {
                            if (gameboard.toms[i]._y > gameboard.toms[j]._y - (gameboard.toms[j]._height / 2 + t) && 
                                 gameboard.toms[i]._y < gameboard.toms[j]._y + (gameboard.toms[j]._height / 2 + t)
                                 && gameboard.toms[i]._x > gameboard.toms[j]._x -(gameboard.toms[j]._width / 2 + t)
                                 && gameboard.toms[i]._x < gameboard.toms[j]._x + (gameboard.toms[j]._width / 2) + t)
                            {
                                playground.Children.Remove(toms[i]);
                                Deadtoms++;
                                count += 30;
                                isAlive[i] = false;
                                if (Deadtoms == 9)
                                {
                                    timer.Stop();
                                    timerCheese.Stop();
                                    await new MessageDialog("you win").ShowAsync();
                                }
                            }
                        }
                    }
                }
            }
        }
        public async void colissionTomJerry()
        {
            for (int i = 0; i < 10; i++)
            {
                if ( gameboard.toms[i]._y > gameboard.jerry._y - (gameboard.toms[i]._height / 2 + 3)
                    && gameboard.toms[i]._y < gameboard.jerry._y + (gameboard.toms[i]._height / 2 + 3)
                    && gameboard.toms[i]._x > gameboard.jerry._x - (gameboard.toms[i]._width / 2 + 3)
                    && gameboard.toms[i]._x < gameboard.jerry._x + (gameboard.toms[i]._width / 2 + 3))
                {
                    timer.Stop();
                    timerCheese.Stop();
                    mediaPlayer.Pause();
                    /*musicYouLose();*/
                    await new MessageDialog("you lose").ShowAsync();
                }
            }
            
        }
        public void colissionJerryCheese()
        {
            for (int i = 0; i < 15; i++)
            {
                if (gameboard.cheese[i]._y > gameboard.jerry._y - (gameboard.cheese[i]._height / 2 + 3)
                    && gameboard.cheese[i]._y < gameboard.jerry._y + (gameboard.cheese[i]._height / 2 + 3)
                    && gameboard.cheese[i]._x > gameboard.jerry._x - (gameboard.cheese[i]._width / 2 + 3)
                    && gameboard.cheese[i]._x < gameboard.jerry._x + (gameboard.cheese[i]._width / 2 + 3))
                {
                    playground.Children.Remove(cheese[i]);
                    count += 10;
                }
            }

        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    if (Canvas.GetLeft(jerry) <= JerrySpeed)
                    {
                        break;
                    }
                    else   
                    Canvas.SetLeft(jerry, Canvas.GetLeft(jerry) - JerrySpeed);
                    gameboard.jerry._x = (int)Canvas.GetLeft(jerry) - JerrySpeed;
                    break;

                case VirtualKey.Right:
                    if (Canvas.GetLeft(jerry) + gameboard.jerry._width / 2 > playground.ActualWidth - JerrySpeed)
                    {
                        break;
                    }
                    else
                        Canvas.SetLeft(jerry, Canvas.GetLeft(jerry) + JerrySpeed);
                    gameboard.jerry._x = (int)Canvas.GetLeft(jerry) + JerrySpeed;
                    break;

                case VirtualKey.Up:
                    if (Canvas.GetTop(jerry) <= JerrySpeed)
                    {
                        break;
                    }
                    else
                        Canvas.SetTop(jerry, Canvas.GetTop(jerry) - JerrySpeed);
                    gameboard.jerry._y = (int)Canvas.GetTop(jerry) - JerrySpeed;
                    break;

                case VirtualKey.Down:
                    if (Canvas.GetTop(jerry) + gameboard.jerry._height / 2 > playground.ActualHeight - JerrySpeed)
                    {
                        break;
                    }
                    else
                        Canvas.SetTop(jerry, Canvas.GetTop(jerry) + JerrySpeed);
                    gameboard.jerry._y = (int)Canvas.GetTop(jerry) + JerrySpeed;
                    break;
            }
          
        }

        public Ellipse AddNewElipsse(GamePiece gamePiece)
        {
           Ellipse ellipse = new Ellipse();
            if (gamePiece is Player)
                ellipse.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("ms-appx:///Assets//jerry2.png"))

                };
            else
                ellipse.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("ms-appx:///Assets//tom_4.png"))

                };
            if(gamePiece is Cheese)
                ellipse.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("ms-appx:///Assets//Squeezy-Cheese-min.png"))

                };

            ellipse.Width = gamePiece._width;
            ellipse.Height = gamePiece._height;
            Canvas.SetLeft(ellipse, gamePiece._x);
            Canvas.SetTop(ellipse, gamePiece._y);

            playground.Children.Add(ellipse);
            return ellipse;
        }

        private void start_game_Click(object sender, RoutedEventArgs e)
        {

            startGame();
        }

        private void restart_game_Click(object sender, RoutedEventArgs e)
        {
            playground.Children.Clear();
            count = 0;
            Deadtoms = 0;
            /*mediaYouLose.Pause()*/
            startGame();
        }

        private void pause_game_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timerCheese.Stop();
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            mediaPlayer.Pause();
        }

        private void score_txtBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
            score_txtBlock.Text = "points:" + count.ToString();
        }

        private void resume_button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            timerCheese.Start();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            mediaPlayer.Play();
        }
        public async void mainThemeSong()
        {
            StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            StorageFile file = await folder.GetFileAsync("tom-and-jerry-ringtone.mp3");
            mediaPlayer = new MediaPlayer() { AutoPlay = true, Source = MediaSource.CreateFromStorageFile(file), Volume = 0.3, IsLoopingEnabled = true };
            mediaPlayer.Play();
        }
       /* public async void musicYouLose()
        {
            StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            StorageFile file = await folder.GetFileAsync("TomJerryGameOver.mp3");
            mediaYouLose = new MediaPlayer() { AutoPlay = true, Source = MediaSource.CreateFromStorageFile(file), Volume = 0.3, IsLoopingEnabled = true };
            mediaYouLose.Play();
        }*/
    }
}



