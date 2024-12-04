using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class StudentSortViewModel
    {
        public StudentSortState CurrentState { get; set; }
        public StudentSortState ClassNameSort { get; set; }
        public StudentSortState DateOfBirthSort { get; set; }
        public StudentSortState SubjectNameSort { get; set; }

        public StudentSortViewModel(StudentSortState sortOrder)
        {
            CurrentState = sortOrder;

            ClassNameSort = sortOrder == StudentSortState.ClassNameAsc
            ? StudentSortState.ClassNameDesc
            : StudentSortState.ClassNameAsc;

            DateOfBirthSort = sortOrder == StudentSortState.DateOfBirthAsc
          ? StudentSortState.DateOfBirthDesc
          : StudentSortState.DateOfBirthAsc;

            SubjectNameSort = sortOrder == StudentSortState.SubjectNameAsc
            ? StudentSortState.SubjectNameDesc
            : StudentSortState.SubjectNameAsc;
        }
    }
}