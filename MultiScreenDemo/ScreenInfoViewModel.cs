using NetEti.MultiScreen;
using System.ComponentModel;
using System.Windows;

namespace NetEti.DemoApplications
{
	/// <summary>
	/// ViewModel für eine ScreenInfo-Instanz..
	/// </summary>
	/// <remarks>
	/// File: ScreenInfoViewModel.cs
	/// Autor: Erik Nagel, NetEti
	///
	/// 31.08.2015 Erik Nagel: erstellt
	/// </remarks>
	public class ScreenInfoViewModel : INotifyPropertyChanged
	{
		#region public members

		#region INotifyPropertyChanged implementation

		/// <summary>
		/// Bindeglied zur UI - wird angesprungen, wenn sich Properties geändert haben.
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		#endregion INotifyPropertyChanged implementation

		/// <summary>
		/// Der (Device-)Name des Screens.
		/// </summary>
		public string? Name
		{
			get
			{
				return this._screenInfo.Name;
			}
			set
			{
				if (this._screenInfo.Name != value)
				{
					this._screenInfo.Name = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
				}
			}
		}

		/// <summary>
		/// Die äußere Begrenzung des Screens.
		/// </summary>
		public Rect Bounds
		{
			get
			{
				return this._screenInfo.Bounds;
			}
			set
			{
				if (this._screenInfo.Bounds != value)
				{
					this._screenInfo.Bounds = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("Bounds"));
				}
			}
		}

		/// <summary>
		/// Die Arbeitsfläche des Screens abzüglich Taskbar und angedockter Fenster.
		/// </summary>
		public Rect WorkingArea
		{
			get
			{
				return this._screenInfo.WorkingArea;
			}
			set
			{
				if (this._screenInfo.WorkingArea != value)
				{
					this._screenInfo.WorkingArea = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("WorkingArea"));
				}
			}
		}

		/// <summary>
		/// True, wenn der Screen der Priäre Bildschirm ist.
		/// </summary>
		public bool IsPrimary
		{
			get
			{
				return this._screenInfo.IsPrimary;
			}
			set
			{
				if (this._screenInfo.IsPrimary != value)
				{
					this._screenInfo.IsPrimary = value;
				}
			}
		}

		/// <summary>
		/// True, wenn die Anwendung sich zu mehr als der Hälfte
		/// ihres MainWindow auf diesem Screen befindet.
		/// </summary>
		public virtual bool IsActualScreen
		{
			get
			{
				return this._isActualScreen;
			}
			set
			{
				if (this._isActualScreen != value)
				{
					this._isActualScreen = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("IsActualScreen"));
				}
			}
		}

		/// <summary>
		/// Konstruktor - übernimmt eine MultiScreen.ScreenInfo Instanz.
		/// </summary>
		/// <param name="screenInfo"></param>
		public ScreenInfoViewModel(ScreenInfo screenInfo)
		{
			this._screenInfo = screenInfo;
		}

		#endregion public members

		#region private members

		/// <summary>
		/// Meldet für die UI, wenn sich eine Property geändert hat.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private ScreenInfo _screenInfo;
		private bool _isActualScreen;

		#endregion private members

	}
}
