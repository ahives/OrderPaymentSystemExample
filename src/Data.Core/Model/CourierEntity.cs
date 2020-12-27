namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Couriers")]
    public class CourierEntity
    {
        [Column("CourierId"), Key, Required]
        public Guid CourierId { get; set; }
        
        [Column("FirstName"), Required]
        public string FirstName { get; set; }
        
        [Column("LastName"), Required]
        public string LastName { get; set; }
        
        [Column("Street"), Required]
        public string Street { get; init; }
        
        [Column("City"), Required]
        public string City { get; init; }
        
        [ForeignKey("RegionId"), Required]
        public int RegionId { get; init; }
        public RegionEntity Region { get; init; }
        
        [Column("ZipCode"), Required]
        public string ZipCode { get; init; }
        
        [Column("IsAvailable"), Required]
        public bool IsAvailable { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; set; }
    }
}