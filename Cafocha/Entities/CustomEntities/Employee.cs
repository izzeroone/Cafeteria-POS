using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public enum EmployeeRole
    {
        Ministering = 0,
        Bar = 1,
        Kitchen = 2,
        Stock = 3,
        Cashier = 4
    }

    public partial class Employee
    {
        [NotMapped]
        public string DecryptedPass { get; set; }
        [NotMapped]
        public string DecryptedCode { get; set; }
    }
}
