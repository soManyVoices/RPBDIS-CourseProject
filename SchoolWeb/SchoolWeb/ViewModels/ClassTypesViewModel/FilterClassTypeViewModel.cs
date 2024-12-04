using System.ComponentModel.DataAnnotations;

namespace SchoolWeb.ViewModels.ClassTypesViewModel
{
    public class FilterClassTypeViewModel
    {
        [Required]
        [Display(Name = "Название типа класса")]
        public string ClassTypeName { get; set; } = null!;

        [Display(Name = "Описание типа класса")]
        public string? Description { get; set; }
    }
}
