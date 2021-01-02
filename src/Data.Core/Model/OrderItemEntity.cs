namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderItems")]
    public class OrderItemEntity
    {
        [Column("OrderItemId"), Key, Required]
        public Guid OrderItemId { get; init; }
        
        [ForeignKey("OrderId"), Required]
        public Guid OrderId { get; init; }
        public OrderEntity Order { get; init; }
        
        [ForeignKey("MenuItemId"), Required]
        public Guid MenuItemId { get; init; }
        public MenuItemEntity MenuItem { get; init; }
        
        [ForeignKey("ShelfId")]
        public Guid? ShelfId { get; set; }
        public ShelfEntity Shelf { get; set; }
        
        [Column("Status"), Required]
        public int Status { get; set; }
        
        [Column("StatusTimestamp"), Required]
        public DateTime StatusTimestamp { get; set; }
        
        [Column("TimePrepared")]
        public DateTime? TimePrepared { get; set; }
        
        [Column("ExpiryTimestamp")]
        public DateTime? ExpiryTimestamp { get; set; }
        
        [Column("ShelfLife")]
        public decimal ShelfLife { get; set; }
        
        [Column("SpecialInstructions")]
        public string SpecialInstructions { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}