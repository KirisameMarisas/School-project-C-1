using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseExperiment
{
    /// <summary>
    /// Interaction logic for ManageStudent.xaml
    /// </summary>
    public partial class ManageStudent : Page
    {
        List<Student> studentsList = new List<Student>(); //List all items of students from dabase

        DataTable studentTable = new DataTable(); // Show on DataGrid

        Window fatherWindow; // get Window
        public ManageStudent()
        {
            InitializeComponent();
            LoadList(); 
        }

        public ManageStudent(Window father)
        {
            fatherWindow = father;
            InitializeComponent();
            LoadList();
        }

        // load data from database 
        private void LoadList()
        {
            studentsList = SqliteDataAccess.LoadPeoples();
            studentTable = ToDataTable(studentsList);


            InitializeDataTable();
            LoadDataTable();
            //defaultData.ItemsSource = studentsList;

            defaultData.ItemsSource = studentTable.DefaultView;

            //defaultData.ItemsSource = testList;
            //defaultData.ItemsSource = students;
        }

        private void InitializeDataTable()
        {
            studentTable = new DataTable();

            studentTable.Columns.Add(new DataColumn("ID", typeof(int)));
            studentTable.Columns.Add(new DataColumn("Name", typeof(string)));
            studentTable.Columns.Add(new DataColumn("Class", typeof(string)));
            studentTable.Columns.Add(new DataColumn("Scorce", typeof(double)));
            studentTable.Columns.Add(new DataColumn("Teacher", typeof(string)));

        }

        // load data element to datatable
        private void LoadDataTable()
        {
            int length = studentsList.Count;
            for (int i = 0; i < length; ++i)
            {
                DataRow currentRow = studentTable.NewRow();
                currentRow[0] = studentsList[i].id;
                currentRow[1] = studentsList[i].name;
                currentRow[2] = studentsList[i].classes;
                currentRow[3] = studentsList[i].score;
                currentRow[4] = studentsList[i].teacher;
                studentTable.Rows.Add(currentRow);
            }
        }

        // Fill datatable from list 
        private DataTable ToDataTable<Student>(List<Student> students)
        {
            DataTable dataTable = new DataTable(typeof(Student).Name);

            PropertyInfo[] properties = typeof(Student).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (Student item in students)
            {
                var values = new object[properties.Length];
                int length = properties.Length;
                for (int i = 0; i < length; ++i)
                {
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        // double a row to display information on the TextBox
        private void RowDoubleClick(object sender,MouseButtonEventArgs e)
        {
            Student select = studentsList[defaultData.SelectedIndex];
            this.idText.Text = select.id == null ? "" : select.id.ToString();
            this.classText.Text = select.classes == null ? "" : select.classes.ToString();
            this.nameText.Text = select.name == null ? "" : select.name.ToString();
            this.scorceText.Text = select.score == null ? "" : select.score.ToString();
            this.teacherText.Text = select.teacher == null ? "" : select.teacher.ToString();
        }

        // back to Main Page 
        private void ReturnMenu(object sender, RoutedEventArgs e)
        {
            this.fatherWindow.Content = (this.fatherWindow as MainWindow).main;
        }

        // Edit item after clicking 'Edit' Button
        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (defaultData.SelectedIndex < 0)
            {
                return;
            }
            Student select = studentsList[defaultData.SelectedIndex];

            // id is empty and repeat 
            if (select.id.ToString() != this.idText.Text.ToString())
            {
                MessageBox.Show("ID haven't been changed!");
                return;
            }
            select.name = this.nameText.Text.ToString() == "" ? null : this.nameText.Text.ToString();
            select.classes = this.classText.Text.ToString() == "" ? null : this.classText.Text.ToString();
            select.score = this.scorceText.Text.ToString() == "" ? -1 : (Double.Parse(this.scorceText.Text.ToString()));

            // teacher is exist

            Teacher tempTeacher = new Teacher();
            tempTeacher.name = this.teacherText.Text.ToString();
            List<Teacher> tempList = SqliteDataAccess.SearchTeacher(tempTeacher, "name = @name");
            if (tempList.Count > 0)
            {
                select.teacher = this.teacherText.Text.ToString();
            }else
            {
                MessageBox.Show("This teacher isn't existed in Teacher Table. Please Add it first.");
                LoadList();
                return;
            }
            SqliteDataAccess.SaveStudent(select);
            LoadList();
        }


        //add item after clicking 'Add' Button 
        private void AddStudent(object sender, RoutedEventArgs e)
        {
            Student select = new Student();

            select.id = studentsList.Count;
            select.name = this.nameText.ToString() == "" ? null : this.nameText.Text.ToString();
            select.classes = this.classText.ToString() == "" ? null : this.classText.Text.ToString();
            select.score = this.scorceText.Text.ToString() == "" ? -1 : (Double.Parse(this.scorceText.Text.ToString()));

            //teacher is exist
            Teacher tempTeacher = new Teacher();
            if (this.teacherText.Text.ToString() != "")
            {
                tempTeacher.name = this.teacherText.Text.ToString();
                List<Teacher> tempList = SqliteDataAccess.SearchTeacher(tempTeacher, "name = @name");
                if (tempList.Count > 0)
                {
                    select.teacher = this.teacherText.Text.ToString();
                }
                else
                {
                    MessageBox.Show("This teacher isn't existed in Teacher Table. Please Add it first.");
                    LoadList();
                    return;
                }
            }else
            {
                select.teacher = null;
            }
            
            //studentsList.Add(select);
            SqliteDataAccess.AddStudent(select);
            LoadList();
        }

        // delete item after clicking 'Delete' Button
        private void DeleteStudent(object sender, RoutedEventArgs e)
        {
            if (defaultData.SelectedIndex < 0)
            {
                return;
            }
            MessageBoxResult result = MessageBox.Show("This operation is dangerous. Please CHECK it!","CHECK IT!", MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                {
                    Student select = studentsList[defaultData.SelectedIndex];
                    //studentsList.Remove(select);
                    SqliteDataAccess.DeleteStudent(select);
                    break;
                }
                default: break;
            }
            LoadList();
        }

        // return search result list 
        private void LoadSearchList(Student student, string search)
        {
            studentsList = SqliteDataAccess.SearchStudent(student,search);
            studentTable = ToDataTable(studentsList);


            InitializeDataTable();
            LoadDataTable();
            //defaultData.ItemsSource = studentsList;

            defaultData.ItemsSource = studentTable.DefaultView;

            //defaultData.ItemsSource = testList;
            //defaultData.ItemsSource = students;
        }

        // searching list after click 'Search' button
        private void SearchFromSQL(object sender, RoutedEventArgs e)
        {

            Student select = new Student();
            string search = "";

            bool flag_id = false;
            if (this.idText.Text.ToString() != "")
            {
                flag_id = true;
                select.id = int.Parse(this.idText.Text.ToString());
                //search = "id = " + this.idText.Text.ToString();
                search = "id = @id";
            }
            if (!flag_id)
            {
                if (this.nameText.Text.ToString() != "")
                {
                    select.name = this.nameText.Text.ToString();
                    //search += "name = "+this.nameText.Text.ToString();
                    search += "name = @name";
                }
                if (this.classText.Text.ToString() != "")
                {
                    select.classes = this.classText.Text.ToString();
                    //search += "classes = " + this.classText.Text.ToString();
                    search += "classes = @classes";
                }
                if (this.scorceText.Text.ToString() != "")
                {
                    select.score = Double.Parse(this.scorceText.Text.ToString());
                    //search += "score = " + this.scorceText.Text.ToString();
                    search += "score = @score";
                }
                if (this.teacherText.Text.ToString() != "")
                {
                    select.teacher = this.teacherText.Text.ToString();
                    // search += "teacher = " + this.teacherText.Text.ToString();
                    search += "teacher = @teacher";
                }
            }

            if (search.Length > 0)
            {
                MessageBoxResult result = MessageBox.Show("Will search from no empty box,and the id's priority is the highest", "Please CHECK!", MessageBoxButton.OKCancel);

                switch (result)
                {
                    case MessageBoxResult.OK:
                    {
                        LoadSearchList(select, search);
                        break;
                    }
                    default: return;
                }
            }else
            {
                LoadList();
            }
            
        }

        // reset the TextBox
        private void ResetClick(object sender, RoutedEventArgs e)
        {
            this.idText.Text = "";
            this.nameText.Text = "";
            this.classText.Text = "";
            this.scorceText.Text = "";
            this.teacherText.Text = "";

        }
    }
}
