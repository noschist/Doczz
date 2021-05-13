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
    /// Interaction logic for CusFolder.xaml
    /// </summary>
    public partial class CusFolder : UserControl
    {
        public CusFolder(string name)
        {
            InitializeComponent();
            FolderNameTxt.Text = name;
        }
    }
}
