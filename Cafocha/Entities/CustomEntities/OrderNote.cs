using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public partial class OrderNote
    {
        [NotMapped] public int paymentMethod { get; set; } // pay_method
    }
}