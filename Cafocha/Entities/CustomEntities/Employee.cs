using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public enum EmployeeRole
    {
        Counter = 0,
        Stock = 3
    }

    public partial class Employee
    {
        [NotMapped] public string DecryptedPass { get; set; }

        [NotMapped] public string DecryptedCode { get; set; }
    }
}