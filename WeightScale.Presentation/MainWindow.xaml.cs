using WeightScale.Presentation.ViewModel;

namespace WeightScale.Presentation
{
    public partial class MainWindow
    {
        private readonly MainViewModel _viewModel;
        
        public MainWindow(MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}