
using System;
using System.IO;
using System.Text;

namespace APIClientPackage
{
    /// <summary>
    /// Class definition for the log usage
    /// </summary>
    public class LogInformation
    {
        private readonly static object lockTakenReadWriterFile = new object();

        //CONSTS

        /// <summary>
        /// Value for class returns with sucess
        /// </summary>
        public const int OK_RET_VAL = 0;

        /// <summary>
        /// Value for class returns with error
        /// </summary>
        public const int ERROR_RET_VAL = (-1);

        /// <summary>
        /// Value for class returns with exception
        /// </summary>
        public const int EXCEPTION_RET_VAL = (-2);

        private string _info_label = "< Information >";
        private string _warning_label = "< warning >";
        private string _error_label = "< Error >";

        private string _log_path = AppDomain.CurrentDomain.BaseDirectory;
        private string _log_file = @"FileLog.log";
        private string _log_path_file = AppDomain.CurrentDomain.BaseDirectory + @"FileLog.log";

        private bool _showTypeLogLabel = false;

        /// <summary>Show the aditional tag to show in the log messages if true</summary>
        public bool ShowTypeLogLabel { get => _showTypeLogLabel; set => _showTypeLogLabel = value; }

        //private int getLogFilesInPath(ref List<string> logFilesInPathList)
        //{
        //    try
        //    {
        //        List<string> ext = new List<string> { ".log" };
        //        logFilesInPathList = Directory.GetFiles(_log_path, "*.*", SearchOption.AllDirectories)
        //             .Where(s => ext.Contains(Path.GetExtension(s))).ToList();
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.EventLog.WriteEntry("getLogFilesInPath", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
        //        return -1;
        //    }
        //}

        private DateTime GetFileCreationDate(string strFilePath)
        {
            DateTime returnDate = DateTime.Now;
            try
            {
                return File.GetCreationTime(strFilePath);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("getFileCreationDate", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return returnDate;
            }
        }

        //private DateTime getFileLastWriteDate(string strFilePath)
        //{
        //    DateTime returnDate = DateTime.Now;
        //    try
        //    {
        //        return File.GetLastWriteTime(strFilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.EventLog.WriteEntry("getFileLastWriteDate", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
        //        return returnDate;
        //    }
        //}

        //private int renameFileWithCreationDate(string strFilePath)
        //{
        //    try
        //    {
        //        DateTime dateOfFile = new DateTime();
        //        dateOfFile = getFileCreationDate(strFilePath);

        //        string strNewFilePath = "";
        //        string strNewFileName = "";
        //        string strNewFileExtention = "";
        //        string strNewFilePathName = "";

        //        string[] dataLineAll = strFilePath.Split('\\');

        //        int countItemsIndataLineAll = dataLineAll.Length;

        //        if (countItemsIndataLineAll < 1)
        //        {
        //            return -2;
        //        }

        //        int index_i = 0;
        //        int nr_of_items = countItemsIndataLineAll;
        //        for (index_i=0; index_i < (nr_of_items-1); index_i++)
        //        {
        //            strNewFilePath += dataLineAll[index_i] + '\\';
        //        }

        //        string[] dataLineFileName = dataLineAll[nr_of_items - 1].Split('.');

        //        int countItemsIndataLineFileName = dataLineFileName.Length;

        //        if (countItemsIndataLineFileName < 2)
        //        {
        //            return -3;
        //        }

        //        strNewFileName = dataLineFileName[0] + dateOfFile.Year.ToString("0000") + dateOfFile.Month.ToString("00") + dateOfFile.Day.ToString("00");
        //        strNewFileExtention = '.' + dataLineFileName[1];

        //        strNewFilePathName = strNewFilePath + strNewFileName + strNewFileExtention;

        //        System.IO.File.Move(strFilePath, strNewFilePathName);

        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.EventLog.WriteEntry("refreshLogPathFiles", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
        //        return -1;
        //    }
        //}

        private int RefreshLogPathFiles(string strFilePath)
        {
            try
            {
                if(!File.Exists(strFilePath))
                {
                    return 0;
                }

                DateTime nowDate = DateTime.Now;
                DateTime dateOfFile = GetFileCreationDate(strFilePath);

                if ((nowDate.Year != dateOfFile.Year) || (nowDate.Month != dateOfFile.Month) || (nowDate.Day != dateOfFile.Day))
                {
                    string strNewFilePathName = strFilePath + string.Format("({0}-{1}-{2})", dateOfFile.Year, dateOfFile.Month, dateOfFile.Day);
                    if (File.Exists(strNewFilePathName))
                    {
                        return 0;
                    }
                    System.IO.File.Move(strFilePath, strNewFilePathName);
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("refreshLogPathFiles", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return -1;
            }
        }

        /// <summary>
        ///     LogInformation Class constructor
        /// </summary>
        /// <param name="strFileName">Name of the log file (string)</param>
        public LogInformation(string strFileName)
        {
            _log_file = strFileName;

            string strTemp001 = _log_file.ToUpper();
            string strTemp002 = "";

            if (!strTemp001.Contains(@"Log/"))
            {
                strTemp002 = @"Log/";
            }

            _log_path_file = AppDomain.CurrentDomain.BaseDirectory + strTemp002 + _log_file;

            if (!strTemp001.Contains(".LOG"))
            {
                _log_path_file +=".log";
            }

            System.IO.FileInfo file = new System.IO.FileInfo(_log_path_file);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
        }

        /// <summary>
        ///     Clean the content of log file
        /// </summary>
        public void CleanFile()
        {
            lock (lockTakenReadWriterFile)
            {
                try
                {
                    System.IO.File.WriteAllText(_log_path_file, string.Empty);
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.EventLog.WriteEntry("CleanFile", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        ///     Set the path of the log file
        /// </summary>
        /// <param name="strPath">Path of the log (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int SetPath(string strPath)
        {
            try
            {
                _log_path = strPath;
                _log_path_file = _log_path + _log_file;
                return OK_RET_VAL;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("SetPath", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return EXCEPTION_RET_VAL;
            }
        }

        /// <summary>
        /// Set the name of the log file
        /// </summary>
        /// <param name="strFileName">File name of the log (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int SetFileName(string strFileName)
        {
            try
            {
                _log_file = @"\" + strFileName;
                _log_path_file = _log_path + _log_file;
                return OK_RET_VAL;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("SetFileName", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return EXCEPTION_RET_VAL;
            }
        }

        /// <summary>
        ///     Changes the info label value
        /// </summary>
        /// <param name="strLabelDescription">Info label new value (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int SetInfoLabel(string strLabelDescription)
        {
            try
            {
                _info_label = strLabelDescription;
                return OK_RET_VAL;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("SetInfoLabel", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return EXCEPTION_RET_VAL;
            }
        }

        /// <summary>
        ///     Changes the warning label value
        /// </summary>
        /// <param name="strLabelDescription">Warning label new value (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int SetWarningLabel(string strLabelDescription)
        {
            try
            {
                _warning_label = strLabelDescription;
                return OK_RET_VAL;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("SetWarningLabel", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return EXCEPTION_RET_VAL;
            }
        }

        /// <summary>
        ///     Changes the Error label value
        /// </summary>
        /// <param name="strLabelDescription">Error label new value (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int SetErrorLabel(string strLabelDescription)
        {
            try
            {
                _error_label = strLabelDescription;
                return OK_RET_VAL;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("SetErrorLabel", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                throw new Exception(ex.Message);
                //return EXCEPTION_RET_VAL;
            }
        }

        /// <summary>
        ///     Append a new message in log file without time and labels info
        /// </summary>
        /// <param name="strLog">Message to write in log (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int LogMsgOnly(string strLog)
        {
            lock (lockTakenReadWriterFile)
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    string strAux = "";

                    strLog = strLog.Replace("\n", Environment.NewLine);

                    strAux += strLog;

                    RefreshLogPathFiles(_log_path_file);

                    using (StreamWriter w = File.AppendText(_log_path_file))
                    {
                        //... Create File
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(_log_path_file, true, Encoding.Default))
                    {
                        file.WriteLine(strAux);
                    }

                    return OK_RET_VAL;
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.EventLog.WriteEntry("LogMsgOnly", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                    throw new Exception(ex.Message);
                    //return EXCEPTION_RET_VAL;
                }
            }
        }

        /// <summary>
        /// Append a new message in log file without time and labels info
        /// </summary>
        /// <param name="text">String with format text</param>
        /// <param name="args">Parameters</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int LogMsgOnly(string text, params object[] args)
        {
            return this.LogMsgOnly(string.Format(text, args));
        }

        /// <summary>
        /// Append a new message in log file without time and labels info
        /// </summary>
        /// <param name="exeption">Exception content to login the file</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int LogMsgOnly(Exception exeption)
        {
            return this.LogMsgOnly(exeption.ToString());
        }

        /// <summary>
        ///     Append a new message in log file with time info and INFO label
        /// </summary>
        /// <param name="strLog">Message to write in log (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Info(string strLog)
        {
            lock (lockTakenReadWriterFile)
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    string strAux = "";

                    strLog = strLog.Replace("\n", Environment.NewLine);

                    if (_showTypeLogLabel)
                    {
                        strAux += dt.ToString("yyyy-MM-dd HH:mm:ss.fff ") + _info_label + " -> " + strLog;// + Environment.NewLine;
                    }
                    else 
                    {
                        strAux += dt.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "-> " + strLog;// + Environment.NewLine;
                    }

                    RefreshLogPathFiles(_log_path_file);

                    using (StreamWriter w = File.AppendText(_log_path_file))
                    {
                        //... Create File
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(_log_path_file, true, Encoding.Default))
                    {
                        file.WriteLine(strAux);
                    }

                    return OK_RET_VAL;
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.EventLog.WriteEntry("Info", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                    throw new Exception(ex.Message);
                    //return EXCEPTION_RET_VAL;
                }
            }
        }

        /// <summary>
        /// Append a new message in log file with time info and INFO label
        /// </summary>
        /// <param name="text">String with format text</param>
        /// <param name="args">Parameters</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Info(string text, params object[] args)
        {
            return this.Info(string.Format(text, args));
        }
        /// <summary>
        /// Append a new message in log file with time info and INFO label
        /// </summary>
        /// <param name="exeption">Exception content to login the file</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Info(Exception exeption)
        {
            return this.Info(exeption.ToString());
        }

        /// <summary>
        ///     Append a new message in log file with time info and WARNING label
        /// </summary>
        /// <param name="strLog">Message to write in log (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Warn(string strLog)
        {
            lock (lockTakenReadWriterFile)
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    string strAux = "";

                    strLog = strLog.Replace("\n", Environment.NewLine);
                    
                    if (_showTypeLogLabel)
                    {
                        strAux += dt.ToString("yyyy-MM-dd HH:mm:ss.fff ") + _warning_label + " -> " + strLog;// + Environment.NewLine;
                    }
                    else
                    {
                        strAux += dt.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "-> " + strLog;// + Environment.NewLine;
                    }

                    RefreshLogPathFiles(_log_path_file);

                    using (StreamWriter w = File.AppendText(_log_path_file))
                    {
                        //... Create File
                    }

                    using (StreamWriter w = File.AppendText(_log_path_file))
                    {
                        //...
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(_log_path_file, true, Encoding.Default))
                    {
                        file.WriteLine(strAux);
                    }

                    return OK_RET_VAL;
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.EventLog.WriteEntry("Warning", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                    throw new Exception(ex.Message);
                    //return EXCEPTION_RET_VAL;
                }
            }
        }

        /// <summary>
        /// Append a new message in log file with time info and WARNING label
        /// </summary>
        /// <param name="text">String with format text</param>
        /// <param name="args">Parameters</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Warn(string text, params object[] args)
        {
            return this.Warn(string.Format(text, args));
        }

        /// <summary>
        /// Append a new message in log file with time info and WARNING label
        /// </summary>
        /// <param name="exeption">Exception content to login the file</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Warn(Exception exeption)
        {
            return this.Warn(exeption.ToString());
        }

        /// <summary>
        /// Append a new message in log file with time info and ERROR label
        /// </summary>
        /// <param name="strLog">Message to write in log (string)</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Error(string strLog)
        {
            lock (lockTakenReadWriterFile)
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    string strAux = "";

                    strLog = strLog.Replace("\n", Environment.NewLine);

                    if (_showTypeLogLabel)
                    {
                        strAux += dt.ToString("yyyy-MM-dd HH:mm:ss.fff ") + _error_label + " -> " + strLog;// + Environment.NewLine;
                    }
                    else
                    {
                        strAux += dt.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "-> " + strLog;// + Environment.NewLine;
                    }

                    RefreshLogPathFiles(_log_path_file);

                    using (StreamWriter w = File.AppendText(_log_path_file))
                    {
                        //... Create File
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(_log_path_file, true, Encoding.Default))
                    {
                        file.WriteLine(strAux);
                    }

                    return OK_RET_VAL;
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.EventLog.WriteEntry("Error", ex.StackTrace, System.Diagnostics.EventLogEntryType.Warning);
                    throw new Exception(ex.Message);
                    //return EXCEPTION_RET_VAL;
                }
            }
        }

        /// <summary>
        /// Append a new message in log file with time info and ERROR label
        /// </summary>
        /// <param name="text">String with format text</param>
        /// <param name="args">Parameters</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Error(string text, params object[] args)
        {
            return this.Error(string.Format(text, args));
        }

        /// <summary>
        /// Append a new message in log file with time info and ERROR label
        /// </summary>
        /// <param name="exeption">Exception content to login the file</param>
        /// <returns>== 0 - Sucess ; -1 - Exception ; != 0 - Error</returns>
        public int Error(Exception exeption)
        {
            return this.Error(exeption.ToString());
        }
    }
}