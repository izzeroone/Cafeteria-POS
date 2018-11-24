namespace Cafocha
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderNote")]
    public partial class OrderNote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderNote()
        {
            OrderNoteDetails = new HashSet<OrderNoteDetail>();
        }

        [Key]
        [StringLength(10)]
        public string ordernote_id { get; set; }

        public int pay_method { get; set; }

        [Column(TypeName = "money")]
        public decimal sale_value { get; set; }

        [Column(TypeName = "money")]
        public decimal totalPrice_nonDisc { get; set; }

        [Column(TypeName = "money")]
        public decimal Svc { get; set; }

        [Column(TypeName = "money")]
        public decimal Vat { get; set; }

        [StringLength(200)]
        public string subEmp_id { get; set; }

        public int discount { get; set; }

        public int Pax { get; set; }

        [StringLength(10)]
        public string cus_id { get; set; }

        [StringLength(10)]
        public string emp_id { get; set; }

        public int ordertable { get; set; }

        public DateTime ordertime { get; set; }

        [Column(TypeName = "money")]
        public decimal total_price { get; set; }

        [Column(TypeName = "money")]
        public decimal customer_pay { get; set; }

        [Column(TypeName = "money")]
        public decimal pay_back { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Employee Employee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderNoteDetail> OrderNoteDetails { get; set; }
    }
}
