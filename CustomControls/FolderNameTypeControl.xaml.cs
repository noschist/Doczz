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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Doczz.CustomControls
{
    /// <summary>
    /// Interaction logic for FolderNameTypeControl.xaml
    /// </summary>
    public partial class FolderNameTypeControl : Window
    {

        string path = "";

        public FolderNameTypeControl(string path)
        {
            InitializeComponent();
            this.path = path;
        }

        private void CLoseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        private async void CreateFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            await mainWindow.client.CreateDirectoryAsync($"{path}/{folderName.Text.Trim()}");
            Close();
        }
    }
}
