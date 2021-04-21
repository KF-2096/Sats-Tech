



using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Variedades    
{
    class RdlcPrint
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        
        public void Run(Bill bill)
        {
            List<Bill> bills = new List<Bill>();
            bills.Add(bill);
            LocalReport report = new LocalReport();
            report.ReportPath = @"Report1.rdlc";

            report.DataSources.Add( new ReportDataSource("DataSet1", bills));
            report.Refresh();
            Export(report);
            Print();
        }
        private void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                
                PaperSize pkCustomSize1 = new PaperSize("First custom size", 350, 700);
                printDoc.DefaultPageSettings.PaperSize = pkCustomSize1;
                printDoc.DefaultPageSettings.Margins.Left = 1;
                printDoc.Print();
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            //Rectangle adjustedRect = new Rectangle(
            //    ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            //    ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            //    ev.PageSettings.PaperSize.Width,
            //    ev.PageSettings.PaperSize.Height); // ev.PageBounds.Height);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageSettings.Margins.Left = 1,
                ev.PageSettings.Margins.Top = 1,
                ev.PageSettings.PaperSize.Width,
                ev.PageSettings.PaperSize.Height); // ev.PageBounds.Height);


            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }
        private void Export(LocalReport report)
        {
            
            string deviceInfo = @"<DeviceInfo><OutputFormat>EMF</OutputFormat><PageWidth>3.5in</PageWidth><MarginTop>0.01in</MarginTop><MarginLeft>0.01in</MarginLeft><MarginRight>0.1in</MarginRight><MarginBottom>0.01in</MarginBottom></DeviceInfo>";

            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
    }
}
