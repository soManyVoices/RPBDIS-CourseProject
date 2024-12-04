using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class ClassSortViewModel
    {
        public ClassSortState CurrentState { get; set; }
        public ClassSortState ClassNameSort { get; set; }
        public ClassSortState YearCreatedSort { get; set; }
        public ClassSortState StudentCountSort { get; set; }
        public ClassSortState ClassTypeSort { get; set; } 

        public ClassSortViewModel(ClassSortState sortOrder)
        {
            CurrentState = sortOrder;

            ClassNameSort = sortOrder == ClassSortState.ClassNameAsc
                ? ClassSortState.ClassNameDesc
                : ClassSortState.ClassNameAsc;

            YearCreatedSort = sortOrder == ClassSortState.YearCreatedAsc
                ? ClassSortState.YearCreatedDesc
                : ClassSortState.YearCreatedAsc;

            StudentCountSort = sortOrder == ClassSortState.StudentCountAsc
                ? ClassSortState.StudentCountDesc
                : ClassSortState.StudentCountAsc;

            // Добавляем сортировку по типу класса
            ClassTypeSort = sortOrder == ClassSortState.ClassTypeAsc
                ? ClassSortState.ClassTypeDesc
                : ClassSortState.ClassTypeAsc;
        }
    }
}
