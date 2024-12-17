using System.ComponentModel.DataAnnotations;

namespace EIUSmartWarehouse.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        [StringLength(10)]
        public string CustomerCode { get; set; }
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }
}
