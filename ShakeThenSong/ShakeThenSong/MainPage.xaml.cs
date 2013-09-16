using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ShakeThenSong.Resources;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace ShakeThenSong
{
    public partial class MainPage : PhoneApplicationPage
    {
        Accelerometer accelerometer;

        float x1 = 0;
        float x2 = 0;

        MediaLibrary mediaLib;
        AlbumCollection albums;
        SongCollection songs;

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();

            if (!Accelerometer.IsSupported)
            {
                // The device on which the application is running does not support
                // the accelerometer sensor. Alert the user and disable the
                // Start and Stop buttons.
                messageTextBlock.Text = "device does not support accelerometer";
            }
            else
            {
                if (accelerometer == null)
                {
                    // Instantiate the Accelerometer.
                    accelerometer = new Accelerometer();
                    accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
                    accelerometer.CurrentValueChanged +=
                        new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                }

                try
                {
                    mediaLib = new MediaLibrary();
                    albums = mediaLib.Albums;
                    songs = mediaLib.Songs;

                    messageTextBlock.Text = "Starting accelerometer.";
                    accelerometer.Start();
                }
                catch (InvalidOperationException ex)
                {
                    messageTextBlock.Text = "Unable to start accelerometer. The app encounter a exception :" + ex.Message;
                }
            }

        }

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Call UpdateUI on the UI thread and pass the AccelerometerReading.
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }


        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            messageTextBlock.Text = "getting data";

            Vector3 acceleration = accelerometerReading.Acceleration;

            x2 = x1;

            x1 = acceleration.X;

            x2 = System.Math.Abs(x2) + System.Math.Abs(x1);

            if (x2 > 1)
            {
                messageTextBlock.Text = "Yes, play music randomly.";
                PlayMediaSongs();
            }
            else
            {
                messageTextBlock.Text = "Please shake your phone.";
            }
        }

        private void PlayMediaSongs()
        {

            try
            {
                Album album = albums[(new System.Random()).Next(albums.Count)];
                Song song = songs[(new System.Random()).Next(albums.Count)];
                MediaPlayer.Play(song);
            }
            catch (Exception ex)
            {
                messageTextBlock.Text = "Unable to play music. The app encounter a exception :" + ex.Message;
            }
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}