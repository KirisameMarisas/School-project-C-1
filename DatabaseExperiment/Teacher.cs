using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseExperiment
{
    public class Teacher
    {
        // properties 
        public int id { set; get; }
        public string name { set; get; }

        public Teacher()
        {

        }

        public Teacher(int id ,string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
