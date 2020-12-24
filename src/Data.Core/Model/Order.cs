namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Orders")]
    public class Order
    {
        [Column("OrderId"), Key, Required]
        public Guid OrderId { get; init; }
        
        [Column("Status"), Required]
        public int Status { get; set; }
        
        [Column("StatusTimestamp"), Required]
        public DateTime StatusTimestamp { get; set; }
        
        [ForeignKey("CustomerId"), Required]
        public Guid CustomerId { get; init; }
        public Customer Customer { get; init; }
        
        [ForeignKey("CourierId")]
        public Guid? CourierId { get; set; }
        public Courier Courier { get; set; }
        
        [Column("Street"), Required]
        public string Street { get; init; }
        
        [Column("City"), Required]
        public string City { get; init; }
        
        [ForeignKey("RegionId"), Required]
        public long RegionId { get; init; }
        public Region Region { get; init; }
        
        [Column("ZipCode"), Required]
        public string ZipCode { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}