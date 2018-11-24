using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public partial class OrderTemp
    {
        [NotMapped]
        public int OrderMode { get; set; }
    }
}
