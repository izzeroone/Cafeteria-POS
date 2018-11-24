using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Shapes;

namespace Cafocha.Entities
{
    public partial class Table
    {
        [NotMapped]
        public Rectangle TableRec { get; set; }
    }
}
