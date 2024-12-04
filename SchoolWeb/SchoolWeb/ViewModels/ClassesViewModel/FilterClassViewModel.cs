using SchoolWeb.DataLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.ClassesViewModel
{
    public class FilterClassViewModel
    {
        [Required]
        [Display(Name = "Название класса")]
        public string ClassName { get; set; } = null!;

        [Required]
        [Display(Name = "Учитель класса")]
        public string ClassTeacher { get; set; } = null!;

        [Required]
        [Display(Name = "Название типа класса")]
        public string ClassTypeName { get; set; }

        [Display(Name = "Количество студентов")]
        public int? StudentCount { get; set; }

        [Required]
        [Display(Name = "Год создания")]
        public int? YearCreated { get; set; }
    }
}
