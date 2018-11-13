using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NorthWindWeek5.Models
{
    public class CustomerSignIn
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
    }
}