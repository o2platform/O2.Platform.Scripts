// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//this is based on the script API_VistaDB.cs (these two should be merged and a common API_DB should be created)
//O2Tag:SkipGlobalCompilation
using System;
using System.Data;
using System.Xml.Serialization;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.Utils;


using VistaDB.Provider;
//O2Ref:C:\Program Files\Checkmarx\Checkmarx Engine Server\VistaDB.NET20.dll

namespace O2.XRules.Database.APIs
{
	public class API_VistaDB_Test
	{
		public void launchTestGui()
		{
			"Util - VistaDB Browser.h2".local().executeH2Script();
		}
	}

    public class API_VistaDB
    {   
    	public string ConnectionString { get;set; }
    	public string LastError { get; set; }
    	
    	public API_VistaDB()
		{
			ConnectionString = @"data source='C:\Program Files\Checkmarx\Checkmarx Application Server\CxDB.vdb3'";	 //default to this one			
		}		
		
		public API_VistaDB(string connectionString)
		{
			ConnectionString = connectionString;
		}
	}
	
	public class Database
	{
		public API_VistaDB VistaDB { get; set; }
		public string Name { get; set; }		
		public List<Table> Tables { get; set; }
		public List<StoredProcedure> StoredProcedures { get; set; }
		
		public Database(string name)
		{
			Name = name.trim();
			Tables = new List<Table>();
			StoredProcedures = new List<StoredProcedure>();
		}
		
		public Database(API_VistaDB vistaDB, string name) : this (name)
		{
			VistaDB = vistaDB;
		
		}
	}
	
	public class Table
	{
		[XmlIgnore] public API_VistaDB VistaDB { get; set; }				
//		public string Catalog {get;set;}
//		public string Schema {get;set;}
		public string Name {get;set;}
//		public string Type {get;set;}
		public List<Table_Column> Columns {get;set;}
		
		public DataTable TableData { get; set; }
		
		public Table()
		{
			Columns = new List<Table_Column>();
		}
		
		public override string ToString()
		{
			/*return (Schema.valid()) 
						? "{0}.{1}".format(Schema, Name)
						: Name;*/
			return Name;			
		}
	}
	
	public class Table_Column
	{
		//name, typeId, objectId,options , scriptValue
		public string Name { get; set; }		
		public string TypeId { get; set; }		
		public string ObjectId { get; set; }		
		public string Options { get; set; }		
		public string ScriptValue	 { get; set; }		
		
		public override string ToString()
		{
			//return "{0} ({1})".format(Name, DataType);
			return "{0} ({1})".format(Name);
		}
	}
	
	public class StoredProcedure
	{
		public string Schema {get;set;}
		public string Name {get;set;}
		public string Value {get;set;}
		
		public StoredProcedure(string schema, string name, string value)
		{
			Schema = schema;
			Name = name;
			Value = value;
		}
		
		public StoredProcedure(string name, string value) : this("",name, value) 
		{
			
		}
		
		public override string ToString()
		{
			return (Schema.valid()) 
						? "{0}.{1}".format(Schema, Name)
						: Name;
		}
	}
	
	//add these queries should be done using Linq
	public static class API_VistaDB_Helps
	{
		public static Database database(this API_VistaDB vistaDB, string name)
		{
			return new Database(vistaDB, name);
		}
	}
	public static class API_VistaDB_getData
	{
		public static List<string> database_Names(this API_VistaDB vistaDB)
		{
			var sqlQuery = "select * from [database schema] where typeid = 1";
			return (from DataRow row in vistaDB.executeReader(sqlQuery).Rows
					select row["name"].str()).toList();
		}
		
		public static List<string> column_Names(this Table table)
		{
			return (from column in table.columns()
				   	select column.Name).toList();
		}
		
		public static List<Table> tables(this API_VistaDB vistaDb)
		{			
			return vistaDb.database("").tables();
		}
		
		public static List<Table> tables(this Database database)
		{			
			if (database.Tables.size() ==0)
				database.map_Tables();
			return database.Tables;
		}
		
		public static Table table(this API_VistaDB vistaDb, string name)
		{
			return vistaDb.database("").table(name);
		}
		
		public static Table table(this Database database, string name)
		{			
			return (from table in database.tables()
					where table.Name.trim() == name
					select table).first();
		}
		
		
		public static List<Table_Column> columns(this Table table)
		{
			return table.Columns;
		}
	}
	
	public static class API_VistaDB_PopulateData
	{
		public static Database map_StoredProcedures(this Database database)
		{		
			var sqlQuery = "select Specific_Schema, Specific_Name, Routine_Definition  from {0}.Information_Schema.Routines".format(database.Name);
			var storedProceduresData = database.VistaDB.executeReader(sqlQuery);			
			foreach(DataRow row in storedProceduresData.Rows)
				database.StoredProcedures.Add(new StoredProcedure(row.ItemArray[0].str(),row.ItemArray[1].str(),row.ItemArray[2].str())); 
			return database;
		}	
		
		public static Database map_Tables(this API_VistaDB vistaDB)
		{
			return vistaDB.database("").map_Tables();
		}
		
		public static Database map_Tables(this Database database)
		{		
			var sqlQuery = "select * from [database schema] where typeid = 1".format();
			var tables = database.VistaDB.executeReader(sqlQuery);			
			foreach(DataRow row in tables.Rows)
				database.Tables.Add(new Table(){
													VistaDB = database.VistaDB,
//													Catalog = row.ItemArray[0].str(),
//													Schema = row.ItemArray[1].str(),
													Name = row["name"].str().trim()
//													Type = row.ItemArray[3].str()
													});
			return database;
		}	
		
		public static Database map_Table_Columns(this Database database)
		{		
			foreach(var table in database.tables())
			{			
				//var sqlQuery = "select Column_Name, Column_Default, Is_Nullable, Data_Type, Character_Maximum_Length from {0}.Information_Schema.Columns where table_Schema='{1}' and table_name='{2}'"
				//					.format(table.Catalog, table.Schema,table.Name);
								
				var objectId = database.VistaDB.executeScalar("select objectId from [database schema] where typeid = 1 and name ='{0}' ".format(table.Name));
				var sqlQuery = "select name, typeId, objectId,options , scriptValue from [database schema] where foreignReference = '{0}' ".format(objectId);
	 			
				var columns = database.VistaDB.executeReader(sqlQuery);			
				
				foreach(DataRow row in columns.Rows)
					table.Columns.Add(new Table_Column(){
														Name =  row.ItemArray[0].str().trim(),
														TypeId = row.ItemArray[1].str().trim(),
														ObjectId = row.ItemArray[2].str().trim(),
														Options = row.ItemArray[3].str().trim(),
														ScriptValue = row.ItemArray[4].str().trim()
														});
			}
			return database;
		}
		
		public static API_VistaDB map_Table_Data(this API_VistaDB vistaDB, Table table)
		{
//			var sqlQuery = "select * from [{0}].[{1}].[{2}]".format(table.Catalog,table.Schema, table.Name);				
			var sqlQuery = "select * from {0}".format(table.Name);							
			table.TableData = vistaDB.executeReader(sqlQuery);
			return vistaDB;
		}
		
		public static Database map_Table_Data(this Database database, Table table)
		{
			database.VistaDB.map_Table_Data(table);
			return database;
		}
		public static Database map_Table_Data(this Database database)
		{
			"Mapping table data".info();
			var timer = new O2Timer("Mapped tabled data").start();
			foreach(var table in database.tables())					
				database.map_Table_Data(table);
			timer.stop();
			return database;				
		}
		
		public static DataTable dataTable(this Table table)
		{
			if (table.isNull())
				return null;
			table.VistaDB.map_Table_Data(table);
			return table.TableData;
		}		
		
		public static string xml(this Table table)
		{			
			var dataSet = new DataSet();
			dataSet.Tables.Add(table.dataTable());
			return dataSet.GetXml();
		}
	}
	
	public static class API_VistaDB_Queries
	{
		public static VistaDBConnection getOpenConnection(this API_VistaDB vistaDB)
		{						
			"[API_VistaDB] Opening Connection".info();
			try
			{
				var sqlConnection = new VistaDBConnection(vistaDB.ConnectionString);			
				sqlConnection.Open();
				return sqlConnection;
			}
			catch(Exception ex)
			{
				vistaDB.LastError = ex.Message;
				"[executeNonQuery] {0}".error(ex.Message);
				//ex.log();
			}			
			return null;
		}
		
		public static VistaDBConnection closeConnection(this API_VistaDB vistaDB, VistaDBConnection sqlConnection)
		{						
			"[API_VistaDB] Closing Connection".info();
			try
			{				
				sqlConnection.Close();
				return sqlConnection;
			}
			catch(Exception ex)
			{
				vistaDB.LastError = ex.Message;
				"[executeNonQuery] {0}".error(ex.Message);
				//ex.log();
			}			
			return null;
		}
		
		public static API_VistaDB executeNonQuery(this API_VistaDB vistaDB, VistaDBConnection sqlConnection, string command)
		{			
			"[API_VistaDB] Executing Non Query: {0}".info(command);
			try
			{
				var sqlCommand = new VistaDBCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = command;
				sqlCommand.CommandType = CommandType.Text;
				sqlCommand.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				vistaDB.LastError = ex.Message;
				"[executeNonQuery] {0}".error(ex.Message);
				//ex.log();
			}
			return vistaDB;
		}
		
		public static API_VistaDB executeNonQuery(this API_VistaDB vistaDB, string command)
		{		
			"[API_VistaDB] Executing Non Query: {0}".info(command);
			VistaDBConnection sqlConnection = null;
			try
			{
				sqlConnection = new VistaDBConnection(vistaDB.ConnectionString);			
				sqlConnection.Open();
				var sqlCommand = new VistaDBCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = command;
				sqlCommand.CommandType = CommandType.Text;
				sqlCommand.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				vistaDB.LastError = ex.Message;
				"[executeNonQuery] {0}".error(ex.Message);
				//ex.log();
			}
			finally
			{
				if (sqlConnection.notNull())
					sqlConnection.Close();
			}
			return vistaDB;
		}
		
		public static object executeScalar(this API_VistaDB vistaDB, string command)
		{	
			"[API_VistaDB] Executing Scalar: {0}".info(command);
			VistaDBConnection sqlConnection = null;
			try
			{
				sqlConnection = new VistaDBConnection(vistaDB.ConnectionString);
				sqlConnection.Open();
				var sqlCommand = new VistaDBCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = command;
				sqlCommand.CommandType = CommandType.Text;
				return sqlCommand.ExecuteScalar();
			}
			catch(Exception ex)
			{
				vistaDB.LastError = ex.Message;
				"[executeNonQuery] {0}".error(ex.Message);
				//ex.log();
			}
			finally
			{
				sqlConnection.Close();
			}
			return null;
		}
		
		public static DataTable executeReader(this API_VistaDB vistaDB, string command)
		{
			var sqlConnection = new VistaDBConnection(vistaDB.ConnectionString);
			sqlConnection.Open();
			try
			{
				var sqlCommand = new VistaDBCommand();
				sqlCommand.Connection = sqlConnection;
				sqlCommand.CommandText = command;
				sqlCommand.CommandType = CommandType.Text;
				var reader =  sqlCommand.ExecuteReader();
				var dataTable = new DataTable();
				dataTable.Load(reader);
				return dataTable;
			}
			catch(Exception ex)
			{
				vistaDB.LastError = ex.Message;
				"[executeNonQuery] {0}".error(ex.Message);
				//ex.log();
			}
			finally
			{
				if (sqlConnection.notNull())
					sqlConnection.Close();
			}
			return null;
		}
	}
		
	public static class API_VistaDB_GUI_Controls
    {
		public static T add_ConnectionStringTester<T>(this API_VistaDB vistaDB , T control, Action afterConnect)
			where T : Control
		{
			control.clear();
			var connectionString = control.add_GroupBox("Connection String").add_TextArea();
			var connectionStringSamples = connectionString.parent().insert_Left<Panel>(200).add_GroupBox("Sample Connection Strings")
														  .add_TreeView()
														  .afterSelect<string>((text)=> connectionString.set_Text(text));
			var connectPanel = connectionString.insert_Below<Panel>(200);
			var button = connectPanel.insert_Above<Panel>(25).add_Button("Connect").fill();  
			var response = connectPanel.add_GroupBox("Response").add_TextArea();						
			
			button.onClick(()=>{
									try
									{ 
										var text = connectionString.get_Text();
										vistaDB.ConnectionString = text;
										response.set_Text("Connecting using: {0}".format(text));
										var sqlConnection = new VistaDBConnection(text);
										sqlConnection.Open();
										response.set_Text("Connected ok");
										afterConnect();
									}
									catch(Exception ex)
									{
										vistaDB.LastError = ex.Message;
										response.set_Text("Error: {0}".format(ex.Message));
									}						
									
								});
			
			//connectionString.set_Text(@"Data Source=.\SQLExpress;Trusted_Connection=True"); 
			var sampleConnectionStrings = new List<string>();
			//from http://www.connectionstrings.com/sql-server-2005
			sampleConnectionStrings.add(@"data source='C:\Program Files\Checkmarx\Checkmarx Application Server\CxDB.vdb3'")
								   .add(@"Data Source=.\SQLExpress;Trusted_Connection=True")
								   .add(@"Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=SSPI")												   
								   .add(@"Data Source=myServerAddress;Initial Catalog=myDataBase;User Id=myUsername;Password=myPassword;")
								   .add(@"Data Source=190.190.200.100,1433;Network Library=DBMSSOCN;Initial Catalog=myDataBase;User ID=myUsername;Password=myPassword;")
								   .add(@"Server=.\SQLExpress;AttachDbFilename=c:\mydbfile.mdf;Database=dbname; Trusted_Connection=Yes;")
								   .add(@"Server=.\SQLExpress;AttachDbFilename=|DataDirectory|mydbfile.mdf; Database=dbname;Trusted_Connection=Yes;")
								   .add(@"Data Source=.\SQLExpress;Integrated Security=true; AttachDbFilename=|DataDirectory|\mydb.mdf;User Instance=true;");
								   
			connectionStringSamples.add_Nodes(sampleConnectionStrings).selectFirst();  			
			
			button.click();
			return control;
			
		}
				
		
		public static API_VistaDB add_Viewer_QueryResult<T>(this API_VistaDB vistaDB , T control, string sqlQuery)
			where T : Control
		{	 
			control.clear();
			var dataTable = vistaDB.executeReader(sqlQuery); 			
			var dataGridView = control.add_DataGridView();
			dataGridView.DataError+= (sender,e) => { // " dataGridView error: {0}".error(e.Context);
												   };
			dataGridView.invokeOnThread(()=> dataGridView.DataSource = dataTable );			
			return vistaDB;
		}
		
		public static API_VistaDB add_Viewer_DataBases<T>(this API_VistaDB vistaDB , T control)
			where T : Control
		{
			var sqlQuery = "select * from [database schema] where typeid = 1"; 
			return vistaDB.add_Viewer_QueryResult(control, sqlQuery);
		}
		
		public static API_VistaDB add_Viewer_Tables_Raw<T>(this API_VistaDB vistaDB , T control, string databaseName)
			where T : Control
		{
			var objectId = vistaDB.executeScalar("select objectId from [database schema] where typeid = 1 and name ='{0}'".format(databaseName));
			
			var sqlQuery = "select * from [database schema] where typeid = 3 and foreignReference ='{0}'".format(objectId); 
			
			return vistaDB.add_Viewer_QueryResult(control, sqlQuery);
		}
		
		public static API_VistaDB add_Viewer_StoredProcedures_Raw<T>(this API_VistaDB vistaDB , T control, string databaseName)
			where T : Control
		{
		
			var sqlQuery = "select * from {0}.Information_Schema.Routines".format(databaseName); 
			return vistaDB.add_Viewer_QueryResult(control, sqlQuery);
		}
		
		public static API_VistaDB add_Viewer_StoredProcedures<T>(this API_VistaDB vistaDB , T control)
			where T : Control
		{
			control.clear();
			Database currentDatabase = null;
			var value = control.add_TextArea();	
			var storedProcedure_Names = value.insert_Left<Panel>(200).add_TreeView().sort();
			var database_Names = storedProcedure_Names.insert_Above<Panel>(100).add_TreeView().sort();
			
			var filter = storedProcedure_Names.insert_Above(20)
											  .add_TextBox("Filter:","")
										  	  .onTextChange((text)=>{ 
										  	  							storedProcedure_Names.clear();
										  	  							var result = (from storedProcedure in currentDatabase.StoredProcedures
										  	  										  where storedProcedure.Name.regEx(text)
										  	  										  select storedProcedure);
										  	  							storedProcedure_Names.add_Nodes(result);
										  	  						});
			
			database_Names.afterSelect<string>(
				(database_Name)=>{
									value.set_Text("");
									currentDatabase = new Database(vistaDB, database_Name);
									currentDatabase.map_StoredProcedures();									
									storedProcedure_Names.clear();						
									storedProcedure_Names.add_Nodes(currentDatabase.StoredProcedures);
									storedProcedure_Names.selectFirst(); 
								 });
			
			storedProcedure_Names.afterSelect<StoredProcedure>(
				(storedProcedure) => value.set_Text(storedProcedure.Value) );
			
			database_Names.add_Nodes(vistaDB.database_Names());
			
			database_Names.selectFirst();
			return vistaDB;
		}
		
		
		public static API_VistaDB add_Viewer_Tables<T>(this API_VistaDB vistaDB , T control)
			where T : Control
		{		
			control.clear();
			var value = control.add_TableList();	
			var tables_Names = value.insert_Left<Panel>(200).add_TreeView().sort();		 	
//			var database_Names = tables_Names.insert_Above<Panel>(100).add_TreeView().sort();


/*			database_Names.afterSelect<string>(
				(database_Name)=>{
									tables_Names.backColor(Color.Salmon);
									O2Thread.mtaThread(
										()=>{
												value.set_Text("");
												var database = new Database(vistaDB, database_Name);									
												database.map_Tables()
														.map_Table_Columns();
												tables_Names.clear();						
												tables_Names.add_Nodes(database.Tables);
												tables_Names.selectFirst(); 
												tables_Names.backColor(Color.White);
											});	 
								 });
*/			
			tables_Names.afterSelect<Table>( 
				(table) => value.show(table.Columns) );
			
//			database_Names.add_Nodes(vistaDB.database_Names());
			
//			database_Names.selectFirst();

			var database = new Database(vistaDB, "");									
			database.map_Tables()
					.map_Table_Columns();
			tables_Names.clear();						
			tables_Names.add_Nodes(database.Tables);
			tables_Names.selectFirst(); 
			tables_Names.backColor(Color.White);

			return vistaDB;
		}
		
		public static API_VistaDB add_Viewer_TablesData<T>(this API_VistaDB vistaDB , T control)
			where T : Control
		{		
			control.clear();
			var dataGridView = control.add_DataGridView();
			
			dataGridView.DataError+= (sender,e) => {}; //" dataGridView error: {0}".error(e.Context);};
			var tables_Names = dataGridView.insert_Left<Panel>(200).add_TreeView().sort();		 	
			var database_Names = tables_Names.insert_Above<Panel>(100).add_TreeView().sort();
			var preloadAllData = false;
			tables_Names.insert_Below(20).add_CheckBox("Preload all data from database",0,0,(value)=>preloadAllData = value).autoSize();//.check();
			var rowData = dataGridView.insert_Below<Panel>(100).add_SourceCodeViewer(); 
			var rowDataField = rowData.insert_Left<Panel>(100).add_TreeView();
			var selectedField = "";
			
			rowDataField.afterSelect<DataGridViewCell>( 
				(cell)=>{
							selectedField = rowDataField.selected().get_Text();
							var fieldContent = cell.Value.str().fixCRLF();
							if (fieldContent.starts("<?xml"))
							{	
								"mapping xml".info(); 
								fieldContent = fieldContent.xmlFormat();
								rowData.set_Text(fieldContent,"a.xml");
							}
							else
								rowData.set_Text(fieldContent);
						});
			
			dataGridView.afterSelect(
				(row)=> {																					
							rowDataField.clear();
							//rowData.set_Text("");
							foreach(DataGridViewCell cell in row.Cells)
							{
								var fieldName = dataGridView.Columns[cell.ColumnIndex].Name;
								var node = rowDataField.add_Node(fieldName,cell);
								if (fieldName == selectedField)
									node.selected();
							}
							if (rowDataField.selected().isNull())
								rowDataField.selectFirst();																						
						});
						
			database_Names.afterSelect<string>(
				(database_Name)=>{
									tables_Names.backColor(Color.Salmon);
									O2Thread.mtaThread(
										()=>{
												var database = new Database(vistaDB, database_Name);									
												database.map_Tables();
												if (preloadAllData)																							
													database.map_Table_Data();												
												tables_Names.clear();						
												tables_Names.add_Nodes(database.Tables);
												tables_Names.selectFirst(); 
												tables_Names.backColor(Color.White);
												
												database_Names.splitContainer().panel1Collapsed(true);
											});
								 }); 
			
			Action<Table> loadTableData = 
				(table)=>{
							tables_Names.backColor(Color.Salmon);
							O2Thread.mtaThread(
										()=>{
												rowDataField.clear();
												rowData.set_Text("");	
												dataGridView.remove_Columns();							
												if (table.TableData.isNull())							
													vistaDB.map_Table_Data(table);								
												dataGridView.invokeOnThread(()=>dataGridView.DataSource= table.TableData);		
												tables_Names.backColor(Color.White);
											});
						 }; 
			tables_Names.afterSelect<Table>( 
				(table)=>{
							loadTableData(table);
						 });
			
			database_Names.add_Nodes(vistaDB.database_Names());
			
			database_Names.selectFirst();  
						
			
			tables_Names.add_ContextMenu().add_MenuItem("reload data",
				()=>{
						var selectedNode = tables_Names.selected();
						if (selectedNode.notNull())
						{
							var table = (Table)tables_Names.selected().get_Tag();
							table.TableData = null;
							loadTableData(table);
						}
					});
			return vistaDB;
		}
		
		public static API_VistaDB add_GUI_SqlCommandExecute<T>(this API_VistaDB vistaDB , T control)
			where T : Control
		{
			Action<string> executeNonQuery=null;
			Action<string> executeReader =null;
			var resultsPanel = control.add_GroupBox("Result") ;  
			var sqlCommandToExecute = resultsPanel.insert_Above("Sql Command to execute").add_TextArea(); 
			var sampleQueries = sqlCommandToExecute.insert_Left(300, "Sample Queries")
												   .add_TreeView()
												   .afterSelect<string>((text)=>sqlCommandToExecute.set_Text(text));
 
			sqlCommandToExecute.insert_Right(200)
							   .add_Button("Execute Non Query") 
							   .fill()
							   .onClick(()=>{
					 							"Executing Non Query".info();		 							
					 							executeNonQuery(sqlCommandToExecute.get_Text());
					 						})
							  .insert_Above() 
							  .add_Button("Execute Reader")
							  .fill()
							  .onClick(()=> {
				 								"Executing Reader".info();
				 								executeReader(sqlCommandToExecute.get_Text());
				 							});;
			
			executeReader = (sqlQuery)=>{
											vistaDB.add_Viewer_QueryResult(resultsPanel, sqlQuery);
											"done".info();
										};	
										
			executeNonQuery = (sqlText)=> {			
												var	log = resultsPanel.control<TextBox>();
												if (log.isNull())
													log = resultsPanel.clear().add_TextArea();
												if (sqlText.contains("GO".line())) 
												{										
													var sqlTexts = sqlText.line().split("GO".line()); 
													log.append_Line("[{0}]Found a GO, so breaking it into {1} queries".format(DateTime.Now,sqlTexts.size()));		  								
													var sqlConnection = vistaDB.getOpenConnection();
													foreach(var text in sqlTexts)																					
													{				
														vistaDB.executeNonQuery(sqlConnection, text);																				
														
														if (vistaDB.LastError.valid())
														{
															log.append_Line("SQL ERROR: {0}".lineBeforeAndAfter().format(vistaDB.LastError)); 
															log.append_Line("ERROR: stoping execution since there was an error which executing the query: {0}".format(text).lineBeforeAndAfter());
															break;
														}			
													}
													vistaDB.closeConnection(sqlConnection);
												}
												else
													{
														log.append_Line("Executing as Non Query: {0}".format(sqlText));
														vistaDB.LastError = ""; 
														vistaDB.executeNonQuery(sqlText);
														if (vistaDB.LastError.valid())
															log.append_Line("SQL ERROR: {0}".lineBeforeAndAfter().format(vistaDB.LastError)); 
													}
												"done".info();
										   };			
				
			sampleQueries.add_Nodes(new string[] {
													"select * from master..sysDatabases",
													"select * from master.Information_Schema.Tables",
													"select * from master.Information_Schema.Routines"
												});
			sampleQueries.selectFirst(); 
			return vistaDB;
		}
	}		
}
