using KrnlAPI;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WeAreDevs_API;

namespace Synapse_X_Remake
{
    /// <summary>
    /// Interaction logic for ScriptHubWindow.xaml
    /// </summary>
    public partial class ScriptHubWindow : Window
    {
        public static string jsonFile = File.ReadAllText("./bin/ScriptHub.json");
        JObject json = JObject.Parse(jsonFile);

        ExploitAPI module = new ExploitAPI();
        KrnlApi krnl = new KrnlApi();

        public ScriptHubWindow()
        {
            InitializeComponent();

            try
            {
                loadSettings();
                loadScripts();
            }
            catch (Exception error)
            {
                var option = MessageBox.Show($"{error.Message}\n\nDo you still want to continue?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (option == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void loadSettings()
        {
            this.Topmost = Convert.ToBoolean(Properties.Settings.Default["TopMost"].ToString());
        }

        private void loadScripts()
        {
            foreach (var i in json["Scripts"])
            {
                ScriptBox.Items.Add(i["Title"]);
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptBox.SelectedIndex != -1)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string content = client.DownloadString(json["Scripts"][ScriptBox.SelectedIndex]["Content"].ToString());

                        if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
                        {
                            krnl.Execute(content);
                        }
                        else
                        {
                            module.SendLuaScript(content);
                        };
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (ScriptBox.SelectedIndex != -1)
            {
                string script = json["Scripts"][ScriptBox.SelectedIndex]["Content"].ToString();
                Uri uri = new Uri(script, UriKind.Absolute);

                WebClient client = new WebClient();
                client.DownloadProgressChanged += scriptDownloadProgressChanged;
                client.DownloadStringCompleted += scriptDownloadCompleted;
                client.DownloadStringAsync(uri);
            };
        }

        private void scriptDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadButton.Content = $"Downloading ... {e.ProgressPercentage}";
        }

        private void scriptDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string title = json["Scripts"][ScriptBox.SelectedIndex]["Title"].ToString();

                File.WriteAllText($"./Scripts/{title}.lua", e.Result);
                DownloadButton.Content = "Download";
            }
            catch (Exception error)
            {
                var option = MessageBox.Show($"{error.Message}\n\nDo you still want to continue?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (option == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ScriptBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (json["Scripts"][ScriptBox.SelectedIndex]["Img"].ToString() == "")
            {
                Uri uri = new Uri("https://i.imgur.com/ArxGPeM.png");
                var BitmapImage = new BitmapImage();
                ImageBox.Visibility = Visibility.Hidden;
                BitmapImage.DownloadProgress += BitmapImage_DownloadProgress;
                BitmapImage.DownloadCompleted += BitmapImage_DownloadCompleted;

                DescriptionBox.Text = json["Scripts"][ScriptBox.SelectedIndex]["Description"].ToString();
                BitmapImage.BeginInit();
                BitmapImage.UriSource = uri;
                BitmapImage.EndInit();

                ImageBox.Source = BitmapImage;
            }
            else
            {
                var BitmapImage = new BitmapImage();
                ImageBox.Visibility = Visibility.Hidden;
                BitmapImage.DownloadProgress += BitmapImage_DownloadProgress;
                BitmapImage.DownloadCompleted += BitmapImage_DownloadCompleted;

                DescriptionBox.Text = json["Scripts"][ScriptBox.SelectedIndex]["Description"].ToString();
                BitmapImage.BeginInit();
                BitmapImage.UriSource = new Uri(json["Scripts"][ScriptBox.SelectedIndex]["Img"].ToString());
                BitmapImage.EndInit();

                ImageBox.Source = BitmapImage;
            }
        }

        private void BitmapImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            ProgressText.Visibility = Visibility.Visible;
            ProgressText.Text = $"Loading ... {e.Progress}%";
        }

        private void BitmapImage_DownloadCompleted(object sender, EventArgs e)
        {
            ProgressText.Visibility = Visibility.Hidden;
            ImageBox.Visibility = Visibility.Visible;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
