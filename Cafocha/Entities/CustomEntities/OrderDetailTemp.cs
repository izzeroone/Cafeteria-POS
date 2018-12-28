using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public partial class OrderDetailsTemp : INotifyPropertyChanged
    {
        private string _oldstat;
        private ObservableCollection<string> _statusItems = new ObservableCollection<string> { "BreakFast", "Starter", "Main", "Dessert", "Drink" };

        public event PropertyChangedEventHandler PropertyChanged;

        [NotMapped]
        public ObservableCollection<string> StatusItems
        {
            get { return _statusItems; }
            set
            {
                _statusItems = value;
                OnPropertyChanged("StatusItems");
            }
        }

        [NotMapped]
        public string OldStat
        {
            get { return _oldstat; }
            set { _oldstat = value; }
        }//OldStat

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class OrderDetailsTemp
    {

        public int OrdertempId { get; set; } // ordertemp_id (Primary key)

        public string ProductId { get; set; } // product_id (Primary key) (length: 10)

        public string SelectedStats { get; set; } // SelectedStats (Primary key) (length: 50)

        public string Note { get; set; } // note (Primary key) (length: 500)

        public int IsPrinted { get; set; } // is_printed (Primary key)

        public int Discount { get; set; } // discount

        public int Quan { get; set; } // quan

        // Foreign keys

        /// <summary>
        /// Parent OrderTemp pointed by [OrderDetailsTemp].([OrdertempId]) (FK_dbo.OrderDetailsTemp_dbo.OrderTemp_ordertemp_id)
        /// </summary>
        public virtual OrderTemp OrderTemp { get; set; } // FK_dbo.OrderDetailsTemp_dbo.OrderTemp_ordertemp_id

        public OrderDetailsTemp()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}