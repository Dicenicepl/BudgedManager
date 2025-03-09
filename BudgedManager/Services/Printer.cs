using System.Drawing;
using System.Drawing.Printing;

namespace BudgedManager.Services;

public class Printer
{
    private Font printFont;
    private StreamReader streamToPrint;
    static string filePath;


    public void Print()
    {
        Printing();
    }
    private void pd_PrintPage(object sender, PrintPageEventArgs ev) 
    {
        
        float linesPerPage = 0;
        float yPos =  0;
        int count = 0;
        float leftMargin = ev.MarginBounds.Left;
        float topMargin = ev.MarginBounds.Top;
        String line=null;
             
        // Calculate the number of lines per page.
        linesPerPage = ev.MarginBounds.Height  / 
                       printFont.GetHeight(ev.Graphics) ;
 
        // Iterate over the file, printing each line.
        while (count < linesPerPage && 
               ((line=streamToPrint.ReadLine()) != null)) 
        {
            yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
            ev.Graphics.DrawString (line, printFont, Brushes.Black, 
                leftMargin, yPos, new StringFormat());
            count++;
        }
 
        // If more lines exist, print another page.
        if (line != null) 
            ev.HasMorePages = true;
        else 
            ev.HasMorePages = false;
    }
    public void Printing()
    {
        try 
        {
            streamToPrint = new StreamReader ("URL");
            try 
            {
                printFont = new Font("Arial", 10);
                PrintDocument pd = new PrintDocument(); 
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                // Print the document.
                pd.Print();
            } 
            finally 
            {
                streamToPrint.Close() ;
            }
        } 
        catch(Exception ex) 
        { 
            Console.WriteLine(ex.Message);
        }
    }

}