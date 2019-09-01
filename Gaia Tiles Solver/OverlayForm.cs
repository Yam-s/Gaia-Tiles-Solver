using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gaia_Tiles_Solver
{
	public partial class OverlayForm : Form
	{
		public List<Point> chain;
		public OverlayForm()
		{
			InitializeComponent();
			this.CreateGraphics();
			this.TransparencyKey = Color.Black;
			this.BackColor = Color.Black;
			this.Width = 640;
			this.Height = 480;
			this.FormBorderStyle = FormBorderStyle.None;
			this.TopMost = true;
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Program.ChainImage == null)
				return;

			e.Graphics.DrawImage(Program.ChainImage, new Point(0,0));
		}
	}
}
