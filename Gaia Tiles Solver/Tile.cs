using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Gaia_Tiles_Solver
{
	class Tile
	{
		private readonly Bitmap image;

		public string ForegroundHash;
		public string BackgroundHash;

		public Point ImagePos;

		public Tile(Bitmap image, Point ImagePos)
		{
			this.image = image;
			this.ImagePos = ImagePos;

			ForegroundHash = GenerateHashForRegion(new Rectangle(new Point(14, 50), new Size(7, 6)));
			BackgroundHash = GenerateHashForRegion(new Rectangle(new Point(13, 13), new Size(40, 10)));
		}

		private string GenerateHashForRegion(Rectangle region)
		{
			Bitmap regionImage = image.Clone(region, PixelFormat.Format32bppArgb);

			byte[] regionPixels = new byte[regionImage.Width * regionImage.Height * 3];

			var length = 0;
			for (var x = 0; x < regionImage.Width; x++)
			{
				for (var y = 0; y < regionImage.Height; y++)
				{
					var tempPixel = image.GetPixel(x + region.X, y + region.Y);
					regionPixels[length] = tempPixel.R;
					regionPixels[length + 1] = tempPixel.G;
					regionPixels[length + 2] = tempPixel.B;
				}
				length += 3;
			}

			var hash = BitConverter.ToString(Program.Sha1.ComputeHash(regionPixels).Take(8).ToArray()).Replace("-", "").ToLower();
			return hash;
		}
		// ROCK 524a5a67ed3c3c3e e7d4d20ac8fdd5b0
		public static bool operator ==(Tile tile1, Tile tile2)
		{
			if ((tile1.BackgroundHash == tile2.BackgroundHash
				|| tile1.ForegroundHash == tile2.ForegroundHash
				|| tile1.BackgroundHash == "bbb2fa7164723a14"  //! WildCard
				|| tile1.ForegroundHash == "d2daaca3f519ce5d"
				|| tile2.BackgroundHash == "bbb2fa7164723a14"  //! WildCard
				|| tile2.ForegroundHash == "d2daaca3f519ce5d")
				&& tile1.ForegroundHash != "524a5a67ed3c3c3e"
				&& tile2.ForegroundHash != "524a5a67ed3c3c3e"
				&& tile1.BackgroundHash != "e7d4d20ac8fdd5b0"
				&& tile2.BackgroundHash != "e7d4d20ac8fdd5b0")
				return true;
			return false;
		}

		public static bool operator !=(Tile tile1, Tile tile2)
		{
			return !(tile1 == tile2);
		}
	}
}
