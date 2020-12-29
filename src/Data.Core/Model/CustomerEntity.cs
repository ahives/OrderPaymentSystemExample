namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customers")]
    public class CustomerEntity
    {
        [Column("CustomerId"), Key, Required]
        public Guid CustomerId { get; set; }
        
        [Column("FirstName"), Required]
        public string FirstName { get; set; }
        
        [Column("LastName"), Required]
        public string LastName { get; set; }
        
        [Column("Street"), Required]
        public string Street { get; set; }
        
        [Column("City"), Required]
        public string City { get; set; }
        
        [ForeignKey("RegionId"), Required]
        public int RegionId { get; init; }
        public RegionEntity Region { get; init; }
        
        [Column("ZipCode"), Required]
        public string ZipCode { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; set; }
    }
}