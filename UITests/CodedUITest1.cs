using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Input;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting.DirectUIControls;
using Microsoft.VisualStudio.TestTools.UITesting.WindowsRuntimeControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using Windows.UI.Xaml.Controls;

namespace UITests {
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest(CodedUITestType.WindowsStore)]
    public class CodedUITest1 {

        public CodedUITest1() {

        }

        //pick any unused username
        private string username = "User6";

        [TestMethod]
        public void TestSignUpNewUserWithValidValues()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");

            this.UIMap.UICountdownWindow.LoginUserName.Text = username;
            this.UIMap.UICountdownWindow.LoginPassword.Text = "whatever";
            Mouse.Click(this.UIMap.UICountdownWindow.RegisterButton);

            Assert.AreEqual(true, this.UIMap.UICountdownWindow.UISearchBar.Enabled, "Did not register user");
        }

        [TestMethod]
        public void TestSignUpNewUserWithInValidValues()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.SidebarMenuListBox.SettingsViewButton);
            Mouse.Click(this.UIMap.UICountdownWindow.LogOutButton);

            this.UIMap.UICountdownWindow.LoginUserName.Text = username;
            this.UIMap.UICountdownWindow.LoginPassword.Text = "whatever";
            Mouse.Click(this.UIMap.UICountdownWindow.RegisterButton);

            Mouse.Click(this.UIMap.UICountdownWindow);
            Assert.AreEqual(true, this.UIMap.ErrorWindow.Enabled, "Error Not Displayed");
            Mouse.Click(this.UIMap.ErrorWindow.ErrorWindowButtons.CloseButton);
        }

        [TestMethod]
        public void TestLogIn()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            this.UIMap.UICountdownWindow.LoginUserName.Text = username;
            this.UIMap.UICountdownWindow.LoginPassword.Text = "whatever";
            Mouse.Click(this.UIMap.UICountdownWindow.LoginButton);
            Assert.AreEqual(true, this.UIMap.UICountdownWindow.UISearchBar.Exists);
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
        public void TestErrorShownWhenCompleteClickedWithNoSelectedTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UICompleteTaskButton);
            Assert.AreEqual(true,this.UIMap.ErrorWindow.Enabled, "Error message not displayed");
        }

        [TestMethod]
        public void TestErrorShownWhenAddSubtaskClickedWithNoSelectedTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddSubtaskButton);
            Assert.AreEqual(true, this.UIMap.ErrorWindow.Enabled, "Error message not displayed");
        }

        [TestMethod]
        public void TestErrorShownWhenEditClickedWithNoSelectedTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIEditTaskButton);
            Assert.AreEqual(true, this.UIMap.ErrorWindow.Enabled, "Error message not displayed");
        }

        [TestMethod]
        public void TestCreatingANewTaskWithInValidValues()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddTaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Sample Task";

            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton);

            Assert.AreEqual(true, this.UIMap.ErrorWindow.Enabled, "Error Message not shown");

            Mouse.Click(this.UIMap.ErrorWindow.ErrorWindowButtons.CloseButton);

            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskCancelButton);

            Assert.AreEqual(0, this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "Item Created");
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
        public void TestDetailedInfoShownWhenTaskIsSelectedFromTaskView()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);

            Assert.AreEqual("Sample Task", this.UIMap.UICountdownWindow.DetailsTaskNameText.DisplayText, "Name Not Shown");
            Assert.AreEqual("Sample Description", this.UIMap.UICountdownWindow.DetailsTaskDescriptionText.DisplayText, "Description Not Shown");

        }

        [TestMethod]
        public void TestCreatingANewSubTaskWithInValidValues()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);

            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddSubtaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UISubtaskNameText.NameTextBox.Text = "Sample Subtask";

            Mouse.Click(this.UIMap.UICountdownWindow.UIAddSubtaskWindow.AddSubtaskButton);

            Assert.AreEqual(true, this.UIMap.ErrorWindow.Enabled, "Error Message not shown");
            Mouse.Click(this.UIMap.ErrorWindow.ErrorWindowButtons.CloseButton);
        }

        [TestMethod]
        public void TestCreatingANewSubTaskWithValidValues()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");

            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);

            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddSubtaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UISubtaskNameText.NameTextBox.Text = "Sample Subtask";
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text =
                "Sample Description";
            Mouse.Click(this.UIMap.UICountdownWindow.UIAddSubtaskWindow.AddSubtaskButton);

            Assert.AreEqual(1, this.UIMap.UICountdownWindow.SubtaskListBox.Items.Count, "Subtask not created");
        }

        [TestMethod]
        public void TestEditTaskName()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIEditTaskButton);

            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Edited Task";
            Mouse.Click(this.UIMap.UICountdownWindow.UIEditTaskWindow.UIUpdateButton);
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);

            Assert.AreEqual("Edited Task", this.UIMap.UICountdownWindow.EditedTaskText.DisplayText, "Name not edited");
        }

        [TestMethod]
        public void TestEditTaskDescription()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIEditTaskButton);

            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text = "Edited Description";
            Mouse.Click(this.UIMap.UICountdownWindow.UIEditTaskWindow.UIUpdateButton);
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);

            Assert.AreEqual("Edited Description", this.UIMap.UICountdownWindow.EditedDescriptionText.DisplayText, "Description not edited");
        }

        [TestMethod]
        public void TestEditSubtask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Mouse.Click(this.UIMap.UICountdownWindow.UISubtaskListBoxList.FirstSubtask);
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIEditTaskButton);

            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UISubtaskNameText.NameTextBox.Text = "Edited Subtask";
            Mouse.Click(this.UIMap.UICountdownWindow.UIEditTaskWindow.UIUpdateButton);

            Assert.AreEqual("Edited Subtask", this.UIMap.UICountdownWindow.UISubtaskListBoxList.FirstSubtask.EditedSubtaskText.DisplayText, "Description not edited");
        }

        [TestMethod]
        public void TestCompleteTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UICompleteTaskButton);

            Assert.AreEqual(true, this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem.Checkmark.Enabled, "Item not completed");
        }

        [TestMethod]
        public void TestCompleteSubTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");

            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Mouse.Click(this.UIMap.UICountdownWindow.UISubtaskListBoxList.FirstSubtask);

            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UICompleteTaskButton);

            Assert.AreEqual(true, this.UIMap.UICountdownWindow.UISubtaskListBoxList.FirstSubtask.CheckMark.Enabled, "Subtask not marked as complete");
        }

        [TestMethod]
        public void TestDeleteSubTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");

            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);

            Mouse.Click(this.UIMap.UICountdownWindow.UISubtaskListBoxList.FirstSubtask);

            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIRemoveTaskButton);
            Mouse.Click(this.UIMap.UICountdownWindow.UIDeleteWindow.UIYesButton);

            Assert.AreEqual(0, this.UIMap.UICountdownWindow.UISubtaskListBoxList.Items.Count, "Subtask not deleted");
        }

        [TestMethod]
        public void TestDeleteTaskWithTasks()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIRemoveTaskButton);
            Mouse.Click(this.UIMap.UICountdownWindow.UIDeleteWindow.UIYesButton);

            Assert.AreEqual(0, this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "Item not deleted");
        }

        [TestMethod]
        public void TestDeleteTaskWithNoTasks()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIRemoveTaskButton);
            Mouse.Click(this.UIMap.UICountdownWindow);
            Assert.AreEqual(true, this.UIMap.ErrorWindow.Enabled, "Message saying no tasks should be displayed");
        }

        [TestMethod]
        public void TestSearchForExistingTask()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddTaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Sample Task 1";
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text =
                "Sample Description";
            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton);

            Mouse.Click(this.UIMap.UICountdownWindow.UIMyCommandBarCustom.UIAddTaskButton);
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Find();
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UITaskNameText.AddTaskTextBox.Text = "Task 2";
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Find();
            this.UIMap.UICountdownWindow.UIContentScrollViewerPane.UIDescriptionText.DescriptionTextBox.Text =
                "Sample Description";
            this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton.Find();
            Mouse.Click(this.UIMap.UICountdownWindow.UIAddTaskWindow.AddTaskAddButton);

            this.UIMap.UICountdownWindow.UISearchBar.SearchBar.Text = "Sample";
            Assert.AreEqual(1, this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "Search did not work");

            this.UIMap.UICountdownWindow.UISearchBar.SearchBar.Find();
            this.UIMap.UICountdownWindow.UISearchBar.SearchBar.Text = "task 2";
            Mouse.Click(this.UIMap.UICountdownWindow.TaskListBox.FirstListBoxItem);
            Assert.AreEqual(1, this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "search didn't work with different capitalization");

            this.UIMap.UICountdownWindow.UISearchBar.SearchBar.Find();
            this.UIMap.UICountdownWindow.UISearchBar.SearchBar.Text = "3";

            Mouse.Click(this.UIMap.UICountdownWindow);
            Assert.AreEqual(0,this.UIMap.UICountdownWindow.TaskListBox.Items.Count, "Search did not work");

        }

        [TestMethod]
        public void TestDaysTasksAreShownWhenDaySelected()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.SidebarMenuListBox.CalendarViewButton);

            //must set to current day
            Mouse.Click(this.UIMap.UICountdownWindow.CalendarView.December7);

            Assert.AreEqual(2, this.UIMap.UICountdownWindow.DayTaskListBox.Items.Count, "Day does not contain task");
        }

        [TestMethod]
        public void TestCalendarViewClickedDayShowsNoEvents()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.SidebarMenuListBox.CalendarViewButton);

            Mouse.Click(this.UIMap.UICountdownWindow.CalendarView.December3);

            Assert.AreEqual(0, this.UIMap.UICountdownWindow.DayTaskListBox.Items.Count, "Day does not contain task");

            Assert.AreEqual("No Tasks", this.UIMap.UICountdownWindow.NoTasksText.DisplayText, "Text Not Displayed");
        }

        [TestMethod]
        public void TestLogOut()
        {
            XamlWindow.Launch("b708e79f-bf08-442b-b3f1-6b3d8ee1315f_mdkh4ynn2814y!App");
            Mouse.Click(this.UIMap.UICountdownWindow.SidebarMenuListBox.SettingsViewButton);
            Mouse.Click(this.UIMap.UICountdownWindow.LogOutButton);

            Assert.AreEqual(true, this.UIMap.UICountdownWindow.LoginButton.Exists);
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
