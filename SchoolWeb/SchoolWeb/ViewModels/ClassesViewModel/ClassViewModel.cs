using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.ViewModels.ClassesViewModel
{
    public class ClassViewModel
    {
        public IEnumerable<Class> Classes { get; set; }

        public FilterClassViewModel FilterClassViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public ClassSortViewModel SortViewModel { get; set; }

        public SelectList SelectedClassTypeList { get; set; } // Список типов классов
    }
}
