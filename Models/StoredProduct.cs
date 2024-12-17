using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIUSmartWarehouse.Models
{
    public class StoredProduct
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(10)]
        public string ProductUnit { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [StringLength(4)]
        public string RFID { get; set; }
        [ForeignKey("RFID")]
        public virtual Warehouse Warehouse { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        [Required]
        [StringLength(20)]
        public string InTime { get; set; }

        [StringLength(20)]
        public string OutTime { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string StaffID { get; set; }
        [ForeignKey("StaffID")]
        public virtual Staff Staff { get; set; }
    }

}
