using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Kiro.Resources;
using System.Xml.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace To_Do_List
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Supported Orientations
            SupportedOrientations = SupportedPageOrientation.Portrait;

            // Load tasks
            LoadTasks();
            
        }

        // Collection to hold tasks
        // Saves into an xml file called Tasks
        ObservableCollection<string> Tasks = new ObservableCollection<string>();

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            string selectedTask = ((CheckBox)sender).DataContext.ToString();
            if (MessageBox.Show(string.Format("Delete {0} ?", selectedTask), "To-Do List",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK) 
            {
                Tasks.Remove(selectedTask);
                SaveTasks();
            }
        }
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewTask.Text))
            {
                Tasks.Add(txtNewTask.Text);
                txtNewTask.Text = string.Empty;
                SaveTasks();
            }
        }

        // Uses IsolatedStorage to retrieve saved tasks
        private void LoadTasks()
        {
            IsolatedStorageFile isfData = IsolatedStorageFile.GetUserStoreForApplication();
            XDocument doc = null;
            IsolatedStorageFileStream isfStream = null;

            if (isfData.FileExists("Tasks.xml"))
            {
                isfStream = new IsolatedStorageFileStream("Tasks.xml", FileMode.Open, isfData);
                doc = XDocument.Load(isfStream);
                isfStream.Close();
            }
            else
            {
                doc = XDocument.Load("Tasks.xml");
                isfStream = new IsolatedStorageFileStream("Tasks.xml", FileMode.CreateNew, isfData);
                doc.Save(isfStream);
                isfStream.Close();
            }

            var taskList = from t in doc.Descendants("Task")
                           select t.Value;

            foreach (var task in taskList)
            {
                Tasks.Add(task);
            }

            bToDoList.ItemsSource = Tasks;
        }

        // Save tasks
        private void SaveTasks()
        {
            var tasksToSave = (from t in Tasks
                               select new XElement("Task", new XElement("Name", t))).ToList();
            try
            {
                XElement xml = new XElement("Tasks", tasksToSave);

                IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream("Tasks.xml",
                    FileMode.Truncate, IsolatedStorageFile.GetUserStoreForApplication());

                xml.Save(isfStream);
                isfStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}