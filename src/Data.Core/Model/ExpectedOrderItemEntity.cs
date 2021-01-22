namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExpectedOrderItems")]
    public class ExpectedOrderItemEntity
    {
        [Column("OrderItemId"), Key, Required]
        public Guid OrderItemId { get; set; }

        [Column("OrderId"), Required]
        public Guid OrderId { get; set; }

        [Column("Status"), Required]
        public int Status { get; set; }
    }
}