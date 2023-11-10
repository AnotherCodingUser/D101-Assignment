using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
using Microsoft.Win32;
using System.IO;
using Classes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window // Create mainwindow class
    {
        ObservableCollection<Category> CategoryList = new ObservableCollection<Category>();
        ObservableCollection<TaskItem> TaskList = new ObservableCollection<TaskItem> ();
        public MainWindow()
        {
            // Intailize window and componetns
            InitializeComponent();
            // Bind Category List and Task List to their respect ListBox and Datagrid in the application
            CategoryDataList.ItemsSource = CategoryList;
            TaskListGrid.DataContext = TaskList;

            // Set the default states for the buttons on the application
            LinkTaskCategoryBtn.Visibility = Visibility.Collapsed;
            ButtonUpdate(TaskList, CategoryList);

        }
        
        // Create method to check the first avaliable Task ID not in use for incirmental TaskItem creation
        int CheckOpenTaskID(List<TaskItem> TaskList)
        {
            // For loop going through all the TaskItems in the TaskList
            for (int i = 0; i < TaskList.Count(); i++)
            {
                // Check if the current TaskItem.ID does not match the expected count 
                if (i + 1 != TaskList[i].ID)
                {
                    // if it does not match then return the inciremntal value for the ID
                    return i + 1;
                }
            }
            // if no gaps were found in the TaskItem.ID's then autoincriment from the last record
            return TaskList.Count + 1;
        }

        // Create method to check the first avaliable Category ID not in use for incirmental Category creation
        int CheckOpenCategoryID(List<Category> CategoryList)
        {
            // For loop going through all the Categories in the CategoryList
            for (int i = 0; i < CategoryList.Count(); i++)
            {
                // Check if the current Category.ID does not match the expected count
                if (i + 1 != CategoryList[i].ID)
                {
                    // if it does not match then return the inciremntal value for the ID
                    return i +1;
                }
            }
            // if no gaps were found in the Category.ID's then autoincriment from the last record
            return CategoryList.Count + 1;
        }

        // Method for handeling button status updates
        void ButtonUpdate(ObservableCollection<TaskItem> TaskList, ObservableCollection<Category> CategoryList)
        {
            // Create a variable to store the selected object(Category or TaskItem)
            Category SelectedCategory = (Category)CategoryDataList.SelectedItem;
            TaskItem SelectedTask = (TaskItem)TaskListGrid.SelectedItem;

            // If there is more than 1 category in the the CategoryList then display the save button
            if (CategoryList.Count() > 0)
            {
                SaveCatBtn.Visibility = Visibility.Visible;
                
            }
            // If there is no categories in the CategoryList then hide it
            if (CategoryList.Count() == 0)
            {
                SaveCatBtn.Visibility = Visibility.Collapsed;
            }
            // If there is a current Selected Category then make the Edit and Delete buttons visible
            if (SelectedCategory != null)
            {
                EditCatBtn.Visibility = Visibility.Visible;
                DeleteCatBtn.Visibility = Visibility.Visible;
            }
            // If there is no current Selected Category then make the Edit and Delete buttons hidden
            if (SelectedCategory == null)
            {
                CategoryTxt.Text = "";
                EditCatBtn.Visibility = Visibility.Collapsed;
                DeleteCatBtn.Visibility = Visibility.Collapsed;
            }
            // If there is more than 1 TaskItem in the TaskList the display the Save and Delete buttons
            if (TaskList.Count() > 0)
            {
                SaveTaskListBtn.Visibility = Visibility.Visible;
                DeleteTaskBtn.Visibility = Visibility.Visible;
            }
            // If there is no TaskItems in the TaskList then hide the Save and Delete buttons
            if (TaskList.Count() == 0)
            {
                SaveTaskListBtn.Visibility = Visibility.Collapsed;
                DeleteTaskBtn.Visibility = Visibility.Collapsed;
            }
            // If there is a current Selected TaskItem then make the Delete button visible
            if (SelectedTask != null)
            {
                DeleteTaskBtn.Visibility = Visibility.Visible;
            }
            // If there is no current Selected TaskItem then make the Delete button hidden
            if (SelectedTask == null)
            {
                DeleteTaskBtn.Visibility = Visibility.Collapsed;
            }
            // If both TaskList and CategoryList contain more than 1 record make the Markdwow button visible
            if ((TaskList.Count() > 0) && (CategoryList.Count() > 0))
            {
                MakeMarkdownBtn.Visibility = Visibility.Visible;
            }
            // Else if either of the Lists are empty then hide the Markdown button
            if ((TaskList.Count() == 0) || (CategoryList.Count() == 0))
            {
                MakeMarkdownBtn.Visibility = Visibility.Collapsed;
            }
            // If there is a current Selected Category and a Selected TaskItem then make the LinkTaskCategoryBtn button visible
            if ((SelectedCategory != null) && (SelectedTask != null))
            {
                LinkTaskCategoryBtn.Visibility = Visibility.Visible;
            }
            // If a Category and TaskItem is not currently selected then make the LinkTaskCategoryBtn button hidden
            else
            {
                LinkTaskCategoryBtn.Visibility = Visibility.Collapsed;
            }
        }

        // Create a FileNameGet Method which opens a OS dialog prompt where the user selects a file
        string FileNameGet (string FileType)
        {
            // Create the OpenDialog object
            OpenFileDialog OpenDialog = new OpenFileDialog();
            // Create a filter to only find .csv files
            OpenDialog.Filter = $"{FileType} File(*.csv)| *.csv";
            // if the file dialog opens correctly, return the file name selected by the user
            if (OpenDialog.ShowDialog() == true)
            {
                return OpenDialog.FileName;
            }
            // if there is an error creating the file dialog return an error
            return "error";
        }

        // Add Category Button functionality
        private void AddCatBtn_Click(object sender, RoutedEventArgs e)
        {
            // Add a New Category object to the CategoryList using the Auto Incriment function
            CategoryList.Add(new Category("New", CheckOpenCategoryID(CategoryList.ToList())));
            // Order the CategoryList by Category ID's
            CategoryList =  new ObservableCollection<Category>(CategoryList.OrderBy(i => i.ID));
            // Rebind the CategoryList to the Listbox
            CategoryDataList.ItemsSource = CategoryList;
            // Update the button statuses
            ButtonUpdate(TaskList, CategoryList);
        }

        // Edit Category button functionality
        private void EditCatBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected category
            Category SelectedCategory = (Category)CategoryDataList.SelectedItem;
            // Button should be hidden if no Category is selected, but this is an extra check
            if (SelectedCategory != null)
            {
                // Set the Selected Category Title to the Category Text that has been typed in the Category Title Texbox
                SelectedCategory.CategoryTitle = CategoryTxt.Text;
                // Rebind the CategoryList to the CategoryListbox
                CategoryDataList.ItemsSource = CategoryList;
            }
        }

        // Delete Category button functionality
        private void DeleteCatBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the Selected category
            Category SelectedCategory = (Category)CategoryDataList.SelectedItem;
            // Error checking to see if the current selected category is null (Button shouldn't even be visible but just good to check)
            if (SelectedCategory != null)
            {
                // Removed the selected category from CategoryList
                CategoryList.Remove(SelectedCategory);
                // Cycle through each TaskItem in the TaskList
                foreach (TaskItem Task in TaskList)
                {
                    // if a TaskItem if found with the CategoryID of the SelectedCategory to be deleted
                    if (Task.CategoryID == SelectedCategory.ID) 
                    {
                        // Set the Found TaskItem's CategoryID to 0 (No Category will have the ID of 0)
                        Task.CategoryID = 0;
                        // Refresh the Datagrid for TaskList by unbinding and rebinding it
                        TaskListGrid.ItemsSource = null;
                        TaskListGrid.ItemsSource = TaskList;
                    }
                }
            }
            // Reorder Category List by ID
            CategoryList = new ObservableCollection<Category>(CategoryList.OrderBy(i => i.ID));
            // Rebind CategoryList to the Listbox
            CategoryDataList.ItemsSource = CategoryList;
            // Update buttons statuses
            ButtonUpdate(TaskList, CategoryList);
        }
       
        // Load Category Button functionality
        private void LoadCatBtn_Click(object sender, RoutedEventArgs e)
        {
            // Reference the current selected category in the ListBox of Categories
            Category SelectedCategory = (Category)CategoryDataList.SelectedItem;
            // Check if any category is selected (this can cause errors when loading new categories)
            // Extra null check to make sure SelectedCategory is not null
            if (SelectedCategory != null)
            {
                // Remove selected Category from the CategoryList
                CategoryList.Remove(SelectedCategory);
            }
            // Re Intialize the Loading class
            Intialize startup = new Intialize();
            // Run the loading function with the filepath of the return of FileNameGet method
            startup.Run(CategoryFile: FileNameGet("Category CSV"));
            // Load the CategoryData into a ObservableCollection of Categories as CategoryList
            CategoryList = new ObservableCollection<Category>(startup.CategoryData);
            // Bind CategoryList to the Listbox
            CategoryDataList.ItemsSource = CategoryList;
            // Update Button Statuses
            ButtonUpdate(TaskList, CategoryList);
        }
       
        // Save Category button functionallity
        private void SaveCatBtn_Click(object sender, RoutedEventArgs e)
        {
            // Initalize Save Dialog
            SaveFileDialog SaveDialog = new SaveFileDialog();
            // Define the Extension and default name for the file
            SaveDialog.Filter = "Category File(*.csv)| *.csv";
            SaveDialog.FileName = "Category";
            // If the Save Dialog initalized fine
            if (SaveDialog.ShowDialog() == true)
            {
                // Save the Category with the user selected Filename
                ProjectLogs.CategorySave(CategoryList.ToList(), SaveDialog.FileName);
            }
            else
            {
                // If there is an error in intalization of the dialog then alert user
                MessageBox.Show("Unable to save file, try again.");
            }
        }

        // Category Listbox functionality and events
        private void CategoryDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Define the current selected Category
            Category SelectedCategory = (Category)CategoryDataList.SelectedItem;
            // Check if any category was selected
            if (SelectedCategory != null)
            {
                // Set the Text in the Category Title Textbox to equal that of the Selected Category's Title
                CategoryTxt.Text = SelectedCategory.CategoryTitle;
            }
            // Update button statuses
            ButtonUpdate(TaskList, CategoryList);

        }

        // Load Task button functionality
        private void LoadTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            // reintialize the Initialize object
            Intialize startup = new Intialize();
            // Load the user selected file into the run method
            startup.Run(TaskFile: FileNameGet("Task CSV"));
            // create a new ObservableCollection with the new TaskData
            TaskList = new ObservableCollection<TaskItem>(startup.TaskData);
            // Update the button selection
            ButtonUpdate(TaskList, CategoryList);
            // Refresh the Datagrid content by clearing it and then assigning TaskList to it
            TaskListGrid.DataContext = null;
            TaskListGrid.DataContext = TaskList;
        }

        // Task DataGrid selection event and functionality
        private void TaskListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If a task is selected then update button selection
            ButtonUpdate(TaskList, CategoryList);
            // All editing functionality for the TaskItems is done via the inbuilt Datagrid controls

        }

        // Delete TaskItem Functionlaity
        private void DeleteTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            // Define the Selected TaskItem in the Datagrid
            TaskItem SelectedTask = (TaskItem)TaskListGrid.SelectedItem;
            // Null check for Selected task
            if (SelectedTask != null)
            {
                // Remove Selected task from TaskList
                TaskList.Remove(SelectedTask);
                // Clear TaskList DataGrid, then assign new TaskList to Datagrid
                TaskListGrid.DataContext = null;
                TaskListGrid.DataContext = TaskList;
                // Update Button statuses
                ButtonUpdate(TaskList, CategoryList);
            }
        }

        // Add Task button functionality
        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            // Add new Task with default values and auto incirmented ID
            TaskList.Add(new TaskItem("new title", "description", DateTime.Now, false, new string[] {"Label1","Label2"}, "Low", CheckOpenTaskID(TaskList.ToList())));
            // Order TaskList by TaskItem ID
            TaskList = new ObservableCollection<TaskItem>(TaskList.OrderBy(i => i.ID));
            // Clear TaskList DataGrid, then assign new TaskList to Datagrid
            TaskListGrid.ItemsSource = null;
            TaskListGrid.ItemsSource = TaskList;
        }

        // Link Task to Category Button functionality
        private void LinkTaskCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the Selected Category and TaskItem
            Category SelectedCategory = (Category)CategoryDataList.SelectedItem;
            TaskItem SelectedTask = (TaskItem)TaskListGrid.SelectedItem;
            // Set the CategoryID of the TaskItem to equal the Selected Category ID
            SelectedTask.CategoryID = SelectedCategory.ID;
            // Clear TaskList DataGrid, then assign new TaskList to Datagrid
            TaskListGrid.ItemsSource = null;
            TaskListGrid.ItemsSource = TaskList;
            // Update Button Status
            LinkTaskCategoryBtn.Visibility = Visibility.Collapsed;
            // Unselect Category
            CategoryDataList.UnselectAll();

        }

        // Save Task Button functionality
        private void SaveTaskListBtn_Click(object sender, RoutedEventArgs e)
        {
            // Initalize Save Dialog
            SaveFileDialog SaveDialog = new SaveFileDialog();
            // Define the Extension and default name for the file
            SaveDialog.Filter = "Task List File(*.csv)| *.csv";
            SaveDialog.FileName = "Task";
            // If the Save Dialog initalized fine
            if (SaveDialog.ShowDialog() == true)
            {
                // Save the TaskItem with the user selected Filename
                ProjectLogs.TaskSave(TaskList.ToList(), SaveDialog.FileName);
            }
            else
            {
                // If there is an error in intalization of the dialog then alert user
                MessageBox.Show("Unable to save file, try again.");
            }
        }

        // Markdown Button functionality
        private void MakeMarkdownBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create new Dialog object
            SaveFileDialog SaveDialog = new SaveFileDialog();
            // Set Dialog filter
            SaveDialog.Filter = "Markdown File(*.md)| *.md";
            // If dialog initalizes properly
            if (SaveDialog.ShowDialog() == true)
            {
                // Create the markdown file
                File.WriteAllText(SaveDialog.FileName, ProjectLogs.MarkDownSave(CategoryList.ToList(), TaskList.ToList()));
            }
            else
            {
                // If there is an error in intalization of the dialog then alert user
                MessageBox.Show("Unable to save file, try again.");
            }
        }


    }


}
