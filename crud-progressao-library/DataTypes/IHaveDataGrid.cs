namespace crud_progressao_library.DataTypes {
    public interface IHaveDataGrid {
        public void SelectAndScrollToItemInDataGrid<T>(T item);
    }
}
