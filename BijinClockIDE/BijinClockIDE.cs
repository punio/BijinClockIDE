using System;
using System.IO;
using System.Windows.Controls;
using System.Xml.Serialization;
using Microsoft.VisualStudio.Text.Editor;

namespace BijinClockIDE
{
	/// <summary>
	/// Adornment class that draws a square box in the top right hand corner of the viewport
	/// </summary>
	class BijinClockIDE
	{
		private Clock _clock;
		private IWpfTextView _view;
		private IAdornmentLayer _adornmentLayer;
		private Setting _setting;

		/// <summary>
		/// Creates a square image and attaches an event handler to the layout changed event that
		/// adds the the square in the upper right-hand corner of the TextView via the adornment layer
		/// </summary>
		/// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
		public BijinClockIDE(IWpfTextView view)
		{
			_view = view;

			try
			{
				var assemblylocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				var settinFilePath = Path.Combine(string.IsNullOrEmpty(assemblylocation) ? "" : assemblylocation, "Setting.xml");
				if (File.Exists(settinFilePath))
				{
					using (var sr = new StreamReader(settinFilePath))
					{
						var serializer = new XmlSerializer(typeof(Setting));
						_setting = (Setting)serializer.Deserialize(sr);
					}
				}
				if (_setting == null)
				{
					#region Default
					_setting = new Setting();
					_setting.Alignment = Setting.ContentAlignment.TopRight;
					_setting.Margin = new System.Windows.Thickness(10);
					_setting.Width = 250;
					_setting.Height = 250;
					_setting.Opacity = 0.3;
					_setting.Source.Add("http://www.bijint.com/assets/pict/sara/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/wasedastyle/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/hairstyle/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/megane/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/tokyo/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/yamanashi/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/nara/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/chiba/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/tottori/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/kagawa/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/kyoto/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/iwate/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/kanagawa/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/kumamoto/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/hiroshima/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/okinawa/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/osaka/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/sendai/pc/{0:HHmm}.jpg");
					_setting.Source.Add("http://www.bijint.com/assets/pict/cc/pc/{0:HHmm}.jpg");
					using (var sw = new StreamWriter(settinFilePath))
					{
						var serializer = new XmlSerializer(typeof(Setting));
						serializer.Serialize(sw, _setting);
					}
					#endregion
				}

				_clock = new Clock(_setting) { Opacity = _setting.Opacity };
			}
			catch { }

			_adornmentLayer = view.GetAdornmentLayer("BijinClockIDE");

			_view.LayoutChanged += delegate { this.onSizeChange(); };
			_view.ViewportHeightChanged += delegate { this.onSizeChange(); };
			_view.ViewportWidthChanged += delegate { this.onSizeChange(); };
		}

		public void onSizeChange()
		{
			//clear the adornment layer of previous adornments
			_adornmentLayer.RemoveAllAdornments();

			#region Size
			if (_setting.Width.HasValue)
			{
				_clock.Width = _setting.Width.Value;
			}
			else
			{
				_clock.Width = Math.Max(0, this._view.ViewportWidth - (_setting.Margin.Left + _setting.Margin.Right));
			}
			if (_setting.Height.HasValue)
			{
				_clock.Height = _setting.Height.Value;
			}
			else
			{
				_clock.Height = Math.Max(0, this._view.ViewportHeight - (_setting.Margin.Top + _setting.Margin.Bottom));
			}
			#endregion

			#region Top
			switch (_setting.Alignment)
			{
			case Setting.ContentAlignment.TopLeft:
			case Setting.ContentAlignment.TopCenter:
			case Setting.ContentAlignment.TopRight:
				Canvas.SetTop(this._clock, this._view.ViewportTop + _setting.Margin.Top);
				break;
			case Setting.ContentAlignment.BottomLeft:
			case Setting.ContentAlignment.BottomCenter:
			case Setting.ContentAlignment.BottomRight:
				Canvas.SetTop(this._clock, this._view.ViewportTop + this._view.ViewportHeight - (this._clock.Height + _setting.Margin.Bottom));
				break;
			default:
				Canvas.SetTop(this._clock, this._view.ViewportTop + this._view.ViewportHeight / 2 - this._clock.Height / 2);
				break;
			}
			#endregion

			#region Left
			switch (_setting.Alignment)
			{
			case Setting.ContentAlignment.TopLeft:
			case Setting.ContentAlignment.MiddleLeft:
			case Setting.ContentAlignment.BottomLeft:
				Canvas.SetLeft(this._clock, this._view.ViewportLeft + _setting.Margin.Left);
				break;
			case Setting.ContentAlignment.TopRight:
			case Setting.ContentAlignment.MiddleRight:
			case Setting.ContentAlignment.BottomRight:
				Canvas.SetLeft(this._clock, this._view.ViewportLeft + this._view.ViewportWidth - (this._clock.Width + _setting.Margin.Right));
				break;
			default:
				Canvas.SetLeft(this._clock, this._view.ViewportLeft + this._view.ViewportWidth / 2 - this._clock.Width / 2);
				break;
			}
			#endregion

			//add the image to the adornment layer and make it relative to the viewport
			_adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _clock, null);
		}
	}
}
