using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BijinClockIDE
{
	/// <summary>
	/// Clock.xaml の相互作用ロジック
	/// </summary>
	public partial class Clock : UserControl
	{
		public Clock(string path, Setting setting)
		{
			System.Diagnostics.Debug.WriteLine("Clock");
			InitializeComponent();
			_targetFolder = path;
			_setting = setting;
			if (_setting.Source.Count <= 0) return;

			_imageControls = new Image[2];
			_imageControls[0] = ClockImage1;
			_imageControls[1] = ClockImage2;
			this.Loaded += Clock_Loaded;
			this.Unloaded += Clock_Unloaded;
		}

		readonly string _targetFolder;
		readonly Setting _setting;
		readonly Random _r = new Random();
		DispatcherTimer _timer;
		DateTime _lastRequestTime = DateTime.MinValue;

		void Clock_Loaded(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Clock_Loaded");
			this.Loaded -= Clock_Loaded;

			if (_timer == null)
			{
				_timer = new DispatcherTimer();
				_timer.Interval = TimeSpan.FromSeconds(5);
				//timer_Tick(null, null);
			}
			_timer.Tick += timer_Tick;
			_timer.Start();
		}

		void Clock_Unloaded(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Clock_Unloaded");
			try
			{
				if (_timer == null) return;
				_timer.Tick -= timer_Tick;
				_timer.Stop();
				this.Loaded += Clock_Loaded;
			}
			catch { }
		}

		int _currentIndex = 0;
		readonly Image[] _imageControls;

		readonly DoubleAnimation _fadeOutAnimation = new DoubleAnimation()
		{
			From = 1,
			To = 0,
			Duration = TimeSpan.FromSeconds(2),
		};

		readonly DoubleAnimation _fadeInAnimation = new DoubleAnimation()
		{
			From = 0,
			To = 1,
			Duration = TimeSpan.FromSeconds(2),
		};

		async void timer_Tick(object sender, EventArgs e)
		{
			var requestTime = DateTime.Now;
			requestTime = new DateTime(requestTime.Year, requestTime.Month, requestTime.Day, requestTime.Hour, requestTime.Minute, 0);
			if (requestTime == _lastRequestTime) return;
			_lastRequestTime = requestTime;
			var url = _setting.Source[_r.Next(_setting.Source.Count)];
			try
			{
				await Dispatcher.BeginInvoke((Action)(() =>
				{
					var files = Directory.GetFiles(_targetFolder, $"{_lastRequestTime:HHmm}_*.png");
					if (files.Length <= 0) return;

					var targetFile = files[_r.Next(files.Length)];

					var bitmapImage = new BitmapImage();
					using (var stream = new WrappingStream(new MemoryStream(File.ReadAllBytes(targetFile))))
					{
						bitmapImage.BeginInit();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.StreamSource = stream;
						bitmapImage.EndInit();
						if (bitmapImage.CanFreeze) bitmapImage.Freeze();
					}

					var next = (_currentIndex + 1) % 2;
					_imageControls[next].Source = bitmapImage;
					_imageControls[_currentIndex].BeginAnimation(OpacityProperty, this._fadeOutAnimation);
					_imageControls[next].BeginAnimation(OpacityProperty, this._fadeInAnimation);
					_currentIndex = next;
				}));
			}
			catch (Exception exp)
			{
				System.Diagnostics.Debug.WriteLine("!!!! {0} / {1}", url, exp.Message);
			}
		}


		Guid _id = Guid.NewGuid();
	}
}
