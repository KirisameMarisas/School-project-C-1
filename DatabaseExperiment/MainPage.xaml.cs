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

namespace DatabaseExperiment
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        Window fatherWindow; // get the top window to switch different content page
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(Window father)
        {
            fatherWindow = father;
            InitializeComponent();
        }
        // Change to Student Information Page
        private void ChangeToStudent(object sender, RoutedEventArgs e)
        {
            Page student = new ManageStudent(this.fatherWindow);
            this.fatherWindow.Content = student;
        }

        // Change to Teacher Information Page
        private void ChangeToTeacher(object sender, RoutedEventArgs e)
        {
            Page teacher = new ManageTeacher(this.fatherWindow);
            this.fatherWindow.Content = teacher;
        }
    }
}
