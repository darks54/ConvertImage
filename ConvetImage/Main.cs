using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public static class ConvertImage
{
    public static Bitmap ConvertTo1Bit(Bitmap input)
    {
        byte[] masks = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };
        Bitmap output = new Bitmap(input.Width, input.Height, PixelFormat.Format1bppIndexed);
        sbyte[,] data = new sbyte[input.Width, input.Height];
        BitmapData inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            IntPtr scanLine = inputData.Scan0;
            byte[] line = new byte[inputData.Stride];
            int y = 0;
            while(y< inputData.Height)
            {
                Marshal.Copy(scanLine, line, 0, line.Length);
                for(int x = 0; x < input.Width; x++)
                {
                    data[x, y] = Convert.ToSByte(64 * (GetGreyLevel(line[x * 3 + 2], line[x * 3 + 1], line[x * 3 + 0]) - 0.5));
                }
                y++;
                scanLine += inputData.Stride;
            }
        }
        finally
        {
            input.UnlockBits(inputData);
        }
        BitmapData outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
        try
        {
            IntPtr scanLine = outputData.Scan0;
            int y = 0;
            while (y < outputData.Height)
            {
                byte[] line = new byte[outputData.Stride];
                for (int x = 0; x < input.Width; x++)
                {
                    bool j = data[x, y] > 0;
                    if (j)
                    {
                        double temp = x / 8;
                        line[Convert.ToInt32(Math.Floor(temp))] |= masks[x % 8];
                    }
                    sbyte error = Convert.ToSByte(data[x, y] - (j? 32: -32));
                    if(x< input.Width - 1)
                    {
                        data[x + 1, y] += Convert.ToSByte(7 * error / 16);
                    }
                    if( y< input.Height - 1)
                    {
                        if (x > 0)
                        {
                            data[x - 1, y + 1] += Convert.ToSByte(3 * error / 16);
                        }
                        data[x, y + 1] += Convert.ToSByte(5 * error / 16);
                        if (x< input.Width - 1)
                        {
                            data[x, y + 1] += Convert.ToSByte(1 * error / 16);
                        }
                    }
                }
                Marshal.Copy(line, 0, scanLine, outputData.Stride);
                y++;
                scanLine += outputData.Stride;
            }
        }
        finally
        {
            output.UnlockBits(outputData);
        }
        return output;
    }
        
    public static double GetGreyLevel(byte r, byte g, byte b)
    {
        return (r * 0.299 + g * 0.587 + b * 0.114) / 255;
    }

}