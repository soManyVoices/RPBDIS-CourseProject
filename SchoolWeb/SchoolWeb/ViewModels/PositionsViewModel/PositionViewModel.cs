using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWeb.ViewModels.SortViewModels;
using System.ComponentModel.DataAnnotations;
using SchoolWeb.DataLayer.Models;

namespace SchoolWeb.ViewModels.PositionsViewModel
{
    public class PositionViewModel
    {
       
        public IEnumerable<Position> Positions { get; set; }

        public FilterPositionViewModel FilterPositionViewModel { get; set; }

        //Свойство для навигации по страницам
        public PageViewModel PageViewModel { get; set; }
        // Порядок сортировки
        public PositionSortViewModel SortViewModel { get; set; }

        public SelectList SelectedPositionNameList { get; set; }
    }
}
