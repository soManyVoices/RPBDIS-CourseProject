using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.SubjectsViewModel
{
    public class FilterSubjectViewModel
    {
        [Key]
        [Display(Name = "Идентификатор предмета")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Название предмета")]
        public string SubjectName { get; set; } = null!;

        [Display(Name = "Описание предмета")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Имя преподавателя")]
        public string? EmployeeName { get; set; }
    }
}
