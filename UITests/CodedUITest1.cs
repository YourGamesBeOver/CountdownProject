﻿using System;
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

        }

        [TestMethod]
        public void TestDaysTasksAreShownWhenDaySelected()
        {
            
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

        }

        [TestMethod]
        public void TestCreatingANewSubTaskWithValidValues()
        {

        }

        [TestMethod]
        public void TestCreatingANewSubTaskWithInValidValues()
        {

        }

        [TestMethod]
        public void TestEditTaskName()
        {

        }
        [TestMethod]
        public void TestEditTaskDescription()
        {

        }

        [TestMethod]
        public void TestEditTaskDueDate()
        {

        }

        [TestMethod]
        public void TestDeleteTask()
        {

        }

        [TestMethod]
        public void TestDeleteSubTask()
        {

        }

        [TestMethod]
        public void TestCompleteTask()
        {

        }

        [TestMethod]
        public void TestCompleteSubTask()
        {

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
