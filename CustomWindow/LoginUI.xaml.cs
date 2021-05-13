using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Doczz.CustomClass;
using FluentFTP;
using System.Net.NetworkInformation;

namespace Doczz.CustomWindow
{
    /// <summary>
    /// Interaction logic for LoginUI.xaml
    /// </summary>
    public partial class LoginUI : Window
    {

        SqlSetup sql = new SqlSetup();

        public LoginUI()
        {
            InitializeComponent();
            CheckRem();
        }

        private void CheckRem()
        {
            if(Properties.Settings.Default.email != "")
            {
                loginUserEmail.Text = Properties.Settings.Default.email;
                loginPass.Password = Properties.Settings.Default.pass;
                RemMeChck.IsChecked = true;
            }
        }

        public async Task CheckInternetAsync()
        {
            Ping myPing = new Ping();
            try
            {
                var pingReply = await myPing.SendPingAsync("google.com", 3000, new byte[32], new PingOptions(64, true));
                FtpButtonManager("int");

            }
            catch (Exception)
            {
                FtpButtonManager("noint");
            }
        }

        private void FtpButtonManager(string v)
        {
            if (v == "noint")
            {
                SwitchToLogin.IsEnabled = false;
                SwitchToCreateAcc.IsEnabled = false;
                SaveDetBtn.IsEnabled = false;
                LoginAccBtn.IsEnabled = false;
                CreateAccBtn.IsEnabled = false;
            }
            else
            {
                SwitchToLogin.IsEnabled = true;
                SwitchToCreateAcc.IsEnabled = true;
                SaveDetBtn.IsEnabled = true;
                LoginAccBtn.IsEnabled = true;
                CreateAccBtn.IsEnabled = true;
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateAccBtn_Click(object sender, RoutedEventArgs e)
        {
            Validator("create");
        }

        //MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        private void Validator(string type)
        {
            if(type == "create")
            {
                if(createUser.Text.Trim() == "" || createEmail.Text.Trim() == "" || createPass.Password.Trim() == "" || createPassRep.Password.Trim() == "")
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please fill in all necessary fields", "Try again")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else if(createPass.Password.Trim() != createPassRep.Password.Trim())
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Password don't match", "Okay")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else if (createEmail.Text.Trim() == "")
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Email id is neccessary", "Got it")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else if (!Regex.IsMatch(createEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please enter a valid mail-id", "Okay")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else
                {
                    CreateAccount();
                }
            }
            else if(type == "ftp")
            {
                if(ftpHost.Text.Trim() == "")
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please enter your FTP server address", "Okay")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else if(ftpUser.Text.Trim() == "" || ftpUser.Text.Trim() == "")
                {
                    VerifyAnonFtpServer();
                }
                else
                {
                    VerifyFtpServer();
                }
            }
            else if(type == "login")
            {
                if(loginUserEmail.Text.Trim() == "" || loginPass.Password.Trim() == "")
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please enter your username and password", "Try again")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else if (!Regex.IsMatch(loginUserEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please enter a valid mail-id", "Okay")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
                else
                {
                    LoginAccount();
                }
            }
        }


        void LoginAccount()
        {
            sql.LoadTable($"select * from Users where Email='{loginUserEmail.Text.Trim()}';", ref dt);
            if (dt.Rows.Count == 0)
            {
                MsgBoxLogin msgBox = new MsgBoxLogin("error", "Can't find an account associated with this email id", "Okay")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
            }
            else
            {
                if(loginUserEmail.Text.Trim() == dt.Rows[0][1].ToString() && loginPass.Password.Trim() == dt.Rows[0][2].ToString())
                {
                    if (RemMeChck.IsChecked == true)
                    {
                        Properties.Settings.Default.email = loginUserEmail.Text.Trim();
                        Properties.Settings.Default.pass = loginPass.Password.Trim();
                        Properties.Settings.Default.Save();
                    }
                    else
                        Properties.Settings.Default.Reset();

                    Properties.Settings.Default.CurrentUser = loginUserEmail.Text.Trim();
                    Properties.Settings.Default.Save();

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    MsgBoxLogin msgBox = new MsgBoxLogin("error", "Password and username doesn't match", "Okay")
                    {
                        Owner = this,
                        Width = ActualWidth,
                        Height = ActualHeight
                    };
                    msgBox.ShowDialog();
                }
            }
        }


        void ConnectionPrep()
        {
            ftpHost.IsEnabled = false;
            ftpUser.IsEnabled = false;
            ftpPass.IsEnabled = false;
            SaveDetBtn.IsHitTestVisible = false;
            SaveDetBtn.Content = "Connecting...";
            ButtonManip(60, true);
        }

        void ResetFtpPage()
        {
            ftpHost.IsEnabled = true;
            ftpUser.IsEnabled = true;
            ftpPass.IsEnabled = true;
            ftpHost.Text = "";
            ftpUser.Text = "";
            ftpPass.Password = "";
            SaveDetBtn.IsHitTestVisible = true;
            SaveDetBtn.Content = "Save details";
            ButtonManip(0, false);

        }

        void ConnectionSucces()
        {
            SaveDetBtn.Content = "Success!";
            ButtonManip(0, false);
            string query = $"insert into hosts values('{createEmail.Text.Trim()}', '{ftpHost.Text.Trim()}', '{ftpUser.Text.Trim()}', '{ftpPass.Password.Trim()}')";
            sql.ExecuteQuery(query);
            Properties.Settings.Default.CurrentUser = createEmail.Text.Trim();
            Properties.Settings.Default.Save();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        void ButtonManip(int val, bool status)
        {
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetValue(SaveDetBtn, val);
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndicatorVisible(SaveDetBtn, status);
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndeterminate(SaveDetBtn, status);
        }

        async void VerifyFtpServer()
        {
            ConnectionPrep();
            FtpClient client = new FtpClient(ftpHost.Text.Trim());
            client.Credentials = new NetworkCredential(ftpUser.Text.Trim(), ftpPass.Password.Trim());
            try
            {
                await client.ConnectAsync();
            }
            catch (FtpAuthenticationException)
            {
                MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please check the username or password", "Okay")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
                ResetFtpPage();
                return;
            }
            catch (SocketException)
            {
                MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please enter a valid host address", "Okay")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
                ResetFtpPage();
                return;
            }

            ConnectionSucces();

        }

        async void VerifyAnonFtpServer()
        {
            ConnectionPrep();
            FtpClient client = new FtpClient(ftpHost.Text.Trim());
            try
            {
                await client.ConnectAsync();
            }
            catch (FtpAuthenticationException)
            {
                MsgBoxLogin msgBox = new MsgBoxLogin("error", "Host doesn't allow anonymous login", "Okay")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
                ResetFtpPage();
                return;
            }
            catch(SocketException)
            {
                MsgBoxLogin msgBox = new MsgBoxLogin("error", "Please enter a valid host address", "Okay")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
                ResetFtpPage();
                return;
            }

            ConnectionSucces();           
        }

        DataTable dt = new DataTable();

        void CreateAccount()
        {
            sql.LoadTable($"select * from Users where Email='{createEmail.Text.Trim()}';", ref dt);
            if(dt.Rows.Count == 0)
            {
                string query = $"insert into Users values('{createUser.Text.Trim()}', '{createEmail.Text.Trim()}', '{createPass.Password.Trim()}')";
                sql.ExecuteQuery(query);
                MsgBoxLogin msgBox = new MsgBoxLogin("succ", "Account created successfully", "Cool")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
                CreateAccGrid.Visibility = Visibility.Collapsed;
                CreateAccStage2Grid.Visibility = Visibility.Visible;
            }
            else
            {
                MsgBoxLogin msgBox = new MsgBoxLogin("error", "Account already exists", "Login")
                {
                    Owner = this,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                msgBox.ShowDialog();
                CreateAccGrid.Visibility = Visibility.Collapsed;
                LoginAccGrid.Visibility = Visibility.Visible;
                loginPass.Password = createPass.Password.Trim();
                loginUserEmail.Text = createEmail.Text.Trim();
            }
        }

        private void SwitchToLogin_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CreateAccGrid.Visibility = Visibility.Collapsed;
            LoginAccGrid.Visibility = Visibility.Visible;
            createEmail.Text = "";
            createUser.Text = "";
            createPass.Password = "";
            createPassRep.Password = "";
        }

        private void SwitchToCreateAcc_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CreateAccGrid.Visibility = Visibility.Visible;
            LoginAccGrid.Visibility = Visibility.Collapsed;
            loginUserEmail.Text = "";
            loginPass.Password = "";
        }

        private void SaveDetBtn_Click(object sender, RoutedEventArgs e)
        {
            Validator("ftp");
        }

        private void LoginAccBtn_Click(object sender, RoutedEventArgs e)
        {
            Validator("login");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            while(true)
                await CheckInternetAsync();
        }
    }
}
