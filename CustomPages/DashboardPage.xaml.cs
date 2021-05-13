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
using FluentFTP;

namespace Doczz.CustomPages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            await GetAllData("/");
            await CountFiles();
        }

        private Task CountFiles()
        {
            foreach(string file in files)
            {
                string ext = System.IO.Path.GetExtension(file);
                if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".gif")
                {
                    imcount++;
                }
                else if (ext == ".mp4" || ext == ".mkv" || ext == ".mov" || ext == ".avi" || ext == ".wmv")
                {
                    vcount++;
                }
                else if (ext == ".mp3" || ext == ".ogg" || ext == ".m4a" || ext == ".wav")
                {
                    mcount++;
                }
                else
                {
                    dcount++;
                }    
            }

            ImgCount.Text = imcount.ToString() + " Files";
            VidCount.Text = vcount.ToString() + " Files";
            MusCount.Text = mcount.ToString() + " Files";
            DocCount.Text = dcount.ToString() + " Files";

            return Task.CompletedTask;
        }

        List<string> files = new List<string>();
        int imcount = 0;
        int vcount = 0;
        int mcount = 0;
        int dcount = 0;

        async Task GetAllData(string path)
        {
            foreach (FtpListItem item in mainWindow.client.GetListing(path))
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await GetAllData(item.FullName);
                }

                if (item.Type == FtpFileSystemObjectType.File)
                    files.Add(item.Name);
            }
        }

    }
}
