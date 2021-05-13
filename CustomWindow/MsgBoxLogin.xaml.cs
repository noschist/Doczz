using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Doczz.CustomWindow
{
    /// <summary>
    /// Interaction logic for MsgBoxLogin.xaml
    /// </summary>
    public partial class MsgBoxLogin : Window
    {

        //#FFFC4745 - error
        //#FF50E2C2 - succ

        public MsgBoxLogin(string msgtype, string body, string btn)
        {
            InitializeComponent();
            MsgBody.Text = body;
            SetupMsg(msgtype, btn);
        }

        void SetupMsg(string msgType, string btn)
        {
            if(msgType == "error")
            {
                MsgIcon.Source = new BitmapImage(new Uri("/Doczz;component/Images/err1.png", UriKind.RelativeOrAbsolute));
                MsgHeader.Text = "oooops";
                DismissBtn.Content = btn;
                Color color = (Color)ColorConverter.ConvertFromString("#FFFC4745");
                Brush brush = new SolidColorBrush(color);
                MsgHeader.Foreground = brush;
                DismissBtn.Background = brush;
            }
            else
            {
                MsgIcon.Source = new BitmapImage(new Uri("/Doczz;component/Images/succ1.png", UriKind.RelativeOrAbsolute));
                MsgHeader.Text = "yaaaay";
                DismissBtn.Content = btn;
                Color color = (Color)ColorConverter.ConvertFromString("#FF50E2C2");
                Brush brush = new SolidColorBrush(color);
                MsgHeader.Foreground = brush;
                DismissBtn.Background = brush;
            }
        }

        private void ExitAnim_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void DismissBtn_Click(object sender, RoutedEventArgs e)
        {
            var DismissAnim = Resources["ExitAnim"] as Storyboard;
            DismissAnim.Begin(MainMsgContent);
        }
    }
}
