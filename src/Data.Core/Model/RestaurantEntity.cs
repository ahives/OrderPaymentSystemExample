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
        public string Name { get; init; }
        
        [ForeignKey("AddressId"), Required]
        public long AddressId { get; init; }
        public AddressEntity Address { get; init; }
        
        [Column("CreationTimestamp")]
        public DateTime CreationTimestamp { get; init; }
    }
}