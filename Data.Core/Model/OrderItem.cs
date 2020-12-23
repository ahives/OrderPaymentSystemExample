namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
        
        [Column("SpecialInstructions")]
        public string SpecialInstructions { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}