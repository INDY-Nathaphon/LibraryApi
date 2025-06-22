namespace LibraryApi.Common.Infos.Base
{
    public class BaseInfo
    {
        public long Id { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
