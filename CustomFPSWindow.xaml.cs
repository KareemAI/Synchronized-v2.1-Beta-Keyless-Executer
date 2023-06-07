using System;
using System.Windows;
using System.Windows.Input;
using WeAreDevs_API;

namespace Synapse_X_Remake
{
    /// <summary>
    /// Interaction logic for CustomFPSWindow.xaml
    /// </summary>
    public partial class CustomFPSWindow : Window
    {
        ExploitAPI exploit = new ExploitAPI();

        public CustomFPSWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MiniButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SetFpsButton_Click(object sender, RoutedEventArgs e)
        {
            exploit.SendLuaScript($"set_fps_cap({fpsBox.Text})");
        }

        // EVENTS

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = Convert.ToBoolean(Properties.Settings.Default["TopMost"].ToString());
        }
    }
}
