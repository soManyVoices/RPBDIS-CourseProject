using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWeb.DataLayer.Models;

public partial class Position
{
    [Key]
    [Display(Name = "Идентификатор должности")]
    public int PositionId { get; set; }

    [Required(ErrorMessage = "Название должности обязательно для заполнения.")]
    [Display(Name = "Название должности")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Описание должности обязательно для заполнения.")]
    [Display(Name = "Описание должности")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Зарплата обязательна для заполнения.")]
    [Range(0, double.MaxValue, ErrorMessage = "Зарплата не может быть отрицательной.")]
    [Display(Name = "Зарплата")]
    public decimal? Salary { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}