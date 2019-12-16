using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sentiment.DataAccess.Shared;
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
        List<string> repositorySheetList;
        public DownloadService()
        {
            Initialize();
        }

        private void Initialize()
        {
            repositorySheetList = new List<string>(Enum.GetNames(typeof(RepositorySheets))) ;
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
                CreateSheets(workbookPart, repositorySheetList);
            }
            return File.ReadAllBytes(outputPath);
        }

        private void CreateSheets(WorkbookPart workbookPart, List<string> repositorySheetList)
        {
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);
            Sheets sheets = workbookPart.Workbook.Sheets;
            UInt32Value i = Convert.ToUInt32(sheets.Count()) + 1;
            
            repositorySheetList.ForEach( (sheetName) => {
                WorksheetPart newPart = workbookPart.AddNewPart<WorksheetPart>();

                string newPartId = workbookPart.GetIdOfPart(newPart);
                
                Sheet sheet = new Sheet()
                {
                    Id = newPartId,
                    Name = sheetName,
                    SheetId = i++
                };
                sheets.Append(sheet);

                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                OpenXmlWriter writer = OpenXmlWriter.Create(newPart);

                while (reader.Read())
                {
                    if (reader.ElementType == typeof(SheetData))
                    {
                        if (reader.IsEndElement)
                            continue;
                        writer.WriteStartElement(new SheetData());

                        WriteSheets(writer, sheetName);


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
            });

            Sheet _sheet = workbookPart.Workbook.Descendants<Sheet>()
            .Where(s => s.Id.Value.Equals(origninalSheetId)).Last();
            sheets.RemoveChild(_sheet);
            workbookPart.DeletePart(worksheetPart);

        }

        private void WriteSheets(OpenXmlWriter writer, string sheetName)
        {
            if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Branch)) { }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Commit)) { }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Issue)) { }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Commit_Comment)) { }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Issue_Comment)) { }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Pull_Request)) { }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Pull_Request_Comment)) { }

        }

        private void Create(WorkbookPart workbookPart, UInt32Value id)
        {

            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            var xx = workbookPart.WorksheetParts.ToList();
            /*Sheets sheets = workbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            WorksheetPart worksheetPart1 = workbookPart.AddNewPart<WorksheetPart>();*/
            /*Worksheet workSheet1 = new Worksheet();
            SheetData sheetData1 = new SheetData();
            workSheet1.AppendChild(sheetData1);
            worksheetPart.Worksheet = workSheet1;
            Sheet sheet1 = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                Name = "Sheet"+id
            };
            sheets.Append(sheet1);*/
            /*Sheet sheet1 = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart1),
                Name = "Sheet" + id
            };
            sheets.Append(sheet1);*/

            

            string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);

            WorksheetPart replacementPart = workbookPart.AddNewPart<WorksheetPart>();
            string replacementPartId = workbookPart.GetIdOfPart(replacementPart);

            Sheets sheets = workbookPart.Workbook.Sheets;
                       
            Sheet sheet1 = new Sheet()
            {
                Id = replacementPartId,
                Name = "Sheet" + id,
                SheetId = id
            };
            sheets.Append(sheet1);

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

            /*Sheet sheet = workbookPart.Workbook.Descendants<Sheet>()
            .Where(s => s.Id.Value.Equals(origninalSheetId)).First();
            sheet.Id.Value = replacementPartId;
            workbookPart.DeletePart(worksheetPart);*/
        }
    }
}
