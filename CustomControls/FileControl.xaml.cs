using Doczz.CustomPages;
using FluentFTP;
using Microsoft.Win32;
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
    /// Interaction logic for FileControl.xaml
    /// </summary>
    public partial class FileControl : UserControl
    {

        FtpClient client;
        FtpListItem item;
        MyFilesPage page;
        TreeViewItem viewItem;

        public FileControl(MyFilesPage page, FtpClient client, FtpListItem item, TreeViewItem viewItem)
        {
            InitializeComponent();
            this.client = client;
            this.item = item;
            this.page = page;
            this.viewItem = viewItem;
        }

        private void DownldBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save " + item.Name;
            sfd.Filter = $"Custom Files (*{System.IO.Path.GetExtension(item.FullName)}) | *{System.IO.Path.GetExtension(item.FullName)}";
            if (sfd.ShowDialog() == true)
            {
                client.DownloadFile(@"" + sfd.FileName, item.FullName);
                MessageBox.Show("Download Complete");
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            client.DeleteFile(item.FullName);
            page.GetAllFileInFolder(viewItem);
            MessageBox.Show("Delete Complete");
        }
    }
}
