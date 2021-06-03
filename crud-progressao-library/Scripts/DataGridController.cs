using System.Windows.Controls;

namespace crud_progressao_library.Scripts {
    public static class DataGridController {
        public static void SelectAndScrollToItemInDataGrid<T>(DataGrid dataGrid, T item) {
            dataGrid.SelectedItem = item;
            dataGrid.ScrollIntoView(item);
        }
    }
}
