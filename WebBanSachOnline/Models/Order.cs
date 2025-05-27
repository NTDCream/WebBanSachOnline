namespace WebBanSachOnline.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int id { get; set; }

        public int userId { get; set; }

        [Required]
        [StringLength(50)]
        public string slug { get; set; }

        [Required]
        [StringLength(20)]
        public string status { get; set; }

        public decimal totalAmount { get; set; }

        [Required]
        [StringLength(255)]
        public string customerName { get; set; }

        [Required]
        [StringLength(20)]
        public string phone { get; set; }

        [Required]
        [StringLength(512)]
        public string address { get; set; }

        public int? price { get; set; }

        [StringLength(100)]
        public string paymentMethod { get; set; }

        public DateTime createdDate { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
