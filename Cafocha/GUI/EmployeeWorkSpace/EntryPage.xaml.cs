using System.Collections.Generic;
using System.Windows.Controls;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for Entry.xaml
    /// </summary>
    public partial class Entry : Page
    {
        internal OrderTemp orderTemp;
        internal List<OrderDetailsTemp> orderDetails;

        public Entry()
        {
            InitializeComponent();

            orderTemp = new OrderTemp();
            orderDetails = new List<OrderDetailsTemp>();
            ucMenu.setOrderData(orderTemp, orderDetails);
            ucOrder.setOrderData(orderTemp, orderDetails);
        }


    }
}
