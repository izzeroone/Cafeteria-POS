using System.Linq;
using System.Windows;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for EmployeeChangePass.xaml
    /// </summary>
    public partial class EmployeeChangePass : Window
    {
        private EmployeewsOfLocalPOS _cloudPosUnitofwork;
        private Employee _emp;

        public EmployeeChangePass(EmployeewsOfLocalPOS cloudPosUnitofwork, Employee emp)
        {
            _cloudPosUnitofwork = cloudPosUnitofwork;
            InitializeComponent();
            _emp = emp;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string oldPass = txtPass.Password.Trim();
            if (!oldPass.Equals(_emp.Pass))
            {
                MessageBox.Show("Your old password is incorrect!");
                txtPass.Focus();
                return;
            }

            string newPass = txtNewPass.Password.Trim();
            if (newPass.Length == 0 || newPass.Length > 50)
            {
                MessageBox.Show("New password is not valid!");
                txtNewPass.Focus();
                return;
            }

            string passcon = txtConNew.Password.Trim();
            if (!passcon.Equals(newPass))
            {
                MessageBox.Show("Confirm new password is not match!");
                txtConNew.Focus();
                return;
            }

            _emp.Pass = newPass;
            _cloudPosUnitofwork.EmployeeRepository.Update(_emp);
            _cloudPosUnitofwork.Save();

            var emplog = EmpLoginListData.emploglist.Where(x => x.Emp.Username.Equals(_emp.Username)).First();
            if(emplog != null)
            {
                emplog.Emp.Pass = newPass;
            }

            MessageBox.Show("Your password was changed!");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
