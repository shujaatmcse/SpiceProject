using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.ViewModel
{

    //We are creating this view model , so to get the data for the Index page of Cart
    // The Cart Index page has a list of menuitem and its quanity+ Order Total and Order coupon
    // In shopping Cart the the menuitem is not mapped hence we cannot not navigate through it.
    // so we are using list of shopping cart instead of just using .inculde in the controller to get the cart data
    public class OrderHeaderShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCart { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
