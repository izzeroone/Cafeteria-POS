using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public partial class StockOutDetail
    {
        [NotMapped] public decimal ItemPrice { get; set; }
    }
}