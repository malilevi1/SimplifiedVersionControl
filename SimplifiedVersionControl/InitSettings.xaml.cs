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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO.IsolatedStorage;
using System.IO;

namespace SimplifiedVersionControl
{
    /// <summary>
    /// Interaction logic for InitSettings.xaml
    /// </summary>
    public partial class InitSettings : Window
    {
        public InitSettings()
        {
            InitializeComponent();
            this.Focus();
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                repositoryPath.Text = dialog.SelectedPath;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(repositoryPath.Text))
            {
                System.Windows.MessageBox.Show("Please choose a path");
                return;
            }

            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("credentials.txt", FileMode.CreateNew, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(repositoryPath.Text);
                }
            }
            MainWindow.RepositoryPath = repositoryPath.Text;
            this.Close();
        }
    }
}
