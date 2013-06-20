using System;
using System.Drawing;

namespace O2.XRules.Database.APIs
{
	public partial class WinAPI
	{
		[Serializable]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
			
			public int Height
			{
				get
				{
					return this.Bottom - this.Top;
				}
			}
			public int Width
			{
				get
				{
					return this.Right - this.Left;
				}
			}
			public Size Size
			{
				get
				{
					return new Size(this.Width, this.Height);
				}
			}
			public Point Location
			{
				get
				{
					return new Point(this.Left, this.Top);
				}
			}
			public RECT(int Left, int Top, int Right, int Bottom)
			{
				this.Left = Left;
				this.Top = Top;
				this.Right = Right;
				this.Bottom = Bottom;
			}
			public Rectangle ToRectangle()
			{
				return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
			}
			public static RECT FromRectangle(Rectangle rectangle)
			{
				return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
			}
		}
	
		//was in Point.cs
		
			public struct POINT
		{
			public int x;
			public int y;
			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
			public POINT ToPoint()
			{
				return new POINT(this.x, this.y);
			}
			public static POINT FromPoint(Point pt)
			{
				return new POINT(pt.X, pt.Y);
			}
			public override bool Equals(object obj)
			{
				if (obj is POINT)
				{
					POINT pOINT = (POINT)obj;
					if (pOINT.x == this.x)
					{
						return pOINT.y == this.y;
					}
				}
				return false;
			}
			public override int GetHashCode()
			{
				return this.x ^ this.y;
			}
			public override string ToString()
			{
				return string.Format("{{X={0}, Y={1}}}", this.x, this.y);
			}
		}
	}
}