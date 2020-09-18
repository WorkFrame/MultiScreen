using NetEti.MultiScreen;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace NetEti.DemoApplications.MultiScreenDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region public members

    /// <summary>
    /// Liste von ScreenInfo-Instanzen für alle Screens plus einen virtuellen Gesamt-Screen.
    /// </summary>
    public ObservableCollection<ScreenInfoViewModel> ScreenInfos {
      get
      {
        return this._screenInfos;
      }
      set
      {
        if (this._screenInfos != value)
        {
          this._screenInfos = value;
        }
      }
    }

    /// <summary>
    /// Konstruktor - initialisiert die ObservableCollection ScreenInfos.
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

      this.DataContext = this;
      this.ScreenInfos = new ObservableCollection<ScreenInfoViewModel>();
    }

    #endregion public members

    #region private members

    private ObservableCollection<ScreenInfoViewModel> _screenInfos { get; set; }

    private void refreshScreenInfos()
    {
      foreach (ScreenInfo screenInfo in ScreenInfo.GetAllScreenInfos(this))
      {
        ScreenInfoViewModel screenInfoViewModel = new ScreenInfoViewModel(screenInfo);
        screenInfoViewModel.IsActualScreen = false;
        ScreenInfos.Add(screenInfoViewModel);
      }
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
      this.actualizeScreenInfos();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      this.refreshScreenInfos();
      this.actualizeScreenInfos();
    }

    private void actualizeScreenInfos()
    {
      string actualScreenName = ScreenInfo.GetActualScreenInfo(this).Name;
      foreach (ScreenInfoViewModel screenInfoViewModel in this.ScreenInfos)
      {
        if (screenInfoViewModel.Name == actualScreenName || screenInfoViewModel.Name == "VirtualScreen")
        {
          screenInfoViewModel.IsActualScreen = true;
        }
        else
        {
          screenInfoViewModel.IsActualScreen = false;
        }
      }
    }

    #endregion private members
  }
}
