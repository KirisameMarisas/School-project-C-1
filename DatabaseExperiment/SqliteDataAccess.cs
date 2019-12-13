using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace DatabaseExperiment
{
    public class SqliteDataAccess
    {

        // return list from database's "Student" table 
        public static List<Student> LoadPeoples()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Student>("select * from Student", new DynamicParameters());
                return output.ToList();
            }
        }

        // return list from database's "Teachers" table 
        public static List<Teacher> LoadTeachers()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Teacher>("select * from Teachers", new DynamicParameters());
                return output.ToList();
            }
        }

        // get Database Source string 
        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        // get search list from "Student" table 
        public static List<Student> SearchStudent(Student student, string search)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                if (search.Length == 0)
                {
                    return LoadPeoples();
                }
                string searchString = "select * from Student where "+search;
                var output = cnn.Query<Student>(searchString, student);
                return output.ToList();
            }
        }

        // get search list from "Teacher" table 
        public static List<Teacher> SearchTeacher(Teacher teacher, string search)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                if (search.Length == 0)
                {
                    return LoadTeachers();
                }
                string searchString = "select * from Teachers where " + search;
                var output = cnn.Query<Teacher>(searchString, teacher);
                return output.ToList();
            }
        }

        // saving item to "Teachers" table
        public static void SaveTeacher(Teacher teacher)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("update Teachers set id = @id ,name = @name where id = @id", teacher);
            }
        }
        // saving item to "Student" table 
        public static void SaveStudent(Student student)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("update Student set id = @id ,name = @name,classes = @classes,Score = @score,Teacher = @teacher where id = @id", student);
            }
        }

        // adding item to "Teachers" table 
        public static void AddTeacher(Teacher teacher)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Teachers (name) values(@name)", teacher);
            }
        }

        // adding item to "Student" table 
        public static void AddStudent(Student student)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Student (name,classes,Score,Teacher) values(@name,@classes,@score,@teacher)", student);
            }
        }

        // delete item from "Teachers" table 
        public static void DeleteTeacher(Teacher teacher)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Student where id = @id", teacher);
            }
        }

        // delete item from "Student" table 
        public static void DeleteStudent(Student student)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from Student where id = @id",student);
            }
        }
    }
}
