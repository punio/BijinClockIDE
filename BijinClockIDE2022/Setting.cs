using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BijinClockIDE {
	public class Setting {
		// 概要:
		//     描画面でのコンテンツの配置方法を指定します。
		public enum ContentAlignment {
			// 概要:
			//     コンテンツは上端左寄せに配置されます。
			TopLeft = 1,
			//
			// 概要:
			//     コンテンツは上端中央に配置されます。
			TopCenter = 2,
			//
			// 概要:
			//     コンテンツは上端右寄せに配置されます。
			TopRight = 4,
			//
			// 概要:
			//     コンテンツは中段左寄せに配置されます。
			MiddleLeft = 16,
			//
			// 概要:
			//     コンテンツは中段中央に配置されます。
			MiddleCenter = 32,
			//
			// 概要:
			//     コンテンツは中段右寄せに配置されます。
			MiddleRight = 64,
			//
			// 概要:
			//     コンテンツは下端左寄せに配置されます。
			BottomLeft = 256,
			//
			// 概要:
			//     コンテンツは下端中央に配置されます。
			BottomCenter = 512,
			//
			// 概要:
			//     コンテンツは下端右寄せに配置されます。
			BottomRight = 1024,
		}

		public Thickness Margin { get; set; }
		public ContentAlignment Alignment { get; set; }
		public double? Width { get; set; }
		public double? Height { get; set; }
		public double Opacity { get; set; }
		public List<string> Source { get; set; }

		public Setting() {
			Source = new List<string>();
		}
	}
}
