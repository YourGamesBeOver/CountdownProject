using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Input;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting.DirectUIControls;
using Microsoft.VisualStudio.TestTools.UITesting.WindowsRuntimeControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace UITests {
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest(CodedUITestType.WindowsStore)]
    public class CodedUITest1 {
        public CodedUITest1() {
        }

        [TestMethod]
        public void TestIfListViewButtonIsPresent()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            this.UIMap.TestListViewButtonIsPresent();

        }

        [TestMethod]
        public void HamburgerButtonOpensSidebarMenu()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.HamburgerButton);
            Assert.AreEqual(200, this.UIMap.UICountdownWindow.UIPaneRootWindow.UIMyListBoxList.Width, "The List Box is not open");
        }

        [TestMethod]
        public void TestListViewIsShownWhenListViewButtonIsPressed()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            this.UIMap.UICountdownWindow.SidebarMenuListBox.ListViewButton.Select();
            Assert.AreEqual(true, this.UIMap.UICountdownWindow.TaskListBox.Exists, "List View is not shown");
            Assert.AreEqual(true, UIMap.UICountdownWindow.TaskListBox.Enabled,"List View is not Enabled");
        }

        [TestMethod]
        public void TestCalendarViewIsShownWhenCalendarViewButtonIsPressed()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            this.UIMap.UICountdownWindow.SidebarMenuListBox.CalendarViewButton.Select();
            Assert.AreEqual(true, this.UIMap.UICountdownWindow.MyCalendar.Exists, "Calendar is not Shown");
            Assert.AreEqual(true, UIMap.UICountdownWindow.MyCalendar.Enabled, "Calendar is not Enabled");
        }

        [TestMethod]
        public void TestDetailedInfoShownWhenTaskIsSelectedFromTaskView()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddTaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Sample Task";
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text =
                "Sample Description";
            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton);
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.TaskListBoxFirstItem);

            Assert.AreEqual("Task Name: Sample Task", this.UIMap.UICountdownWindow.DetailedViewTaskName.DisplayText, "Details Not Shown");
            Assert.AreEqual("Description: Sample Description", this.UIMap.UICountdownWindow.DetailedViewDescription.DisplayText, "Details Not Shown");
        }

        [TestMethod]
        public void TestDetailedInfoShownWhenTaskIsSelectedFromCalendarView()
        {
            /*
             *  1) Open app
             *  2) Navigate to Calendar view
             *  3) create a task
             *  4) Click on a day
             *  5) Click on specific task
             *  6) Check that the specific details appear  
             */
        }

        [TestMethod]
        public void TestDaysTasksAreShownWhenDaySelected()
        {
            /*
                *  1) Open app
                *  2) Navigate to Calendar view
                *  3) create a task
                *  4) Click on a day
                *  5) check for specific task 
            */
        }

        [TestMethod]
        public void TestCreatingANewTaskWithValidValues()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddTaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Sample Task";
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text =
                "Sample Description";
            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton);

            Assert.AreEqual(1, this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "Item Not Created");

        }

        [TestMethod]
        public void TestCreatingANewTaskWithInValidValues()
        {
            /*
                *  1) Open app
                *  2) open create a task dialog
                *  3) enter invalid info
                *  4) check for error message
                *  5) check that task is not created
            */
        }

        [TestMethod]
        public void TestCreatingANewSubTaskWithValidValues()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) create subtask
                *  5) check if subtask created
            */
        }

        [TestMethod]
        public void TestCreatingANewSubTaskWithInValidValues()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) create subtask
                *  5) check for correct error message
            */
        }

        [TestMethod]
        public void TestEditTaskName()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) click edit task
                *  5) edit name
                *  6) check that name is different
            */
        }
        [TestMethod]
        public void TestEditTaskDescription()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) click edit task
                *  5) edit description
                *  6) check that description is different
            */
        }

        [TestMethod]
        public void TestEditTaskDueDate()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) click edit task
                *  5) edit due date
                *  6) check that due date is different
            */
        }

        [TestMethod]
        public void TestDeleteTaskWithNoTasks()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIRemoveTaskButton);
            Assert.AreEqual("No Tasks to delete", this.UIMap.UICountdownWindow.UIContentScrollViewerPane.NoTasksToDeleteText.DisplayText, "Message saying no tasks should be displayed");
        }

        [TestMethod]
        public void TestDeleteTaskWithTasks()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddTaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Sample Task";
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text =
                "Sample Description";
            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton);

            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIRemoveTaskButton);

            Assert.AreEqual("Sample Task", this.UIMap.UICountdownWindow.DeleteTaskList.TaskToDelete.DisplayText, "Task not displayed in delete dialog");
            Mouse.Click(this.UIMap.UICountdownWindow.DeleteTaskList.TaskToDelete);
            Mouse.Click(this.UIMap.UICountdownWindow.RemoveTaskDialog.DeleteTaskButton);

            Assert.AreEqual(0, this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "Item not deleted");
        }

        [TestMethod]
        public void TestDeleteSubTask()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) add subtask
                *  4) click delete
                *  5) click delete subtask
                *  6) choose subtask
                *  7) check subtask is removed
            */
        }

        [TestMethod]
        public void TestCompleteTask()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click complete task
                *  4) check that app is considered completed
            */
        }

        [TestMethod]
        public void TestCompleteSubTask()
        {
            /*
                *  1) Open app
                *  2) create new task
                *  3) click on task
                *  4) create subtask
                *  5) click complete/complete subtask
                *  6) make sure subtask is considered complete
            */
        }

        [TestMethod]
        public void TestAddTagToTask()
        {

        }

        [TestMethod]
        public void TestSignUpNewUserWithValidValues()
        {

        }

        [TestMethod]
        public void TestSignUpNewUserWithInValidValues()
        {

        }

        [TestMethod]
        public void TestLogInAsARegisteredUser()
        {

        }

        [TestMethod]
        public void TestSearchForExistingTask()
        {

        }

        [TestMethod]
        public void TestSearchForNonExistingTask()
        {

        }


        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap {
            get {
                if ((this.map == null)) {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
