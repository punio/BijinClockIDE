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
		public Clock(Setting setting)
		{
			InitializeComponent();
			_setting = setting;
			if (_setting.Source.Count <= 0) return;

			_imageControls = new Image[2];
			_imageControls[0] = ClockImage1;
			_imageControls[1] = ClockImage2;
			this.Loaded += Clock_Loaded;
			this.Unloaded += Clock_Unloaded;
		}

		Setting _setting;
		Random _r = new Random();
		DispatcherTimer _timer;
		void Clock_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= Clock_Loaded;

			if (_timer == null)
			{
				_timer = new DispatcherTimer();
#if DEBUG
				_timer.Interval = TimeSpan.FromSeconds(5);
#else
				_timer.Interval = TimeSpan.FromSeconds(60);
#endif
				_timer.Tick += timer_Tick;
				_timer.Start();
				timer_Tick(null, null);
			}
		}

		void Clock_Unloaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (_timer == null) return;
				_timer.Tick -= timer_Tick;
				_timer.Stop();
				_timer = null;
				this.Loaded += Clock_Loaded;
			}
			catch { }
		}

		int _currentIndex = 0;
		Image[] _imageControls;
		DoubleAnimation FadeOutAnimation = new DoubleAnimation()
		{
			From = 1,
			To = 0,
			Duration = TimeSpan.FromSeconds(2),
		};
		DoubleAnimation FadeInAnimation = new DoubleAnimation()
		{
			From = 0,
			To = 1,
			Duration = TimeSpan.FromSeconds(2),
		};

		async void timer_Tick(object sender, EventArgs e)
		{
			var url = _setting.Source[_r.Next(_setting.Source.Count)];
			try
			{
				using (var client = new HttpClient())
				{
					var buffer = await client.GetByteArrayAsync(String.Format(url, DateTime.Now));
					var bitmapImage = new BitmapImage();

					using (var stream = new MemoryStream(buffer))
					{
						bitmapImage.BeginInit();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.StreamSource = stream;
						bitmapImage.EndInit();
						bitmapImage.Freeze();
					}

					await Dispatcher.BeginInvoke((Action)(() =>
					{
						var next = (_currentIndex + 1) % 2;
						_imageControls[next].Source = bitmapImage;
						_imageControls[_currentIndex].BeginAnimation(Control.OpacityProperty, FadeOutAnimation);
						_imageControls[next].BeginAnimation(Control.OpacityProperty, FadeInAnimation);
						_currentIndex = next;
					}));
				}
			}
			catch (Exception exp)
			{
				System.Diagnostics.Debug.WriteLine("!!!! {0} / {1}", url, exp.Message);
			}
		}
	}
}
