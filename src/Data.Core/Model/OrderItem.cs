namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderItems")]
    public class OrderItem
    {
        [Column("OrderItemId"), Key, Required]
        public Guid OrderItemId { get; init; }
        
        [ForeignKey("OrderId"), Required]
        public Guid OrderId { get; init; }
        public Order Order { get; init; }
        
        [ForeignKey("MenuItemId"), Required]
        public Guid MenuItemId { get; init; }
        public MenuItem MenuItem { get; init; }
        
        [Column("IsExpired"), Required]
        public bool IsExpired { get; set; }
        
        [ForeignKey("ShelfId"), Required]
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }
        
        [Column("Status"), Required]
        public int Status { get; set; }
        
        [Column("StatusTimestamp"), Required]
        public DateTime StatusTimestamp { get; set; }
        
        [Column("TimePrepared"), Required]
        public DateTime TimePrepared { get; set; }
        
        [Column("SpecialInstructions")]
        public string SpecialInstructions { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}