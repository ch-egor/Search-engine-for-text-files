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

namespace SearchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppController appController;
        
        public MainWindow()
        {
            InitializeComponent();
            appController = new AppController();
            this.DataContext = appController;
        }

        private void IncludeFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog();
            if (result == true)
                appController.IncludeFiles(dialog.FileNames);
        }

        private void ExcludeFile_Click(object sender, RoutedEventArgs e)
        {
            List<string> files = new List<string>();
            foreach (FileDescription item in IncludedFiles.SelectedItems)
                files.Add(item.FullName);
            appController.ExcludeFiles(files);
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            appController.Find();
        }

        private void DisplayIndex_Click(object sender, RoutedEventArgs e)
        {
            IndexWindow indexWindow = new IndexWindow();
            indexWindow.Index.DataContext = appController;
            indexWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            appController.Save();
        }
    }
}
