using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.SchedulesViewModel
{
    public class FilterScheduleViewModel
    {
        [Required]
        [Display(Name = "Дата")]
        public DateOnly Date { get; set; }

        [Required]
        [Display(Name = "День недели")]
        public string DayOfWeek { get; set; } = null!;

        [Required]
        [Display(Name = "Название класса")]
        public string ClassName { get; set; }

        [Required]
        [Display(Name = "Название предмета")]
        public string SubjectName { get; set; }

        [Required]
        [Display(Name = "Время начала")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Display(Name = "Время окончания")]
        public TimeOnly EndTime { get; set; }
    }
}
