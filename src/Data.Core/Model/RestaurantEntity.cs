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
        
        [Column("Street"), Required]
        public string Street { get; init; }
        
        [Column("City"), Required]
        public string City { get; init; }
        
        [ForeignKey("RegionId"), Required]
        public int RegionId { get; init; }
        public RegionEntity Region { get; init; }
        
        [Column("ZipCode"), Required]
        public string ZipCode { get; init; }
        
        [Column("CreationTimestamp")]
        public DateTime CreationTimestamp { get; init; }
    }
}