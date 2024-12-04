using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWeb.DataLayer.Models
{
    public partial class Employee
    {
        [Key]
        [Display(Name = "Код сотрудника")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Имя сотрудника")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Имя сотрудника должно содержать только буквы.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Фамилия")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Фамилия должна содержать только буквы.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Отчество")]
        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Отчество должно содержать только буквы.")]
        public string? MiddleName { get; set; }

        [ForeignKey("PositionId")]
        [Display(Name = "Должность")]
        public int PositionId { get; set; }

        public virtual Position Position { get; set; } = null!;

        public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
