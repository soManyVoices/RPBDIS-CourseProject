using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWeb.DataLayer.Models
{
    public partial class Class
    {
        [Key]
        [Display(Name = "Идентификатор класса")]
        public int ClassId { get; set; }

        [Required]
        [Display(Name = "Название класса")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Учитель класса")]
        public string ClassTeacher { get; set; } = null!;

        [Required]
        [Display(Name = "Тип класса (ID)")]
        public int ClassTypeId { get; set; }

        [Display(Name = "Количество учеников")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Количество учеников не может быть отрицательным.")]
        public int? StudentCount { get; set; }

        [Required]
        [Display(Name = "Год создания")]
        [Range(1990, int.MaxValue, ErrorMessage = "Год создания должен быть больше или равен 1990.")]
        public int? YearCreated { get; set; }

        [ForeignKey("ClassTypeId")]
        public virtual ClassType ClassType { get; set; } = null!;

        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
