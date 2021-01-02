namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MenuItems")]
    public class MenuItemEntity
    {
        [Column("MenuItemId"), Key, Required]
        public Guid MenuItemId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [Column("Price"), Required]
        public decimal Price { get; init; }
        
        [Column("IsActive"), Required]
        public bool IsActive { get; init; }
        
        [ForeignKey("MenuId"), Required]
        public Guid MenuId { get; init; }
        public MenuEntity Menu { get; init; }
        
        [Column("ShelfLife")]
        public decimal ShelfLife { get; set; }
        
        [ForeignKey("TemperatureId"), Required]
        public Guid TemperatureId { get; init; }
        public TemperatureEntity Temperature { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}