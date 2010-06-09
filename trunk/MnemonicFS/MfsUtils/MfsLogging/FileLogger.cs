/**
 * Copyright © 2009, Najeeb Shaikh
 * All rights reserved.
 * http://www.mnemonicfs.org
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 * - Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * 
 * - Neither the name of the MnemonicFS Team, nor the names of its
 * contributors may be used to endorse or promote products
 * derived from this software without specific prior written
 * permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Data.SQLite;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsCore;
using MnemonicFS.MfsUtils.MfsDB;
using MnemonicFS.MfsUtils.MfsSystem;
using MnemonicFS.MfsUtils.MfsConfig;

namespace MnemonicFS.MfsUtils.MfsLogging {
    internal static class FileLogger {
        internal static string LOGDB_CONN_STR_FOR_WRITING = null;
        internal static string LOGDB_CONN_STR_FOR_READING = null;

        static FileLogger () {
            // Run during application startup.
            LOGDB_CONN_STR_FOR_READING = GetLogDBConnectionStringForReading ();
            LOGDB_CONN_STR_FOR_WRITING = GetLogDBConnectionStringForWriting ();

            string logDB = BaseSystem.GetLogDBBaseLocation () + BaseSystem.GetLogDBFileName ();
            Debug.Print ("Got log db location: " + logDB);

            if (!File.Exists (logDB)) {
                Trace.TraceInformation ("Log db does not exist.");
                InitLogDB (logDB);
            } else {
                // Else do nothing.
                Debug.Print ("Log db exists.");
            }
        }

        private static string GetLogDBConnectionStringForWriting () {
            //return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetSystemDBFileName () + ";Password=" + BaseSystem.GetLogDBPassword () + ";Pooling=True;Max Pool Size=100";
            return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetLogDBFileName () + ";Password=" + BaseSystem.GetLogDBPassword ();
        }

        private static string GetLogDBConnectionStringForReading () {
            //return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetSystemDBFileName () + ";Read Only=True;Password=" + BaseSystem.GetLogDBPassword () + ";Pooling=True;Max Pool Size=100";
            return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetLogDBFileName () + ";Read Only=True;Password=" + BaseSystem.GetLogDBPassword ();
        }

        private static void InitLogDB (string logDB) {
            Trace.TraceInformation ("Creating log db...");
            SQLiteConnection cnn = new SQLiteConnection (LOGDB_CONN_STR_FOR_WRITING);

            Debug.Print ("New connection created. Opening connection for log db...");
            cnn.Open ();

            Debug.Print ("Changing password.");
            cnn.ChangePassword (BaseSystem.GetLogDBPassword ());

            string sql = LogSchema.GetLogSchema ();
            Debug.Print ("Adding log db table schema: " + sql);

            SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
            myCommand.ExecuteNonQuery ();
            Trace.TraceInformation ("Done creating log db!");

            cnn.Close ();
            Debug.Print ("Closed connection for creating log db.");
        }

        internal static void AddLogEntry (string userIDStr, ulong fileID, FileLogEntryType fileLogEntryType, DateTime when, string extraInfo1, string extraInfo2) {
            string insertDateTime = StringUtils.GetAsZeroPaddedFourCharString (when.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Second);

            string sql = "insert into L_FileLogEntries (UserIDStr, fkey_FileID, AccessType, WhenDateTime, ExtraInfo1, ExtraInfo2) values ('" +
                userIDStr + "', " + fileID + ", '" + fileLogEntryType.ToString () + "', '" + insertDateTime + "', " +
                (extraInfo1 == null ? "null, " : "'" + extraInfo1 + "', ") +
                (extraInfo2 == null ? "null" : "'" + extraInfo2 + "'") +
                ")";
            Debug.Print ("Add file log entry: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (LOGDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.ExecuteNonQuery ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (transaction != null) {
                    transaction.Commit ();
                }
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static List<MfsFileLogEntry> RetrieveFileLogs (string userIDStr, ulong fileID) {
            string sql = "select AccessType, WhenDateTime, ExtraInfo1, ExtraInfo2 from L_FileLogEntries where UserIdStr=@userIDStr and fkey_FileID=" + fileID;
            Debug.Print ("Get file logs: " + sql);

            List<MfsFileLogEntry> fileLogs = new List<MfsFileLogEntry> ();

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (LOGDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);
                foreach (DataRow row in dt.Rows) {
                    FileLogEntryType accessType = GetFileLogEntryType (row[0].ToString ());
                    DateTime when = DateTime.Parse (row[1].ToString ());
                    string extraInfo1 = null;
                    if (!(row[2].ToString () == string.Empty)) {
                        extraInfo1 = row[2].ToString ();
                    }
                    string extraInfo2 = null;
                    if (!(row[3].ToString () == string.Empty)) {
                        extraInfo2 = row[3].ToString ();
                    }

                    MfsFileLogEntry fileLogEntry = new MfsFileLogEntry (userIDStr, fileID, accessType, when, extraInfo1, extraInfo2);
                    fileLogs.Add (fileLogEntry);
                }
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }

            return fileLogs;
        }

        internal static List<MfsFileLogEntry> RetrieveAllFilesLogs (string userIDStr) {
            string sql = "select fkey_FileID, AccessType, WhenDateTime, ExtraInfo1, ExtraInfo2 from L_FileLogEntries where UserIdStr=@userIDStr";
            Debug.Print ("Get file logs: " + sql);

            List<MfsFileLogEntry> fileLogs = new List<MfsFileLogEntry> ();

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (LOGDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = UInt64.Parse (row[0].ToString ());
                    FileLogEntryType accessType = GetFileLogEntryType (row[1].ToString ());
                    DateTime when = DateTime.Parse (row[2].ToString ());
                    string extraInfo1 = null;
                    if (!(row[3].ToString () == string.Empty)) {
                        extraInfo1 = row[3].ToString ();
                    }
                    string extraInfo2 = null;
                    if (!(row[4].ToString () == string.Empty)) {
                        extraInfo2 = row[4].ToString ();
                    }

                    MfsFileLogEntry fileLogEntry = new MfsFileLogEntry (userIDStr, fileID, accessType, when, extraInfo1, extraInfo2);
                    fileLogs.Add (fileLogEntry);
                }
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }

            return fileLogs;
        }

        internal static int DeleteFileLogs (string userIDStr, ulong fileID) {
            string sql = "delete from L_FileLogEntries where fkey_FileID=" + fileID;
            Debug.Print ("Delete file logs: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (LOGDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                return myCommand.ExecuteNonQuery ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (transaction != null) {
                    transaction.Commit ();
                }
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static int DeleteUserLogs (string userIDStr) {
            string sql = "delete from L_FileLogEntries where UserIdStr=@UserIdStr";
            Debug.Print ("Delete file logs: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (LOGDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);
                return myCommand.ExecuteNonQuery ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (transaction != null) {
                    transaction.Commit ();
                }
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        private static FileLogEntryType GetFileLogEntryType (string fileLogEntryTypeAsString) {
            switch (fileLogEntryTypeAsString) {
                case "CREATED":
                    return FileLogEntryType.CREATED;
                case "NEW_VERSION_CREATED":
                    return FileLogEntryType.NEW_VERSION_CREATED;
                case "DELETED":
                    return FileLogEntryType.DELETED;
                case "ACCESSED_ORIGINAL":
                    return FileLogEntryType.ACCESSED_ORIGINAL;
                case "ACCESSED_VERSION":
                    return FileLogEntryType.ACCESSED_VERSION;
                case "DELETION_DATETIME_SET":
                    return FileLogEntryType.DELETION_DATETIME_SET;
                case "DELETION_DATETIME_UPDATED":
                    return FileLogEntryType.DELETION_DATETIME_UPDATED;
                case "DELETION_DATETIME_RESET":
                    return FileLogEntryType.DELETION_DATETIME_RESET;
                case "FILENAME_UPDATED":
                    return FileLogEntryType.FILENAME_UPDATED;
                case "FILE_NARRATION_UPDATED":
                    return FileLogEntryType.FILE_NARRATION_UPDATED;
                case "FILE_SAVEDATETIME_UPDATED":
                    return FileLogEntryType.FILE_SAVEDATETIME_UPDATED;
                default:
                    return FileLogEntryType.UNDEFINED;
            }
        }
    }
}
