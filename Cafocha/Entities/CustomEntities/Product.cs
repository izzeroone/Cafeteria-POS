using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media.Imaging;

namespace Cafocha.Entities
{
    public partial class Product
    {
        private static readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        [NotMapped] public BitmapImage ImageData => LoadImage(ImageLink);

        public static BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri(startupProjectPath + @"/Images/Products/" + filename, UriKind.Absolute));
        }

        public override string ToString()
        {
            return ProductId;
        }
    }
}