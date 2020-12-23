namespace DatabaseDeploy.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Orders")]
    public class Order
    {
        [Column("OrderId"), Key, Required]
        public Guid OrderId { get; set; }
        
        [Column("Status"), Required]
        public string Status { get; init; }
        
        [Column("StatusTimestamp"), Required]
        public DateTime StatusTimestamp { get; init; }
        
        [ForeignKey("CustomerId"), Required]
        public Guid CustomerId { get; init; }
        public Customer Customer { get; init; }
        
        [ForeignKey("MenuItemId"), Required]
        public long MenuItemId { get; init; }
        public MenuItem MenuItem { get; init; }
        
        [ForeignKey("CourierId")]
        public Guid CourierId { get; init; }
        public Courier Courier { get; init; }
        
        [Column("SpecialInstructions"), Required]
        public string SpecialInstructions { get; init; }
        
        [ForeignKey("RegionId"), Required]
        public long RegionId { get; set; }
        public Region Region { get; set; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; set; }
    }
}