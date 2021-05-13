using crud_progressao_library.DataTypes;
using System.Collections.ObjectModel;

namespace crud_progressao_library.Scripts {
    public static class DynamicToObservableCollectionConverter {
        public static ObservableCollection<T> Convert<T>(dynamic data, IDynamicConverter<T> converter) where T : struct {
            ObservableCollection<T> collection = new();

            if (data.Count == 0) return collection;

            for (int i = 0; i < data.Count; i++)
                collection.Add(converter.Convert(data[i]));

            return collection;
        }
    }
}
