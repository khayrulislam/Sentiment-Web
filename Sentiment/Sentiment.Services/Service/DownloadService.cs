using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class DownloadService
    {

        public DownloadService()
        {

        }

        private void Initialize()
        {

        }

        private string GetFilePath()
        {
            var buildDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var path = Path.Combine(buildDir, "bin\\download.xlsx");

            var newPath = Path.Combine(buildDir, Guid.NewGuid().ToString() + ".xlsx");

            File.Copy(path, newPath);


            return newPath;
        }


        public byte[] GetRepositoryContent()
        {
            var outputPath = GetFilePath();

            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(outputPath, true))
            {
                WorkbookPart workbookPart = myDoc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);

                WorksheetPart replacementPart = workbookPart.AddNewPart<WorksheetPart>();
                string replacementPartId = workbookPart.GetIdOfPart(replacementPart);

                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                OpenXmlWriter writer = OpenXmlWriter.Create(replacementPart);


                Row r = new Row();
                Cell c = new Cell();
                CellFormula f = new CellFormula();
                f.CalculateCell = true;
                f.Text = "RAND()";
                c.Append(f);
                CellValue v = new CellValue();
                c.Append(v);

                while (reader.Read())
                {
                    if (reader.ElementType == typeof(SheetData))
                    {
                        if (reader.IsEndElement)
                            continue;
                        writer.WriteStartElement(new SheetData());

                        for (int row = 0; row < 10; row++)
                        {
                            writer.WriteStartElement(r);
                            for (int col = 0; col < 10; col++)
                            {
                                writer.WriteElement(c);
                            }
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    else
                    {
                        if (reader.IsStartElement)
                        {
                            writer.WriteStartElement(reader);
                        }
                        else if (reader.IsEndElement)
                        {
                            writer.WriteEndElement();
                        }
                    }
                }

                reader.Close();
                writer.Close();

                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>()
                .Where(s => s.Id.Value.Equals(origninalSheetId)).First();
                sheet.Id.Value = replacementPartId;
                workbookPart.DeletePart(worksheetPart);


            }
            return File.ReadAllBytes(outputPath);

        }

    }
}
