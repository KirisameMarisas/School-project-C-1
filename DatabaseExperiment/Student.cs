using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseExperiment
{
    public class Student
    {
        // properties 
        public int id { get; set; }
        public string name { get; set; }
        public string classes{ get; set; } 
        public string teacher{ get; set; }
        public double score { get; set; }

        public Student()
        {

        }
        public Student(int id,string name,string classes,double score,string teacher)
        {
            this.id = id;
            this.name = name;
            this.classes = classes;
            this.score = score;
            this.teacher = teacher;
        }
    }
}
