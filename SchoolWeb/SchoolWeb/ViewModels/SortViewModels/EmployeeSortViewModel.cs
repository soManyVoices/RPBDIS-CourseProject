using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class EmployeeSortViewModel
    {
        public EmployeeSortState CurrentState { get; set; }
        public EmployeeSortState PositionNameSort { get; set; }
        public EmployeeSortState FirstNameSort { get; set; }
        public EmployeeSortState LastNameSort { get; set; }
        public EmployeeSortState MiddleNameSort { get; set; }

        public EmployeeSortViewModel(EmployeeSortState sortOrder)
        {
            CurrentState = sortOrder;

            // Сортировка по должности
            PositionNameSort = sortOrder == EmployeeSortState.PositionNameAsc
                ? EmployeeSortState.PositionNameDesc
                : EmployeeSortState.PositionNameAsc;

            // Сортировка по имени
            FirstNameSort = sortOrder == EmployeeSortState.EmployeeFirstNameAsc
                ? EmployeeSortState.EmployeeFirstNameDesc
                : EmployeeSortState.EmployeeFirstNameAsc;

            // Сортировка по фамилии
            LastNameSort = sortOrder == EmployeeSortState.EmployeeLastNameAsc
                ? EmployeeSortState.EmployeeLastNameDesc
                : EmployeeSortState.EmployeeLastNameAsc;

            // Сортировка по отчеству
            MiddleNameSort = sortOrder == EmployeeSortState.EmployeeMiddleNameAsc
                ? EmployeeSortState.EmployeeMiddleNameDesc
                : EmployeeSortState.EmployeeMiddleNameAsc;
        }
    }
}
