using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace MaintenanceSimulatorShuJuJianKong
{
    public class AccessHelper
    {
        public static string currentDatabasePath, connectionStatement, currentDatabaseName;

        public static void SetDatabasePath(string databasePath, string databaseName)
        {
            currentDatabaseName = databaseName;
            currentDatabasePath = System.IO.Path.Combine(databasePath, databaseName);
            connectionStatement = "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + currentDatabasePath;
        }

        //执行增加（insert）、删除（delete）、修改（update）的方法
        public static int ExecuteNonQuery(string sql, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionStatement))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        //返回有且只有一行一列查询结果
        public static object ExecuteScalar(string sql, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionStatement))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
        }

        //执行多行多列查询
        /*public static DataTable ExecuteDataSet(string sql, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionSentence))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataSet dataset = new DataSet();
                    adapter.Fill(dataset);

                    return dataset.Tables[0];
                }
            }
        }*/
        public static DataTable ExecuteDataTable(string sql, params OleDbParameter[] parameters)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionStatement))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    DataTable dt = new DataTable();
                    OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                    oleda.Fill(dt);

                    return (dt);
                }
            }
        }

        ///   <summary>   
        ///   判断字符串是否为全数字
        ///   </summary>   
        ///   <param   name="value">字符串</param>
        ///   <returns>true:字符串全部为数字；false:字符串非全数字</returns>  
        public static bool IsNumber(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]*$");
        }

        public enum SortType
        {
            commandReceivedTimestamp,
            transmittingDeviceAddress,
            transmittingDeviceAddressInChinese,
            receivingDeviceAddress,
            receivingDeviceAddressInChinese,
            commandCode,
            commandCodeInChinese
        };
        public static SortType currentSortType = SortType.commandReceivedTimestamp;
        public static List<GlobalDefinitions.Command> SortBy(SortType sortType, List<GlobalDefinitions.Command> array)
        {
            List<GlobalDefinitions.Command> temp = new List<GlobalDefinitions.Command>();
            temp = array;
            currentSortType = sortType;
            temp.Sort(CompareItem);

            return temp;
        }

        public static int CompareItem(GlobalDefinitions.Command x, GlobalDefinitions.Command y)
        {
            string innerX, innerY;
            
            switch(currentSortType)
            {
                case SortType.commandReceivedTimestamp:
                    innerX = x.commandReceivedTimestamp;
                    innerY = y.commandReceivedTimestamp;
                    break;
                case SortType.transmittingDeviceAddress:
                    innerX = x.transmittingDeviceAddress;
                    innerY = y.transmittingDeviceAddress;
                    break;
                case SortType.transmittingDeviceAddressInChinese:
                    innerX = x.transmittingDeviceAddressInChinese;
                    innerY = y.transmittingDeviceAddressInChinese;
                    break;
                case SortType.receivingDeviceAddress:
                    innerX = x.receivingDeviceAddress;
                    innerY = y.receivingDeviceAddress;
                    break;
                case SortType.receivingDeviceAddressInChinese:
                    innerX = x.receivingDeviceAddressInChinese;
                    innerY = y.receivingDeviceAddressInChinese;
                    break;
                case SortType.commandCode:
                    innerX = x.commandCode;
                    innerY = y.commandCode;
                    break;
                case SortType.commandCodeInChinese:
                    innerX = x.commandCodeInChinese;
                    innerY = y.commandCodeInChinese;
                    break;
                default:
                    innerX = x.commandReceivedTimestamp;
                    innerY = y.commandReceivedTimestamp;
                    break;
            }
            
            if (innerX == String.Empty)
            {
                if (innerY == String.Empty)
                {
                    //X与Y皆为空，X等于Y
                    return 0;
                }
                else
                {
                    //X为空，Y非空，Y大于X
                    return -1;
                }
            }
            else
            {
                //X非空
                if (innerY == String.Empty)
                //Y为空，X大于Y
                {
                    return 1;
                }
                else
                {
                    //Y非空，则按照拼音顺序比较X与Y
                    int temp = innerX.CompareTo(innerY);
                    return temp;
                }
            }
        }

        public static SortType ConvertStringToSortType(string type)
        {
            SortType result = SortType.commandReceivedTimestamp;
            switch(type)
            {
                case "来报时间":
                    result = SortType.commandReceivedTimestamp;
                    break;
                case "发送方":
                    result = SortType.transmittingDeviceAddressInChinese;
                    break;
                case "发送方编码":
                    result = SortType.transmittingDeviceAddress;
                    break;
                case "接收方":
                    result = SortType.receivingDeviceAddressInChinese;
                    break;
                case "接收方编码":
                    result = SortType.receivingDeviceAddress;
                    break;
                case "命令码":
                    result = SortType.commandCode;
                    break;
                case "命令名称":
                    result = SortType.commandCodeInChinese;
                    break;
                default:
                    result = SortType.commandReceivedTimestamp;
                    break;
            }

            return result;
        }

        ///   <summary>   
        ///   从数据库中读取所有数据项
        ///   </summary>   
        ///   <param   name="results">命令类集合，引用类型</param>
        ///   <returns>空</returns>  
        public static void ReadAllItemsInDatabase(ref List<GlobalDefinitions.Command> results)
        {
            //"SELECT * FROM usfFncs WHERE idName = 'GetBuffer'";
            //List<usfFncsDefinitions> results = new List<usfFncsDefinitions>();
            string queryString = String.Format("SELECT * FROM {0}", currentDatabasePath);
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);                                
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();                    
                    while (reader.Read())
                    {

                        GlobalDefinitions.Command temp = new GlobalDefinitions.Command(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                                                       reader.GetString(3), reader.GetString(4), reader.GetString(5),
                                                                                       reader.GetString(6), reader.GetString(7));   
                        results.Add(temp);                        
                    }
                    // always call Close when done reading.
                    reader.Close();  
                }
                catch (System.Exception ex)
                {
                    
                }
                finally
                {
                    connection.Close();
                }                                              
            }
        }

        /*public static void ReadAllItemsWithSpecifiedType(string type, ref List<string> results)
        {
            string queryString = "SELECT " + type + "FROM " + currentDatabaseName;
            //string judge = String.Empty;
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string temp = reader.GetString(0);
                        //if(!temp.Equals(judge))
                        if(!results.Contains(temp))
                        {
                            results.Add(temp);
                            //judge = temp;
                        }                        
                    }
                    // always call Close when done reading.
                    reader.Close();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }*/

        public static void ReadAllItemsWithSpecifiedType(string type, ref List<GlobalDefinitions.Command> results)
        {            
            //string queryString = String.Format("SELECT * FROM usfFncs WHERE idType = '{0}' ORDER BY (idType+idSubType+idName) ASC", idType);
            string queryString = String.Format("SELECT * FROM {0} WHERE {1} = '{2}' ORDER BY ({3}) ASC", currentDatabasePath, type, type, type);
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        GlobalDefinitions.Command temp = new GlobalDefinitions.Command(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                                                       reader.GetString(3), reader.GetString(4), reader.GetString(5),
                                                                                       reader.GetString(6), reader.GetString(7));                        
                        results.Add(temp);
                    }
                    // always call Close when done reading.
                    reader.Close();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    connection.Close();                    
                }
            }
        }

        /*public static void ReadDatabaseByIdNameAndIdVersion(string idName, string idVersion, ref List<GlobalDefinitions.Command> results)
        {
            string queryString = String.Format("SELECT * FROM usfFncs WHERE idName = '{0}' and idVersion = '{1}'", idName, idVersion);
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        GlobalDefinitions.Command temp = new GlobalDefinitions.Command(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                                                        reader.GetString(3), reader.GetString(4), reader.GetString(5),
                                                                                        reader.GetString(6), reader.GetString(7));                       
                        results.Add(temp);
                    }
                    // always call Close when done reading.
                    reader.Close();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }*/

        ///   <summary>   
        ///   从数据库中读取符合关键字的数据项
        ///   </summary>   
        ///   <param   name="keyword">关键字</param>
        ///   <param   name="results">命令类集合，引用类型</param>
        ///   <returns>空</returns>  
        public static void ReadDatabaseByKeyword(string keyword, ref List<GlobalDefinitions.Command> results)
        {
            //SELECT * FROM usfFncs WHERE idContent LIKE %Keyword%
            //string queryString = String.Format("SELECT * FROM usfFncs WHERE idName LIKE '%{0}%' OR idDescription LIKE '%{1}%'", keyword, keyword);   
            //string queryString = String.Format("SELECT * FROM usfFncs WHERE idDescription LIKE '%{0}%'", keyword);
            string queryString = String.Format("SELECT * FROM {0} WHERE commandReceivedTimestamp LIKE '%{1}%' OR transmittingDeviceAddressInChinese LIKE '%{2}%' OR receivingDeviceAddressInChinese LIKE '%{3}%' OR commandCodeInChinese LIKE '%{4}%'", currentDatabasePath, keyword, keyword, keyword, keyword);
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        GlobalDefinitions.Command temp = new GlobalDefinitions.Command(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                                                       reader.GetString(3), reader.GetString(4), reader.GetString(5),
                                                                                       reader.GetString(6), reader.GetString(7));
                        results.Add(temp);
                    }
                    // always call Close when done reading.
                    reader.Close();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }

        ///   <summary>   
        ///   从数据库中读取符合关键字的数据项
        ///   </summary>   
        ///   <param   name="sortType">需要在数据项中搜索的类别</param>
        ///   <param   name="keyword">关键字</param>
        ///   <param   name="results">命令类集合，引用类型</param>
        ///   <returns>空</returns>  
        public static void ReadDatabaseByKeyword(SortType sortType, string keyword, ref List<GlobalDefinitions.Command> results)
        {
            string queryString = String.Empty;//String.Format("SELECT * FROM {0} WHERE commandReceivedTimestamp LIKE '%{1}%' OR transmittingDeviceAddressInChinese LIKE '%{2}%' OR receivingDeviceAddressInChinese LIKE '%{3}%' OR commandCodeInChinese LIKE '%{4}%'", currentDatabasePath, keyword, keyword, keyword, keyword);

            switch(sortType)
            {
                case SortType.transmittingDeviceAddressInChinese:
                    queryString = String.Format("SELECT * FROM {0} WHERE transmittingDeviceAddressInChinese LIKE '%{1}%'", currentDatabasePath, keyword);
                    break;
                case SortType.receivingDeviceAddressInChinese:
                    queryString = String.Format("SELECT * FROM {0} WHERE receivingDeviceAddressInChinese LIKE '%{1}%'", currentDatabasePath, keyword);
                    break;
                case SortType.commandReceivedTimestamp:
                    queryString = String.Format("SELECT * FROM {0} WHERE commandReceivedTimestamp LIKE '%{1}%'", currentDatabasePath, keyword);
                    break;
                case SortType.commandCodeInChinese:
                    queryString = String.Format("SELECT * FROM {0} WHERE commandCodeInChinese LIKE '%{1}%'", currentDatabasePath, keyword);
                    break;
                default:
                    queryString = String.Format("SELECT * FROM {0} WHERE commandReceivedTimestamp LIKE '%{1}%' OR transmittingDeviceAddressInChinese LIKE '%{2}%' OR receivingDeviceAddressInChinese LIKE '%{3}%' OR commandCodeInChinese LIKE '%{4}%'", currentDatabasePath, keyword, keyword, keyword, keyword);
                    break;
            }
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        GlobalDefinitions.Command temp = new GlobalDefinitions.Command(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                                                                                       reader.GetString(3), reader.GetString(4), reader.GetString(5),
                                                                                       reader.GetString(6), reader.GetString(7));
                        results.Add(temp);
                    }
                    // always call Close when done reading.
                    reader.Close();
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }

        ///   <summary>   
        ///   向数据库插入符合规定的数据项
        ///   </summary>   
        ///   <param   name="item">Command类</param>
        ///   <returns>空</returns>  
        public static string InsertItemIntoLibrary(GlobalDefinitions.Command item)
        {
            //用两个单引号替换一个单引号，否则会语法错误
            /*string queryString = String.Format("INSERT INTO {0}(commandReceivedTimestamp,transmittingDeviceAddressInChinese,transmittingDeviceAddress,receivingDeviceAddressInChinese,receivingDeviceAddress,commandCodeInChinese,commandCode,commandBuffer) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                                currentDatabasePath,                                
                                                item.commandReceivedTimestamp.Replace("'", "''"), 
                                                item.transmittingDeviceAddressInChinese.Replace("'", "''"), 
                                                item.transmittingDeviceAddress.Replace("'", "''"), 
                                                item.receivingDeviceAddressInChinese.Replace("'", "''"), 
                                                item.receivingDeviceAddress.Replace("'", "''"), 
                                                item.commandCodeInChinese.Replace("'", "''"), 
                                                item.commandCode.Replace("'", "''"), 
                                                item.commandBuffer.Replace("'", "''"));*/         
            /*string queryString = String.Format("INSERT INTO {0}(commandReceivedTimestamp,transmittingDeviceAddressInChinese,transmittingDeviceAddress,receivingDeviceAddressInChinese,receivingDeviceAddress,commandCodeInChinese,commandCode,commandBuffer) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                                currentDatabasePath,
                                                item.commandReceivedTimestamp,
                                                item.transmittingDeviceAddressInChinese,
                                                item.transmittingDeviceAddress,
                                                item.receivingDeviceAddressInChinese,
                                                item.receivingDeviceAddress,
                                                item.commandCodeInChinese,
                                                item.commandCode,
                                                item.commandBuffer);*/
            string queryString = String.Format("INSERT INTO {0}(commandReceivedTimestamp,transmittingDeviceAddress,transmittingDeviceAddressInChinese,receivingDeviceAddress,receivingDeviceAddressInChinese,commandCode,commandCodeInChinese,commandBuffer) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                                currentDatabasePath,
                                                item.commandReceivedTimestamp,
                                                item.transmittingDeviceAddress,
                                                item.transmittingDeviceAddressInChinese,
                                                item.receivingDeviceAddress,
                                                item.receivingDeviceAddressInChinese,
                                                item.commandCode,
                                                item.commandCodeInChinese,                                                
                                                item.commandBuffer);
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return "0";
                }
                catch (System.Exception ex)
                {
                    return ex.ToString();
                }
                /*finally
                {
                    connection.Close();
                }*/
            }
        }

        public static string SaveItemToLibrary(GlobalDefinitions.Command itemToAdd)
        {
            /*if (String.IsNullOrWhiteSpace(itemToAdd.commandReceivedTimestamp) || String.IsNullOrWhiteSpace(itemToAdd.transmittingDeviceAddressInChinese) 
                || String.IsNullOrWhiteSpace(itemToAdd.transmittingDeviceAddress) || String.IsNullOrWhiteSpace(itemToAdd.receivingDeviceAddressInChinese) 
                || String.IsNullOrWhiteSpace(itemToAdd.receivingDeviceAddress) || String.IsNullOrWhiteSpace(itemToAdd.commandCodeInChinese)
                || String.IsNullOrWhiteSpace(itemToAdd.commandCode) || String.IsNullOrWhiteSpace(itemToAdd.commandBuffer))
            {
                return false;
            }
            else*/
            {                
                string result = InsertItemIntoLibrary(itemToAdd);

                return result;
            }
        }

        public static bool DeleteItemFromLibrary(GlobalDefinitions.Command itemToDelete)
        {
            string queryString = String.Format("DELETE FROM {0} WHERE commandReceivedTimestamp='{1}' and transmittingDeviceAddressInChinese='{2}' and transmittingDeviceAddress='{3}' and receivingDeviceAddressInChinese='{4}' and receivingDeviceAddress='{5}' and commandCodeInChinese='{6}' and commandCode='{7}' and commandBuffer='{8}'",
                                                currentDatabasePath,
                                                itemToDelete.commandReceivedTimestamp.Replace("'", "''"), 
                                                itemToDelete.transmittingDeviceAddressInChinese.Replace("'", "''"), 
                                                itemToDelete.transmittingDeviceAddress.Replace("'", "''"), 
                                                itemToDelete.receivingDeviceAddressInChinese.Replace("'", "''"), 
                                                itemToDelete.receivingDeviceAddress.Replace("'", "''"), 
                                                itemToDelete.commandCodeInChinese.Replace("'", "''"), 
                                                itemToDelete.commandCode.Replace("'", "''"), 
                                                itemToDelete.commandBuffer.Replace("'", "''"));
            /*string queryString = String.Format("DELETE FROM usfFncs WHERE idType='{0}' and idSubType='{1}' and idName='{2}' and idVersion='{3}' and idFinalModificationDate='{4}' and idFinalModificationPerson='{5}' and idDescription='{6}' and idContent='{7}'",
                                                itemToDelete.idType, itemToDelete.idSubType, itemToDelete.idName,
                                                itemToDelete.idVersion, itemToDelete.idFinalModificationDate,
                                                itemToDelete.idFinalModificationPerson, itemToDelete.idDescription, itemToDelete.idContent.Replace("'", "''"));*/
            bool result = true;
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    result = false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static bool UpdateItemFromLibrary(GlobalDefinitions.Command newData, GlobalDefinitions.Command pleaseUpdateMe)
        {
            string queryString = String.Format("UPDATE {0} SET commandReceivedTimestamp='{1}',transmittingDeviceAddressInChinese='{2}',transmittingDeviceAddress='{3}',receivingDeviceAddressInChinese='{4}'," +
                                                "receivingDeviceAddress='{5}',commandCodeInChinese='{6}',commandCode='{7}',commandBuffer='{8}' " +
                                                "WHERE commandReceivedTimestamp='{9}' and transmittingDeviceAddressInChinese='{10}' and transmittingDeviceAddress='{11}' and receivingDeviceAddressInChinese='{12}' and receivingDeviceAddress='{13}' " +
                                                "and commandCodeInChinese='{14}' and commandCode='{15}' and commandBuffer='{16}'",
                                                currentDatabasePath,
                                                newData.commandReceivedTimestamp, 
                                                newData.transmittingDeviceAddressInChinese.Replace("'", "''"), 
                                                newData.transmittingDeviceAddress.Replace("'", "''"),
                                                newData.receivingDeviceAddressInChinese.Replace("'", "''"), 
                                                newData.receivingDeviceAddress.Replace("'", "''"),
                                                newData.commandCodeInChinese.Replace("'", "''"), 
                                                newData.commandCode.Replace("'", "''"), 
                                                newData.commandBuffer.Replace("'", "''"), 
                                                pleaseUpdateMe.commandReceivedTimestamp, 
                                                pleaseUpdateMe.transmittingDeviceAddressInChinese.Replace("'", "''"), 
                                                pleaseUpdateMe.transmittingDeviceAddress.Replace("'", "''"),
                                                pleaseUpdateMe.receivingDeviceAddressInChinese.Replace("'", "''"), 
                                                pleaseUpdateMe.receivingDeviceAddress.Replace("'", "''"),
                                                pleaseUpdateMe.commandCodeInChinese.Replace("'", "''"), 
                                                pleaseUpdateMe.commandCode.Replace("'", "''"), 
                                                pleaseUpdateMe.commandBuffer.Replace("'", "''"));           
            bool result = true;
            using (OleDbConnection connection = new OleDbConnection(connectionStatement))
            {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    result = false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        //数据项重复性检测，返回true表明数据重复，返回false表明数据不重复
        public static bool RepeatabilityCheck(GlobalDefinitions.Command itemWaitForCheck)
        {
            bool result = false;
            List<GlobalDefinitions.Command> correspondItems = new List<GlobalDefinitions.Command>();
            ReadAllItemsInDatabase(ref correspondItems);
            foreach(GlobalDefinitions.Command item in correspondItems)
            {
                if(item.commandReceivedTimestamp.Equals(itemWaitForCheck.commandReceivedTimestamp) 
                    && item.transmittingDeviceAddress.Equals(itemWaitForCheck.transmittingDeviceAddress)
                    && item.transmittingDeviceAddressInChinese.Equals(itemWaitForCheck.transmittingDeviceAddressInChinese)
                    && item.receivingDeviceAddress.Equals(itemWaitForCheck.receivingDeviceAddress)
                    && item.receivingDeviceAddressInChinese.Equals(itemWaitForCheck.receivingDeviceAddressInChinese)
                    && item.commandCode.Equals(itemWaitForCheck.commandCode)
                    && item.commandCodeInChinese.Equals(itemWaitForCheck.commandCodeInChinese)
                    && item.commandBuffer.Equals(itemWaitForCheck.commandBuffer))
                {
                    result = true;
                    break;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }      
    }
}
