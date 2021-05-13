namespace crud_progressao_library.DataTypes {
    public interface IDynamicConverter<T> where T : struct {
        T Convert (dynamic data);
    }
}
