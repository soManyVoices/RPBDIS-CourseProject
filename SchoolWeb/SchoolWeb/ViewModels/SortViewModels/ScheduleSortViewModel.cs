using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class ScheduleSortViewModel
    {
        public ScheduleSortState CurrentState { get; set; }
        public ScheduleSortState ClassNameSort { get; set; }
        public ScheduleSortState SubjectNameSort { get; set; }

        public ScheduleSortViewModel(ScheduleSortState sortOrder)
        {
            CurrentState = sortOrder;

            ClassNameSort = sortOrder == ScheduleSortState.ClassNameAsc
                ? ScheduleSortState.ClassNameDesc
                : ScheduleSortState.ClassNameAsc;

            SubjectNameSort = sortOrder == ScheduleSortState.SubjectNameAsc
            ? ScheduleSortState.SubjectNameDesc
            : ScheduleSortState.SubjectNameAsc;
        }
    }
}
