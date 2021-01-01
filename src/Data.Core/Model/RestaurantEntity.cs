namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Restaurants")]
    public class RestaurantEntity
    {
        [Column("RestaurantId"), Key, Required]
        public Guid RestaurantId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; set; }
        
        [Column("IsOpen"), Required]
        public bool IsOpen { get; set; }
        
        [Column("IsActive"), Required]
        public bool IsActive { get; set; }
        
        [ForeignKey("AddressId"), Required]
        public long AddressId { get; set; }
        public AddressEntity Address { get; set; }
        
        [Column("CreationTimestamp")]
        public DateTime CreationTimestamp { get; init; }
    }
}