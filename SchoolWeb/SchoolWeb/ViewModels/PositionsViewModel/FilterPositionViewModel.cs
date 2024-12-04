using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.PositionsViewModel
{
    public class FilterPositionViewModel
    {
        [Required]
        [Display(Name = "Название должности")]
        public string PositionName { get; set; } = null!;

        [Display(Name = "Описание должности")]
        public string? Description { get; set; }

        [Display(Name = "Зарплата")]
        public decimal? Salary { get; set; }
    }
}
