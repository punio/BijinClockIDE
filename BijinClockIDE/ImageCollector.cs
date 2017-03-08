using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BijinClockIDE
{
	class ImageCollector
	{
		readonly string _targetFolder;
		readonly Setting _setting;
		readonly System.Timers.Timer _collectTimer;
		DateTime _lastRequestTime = DateTime.MinValue;

		public ImageCollector(string path, Setting setting)
		{
			_targetFolder = path;
			_setting = setting;

			_collectTimer = new System.Timers.Timer(10 * 1000);
			_collectTimer.Elapsed += _collectTimer_Elapsed;
			_collectTimer.Start();
		}

		private void _collectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			var requestTime = DateTime.Now.AddMinutes(1);
			requestTime = new DateTime(requestTime.Year, requestTime.Month, requestTime.Day, requestTime.Hour, requestTime.Minute, 0);
			if (requestTime == _lastRequestTime) return;
			_lastRequestTime = requestTime;

			Log("timer");

			var mutex = new Mutex(false, @"Global\BijinClockIde");
			if (!mutex.WaitOne(100))
			{
				mutex.Close();
				return;
			}

			#region Pre Download
			for (var i = 0; i < _setting.Source.Count; i++)
			{
				var fileName = Path.Combine(_targetFolder, $"{_lastRequestTime:HHmm}_{i}.png");
				if (File.Exists(fileName)) continue;

				try
				{
					using (var client = new HttpClient())
					{
						Log("begin load");
						var task = client.GetByteArrayAsync(string.Format(_setting.Source[i], requestTime));
						task.Wait();
						var buffer = task.Result;
						Log("save");
						File.WriteAllBytes(fileName, buffer);
					}
				}
				catch (Exception exp)
				{
					Log($"Download error.{exp.Message}");
				}
			}
			#endregion

			#region Delete old files
			var dirInfo = new DirectoryInfo(_targetFolder);
			var files = dirInfo.GetFiles();

			try
			{
				foreach (var file in files.Where(file => Path.GetExtension(file.FullName).ToLower() == ".png" && ((this._lastRequestTime - file.LastAccessTime).TotalMinutes > 10)))
				{
					if (Path.GetFileName(file.FullName).ToLower() == "preview.png") continue;
					File.Delete(file.FullName);
				}
			}
			catch (Exception exp)
			{
				Log($"Delete error.{exp.Message}");
			}
			#endregion

			mutex.ReleaseMutex();
			mutex.Close();
		}

		readonly Guid _id = Guid.NewGuid();
		readonly HdLog46.Logger _logger = new HdLog46.Logger("BijinClockIde");
		void Log(string log) => this._logger.Information($"{_id} {log}");
	}
}
