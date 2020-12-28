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
        
        [Column("IsValid"), Required]
        public bool IsValid { get; init; }
        
        [ForeignKey("MenuId"), Required]
        public Guid MenuId { get; init; }
        public MenuEntity Menu { get; init; }
        
        [Column("ShelfLife")]
        public decimal ShelfLife { get; set; }
        
        [ForeignKey("StorageTemperatureId"), Required]
        public int StorageTemperatureId { get; init; }
        public StorageTemperatureEntity StorageTemperature { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}