using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;

namespace VAdvantage.Print
{
    public class PrintEngine
    {
        public void Print()
        {
            PrinterSettings ps = new PrinterSettings();
        }
    }
    /// <summary>
    /// Interface to print
    /// </summary>
    public interface IPrintable
    {
        // Print 
        int Print(Graphics graphics, PageFormat pageFormat, int pageIndex);
    } 
}
