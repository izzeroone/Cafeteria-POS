using System.Windows;
using System.Windows.Controls;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Page
    {
        private Employee emp = Application.Current.Properties["EmpLogin"] as Employee;
        private string formatDate = "yyyy-MM-dd";

        public Info()
        {
            InitializeComponent();

            //  initEmployeeInfo();
        }

        //private void initEmployeeInfo()
        //{
        //    txtUsername.Text = emp.Username;
        //    txtPass.Password = emp.Pass.ToString(); ;
        //    txtName.Text = emp.Name;
        //    txtBirth.Text = emp.Birth.ToString(formatDate);
        //    txtAddr.Text = emp.Addr;
        //    txtEmail.Text = emp.Email;
        //    txtPhone.Text = emp.Phone;
        //    txtStartDay.Text = emp.Startday.ToString(formatDate);
        //    txtHourWage.Text = emp.Hour_wage.ToString();
        //}
    }
}