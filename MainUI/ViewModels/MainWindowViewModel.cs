using Prism.Mvvm;

namespace MainUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Google Speech Services";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
