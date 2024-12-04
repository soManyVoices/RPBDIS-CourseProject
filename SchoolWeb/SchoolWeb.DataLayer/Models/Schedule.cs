using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWeb.DataLayer.Models;

public partial class Schedule
{
    [Key]
    [Display(Name = "Идентификатор расписания")]
    public int ScheduleId { get; set; }

    [Required]
    [Display(Name = "Дата")]
    public DateOnly Date { get; set; }

    [Required]
    [Display(Name = "День недели")]
    public string DayOfWeek { get; set; } = null!;

    [Required]
    [Display(Name = "Идентификатор класса")]
    public int ClassId { get; set; }

    [Required]
    [Display(Name = "Идентификатор предмета")]
    public int SubjectId { get; set; }

    [Required]
    [Display(Name = "Время начала")]
    public TimeOnly StartTime { get; set; }

    [Required]
    [Display(Name = "Время окончания")]
    public TimeOnly EndTime { get; set; }

    [ForeignKey("ClassId")]
    public virtual Class Class { get; set; } = null!;

    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;
}
