using crud_progressao_library.Constants;
using crud_progressao_library.Services;
using System.ComponentModel;
using System.Windows.Media;

namespace crud_progressao_library.ViewModels {
    public class BaseViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        #region UI Bindings
        public bool IsControlsEnabled {
            get => _isControlsEnabled;
            private set {
                _isControlsEnabled = value;
                OnPropertyChange(nameof(IsControlsEnabled));
            }
        }
        public string LabelFeedbackText {
            get => _labelFeedbackText;
            private set {
                _labelFeedbackText = value;
                OnPropertyChange(nameof(LabelFeedbackText));
            }
        }
        public Brush LabelFeedbackColor {
            get => _labelFeedbackColor;
            private set {
                _labelFeedbackColor = value;
                OnPropertyChange(nameof(LabelFeedbackColor));
            }
        }

        private bool _isControlsEnabled = true;
        private string _labelFeedbackText;
        private Brush _labelFeedbackColor = BrushColors.Black;
        #endregion

        public void SetFeedbackContent(string text, bool error = false) {
            LabelFeedbackText = text;
            LabelFeedbackColor = error ? BrushColors.Red : BrushColors.Black;
        }

        protected virtual void EnableControls(bool value) {
            IsControlsEnabled = value;
        }

        protected bool IsProcessingAsyncOperation() {
            if (ServerApi.IsProcessingAsyncOperation) {
                SetFeedbackContent("Ainda processando operação!", true);
                return true;
            }

            return false;
        }

        protected void OnPropertyChange(string props) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(props));
        }
    }
}
