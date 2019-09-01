using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Input;

namespace Gaia_Tiles_Solver
{
	class Program
	{
		public static HashAlgorithm Sha1 = SHA1.Create();

		public static Rectangle GameRect = new Rectangle();
		public static IntPtr GameHwnd;

		public static Bitmap ChainImage;
		public static OverlayForm Overlay;

		private static Thread inputThread = new Thread(new ThreadStart(InputThread));

		static void Main(string[] args)
		{
			inputThread.SetApartmentState(ApartmentState.STA);
			inputThread.Start();

			// Setup overlay
			var OverlayThread = new Thread(() =>
			{
				Overlay = new OverlayForm();
				Overlay.ShowDialog();
				Overlay.Refresh();
			});
			OverlayThread.Start();

			var OverlayPositionThread = new Thread(() =>
			{
				// Regularly set overlay Position
				while (true)
				{
					Thread.Sleep(10);
					GameHwnd = Natives.FindWindow(null, "Gaia Games | Gaia Online - Google Chrome");

					if (!Natives.GetWindowRect(GameHwnd, ref GameRect))
						continue;

					if (Overlay == null)
						continue;

					Overlay.Invoke(new Action(() =>
					{
						Overlay.Refresh();
						Overlay.Left = GameRect.X + 8;
						Overlay.Top = GameRect.Y + 198;
					}));
				}
			});
			OverlayPositionThread.Start();
		}

		private static void Solve()
		{
			// Capture window
			var img = new Bitmap(GameRect.Width - GameRect.X, GameRect.Height - GameRect.Y);
			var imgg = Graphics.FromImage(img);
			Natives.PrintWindow(GameHwnd, imgg.GetHdc(), 0);
			imgg.ReleaseHdc();

			// Crop game from window
			var crop = new Bitmap(600, 444);
			var cropg = Graphics.FromImage(crop);
			cropg.DrawImage(img, new Rectangle(Point.Empty, crop.Size), new Rectangle(8, 198, 600, 444), GraphicsUnit.Pixel);

			// Solve game
			var grid = new Grid(crop);
			ChainImage = grid.Solve(6);
		}

		private static void InputThread()
		{
			while (true)
			{
				if (Keyboard.IsKeyDown(Key.F1))
				{
					Solve();
					Thread.Sleep(250);
				}
			}
		}

	}
}
