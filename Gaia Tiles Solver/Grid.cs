using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace Gaia_Tiles_Solver
{
	class Grid
	{
		private const int tileWidth = 79;
		private const int tileHeight = 78;
		private const int tileOffset_x = 15;
		private const int tileOffset_y = 13;

		public enum HEX_DIR
		{
			DOWN_RIGHT = 0,
			UP_RIGHT = 1,
			UP = 2,
			UP_LEFT = 3,
			DOWN_LEFT = 4,
			DOWN = 5,
		};

		private List<List<Point>> pDirections = new List<List<Point>> { new List<Point> { new Point(1, 0), new Point(1, -1), new Point(0, -1), new Point(-1, -1), new Point(-1, 0), new Point(0, 1 ) },
																		new List<Point> { new Point(1, 1), new Point(1, 0), new Point(0, -1), new Point(-1, 0), new Point(-1, 1), new Point(0, 1 ) }};
		private Tile[,] Tiles = new Tile[6, 5];

		public Grid(Bitmap grid)
		{
			var img = grid;
			var imgbounds = new Rectangle(new Point(0, 0), new Size(img.Width, img.Height));

			var cutup = new Bitmap(img.Width, img.Height);
			var cutg = Graphics.FromImage(cutup);

			for (var tile_x = 0; tile_x < Tiles.GetLength(0); tile_x++)
			{
				for (var tile_y = 0; tile_y < Tiles.GetLength(1); tile_y++)
				{
					var image_x = tileOffset_x + (tile_x * tileWidth);
					var image_y = tileOffset_y + (tile_y * tileHeight) + (tile_x % 2 != 0 ? tileHeight / 2 : 0);

					var temp = new Bitmap(tileWidth - tileOffset_x, tileHeight - tileOffset_y);
					var tempg = Graphics.FromImage(temp);

					var rect = new Rectangle(new Point(image_x, image_y), new Size(tileWidth, tileHeight));

					if (imgbounds.Contains(rect))
					{
						tempg.DrawImage(img, new Rectangle(Point.Empty, rect.Size), rect, GraphicsUnit.Pixel);
						Tiles[tile_x, tile_y] = new Tile(temp, new Point(image_x, image_y));
						//temp.Save("test\\" + tile_x.ToString() + " . " + tile_y.ToString() + " " + tiles[tile_x, tile_y].ForegroundHash + " " + tiles[tile_x, tile_y].BackgroundHash + ".png", ImageFormat.Png);

#if DEBUG
						cutg.DrawImage(img, image_x, image_y, rect, GraphicsUnit.Pixel);
						cutg.DrawString($"{tile_x},{tile_y}", new Font("Arial", 16), new SolidBrush(Color.Black), image_x, image_y);
						cutg.DrawString($"{Tiles[tile_x, tile_y].ForegroundHash.Substring(0, 3)}", new Font("Arial", 12), new SolidBrush(Color.Black), image_x + 40, image_y + 25);
						cutg.DrawString($"{Tiles[tile_x, tile_y].ForegroundHash.Substring(0, 3)}", new Font("Arial", 12), new SolidBrush(Color.White), image_x + 41, image_y + 26);
						cutg.DrawString($"{Tiles[tile_x, tile_y].BackgroundHash.Substring(0, 3)}", new Font("Arial", 12), new SolidBrush(Color.Black), image_x + 40, image_y + 45);
						cutg.DrawString($"{Tiles[tile_x, tile_y].BackgroundHash.Substring(0, 3)}", new Font("Arial", 12), new SolidBrush(Color.White), image_x + 41, image_y + 46);
						try
						{
							cutup.Save("cut.png", ImageFormat.Png);
						}
						catch (Exception e)
						{
							Console.WriteLine($"error {e.Message}");
						}
#endif
					}
				}
			}


		}

		public Bitmap Solve(int targetchain)
		{
			Bitmap ChainImage = new Bitmap(600, 444);

			//! Generate Chains
			List<Point> chain = new List<Point>();
			Parallel.For(0, Tiles.GetLength(0) - 1, tile_x =>
			{
				for (var tile_y = 0; tile_y < Tiles.GetLength(1); tile_y++)
				{
					var result = BuildChain(new Point(tile_x, tile_y), new List<Point>());

					chain = chain.Count < result.Count ? result : chain;
				}
			});

			//! Draw line
			var chainImageg = Graphics.FromImage(ChainImage);

			if (chain.Count > 1)
				chainImageg.DrawLines(new Pen(Color.DeepPink, 5), chain.Select(p => new PointF(Tiles[p.X, p.Y].ImagePos.X + 30, Tiles[p.X, p.Y].ImagePos.Y + 30)).ToArray());

			return ChainImage;
		}

		private List<Point> BuildChain(Point startPos, List<Point> stepped)
		{
			List<List<Point>> steppeds = new List<List<Point>>();
			stepped.Add(new Point(startPos.X, startPos.Y));
			Parallel.For(0, 5, i =>
			{
				var tempStepped = stepped.ToList();
				var neighbourPos = GetNeighbour(new Point(startPos.X, startPos.Y), i);

				if (neighbourPos.X < 0 || neighbourPos.X > 5 || neighbourPos.Y < 0 || neighbourPos.Y > 4)
					return;

				if (stepped.Exists(e => e.X == neighbourPos.X && e.Y == neighbourPos.Y))
					return;

				if (Tiles[neighbourPos.X, neighbourPos.Y] == Tiles[startPos.X, startPos.Y])
					tempStepped = BuildChain(new Point(neighbourPos.X, neighbourPos.Y), tempStepped)?.ToList();

				steppeds.Add(tempStepped);
			});
			return steppeds.OrderByDescending(s => s?.Count).FirstOrDefault();
		}

		public Point GetNeighbour(Point tile, int direction)
		{
			var parity = tile.X & 1;
			var dir = pDirections[parity][direction];
			return new Point(tile.X + dir.X, tile.Y + dir.Y);
		}

		public static bool operator ==(Grid grid1, Grid grid2)
		{
			bool result = false;
			Parallel.For(0, grid1.Tiles.GetLength(0) - 1, tile_x =>
			{
				for (var tile_y = 0; tile_y < grid1.Tiles.GetLength(1); tile_y++)
				{
					if (grid1.Tiles[tile_x, tile_y].ForegroundHash == grid2.Tiles[tile_x, tile_y].ForegroundHash
					|| grid1.Tiles[tile_x, tile_y].BackgroundHash == grid2.Tiles[tile_x, tile_y].BackgroundHash)
					{
						result = true;
					}
				}
			});
			return result;
		}

		public static bool operator !=(Grid grid1, Grid grid2)
		{
			return !(grid1 == grid2);
		}
	}
}
