using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class ClassTypeSortViewModel
    {
        public ClassTypeSortState CurrentState { get; set; }
        public ClassTypeSortState ClassTypeNameSort { get; set; }

        public ClassTypeSortViewModel(ClassTypeSortState sortOrder)
        {
            CurrentState = sortOrder;

            ClassTypeNameSort = sortOrder == ClassTypeSortState.ClassTypeNameAsc
                ? ClassTypeSortState.ClassTypeNameDesc
                : ClassTypeSortState.ClassTypeNameAsc;
        }
    }
}