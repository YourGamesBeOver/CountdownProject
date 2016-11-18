using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown
{
    public class Task
    {
        private int id;
        private string name, description;
        private DateTime dueDate;
        private List<Task> subtasks;
        private bool completed;
        private TimeSpan remainingTime;

        public Task(int ID, string Name, string Description, DateTime DueDate, List<Task> Subtasks, bool Completed, TimeSpan RemainingTime)
        {
            id = ID;
            name = Name;
            description = Description;
            dueDate = DueDate;
            subtasks = Subtasks;
            completed = Completed;
            remainingTime = RemainingTime;
        }

        public int ID
        {
            get
            {
                return id;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
        }
        public DateTime DueDate
        {
            get
            {
                return dueDate;
            }
        }

        public List<Task> Subtasks
        {
            get
            {
                return subtasks;
            }
        }

        public bool Completed
        {
            get
            {
                return completed;
            }
        }

        public TimeSpan RemainingTime
        {
            get
            {
                return remainingTime;
            }
            set { remainingTime = value; }
        }
    }
}
