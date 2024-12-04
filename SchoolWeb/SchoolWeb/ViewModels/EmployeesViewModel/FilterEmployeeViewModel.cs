using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.EmployeesViewModel
{
    public class FilterEmployeeViewModel
    {
        [Required]
        [Display(Name = "Имя сотрудника")]
        public string EmployeeFirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Фамилия")]
        public string EmployeeLastName { get; set; } = null!;

        [Display(Name = "Отчество")]
        public string? EmployeeMiddleName { get; set; }

        [Display(Name = "Должность")]
        public string PositionName { get; set; }
    }
}
