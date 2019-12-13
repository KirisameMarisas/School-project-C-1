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
    /// Interaction logic for ManageTeacher.xaml
    /// </summary>
    public partial class ManageTeacher : Page
    {
        List<Teacher> teacherList = new List<Teacher>();

        DataTable studentTable = new DataTable();

        Window fatherWindow;
        public ManageTeacher()
        {
            InitializeComponent();
            LoadList();
        }

        public ManageTeacher(Window father)
        {
            fatherWindow = father;
            InitializeComponent();
            LoadList();
        }
        private void ReturnMenu(object sender, RoutedEventArgs e)
        {
            this.fatherWindow.Content = (this.fatherWindow as MainWindow).main;
        }


        private void LoadList()
        {
            teacherList = SqliteDataAccess.LoadTeachers();
            studentTable = ToDataTable(teacherList);


            InitializeDataTable();
            LoadDataTable();
            //defaultData.ItemsSource = teacherList;

            defaultData.ItemsSource = studentTable.DefaultView;

            //defaultData.ItemsSource = testList;
            //defaultData.ItemsSource = students;
        }

        private void InitializeDataTable()
        {
            studentTable = new DataTable();

            studentTable.Columns.Add(new DataColumn("ID", typeof(int)));
            studentTable.Columns.Add(new DataColumn("Name", typeof(string)));

        }

        private void LoadDataTable()
        {
            int length = teacherList.Count;
            for (int i = 0; i < length; ++i)
            {
                DataRow currentRow = studentTable.NewRow();
                currentRow[0] = teacherList[i].id;
                currentRow[1] = teacherList[i].name;

                studentTable.Rows.Add(currentRow);
            }
        }

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

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (defaultData.SelectedIndex < 0)
            {
                return;
            }

            Teacher select = teacherList[defaultData.SelectedIndex];

            if (select.id.ToString() != this.idText.Text.ToString())
            {
                MessageBox.Show("ID haven't been changed!");
                return;
            }
            select.name = this.nameText.Text.ToString() == "" ? null : this.nameText.Text.ToString();

            SqliteDataAccess.SaveTeacher(select);
            LoadList();
        }

        private void AddTeacher(object sender, RoutedEventArgs e)
        {
            Teacher select = new Teacher();
            select.id = teacherList.Count;
            select.name = this.nameText.ToString() == "" ? null : this.nameText.Text.ToString();
            SqliteDataAccess.AddTeacher(select);
            LoadList();
        }

        private void DeleteTeacher(object sender, RoutedEventArgs e)
        {
            if (defaultData.SelectedIndex < 0)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show("This operation is dangerous. Please CHECK it!", "CHECK IT!", MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.Yes:
                {
                    Teacher select = teacherList[defaultData.SelectedIndex];
                    //studentsList.Remove(select);
                    SqliteDataAccess.DeleteTeacher(select);
                    break;
                }
                default: break;
            }
            LoadList();
        }
        private void RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Teacher select = teacherList[defaultData.SelectedIndex];
            this.idText.Text = select.id == null ? "" : select.id.ToString();
            this.nameText.Text = select.name == null ? "" : select.name.ToString();
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            this.idText.Text = "";
            this.nameText.Text = "";
        }

        private void SearchTeacher(object sender, RoutedEventArgs e)
        {
            Teacher select = new Teacher();
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

        private void LoadSearchList(Teacher teacher,string search)
        {
            teacherList = SqliteDataAccess.SearchTeacher(teacher, search);
            studentTable = ToDataTable(teacherList);


            InitializeDataTable();
            LoadDataTable();
            //defaultData.ItemsSource = teacherList;

            defaultData.ItemsSource = studentTable.DefaultView;

            //defaultData.ItemsSource = testList;
            //defaultData.ItemsSource = students;
        }

    }
}
