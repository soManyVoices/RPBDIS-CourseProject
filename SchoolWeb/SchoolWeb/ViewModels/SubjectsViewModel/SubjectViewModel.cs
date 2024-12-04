using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.SchedulesViewModel;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.ViewModels.SubjectsViewModel
{
    public class SubjectViewModel
    {
        public IEnumerable<Subject> Subjects { get; set; }

        public FilterSubjectViewModel FilterSubjectViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public SubjectSortViewModel SortViewModel { get; set; }
        public SelectList SelectSubjectList { get; set; }
    }
}
