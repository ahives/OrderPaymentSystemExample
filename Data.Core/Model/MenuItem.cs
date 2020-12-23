namespace DatabaseDeploy.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MenuItems")]
    public class MenuItem
    {
        [Column("MenuItemId"), Key, Required]
        public Guid MenuItemId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [Column("IsValid"), Required]
        public bool IsValid { get; init; }
        
        [ForeignKey("MenuId"), Required]
        public Guid MenuId { get; init; }
        public Menu Menu { get; init; }
        
        [ForeignKey("StorageTemperatureId"), Required]
        public long StorageTemperatureId { get; init; }
        public StorageTemperature StorageTemperature { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}