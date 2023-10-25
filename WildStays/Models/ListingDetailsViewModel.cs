namespace WildStays.Models;

public class ListingDetailsViewModel
{
    public Listing Listing { get; set; }
    public IEnumerable<Reservation> Reservations { get; set; }
}
