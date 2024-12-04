using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.ClassesViewModel;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.ViewModels.SchedulesViewModel
{
    public class ScheduleViewModel
    {
        public IEnumerable<Schedule> Schedules { get; set; }

        public FilterScheduleViewModel FilterScheduleViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public ScheduleSortViewModel SortViewModel { get; set; }

        public SelectList SelectedClassNameList { get; set; }
        public SelectList SelectedSubjectNameList { get; set; } 
    }
}
