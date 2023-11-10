using Classes;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Linq;
using System.Diagnostics;

namespace test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;
        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void Test1() // Check if TaskItem is correctly initalizing and saving data correctly, including date translation to String from DateTime
        {
            DateTime TestingDate = DateTime.Today;
            TaskItem NewTask = new TaskItem("title", "description", TestingDate, true, new string[] { "label1", "label2" }, "Low");
            bool result = (NewTask.Title == "title") && (NewTask.Description == "description") && (NewTask.DueDate == TestingDate.ToString("dd-MM-yyyy"));
            Assert.True(result, $"Task Failed to build Correctly");
        }
        [Fact]
        public void Test2() // Check if TaskItem is correctly making an auto incrimented TaskItem ID
        {
            DateTime TestingDate = DateTime.Today;
            List<TaskItem> TaskList = new List<TaskItem>();
            int count = 0;
            for ( int i = 0; i < 4; i++ )
            {
                TaskList.Add(new TaskItem("title", "description", TestingDate, true, new string[] { "label1", "label2" }, "Low"));
                count++;
            }
            TaskItem LastItem = TaskList.Last();
            Assert.True(LastItem.ID == 5, $"Tasks failed to auto incriment correctly");
        }
        [Fact] // Check if Category's Initalize correctly
        public void Test3()
        {
            Category NewCategory = new Category("Title");
            Assert.True((NewCategory.ID == 1 && NewCategory.CategoryTitle == "Title"),"Category Failed to Initialize");
        }

        [Fact]
        public void Test4() // Check if Category is correctly making an auto incrimented Category ID 
        {
            List<Category> CategoryList = new List<Category>();
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                CategoryList.Add(new Category("title"));
                count++;
            }
            Category LastItem = CategoryList.Last();
            Assert.True(LastItem.ID == 5, $"Category failed to auto incriment correctly");
        }
        [Fact]
        public void Test5() // Check loading and saving functionality of TaskItems
        {
            DateTime TestingDate = DateTime.Today;
            List<TaskItem> TaskList = new List<TaskItem>();
            for (int i = 0;i < 4;i++)
            {
                TaskList.Add(new TaskItem("title", "description", TestingDate, true, new string[] { "label1", "label2" }, "Low"));
            }
            ProjectLogs.TaskSave(TaskList, "TaskTest.csv");
            Intialize startup = new Intialize();
            startup.Run("TaskTest.csv","");
            File.Delete("TaskTest.csv");
            Assert.True(startup.TaskData.Count() == 4, $"Error Writing and Loading TaskItems to CSV File");

        }
        [Fact]
        public void Test6() // Check loading and saving functionality of Categories
        {
            List<Category> CategoryList = new List<Category>();
            for ( int i = 0; i < 4; i++)
            {
                CategoryList.Add(new Category("title"));
            }
            ProjectLogs.CategorySave(CategoryList, "CategoryTest.csv");
            Intialize startup = new Intialize();
            startup.Run("", "CategoryTest.csv");
            File.Delete("CategoryTest.csv");
            Assert.True(startup.CategoryData.Count() == 4, $"Error Writing and Loading Categorys to CSV File");
        }
    }
}