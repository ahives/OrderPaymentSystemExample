namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Menus")]
    public class MenuEntity
    {
        [Column("MenuId"), Key, Required]
        public Guid MenuId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [ForeignKey("RestaurantId"), Required]
        public Guid RestaurantId { get; init; }
        public RestaurantEntity Restaurant { get; init; }
        
        [Column("IsActive"), Required]
        public bool IsActive { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}