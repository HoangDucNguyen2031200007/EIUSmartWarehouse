using System.ComponentModel.DataAnnotations;

namespace EIUSmartWarehouse.Models
{
    public class Warehouse
    {
        [Key]
        [StringLength(4)]
        public string RFID { get; set; }
        [Required]
        [StringLength(10)]
        public string Location_status { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }
}
