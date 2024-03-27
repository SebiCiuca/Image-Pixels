using System.Reactive.Disposables;
using ReactiveUI;

namespace ReactveUI_WPF_StartPoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            
            ViewModel = mainWindowViewModel;

            DataContextChanged += (sender, args) => ViewModel = DataContext as MainWindowViewModel;

            this.WhenActivated(cleanup =>
            {
                //this.BindCommand(ViewModel, vm => vm.RandonListCommand, view => view.RandomButton).DisposeWith(cleanup);
            });
        }
    }
}
