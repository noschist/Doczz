using Doczz.CustomClass;
using Doczz.CustomControls;
using FluentFTP;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using MahApps.Metro.IconPacks;
using Microsoft.Win32;

namespace Doczz.CustomPages
{
    /// <summary>
    /// Interaction logic for MyFilesPage.xaml
    /// </summary>
    public partial class MyFilesPage : Page
    {

        public MyFilesPage()
        {
            InitializeComponent();
            ConnectFtp();
        }

        private void ConnectFtp()
        {
            VerifyFTP(helper.GetDetails());
        }

        FtpHelper helper = new FtpHelper();

        private async void VerifyFTP(List<String> vs)
        {
            await GetFileList();
        }

        MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();


        async Task<Task> GetFileList()
        {
            foreach (FtpListItem item in mainWindow.client.GetListing("/"))
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await GetFolderList(FolderTreeView, item);
                }
            }
            return Task.CompletedTask;
        }

        private Task GetFolderList(TreeView root, FtpListItem item)
        {
            root.Items.Add(CreateFolderNode(item));
            return Task.CompletedTask;
        }

        private TreeViewItem CreateFolderNode(FtpListItem item)
        {
            var SubFolder = new TreeViewItem();
            SubFolder.Header = item.FullName;
            foreach (FtpListItem item1 in mainWindow.client.GetListing(item.FullName))
            {
                if(item1.Type == FtpFileSystemObjectType.Directory)
                    SubFolder.Items.Add(CreateFolderNode(item1));
            }
            return SubFolder;
        }

        private void AddFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem viewItem = FolderTreeView.SelectedItem as TreeViewItem;

            if(viewItem != null)
            {
                FolderNameTypeControl folderName = new FolderNameTypeControl(viewItem.Header.ToString())
                {
                    Owner = mainWindow,
                    Width = mainWindow.ActualWidth,
                    Height = mainWindow.ActualHeight
                };

                folderName.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please choose a root folder first!");
            }
        }

        private async void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem viewItem = FolderTreeView.SelectedItem as TreeViewItem;
            await GetAllFileInFolder(viewItem);
        }

        public Task GetAllFileInFolder(TreeViewItem viewItem)
        {
            FileContainer.Children.RemoveRange(0, int.MaxValue);
            foreach(FtpListItem item in mainWindow.client.GetListing(viewItem.Header.ToString()))
            {
                if(item.Type == FtpFileSystemObjectType.File)
                {
                    FileControl fileControl = new FileControl(this, mainWindow.client, item, viewItem);
                    fileControl.FileIcon.Kind = GetFileIcon(System.IO.Path.GetExtension(item.FullName));
                    fileControl.FileNameTxt.Text = item.Name;
                    DateTime date = mainWindow.client.GetModifiedTime(item.FullName);
                    fileControl.DateModified.Text = $"{date.ToLongDateString()} {date.ToShortTimeString()}";
                    fileControl.FileType.Text = System.IO.Path.GetExtension(item.FullName);
                    if (fileControl.FileType.Text == "")
                        fileControl.FileType.Text = "N/A";
                    fileControl.FileSize.Text = GetFileSize(mainWindow.client.GetFileSize(item.FullName));
                    FileContainer.Children.Add(fileControl);
                }
            }
            return Task.CompletedTask;
        }

        private string GetFileSize(long v)
        {
            string str;
            if(v >= 1024 && v < 1048576)
            {
                v /= 1024;
                str = $"{v} KB";
            }
            else if(v >= 1048576)
            {
                v /= 1048576;
                str = $"{v} MB";
            }
            else
            {
                str = $"{v} B";
            }

            return str;
        }

        private PackIconBootstrapIconsKind GetFileIcon(string ext)
        {
            if(ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".gif")
            {
                return PackIconBootstrapIconsKind.FileEarmarkImage;
            }
            else if (ext == ".mp4" || ext == ".mkv" || ext == ".mov" || ext == ".avi" || ext == ".wmv")
            {
                return PackIconBootstrapIconsKind.FileEarmarkPlay;
            }
            else if (ext == ".txt" || ext == ".doc" || ext == ".docx" || ext == ".pdf" || ext == ".ppt" || ext == ".html")
            {
                return PackIconBootstrapIconsKind.FileEarmarkText;
            }
            else if (ext == ".mp3" || ext == ".ogg" || ext == ".m4a" || ext == ".wav")
            {
                return PackIconBootstrapIconsKind.FileEarmarkImage;
            }
            else if (ext == ".zip" || ext == ".rar" || ext == ".iso" || ext == ".tar")
            {
                return PackIconBootstrapIconsKind.FileEarmarkZip;
            }
            else
            {
                return PackIconBootstrapIconsKind.FileEarmark;
            }
        }

        private void UploadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            TreeViewItem viewItem = FolderTreeView.SelectedItem as TreeViewItem;
            OpenFileDialog ofd = new OpenFileDialog();
            Nullable<bool> res = ofd.ShowDialog();
            if(res == true)
            {
                fileName = ofd.FileName;
                string fil = ofd.SafeFileName;
                mainWindow.client.UploadFile(@"" + fileName, viewItem.Header.ToString() + "/" + fil);
                GetAllFileInFolder(viewItem);                
                MessageBox.Show("Upload Complete");

            }
        }

        private void DeleteFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem viewItem = FolderTreeView.SelectedItem as TreeViewItem;
            if(MessageBox.Show("Deleting this folder will also delete its contents\nDo you want to continue?", "Delete Folder", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                mainWindow.client.DeleteDirectory(viewItem.Header.ToString());
                MessageBox.Show($"Folder at location {viewItem.Header} successfully deleted!");
            }
        }
    }

}
