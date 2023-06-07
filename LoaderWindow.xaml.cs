using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Input;
using WeAreDevs_API;

namespace Synapse_X_Remake
{
    /// <summary>
    /// Interaction logic for LoaderWindow.xaml
    /// </summary>
    public partial class LoaderWindow : Window
    {
        ExploitAPI exploit = new ExploitAPI(); // WeAreDevs

        public string binFileUrl = "https://raw.githubusercontent.com/Charlzk05/SynxRemakeResources/main/v1.0.8/bin.zip";
        public string ScriptsFileUrl = "https://raw.githubusercontent.com/Charlzk05/SynxRemakeResources/main/v1.0.7/Scripts.zip";
        public string versionCheckerFileUrl = "https://github.com/Charlzk05/Synapse-X-Remake-Synapse-X-Free-Version/files/9867386/VersionChecker.zip";

        public LoaderWindow()
        {
            InitializeComponent();

            try
            {
                checkVersion();
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

        private void checkVersion()
        {
            if (File.Exists("./VersionChecker.exe"))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "VersionChecker.exe";
                Process.Start(startInfo).WaitForExit();

                versionCheck();
            }
            else
            {
                var option = MessageBox.Show("Would you like to install the Version Checker?", "Version Checker", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (option == MessageBoxResult.Yes)
                {
                    using (WebClient client = new WebClient())
                    {
                        Uri uri = new Uri(versionCheckerFileUrl, UriKind.Absolute);
                        client.DownloadProgressChanged += VersionCheckerDownloadProgress;
                        client.DownloadFileCompleted += VersionCheckerDownloadCompleted;
                        client.DownloadFileAsync(uri, "./VersionChecker.zip");
                    }
                }
                else
                {
                    versionCheck();
                }
            }
        }

        private void versionCheck()
        {
            if (File.Exists("./bin/VersionCheck.json"))
            {
                string CurrentVersion = File.ReadAllText("./bin/RemakeVersion.txt");
                string VersionCheckFile = File.ReadAllText("./bin/VersionCheck.json");
                JObject json = JObject.Parse(VersionCheckFile);

                if (CurrentVersion == json["Github"]["Latest Version"].ToString())
                {
                    TitleBox.Text += $" ({json["Message"].ToString()})";
                }
                else
                {
                    TitleBox.Text += $" ({json["Message"].ToString()})";

                    var option = MessageBox.Show("Would you like to install the latest version?", $"You're not up to date", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (option == MessageBoxResult.Yes)
                    {
                        Process.Start(json["Github"]["Release Link"].ToString());
                    }
                }

                CheckNDownload();
            }
            else
            {
                CheckNDownload();
            }
        }

        private void VersionCheckerDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            StatusBox.Content = $"Downloading Version Checker ... {e.ProgressPercentage}%";
            ProgressBox.Value = e.ProgressPercentage;
        }

        private void VersionCheckerDownloadCompleted(object sender, EventArgs e)
        {
            ZipFile.ExtractToDirectory("./VersionChecker.zip", "./");
            File.Delete("./VersionChecker.zip");
            StatusBox.Content = "Done!";

            CheckNDownload();
        }

        private void CheckNDownload()
        {
            if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
            {
                if (Directory.Exists("./bin") && Directory.Exists("./Scripts"))
                {
                    StatusBox.Content = "Done!";
                    ProgressBox.Value = 100;

                    try
                    {
                        this.Hide();
                        MainWindow main = new MainWindow();
                        main.Show();
                    }
                    catch (Exception LoadingError)
                    {
                        var option = MessageBox.Show($"{LoadingError.Message}\n\nDo you still want to continue?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (option == MessageBoxResult.No)
                        {
                            Application.Current.Shutdown();
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadProgressChanged += binFileDownloadProgress;
                            client.DownloadFileCompleted += binFileDownloadComplete;
                            client.DownloadFileAsync(new Uri(binFileUrl), "./bin.zip");
                        }
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
            }
            else
            {
                if (exploit.IsUpdated() == true)
                {
                    if (Directory.Exists("./bin") && Directory.Exists("./Scripts"))
                    {
                        StatusBox.Content = "Done!";
                        ProgressBox.Value = 100;

                        try
                        {
                            this.Hide();
                            MainWindow main = new MainWindow();
                            main.Show();
                        }
                        catch (Exception LoadingError)
                        {
                            var option = MessageBox.Show($"{LoadingError.Message}\n\nDo you still want to continue?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (option == MessageBoxResult.No)
                            {
                                Application.Current.Shutdown();
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            using (var client = new WebClient())
                            {
                                client.DownloadProgressChanged += binFileDownloadProgress;
                                client.DownloadFileCompleted += binFileDownloadComplete;
                                client.DownloadFileAsync(new Uri(binFileUrl), "./bin.zip");
                            }
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
                }
                else
                {
                    var msgbox = MessageBox.Show("WeAreDevs is not yet updated, Are you sure you want continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Error);

                    if (msgbox == MessageBoxResult.Yes)
                    {
                        if (Directory.Exists("./bin") && Directory.Exists("./Scripts"))
                        {
                            StatusBox.Content = "Done!";
                            ProgressBox.Value = 100;

                            this.Hide();
                            MainWindow main = new MainWindow();
                            main.Show();
                        }
                        else
                        {
                            try
                            {
                                using (var client = new WebClient())
                                {
                                    client.DownloadProgressChanged += binFileDownloadProgress;
                                    client.DownloadFileCompleted += binFileDownloadComplete;
                                    client.DownloadFileAsync(new Uri(binFileUrl), "./bin.zip");
                                }
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
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void binFileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            StatusBox.Content = $"Downloading Bin ... {e.ProgressPercentage}%";
            ProgressBox.Value = e.ProgressPercentage;
        }

        private void binFileDownloadComplete(object sender, EventArgs e)
        {
            try
            {
                StatusBox.Content = "Done!";
                string bin = "./bin";

                if (Directory.Exists(bin))
                {
                    DirectoryInfo directory = new DirectoryInfo(bin);

                    foreach (var file in directory.GetFiles())
                    {
                        file.Delete();
                    }

                    foreach (var subDir in directory.GetDirectories())
                    {
                        subDir.Delete(true);
                    }

                    ZipFile.ExtractToDirectory("./bin.zip", "./");
                    File.Delete("./bin.zip");

                    using (var client = new WebClient())
                    {
                        client.DownloadProgressChanged += scriptsFileDownloadProgress;
                        client.DownloadFileCompleted += scriptsFileDownloadCompleted;
                        client.DownloadFileAsync(new Uri(ScriptsFileUrl), "./Scripts.zip");
                    }
                }
                else
                {
                    ZipFile.ExtractToDirectory("./bin.zip", "./");
                    File.Delete("./bin.zip");

                    using (var client = new WebClient())
                    {
                        client.DownloadProgressChanged += scriptsFileDownloadProgress;
                        client.DownloadFileCompleted += scriptsFileDownloadCompleted;
                        client.DownloadFileAsync(new Uri(ScriptsFileUrl), "./Scripts.zip");
                    }
                }
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

        private void scriptsFileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            StatusBox.Content = $"Downloading Scripts ... {e.ProgressPercentage}%";
            ProgressBox.Value = e.ProgressPercentage;
        }

        private void scriptsFileDownloadCompleted(object sender, EventArgs e)
        {
            try
            {
                StatusBox.Content = "Done!";
                string scripts = "./Scripts";

                if (Directory.Exists(scripts))
                {
                    DirectoryInfo directory = new DirectoryInfo(scripts);

                    foreach (var file in directory.GetFiles())
                    {
                        file.Delete();
                    }

                    foreach (var subDir in directory.GetDirectories())
                    {
                        subDir.Delete(true);
                    }

                    ZipFile.ExtractToDirectory("./Scripts.zip", "./");
                    File.Delete("./Scripts.zip");

                    this.Hide();
                    MainWindow main = new MainWindow();
                    main.Show();
                }
                else
                {
                    ZipFile.ExtractToDirectory("./Scripts.zip", "./");
                    File.Delete("./Scripts.zip");

                    this.Hide();
                    MainWindow main = new MainWindow();
                    main.Show();
                }
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

        private void MonacoDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            StatusBox.Content = $"Downloading Old Editor ... {e.ProgressPercentage}%";
            ProgressBox.Value = e.ProgressPercentage;
        }
    }
}
