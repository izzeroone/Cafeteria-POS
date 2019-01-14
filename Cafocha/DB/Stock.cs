//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cafocha.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Stock
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stock()
        {
            this.StockInDetails = new HashSet<StockInDetail>();
            this.StockOutDetails = new HashSet<StockOutDetail>();
        }
    
        public string sto_id { get; set; }
        public string apwarehouse_id { get; set; }
        public string name { get; set; }
        public string st_id { get; set; }
        public string unit { get; set; }
        public decimal standard_price { get; set; }
        public string info { get; set; }
        public string supplier { get; set; }
        public int deleted { get; set; }
    
        public virtual APWareHouse APWareHouse { get; set; }
        public virtual StockType StockType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockInDetail> StockInDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockOutDetail> StockOutDetails { get; set; }
    }
}