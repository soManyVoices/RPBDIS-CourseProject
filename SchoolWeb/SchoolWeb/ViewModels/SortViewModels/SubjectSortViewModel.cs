using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class SubjectSortViewModel
    {
        public SubjectSortState CurrentState { get; set; }
        public SubjectSortState SubjectNameSort { get; set; }
        public SubjectSortState EmployeeNameSort { get; set; }

        public SubjectSortViewModel(SubjectSortState sortOrder)
        {
            CurrentState = sortOrder;

            SubjectNameSort = sortOrder == SubjectSortState.SubjectNameAsc
            ? SubjectSortState.SubjectNameDesc
            : SubjectSortState.SubjectNameAsc;

            EmployeeNameSort = sortOrder == SubjectSortState.EmployeeNameAsc
          ? SubjectSortState.EmployeeNameDesc
          : SubjectSortState.EmployeeNameAsc;
        }
    }
}
