using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafocha.Entities
{
    public partial class OrderDetailsTemp : INotifyPropertyChanged
    {
        private ObservableCollection<string> _statusItems = new ObservableCollection<string>
            {"BreakFast", "Starter", "Main", "Dessert", "Drink"};

        [NotMapped]
        public ObservableCollection<string> StatusItems
        {
            get => _statusItems;
            set
            {
                _statusItems = value;
                OnPropertyChanged("StatusItems");
            }
        }

        [NotMapped] public string OldStat { get; set; } //OldStat

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class OrderDetailsTemp
    {
        public OrderDetailsTemp()
        {
            InitializePartial();
        }

        public int OrdertempId { get; set; } // ordertemp_id (Primary key)

        public string ProductId { get; set; } // product_id (Primary key) (length: 10)

        public string Note { get; set; } // note (Primary key) (length: 500)

        public int Discount { get; set; } // discount

        public int Quan { get; set; } // quan

        // Foreign keys

        /// <summary>
        ///     Parent OrderTemp pointed by [OrderDetailsTemp].([OrdertempId]) (FK_dbo.OrderDetailsTemp_dbo.OrderTemp_ordertemp_id)
        /// </summary>
        public virtual OrderTemp OrderTemp { get; set; } // FK_dbo.OrderDetailsTemp_dbo.OrderTemp_ordertemp_id

        partial void InitializePartial();
    }
}