﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthWindWeek5.Models
{
    public class CartDTO
    {
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        public int Quantity { get; set; }
    }
}