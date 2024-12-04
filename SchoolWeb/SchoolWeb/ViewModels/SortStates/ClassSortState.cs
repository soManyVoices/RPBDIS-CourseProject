namespace SchoolWeb.ViewModels.SortStates
{
    public enum ClassSortState
    {
        No,               // Не сортировать
        ClassNameAsc,     // Название класса по возрастанию
        ClassNameDesc,    // Название класса по убыванию
        YearCreatedAsc,   // Год создания по возрастанию
        YearCreatedDesc,  // Год создания по убыванию
        StudentCountAsc,  // Количество учеников по возрастанию
        StudentCountDesc, // Количество учеников по убыванию
        ClassTypeAsc,     // Сортировка по типу класса (по возрастанию)
        ClassTypeDesc     // Сортировка по типу класса (по убыванию)
    }
}
