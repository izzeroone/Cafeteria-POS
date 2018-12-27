﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media.Imaging;

namespace Cafocha.Entities
{
    public partial class Product
    {
        [NotMapped]
        public BitmapImage ImageData
        {
            get { return LoadImage(ImageLink); }
        }

        public static BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri(@"/Images/Products/" + filename, UriKind.RelativeOrAbsolute));
        }
    }
}