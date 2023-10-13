namespace WildStays.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int ListingId { get; set; }
        public Listing Listing { get; set; }
    }

}
