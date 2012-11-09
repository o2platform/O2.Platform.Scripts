// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.ViewHelpers
{
    public class CreateDataTable_Local
    {
        public static DataTable fromGenericList<T>(List<T> genericList)
        {
            var dataTable = new DataTable();
            //var type = genericList.GetType();
            //var typeName = genericList.GetType().Name;      //"List`1
            foreach (var property in DI.reflection.getProperties(typeof(T)))
                dataTable.Columns.Add(property.Name);

            foreach (var item in genericList)
            {
                var newRow = dataTable.NewRow();
                foreach (var property in DI.reflection.getProperties(typeof(T)))
                {
                    var cellValue = DI.reflection.getProperty(property.Name, item) ?? "";                    
                    switch (cellValue.GetType().ToString())
                    {
                        case "System.Collections.Generic.Dictionary`2[System.String,System.String]":
                            var dictionary = (Dictionary<string, string>) cellValue;
                            cellValue = "";
                            foreach (var dictionaryItem in dictionary)
                                cellValue += string.Format("{0}={1}  ,  ", dictionaryItem.Key, dictionaryItem.Value);
                            break;    

                        case "System.Collections.Generic.List`1[O2.Core.FileViewers.Interfaces.IStrutsConfig_Action_Forward]":
                            var forwards = (List<IStrutsConfig_Action_Forward>) cellValue;
                            cellValue = "";
                            foreach (var forward in forwards)
                                cellValue += forward.ToString();
                            break;
                    }                    
                    newRow[property.Name] = cellValue;                    
                }
                dataTable.Rows.Add(newRow);
            }            
            return dataTable;
        }        
        

        /*public static DataTable fromList_Object<T>(List<T> list, string columnTitle)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(columnTitle);
            if (list != null)
                foreach (var item in list)
                    dataTable.Rows.Add(new[] { item });
            return dataTable;
        }*/

        /*public static DataTable firstFieldTextSecondFieldTag<T>(List<T> genericList)
        {
            var dataTable = new DataTable();
            var properties = DI.reflection.getProperties(typeof(T));
            var fieldTextName = properties[0].Name;
            if (properties.Count > 1)
            {
                dataTable.Columns.Add(fieldTextName);
                foreach (var item in genericList)
                {
                    var newRow = dataTable.NewRow();
                    newRow[0] = DI.reflection.getProperty(fieldTextName, item);                
                    newRow.
                    dataTable.Rows.Add()
                    //dataTable.Rows.Add(});
                }
            }
            return dataTable;

        }*/
    }
}
