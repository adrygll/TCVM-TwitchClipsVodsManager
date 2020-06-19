using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TCVM_TwitchClipsVodsManager
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string ID = "gt5sxbffv6r2gw1kxxo53jvdj2u3f2";
        private ObservableCollection<Clip> clips = null;
        public MainWindow()
        {
            InitializeComponent();
            settingsManagment();
        }

        private void btnClips_Click(object sender, RoutedEventArgs e)
        {
            btnClips.Style = (Style)FindResource("ButtonClicked");
            btnVods.Style = (Style)FindResource("CategoryButton");
            btnSettings.Style = (Style)FindResource("CategoryButton");
            content.Visibility = Visibility.Visible;
        }

        private void btnSearchClips_Click(object sender, RoutedEventArgs e)
        {
            string channelName = txtChannel.Text.Trim();
            getClips(channelName);
        }

        private void getClips(string channelName)
        {
            string url = $"https://api.twitch.tv/kraken/clips/top?channel={channelName}&period=all&limit=100";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/json; charset=utf-8";
            req.Accept = "application/vnd.twitchtv.v5+json";
            req.Headers.Add("Client-ID", ID);

            try
            {
                using (Stream dataStream = req.GetResponse().GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();
                    // Display the content.
                    Console.WriteLine(responseFromServer);

                    reader.Close();

                    generateList(responseFromServer);
                }
            }
            catch (WebException we)
            {
                System.Windows.MessageBox.Show($"{we.Message} \n\n • Please make sure the channel name exists", "Error remoto", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void generateList(string responseFromServer)
        {
            string[] thumbnails = new string[3];
            clips = new ObservableCollection<Clip>();
            JObject json = JsonConvert.DeserializeObject<JObject>(responseFromServer);
            foreach (JObject obj in json["clips"])
            {
                string slug = (string)obj.GetValue("slug");
                string game = (string)obj.GetValue("game");
                string title = (string)obj.GetValue("title");
                int views = (int)obj.GetValue("views");
                int duration = (int)obj.GetValue("duration");
                DateTime d = DateTime.Parse(obj.GetValue("created_at").ToString());
                thumbnails[0] = (string)obj.GetValue("thumbnails")["medium"];
                thumbnails[1] = (string)obj.GetValue("thumbnails")["small"];
                thumbnails[2] = (string)obj.GetValue("thumbnails")["tiny"];
                clips.Add(new Clip(slug, game, title, views, duration, d, thumbnails));
            }
            this.clipList.ItemsSource = clips;
            clipList.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            content.Visibility = Visibility.Collapsed;
            settinsScreen.Visibility = Visibility.Collapsed;
            btnClips.Style = (Style)FindResource("CategoryButton");
            btnVods.Style = (Style)FindResource("CategoryButton");
            btnSettings.Style = (Style)FindResource("CategoryButton");
            home.Visibility = Visibility.Visible;
        }

        private void btnOpenClip_Click(object sender, RoutedEventArgs e)
        {
            if (clipList.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("No clip selected", "Selection error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Clip c = (Clip)clipList.SelectedItem;
                System.Diagnostics.Process.Start($"https://clips.twitch.tv/{c.Slug}");
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            btnClips.Style = (Style)FindResource("CategoryButton");
            btnVods.Style = (Style)FindResource("CategoryButton");
            btnSettings.Style = (Style)FindResource("ButtonClicked");
            settinsScreen.Visibility = Visibility.Visible;
            content.Visibility = Visibility.Collapsed;
            home.Visibility = Visibility.Collapsed;

        }

        private void btnChangeClipPath_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Properties.Settings.Default.clipPath = fbd.SelectedPath;
                    btnChangeClipPath.Content = Properties.Settings.Default.clipPath;
                }
            }
        }

        private void btnChangeVodPath_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Properties.Settings.Default.vodPath = fbd.SelectedPath;
                    btnChangeVodPath.Content = Properties.Settings.Default.vodPath;
                }
            }
        }

        private void settingsManagment()
        {
            btnChangeClipPath.Content = Properties.Settings.Default.clipPath;
            btnChangeVodPath.Content = Properties.Settings.Default.vodPath;
        }
    }
}
