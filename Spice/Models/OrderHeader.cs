using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderHeader
    {
            //Primary Key
            [Key]
            public int Id { get; set; }

            //Required
            public string UserId { get; set; }

            // Note this require different DataAnnotationn than the reqular one, make sure to write it correctly.
            [ForeignKey("UserId")]
            public virtual ApplicationUser applicationUser { get; set; }
 
            public decimal OrignalTotal { get; set; }

            [DisplayFormat(DataFormatString ="{0:C}")]
            [Display(Name ="Order Total")]
            [Required]
            public decimal OrderTotal { get; set; }

            [Required]
            [Display(Name ="PickUp Time")]
            public DateTime PickUpTime { get; set; }

            // The Following property is not mapped because  we will use the date from here and use it in Pickup Time
            [Required]
            [Display(Name ="Pickup Date")]
            [NotMapped]
            public DateTime PickUpDate { get; set; }

        //Checking/ Using Coupon, Not using the Coupon Object  
        //for Security reason,User Will enter the coupon details.

        public string CouponCode { get; set; }
        public string CouponCodeDiscount { get; set; }
        public string CoupnStatus { get; set; }

        public string PaymentStatus { get; set; }
        public string Comments { get; set; }

        [Display(Name ="Pick Up by :")]
        public string PickupName { get; set; }

        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }
        public string TransactionId { get; set; }
    }
}
