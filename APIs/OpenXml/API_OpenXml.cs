// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using DocumentFormat.OpenXml; 
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using O2.XRules.Database.Utils;

//O2Ref:DocumentFormat.OpenXml.dll
//O2Ref:WindowsBase.dll

namespace O2.XRules.Database.APIs
{
    public static class API_OpenXml
    {        
    
    	public static OpenXml_SpreadSheet show_SpreadSheetGUI()
    	{
    		var panel = O2Gui.open<Panel>("OpenXml viewer", 1000,500);
    		var spreadSheet = new OpenXml_SpreadSheet();
			spreadSheet.showInControl(panel); 
			return spreadSheet;
    	}
    	public static OpenXml_SpreadSheet show_SpreadSheet(this string fileToLoad)
    	{
    		var panel = O2Gui.open<Panel>("OpenXml viewer: {0}".format(fileToLoad.fileName()), 1000,500);
    		var spreadSheet = fileToLoad.open_SpreadSheet(); 	
			spreadSheet.showInControl(panel); 
			return spreadSheet;
    	}
    	
    	public static OpenXml_SpreadSheet open_SpreadSheet(this string pathToFileToLoad)
    	{
    		return open_SpreadSheet(pathToFileToLoad, false);
    	}
    	
    	public static OpenXml_SpreadSheet open_SpreadSheet(this string pathToFileToLoad, bool isEditable)
    	{
    		try
    		{
	    		var spreadSheet = new OpenXml_SpreadSheet();
	    		spreadSheet.IsEditable = isEditable;
	    		spreadSheet.FilePath = pathToFileToLoad;
	    		spreadSheet.SpreadSheetDocument = SpreadsheetDocument.Open(pathToFileToLoad,isEditable); 
	    		// this needs to be more robust to handle the cases where there is no SharedStringTablePart
				spreadSheet.StringTable= (SharedStringTable)spreadSheet.SpreadSheetDocument
																	   .WorkbookPart
																	   .GetPartsOfType<SharedStringTablePart>()
																	   .First()
																	   .SharedStringTable;
																	   //.toList()[0];
			    
				foreach(var sheet in  spreadSheet.sheets_Raw())			
					spreadSheet.add_Sheet(sheet);									
																	   
				return 	spreadSheet;
			}
			catch (Exception ex)
			{
				ex.log("in open_SpreadSheet");
				return null;
			}
    	}

    }
    
/*
//tableList();
var dataToShow = fileData["Weakness"];//["Implementations"];
	
*/
    
    public class OpenXml_SpreadSheet
    {
    	public SpreadsheetDocument SpreadSheetDocument { get; set; }
    	public SharedStringTable StringTable { get; set; }
    	public string FilePath { get; set; }
    	public bool IsEditable {get; set;}
    	
    	public List<OpenXml_SpreadSheet_WorkSheet> WorkSheets { get; set; }
    	
    	public OpenXml_SpreadSheet()
    	{
			WorkSheets = new List<OpenXml_SpreadSheet_WorkSheet>();
    	}    	    	    	
    }
    
    public class OpenXml_SpreadSheet_WorkSheet
    {    	
    	public OpenXml_SpreadSheet OpenXml_SpreadSheet { get; set; }
    	public string ID {get;set;}
    	public string Name {get;set;}
    	public Sheet Sheet {get;set;}
    	public WorksheetPart WorksheetPart { get; set; }
    	public List<OpenXml_SpreadSheet_Row> Rows { get; set; }    	
    	
    	public OpenXml_SpreadSheet_WorkSheet(OpenXml_SpreadSheet openXml_SpreadSheet)
    	{
    		OpenXml_SpreadSheet = openXml_SpreadSheet;
    		Rows = new List<OpenXml_SpreadSheet_Row>();
    	}
    	
    	public override String ToString()
    	{
    		return this.Name;
    	}
    	
    }
    
    public class OpenXml_SpreadSheet_Row
    {
    	public List<String> Cells {get;set;}
    	
    	public OpenXml_SpreadSheet_Row()
    	{
    		Cells = new List<string>();
    	}
    	
    	public OpenXml_SpreadSheet_Row(List<string> cells)
    	{
    		Cells = cells;
    	}
    }
    
    public static class API_OpenXml_ExtensionMethods
    {
    	
    }
    
    public static class API_OpenXml_ExtensionMethods_SpreadSheet
    {
    	public static List<OpenXml_SpreadSheet_WorkSheet> workSheets(this OpenXml_SpreadSheet spreadSheet)
    	{
    		return spreadSheet.WorkSheets;
    	}
    	
    	public static OpenXml_SpreadSheet_WorkSheet workSheet(this OpenXml_SpreadSheet spreadSheet, string name)
    	{
    		foreach(var worksheet in spreadSheet.workSheets())
    			if (worksheet.Name == name)
    				return worksheet;
    		return null; 
    	}
    	
    
    	public static List<Sheet> sheets_Raw(this OpenXml_SpreadSheet spreadSheet)
    	{
    		return (from Sheet sheet in spreadSheet.SpreadSheetDocument
					    						 .WorkbookPart
					    						 .Workbook
					    					     .GetFirstChild<Sheets>()
					select sheet).toList();
    	}
    	
    	public static List<SharedStringItem> sharedStrings(this OpenXml_SpreadSheet spreadSheet)
    	{
    		return (from SharedStringItem item in spreadSheet.StringTable.toList()
    			    select item).toList();
    	}
    	public static OpenXml_SpreadSheet add_Sheet(this OpenXml_SpreadSheet spreadSheet, Sheet sheet)
    	{
    		var workSheet = new OpenXml_SpreadSheet_WorkSheet(spreadSheet);
			workSheet.Sheet = sheet;
			workSheet.ID = 	sheet.Id.Value;
			workSheet.Name = 	sheet.Name.Value;
			workSheet.WorksheetPart = (WorksheetPart)spreadSheet.SpreadSheetDocument.WorkbookPart.GetPartById(workSheet.ID); 						
			var sharedStrings = workSheet.OpenXml_SpreadSheet.sharedStrings();
			foreach(var row in workSheet.rows_Raw())
				workSheet.Rows.Add(new OpenXml_SpreadSheet_Row(row.rowData(sharedStrings)));
			spreadSheet.WorkSheets.Add(workSheet);
			
			return spreadSheet;
		}
		
		
		public static List<Row> rows_Raw(this OpenXml_SpreadSheet_WorkSheet worksheet)
		{
			var id = worksheet.ID;
			return (from Row row in worksheet.WorksheetPart.Worksheet.GetFirstChild<SheetData>()
			        select row).toList();
		}
		
		public static List<string> rowData(this Row row, List<SharedStringItem> sharedStrings)
		{
			var rowData = new List<string>();		
			foreach(Cell cell in row.Elements())
			{	
				var cellValue = "";
				if (cell.CellValue.notNull())
				{			
					if (cell.DataType.isNull())
						cellValue = cell.CellValue.Text;		
					else switch (cell.DataType.str())   
					{
						case "s": //CellValues.SharedString:				 
							 var index = cell.CellValue.Text.toInt();
							 var item = sharedStrings[index];
							 cellValue = item.Text.Text.str();
							 break;					 
						case "str": //CellValues.String: 				
						case null:  
							cellValue = cell.CellValue.Text;					
							break;
						default:	
							cellValue = "ERROR: [Unsupported DataType] {0} : {1}".format(cell.DataType, cell.CellValue.Text);
							break;
					}					
				}					
				rowData.add(cellValue);
			}
			return rowData;
		}
		
		public static List<OpenXml_SpreadSheet_Row> rows(this OpenXml_SpreadSheet_WorkSheet workSheet)
		{
			return workSheet.Rows;
		}
		
		public static List<List<string>> rowsData(this OpenXml_SpreadSheet_WorkSheet workSheet)
		{
			var rowsData = new List<List<string>>();
			var rows = workSheet.rows();			
			foreach(var row in rows)
			{
				var cells = new List<string>();		// create clone so that we don't affect the source 
				cells.AddRange(row.Cells);				
				rowsData.Add(cells);
			}
			return rowsData;
		}
		
		
		
		
					//workSheet.Name = sheet.Text;
					//fileData.Add(sheet.Id.Value, new List<List<String>>());  
    }
    
    public static class GUI
    {
        public static System.Windows.Forms.Control showInForm(this OpenXml_SpreadSheet spreadSheet)        	
        {
        	var panel = O2Gui.open<Panel>("OpenXml viewer", 1000,500);
        	return spreadSheet.showInControl(panel);
        	
        }
    	public static T showInControl<T>(this OpenXml_SpreadSheet spreadSheet, T control)
    		where T : System.Windows.Forms.Control
    		{	
				control.clear();   
				var topPanel = control.add_Panel(); 
				var tableList = topPanel.add_TableList(); 
				var treeView = tableList.insert_Left<Panel>(200).add_TreeView();
				treeView.afterSelect<OpenXml_SpreadSheet_WorkSheet>(
					(workSheet)=>{  
									try
									{
										tableList.clearTable();								
										var rowsData = workSheet.rowsData();
										tableList.add_Columns(rowsData[0]);
										rowsData.RemoveAt(0);
										foreach(var row in rowsData)
											tableList.add_Row(row);
									}
									catch(Exception ex)
									{
										ex.log("in OpenXml_SpreadSheet_WorkSheet treeView.afterSelect");
									}
		
									//tableList.show(workSheet.rows()); 
								 });
				
				//foreach(var workSheet in )
				Action<OpenXml_SpreadSheet> loadSpreadSheetData =
					(_spreadSheet)=>{										
										treeView.clear();
										tableList.clearTable();										
										if (_spreadSheet.notNull())
										{											
											treeView.add_Nodes(_spreadSheet.WorkSheets);  				
											treeView.selectFirst(); 
										}
									};
				
				Action<string> loadSpreadSheetFile =
					(fileToLoad)=>{
									"Loading File: {0}".info(fileToLoad);
									loadSpreadSheetData(fileToLoad.open_SpreadSheet());
							};
				treeView.onDrop(loadSpreadSheetFile);					
				tableList.getListViewControl().onDrop(loadSpreadSheetFile);
				//load data			
				loadSpreadSheetData(spreadSheet);
				return control;
			}
    }    
    
    
    //from http://msdn.microsoft.com/en-us/library/cc861607.aspx
    
    public class HelperMethods     
    {    	
    	public static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
		{
		    // If the part does not contain a SharedStringTable, create one.
		    if (shareStringPart.SharedStringTable == null)
		    {
		        shareStringPart.SharedStringTable = new SharedStringTable();
		    }
		
		    int i = 0;
		
		    // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
		    foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
		    {
		        if (item.InnerText == text)
		        {
		            return i;
		        }
		
		        i++;
		    }
		
		    // The text does not exist in the part. Create the SharedStringItem and return its index.
		    shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
		    shareStringPart.SharedStringTable.Save();
		
		    return i;
		}
		
		public static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
		{
		    // Add a new worksheet part to the workbook.
		    WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
		    newWorksheetPart.Worksheet = new Worksheet(new SheetData());
		    newWorksheetPart.Worksheet.Save();
		
		    Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
		    string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);
		
		    // Get a unique ID for the new sheet.
		    uint sheetId = 1;
		    if (sheets.Elements<Sheet>().Count() > 0)
		    {
		        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
		    }
		
		    string sheetName = "Sheet" + sheetId;
		
		    // Append the new worksheet and associate it with the workbook.
		    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
		    sheets.Append(sheet);
		    workbookPart.Workbook.Save();
		
		    return newWorksheetPart;
		}

		// Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
		// If the cell already exists, returns it. 
		public static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
		{
		    Worksheet worksheet = worksheetPart.Worksheet;
		    SheetData sheetData = worksheet.GetFirstChild<SheetData>();
		    string cellReference = columnName + rowIndex;
		
		    // If the worksheet does not contain a row with the specified row index, insert one.
		    Row row;
		    if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
		    {
		        row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
		    }
		    else
		    {
		        row = new Row() { RowIndex = rowIndex };
		        sheetData.Append(row);
		    }
		
		    // If there is not a cell with the specified column name, insert one.  
		    if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
		    {
		        return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
		    }
		    else
		    {
		        // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
		        Cell refCell = null;
		        foreach (Cell cell in row.Elements<Cell>())
		        {
		            if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
		            {
		                refCell = cell;
		                break;
		            }
		        }
		
		        Cell newCell = new Cell() { CellReference = cellReference };
		        row.InsertBefore(newCell, refCell);
		
		        worksheet.Save();
		        return newCell;
		    }
		}					
	}
	
	public static class HelperMethods_ExtensionMethods
	{
		public static Tuple<SpreadsheetDocument, WorkbookPart, WorksheetPart> newSpreadSheet(this string file, string sheetName)
		{
			if (file.fileExists())
				Files.deleteFile(file);
			// Create a spreadsheet document by supplying the filepath.
		    // By default, AutoSave = true, Editable = true, and Type = xlsx.
		    SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(file, SpreadsheetDocumentType.Workbook);
		
		    // Add a WorkbookPart to the document.
		    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
		    workbookpart.Workbook = new Workbook();
		
		    // Add a WorksheetPart to the WorkbookPart.
		    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
		    worksheetPart.Worksheet = new Worksheet(new SheetData());
		
		    // Add Sheets to the Workbook.
		    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
		
		    // Append a new worksheet and associate it with the workbook.
		    Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
		    sheets.Append(sheet);
		    
			return new Tuple<SpreadsheetDocument, WorkbookPart, WorksheetPart>(spreadsheetDocument, workbookpart, worksheetPart);			
		}
		
		public static Tuple<SpreadsheetDocument, WorkbookPart,WorksheetPart> save(this Tuple<SpreadsheetDocument, WorkbookPart, WorksheetPart>  tuple)
		{
			var spreadsheetDocument = tuple.Item1;
			var workbookpart = tuple.Item2;
			var worksheetPart = tuple.Item3; // not used in save
			workbookpart.Workbook.Save();    
    		spreadsheetDocument.Close();
			return tuple;
		}
		
		public static Tuple<SpreadsheetDocument, WorkbookPart,WorksheetPart> writeText(this Tuple<SpreadsheetDocument, WorkbookPart,WorksheetPart>  tuple, int cell_Row, params string[] texts)
		{
			if (texts.size() > 25)
			{
				"in writeText, the maximum number of columns is 25, aborting row edit".error();
				return tuple;
			}
			var letter = 65;			
			foreach(var text in texts)
			{ 
				var cell_column = ((byte)(letter++)).ascii();
				tuple.writeText(cell_column, cell_Row,  text);
			}
			return tuple;
		}
		
		public static Tuple<SpreadsheetDocument, WorkbookPart,WorksheetPart> writeText(this Tuple<SpreadsheetDocument, WorkbookPart,WorksheetPart>  tuple, string cell_Column, int cell_Row, string text)
		{
			var spreadsheetDocument = tuple.Item1;
			var workbookpart = tuple.Item2;// not used in writeText
			var worksheetPart = tuple.Item3;
			spreadsheetDocument.writeText(worksheetPart, cell_Column, cell_Row, text);
			return tuple;
		}
		public static SpreadsheetDocument writeText(this SpreadsheetDocument spreadsheetDocument , WorksheetPart worksheetPart, string cell_Column, int cell_Row, string text)
		{
			SharedStringTablePart shareStringPart; 
	        if (spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
	        {
	            shareStringPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
	        }
	        else 
	        {
	            shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
	        }
	
	        // Insert the text into the SharedStringTablePart.
	        int index = HelperMethods.InsertSharedStringItem(text, shareStringPart);
	        
	        Cell cell = HelperMethods.InsertCellInWorksheet(cell_Column, (uint)cell_Row, worksheetPart);
	        
	        cell.CellValue = new CellValue(index.ToString());
	        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
	        return spreadsheetDocument;
		}
	}
}
