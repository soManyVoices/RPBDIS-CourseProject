using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.ClassesViewModel;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.ViewModels.ClassTypesViewModel
{
    public class ClassTypeViewModel
    {
        public IEnumerable<ClassType> ClassTypes { get; set; }

        public FilterClassTypeViewModel FilterClassTypeViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public ClassTypeSortViewModel SortViewModel { get; set; }

        public SelectList SelectedClassTypeList { get; set; } // Список типов классов
    }
}
