using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WildStays.Models;

namespace WildStays.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        // You can add additional properties here as needed
        // For example, a reference to the user who made the reservation:
        public string UserId { get; set; }
    }

}
