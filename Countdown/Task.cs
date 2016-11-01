using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown
{
    public class Task
    {
        int id;
        string name, description;
        DateTime dueDate;

        public Task(int ID, string Name, string Description, DateTime DueDate)
        {
            id = ID;
            name = Name;
            description = Description;
            dueDate = DueDate;
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
    }
}
