using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.ClassesViewModel;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.ViewModels.EmployeesViewModel
{
    public class EmployeeViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }

        public FilterEmployeeViewModel FilterEmployeeViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public EmployeeSortViewModel SortViewModel { get; set; }

        public SelectList SelectedPositionTypeList { get; set; }
    }
}
