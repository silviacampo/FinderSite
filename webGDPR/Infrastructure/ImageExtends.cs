using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Infrastructure
{
    public static class ImageExtends
    {
		private static int MaxSize = 120;

		public static Image Resize(this Image current)
		{
			int width, height;
			#region reckon size
			if (current.Width > current.Height)
			{
				width = MaxSize;
				height = Convert.ToInt32(current.Height * MaxSize / (double)current.Width);
			}
			else
			{
				width = Convert.ToInt32(current.Width * MaxSize / (double)current.Height);
				height = MaxSize;
			}
			#endregion

			#region get resized bitmap
			var canvas = new Bitmap(width, height);

			using (var graphics = Graphics.FromImage(canvas))
			{
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.DrawImage(current, 0, 0, width, height);
			}

			return canvas;
			#endregion
		}

		public static byte[] ToByteArray(this Image current)
		{
			using (var stream = new MemoryStream())
			{
				current.Save(stream, current.RawFormat);
				return stream.ToArray();
			}
		}
	}
}
