using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.ViewModels.SortViewModels
{
    public class PositionSortViewModel
    {
        public PositionSortState CurrentState { get; set; }
        public PositionSortState PositionNameSort { get; set; }

        public PositionSortViewModel(PositionSortState sortOrder)
        {
            CurrentState = sortOrder;

            PositionNameSort = sortOrder == PositionSortState.PositionNameAsc
                ? PositionSortState.PositionNameDesc
                : PositionSortState.PositionNameAsc;
        }
    }
}
