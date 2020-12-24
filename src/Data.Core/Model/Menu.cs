namespace Data.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Menus")]
    public class Menu
    {
        [Column("MenuId"), Key, Required]
        public Guid MenuId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        public List<MenuItem> Items { get; init; }
        
        [ForeignKey("RestaurantId"), Required]
        public Guid RestaurantId { get; init; }
        public Restaurant Restaurant { get; init; }
        
        [Column("IsActive"), Required]
        public bool IsActive { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}