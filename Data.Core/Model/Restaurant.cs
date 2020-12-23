namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Restaurants")]
    public class Restaurant
    {
        [Column("RestaurantId"), Key, Required]
        public Guid RestaurantId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [Column("Street")]
        public string Street { get; init; }
        
        [Column("City")]
        public string City { get; init; }
        
        [Column("RegionId")]
        public long RegionId { get; init; }
        public Region Region { get; init; }
        
        [Column("ZipCode")]
        public string ZipCode { get; init; }
        
        [Column("CreationTimestamp")]
        public DateTime CreationTimestamp { get; init; }
    }
}