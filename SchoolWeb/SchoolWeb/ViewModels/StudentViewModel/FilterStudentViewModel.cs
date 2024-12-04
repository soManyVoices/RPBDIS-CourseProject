using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.StudentViewModel
{
    public class FilterStudentViewModel
    {
        [Required(ErrorMessage = "Имя ученика обязательно для заполнения.")]
        [Display(Name = "Имя ученика")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Фамилия ученика обязательно для заполнения.")]
        [Display(Name = "Фамилия ученика")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Отчество ученика")]
        [Required]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательно для заполнения.")]
        [Display(Name = "Дата рождения")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Пол обязателен для выбора.")]
        [RegularExpression("^(Мужской|Женский)$", ErrorMessage = "Пол должен быть 'Мужской' или 'Женский'.")]
        [Display(Name = "Пол")]
        public string Gender { get; set; } = null!;

        [Display(Name = "Адрес")]
        [Required]
        public string? Address { get; set; }

        [Display(Name = "Имя отца")]
        [Required]
        public string? FatherFirstName { get; set; }

        [Display(Name = "Фамилия отца")]
        [Required]
        public string? FatherLastName { get; set; }

        [Display(Name = "Отчество отца")]
        [Required]
        public string? FatherMiddleName { get; set; }

        [Display(Name = "Имя матери")]
        [Required]
        public string? MotherFirstName { get; set; }

        [Display(Name = "Фамилия матери")]
        [Required]
        public string? MotherLastName { get; set; }

        [Display(Name = "Отчество матери")]
        [Required]
        public string? MotherMiddleName { get; set; }

        [Display(Name = "Наименование класса")]
        public string? ClassName { get; set; }

        [Display(Name = "Дополнительная информация")]
        public string? AdditionalInfo { get; set; }
        [Display(Name = "Название предмета")]
        public string? SubjectName { get; set; }
    }
}
