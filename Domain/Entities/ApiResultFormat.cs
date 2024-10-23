using System.Security;

namespace Domain.Entities
{
    public class ApiResultFormat<T>
    {
        public T data { get; set; }

        public Permissions permissions { get; set; }
        public int totalData { get; set; }
    }

    public class Permissions
    {
        public bool create { get; set; }
        public bool read { get; set; }
        public bool write { get; set; }
        public bool delete { get; set; }
    }
}
