using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.DirectoryServices;
using Classes;

namespace CLI;

public class Cli
{
    // Start function which is called in App.xaml.cs to start the CLI 
    public static void Start()
    {
        // Start startup intialization 
        Intialize startup = new();
        // Load the Task.csv and Category.csv
        startup.Run();

        // Initalize a List of Categories 
        List<Category> CategoryList = new List<Category> { };
        if (startup.CategoryData != null)
        {
            // if CategoryData is not null then Assign CategoryData to CategoryList
            CategoryList = startup.CategoryData;
        }

        // Initalize a List of TaskItems 
        List<TaskItem> TaskList = new List<TaskItem> { };
        if (startup.TaskData != null)
        {
            // if TaskData is not null then Assign TaskData to TaskList
            TaskList = startup.TaskData;
        }
        PrettyPrint Printer = new PrettyPrint();

        // Null check method
        string NullCheck(string? input)
        {
            if (input != null)
            {
                // if input is not null return input, else return empty string
                return input;
            }
            return " ";
        }
        // Checks the List of Tasks and will return then next ID not in use for incrimental ID assignment
        int CheckOpenTaskID(List<TaskItem> TaskList)
        {
            // Create a list to store open ID's
            List<int> OpenTaskIDs = new List<int>();
            // Assign a default ID to start at
            int CurrentTaskIDCheck = 1;
            foreach (TaskItem Task in TaskList)
            {

                while (CurrentTaskIDCheck != Task.ID)
                {
                    // If CurrentTaskIDCheck (Expected next ID) does not equal Task.ID then add CurrentTaskIDCheck to the OpenTaskIDs List
                    OpenTaskIDs.Add(CurrentTaskIDCheck);
                    // Incriment CurrentTaskIDCheck, loop until CurrentTaskIDCheck is equal to Task.ID
                    CurrentTaskIDCheck++;
                }
                CurrentTaskIDCheck++;
            }
            // If there is no open Task ID's in the TaskList then return -1 (-1 is a int declared to create a new ID in the TaskItem class)
            if (!OpenTaskIDs.Any())
            {
                // returns auto incriment value
                return -1;
            }
            // return first item in the OpenTaskIDs list
            return OpenTaskIDs.First();
        }
        // Checks the List of Categories and will return then next ID not in use for incrimental ID assignment
        int CheckOpenCategoryID(List<Category> CategoryList)
        {
            // Create a list to store open ID's
            List<int> OpenCategoryIDs = new List<int>();
            // Assign a default ID to start at
            int CurrentCategoryIDCheck = 1;
            foreach (Category CategoryItem in CategoryList)
            {

                while (CurrentCategoryIDCheck != CategoryItem.ID)
                {
                    // If CurrentCategoryIDCheck (Expected next ID) does not equal CategoryItem.ID then add CurrentCategoryIDCheck to the OpenCategoryIDs List
                    OpenCategoryIDs.Add(CurrentCategoryIDCheck);
                    // Incriment CurrentCategoryIDCheck, loop until CurrentCategoryIDCheck is equal to CategoryItem.ID
                    CurrentCategoryIDCheck++;
                }
                CurrentCategoryIDCheck++;
            }
            // If there is no open Category IDs in the CategoryList then return -1 (-1 is a int declared to create a new ID in the Category class)
            if (!OpenCategoryIDs.Any())
            {
                // returns auto incriment value
                return -1;
            }
            // return first item in the OpenCategoryIDs list
            return OpenCategoryIDs.First();
        }

        // Method for removing a task
        void RemoveTask(List<TaskItem> TaskList, int TaskID, List<Category> CategoryList)
        {
            // Delcare DeletedTask of the TaskItem which has the ID of the users input TaskID
            TaskItem? DeletedTask = TaskList.Find(x => x.ID == TaskID);

            // If a TaskItem is found with the ID of TaskID
            if (DeletedTask != null)
            {
                Console.Clear();
                Console.WriteLine("Deleting the following record:");
                // Write to console the TaskItem that is getting deleted
                Printer.PaddingMethodTask(new List<TaskItem>() { DeletedTask }, CategoryList);
                // Delete the TaskItem that the user wants to delete
                TaskList.Remove(DeletedTask);
            }
            // If no TaskItem is found with the ID of TaskID
            else
            {
                // Write to console that no TaskItem was found 
                Console.WriteLine("Task with ID " + TaskID + " not found.");
            }
        }

        // Method for adding a new task, contains all the prompting( Console.Read()'s )
        void AddTask(List<TaskItem> TaskList, List<Category> CategoryList)
        {
            // Delcare variables for user inputs, and declare default values
            string? InputTitle = "";
            string? InputDescription = "";
            string? InputDueDate = "";
            DateTime InputDateParsed = DateTime.Now;
            bool InputCompletedFlagPrased = false;
            string? InputPriority = "";

            // Loop to check that User input for the Title field is not null
            while (true)
            { // Title user input
                Console.Write("\nTitle of task: ");
                // Check if Variable is null
                InputTitle = NullCheck(Console.ReadLine());
                if (InputTitle.Trim() != "")
                {
                    // if InputTitle is not null and does not contain whitespace, break out of loop 
                    break;
                }

                // if input is null or whitespace then write out invalid message
                Console.Write($"You can't have {InputTitle} as a Title");
            }

            while (true)
            { // Description user input
                Console.Write("\nDescription of task: ");
                // Check if InputDescription is null
                InputDescription = NullCheck(Console.ReadLine());
                if (InputDescription.Trim() != "")
                {
                    // if InputDescription is not null and does not contain whitespace, break out of loop 
                    break;
                }
                // if input is null or whitespace then write out invalid message
                Console.Write($"You can't have {InputDescription} as a Description");
            }

            while (true) // Date user input
            {
                Console.Write("\nDue Date of task (DD-MM-YYYY): ");
                // Check if InputDueDate is null
                InputDueDate = NullCheck(Console.ReadLine());
                try
                {
                    // Prase user input string into a DateTime value
                    InputDateParsed = DateTime.ParseExact(InputDueDate, "d-M-yyyy", CultureInfo.InvariantCulture);
                    // if InputDateParsed is not null and does not contain whitespace, break out of loop 
                    break;
                }
                catch
                {   // if input is null or whitespace then write out invalid message
                    Console.WriteLine("Error prasing time, did you use dd-mm-yy?");
                }
            }

            while (true) // CompletedFlag user input
            {
                Console.Write("\nHas Task been completed?: ");
                // Check if user input is null
                string? InputCompletedFlag = NullCheck(Console.ReadLine());
                // Declare true inputs
                string[] AgreeArray = new string[] { "y", "Y", "Yes", "yes", "YES", "true", "True", "TRUE" };
                // If InputCompletedFlag is in AgreeArray then declare InputCompletedFlagPrased as true
                if (AgreeArray.Any(InputCompletedFlag.Contains))
                {
                    InputCompletedFlagPrased = true;
                    // break out of loop
                    break;
                }
                // Else keep InputCompletedFlagPrased as default, which is false
                else
                {
                    // break out of loop
                    break;
                }
            }

            Console.Write("\nLabels of task (Seperate each label with a comma, or leave it blank if you have no labels): ");
            // Declear variables for user Label input
            string? InputLables = NullCheck(Console.ReadLine());
            // split user input on each , detected
            string[] InputLablesPrased = InputLables.Split(",");

            while (true) // Priority user input
            {
                Console.Write("\nPriority of task (Low, Medium, High):");
                // check if InputPriority is null
                InputPriority = NullCheck(Console.ReadLine());
                // delcare options avalaibe to be inputted for Priority
                string[] AgreeArray = new string[] { "Low", "low", "LOW", "Medium", "medium", "MEDIUM", "High", "high", "HIGH" };
                //Check if InputPriority is in the AgreeArray
                if (AgreeArray.Any(InputPriority.Contains))
                {
                    break;
                }
                // if InputPriority is in not the AgreeArray write to the console that the input is invalid
                Console.WriteLine($"{InputPriority} Is an invalid option");
            }
            int prasedint;
            while (true)
            { // Category input

                Console.WriteLine("");
                // Write Category list to the console in a nice padded and formatted table
                Printer.PaddingMethodCategory(CategoryList);
                Console.Write("\nCategory of task (Leave blank if no category): ");
                // check if userinput is null
                string? input = NullCheck(Console.ReadLine());

                // input is not null or whitespace
                if (input.Trim() != "")
                {
                    // Try to convert string input into an integer
                    if (int.TryParse(input, out int _))
                    {
                        try
                        {
                            prasedint = Convert.ToInt32(input);
                            // check if any of the Categories CategoryList has the Category.ID of user input
                            if (!CategoryList.Any(item => item.ID == prasedint))
                            {
                                // if there is no category with that Category.ID then write error message
                                Console.WriteLine($"No Category with the ID of {prasedint} found");
                            }
                            break;


                        }
                        catch (Exception e)
                        {
                            // Catch any unexpected errors 
                            Console.WriteLine($"Error checking if category exists: \n{e.Message}");
                            continue;
                        }
                    }
                    else
                    {
                        // If user doesn't provide a valid integer 
                        Console.WriteLine($"{input} is not a valid number!");
                        continue;
                    }
                }
                else
                {
                    // if user input is null or whitespace set the category field to 0 (0 means no category for this TaskItem)
                    prasedint = 0;
                    break;
                }
            }
            // Using all validiated inputs, create a new TaskItem
            TaskItem NewTask = new TaskItem(InputTitle, InputDescription, InputDateParsed, InputCompletedFlagPrased, InputLablesPrased, InputPriority, CheckOpenTaskID(TaskList), prasedint);

            Console.Clear();
            Console.WriteLine("Adding new record: ");
            // Write new TaskItem to console
            Printer.PaddingMethodTask(new List<TaskItem>() { NewTask }, CategoryList);
            // Add new TaskItem to TaskList
            TaskList.Add(NewTask);
            // Sort TaskList by Task.ID
            TaskList.Sort((x, y) => x.ID.CompareTo(y.ID));



        }

        // Method for Deleting a Category
        void RemoveCategory(List<Category> CategoryList, List<TaskItem> TaskList, int CategoryID)
        {
            // Find a Category that matches the the user input of CategoryID to a Category with the same ID, return null if no category found
            Category? DeleteCategory = CategoryList.Find(x => x.ID == CategoryID);
            Console.Clear();
            Console.WriteLine("Deleting the following record: ");
            // If a Category is found with the ID of CategoryID
            if (DeleteCategory != null)
            {
                // Write to console the Category in a formatted tabular method
                Printer.PaddingMethodCategory(new List<Category>() { DeleteCategory });
                // Remove Category from the Category List
                CategoryList.Remove(DeleteCategory);
                // Sort Categories in the Category List by Category ID
                CategoryList.Sort((x, y) => x.ID.CompareTo(y.ID));

                // Check the TaskList for any TaskItems with the Task.CategoryID of the now deleted category
                foreach (TaskItem Task in TaskList)
                {
                    // if a task if found with the same CategoryID as the deleted category set that task to have no Category (int of 0)
                    if (Task.CategoryID == CategoryID)
                    {
                        Task.CategoryID = 0;
                    }
                }
            }
            else
            {
                // if no category is found from the user's input then Write to console error message
                Console.WriteLine("Category with ID " + CategoryID + " not found.");
            }
        }

        // Method for creating a new Category
        void AddCategory(List<Category> CategoryList)
        {
            Console.Write("What is the title of the new Category?\n");
            // Check if user input is null
            string? UserCategoryInput = NullCheck(Console.ReadLine());

            // if user input is not null
            if (UserCategoryInput.Trim() != "")
            {
                // Create a new category with the Category Title of the user input
                Category NewCategory = new Category(UserCategoryInput, CheckOpenCategoryID(CategoryList));
                Console.Clear();
                Console.WriteLine("Adding new record: ");
                // write to console the new category
                Printer.PaddingMethodCategory(new List<Category> { NewCategory });
                // Add new category to the Category List
                CategoryList.Add(NewCategory);
                // Sort Category List by Category ID
                CategoryList.Sort((x, y) => x.ID.CompareTo(y.ID));
            }
            else
            {
                // if user input is empty or null
                Console.WriteLine("You didn't input anything!");
            }
        }

        void EnterContinue() // A simple pause until user continues method
        {
            Console.WriteLine("\nPress Enter to continue");
            Console.Read();
        }

        while (true)
        {
            Console.Clear();
            // Write options avalible to console
            Console.WriteLine("1. List Tasks \n2. Add Task \n3. Remove Task \n4. List Categories \n5. Add Category \n6. Remove Category\n7. Save to disk");
            Console.Write("Option: ");
            // check if input is null
            string? input = NullCheck(Console.ReadLine());
            switch (input)
            {
                case "1": // List Tasks
                    Console.Clear();
                    // List TaskList in a neatly padded tabular format
                    Printer.PaddingMethodTask(TaskList, CategoryList);
                    // await user input to continue back to main menu
                    EnterContinue();
                    break;
                case "2": // Add Task
                    Console.Clear();
                    // List TaskList in a neatly padded tabular format
                    Printer.PaddingMethodTask(TaskList, CategoryList);
                    // Call AddTask Method to begin the process of adding a new TaskItem to TaskList
                    AddTask(TaskList, CategoryList);
                    // await user input to continue back to main menu
                    EnterContinue();
                    Console.Clear();
                    break;
                case "3": // Remove task
                    Console.Clear();
                    // List TaskList in a neatly padded tabular format
                    Printer.PaddingMethodTask(TaskList, CategoryList);
                    Console.Write("\nWhat task would you like to remove? (Use the ID to select the task): ");
                    // Null cehck user input
                    string TaskRemoveInput = NullCheck(Console.ReadLine());
                    try
                    {
                        // Conver userinput into an Integer
                        int filtered = Convert.ToInt32(TaskRemoveInput);
                        // start the remove task process by calling the RemoveTask method
                        RemoveTask(TaskList, filtered, CategoryList);
                        // await user input to continue back to main menu
                        EnterContinue();
                        Console.Clear();
                    }
                    catch
                    {
                        // if there is an error with the user input, return an error message
                        Console.WriteLine($"Uncountered an issue with ID input: {TaskRemoveInput}");
                        // await user input to continue back to main menu
                        EnterContinue();
                    }
                    break;
                case "4": // List Categories
                    Console.Clear();
                    // List CategoryList in a neatly padded tabular format
                    Printer.PaddingMethodCategory(CategoryList);
                    // await user input to continue back to main menu
                    EnterContinue();
                    break;
                case "5": // Add Category
                    Console.Clear();
                    // List CategoryList in a neatly padded tabular format
                    Printer.PaddingMethodCategory(CategoryList);
                    AddCategory(CategoryList);
                    // await user input to continue back to main menu
                    EnterContinue();
                    Console.Clear();
                    break;
                case "6": // Remove Category
                    Console.Clear();
                    // List CategoryList in a neatly padded tabular format
                    Printer.PaddingMethodCategory(CategoryList);
                    Console.Write("\nWhat Category would you like to remove? (Use the ID to select the task): ");
                    // check if user input is null
                    string CategoryRemoveInput = NullCheck(Console.ReadLine());
                    try
                    {
                        int filtered = Convert.ToInt32(CategoryRemoveInput);
                        RemoveCategory(CategoryList, TaskList, filtered);
                        // await user input to continue back to main menu
                        EnterContinue();
                        Console.Clear();
                    }
                    catch
                    {
                        // if there is an error with the user input, return an error message
                        Console.WriteLine($"Uncountered an issue with ID input: {CategoryRemoveInput}");
                        // await user input to continue back to main menu
                        EnterContinue();
                    }
                    break;
                case "7": // Save to Disk
                    ProjectLogs.CategorySave(CategoryList);
                    ProjectLogs.TaskSave(TaskList);
                    Console.WriteLine("Saved Category List and Task List to Disk at /stores/");
                    // await user input to continue back to main menu
                    EnterContinue();
                    Console.Clear();
                    break;
                default:
                    Console.WriteLine($"{input} Is an invalid option");
                    // await user input to continue back to main menu
                    EnterContinue();
                    break;


            }
        }
    }
}