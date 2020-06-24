using Prism.Mvvm;

namespace MainUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Balconette’s Speech Services";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public MainWindowViewModel()
        {
        }
    }
}
