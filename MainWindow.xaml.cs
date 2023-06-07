using KrnlAPI;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WeAreDevs_API;

namespace Synapse_X_Remake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ExploitAPI module = new ExploitAPI(); // WeAreDevs
        KrnlApi krnl = new KrnlApi(); // Krnl

        static string ThemeFile = File.ReadAllText("./bin/Theme.json");
        JObject json = JObject.Parse(ThemeFile);

        static string SelectedPath = "./Scripts";
        static string currentVersion = File.ReadAllText("./bin/RemakeVersion.txt");
        static string AceEditor = "file:///{0}/bin/AceEditor/editor.html";
        static string MonacoEditor = "file:///{0}/bin/MonacoEditor/editor.html";

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                MEditor(Monaco);
                loadThemes();
                loadSettings();
                loadScriptBox();
                runAttachedDetector();
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

        private void runAttachedDetector()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += AttachedDetectorTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void AttachedDetectorTick(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
            {
                if (krnl.IsInitialized() == true)
                {
                    var Header = json["Main"]["Header"];
                    TitleBox.Text = $"{Header["Title"]["Text"].ToString()} - {currentVersion} (Krnl Initialized)";

                    if (krnl.IsInjected() == true)
                    {
                        TitleBox.Text = $"{Header["Title"]["Text"].ToString()} - {currentVersion} (Krnl Attached)";
                    }
                    else
                    {
                        TitleBox.Text = $"{Header["Title"]["Text"].ToString()} - {currentVersion} (Krnl Not Attached)";
                    }
                }
                else
                {
                    var Header = json["Main"]["Header"];
                    TitleBox.Text = $"{Header["Title"]["Text"].ToString()} - {currentVersion} (Krnl Not Initialized)";
                }
            }
            else
            {
                if (module.isAPIAttached() == true)
                {
                    var Header = json["Main"]["Header"];
                    TitleBox.Text = $"{Header["Title"]["Text"].ToString()} - {currentVersion} (WeAreDevs Attached)";
                }
                else
                {
                    var Header = json["Main"]["Header"];
                    TitleBox.Text = $"{Header["Title"]["Text"].ToString()} - {currentVersion} (WeAreDevs Not Attached)";
                }
            }
        }

        private void loadSettings()
        {
            this.Topmost = Convert.ToBoolean(Properties.Settings.Default["TopMost"].ToString());
        }

        private void loadScriptBox()
        {
            DirectoryInfo directory = new DirectoryInfo("./Scripts");
            FileInfo[] file = directory.GetFiles("*.txt");
            foreach (FileInfo files in file)
            {
                ScriptBox.Items.Add(files.Name);
            }

            DirectoryInfo directory1 = new DirectoryInfo("./Scripts");
            FileInfo[] file1 = directory1.GetFiles("*.lua");
            foreach (FileInfo files1 in file1)
            {
                ScriptBox.Items.Add(files1.Name);
            }
        }

        private void loadThemes()
        {
            BrushConverter converter = new BrushConverter();

            var ListBox = json["Main"]["ScriptBox"];
            ScriptBox.Background = (Brush)converter.ConvertFrom(ListBox["Background"].ToString());
            ScriptBox.BorderBrush = (Brush)converter.ConvertFrom(ListBox["Background"].ToString());
            ScriptBox.Foreground = (Brush)converter.ConvertFrom(ListBox["Foreground"].ToString());

            var background = json["Main"];
            this.Background = (Brush)converter.ConvertFrom(background["Background"].ToString());

            var execute = json["Main"]["Buttons"]["Execute"];
            ExecuteButton.Background = (Brush)converter.ConvertFrom(execute["Background"].ToString());
            ExecuteButton.Foreground = (Brush)converter.ConvertFrom(execute["Foreground"].ToString());

            var Clear = json["Main"]["Buttons"]["Clear"];
            ClearButton.Background = (Brush)converter.ConvertFrom(Clear["Background"].ToString());
            ClearButton.Foreground = (Brush)converter.ConvertFrom(Clear["Foreground"].ToString());

            var Open = json["Main"]["Buttons"]["Open"];
            OpenFileButton.Background = (Brush)converter.ConvertFrom(Open["Background"].ToString());
            OpenFileButton.Foreground = (Brush)converter.ConvertFrom(Open["Foreground"].ToString());

            var ExecuteFile = json["Main"]["Buttons"]["ExecuteFile"];
            ExecuteFileButton.Background = (Brush)converter.ConvertFrom(ExecuteFile["Background"].ToString());
            ExecuteFileButton.Foreground = (Brush)converter.ConvertFrom(ExecuteFile["Foreground"].ToString());

            var Save = json["Main"]["Buttons"]["Save"];
            SaveFileButton.Background = (Brush)converter.ConvertFrom(Save["Background"].ToString());
            SaveFileButton.Foreground = (Brush)converter.ConvertFrom(Save["Foreground"].ToString());

            var Options = json["Main"]["Buttons"]["Options"];
            OptionsButton.Background = (Brush)converter.ConvertFrom(Options["Background"].ToString());
            OptionsButton.Foreground = (Brush)converter.ConvertFrom(Options["Foreground"].ToString());

            var Attach = json["Main"]["Buttons"]["Attach"];
            AttachButton.Background = (Brush)converter.ConvertFrom(Attach["Background"].ToString());
            AttachButton.Foreground = (Brush)converter.ConvertFrom(Attach["Foreground"].ToString());

            var ScriptHub = json["Main"]["Buttons"]["ScriptHub"];
            ScriptHubButton.Background = (Brush)converter.ConvertFrom(ScriptHub["Background"].ToString());
            ScriptHubButton.Foreground = (Brush)converter.ConvertFrom(ScriptHub["Foreground"].ToString());

            var Header = json["Main"]["Header"];
            Uri uri = new Uri(Header["Logo"].ToString(), UriKind.Absolute);

            TitleBox.Foreground = (Brush)converter.ConvertFrom(Header["Title"]["Foreground"].ToString());

            BitmapImage logo = new BitmapImage();
            logo.DownloadProgress += delegate (object sender, DownloadProgressEventArgs e)
            {
                TitleBox.Text = $"{Header["Title"]["Text"]} - {currentVersion} {e.Progress}";
            };
            logo.DownloadCompleted += delegate (object sender, EventArgs e)
            {
                TitleBox.Text = $"{Header["Title"]["Text"]} - {currentVersion}";
            };

            logo.BeginInit();
            logo.UriSource = uri;
            logo.EndInit();
            IconBox.Source = logo;

            TopBox.Background = (Brush)converter.ConvertFrom(Header["Background"].ToString());

            var Exit = json["Main"]["Header"]["Buttons"]["Exit"];
            CloseButton.Background = (Brush)converter.ConvertFrom(Exit["Background"].ToString());
            CloseButton.Foreground = (Brush)converter.ConvertFrom(Exit["Foreground"].ToString());

            var Minimize = json["Main"]["Header"]["Buttons"]["Minimize"];
            MiniButton.Background = (Brush)converter.ConvertFrom(Minimize["Background"].ToString());
            MiniButton.Foreground = (Brush)converter.ConvertFrom(Minimize["Foreground"].ToString());
        }

        private void MEditor(WebBrowser wb)
        {
            WebClient wc = new WebClient();
            wc.Proxy = null;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                string friendlyName = AppDomain.CurrentDomain.FriendlyName;
                bool flag2 = registryKey.GetValue(friendlyName) == null;
                if (flag2)
                {
                    registryKey.SetValue(friendlyName, 11001, RegistryValueKind.DWord);
                }
                registryKey = null;
                friendlyName = null;
            }
            catch (Exception error)
            {
                var option = MessageBox.Show($"{error.Message}\n\nDo you still want to continue?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (option == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
            }

            if (Convert.ToBoolean(Properties.Settings.Default["OldEditor"]) == true)
            {
                wb.Navigate(string.Format(MonacoEditor, System.IO.Directory.GetCurrentDirectory()));
            }
            else
            {
                wb.Navigate(string.Format(AceEditor, System.IO.Directory.GetCurrentDirectory()));
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
            {
                string scriptName = "GetText";
                object[] args = new string[0];
                object obj = Monaco.InvokeScript(scriptName, args);
                string script = obj.ToString();
                krnl.Execute(script);
            }
            else
            {
                string scriptName = "GetText";
                object[] args = new string[0];
                object obj = Monaco.InvokeScript(scriptName, args);
                string script = obj.ToString();
                module.SendLuaScript(script);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Monaco.InvokeScript("SetText", new object[]
            {
                ""
            });
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua";

            if (ofd.ShowDialog() == true)
            {
                string content = File.ReadAllText(ofd.FileName);
                Monaco.InvokeScript("SetText", new object[]
                {
                    content
                });
            }
        }

        private void ExecuteFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ef = new OpenFileDialog();
            ef.Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";
            if (ef.ShowDialog() == true)
            {
                if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
                {
                    krnl.Execute(File.ReadAllText(ef.FileName));
                }
                else
                {
                    module.SendLuaScript(File.ReadAllText(ef.FileName));
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, Monaco.InvokeScript("GetText", new object[0]).ToString());
            }
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OptionsWindow optionsWindow = new OptionsWindow();
                optionsWindow.Show();
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

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
            {
                if (krnl.IsInitialized() == true)
                {
                    krnl.Inject();
                }
                else
                {
                    krnl.Initialize();
                    krnl.Inject();
                }
            }
            else
            {
                if (module.IsUpdated() == false)
                {
                    var msgbox = MessageBox.Show("WeAreDevs is not yet updated, Are you sure you want continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Error);

                    if (msgbox == MessageBoxResult.Yes)
                    {
                        if (module.isAPIAttached() == true)
                        {
                            MessageBox.Show("Already Attached", "Exploit is running", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            module.LaunchExploit();
                        }
                    }
                }
                else
                {
                    if (module.isAPIAttached() == true)
                    {
                        MessageBox.Show("Already Attached", "Exploit is running", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        module.LaunchExploit();
                    }
                }
            }
        }

        private void ScriptHubButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ScriptHubWindow scriptHubWindow = new ScriptHubWindow();
                scriptHubWindow.Show();
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

        private void ExecuteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToBoolean(Properties.Settings.Default["KrnlAPI"]) == true)
                {
                    krnl.Execute(File.ReadAllText($"{SelectedPath}/{ScriptBox.SelectedItem}"));
                }
                else
                {
                    module.SendLuaScript(File.ReadAllText($"{SelectedPath}/{ScriptBox.SelectedItem}"));
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

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScriptBox.SelectedIndex != -1)
                {
                    Monaco.InvokeScript("SetText", new object[1]
                    {
                    File.ReadAllText($"{SelectedPath}/{ScriptBox.SelectedItem}")
                    });
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

        private void ChangeFolderItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = folderBrowser.SelectedPath;

                DirectoryInfo directory = new DirectoryInfo(SelectedPath);
                FileInfo[] file = directory.GetFiles("*.txt");
                foreach (FileInfo files in file)
                {
                    ScriptBox.Items.Add(files.Name);
                }

                DirectoryInfo directory1 = new DirectoryInfo(SelectedPath);
                FileInfo[] file1 = directory1.GetFiles("*.lua");
                foreach (FileInfo files in file1)
                {
                    ScriptBox.Items.Add(files.Name);
                }
            }
        }

        private void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            ScriptBox.Items.Refresh();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MiniButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
                {
                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    sfd.Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*";

                    if (sfd.ShowDialog() == true)
                    {
                        File.WriteAllText(sfd.FileName, Monaco.InvokeScript("GetText", new object[0]).ToString());
                    }
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.F))
                {
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua";

                    if (ofd.ShowDialog() == true)
                    {
                        string content = File.ReadAllText(ofd.FileName);
                        Monaco.InvokeScript("SetText", new object[]
                        {
                        content
                        });
                    }
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.X))
                {
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua";

                    if (ofd.ShowDialog() == true)
                    {
                        File.ReadAllText(ofd.FileName);
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

        private void Monaco_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                Monaco.InvokeScript("SetText", new object[]
                {
                    Properties.Settings.Default["EditorSave"].ToString()
                });
            }
            catch
            {

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                object obj = Monaco.InvokeScript("GetText", new string[0]);
                Properties.Settings.Default["EditorSave"] = obj.ToString();
                Properties.Settings.Default.Save();
            }
            catch
            {

            }
        }
    }
}