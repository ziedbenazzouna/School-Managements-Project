using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthECAPI.Models
{
    public class StudentDetail
    {
        [Key]
        public int StudentDetailId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string StudentFullName { get; set; } = "";

        [Column(TypeName = "nvarchar(16)")]
        public string InscriptionCardNumber { get; set; } = "";

        //mm/yy
        [Column(TypeName = "nvarchar(5)")]
        public string ExpirationCardDate { get; set; } = "";

        [Column(TypeName = "nvarchar(2)")]
        public string Age { get; set; } = "";
    }
}
