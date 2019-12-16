using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Sentiment.Services.Service
{
    public class DownloadService
    {
        List<string> repositorySheetList;
        BranchService branchService;
        CommitService commitService;
        IssueService issueService;
        CommentService commentService;

        public DownloadService()
        {
            Initialize();
        }

        private void Initialize()
        {
            repositorySheetList = new List<string>(Enum.GetNames(typeof(RepositorySheets))) ;
            branchService = new BranchService();
            commitService = new CommitService();
            issueService = new IssueService();
            commentService = new CommentService();
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
                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart );
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
            if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Branch)) { WriteBranchSheet(writer); }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Commit)) { WriteCommitSheet(writer); }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Issue)) { WriteIssueSheet(writer); }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Pull_Request)) { WritePullRequestSheet(writer); }
            else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Commit_Comment)) { WriteCommitCommentSheet(writer); }
            //else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Issue_Comment)) { WriteIssueCommentSheet(writer); }
            //else if (sheetName == Enum.GetName(typeof(RepositorySheets), RepositorySheets.Pull_Request_Comment)) { /*WritePullRequestCommentSheet(writer);*/ }

        }

        private void WritePullRequestSheet(OpenXmlWriter writer)
        {
            List<string> issueHeaderList = new List<string>(Enum.GetNames(typeof(IssueHeader)));
            WriteSheetHeader(writer, issueHeaderList);
            WritePullRequestData(writer);
        }

        private void WritePullRequestData(OpenXmlWriter writer)
        {
            var issueList = issueService.GetPullRequestList(1);
            issueList.ForEach((issue) => {
                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Id.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.IssueNumber.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.State } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.PosTitle.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.NegTitle.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Title } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Pos.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Neg.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Body } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.UpdateDate.ToString() } });

                writer.WriteEndElement();
            });
        }

        private void WritePullRequestCommentSheet(OpenXmlWriter writer)
        {
            List<string> issueCommentHeader = new List<string>(Enum.GetNames(typeof(IssueCommentHeader)));
            WriteSheetHeader(writer, issueCommentHeader);
            WritePullRequestCommentData(writer);
        }

        private void WritePullRequestCommentData(OpenXmlWriter writer)
        {
            // repoId
            var issueCommentList = commentService.GetPullRequestCommentList(1);
            issueCommentList.ForEach((comment) => {

                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Id.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.IssueId.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.CommentNumber.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Date.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Pos.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Neg.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Message } });

                writer.WriteEndElement();
            });
        }

        private void WriteIssueCommentSheet(OpenXmlWriter writer)
        {
            List<string> issueCommentHeader = new List<string>(Enum.GetNames(typeof(IssueCommentHeader)));
            WriteSheetHeader(writer, issueCommentHeader);
            WriteIssueCommentData(writer);
        }

        private void WriteIssueCommentData(OpenXmlWriter writer)
        {
            // repoId
            var issueCommentList = commentService.GetIssueCommentList(1);
            issueCommentList.ForEach( (comment)=> {

                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Id.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.IssueId.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.CommentNumber.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Date.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Pos.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Neg.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = Regex.Replace(comment.Message, @"[\u0000-\u0008,\u000B,\u000C,\u000E-\u001F]", "") } });

                writer.WriteEndElement();
            } );

        }

        private void WriteCommitCommentSheet(OpenXmlWriter writer)
        {
            List<string> commitCommentHeaderList = new List<string>(Enum.GetNames(typeof(CommitCommentHeader)));
            WriteSheetHeader(writer, commitCommentHeaderList);
            WriteCommitCommentData(writer);
        }

        private void WriteCommitCommentData(OpenXmlWriter writer)
        {
            var commitCommentList = commentService.GetCommitCommentList(1);

            commitCommentList.ForEach((comment)=> {
                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Id.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.CommitId.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.CommentNumber.ToString()} });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Date.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Pos.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Neg.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = comment.Message } });

                writer.WriteEndElement();
            });

        }

        private void WriteIssueSheet(OpenXmlWriter writer)
        {
            List<string> issueHeaderList = new List<string>(Enum.GetNames(typeof(IssueHeader)));
            WriteSheetHeader(writer, issueHeaderList);
            WriteIssueData(writer);
        }

        private void WriteIssueData(OpenXmlWriter writer)
        {
            var issueList = issueService.GetIssueList(1);
            issueList.ForEach((issue)=> {
                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Id.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.IssueNumber.ToString()} });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.State } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.PosTitle.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.NegTitle.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Title} });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Pos.ToString()} });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Neg.ToString()} });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.Body} });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = issue.UpdateDate.ToString()} });

                writer.WriteEndElement();
            });
        }

        private void WriteCommitSheet(OpenXmlWriter writer)
        {
            List <string> commitHeaderList = new List<string>(Enum.GetNames(typeof(CommitHeader)));
            WriteSheetHeader(writer, commitHeaderList);
            WriteCommitData(writer);
        }

        private void WriteCommitData(OpenXmlWriter writer)
        {
            // repoid input
            var commitList = commitService.GetCommitList(1);

            commitList.ForEach((commit)=> {
                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() {  DataType = CellValues.String, CellValue = new CellValue() { Text = commit.Id.ToString() } });
                writer.WriteElement(new Cell() {  DataType = CellValues.String, CellValue = new CellValue() { Text = commit.Sha } });
                writer.WriteElement(new Cell() {  DataType = CellValues.String, CellValue = new CellValue() { Text = commit.Pos.ToString() } });
                writer.WriteElement(new Cell() {  DataType = CellValues.String, CellValue = new CellValue() { Text = commit.Neg.ToString() } });
                writer.WriteElement(new Cell() {  DataType = CellValues.String, CellValue = new CellValue() { Text = commit.DateTime.ToString() } });
                writer.WriteElement(new Cell() {  DataType = CellValues.String, CellValue = new CellValue() { Text = commit.Message } });

                writer.WriteEndElement();

            });

        }

        private void WriteBranchSheet(OpenXmlWriter writer)
        {
            List <string> branchHeaderList= new List<string>(Enum.GetNames(typeof(BranchHeader)));
            WriteSheetHeader(writer, branchHeaderList);
            WriteBranchData(writer);
        }

        private void WriteBranchData(OpenXmlWriter writer)
        {
            var branchList = branchService.GetBranchList(1);
            branchList.ForEach((branch)=> {
                writer.WriteStartElement(new Row());

                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = branch.Id.ToString() } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = branch.Name } });
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = branch.Sha } });

                writer.WriteEndElement();
            });

        }

        private void WriteSheetHeader(OpenXmlWriter writer, List<string> headerList)
        {
            writer.WriteStartElement(new Row());
            headerList.ForEach( (headerItem) => {
                writer.WriteElement(new Cell() { DataType = CellValues.String, CellValue = new CellValue() { Text = headerItem } });
            });
            writer.WriteEndElement();
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
