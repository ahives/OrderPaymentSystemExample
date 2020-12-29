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
        
        [ForeignKey("AddressId"), Required]
        public long AddressId { get; init; }
        public AddressEntity Address { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; set; }
    }
}