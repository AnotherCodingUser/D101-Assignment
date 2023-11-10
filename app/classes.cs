using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Classes;

public class Todo // Todo Class (title, description)
{
    // Define Properties of Todo Class
    private static int CurrentID;

    public int ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public Todo(string title, string desc, int id)
    {
        // Checks if ID -1 (-1 is used as an input when none is provided) 
        if (id == -1)
        {
            // If ID is -1 then it will preform the GetNextID function which will find the next ID avalible
            ID = GetNextID();
        }
        else
        {
            // if an ID is provided then it will set ID to the id provided
            ID = id;
            if (id > CurrentID)
            {
                // if the provided ID is larger than the CurrentID stored in the contructor, then make the CurrentID equal to ID
                CurrentID = id;
            }
        }
        // Set properites to the inputted fields
        Title = title;
        Description = desc;
    }
    // Incrimental ID function so no new ID's are duplicates
    protected int GetNextID() => ++CurrentID;
}
public class TaskItem : Todo  // TaskItem class (title, desc, date(datetime), flag(bool), lables(array), priority, category(int optional))
{
    // TaskItem Properties
    public bool CompletedFlag { get; set; }
    public string DueDate { get; set; }
    public string[] Labels { get; set; }
    public string Priority { get; set; }
    public int CategoryID { get; set; }

    // Task Item Constructor, arguments id and category are both optional, inherits title,desc,id from Todo class
    public TaskItem(string title, string desc, DateTime date, bool flag, string[] labels, string priority, int id = -1, int category = 0) : base(title, desc, id)
    {

        // New properties of the TaskItem Class 
        CompletedFlag = flag;
        DueDate = date.ToString("dd-M-yyyy");
        Labels = labels;
        Priority = priority;
        CategoryID = category;
    }
    // Save method used to write a TaskItem to a file
    public void _Save(StreamWriter sws)
    {
        try
        {
            // Write Line into open StreamWriter File with the TaskItem Properties in a CSV format (Will Join the Labels String Array into a single string with a "=" seperating each label)
            sws.WriteLine($"{ID},{Title},{Description},{DueDate},{CompletedFlag},{string.Join("=", Labels)},{Priority},{CategoryID}");
        }
        catch (Exception e)
        {
            // Catch unexpected exception (Should never happen)
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}
public class Category // Category class (title, id)
{
    // Category Properties 
    private static int CurrentID;

    public int ID { get; set; }
    public string CategoryTitle { get; set; }

    // Category Constructor, id is an optional field which defaults to -1 if not specified
    public Category(string title, int id = -1)
    {
        CategoryTitle = title;
        if (id == -1)
        {
            // Incrimental ID assignment 
            ID = GetNextID();
        }
        else
        {
            ID = id;
            // if inputted id is greater than CurrentID then set currentId to the value of inputted id
            if (id > CurrentID)
            {
                CurrentID = id;
            }
        }
    }
    // incrimental ID assignment function
    protected int GetNextID() => ++CurrentID;
    // Writes Category to a file when called 
    public void CatSave(StreamWriter sw)
    {
        try
        {
            // Write Line into open StreamWriter File with the Category Properties in a CSV format
            sw.WriteLine($"{ID},{CategoryTitle}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }



    }
}
// Class for Logging/Saving lists into csv and mk files
public class ProjectLogs
{
    // Method for saving Task Lists to a csv file
    static public void TaskSave(List<TaskItem> TaskData, string TaskFile = @".\stores\Task.csv")
    {
        // if the TaskFile is equal to the default varible then check if file already exists, if it doesn't create the file
        if (TaskFile == @".\stores\Task.csv")
        {
            if (!File.Exists(TaskFile))
            {
                File.Create(TaskFile);
            }
        }
        // Start a new instance of StremWriter with the filepath of TaskFile
        StreamWriter sw = new StreamWriter(TaskFile);
        //Write the headers for the CSV file
        sw.WriteLine("ID,Title,Description,DueDate,CompletedFlag,Labels,Priority,CategoryID");
        // For each TaskItem in TaskData
        foreach (TaskItem Task in TaskData)
        {
            // Write line for each Task_item in the TaskData List
            Task._Save(sw);
        }
        //Close the file
        sw.Close();
    }
    // Method for Saving Categories into a CSV file
    static public void CategorySave(List<Category> CategoryData, string CategoryFile = @".\stores\Category.csv")
    {
        // if the CategoryFile is equal to the default varible then check if file already exists, if it doesn't create the file
        if (CategoryFile == @".\stores\Category.csv")
        {
            if (!File.Exists(CategoryFile))
            {
                File.Create(CategoryFile);
            }
        }
        // Start a new instance of StremWriter with the filepath of CategoryFile
        StreamWriter sw = new StreamWriter(CategoryFile);
        //Write a second line of text
        sw.WriteLine("ID,CategoryTitle");
        foreach (Category CategoryItem in CategoryData)
        {
            CategoryItem.CatSave(sw);
        }
        //Close the file
        sw.Close();
    }
    // Method for Saving TaskList with Categories into a formatted Markdown File 
    static public string MarkDownSave(List<Category> Categories, List<TaskItem> Tasks)
    {
        // Create the header section of the Markdown table text
        string ReturnString = "| ID | Title | Description | Due Date | Completed? | Labels | Priority | Category |\n| - | - | - | - | - | - | - | - |";
        // For each TaskItem in Tasks return Markdown formatted row for that TaskItem
        foreach (TaskItem Task in Tasks)
        {
            string Category; // This varible will hold the category title used at the end of the row
            // Null check for Category
            if (Categories != null)
            {
                // if CategoryID equals 0 (which means no category for that TaskItem) then the category title will be None
                if (Task.CategoryID == 0)
                {
                    Category = "None";
                }
                else
                {
                    // else find the Category with the ID of the current TaskItem's CategoryID
                    var FoundCategory = Categories.Find(x => x.ID == Task.CategoryID);
                    // Another Null check to make sure Category Title is not null, if it is then return None (A category should never have a null title, this is just error handeling)
                    Category = FoundCategory != null ? FoundCategory.CategoryTitle : "None";
                }
            }
            else
            {
                // if Category List (Categories) is null then return Category title as None
                Category = "None";
            }
            // Append a new Markdown formated table row to the string with the data of this curent TaskItem with the Category Title
            ReturnString += $"\n| {Task.ID} | {Task.Title} | {Task.Description} | {Task.DueDate} | {Task.CompletedFlag} | {string.Join(", ", Task.Labels)} | {Task.Priority} | {Category} |";
        }
        return ReturnString;
    }
}
// Initalizing Class, used for loading the project
public class Intialize
{
    public List<TaskItem> TaskData = new List<TaskItem> { };
    public List<Category> CategoryData = new List<Category> { };
    private List<string[]> reader(string filepath)
    {
        // Create rows string array list to store the lines in the file StreamReader is reading
        List<string[]> rows = new List<string[]>();
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(filepath);
            //Read the first line of text
            var line = sr.ReadLine();
            // Read next line
            line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                string[] content = line.Split(",");
                try
                {
                    // if the line content is greater than 1 (checks if line is not empty)
                    if (content.Length > 1)
                    {
                        rows.Add(content);
                    }
                    //Read the next line
                    line = sr.ReadLine();
                }
                // This should never happen, however if it does it will write the error to a console window and skip that line
                catch (Exception e)
                {
                    Console.Write("Error loading following entry: \n" + line + "\n" + e.Message);
                    line = sr.ReadLine();
                }
            }
            //close the file
            sr.Close();
            // return the rows in a list of String arrays
            return rows;
        }
        catch (Exception e)
        {
            // Write Exception to console
            Console.WriteLine("Exception: " + e.Message);
            // Create a new list for the error to be returned
            List<string[]> error = new List<string[]> { };
            // Add error string
            error.Add(new string[] { "Error", e.Message });
            // Return the error
            return error;
        }
    }

    // Standard run fuction which is called to run all the functions for loading files 
    public void Run(string TaskFile = @"./stores/Task.csv", string CategoryFile = @"./stores/Category.csv")
    {
        // Defines a List of TaskItem's called Tasks
        List<TaskItem> Tasks = new List<TaskItem>();

        // Defines a List of string Arrays which contains the rows of the TaskFile (the return of the reader() function with the input of the TaskFile path)
        List<string[]> TaskRows = reader(TaskFile);
        // For each string array defined as row in TaskRows List
        foreach (string[] row in TaskRows)
        {
            if (row[0] == "Error")
            {
                break;
            }
            try
            {
                // Add a new TaskItem to the Tasks List with the slices of the array input them as properties into the TaskItem Constructor
                Tasks.Add(new TaskItem(
                row[1],
                row[2],
                DateTime.ParseExact(row[3], "d-M-yyyy", CultureInfo.InvariantCulture), // convert string date into DateTime
                bool.Parse(row[4]), //Convert string into bool
                row[5].Split("="), // Convert string into String Array, split string on each =
                row[6],
                Convert.ToInt32(row[0]), //Convert string into int
                Convert.ToInt32(row[7]) //Convert string into int
            ));
            }
            catch (Exception e)
            {
                // Catch unexpected error (should never happen unless file is corrupted, will skip the record if errors)
                Console.WriteLine("Error adding record: \n" + string.Join(", ", row) + "\n" + e.Message);
            }

        }
        // Defines a new List of Category's as Cataegorys
        List<Category> Categorys = new List<Category>();

        // Defines a List of string Arrays which contains the rows of the CategoryFile (the return of the reader() function with the input of the CategoryFile path)
        List<string[]> CategoryRows = reader(CategoryFile);
        // for each string array as row in CategoryRows
        foreach (string[] row in CategoryRows)
        {
            try
            {
                // Add a new Category to the Categories List with the content of the row
                Categorys.Add(new Category(row[1], Convert.ToInt32(row[0])));
            }
            catch (Exception e)
            {
                // Catch unexpected error (should never happen unless file is corrupted, will skip the record if errors)
                Console.WriteLine("Error adding record: \n" + string.Join(", ", row) + "\n" + e.Message);
            }
        }
        // Set Properties of the Intialize class
        TaskData = Tasks;
        CategoryData = Categorys;
    }
}
// Defines a class called PrettyPrint
public class PrettyPrint
{
    // defines a method called PaddingMethodTask
    public void PaddingMethodTask(List<TaskItem> TaskList, List<Category> CategoryList)
    {
        // define and array with a Minimum/Default set of padding integers 
        int[] LengthArray = new int[8] { 2, 6, 13, 12, 11, 7, 12, 8 };

        // This will alter the LengthArray to find the largest value for each property and set the relevant LengthArray slice to equal the largest length of Properties out of all the TaskItems in the TaskList
        foreach (TaskItem Task in TaskList)
        {
            // set the length array value for that property to be the largest value out of the current value set vs the Task's property
            LengthArray[0] = Math.Max(LengthArray[0], Convert.ToString(Task.ID).Length) + 1;         // TaskItem ID
            LengthArray[1] = Math.Max(LengthArray[1], Task.Title.Length) + 1;                        // TaskItem Title 
            LengthArray[2] = Math.Max(LengthArray[2], Task.Description.Length) + 1;                  // TaskItem Description
            LengthArray[5] = Math.Max(LengthArray[5], string.Join(", ", Task.Labels).Length) + 1;  // TaskItem Labels
            LengthArray[7] = Math.Max(LengthArray[7], Convert.ToString(Task.CategoryID).Length) + 1; // TaskItem Catagory ID
        }
        // Create start padding
        string FormattedPadding = "|";
        // Loop through the LengthArray Array processing the lines for the header box and bottom of the table
        foreach (int Column in LengthArray)
        {
            // Adds a whitespace plus as many ='s as the value of Column is (so Column=5 then it would add "=====")
            FormattedPadding += " " + new string('=', Column) + " |";
        }

        Console.WriteLine(FormattedPadding); // Write header padding row formatted like "| ==== | =========== | ============ | ============ |" etc.

        Console.WriteLine("| {0," + LengthArray[0] + "} | {1," + LengthArray[1] + "} | {2," +
            LengthArray[2] + "} | {3,12} | {4,11} | {5," + LengthArray[5] + "} | {6,12} | {7," + LengthArray[7] + "} |", // Write header row column names " |  ID  | Title | Description |" etc.
            "ID", "Title", "Description", "Due Date", "Completed?", "Labels", "Priority", "Category");

        Console.WriteLine(FormattedPadding); // Write header padding row formatted like "| ==== | =========== | ============ | ============ |" etc.


        foreach (TaskItem Task in TaskList)
        {
            string CategoryID;
            if (Task.CategoryID == 0)
            {
                CategoryID = "None";
            }
            else
            {
                // if find the Categroy in CategoryList where ID is equal to that of the Task's CategoryID field, if CatagoryTitle is null then return "None" else return CategoryTitle
                CategoryID = CategoryList.Find(x => x.ID == Task.CategoryID)?.CategoryTitle ?? "None";

            }
            // Write row for Task formated like " |  0  | Title text | Description Text | etc."
            Console.WriteLine("| {0," + LengthArray[0] + "} | {1," + LengthArray[1] + "} | {2," + LengthArray[2] + "} | {3,12} | {4,11} | {5," + LengthArray[5] + "} | {6,12} | {7," + LengthArray[7] + "} |", Task.ID, Task.Title, Task.Description, Task.DueDate, Task.CompletedFlag, string.Join(", ", Task.Labels), Task.Priority, CategoryID);
        }
        // Once all Tasks have been Written to the console write a final padding row formatted like "| ==== | =========== | ============ | ============ |" etc.
        Console.WriteLine(FormattedPadding);
    }
    public void PaddingMethodCategory(List<Category> CategoryList)
    {
        // define and array with a Minimum/Default set of padding integers 
        int[] LengthArray = new int[2] { 1, 1 };

        // This will alter the LengthArray to find the largest value for each property and set the relevant LengthArray slice to equal the largest length of Properties out of all the TaskItems in the CategoryList
        foreach (Category CategoryItem in CategoryList)
        {
            LengthArray[0] = Math.Max(LengthArray[0], Convert.ToString(CategoryItem.ID).Length) + 1; // Category ID
            LengthArray[1] = Math.Max(LengthArray[1], CategoryItem.CategoryTitle.Length) + 1; // Category Title
        }
        // Create start padding
        string FormattedPadding = "|";
        // Loop through the LengthArray Array processing the lines for the header box and bottom of the table
        foreach (int Column in LengthArray)
        {
            // Adds a whitespace plus as many ='s as the value of Column is (so Column=5 then it would add "=====")
            FormattedPadding += " " + new string('=', Column) + " |";
        }
        Console.WriteLine(FormattedPadding);  // Write header padding row formatted like "| ==== | =========== |"
        Console.WriteLine("| {0," + LengthArray[0] + "} | {1," + LengthArray[1] + "} |", "ID", "Title");  // Write header column row formatted like "| ID | Category Title |"
        Console.WriteLine(FormattedPadding);  // Write header padding row formatted like "| ==== | =========== |"


        foreach (Category CategoryItem in CategoryList)
        {
            // Write row for Task formated like " |  0  | Title text |"
            Console.WriteLine("| {0," + LengthArray[0] + "} | {1," + LengthArray[1] + "} |", CategoryItem.ID, CategoryItem.CategoryTitle);
        }
        Console.WriteLine(FormattedPadding); // Write final padding row formatted like "| ==== | =========== |"
    }
}
// String Array converter for Labels editing and display in the WPF application
public class StringArrayToStringConverter : IValueConverter
{
    // When converting the string array to a string
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Null check to see if value is string array
        if (value is string[] stringArray)
        {
            // return the string array as a string joined with a ", " on each slice
            return string.Join(", ", stringArray);
        }
        return string.Empty;
    }

    // When converting the string to a string array
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Null check to see if value is string
        if (value is string stringValue)
        {
            // return the string as a string array split a "," on each slice
            return stringValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }
        return "";
    }
}
// DateTiem converter for Due Date editing and display in the WPF application
public class DateConverter : IValueConverter
{
    // Converting string into DateTime
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Null check to if string will prase into DateTime correctly and is not null
        if (value is string dateString && DateTime.TryParseExact(dateString, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            // return the DateTime result 
            return date;
        }
        // Return null if input is null
        return "";
    }

    // Convert DateTime to String
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if DateTime and not null 
        if (value is DateTime date)
        {
            // Return DateTime prased into a formatted DD-MM-YYY format
            return date.ToString("dd-MM-yyyy");
        }
        // Return null if input is null
        return "";
    }
}
