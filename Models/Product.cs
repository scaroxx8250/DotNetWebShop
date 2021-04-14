﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASPDotNetShoppingCart.Models
{
    public class Product
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(32)]
        public string ProductName { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [MaxLength(128)]
        public string Description { get; set; }
        [Required]
        [MaxLength(64)]
        public string ImagePath { get; set; }
        [Required]
        [MaxLength(128)]
        public string DownloadLink { get; set; }


    }
}
