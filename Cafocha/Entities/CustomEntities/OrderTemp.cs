using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{

    public partial class OrderTemp
    {

        public int OrdertempId { get; set; } // ordertemp_id (Primary key)

      
        public decimal SaleValue { get; set; } // sale_value


        public decimal TotalPriceNonDisc { get; set; } // totalPrice_nonDisc

      
        public decimal Svc { get; set; } // Svc


        public decimal Vat { get; set; } // Vat

        public string SubEmpId { get; set; } // subEmp_id (length: 200)

        public int Discount { get; set; } // discount

        public int Pax { get; set; } // Pax

        public string CusId { get; set; } // cus_id (length: 10)

        public string EmpId { get; set; } // emp_id (length: 10)

        public int? TableOwned { get; set; } // table_owned

        public System.DateTime Ordertime { get; set; } // ordertime

        public decimal TotalPrice { get; set; } // total_price

        public decimal CustomerPay { get; set; } // customer_pay

        public decimal PayBack { get; set; } // pay_back

        [NotMapped]
        public int OrderMode { get; set; }

        // Reverse navigation

        public virtual System.Collections.Generic.ICollection<OrderDetailsTemp> OrderDetailsTemps { get; set; } // OrderDetailsTemp.FK_dbo.OrderDetailsTemp_dbo.OrderTemp_ordertemp_id

  
      
        public OrderTemp()
        {
            OrderDetailsTemps = new System.Collections.Generic.List<OrderDetailsTemp>();
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
