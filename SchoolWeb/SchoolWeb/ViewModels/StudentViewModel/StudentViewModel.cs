using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.ViewModels.StudentViewModel
{
    public class StudentViewModel
    {
        public IEnumerable<Student> Students { get; set; }

        public FilterStudentViewModel FilterStudentViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public StudentSortViewModel SortViewModel { get; set; }

        public SelectList SelectedClassNameList { get; set; }
        public SelectList SelectedSubjectNameList { get; set; }
    }
}
