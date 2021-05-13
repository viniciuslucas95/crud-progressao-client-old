using crud_progressao_library.Services;
using System.Windows.Controls;

namespace crud_progressao_library.Scripts {
    public static class AsyncOperationChecker {
        public static bool Check(Label label = null) {
            if (ServerApi.IsProcessingAsyncOperation) {
                if (label != null)
                    LabelTextSetter.SetText(label, $"Ainda processando operação!", true);

                return true;
            }

            return false;
        }
    }
}
