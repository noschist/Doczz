using Doczz.CustomClass;
using Doczz.CustomPages;
using Doczz.CustomWindow;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

namespace Doczz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        FtpHelper helper = new FtpHelper();

        public MainWindow()
        {
            InitializeComponent();
            ConnectFtp();
        }

        private void ConnectFtp()
        {
            VerifyFTP(helper.GetDetails());
        }

        public FtpClient client;

        private async void VerifyFTP(List<string> vs)
        {
            string name = vs[0], hostname = vs[1], hostuser = vs[2], hostpass = vs[3];
            client = new FtpClient
            {
                Host = hostname,
                ConnectTimeout = 200000
            };
            if (hostuser != "" && hostpass != "")
            {
                client.Credentials = new NetworkCredential(hostuser, hostpass);
            }
            try
            {
                await client.ConnectAsync();
            }
            catch(TimeoutException)
            {
                TopBarStatus.Foreground = (Brush)converter.ConvertFromString("#DDAA0000");
                TopBarStatus.Text = "Connection Timed Out!";
                return;
            }
            catch (FtpAuthenticationException)
            {
                TopBarStatus.Foreground = (Brush)converter.ConvertFromString("#DDAA0000");
                TopBarStatus.Text = "Invalid credentials!";
                return;
            }
            catch (SocketException)
            {
                TopBarStatus.Foreground = (Brush)converter.ConvertFromString("#DDAA0000");
                TopBarStatus.Text = "Invalid hostname!";
                return;
            }

            PostConnect(name, hostname, hostuser);
        }

        BrushConverter converter = new BrushConverter();

        private void PostConnect(string name, string hostname, string hostuser)
        {
            TopBarStatus.Text = "Connected";
            TopBarStatus.Foreground = (Brush)converter.ConvertFromString("#DD0FAA00");
            TopBarHost.Text = hostname;
            TopBarUser.Text = hostuser + $"({name})";
            DashboardPage dashboardPage = new DashboardPage();
            MainFrame.Navigate(dashboardPage);
        }

        private void LogoutBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            LoginUI loginUI = new LoginUI();
            loginUI.Show();
            Close();
        }


        private void DashMenuBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Brush selCo = (Brush)converter.ConvertFromString("#FF3390FF");
            Brush unSelCo = (Brush)converter.ConvertFromString("#FFA8B1C9");
            DashboardPage dashboardPage = new DashboardPage();
            MainFrame.Navigate(dashboardPage);
            DashIco.Foreground = selCo;
            FileIco.Foreground = unSelCo;
            SettIco.Foreground = unSelCo;
            DashTxt.Foreground = selCo;
            FileTxt.Foreground = unSelCo;
            SettTxt.Foreground = unSelCo;
            DashMenuBtn.Background = Brushes.White;
            SettMenuBtn.Background = Brushes.Transparent;
            FileMenuBtn.Background = Brushes.Transparent;
        }

        private void FileMenuBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Brush selCo = (Brush)converter.ConvertFromString("#FF3390FF");
            Brush unSelCo = (Brush)converter.ConvertFromString("#FFA8B1C9");
            MyFilesPage myFilesPage = new MyFilesPage();
            MainFrame.Navigate(myFilesPage);
            DashIco.Foreground = unSelCo;
            FileIco.Foreground = selCo;
            SettIco.Foreground = unSelCo;
            DashTxt.Foreground = unSelCo;
            FileTxt.Foreground = selCo;
            SettTxt.Foreground = unSelCo;
            DashMenuBtn.Background = Brushes.Transparent;
            SettMenuBtn.Background = Brushes.Transparent;
            FileMenuBtn.Background = Brushes.White;
        }

        private void SettMenuBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Brush selCo = (Brush)converter.ConvertFromString("#FF3390FF");
            Brush unSelCo = (Brush)converter.ConvertFromString("#FFA8B1C9");

            DashIco.Foreground = unSelCo;
            FileIco.Foreground = unSelCo;
            SettIco.Foreground = selCo;
            DashTxt.Foreground = unSelCo;
            FileTxt.Foreground = unSelCo;
            SettTxt.Foreground = selCo;
            DashMenuBtn.Background = Brushes.Transparent;
            SettMenuBtn.Background = Brushes.White;
            FileMenuBtn.Background = Brushes.Transparent;
        }

        private async void DiscoMenuBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            await client.DisconnectAsync();
            TopBarStatus.Text = "Disconnected";
            TopBarStatus.Foreground = (Brush)converter.ConvertFromString("#DDAA0000");
        }

        private void AboutUsBTn_Click(object sender, RoutedEventArgs e)
        {
            AboutUsUI aboutUs = new AboutUsUI
            {
                Owner = this,
                Width = ActualWidth,
                Height = ActualHeight
            };

            aboutUs.ShowDialog();
        }
    }
}
