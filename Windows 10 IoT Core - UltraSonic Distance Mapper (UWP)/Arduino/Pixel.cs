using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonarScope.Arduino
{
    class Pixel
    {

        public int ColourValue => this.Red << 16 | this.Green << 8 | this.Blue;

        public byte Green { get; set; }

        public byte Red { get; set; }

        public byte Blue { get; set; }

        public Pixel() : this((byte)0, (byte)0, (byte)0) { }

        public Pixel(byte r, byte g, byte b)
        {
            this.Green = g;
            this.Red = r;
            this.Blue = b;
        }

        public Pixel(int r, int g, int b)
        {
            this.Green = (byte)g;
            this.Red = (byte)r;
            this.Blue = (byte)b;
        }

        /// <summary>
        /// Creates a new pixel with given ARGB color, where A is ignored
        /// </summary>
        /// <param name="argb">ARGB color value</param>
        public Pixel(int argb)
        {
            this.Blue = (byte)argb;
            this.Green = (byte)(argb >> 8);
            this.Red = (byte)(argb >> 16);
        }
    }
}
