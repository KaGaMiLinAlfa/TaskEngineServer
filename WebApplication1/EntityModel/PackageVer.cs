using System.ComponentModel.DataAnnotations;

namespace Worker2.EntityModel
{
    public class PackageVer
    {
        [Key]
        public int Id { get; set; }

        public string PackageName { get; set; }
        public string PackageUrl { get; set; }

        public string Remark { get; set; }
    }
}
