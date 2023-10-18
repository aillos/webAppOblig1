using System;
using System.ComponentModel.DataAnnotations;

namespace WildStays.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public int ListingId { get; set; }
        //Incoreperates the listings view so that they can work together.
        public Listing Listing { get; set; }

        public string UserId { get; set; }
        
        public string Place { get; set; }
    }
}
