namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Orders")]
    public class OrderEntity
    {
        [Column("OrderId"), Key, Required]
        public Guid OrderId { get; init; }
        
        [ForeignKey("RestaurantId"), Required]
        public Guid RestaurantId { get; init; }
        public RestaurantEntity Restaurant { get; init; }
        
        [Column("Status"), Required]
        public int Status { get; set; }
        
        [Column("StatusTimestamp"), Required]
        public DateTime StatusTimestamp { get; set; }
        
        [ForeignKey("CustomerId"), Required]
        public Guid CustomerId { get; init; }
        public CustomerEntity Customer { get; init; }
        
        [ForeignKey("CourierId")]
        public Guid? CourierId { get; set; }
        public CourierEntity Courier { get; set; }
        
        [ForeignKey("AddressId"), Required]
        public Guid AddressId { get; init; }
        public AddressEntity Address { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}