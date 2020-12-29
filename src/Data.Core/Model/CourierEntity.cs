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
        
        [ForeignKey("AddressId"), Required]
        public long AddressId { get; init; }
        public AddressEntity Address { get; init; }
        
        [Column("IsAvailable"), Required]
        public bool IsAvailable { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; set; }
    }
}