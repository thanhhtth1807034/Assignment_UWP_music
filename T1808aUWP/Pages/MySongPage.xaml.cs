using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using T1808aUWP.Entity;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1808aUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MySongPage : Page
    {
        private const string GetMineSong = " https://2-dot-backup-server-003.appspot.com/_api/v2/songs/get-mine";
        private List<Song> _listSongs;
        private int _currentIndex = 0;
        private bool _isPlaying;
        private DispatcherTimer _playTimer;
        public MySongPage()
        {
            Debug.WriteLine("Init My song.");
            ReadTokenFromFile();
            this.InitializeComponent();
        }

        private async void ReadTokenFromFile()
        {
            var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("SavedFile",
                CreationCollisionOption.OpenIfExists);
            try
            {
                var storageFile =
                    await storageFolder.GetFileAsync("token.txt");

                if (storageFile != null)
                {
                    var jsonContent = await FileIO.ReadTextAsync(storageFile);
                    var memberCredential = JsonConvert.DeserializeObject<MemberCredential>(jsonContent);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(GetMineSong));
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers.Add("Authorization", "Basic" + " " + memberCredential.token);

                    var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    Debug.WriteLine(httpWebResponse.StatusCode);
                    Debug.WriteLine(httpWebResponse.Server);

                    string jsonString;
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8);
                        jsonString = reader.ReadToEnd();
                    }


                    _listSongs = new List<Song>() { };

                    _listSongs = JsonConvert.DeserializeObject<List<Song>>(jsonString);

                    TopSongs.ItemsSource = _listSongs;
                    Debug.WriteLine(jsonString);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var chooseSong = sender as StackPanel;
            MediaPlayer.Pause();
            if (chooseSong != null)
            {
                if (chooseSong.Tag is Song currentSong)
                {
                    _currentIndex = _listSongs.IndexOf(currentSong);
                    MediaPlayer.Source = new Uri(currentSong.link);
                    NowPlayingText.Text = "Now playing: " + currentSong.name + " - " + currentSong.singer;
                    Play();
                }
            }

            MediaPlayer.Play();
            Media_StateChanged();
        }

        private void Media_StateChanged()
        {
            if (MediaPlayer.CurrentState == MediaElementState.Playing)
            {
                progressBar1.Maximum = MediaPlayer.Position.Duration().TotalSeconds;
                _playTimer.Start();
            }
            else if (MediaPlayer.CurrentState == MediaElementState.Paused)
            {
                _playTimer.Stop();
            }
            else if (MediaPlayer.CurrentState == MediaElementState.Stopped)
            {
                _playTimer.Stop();
                progressBar1.Value = 0;
            }
        }

        private void Play()
        {
            MediaPlayer.Source = new Uri(_listSongs[_currentIndex].link);
            NowPlayingText.Text = "Now playing: " + _listSongs[_currentIndex].name + " - " +
                                  _listSongs[_currentIndex].singer;
            TopSongs.SelectedIndex = _currentIndex;
            MediaPlayer.Play();
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);
            _isPlaying = true;
            if (_isPlaying)
            {
                _playTimer = new DispatcherTimer();
                _playTimer.Interval = new TimeSpan(0, 0, 0);
                _playTimer.Tick += new EventHandler<object>(playTimer_Tick);
                _playTimer.Start();
            }
        }

        private void playTimer_Tick(object sender, object e)
        {
            TimeStart.Text = MediaPlayer.Position.ToString();
            TimeEnd.Text = MediaPlayer.NaturalDuration.TimeSpan.ToString();
            if (MediaPlayer.CurrentState == MediaElementState.Playing)
            {
                progressBar1.Value = MediaPlayer.Position.Seconds;
            }

        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _listSongs.Count - 1;
            }
            else if (_currentIndex >= _listSongs.Count)
            {
                _currentIndex = 0;
            }
            Play();
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        private void Pause()
        {
            MediaPlayer.Pause();
            PlayButton.Icon = new SymbolIcon(Symbol.Play);
            _isPlaying = false;
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentIndex++;
            if (_currentIndex >= _listSongs.Count || _currentIndex < 0)
            {
                _currentIndex = 0;
            }
            Play();
        }
    }
}
