namespace LibraryApi.Domain.Entities
{
    public class Address
    {
        public long Id { get; set; }
        public long UserId { get; set; } // Foreign Key ไปที่ User
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
