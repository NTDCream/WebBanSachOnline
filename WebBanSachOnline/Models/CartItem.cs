namespace WebBanSachOnline.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CartItem")]
    public partial class CartItem
    {
        public int id { get; set; }

        public int userId { get; set; }

        public int bookId { get; set; }

        public int quantity { get; set; }

        public virtual Book Book { get; set; }

        public virtual User User { get; set; }
    }
}
