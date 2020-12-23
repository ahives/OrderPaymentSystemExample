namespace DatabaseDeploy.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Couriers")]
    public class Courier
    {
        [Column("CourierId"), Key, Required]
        public Guid CourierId { get; set; }
        
        [Column("FirstName"), Required]
        public string FirstName { get; set; }
        
        [Column("LastName"), Required]
        public string LastName { get; set; }
        
        [Column("Street")]
        public string Street { get; init; }
        
        [Column("City")]
        public string City { get; init; }
        
        [Column("RegionId")]
        public long RegionId { get; init; }
        public Region Region { get; init; }
        
        [Column("ZipCode")]
        public string ZipCode { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; set; }
    }
}