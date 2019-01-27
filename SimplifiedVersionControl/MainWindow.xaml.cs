using GitParser;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Linq;

namespace SimplifiedVersionControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GitCommands gitCommands;
        public static string RepositoryPath { get; set; }

        public MainWindow()
        {
            gitCommands = new GitCommands();
            InitializeComponent();
            InitRepository();
            PopulateGitHistory();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                foreach (string filename in openFileDialog.FileNames)
                    lbFiles.Items.Add(System.IO.Path.GetFileName(filename));
            }
        }

        private void btnCommitFiles_click(object sender, RoutedEventArgs e)
        {

        }

        #region Custom initialization

        private void InitRepository()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (isoStore.FileExists("credentials.txt"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("credentials.txt", System.IO.FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        RepositoryPath = reader.ReadToEnd().Trim();
                    }
                }
            }
            else
            {
                // Open new Form with FileChooser
                // On save save in IsolatedStorage the path!
                InitSettings settings = new InitSettings();
                //settings.Focus();
                settings.Show();
                settings.Activate();
                settings.Focus();
            }
        }

        private void PopulateGitHistory()
        {
            if (!string.IsNullOrWhiteSpace(RepositoryPath))
            {
                var commits = gitCommands.DoRun(new string[] {RepositoryPath });
                //List<string> headers = new List<string>();
                //commits.ForEach(e => { headers.Add(e.GetHeader()); });
                var collection = new ObservableCollection<GitCommit>(commits);
                commitList.ItemsSource = commits;

            }
        }

        #endregion
    }
}
