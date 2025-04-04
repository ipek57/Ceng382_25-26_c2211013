using System.ComponentModel.DataAnnotations;

namespace Week5Project.Models
{
    public class ClassModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required.")]
        public string ClassName { get; set; }

        [Range(1, 1000, ErrorMessage = "Student count must be between 1 and 1000.")]
        public int StudentCount { get; set; }

        public string? Description { get; set; }
    }
}
