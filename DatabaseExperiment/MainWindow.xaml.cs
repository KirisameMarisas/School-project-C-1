using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.OleDb;
using System.Data;
using System.Reflection;

namespace DatabaseExperiment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Student> studentsList = new List<Student>();

        DataTable studentTable = new DataTable();
        public MainPage main { get; set; } // Main Page 
        public MainWindow()
        {
            InitializeComponent();
            main = new MainPage(this);
            this.Content = main;
        }

      
    }
}
