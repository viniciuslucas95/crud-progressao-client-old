namespace crud_progressao_library.DataTypes {
    public interface IDynamicConverter<T> where T : struct {
        public T Convert (dynamic data);
    }
}
