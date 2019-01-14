using System.ComponentModel;

namespace Cafocha.Entities
{
    public enum ProductType
    {
        All = -1,
        [Description("Đồ uống")]
        Drink = 1,
        [Description("Topping")]
        Topping = 2,
        [Description("Đồ ngọt")]
        Dessert = 3,
        [Description("Khác")]
        Other = 4
    }
}