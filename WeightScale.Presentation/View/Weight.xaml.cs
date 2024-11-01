using System.Windows.Controls;
using System.Windows.Input;

namespace WeightScale.Presentation.View
{
    public partial class Weight : UserControl
    {
        public Weight()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MainScrollViewer != null)
            {
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}
