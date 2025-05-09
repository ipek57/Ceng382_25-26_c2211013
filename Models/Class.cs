using System.ComponentModel.DataAnnotations;

namespace Week5Project.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int PersonCount { get; set; }

        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
