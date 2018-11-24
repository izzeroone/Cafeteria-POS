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
}