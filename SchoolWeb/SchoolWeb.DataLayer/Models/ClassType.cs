using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWeb.DataLayer.Models
{
    public partial class ClassType
    {
        [Key]
        [Display(Name = "Тип класса")]
        public int ClassTypeId { get; set; }

        [Required]
        [Display(Name = "Название типа класса")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Описание типа класса")]
        public string? Description { get; set; }

        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
