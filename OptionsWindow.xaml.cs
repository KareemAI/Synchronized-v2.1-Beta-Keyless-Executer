using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WeAreDevs_API;

namespace Synapse_X_Remake
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        ExploitAPI exploit = new ExploitAPI();

        public OptionsWindow()
        {
            InitializeComponent();

            try
            {
                loadSettings();
                CheckVersion();
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

        private void CheckVersion()
        {
            if (File.Exists("./VersionChecker.exe"))
            {
                if (File.Exists("./bin/VersionCheck.json"))
                {
                    string RemakeVersionFile = File.ReadAllText("./bin/RemakeVersion.txt");
                    string VersionCheckFile = File.ReadAllText("./bin/VersionCheck.json");
                    JObject json = JObject.Parse(VersionCheckFile);

                    if (RemakeVersionFile == json["Github"]["Latest Version"].ToString())
                    {
                        VersionStatusBox.Text = $"{json["Message"].ToString()} ";
                        ReleaseLinkBox.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        VersionStatusBox.Text = "You're not up to date ";
                        ReleaseLinkBox.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                VersionStatusBox.Text = "Please download the Version Checker First ";
                ReleaseLinkBox.Visibility = Visibility.Hidden;
            }
        }

        private void loadSettings()
        {
            this.Topmost = Convert.ToBoolean(Properties.Settings.Default["TopMost"].ToString());
            TopMostBox.IsChecked = Convert.ToBoolean(Properties.Settings.Default["TopMost"].ToString());
            OldEditorCheckBox.IsChecked = Convert.ToBoolean(Properties.Settings.Default["OldEditor"].ToString());
            KrnlAPICheckBox.IsChecked = Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"].ToString());
        }

        private void UnlockBox_Checked(object sender, RoutedEventArgs e)
        {
            Process[] dunlockfps = Process.GetProcessesByName("rbxfpsunlocker");
            if (dunlockfps.Length > 0)
            {
                MessageBox.Show(this, "Fps Unlocker is already running\nUncheck the CheckBox again to turn off FPS Unlocker.", "Fps Unlocker", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.FileName = "./bin/rbxfpsunlocker.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.UseShellExecute = false;
                    myProcess.Start();
                }
            }
        }

        private void UnlockBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var killunfps in Process.GetProcessesByName("rbxfpsunlocker"))
            {
                killunfps.Kill();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void KillRoblox_Checked(object sender, RoutedEventArgs e)
        {
            Process[] DetectingRoblox = Process.GetProcessesByName("RobloxPlayerBeta");
            if (DetectingRoblox.Length > 0)
            {
                foreach (var RobloxKilled in Process.GetProcessesByName("RobloxPlayerBeta"))
                {
                    RobloxKilled.Kill();
                    MessageBox.Show("Roblox has killed", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Couldn't find RobloxPlayerBeta.exe!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TopMostBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["TopMost"] = true;
        }

        private void TopMostBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["TopMost"] = false;
        }

        private void TopMostBox_Click(object sender, RoutedEventArgs e)
        {
            if (TopMostBox.IsChecked == true)
            {
                var restart = MessageBox.Show("Restart is require for changes to take effect.", "Restart require", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (restart == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                var restart = MessageBox.Show("Restart is require for changes to take effect.", "Restart needed", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (restart == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void CustomFPSButton_Click(object sender, RoutedEventArgs e)
        {
            if (exploit.isAPIAttached() == true)
            {
                CustomFPSWindow customFPS = new CustomFPSWindow();
                customFPS.Show();
            }
            else
            {
                MessageBox.Show("Please attach the exploit first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void OldEditorCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (OldEditorCheckBox.IsChecked == true)
            {
                var restart = MessageBox.Show("Restart is require for changes to take effect.", "Restart require", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (restart == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                var restart = MessageBox.Show("Restart is require for changes to take effect.", "Restart needed", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (restart == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
        }

        private void OldEditorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["OldEditor"] = true;
        }

        private void OldEditorCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["OldEditor"] = false;
        }

        private void ReleaseHyperLinkBox_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("./bin/VersionCheck.json"))
            {
                string VersionCheckFile = File.ReadAllText("./bin/VersionCheck.json");
                JObject json = JObject.Parse(VersionCheckFile);

                Process.Start(json["Github"]["Release Link"].ToString());
                e.Handled = true;
            }
        }

        private void SourceCodeBox_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Charlzk05/Synapse-X-Remake-Synapse-X-Free-Version");
            e.Handled = true;
        }

        private void KrnlAPICheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (KrnlAPICheckBox.IsChecked == true)
            {
                var restart = MessageBox.Show("Restart is require for changes to take effect.", "Restart require", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (restart == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                var restart = MessageBox.Show("Restart is require for changes to take effect.", "Restart needed", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (restart == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
        }

        private void KrnlAPICheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["KrnlAPI"] = true;
        }

        private void KrnlAPICheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["KrnlAPI"] = false;
        }
    }
}
