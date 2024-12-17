using System.ComponentModel.DataAnnotations;

namespace EIUSmartWarehouse.Models
{
    public class Staff
    {
        [Key]
        [StringLength(10)]
        public string StaffID { get; set; }
        [Required]
        [StringLength(50)]
        public string StaffName { get; set; }
        [Required]
        [StringLength(10)]
        public string StaffContractStart { get; set; }
        [Required]
        [StringLength(10)]
        public string StaffContractEnd { get; set; }
        [Required]
        [StringLength(50)]
        public string StaffRole { get; set; }
        [Required]
        [StringLength(20)]
        public string StaffStatus { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }
}
