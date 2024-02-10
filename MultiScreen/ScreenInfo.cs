using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace NetEti.MultiScreen
{
    /// <summary>
    /// Mindestinformationen über einen Screen.
    /// Thanks to Nils on StackOverflow http://stackoverflow.com/questions/1927540/how-to-get-the-size-of-the-current-screen-in-wpf.
    /// </summary>
    /// <remarks>
    /// File: ScreenInfo.cs
    /// Autor: Erik Nagel, NetEti
    ///
    /// 31.08.2015 Erik Nagel: erstellt.
    /// 17.01.2024 Erik Nagel: ClipToAllScreens, GetMainWindowScreenInfo und GetFirstScreenInfo implementiert.
    /// </remarks>
    public class ScreenInfo
    {
        #region public members

        /// <summary>
        /// Der (Device-)Name des Screens.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Die äußere Begrenzung des Screens.
        /// </summary>
        public Rect Bounds { get; set; }

        /// <summary>
        /// Die Abmessungen des Screens.
        /// </summary>
        public Rect WorkingArea { get; set; }

        /// <summary>
        /// True, wenn der Screen der primäre Bildschirm ist.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// True, wenn sich der übergebene Punkt innerhalb der
        /// Screen-Koordinaten des aktuellen Bildschirms befindet.
        /// </summary>
        /// <param name="point">Zu prüfender Punkt.</param>
        /// <returns>True, wenn sich der übergebene Punkt innerhalb des aktuellen
        /// Bildschirms befindet.</returns>
        public bool IsWithinActualScreenCoordinates(System.Windows.Point point)
        {
            return IsWithinActualScreenCoordinates(point, 0.0, 0.0);
        }

        /// <summary>
        /// True, wenn sich der übergebene Punkt mindestens mit dem Abstand margin
        /// innerhalb der Screen-Koordinaten des aktuellen Bildschirms befindet.
        /// </summary>
        /// <param name="point">Zu prüfender Punkt.</param>
        /// <param name="horizontalMargin">Horizontaler Mindestabstand zu den Bildschirmrändern.</param>
        /// <param name="verticalMargin">Vertikaler Mindestabstand zu den Bildschirmrändern.</param>
        /// <returns>True, wenn sich der übergebene Punkt inklusive margin innerhalb 
        /// des aktuellen Bildschirms befindet.</returns>
        public bool IsWithinActualScreenCoordinates(System.Windows.Point point, double horizontalMargin, double verticalMargin)
        {
            ScreenInfo screenInfo = GetLastActualScreenInfo();
            point.X += horizontalMargin;
            point.Y += verticalMargin;
            bool within = screenInfo.Bounds.Contains(point);
            point.X -= horizontalMargin * 2;
            point.Y -= verticalMargin * 2;
            return within && screenInfo.Bounds.Contains(point);
        }

        /// <summary>
        /// Liefert den übergebenen Punkt zurück, ändert aber, wenn erforderlich,
        /// seine Koordinaten so ab, dass der Punkt sich auf jeden Fall innerhalb
        /// der Koordinaten des aktuellen Bildschirms befindet.
        /// </summary>
        /// <param name="point">Punkt, der möglicherweise nicht innerhalb des aktuellen Bildschirms liegt.</param>
        /// <returns>Punkt, der auf jeden Fall innerhalb des aktuellen Bildschirms liegt.</returns>
        public System.Windows.Point GetNextPointWithinActualScreenCoordinates(System.Windows.Point point)
        {
            return GetNextPointWithinActualScreenCoordinates(point, 0.0, 0.0);
        }

        /// <summary>
        /// Liefert den übergebenen Punkt zurück, ändert aber, wenn erforderlich,
        /// seine Koordinaten so ab, dass der Punkt sich inklusive margin auf jeden
        /// Fall innerhalb der Koordinaten des aktuellen Bildschirms befindet.
        /// </summary>
        /// <param name="point">Punkt, der möglicherweise nicht innerhalb des aktuellen Bildschirms liegt.</param>
        /// <param name="horizontalMargin">Horizontaler Mindestabstand zu den Bildschirmrändern.</param>
        /// <param name="verticalMargin">Vertikaler Mindestabstand zu den Bildschirmrändern.</param>
        /// <returns>Punkt, der inklusive margin auf jeden Fall innerhalb des aktuellen Bildschirms liegt.</returns>
        public System.Windows.Point GetNextPointWithinActualScreenCoordinates(
          System.Windows.Point point, double horizontalMargin, double verticalMargin)
        {
            if (!IsWithinActualScreenCoordinates(point, horizontalMargin, verticalMargin))
            {
                ScreenInfo screenInfo = GetLastActualScreenInfo();
                point.X = point.X < screenInfo.Bounds.Left + horizontalMargin ? screenInfo.Bounds.Left + horizontalMargin : point.X;
                point.X = point.X > screenInfo.Bounds.Right - horizontalMargin ? screenInfo.Bounds.Right - horizontalMargin : point.X;
                point.X = point.X < screenInfo.Bounds.Left ? screenInfo.Bounds.Left : point.X;
                point.Y = point.Y < screenInfo.Bounds.Top + verticalMargin ? screenInfo.Bounds.Top + verticalMargin : point.Y;
                point.Y = point.Y > screenInfo.Bounds.Bottom - verticalMargin ? screenInfo.Bounds.Bottom - verticalMargin : point.Y;
                point.Y = point.Y < screenInfo.Bounds.Top ? screenInfo.Bounds.Top : point.Y;
            }
            return point;
        }

        /// <summary>
        /// Liefert den übergebenen Punkt zurück, ändert aber, wenn erforderlich,
        /// seine Koordinaten so ab, dass der Punkt sich inklusive Margins auf jeden
        /// Fall innerhalb der für alle Bildschirme maximalen Koordinaten befindet.
        /// </summary>
        /// <param name="point">Punkt, der möglicherweise nicht innerhalb des aktuellen Bildschirms liegt.</param>
        /// <param name="horizontalMargin">Horizontaler Mindestabstand zu den Bildschirmrändern.</param>
        /// <param name="verticalMargin">Vertikaler Mindestabstand zu den Bildschirmrändern.</param>
        /// <returns>Punkt, der inklusive margin auf jeden Fall innerhalb des aktuellen Bildschirms liegt.</returns>
        public static System.Windows.Point ClipToAllScreens(
          System.Windows.Point point, double horizontalMargin, double verticalMargin)
        {
            point.X = point.X > _allScreensMaxX - horizontalMargin ? _allScreensMaxX - horizontalMargin : point.X;
            point.X = point.X < 0 ? 0 : point.X;
            point.Y = point.Y > _allScreensMaxY - verticalMargin ? _allScreensMaxY - verticalMargin : point.Y;
            point.Y = point.Y < 0 ? 0 : point.Y;
            return point;
        }

        /// <summary>
        /// Liefert Eigenschaften des letzten aktuellen Bildschirms.
        /// </summary>
        /// <returns>Eigenschaften des letzten aktuellen Bildschirms</returns>
        public static ScreenInfo GetLastActualScreenInfo()
        {
            int actualScreenIndex = _lastScreenIndex;
            return _allScreenInfos[actualScreenIndex];
        }

        /// <summary>
        /// Liefert den Index des aktuellen Bildschirms in der Liste aller Bildschirme.
        /// </summary>
        /// <param name="window">Ein WPF-Window.</param>
        /// <returns>Index des aktuellen Bildschirms in der Liste aller Bildschirme.</returns>
        public static int? GetActualScreenInfoIndex(Window window)
        {
            return setActualScreenDimensions(window);
        }

        /// <summary>
        /// Liefert Eigenschaften des aktuellen Bildschirms.
        /// </summary>
        /// <param name="window">Ein WPF-Window.</param>
        /// <returns>Eigenschaften des aktuellen Bildschirms.</returns>
        public static ScreenInfo? GetActualScreenInfo(Window window)
        {
            int? actualScreenIndex = setActualScreenDimensions(window);
            if (actualScreenIndex == null)
            {
                return null;
            }
            return _allScreenInfos[(int)actualScreenIndex];
        }

        /// <summary>
        /// Liefert Eigenschaften des ersten (Haupt-) Bildschirms.
        /// </summary>
        /// <returns>Eigenschaften des ersten (Haupt-) Bildschirms.</returns>
        public static ScreenInfo GetFirstScreenInfo()
        {
            return _allScreenInfos[0];
        }

        /// <summary>
        /// Liefert thread safe Position und Maße des MainWindow.
        /// </summary>
        /// <returns>Bildschirminformationen zum MainWindow.</returns>
        public static ScreenInfo? GetMainWindowScreenInfo()
        {
            return (ScreenInfo?)System.Windows.Application.Current.Dispatcher.Invoke(
                new Func<ScreenInfo?>(ThreadAccessMainWindowScreenInfoOnGuiDispatcher), DispatcherPriority.Normal);
        }

        /// <summary>
        /// Liefert thread safe Position und Maße des MainWindow.
        /// </summary>
        /// <returns>Bildschirminformationen zum MainWindow.</returns>
        private static ScreenInfo? ThreadAccessMainWindowScreenInfoOnGuiDispatcher()
        {
            Window mainWindow = System.Windows.Application.Current.MainWindow;
            return ScreenInfo.GetActualScreenInfo(mainWindow);
        }

        /// <summary>
        /// Infos für alle Bildschirme plus einem virtuellen Gesamtbildschirm.
        /// </summary>
        /// <param name="window">Ein WPF-Window.</param>
        /// <returns>Eigenschaften aller Bildschirme plus eines virtuellen Gesamtbildschirms.</returns>
        public static List<ScreenInfo> GetAllScreenInfos(Window window)
        {
            setActualScreenDimensions(window);
            return _allScreenInfos;
        }

        /// <summary>
        /// Standard-Konstruktor.
        /// </summary>
        public ScreenInfo() { }

        #endregion public members

        #region private members

        private static List<ScreenInfo> _allScreenInfos = new List<ScreenInfo>();
        private static int _lastScreenIndex;
        private static double _allScreensMaxX;
        private static double _allScreensMaxY;

        static ScreenInfo()
        {
            _allScreenInfos = new List<ScreenInfo>();
            _lastScreenIndex = 0;
            _allScreensMaxX = double.MinValue;
            _allScreensMaxY = double.MinValue;
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Rect actBounds = ScreenInfo.getRect(Screen.AllScreens[i].Bounds);
                _allScreensMaxX = _allScreensMaxX < actBounds.BottomRight.X ? actBounds.BottomRight.X : _allScreensMaxX;
                _allScreensMaxY = _allScreensMaxY < actBounds.BottomRight.Y ? actBounds.BottomRight.Y : _allScreensMaxY;
                _allScreenInfos.Add(
                  new ScreenInfo()
                  {
                      Name = Screen.AllScreens[i].DeviceName,
                      IsPrimary = Screen.AllScreens[i].Primary,
                      Bounds = actBounds,
                      WorkingArea = ScreenInfo.getRect(Screen.AllScreens[i].WorkingArea)
                  });
            }
            _allScreenInfos.Add(
              new ScreenInfo()
              {
                  Name = "VirtualScreen",
                  IsPrimary = true,
                  Bounds = new Rect(0, 0, System.Windows.SystemParameters.VirtualScreenWidth, System.Windows.SystemParameters.VirtualScreenHeight),
                  WorkingArea = new Rect(0, 0, System.Windows.SystemParameters.VirtualScreenWidth, System.Windows.SystemParameters.VirtualScreenHeight)
              });
        }

        private static int? setActualScreenDimensions(Window window)
        {
            int actualScreenIndex = getAktualScreenIndex(window);
            PresentationSource mainWindowPresentationSource = PresentationSource.FromVisual(window);
            if (mainWindowPresentationSource == null)
            {
                return null;
            }
            Matrix m = mainWindowPresentationSource.CompositionTarget.TransformToDevice;
            double DpiWidthFactor = m.M11;
            double DpiHeightFactor = m.M22;

            Rect bounds = ScreenInfo.getRect(Screen.AllScreens[actualScreenIndex].Bounds);
            bounds.Width /= DpiWidthFactor;
            bounds.Height /= DpiHeightFactor;
            _allScreenInfos[actualScreenIndex].Bounds = bounds;
            Rect workingArea = ScreenInfo.getRect(Screen.AllScreens[actualScreenIndex].WorkingArea);
            workingArea.Width /= DpiWidthFactor;
            workingArea.Height /= DpiHeightFactor;
            _allScreenInfos[actualScreenIndex].WorkingArea = workingArea;
            return actualScreenIndex;
        }

        private static int getAktualScreenIndex(Window window)
        {
            int actualScreenIndex = -1;
            double actualLeft = window.ActualLeft();
            double actualTop = window.ActualTop();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                bool isActualScreen = actualLeft + window.ActualWidth / 2 >= Screen.AllScreens[i].Bounds.Left && actualTop + window.ActualHeight / 2 >= Screen.AllScreens[i].Bounds.Top
                  && actualLeft + window.ActualWidth / 2 <= Screen.AllScreens[i].Bounds.Right && actualTop + window.ActualHeight / 2 <= Screen.AllScreens[i].Bounds.Bottom;
                if (isActualScreen)
                {
                    actualScreenIndex = i;
                }
            }
            if (actualScreenIndex < 0)
            {
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    bool isActualScreen = actualLeft >= Screen.AllScreens[i].Bounds.Left && actualTop + window.ActualHeight / 2 >= Screen.AllScreens[i].Bounds.Top
                      && actualLeft <= Screen.AllScreens[i].Bounds.Right && actualTop + window.ActualHeight / 2 <= Screen.AllScreens[i].Bounds.Bottom;
                    if (isActualScreen)
                    {
                        actualScreenIndex = i;
                    }
                }
                if (actualScreenIndex < 0)
                {
                    for (int i = 0; i < Screen.AllScreens.Length; i++)
                    {
                        bool isActualScreen = actualLeft + window.ActualWidth / 2 >= Screen.AllScreens[i].Bounds.Left && actualTop >= Screen.AllScreens[i].Bounds.Top
                          && actualLeft + window.ActualWidth / 2 <= Screen.AllScreens[i].Bounds.Right && actualTop <= Screen.AllScreens[i].Bounds.Bottom;
                        if (isActualScreen)
                        {
                            actualScreenIndex = i;
                        }
                    }
                    if (actualScreenIndex < 0)
                    {
                        for (int i = 0; i < Screen.AllScreens.Length; i++)
                        {
                            bool isActualScreen = actualLeft >= Screen.AllScreens[i].Bounds.Left && actualTop >= Screen.AllScreens[i].Bounds.Top
                              && actualLeft <= Screen.AllScreens[i].Bounds.Right && actualTop <= Screen.AllScreens[i].Bounds.Bottom;
                            if (isActualScreen)
                            {
                                actualScreenIndex = i;
                            }
                        }
                    }
                }
            }
            if (actualScreenIndex < 0)
            {
                StringBuilder sb = new StringBuilder();
                string delimiter = "";
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    sb.Append(delimiter + String.Format("sbL: {0}, sbT: {1}, sbR: {2}, sbB: {3}, L: {4}, T: {5}, W: {6}, H: {7}",
                        Screen.AllScreens[i].Bounds.Left, Screen.AllScreens[i].Bounds.Top, Screen.AllScreens[i].Bounds.Right, Screen.AllScreens[i].Bounds.Bottom,
                        window.Left, window.Top, window.ActualWidth, window.ActualHeight));
                    delimiter = Environment.NewLine;
                }
                // Exception wg. Window.Left = -25000
                // throw new ApplicationException(sb.ToString());
                actualScreenIndex = _lastScreenIndex;
            }
            _lastScreenIndex = actualScreenIndex;
            return actualScreenIndex;
        }

        private static Rect getRect(Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        #endregion private members

    }
}
