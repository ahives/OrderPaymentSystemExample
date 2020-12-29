namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Addresses")]
    public class AddressEntity
    {
        [Column("AddressId"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public long AddressId { get; init; }
        
        [Column("Street"), Required]
        public string Street { get; init; }
        
        [Column("City"), Required]
        public string City { get; init; }
        
        [ForeignKey("RegionId"), Required]
        public int RegionId { get; init; }
        public RegionEntity Region { get; init; }
        
        [Column("ZipCode"), Required]
        public string ZipCode { get; init; }
        
        [Column("CreationTimestamp")]
        public DateTime CreationTimestamp { get; init; }
    }
}