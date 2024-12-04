using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWeb.DataLayer.Models;

public partial class Subject
{
    [Key]
    [Display(Name = "Идентификатор предмета")]
    public int SubjectId { get; set; }

    [Required]
    [Display(Name = "Название предмета")]
    public string Name { get; set; } = null!;

    [Display(Name = "Описание предмета")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Имя сотрудника")]
    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [Display(Name = "Сотрудник")]
    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
