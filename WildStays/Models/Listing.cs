using System;
using System.ComponentModel.DataAnnotations;

namespace WildStays.Models
{
    public class Listing
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        //Checks that string is smaller that 100 characters.
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Place is required.")]
        public string Place { get; set; }

        public string Description { get; set; }

        public string? Type { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        //Cheks that the price is between 1 and the max int value.
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Guests is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Guests must be greater than 0.")]
        public int Guests { get; set; }

        [Required(ErrorMessage = "Bedrooms is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Bedrooms must be greater than 0.")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Bathrooms is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Bathrooms must be greater than 0.")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Please enter a valid image URL.")]
        [Url(ErrorMessage = "Please enter a valid image URL.")]
        public string Image { get; set; }

        public string UserId { get; set; } = string.Empty;

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

        public Listing()
        {

        }
    }
}
