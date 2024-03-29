using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using System.Linq;
using System;
using System.IO.Packaging;
using System.Windows.Input;

namespace ReactveUI_WPF_StartPoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private IDisposable _renderingGridMouseMove;
        private IDisposable _imageMouseMove;

        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            ViewModel = mainWindowViewModel;

            DataContextChanged += (sender, args) => ViewModel = DataContext as MainWindowViewModel;

            //set initial size using notifyimagecontainersizechanged



            this.WhenActivated(cleanup =>
            {
                Observable.FromEventPattern<SizeChangedEventArgs>(RenderGrid, nameof(RenderGrid.SizeChanged))
                          .Subscribe(s => ViewModel.NotifyImageContainerSizeChanged(s.EventArgs))
                          .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                             vm => vm.ImageSource,
                             v => v.RenderingImage.Source)
                 .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                             vm => vm.TransformGroup,
                             v => v.RenderingImage.RenderTransform)
                 .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                             vm => vm.MousePositionGridX,
                             v => v.MousePositionGridOnGridX.Text)
                 .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                             vm => vm.MousePositionGridY,
                             v => v.MousePositionGridOnGridY.Text)
                 .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                                vm => vm.MousePositionGridRelativeToImageX,
                                v => v.MousePositionImageOnGridX.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                                vm => vm.MousePositionGridRelativeToImageY,
                                v => v.MousePositionImageOnGridY.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                    vm => vm.MousePositionImageX,
                    v => v.MousePositionImageOnImageX.Text)
                     .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                             vm => vm.MousePositionImageY,
                             v => v.MousePositionImageOnImageY.Text)
                 .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                                vm => vm.MousePositionImageRelativeToGridX,
                                v => v.MousePositionGridOnImageX.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                                vm => vm.MousePositionImageRelativeToGridY,
                                v => v.MousePositionGridOnImageY.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                              vm => vm.TransformGroup,
                              v => v.DrawingGrid.RenderTransform)
                  .DisposeWith(cleanup);    
                
                this.OneWayBind(ViewModel,
                                vm => vm.OnDemandXPosition,
                                v => v.MousePositionOnDemandX.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                              vm => vm.OnDemandYPosition,
                              v => v.MousePositionOnDemandY.Text)
                  .DisposeWith(cleanup);     
                
                this.OneWayBind(ViewModel,
                                vm => vm.MouseEnterPositionX,
                                v => v.MouseEnterPositionX.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                              vm => vm.MouseEnterPositionY,
                              v => v.MouseEnterPositionY.Text)
                  .DisposeWith(cleanup);        
                
                this.OneWayBind(ViewModel,
                                vm => vm.MouseLeavePositionX,
                                v => v.MouseLeavePositionX.Text)
                .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                              vm => vm.MouseLeavePositionY,
                              v => v.MouseLeavePositionY.Text)
                  .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                              vm => vm.CurrentZoom,
                              v => v.CurrentZoom.Text)
                  .DisposeWith(cleanup);
                         
                Observable.FromEventPattern<MouseEventArgs>(RenderGrid, nameof(RenderGrid.MouseMove))
                          .Subscribe(s => ViewModel.RenderGridMouseMoveEventForGrid(s.EventArgs.GetPosition(RenderGrid)))
                          .DisposeWith(cleanup);

                Observable.FromEventPattern<MouseEventArgs>(RenderGrid, nameof(RenderGrid.MouseMove))
                            .Subscribe(s => ViewModel.RenderGridMouseMoveEventForImageSource(s.EventArgs.GetPosition(RenderingImage)))
                            .DisposeWith(cleanup);

                Observable.FromEventPattern<MouseEventArgs>(RenderingImage, nameof(RenderingImage.MouseMove))
                          .Subscribe(s => ViewModel.ImageMouseMoveEventForImage(s.EventArgs.GetPosition(RenderingImage)))
                          .DisposeWith(cleanup);

                Observable.FromEventPattern<MouseEventArgs>(RenderingImage, nameof(RenderingImage.MouseMove))
                            .Subscribe(s => ViewModel.ImageMouseMoveEventForGrid(s.EventArgs.GetPosition(RenderGrid)))
                            .DisposeWith(cleanup);

                Observable.FromEventPattern<MouseButtonEventArgs>(RenderingImage, nameof(RenderingImage.PreviewMouseDown))
                            .Subscribe(s => ViewModel.ImageMouseDownEventForImage(s.EventArgs, s.Sender))
                            .DisposeWith(cleanup);

                Observable.FromEventPattern<MouseEventArgs>(RenderingImage, nameof(RenderingImage.MouseEnter))
                            .Subscribe(s => ViewModel.ImageMouseEnterEventForImage(s.EventArgs.GetPosition(RenderingImage)))
                            .DisposeWith(cleanup);

                Observable.FromEventPattern<MouseEventArgs>(RenderingImage, nameof(RenderingImage.MouseLeave))
                            .Subscribe(s => ViewModel.ImageMouseLeaveEventForImage(s.EventArgs.GetPosition(RenderingImage)))
                            .DisposeWith(cleanup);
        });
        }
}
}
