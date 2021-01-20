namespace OrderProcessingService.Core.StateMachines.Sagas
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExpectedOrderItem")]
    public class ExpectedOrderItem
    {
        [Column("OrderItemId"), Key, Required]
        public Guid OrderItemId { get; set; }

        [ForeignKey("OrderId"), Required]
        public Guid OrderId { get; set; }
        public OrderState Order { get; set; }

        [Column("Status"), Required]
        public int Status { get; set; }
    }
}