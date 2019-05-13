using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.ViewModel
{
    public class LandingPageViewModel
    {
        public IEnumerable<MenuItem> MenuItem { get; set; }
        public IEnumerable<Coupon> Coupon  { get; set; }

        public IEnumerable<Category> Category { get; set; }
    }
}
