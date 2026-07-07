using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLes1Nike.Data.Entities
{
    [Table("tblCategories")]
    public class CategoryEntitiy
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string? Image { get; set; } = string.Empty;
        [StringLength(250)]
        public string Slug { get; set; } = null!;
        public bool isDeleted { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
