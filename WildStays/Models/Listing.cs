﻿using System.ComponentModel.DataAnnotations;

namespace WildStays.Models
{
    public class Listing
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public int Guests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        [Required]
        [Url(ErrorMessage = "Please enter a valid image URL.")]
        public string Image { get; set; }
        public string UserId { get; set; } = string.Empty;



        public Listing()
        {

        }

    }
}

