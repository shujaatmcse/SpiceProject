using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ShoppingCart
    {
        //We want the count of an item to be shown as 1 by default 
        public ShoppingCart()
        {
            Count = 1;
        }
        public int Id { get; set; }

        public string AppliUserId { get; set; }
        //We do not need to map, but yes we want to store the user Id , 
        //map means likes refrencing the Forign key and linking it.
        [NotMapped]
        [ForeignKey("AppliUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int MenuItemId { get; set; }
        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual MenuItem menuItem { get; set; }

        [Range(1,int.MaxValue, ErrorMessage ="Please enter value greater than or equal to 1")]
        public int Count { get; set; }
    }
}
