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
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Diagnostics;
using MnemonicFS.MfsUtils.MfsSystem;
using MnemonicFS.MfsUtils.MfsDB;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsUtils.MfsConfig;

namespace MnemonicFS.MfsCore {
    internal class MfsDBOperations {
        #region << Constants & Class Init Code >>

        private static string SYSTEMDB_CONN_STR_FOR_WRITING = null;
        private static string SYSTEMDB_CONN_STR_FOR_READING = null;

        static MfsDBOperations () {
            SYSTEMDB_CONN_STR_FOR_READING = GetSystemDBConnectionStringForReading ();
            SYSTEMDB_CONN_STR_FOR_WRITING = GetSystemDBConnectionStringForWriting ();

            string systemDB = BaseSystem.GetSystemDBBaseLocation () + BaseSystem.GetSystemDBFileName ();
            Debug.Print ("Got system db location: " + systemDB);

            if (!File.Exists (systemDB)) {
                Trace.TraceInformation ("System db does not exist.");
                InitSystemDB (systemDB);
            } else {
                // Else do nothing.
                Debug.Print ("System db exists.");
            }
        }

        private static void InitSystemDB (string systemDB) {
            Trace.TraceInformation ("Creating system db...");
            SQLiteConnection cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_WRITING);

            Debug.Print ("New connection created. Opening connection for system db...");
            cnn.Open ();

            Debug.Print ("Changing password.");
            cnn.ChangePassword (BaseSystem.GetSystemDBPassword ());

            string sql = SystemSchema.GetSystemSchema ();
            Debug.Print ("Adding system db table schema: " + sql);

            SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
            myCommand.ExecuteNonQuery ();
            Trace.TraceInformation ("Done creating system db!");

            cnn.Close ();
            Debug.Print ("Closed connection for creating system db.");
        }

        private static string GetSystemDBConnectionStringForWriting () {
            //return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetSystemDBFileName () + ";Password=" + BaseSystem.GetSystemDBPassword () + ";Pooling=True;Max Pool Size=100";
            return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetSystemDBFileName () + ";Password=" + BaseSystem.GetSystemDBPassword ();
        }

        private static string GetSystemDBConnectionStringForReading () {
            //return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetSystemDBFileName () + ";Read Only=True;Password=" + BaseSystem.GetSystemDBPassword () + ";Pooling=True;Max Pool Size=100";
            return "data source=" + Config.GetStorageBasePath () + BaseSystem.GetSystemDBFileName () + ";Read Only=True;Password=" + BaseSystem.GetSystemDBPassword ();
        }

        #endregion

        #region << User-related Operations >>

        internal static ulong CreateUser (string userIDStr, string passwordHash, string userSpecificPath) {
            string sql = null;
            sql = "insert into L_Users (UserIDStr, PasswordHash, UserSpecificPath) values (@userIDStr, @passwordHash, @UserSpecificPath); "
                      + "select last_insert_rowid() from L_Users";
            Debug.Print ("Create user: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);
                if (passwordHash != null) {
                    myCommand.Parameters.AddWithValue ("@passwordHash", passwordHash);
                }
                myCommand.Parameters.AddWithValue ("@UserSpecificPath", userSpecificPath);

                Debug.Print ("Executing create user.");
                SQLiteDataReader reader = myCommand.ExecuteReader ();
                Debug.Print ("Done executing create user.");

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        internal static bool AuthenticateUser (string userIDStr, string passwordHash) {
            string sql = "select PasswordHash from L_Users where UserIDStr=@userIDStr";
            Debug.Print ("Authenticate user: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                if (dt.Rows.Count == 0 || !dt.Rows[0][0].ToString ().Equals (passwordHash)) {
                    return false;
                }

                return true;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static List<string> GetMfsUsers () {
            string sql = "select UserIDStr from L_Users";
            Debug.Print ("Get Mfs Users: " + sql);

            List<string> users = new List<string> ();

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);
                foreach (DataRow row in dt.Rows) {
                    string user = row[0].ToString ();
                    users.Add (user);
                }
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }

            return users;
        }

        internal static int UpdateUserPassword (string userIDStr, string newPasswordHash) {
            string sql = "update L_Users set PasswordHash=@newPasswordHash where UserIDStr=@userIDStr";
            Debug.Print ("Delete user: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newPasswordHash", newPasswordHash);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        internal static int GetUserCount () {
            string sql = "select count(*) from L_Users";
            Debug.Print ("Get user count: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return int.Parse (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static int DeleteUser (string userIDStr) {
            string sql = "delete from L_Users where UserIDStr=@userIDStr";
            Debug.Print ("Delete user: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        internal static ulong DoesUserExist (string userIDStr) {
            string sql = "select key_UserID from L_Users where UserIDStr=@userIDStr";
            Debug.Print ("Does user exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                if (dt.Rows.Count == 0) {
                    return 0;
                }

                return ulong.Parse (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static string GetUserSpecificPath (string userIDStr) {
            string sql = "select UserSpecificPath from L_Users where UserIDStr=@userIDStr";
            Debug.Print ("Get user specific path: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", userIDStr);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                if (dt.Rows.Count == 0) {
                    return null;
                }

                return dt.Rows[0][0].ToString ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << User Instantiation Methods & Variables >>

        private string _userIDStr;
        private string _userSpecificPath;

        private string USERDB_CONN_STR_FOR_WRITING = null;
        private string USERDB_CONN_STR_FOR_READING = null;

        public MfsDBOperations (string userIDStr, string userSpecificPath) {
            _userIDStr = userIDStr;
            _userSpecificPath = userSpecificPath;

            // User db checking and creation:
            string userDBBaseLocation = BaseSystem.GetUserDBBaseLocation ();

            string userDBSpecificLocation = userDBBaseLocation + _userSpecificPath;

            Debug.Print ("Got user db file location: " + userDBSpecificLocation);

            USERDB_CONN_STR_FOR_READING = GetUserDBConnectionStringForReading ();
            USERDB_CONN_STR_FOR_WRITING = GetUserDBConnectionStringForWriting ();

            Debug.Print ("Got CONN_STR_FOR_WRITING: " + USERDB_CONN_STR_FOR_WRITING);
            Debug.Print ("Got CONN_STR_FOR_READING: " + USERDB_CONN_STR_FOR_READING);

            // If db does not already exist, create it as an encrypted db:
            // Stat the db file once to see if it exists:
            Debug.Print ("Checking to see if user db exists: " + userDBSpecificLocation);
            if (!File.Exists (userDBSpecificLocation + BaseSystem.GetUserDBFileName ())) {
                Trace.TraceInformation ("User db does not exist.");
                string pathToCreate = Config.GetStorageBasePath () + _userSpecificPath;
                Trace.TraceInformation ("Creating file path: " + pathToCreate);
                Directory.CreateDirectory (pathToCreate);
                Trace.TraceInformation ("Done creating path.");
                InitUserDB (userDBSpecificLocation);
            } else {
                // Else do nothing.
                Debug.Print ("User db exists.");
            }
        }

        private void InitUserDB (string userDB) {
            Trace.TraceInformation ("Creating user db...");
            SQLiteConnection cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);

            Debug.Print ("New connection created. Opening connection for user db...");
            cnn.Open ();

            Debug.Print ("Changing password.");
            cnn.ChangePassword (BaseSystem.GetUserDBPassword ());

            string sql = UserSchema.GetUserSchema ();
            Debug.Print ("Adding user db table schema: " + sql);

            SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
            myCommand.ExecuteNonQuery ();
            Trace.TraceInformation ("Done creating user db!");

            cnn.Close ();
            Debug.Print ("Closed connection for creating user db.");
        }

        internal string GetUserDBConnectionStringForWriting () {
            //return "data source=" + Config.GetStorageBasePath () + _userSpecificPath + BaseSystem.GetUserDBFileName () + ";Password=" + BaseSystem.GetUserDBPassword () + ";Pooling=True;Max Pool Size=100";
            return "data source=" + Config.GetStorageBasePath () + _userSpecificPath + BaseSystem.GetUserDBFileName () + ";Password=" + BaseSystem.GetUserDBPassword ();
        }

        internal string GetUserDBConnectionStringForReading () {
            //return "data source=" + Config.GetStorageBasePath () + _userSpecificPath + BaseSystem.GetUserDBFileName () + ";Read Only=True;Password=" + BaseSystem.GetUserDBPassword () + ";Pooling=True;Max Pool Size=100";
            return "data source=" + Config.GetStorageBasePath () + _userSpecificPath + BaseSystem.GetUserDBFileName () + ";Read Only=True;Password=" + BaseSystem.GetUserDBPassword ();
        }

        #endregion

        #region << Class-level Storage Operations >>

        internal static ulong SaveByteArrayMetaData (string assumedFileName, string archiveName, string destDir, int referenceNumber) {
            string sql = "insert into L_ByteStreams (ReferenceNumber, AssumedFileName, ArchiveName, ArchivePath) "
                   + "values (" + referenceNumber + ", @assumedFileName, @archiveName, @archivePath); "
                   + "select last_insert_rowid() from L_ByteStreams";
            Debug.Print ("Save file metadata: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@assumedFileName", assumedFileName);
                myCommand.Parameters.AddWithValue ("@archiveName", archiveName);
                myCommand.Parameters.AddWithValue ("@archivePath", destDir);

                SQLiteDataReader reader = myCommand.ExecuteReader ();
                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        internal static void GetByteStreamMetaData (ulong byteStreamID, out string assumedFileName, out string archiveName, out string archivePath, out int referenceNumber) {
            string sql = "select ReferenceNumber, AssumedFileName, ArchiveName, ArchivePath from L_ByteStreams where key_ByteStreamID=" + byteStreamID;
            Debug.Print ("Get byte stream metadata: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                referenceNumber = Int32.Parse (dt.Rows[0][0].ToString ());
                assumedFileName = dt.Rows[0][1].ToString ();
                Debug.Print (assumedFileName);
                archiveName = dt.Rows[0][2].ToString ();
                Debug.Print (archiveName);
                archivePath = dt.Rows[0][3].ToString ();
                Debug.Print (archivePath);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static int GetByteStreamReferenceNumber (ulong byteStreamID) {
            string sql = "select ReferenceNumber from L_ByteStreams where key_ByteStreamID=" + byteStreamID;
            Debug.Print ("Get byte stream reference number: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return Int32.Parse (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static bool DoesByteStreamExist (ulong byteStreamID) {
            string sql = "select ReferenceNumber from L_ByteStreams where key_ByteStreamID=" + byteStreamID;
            Debug.Print ("Does byte stream exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal static void DeleteByteStreamMetaData (ulong byteStreamID) {
            string sql = "delete from L_ByteStreams where key_ByteStreamID=" + byteStreamID;
            Debug.Print ("Delete byte stream: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_WRITING);
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

        #endregion

        #region << File-retention Operations >>

        /// <summary>
        /// This method returns a list of all the expired file names within the system. Expired files are those
        /// which files, the deletion dates of which have passed.
        /// </summary>
        /// <returns>A list containing the unique ids of all the expired files in the system.</returns>
        internal List<ulong> GetAllExpiredFiles () {
            string currentDateTime = "'" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " "
                + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "'";

            string sql = "select key_FileID from L_Files where DeletionDateTime<=" + currentDateTime;

            Debug.Print ("Get all expired files: " + sql);

            SQLiteConnection cnn = null;

            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();
                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> listExpiredFiles = new List<ulong> ();

                foreach (DataRow row in dt.Rows) {
                    listExpiredFiles.Add (ulong.Parse (dt.Rows[0][0].ToString ()));
                }

                return listExpiredFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << User Details-related Operations >>

        internal void GetUserName (out string fName, out string lName) {
            string sql = "select UserFName, UserLName from L_Users where UserIDStr=@userIDStr";
            Debug.Print ("Get user specific path: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (SYSTEMDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@userIDStr", _userIDStr);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                fName = dt.Rows[0][0].ToString ();
                lName = dt.Rows[0][1].ToString ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << File Meta-data Save / Deletion / Retrieval / Updation Operations >>

        /// <summary>
        /// This method saves the meta-data of the file that has already been persisted to storage, including its
        /// pathname on the storage device.
        /// </summary>
        /// <param name="fileName">Name of the file that has been saved.</param>
        /// <param name="fileNarration">Narration of the file that has been saved.</param>
        /// <param name="fileSize">Size of the file that has been saved.</param>
        /// <param name="filePath">Path of the file that has been saved on the storage device.</param>
        /// <param name="key">Password using which the file has been saved to storage. Null if file has been saved a plaintext.</param>
        /// <param name="when">Date-time value at which the file has been saved.</param>
        /// <returns>An id that uniquely identifies the file across the entire system.</returns>
        internal ulong SaveFileMetaData (
            string fileName, string fileNarration, int fileSize, string fileHash,
            string archiveName, string filePath, DateTime when,
            string assumedFileName, string filePassword
            ) {
            string insertDateTime = StringUtils.GetAsZeroPaddedFourCharString (when.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Second);

            string sql = "insert into L_Files (FileName, FileNarration, FileSize, FileHash, ArchiveName, FilePath, AssumedFileName, FilePassword, WhenDateTime) "
                   + "values (@fileName, @fileNarration, " + fileSize + ", '" + fileHash + "', '" + archiveName + "', '" + filePath + "', '" + assumedFileName + "', '" + filePassword + "', '"
                   + insertDateTime + "'); select last_insert_rowid() from L_Files";
            Debug.Print ("Save file metadata: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();
                
                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@fileName", fileName);
                myCommand.Parameters.AddWithValue ("@fileNarration", fileNarration);

                SQLiteDataReader reader = myCommand.ExecuteReader ();
                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        /// <summary>
        /// This method returns the name of a file stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which is sought.</param>
        /// <returns>Name of the file.</returns>
        internal string GetFileName (ulong fileID) {
            string sql = "select FileName from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file name: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal string GetFileAssumedName (ulong fileID) {
            string sql = "select AssumedFileName from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get assumed file name: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal string GetFilePassword (ulong fileID) {
            string sql = "select FilePassword from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file password: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal string GetFilePath (ulong fileID) {
            string sql = "select FilePath from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file path: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method returns the narration of a file stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which is sought.</param>
        /// <returns>Narration of the file.</returns>
        internal string GetFileNarration (ulong fileID) {
            string sql = "select FileNarration from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file narration: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method returns the size of a file stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the size of which is sought.</param>
        /// <returns>Size of the file.</returns>
        internal int GetFileSize (ulong fileID) {
            string sql = "select FileSize from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file size: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return int.Parse (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method returns the hash of a file stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the hash of which is sought.</param>
        /// <returns>Hash of the file.</returns>
        internal string GetFileHash (ulong fileID) {
            string sql = "select FileHash from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file hash: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method checks for file duplicates.
        /// </summary>
        /// <param name="fileID">Id of the file for which duplicate files are sought.</param>
        /// <returns>A dictionary of file ids along with their heuristic hit score.</returns>
        internal Dictionary<ulong, double> GetDuplicateFiles (ulong fileID) {
            int fileSize = GetFileSize (fileID);
            string fileHash = GetFileHash (fileID);

            // Though a very rare possibility, it could be that there may be a hash collision. So we first
            // check to see if the file size is the same, and only then do we check for the file hash. It's
            // a rare possibility that the file size *and* the file hash would be the same for two different
            // files. This also serves another purpose: In the query, we first check the file size (int
            // comparison) and only if that is the same should the query then do a hash (string) comparison.
            // Don't have much idea about how queries are parsed, but am assuming that the where clause is a
            // short-circuit operation.
            string sql = "select key_FileID from L_Files where key_FileID!=" + fileID + " and  FileSize=" + fileSize + " and FileHash='" + fileHash + "'";
            Debug.Print ("Get duplicate files: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);
                List<ulong> matchedFileIDs = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    string user = row[0].ToString ();
                    ulong matchedFileID = UInt64.Parse (row[0].ToString ());
                    matchedFileIDs.Add (matchedFileID);
                }
                if (matchedFileIDs.Count == 0) {
                    return new Dictionary<ulong, double> ();
                }

                Dictionary<ulong, double> opFilesScores = new Dictionary<ulong, double> ();
                string fileName = GetFileName (fileID);
                string fileNarration = GetFileNarration (fileID);

                foreach (ulong matchedFileID in matchedFileIDs) {
                    string otherFileName = GetFileName (matchedFileID);
                    string otherFileNarration = GetFileNarration (matchedFileID);
                    double score = 0.75;
                    if (fileName.Equals (otherFileName)) {
                        score += 0.15;
                    }
                    if (fileNarration.Equals (otherFileNarration)) {
                        score += 0.10;
                    }
                    opFilesScores.Add (matchedFileID, score);
                }

                return opFilesScores;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method returns the save date-time of a file stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which is sought.</param>
        /// <returns>A DateTime object that indicates what date-time the file was stored at.</returns>
        internal DateTime GetFileSaveDateTime (ulong fileID) {
            string sql = "select WhenDateTime from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file save date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                string dtTmStr = dt.Rows[0][0].ToString ();
                Debug.Print ("Got file save date-time: " + dtTmStr);
                Debug.Print ("Now parsing it to get DateTime object.");
                return (DateTime.Parse (dtTmStr));
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method returns the path of a file stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which is sought.</param>
        /// <returns>A string indicating the path of the file where it has been stored.</returns>
        internal string GetFileContainingDirPath (ulong fileID) {
            string sql = "select FilePath from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get file path: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal string GetFileArchiveName (ulong fileID) {
            string sql = "select ArchiveName from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get archive name: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method tells the caller if a file exists in the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which is sought.</param>
        /// <returns>A boolean value indicating whether the file exists in the system or not.</returns>
        internal bool DoesFileExist (ulong fileID) {
            string sql = "select FileSize from L_Files where key_FileID=" + fileID;
            Debug.Print ("Does file exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method deletes a file that is stored on the system.
        /// </summary>
        /// <param name="fileID">Id of the file that has to be deleted.</param>
        /// <returns>An integer value that indicates how many files have been deleted. Is always one.</returns>
        internal int DeleteFile (ulong fileID) {
            string sql = "delete from L_Files where key_FileID=" + fileID;
            Debug.Print ("Delete file: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                string referencedSql1 = "delete from M_Aspects_Files where fkey_FileID=" + fileID;
                string referencedSql2 = "delete from M_Files_Collections where fkey_FileID=" + fileID;
                string referencedSql3 = "delete from M_Files_Versions where fkey_FileID=" + fileID;
                string referencedSql4 = "delete from L_FileBookmarks where fkey_FileID=" + fileID;

                SQLiteCommand myCommand1 = new SQLiteCommand (referencedSql1, cnn);
                SQLiteCommand myCommand2 = new SQLiteCommand (referencedSql2, cnn);
                SQLiteCommand myCommand3 = new SQLiteCommand (referencedSql3, cnn);
                SQLiteCommand myCommand4 = new SQLiteCommand (referencedSql4, cnn);

                int val = myCommand.ExecuteNonQuery ();

                myCommand1.ExecuteNonQuery ();
                myCommand2.ExecuteNonQuery ();
                myCommand3.ExecuteNonQuery ();
                myCommand4.ExecuteNonQuery ();

                return val;
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

        /// <summary>
        /// This method sets the deletion date-time of a file.
        /// </summary>
        /// <param name="fileID">Id of the file the deletion date of which has to be set.</param>
        /// <param name="deletionDateTime">A DateTime object that specifies the deletion date-time for the file.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool SetDeletionDateTime (ulong fileID, DateTime deletionDate) {
            string deletionDateTimeForInsert = StringUtils.GetAsZeroPaddedFourCharString (deletionDate.Year) + "-"
                                                + StringUtils.GetAsZeroPaddedTwoCharString (deletionDate.Month) + "-"
                                                + StringUtils.GetAsZeroPaddedTwoCharString (deletionDate.Day) + " "
                                                + StringUtils.GetAsZeroPaddedTwoCharString (deletionDate.Hour) + ":"
                                                + StringUtils.GetAsZeroPaddedTwoCharString (deletionDate.Minute) + ":"
                                                + StringUtils.GetAsZeroPaddedTwoCharString (deletionDate.Second);

            string sql = "update L_Files set DeletionDateTime='" + deletionDateTimeForInsert + "' where key_FileID=" + fileID;
            Debug.Print ("Set deletion date-time: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method returns the deletion date-time of a file.
        /// </summary>
        /// <param name="fileID">Id of the file, the deletion date-time of which is sought.</param>
        /// <returns>A DateTime object that indicates the deletion date-time for this file.</returns>
        internal DateTime GetDeletionDateTime (ulong fileID) {
            string sql = "select DeletionDateTime from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get deletion date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                string dtTmStr = dt.Rows[0][0].ToString ();
                if (dtTmStr.Length < 1) {
                    return DateTime.MaxValue;
                }
                Debug.Print ("Got file deletion date-time: " + dtTmStr);
                Debug.Print ("Now parsing it to get DateTime object.");
                DateTime dateTime = DateTime.Parse (dtTmStr);
                // TODO: For some weird reason, sqlite reverts dates like
                // '9999-12-12 23:59:59.999' to '0001-01-01 00:00:00.000'
                // This in turn is converted again to date: '01-Jan-01 00:00:00'
                // or 2001-01-01, 12:00 am, zero secs and msecs.
                // Until this is solved, I will manually check for this date and
                // use that as the max date. Sigh! This still doesn't make sense.
                if (dateTime.Year == 2001 && dateTime.Month == 1 && dateTime.Day == 1 &&
                    dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0) {
                    Debug.Print ("Deliberately reverting to MAX_DATE!");
                    dateTime = DateTime.MaxValue;
                }

                return dateTime;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method updates the name of a file.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which has to be updated.</param>
        /// <param name="newName">New name of the file.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool UpdateFileName (ulong fileID, string newName) {
            string sql = "update L_Files set FileName=@FileName where key_FileID=" + fileID;
            Debug.Print ("Update file name: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@FileName", newName);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method updates the narration of a file.
        /// </summary>
        /// <param name="fileID">Id of the file, the narration of which has to be updated.</param>
        /// <param name="newNarration">New narration for the file.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool UpdateFileNarration (ulong fileID, string newNarration) {
            string sql = "update L_Files set FileNarration=@newNarration where key_FileID=" + fileID;
            Debug.Print ("Update file narration: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newNarration", newNarration);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method updates the save date-time of a file already saved within the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the save date-time of which has to be updated.</param>
        /// <param name="newDateTime">New save date-time for the file.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool UpdateFileSaveDateTime (ulong fileID, DateTime newDateTime) {
            string dtTmStr = StringUtils.GetAsZeroPaddedFourCharString (newDateTime.Year) + "-"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDateTime.Month) + "-"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDateTime.Day) + " "
                + StringUtils.GetAsZeroPaddedTwoCharString (newDateTime.Hour) + ":"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDateTime.Minute) + ":"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDateTime.Second);

            string sql = "update L_Files set WhenDateTime='" + dtTmStr + "' where key_FileID=" + fileID;
            Debug.Print ("Update file save date-time: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method updates the deletion date-time of a file already saved within the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the deletion date-time of which has to be updated.</param>
        /// <param name="maxDateTime">New deletion date-time for the file.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool UpdateFileDeletionDateTime (ulong fileID, DateTime newDeletionDateTime) {
            string dtTmStr = StringUtils.GetAsZeroPaddedFourCharString (newDeletionDateTime.Year) + "-"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDeletionDateTime.Month) + "-"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDeletionDateTime.Day) + " "
                + StringUtils.GetAsZeroPaddedTwoCharString (newDeletionDateTime.Hour) + ":"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDeletionDateTime.Minute) + ":"
                + StringUtils.GetAsZeroPaddedTwoCharString (newDeletionDateTime.Second);

            string sql = "update L_Files set DeletionDateTime='" + dtTmStr + "' where key_FileID=" + fileID;
            Debug.Print ("File deletion date-time: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method resets the deletion date-time value of a file.
        /// </summary>
        /// <param name="fileID">Id of the file, the deletion date-time of which has to be reset.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool ResetDeletionDateTime (ulong fileID) {
            string sql = "update L_Files set DeletionDateTime=null where key_FileID=" + fileID;
            Debug.Print ("Reset deletion date-time: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        #endregion

        #region << File Retrieval in Date/Time Ranges >>

        internal List<ulong> GetAllFiles () {
            string sql = "select key_FileID from L_Files";
            Debug.Print ("Get all files: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesInDateRange (DateTime startDate, DateTime endDate) {
            string start = "'" + StringUtils.GetAsZeroPaddedFourCharString (startDate.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDate.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDate.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + "'";

            string end = "'" + StringUtils.GetAsZeroPaddedFourCharString (endDate.Year) + "-"
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDate.Month) + "-"
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDate.Day) + " "
                             + StringUtils.GetAsZeroPaddedTwoCharString (23) + ":"
                             + StringUtils.GetAsZeroPaddedTwoCharString (59) + ":"
                             + StringUtils.GetAsZeroPaddedTwoCharString (59) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime between " + start + " and " + end;
            Debug.Print ("Get files in date range: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesInDateTimeRange (DateTime startDateTime, DateTime endDateTime) {
            string start = "'" + StringUtils.GetAsZeroPaddedFourCharString (startDateTime.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDateTime.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDateTime.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDateTime.Hour) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDateTime.Minute) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (startDateTime.Second) + "'";

            string end = "'" + StringUtils.GetAsZeroPaddedFourCharString (endDateTime.Year) + "-"
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDateTime.Month) + "-"
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDateTime.Day) + " "
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDateTime.Hour) + ":"
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDateTime.Minute) + ":"
                             + StringUtils.GetAsZeroPaddedTwoCharString (endDateTime.Second) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime between " + start + " and " + end;
            Debug.Print ("Get files in date-time range: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesOnDate (DateTime onDate) {
            string start = "'" + StringUtils.GetAsZeroPaddedFourCharString (onDate.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (onDate.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (onDate.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + "'";

            string end = "'" + StringUtils.GetAsZeroPaddedFourCharString (onDate.Year) + "-"
                             + StringUtils.GetAsZeroPaddedTwoCharString (onDate.Month) + "-"
                             + StringUtils.GetAsZeroPaddedTwoCharString (onDate.Day) + " "
                             + StringUtils.GetAsZeroPaddedTwoCharString (23) + ":"
                             + StringUtils.GetAsZeroPaddedTwoCharString (59) + ":"
                             + StringUtils.GetAsZeroPaddedTwoCharString (59) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime between " + start + " and " + end;
            Debug.Print ("Get files on date: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesOnDateTime (DateTime onDateTime) {
            string on = "'" + StringUtils.GetAsZeroPaddedFourCharString (onDateTime.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (onDateTime.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (onDateTime.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (onDateTime.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (onDateTime.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (onDateTime.Second) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime=" + on;
            Debug.Print ("Get files on date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesBeforeDate (DateTime beforeDate) {
            string before = "'" + StringUtils.GetAsZeroPaddedFourCharString (beforeDate.Year) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDate.Month) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDate.Day) + " "
                                + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (0) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime<" + before;
            Debug.Print ("Get files before date: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesBeforeDateTime (DateTime beforeDateTime) {
            string before = "'" + StringUtils.GetAsZeroPaddedFourCharString (beforeDateTime.Year) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDateTime.Month) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDateTime.Day) + " "
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDateTime.Hour) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDateTime.Minute) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeDateTime.Second) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime<" + before;
            Debug.Print ("Get files before date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesBeforeAndOnDate (DateTime beforeOnDate) {
            string before = "'" + StringUtils.GetAsZeroPaddedFourCharString (beforeOnDate.Year) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDate.Month) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDate.Day) + " "
                                + StringUtils.GetAsZeroPaddedTwoCharString (23) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (59) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (59) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime<=" + before;
            Debug.Print ("Get files before and on date: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesBeforeAndOnDateTime (DateTime beforeOnDateTime) {
            string before = "'" + StringUtils.GetAsZeroPaddedFourCharString (beforeOnDateTime.Year) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDateTime.Month) + "-"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDateTime.Day) + " "
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDateTime.Hour) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDateTime.Minute) + ":"
                                + StringUtils.GetAsZeroPaddedTwoCharString (beforeOnDateTime.Second) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime<=" + before;
            Debug.Print ("Get files before and on date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesAfterDate (DateTime afterDate) {
            string after = "'" + StringUtils.GetAsZeroPaddedFourCharString (afterDate.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDate.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDate.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (23) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (59) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (59) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime>" + after;
            Debug.Print ("Get files after date: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesAfterDateTime (DateTime afterDateTime) {
            string after = "'" + StringUtils.GetAsZeroPaddedFourCharString (afterDateTime.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDateTime.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDateTime.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDateTime.Hour) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDateTime.Minute) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterDateTime.Second) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime>" + after;
            Debug.Print ("Get files after date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesAfterAndOnDate (DateTime afterOnDate) {
            string after = "'" + StringUtils.GetAsZeroPaddedFourCharString (afterOnDate.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDate.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDate.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (0) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime>=" + after;
            Debug.Print ("Get files after and on date: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesAfterAndOnDateTime (DateTime afterOnDateTime) {
            string after = "'" + StringUtils.GetAsZeroPaddedFourCharString (afterOnDateTime.Year) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDateTime.Month) + "-"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDateTime.Day) + " "
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDateTime.Hour) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDateTime.Minute) + ":"
                               + StringUtils.GetAsZeroPaddedTwoCharString (afterOnDateTime.Second) + "'";

            string sql = "select key_FileID from L_Files where WhenDateTime>=" + after;
            Debug.Print ("Get files after and on date-time: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Aspect-related Operations >>

        /// <summary>
        /// This method creates a new aspect within the system.
        /// </summary>
        /// <param name="aspectName">Name of the new aspect.</param>
        /// <param name="aspectDesc">Description of the new aspect.</param>
        /// <returns>An id that uniquely identifies the aspect across the entire system, if the call was successful.</returns>
        internal ulong CreateAspect (string aspectName, string aspectDesc) {
            string sql = "insert into L_Aspects (AspectName, AspectDesc) values (@aspectName, @aspectDesc); "
                            + "select last_insert_rowid() from L_Aspects";
            Debug.Print ("Create aspect: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@aspectName", aspectName);
                myCommand.Parameters.AddWithValue ("@aspectDesc", aspectDesc);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        /// <summary>
        /// This method gets the aspect name associated with an aspect id.
        /// </summary>
        /// <param name="aspectID">The aspect id for which this information is sought.</param>
        /// <returns>Name of the aspect sought.</returns>
        internal void GetAspectNameAndDesc (ulong aspectID, out string aspectName, out string aspectDesc) {
            string sql = "select AspectName, AspectDesc from L_Aspects where key_AspectID=" + aspectID;
            Debug.Print ("Get aspect name and desc: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                aspectName = dt.Rows[0][0].ToString ();
                aspectDesc = dt.Rows[0][1].ToString ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetAllAspects () {
            string sql = "select key_AspectID from L_Aspects";
            Debug.Print ("Get all aspects: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allAspects = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong briefcaseID = ulong.Parse (row[0].ToString ());
                    allAspects.Add (briefcaseID);
                }

                return allAspects;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method updates the name of an existing aspect.
        /// </summary>
        /// <param name="aspectID">The aspect id, the name of which has to be updated.</param>
        /// <param name="newAspectName">New aspect name.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        internal bool UpdateAspectName (ulong aspectID, string newAspectName) {
            string sql = "update L_Aspects set AspectName=@newAspectName where key_AspectID=" + aspectID;
            Debug.Print ("Update aspect name: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newAspectName", newAspectName);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method updates the description of an existing aspect.
        /// </summary>
        /// <param name="aspectID">The aspect id, the name of which has to be updated.</param>
        /// <param name="newAspectDesc">New aspect description.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        internal bool UpdateAspectDesc (ulong aspectID, string newAspectDesc) {
            string sql = "update L_Aspects set AspectDesc=@newAspectDesc where key_AspectID=" + aspectID;
            Debug.Print ("Update aspect desc: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newAspectDesc", newAspectDesc);
                
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method tells the client whether an aspect exists or not.
        /// </summary>
        /// <param name="aspectID">The aspect id for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this aspect exists or not.</returns>
        internal bool DoesAspectExist (ulong aspectID) {
            string sql = "select key_AspectID from L_Aspects where key_AspectID=" + aspectID;
            Debug.Print ("Does aspect exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method tells the client whether an aspect exists or not.
        /// </summary>
        /// <param name="aspectName">The aspect name for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this aspect exists or not.</returns>
        internal bool DoesAspectExist (string aspectName) {
            string sql = "select key_AspectID from L_Aspects where AspectName=@aspectName";
            Debug.Print ("Does aspect exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@aspectName", aspectName);
                
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method deletes an aspect.
        /// </summary>
        /// <param name="aspectID">Id of the aspect that has to be deleted.</param>
        /// <returns>An integer value that indicates how many aspects have been deleted. Is always one.</returns>
        internal int DeleteAspect (ulong aspectID) {
            string sql = "delete from L_Aspects where key_AspectID=" + aspectID;
            Debug.Print ("Delete aspect: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int val = myCommand.ExecuteNonQuery ();

                string referencedQuery1 = "delete from M_Aspects_Files where fkey_AspectID=" + aspectID;
                SQLiteCommand myCommand1 = new SQLiteCommand (referencedQuery1, cnn);
                myCommand1.ExecuteNonQuery ();

                return val;
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

        internal ulong GetAspectIDFromName (string aspectName) {
            string sql = "select key_AspectID from L_Aspects where AspectName=@aspectName";
            Debug.Print ("Get aspect ID: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@aspectName", aspectName);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return UInt64.Parse ((dt.Rows[0][0]).ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// Be careful while using this method. It deletes ALL saved aspects within the system.
        /// </summary>
        /// <returns>Total number of aspects deleted.</returns>
        internal int DeleteAllAspectsInSystem () {
            string sql = "delete from L_Aspects where key_AspectID>0";
            Debug.Print ("Delete all aspects: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int val = myCommand.ExecuteNonQuery ();

                string referencedQuery1 = "delete from M_Aspects_Files";
                SQLiteCommand myCommand1 = new SQLiteCommand (referencedQuery1, cnn);
                myCommand1.ExecuteNonQuery ();

                return val;
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

        #endregion

        #region << Aspect Group-related Operations >>

        internal ulong CreateAspectGroup (ulong parentAspectGroupID, string aspectGroupName, string aspectGroupDesc) {
            string sql = "insert into L_AspectGroups (fkey_ParentAspectGroupID, AspectGroupName, AspectGroupDesc) values (" + parentAspectGroupID + ", @aspectGroupName, @aspectGroupDesc); "
                            + "select last_insert_rowid() from L_AspectGroups";
            Debug.Print ("Create aspect group: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@aspectGroupName", aspectGroupName);
                myCommand.Parameters.AddWithValue ("@aspectGroupDesc", aspectGroupDesc);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        internal bool DoesAspectGroupExistAtLevel (ulong parentAspectGroupID, string aspectGroupName) {
            string sql = "select AspectGroupName from L_AspectGroups where fkey_ParentAspectGroupID=" + parentAspectGroupID + " and AspectGroupName=@aspectGroupName";
            Debug.Print ("Does aspect group exist at level: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@aspectGroupName", aspectGroupName);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return dt.Rows.Count > 0;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal void GetAspectGroupNameAndDesc (ulong aspectGroupID, out string aspectGroupName, out string aspectGroupDesc) {
            string sql = "select AspectGroupName, AspectGroupDesc from L_AspectGroups where key_AspectGroupID=" + aspectGroupID;
            Debug.Print ("Get aspect group name and desc: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                aspectGroupName = dt.Rows[0][0].ToString ();
                aspectGroupDesc = dt.Rows[0][1].ToString ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool DoesAspectGroupExist (ulong aspectGroupID) {
            string sql = "select key_AspectGroupID from L_AspectGroups where key_AspectGroupID=" + aspectGroupID;
            Debug.Print ("Does aspect group exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetChildAspectGroups (ulong parentAspectID) {
            string sql = "select key_AspectGroupID from L_AspectGroups where fkey_ParentAspectGroupID=" + parentAspectID;
            Debug.Print ("Get child aspect groups: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> childAspectGroups = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong aspectGroupID = ulong.Parse (row[0].ToString ());
                    childAspectGroups.Add (aspectGroupID);
                }

                return childAspectGroups;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int GetNumAspectsInAspectGroup (ulong aspectGroupID) {
            string sql = "select count(key_AspectID) from L_Aspects where fkey_AspectGroupID=" + aspectGroupID;
            Debug.Print ("Get number of aspects in aspect group: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return Int32.Parse (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int GetNumAspectGroupsInAspectGroup (ulong aspectGroupID) {
            string sql = "select count(key_AspectGroupID) from L_AspectGroups where fkey_ParentAspectGroupID=" + aspectGroupID;
            Debug.Print ("Get number of aspect groups in aspect group: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return Int32.Parse (dt.Rows[0][0].ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int DeleteAspectGroup (ulong aspectGroupID) {
            string sql = "delete from L_AspectGroups where key_AspectGroupID=" + aspectGroupID;
            Debug.Print ("Delete aspect group: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        #endregion

        #region << File Version-related Operations >>

        internal void SaveAsNextVersion (ulong fileID, string fileHash, string comments, string fullyQualifiedPath, int versionNumber) {
            DateTime when = DateTime.Now;

            // TODO: Use StringBuilder class.
            string insertDateTime = StringUtils.GetAsZeroPaddedFourCharString (when.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Second);

            string sql = "insert into M_Files_Versions (fkey_FileID, VersionNumber, FileHash, Comments, ArchiveNameWithPath, WhenDateTime) values ("
                                + fileID + ", " + versionNumber + ", '" + fileHash + "', @comments, '" + fullyQualifiedPath + "', '"
                                + insertDateTime + "')";
            Debug.Print ("Save as next version: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@comments", comments);

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
        
        internal int GetLastFileVersionNumber (ulong fileID) {
            string sql = "select max(VersionNumber) from M_Files_Versions where fkey_FileID=" + fileID;
            Debug.Print ("Get last file version: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);
                Debug.Print ("Got num rows: " + dt.Rows.Count);

                int lastVersionNumber = 0;
                if (dt.Rows.Count > 0) {
                    string val = dt.Rows[0][0].ToString ();
                    Debug.Print ("Got from row: " + val);
                    try {
                        lastVersionNumber = int.Parse (val);
                    } catch (Exception e) {
                        // For some weird reason, sqlite actually returns a row, even for
                        // no entries in table. Control will reach here, i.e., exception
                        // will get thrown the first time (when there are no file versions).
                        // So just catch it and ignore it.
                        Debug.Print ("Caught error, just ignore this: " + e.Message);
                    }
                }

                return lastVersionNumber;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal string GetFileVersionHash (ulong fileID, int versionNumber) {
            string sql = "select FileHash from M_Files_Versions where fkey_FileID=" + fileID + " and VersionNumber=" + versionNumber;
            Debug.Print ("Get version hash for file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                string fileHash = dt.Rows[0][0].ToString ();
                Debug.Print (fileHash);
                return fileHash;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool DoesFileVersionExist (ulong fileID, int versionNumber) {
            string sql = "select max(VersionNumber) from M_Files_Versions where fkey_FileID=" + fileID;
            Debug.Print ("Does file version exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);
                Debug.Print ("Got num rows: " + dt.Rows.Count);

                int lastVersionNumber = 0;
                if (dt.Rows.Count > 0) {
                    string val = dt.Rows[0][0].ToString ();
                    Debug.Print ("Got from row: " + val);
                    try {
                        lastVersionNumber = int.Parse (val);
                    } catch (Exception e) {
                        // For some weird reason, sqlite actually returns a row, even for
                        // no entries in table. Control will reach here, i.e., exception
                        // will get thrown the first time (when there are no file versions).
                        // So just catch it and ignore it.
                        Debug.Print ("Caught error, just ignore this: " + e.Message);
                    }
                }

                return (versionNumber <= lastVersionNumber);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<string> GetAllVersionsPathsForFile (ulong fileID) {
            string sql = "select ArchiveNameWithPath from M_Files_Versions where fkey_FileID=" + fileID;
            Debug.Print ("Get all version paths for file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<string> versionedFileNamesWithPath = new List<string> ();
                foreach (DataRow row in dt.Rows) {
                    string fileNameWithPath = row[0].ToString ();
                    Debug.Print (fileNameWithPath);
                    versionedFileNamesWithPath.Add (fileNameWithPath);
                }

                return versionedFileNamesWithPath;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal void GetFileVersionPath (ulong fileID, int versionNumber, out string fileNameWithPath) {
            string sql = "select ArchiveNameWithPath from M_Files_Versions where fkey_FileID=" + fileID + " and VersionNumber=" + versionNumber;
            Debug.Print ("Get version path for file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                fileNameWithPath = dt.Rows[0][0].ToString ();
                Debug.Print (fileNameWithPath);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal void GetFileVersionDetails (ulong fileID, int versionNumber, out string comments, out DateTime whenDateTime) {
            string sql = "select Comments, WhenDateTime from M_Files_Versions where fkey_FileID=" + fileID + " and VersionNumber=" + versionNumber;
            Debug.Print ("Get version details for file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                comments = dt.Rows[0][0].ToString ();
                Debug.Print (comments);
                string dtTmStr = dt.Rows[0][1].ToString ();
                whenDateTime = DateTime.Parse (dtTmStr);
                Debug.Print (whenDateTime.ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal void GetFileVersionHistoryLog (ulong fileID, out string[] versionComments, out DateTime[] versionDateTimes) {
            string sql = "select Comments, WhenDateTime from M_Files_Versions where fkey_FileID=" + fileID;
            Debug.Print ("Get version history log for file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                versionComments = new string[dt.Rows.Count];
                versionDateTimes = new DateTime[dt.Rows.Count];

                int count = 0;
                foreach (DataRow row in dt.Rows) {
                    versionComments[count] = row[0].ToString ();
                    string dtTmStr = row[1].ToString ();
                    versionDateTimes[count++] = DateTime.Parse (dtTmStr);
                }
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Files-Aspects Operations >>

        /// <summary>
        /// This method links a file to an aspect.
        /// </summary>
        /// <param name="aspectID">Aspect to be applied.</param>
        /// <param name="fileID">File to be applied aspect to.</param>
        /// <returns>A boolean value indicating if the operation was successful.</returns>
        internal bool ApplyAspectToFile (ulong aspectID, ulong fileID) {
            string sql = "insert into M_Aspects_Files (fkey_AspectID, fkey_FileID) values (" + aspectID + ", " + fileID + ")";
            Debug.Print ("Apply aspect to file: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        /// <summary>
        /// This method tells the caller if an aspect has been applied to a file.
        /// </summary>
        /// <param name="aspectID">Aspect to be checked for being applied.</param>
        /// <param name="fileID">File to be checked for being applied to.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        internal bool IsAspectAppliedToFile (ulong aspectID, ulong fileID) {
            string sql = "select * from M_Aspects_Files where fkey_AspectID=" + aspectID + " and fkey_FileID=" + fileID;
            Debug.Print ("Is aspect applied to file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method "unapplies" an aspect from a file.
        /// </summary>
        /// <param name="aspectID">Aspect to be "unapplied."</param>
        /// <param name="fileID">File to be "unapplied" from.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        internal bool UnapplyAspectFromFile (ulong aspectID, ulong fileID) {
            string sql = "delete from M_Aspects_Files where fkey_AspectID=" + aspectID + " and fkey_FileID=" + fileID;
            Debug.Print ("Unapply aspect from file: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        /// <summary>
        /// This method "unapplies" all aspects from a file.
        /// </summary>
        /// <param name="fileID">File to be "unapplied" from.</param>
        /// <returns>An integer value that indicates how many aspects were "unapplied."</returns>
        internal int UnapplyAllAspectsFromFile (ulong fileID) {
            string sql = "delete from M_Aspects_Files where fkey_FileID=" + fileID;
            Debug.Print ("Unapply all aspects from file: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        /// <summary>
        /// This method "unapplies" an aspect from all files.
        /// </summary>
        /// <param name="aspectID">Aspect to be "unapplied."</param>
        /// <returns>An integer value that indicates how many files were "unapplied" from.</returns>
        internal int UnapplyAspectFromAllFiles (ulong aspectID) {
            string sql = "delete from M_Aspects_Files where fkey_AspectID=" + aspectID;
            Debug.Print ("Unapply aspect from all files: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        /// <summary>
        /// This method gets all the aspects aspects to a file.
        /// </summary>
        /// <param name="fileID">File for which applied aspects are sought.</param>
        /// <returns>List of all aspects applied to file.</returns>
        internal List<ulong> GetAspectsAppliedOnFile (ulong fileID) {
            string sql = "select fkey_AspectID from M_Aspects_Files where fkey_FileID=" + fileID;
            Debug.Print ("Get aspects applied on file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allAspects = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong aspectID = ulong.Parse (row[0].ToString ());
                    allAspects.Add (aspectID);
                }

                return allAspects;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method returns all the files that have been applied an aspect.
        /// </summary>
        /// <param name="aspectID">Aspect for which applied files are sought.</param>
        /// <returns>List of all files applied to.</returns>
        internal List<ulong> GetFilesAppliedWithAspect (ulong aspectID) {
            string sql = "select fkey_FileID from M_Aspects_Files where fkey_AspectID=" + aspectID;
            Debug.Print ("Get files applied with aspect: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Briefcase-related Operations >>

        /// <summary>
        /// This method creates a new briefcase within the system.
        /// </summary>
        /// <param name="briefcaseName">Name of the new briefcase.</param>
        /// <param name="briefcaseDesc">Description of the new briefcase.</param>
        /// <returns>An id that uniquely identifies the briefcase across the entire system, if the call was successful.</returns>
        internal ulong CreateBriefcase (string briefcaseName, string briefcaseDesc) {
            string sql = "insert into L_Briefcases (BriefcaseName, BriefcaseDesc) values (@briefcaseName, @briefcaseDesc); "
                            + "select last_insert_rowid() from L_Briefcases";
            Debug.Print ("Create briefcase: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@briefcaseName", briefcaseName);
                myCommand.Parameters.AddWithValue ("@briefcaseDesc", briefcaseDesc);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        /// <summary>
        /// This method deletes a briefcase.
        /// </summary>
        /// <param name="briefcaseID">d of the briefcase that has to be deleted.</param>
        /// <returns>An integer value that indicates how many briefcases have been deleted. Is always one.</returns>
        internal int DeleteBriefcase (ulong briefcaseID) {
            string sql = "delete from L_Briefcases where key_BriefcaseID=" + briefcaseID;
            Debug.Print ("Delete briefcase: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int val = myCommand.ExecuteNonQuery ();

                string referencedQuery1 = "update L_Files set fkey_BriefcaseID=1 where fkey_BriefcaseID=" + briefcaseID;
                SQLiteCommand myCommand1 = new SQLiteCommand (referencedQuery1, cnn);
                myCommand1.ExecuteNonQuery ();

                return val;
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

        /// <summary>
        /// This method tells the client whether a briefcase exists or not.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this briefcase exists or not.</returns>
        internal bool DoesBriefcaseExist (ulong briefcaseID) {
            string sql = "select key_BriefcaseID from L_Briefcases where key_BriefcaseID=" + briefcaseID;
            Debug.Print ("Does briefcase exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method tells the client whether a briefcase exists or not.
        /// </summary>
        /// <param name="briefcaseName">The briefcase name for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this briefcase exists or not.</returns>
        internal bool DoesBriefcaseExist (string briefcaseName) {
            string sql = "select key_BriefcaseID from L_Briefcases where BriefcaseName=@briefcaseName";
            Debug.Print ("Does briefcase exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@briefcaseName", briefcaseName);
                
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method gets the briefcase name associated with a briefcase id.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id for which this information is sought.</param>
        /// <returns>Name of the briefcase sought.</returns>
        internal void GetBriefcaseNameAndDesc (ulong briefcaseID, out string briefcaseName, out string briefcaseDesc) {
            string sql = "select BriefcaseName, BriefcaseDesc from L_Briefcases where key_BriefcaseID=" + briefcaseID;
            Debug.Print ("Get briefcase name: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                briefcaseName = dt.Rows[0][0].ToString ();
                briefcaseDesc = dt.Rows[0][1].ToString ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal ulong GetBriefcaseIDFromName (string briefcaseName) {
            string sql = "select key_BriefcaseID from L_Briefcases where BriefcaseName=@briefcaseName";
            Debug.Print ("Get briefcase ID: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@briefcaseName", briefcaseName);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return UInt64.Parse ((dt.Rows[0][0]).ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetAllBriefcases () {
            string sql = "select key_BriefcaseID from L_Briefcases";
            Debug.Print ("Get all briefcases: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allBriefcases = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong briefcaseID = ulong.Parse (row[0].ToString ());
                    allBriefcases.Add (briefcaseID);
                }

                return allBriefcases;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method updates the name of an existing briefcase.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id, the name of which has to be updated.</param>
        /// <param name="newBriefcaseName">New briefcase name.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        internal bool UpdateBriefcaseName (ulong briefcaseID, string newBriefcaseName) {
            string sql = "update L_Briefcases set BriefcaseName=@newBriefcaseName where key_BriefcaseID=" + briefcaseID;
            Debug.Print ("Update briefcase name: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newBriefcaseName", newBriefcaseName);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method updates the description of an existing briefcase.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id, the name of which has to be updated.</param>
        /// <param name="newBriefcaseDesc">New briefcase description.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        internal bool UpdateBriefcaseDesc (ulong briefcaseID, string newBriefcaseDesc) {
            string sql = "update L_Briefcases set BriefcaseDesc=@newBriefcaseDesc where key_BriefcaseID=" + briefcaseID;
            Debug.Print ("Update briefcase desc: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newBriefcaseDesc", newBriefcaseDesc);
                
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// Be careful while using this method. It deletes ALL saved briefcases within the system.
        /// </summary>
        /// <returns>Total number of briefcases deleted.</returns>
        internal int DeleteAllBriefcasesInSystem () {
            string sql = "delete from L_Briefcases where key_BriefcaseID <> 1";
            Debug.Print ("Delete all briefcases: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int val = myCommand.ExecuteNonQuery ();

                string referencedSql1 = "update L_Files set fkey_BriefcaseID=1";
                SQLiteCommand myCommand1 = new SQLiteCommand (referencedSql1, cnn);
                myCommand1.ExecuteNonQuery ();

                return val;
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

        #endregion

        #region << Files-Briefcases Addition / Removal Operations >>

        /// <summary>
        /// This method returns the briefcase id that contains a file.
        /// </summary>
        /// <param name="fileID">File for which this information is sought.</param>
        /// <returns>Id of the briefcase.</returns>
        internal ulong GetContainingBriefcase (ulong fileID) {
            string sql = "select fkey_BriefcaseID from L_Files where key_FileID=" + fileID;
            Debug.Print ("Get containing briefcase: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method moves a file to a briefcase.
        /// </summary>
        /// <param name="fileID">Id of the file to be moved.</param>
        /// <param name="briefcaseID">Id of the briefcase to be moved to.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        internal bool MoveFileToBriefcase (ulong fileID, ulong briefcaseID) {
            string sql = "update L_Files set fkey_BriefcaseID=" + briefcaseID + " where key_FileID=" + fileID;
            Debug.Print ("Move file to briefcase: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method removes a file from its containing briefcase to the global briefcase.
        /// </summary>
        /// <param name="fileID">Id of the file to be moved.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        internal bool RemoveFileFromBriefcase (ulong fileID) {
            string sql = "update L_Files set fkey_BriefcaseID=1 where key_FileID=" + fileID;
            Debug.Print ("Remove file from briefcase: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method gets all the files in a briefcase.
        /// </summary>
        /// <param name="briefcaseID">Id of the briefcase from which its contained files are sought.</param>
        /// <returns>A list of files that are contained within the briefcase.</returns>
        internal List<ulong> GetFilesInBriefcase (ulong briefcaseID) {
            string sql = "select key_FileID from L_Files where fkey_BriefcaseID=" + briefcaseID;
            Debug.Print ("Get all files in briefcase: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> filesInBriefcase = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    filesInBriefcase.Add (ulong.Parse (row[0].ToString ()));
                }

                return filesInBriefcase;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Collection-related Operations >>

        /// <summary>
        /// This method creates a new collection.
        /// </summary>
        /// <param name="collectionName">Name of the new collection.</param>
        /// <param name="collectionDesc">Description of the new collection.</param>
        /// <returns>A unique integer that identifies the collection across the entire system.</returns>
        internal ulong CreateCollection (string collectionName, string collectionDesc) {
            string sql = "insert into L_Collections (CollectionName, CollectionDesc) values (@collectionName, @collectionDesc); "
                            + "select last_insert_rowid() from L_Collections";
            Debug.Print ("Create collection: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@collectionName", collectionName);
                myCommand.Parameters.AddWithValue ("@collectionDesc", collectionDesc);
                
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        /// <summary>
        /// This method deletes a collection.
        /// </summary>
        /// <param name="collectionID">The collection to be deleted.</param>
        /// <returns>Number of collections deleted. Always one.</returns>
        internal int DeleteCollection (ulong collectionID) {
            string sql = "delete from L_Collections where key_CollectionID=" + collectionID;
            Debug.Print ("Delete collection: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int val = myCommand.ExecuteNonQuery ();

                string referencedQuery1 = "delete from M_Files_Collections where fkey_CollectionID=" + collectionID;
                SQLiteCommand myCommand1 = new SQLiteCommand (referencedQuery1, cnn);
                myCommand1.ExecuteNonQuery ();

                return val;
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

        /// <summary>
        /// This method tells the client whether a collection exists or not.
        /// </summary>
        /// <param name="collectionID">The collection id for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this collection exists or not.</returns>
        internal bool DoesCollectionExist (ulong collectionID) {
            string sql = "select key_CollectionID from L_Collections where key_CollectionID=" + collectionID;
            Debug.Print ("Does collection exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method tells the client whether a collection exists or not.
        /// </summary>
        /// <param name="collectionName">The collection name for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this collection exists or not.</returns>
        internal bool DoesCollectionExist (string collectionName) {
            string sql = "select key_CollectionID from L_Collections where CollectionName=@collectionName";
            Debug.Print ("Does collection exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@collectionName", collectionName);
                
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method retrieves a collection's name.
        /// </summary>
        /// <param name="collectionID">Id of the collection the name of which is sought.</param>
        /// <returns>Name of the collection.</returns>
        internal void GetCollectionNameAndDesc (ulong collectionID, out string collectionName, out string collectionDesc) {
            string sql = "select CollectionName, CollectionDesc from L_Collections where key_CollectionID=" + collectionID;
            Debug.Print ("Get collection name: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                collectionName = dt.Rows[0][0].ToString ();
                collectionDesc = dt.Rows[0][1].ToString ();
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal ulong GetCollectionIDFromName (string collectionName) {
            string sql = "select key_CollectionID from L_Collections where CollectionName=@collectionName";
            Debug.Print ("Get collection ID: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@collectionName", collectionName);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return UInt64.Parse ((dt.Rows[0][0]).ToString ());
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetAllCollections () {
            string sql = "select key_CollectionID from L_Collections";
            Debug.Print ("Get all collections: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allCollections = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong collectionID = ulong.Parse (row[0].ToString ());
                    allCollections.Add (collectionID);
                }

                return allCollections;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        /// <summary>
        /// This method updates a collection's name.
        /// </summary>
        /// <param name="collectionID">Id of the collection the name of which is to be updated.</param>
        /// <param name="newCollectionName">New name for the collection.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool UpdateCollectionName (ulong collectionID, string newCollectionName) {
            string sql = "update L_Collections set CollectionName=@newCollectionName where key_CollectionID=" + collectionID;
            Debug.Print ("Update collection name: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newCollectionName", newCollectionName);
                
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// This method updates the description of a collection.
        /// </summary>
        /// <param name="collectionID">Id of the collection the description of which is to be updated.</param>
        /// <param name="newCollectionDesc">New description for the collection.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        internal bool UpdateCollectionDesc (ulong collectionID, string newCollectionDesc) {
            string sql = "update L_Collections set CollectionDesc=@newCollectionDesc where key_CollectionID=" + collectionID;
            Debug.Print ("Update collection desc: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@newCollectionDesc", newCollectionDesc);
                
                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        /// <summary>
        /// Be careful while using this method. It deletes ALL saved collections within the system.
        /// </summary>
        /// <returns>Total number of collections deleted.</returns>
        internal int DeleteAllCollectionsInSystem () {
            string sql = "delete from L_Collections where key_CollectionID>0";
            Debug.Print ("Delete all collections: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                int val = myCommand.ExecuteNonQuery ();
                
                string referencedQuery1 = "delete from M_Files_Collections";
                SQLiteCommand myCommand1 = new SQLiteCommand (referencedQuery1, cnn);
                myCommand1.ExecuteNonQuery ();

                return val;
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

        #endregion

        #region << Files-Collections Addition / Removal Operations >>

        internal bool AddFileToCollection (ulong fileID, ulong collectionID) {
            string sql = "insert into M_Files_Collections (fkey_FileID, fkey_CollectionID) values (" + fileID + ", " + collectionID + ")";
            Debug.Print ("Add file to collection: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal bool IsFileInCollection (ulong fileID, ulong collectionID) {
            string sql = "select * from M_Files_Collections where fkey_FileID=" + fileID + " and fkey_CollectionID=" + collectionID;
            Debug.Print ("Is file in collection: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool RemoveFileFromCollection (ulong fileID, ulong collectionID) {
            string sql = "delete from M_Files_Collections where fkey_FileID=" + fileID + " and fkey_CollectionID=" + collectionID;
            Debug.Print ("Remove file from collection: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal List<ulong> GetCollectionsWithFile (ulong fileID) {
            string sql = "select fkey_CollectionID from M_Files_Collections where fkey_FileID=" + fileID;
            Debug.Print ("Get collections with file: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allCollections = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong aspectID = ulong.Parse (row[0].ToString ());
                    allCollections.Add (aspectID);
                }

                return allCollections;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetFilesInCollection (ulong collectionID) {
            string sql = "select fkey_FileID from M_Files_Collections where fkey_CollectionID=" + collectionID;
            Debug.Print ("Get files in collection: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int RemoveFileFromAllCollections (ulong fileID) {
            string sql = "delete from M_Files_Collections where fkey_FileID=" + fileID;
            Debug.Print ("Remove file from all collections: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        internal int RemoveAllFilesFromCollection (ulong collectionID) {
            string sql = "delete from M_Files_Collections where fkey_CollectionID=" + collectionID;
            Debug.Print ("Remove all files from collection: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        #endregion

        #region << Url-related Operations >>

        internal ulong AddUrl (string url, string description, DateTime when) {
            string insertDateTime = StringUtils.GetAsZeroPaddedFourCharString (when.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Second);

            string sql = "insert into L_Urls (Url, Description, WhenDateTime) values (@url, @description, '" + insertDateTime + "'); "
                            + "select last_insert_rowid() from L_Urls";
            Debug.Print ("Add Url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@url", url);
                myCommand.Parameters.AddWithValue ("@description", description);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        internal void GetUrlDetails (ulong urlID, out string url, out string description, out DateTime when) {
            string sql = "select Url, Description, WhenDateTime from L_Urls where key_UrlID=" + urlID;
            Debug.Print ("Get url details: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                url = dt.Rows[0][0].ToString ();
                description = dt.Rows[0][1].ToString ();
                string dtTmStr = dt.Rows[0][2].ToString ();
                Debug.Print ("Got url save date: " + dtTmStr);
                Debug.Print ("Now parsing it to get DateTime object.");
                when = DateTime.Parse (dtTmStr);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool UpdateUrl (ulong urlID, string newUrl) {
            string sql = "update L_Urls set Url=@url where key_UrlID=" + urlID;
            Debug.Print ("Update Url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@url", newUrl);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        internal bool UpdateUrlDescription (ulong urlID, string newDescription) {
            string sql = "update L_Urls set Description=@description where key_UrlID=" + urlID;
            Debug.Print ("Update Url Description: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@description", newDescription);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        internal bool UpdateUrlDateTime (ulong urlID, DateTime newWhen) {
            string newInsertDateTime = StringUtils.GetAsZeroPaddedFourCharString (newWhen.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (newWhen.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (newWhen.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (newWhen.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (newWhen.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (newWhen.Second);
            string sql = "update L_Urls set WhenDateTime='" + newInsertDateTime + "' where key_UrlID=" + urlID;
            Debug.Print ("Update Url Description: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();

                return (updatedRows > 0);
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

        internal int DeleteUrl (ulong urlID) {
            string sql = "delete from L_Urls where key_UrlID=" + urlID;
            Debug.Print ("Remove url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                string referencedSql1 = "delete from M_Aspects_Urls where fkey_UrlID=" + urlID;
                string referencedSql2 = "delete from L_UrlBookmarks where fkey_UrlID=" + urlID;

                SQLiteCommand myCommand1 = new SQLiteCommand (referencedSql1, cnn);
                SQLiteCommand myCommand2 = new SQLiteCommand (referencedSql2, cnn);

                int val = myCommand.ExecuteNonQuery ();

                myCommand1.ExecuteNonQuery ();
                myCommand2.ExecuteNonQuery ();
                
                return val;
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

        internal bool DoesUrlExist (ulong urlID) {
            string sql = "select Url from L_Urls where key_UrlID=" + urlID;
            Debug.Print ("Does url exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Note-related Operations >>

        internal ulong AddNote (MfsNote note) {
            DateTime when = note.NoteDateTime;
            string insertDateTime = StringUtils.GetAsZeroPaddedFourCharString (when.Year) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Month) + "-"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Day) + " "
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Hour) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Minute) + ":"
                                    + StringUtils.GetAsZeroPaddedTwoCharString (when.Second);

            string sql = "insert into L_Notes (NoteContent, WhenDateTime) values (@noteContent, '" + insertDateTime + "'); "
                            + "select last_insert_rowid() from L_Notes";
            Debug.Print ("Add Note: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                myCommand.Parameters.AddWithValue ("@noteContent", note.NoteContent);

                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (ulong.Parse (dt.Rows[0][0].ToString ()));
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

        internal int DeleteNote (ulong noteID) {
            string sql = "delete from L_Notes where key_NoteID=" + noteID;
            Debug.Print ("Remove note: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                string referencedSql1 = "delete from M_Aspects_Notes where fkey_NoteID=" + noteID;
                string referencedSql2 = "delete from L_NoteBookmarks where fkey_NoteID=" + noteID;

                SQLiteCommand myCommand1 = new SQLiteCommand (referencedSql1, cnn);
                SQLiteCommand myCommand2 = new SQLiteCommand (referencedSql2, cnn);

                int val = myCommand.ExecuteNonQuery ();

                myCommand1.ExecuteNonQuery ();
                myCommand2.ExecuteNonQuery ();

                return val;
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

        internal MfsNote GetNote (ulong noteID) {
            string sql = "select NoteContent, WhenDateTime from L_Notes where key_NoteID=" + noteID;
            Debug.Print ("Get note: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                string noteContent = dt.Rows[0][0].ToString ();
                string dtTmStr = dt.Rows[0][1].ToString ();
                Debug.Print ("Got note save date: " + dtTmStr);
                Debug.Print ("Now parsing it to get DateTime object.");
                return (new MfsNote (noteContent, DateTime.Parse (dtTmStr)));
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal DateTime GetNoteDateTime (ulong noteID) {
            string sql = "select WhenDateTime from L_Notes where key_NoteID=" + noteID;
            Debug.Print ("Get note save date: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                string dtTmStr = dt.Rows[0][0].ToString ();
                Debug.Print ("Got note save date: " + dtTmStr);
                Debug.Print ("Now parsing it to get DateTime object.");
                return (DateTime.Parse (dtTmStr));
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool DoesNoteExist (ulong noteID) {
            string sql = "select NoteContent from L_Notes where key_NoteID=" + noteID;
            Debug.Print ("Does note exist: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Urls-Aspects Operations >>

        internal bool ApplyAspectToUrl (ulong aspectID, ulong urlID) {
            string sql = "insert into M_Aspects_Urls (fkey_AspectID, fkey_UrlID) values (" + aspectID + ", " + urlID + ")";
            Debug.Print ("Apply aspect to url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal bool IsAspectAppliedToUrl (ulong aspectID, ulong urlID) {
            string sql = "select * from M_Aspects_Urls where fkey_AspectID=" + aspectID + " and fkey_UrlID=" + urlID;
            Debug.Print ("Is aspect applied to url: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool UnapplyAspectFromUrl (ulong aspectID, ulong urlID) {
            string sql = "delete from M_Aspects_Urls where fkey_AspectID=" + aspectID + " and fkey_UrlID=" + urlID;
            Debug.Print ("Unapply aspect from url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal int UnapplyAllAspectsFromUrl (ulong urlID) {
            string sql = "delete from M_Aspects_Urls where fkey_UrlID=" + urlID;
            Debug.Print ("Unapply all aspects from url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        internal int UnapplyAspectFromAllUrls (ulong aspectID) {
            string sql = "delete from M_Aspects_Urls where fkey_AspectID=" + aspectID;
            Debug.Print ("Unapply aspect from all urls: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        internal List<ulong> GetAspectsAppliedOnUrl (ulong urlID) {
            string sql = "select fkey_AspectID from M_Aspects_Urls where fkey_UrlID=" + urlID;
            Debug.Print ("Get aspects applied on url: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allAspects = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong aspectID = ulong.Parse (row[0].ToString ());
                    allAspects.Add (aspectID);
                }

                return allAspects;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetUrlsAppliedWithAspect (ulong aspectID) {
            string sql = "select fkey_UrlID from M_Aspects_Urls where fkey_AspectID=" + aspectID;
            Debug.Print ("Get urls applied with aspect: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        #endregion

        #region << Notes-Aspects Operations >>

        internal bool ApplyAspectToNote (ulong aspectID, ulong noteID) {
            string sql = "insert into M_Aspects_Notes (fkey_AspectID, fkey_NoteID) values (" + aspectID + ", " + noteID + ")";
            Debug.Print ("Apply aspect to note: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal bool IsAspectAppliedToNote (ulong aspectID, ulong noteID) {
            string sql = "select * from M_Aspects_Notes where fkey_AspectID=" + aspectID + " and fkey_NoteID=" + noteID;
            Debug.Print ("Is aspect applied to note: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool UnapplyAspectFromNote (ulong aspectID, ulong noteID) {
            string sql = "delete from M_Aspects_Notes where fkey_AspectID=" + aspectID + " and fkey_NoteID=" + noteID;
            Debug.Print ("Unapply aspect from note: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal List<ulong> GetAspectsAppliedOnNote (ulong noteID) {
            string sql = "select fkey_AspectID from M_Aspects_Notes where fkey_NoteID=" + noteID;
            Debug.Print ("Get aspects applied on note: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allAspects = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong aspectID = ulong.Parse (row[0].ToString ());
                    allAspects.Add (aspectID);
                }

                return allAspects;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal List<ulong> GetNotesAppliedWithAspect (ulong aspectID) {
            string sql = "select fkey_NoteID from M_Aspects_Notes where fkey_AspectID=" + aspectID;
            Debug.Print ("Get notes applied with aspect: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int UnapplyAllAspectsFromNote (ulong noteID) {
            string sql = "delete from M_Aspects_Notes where fkey_NoteID=" + noteID;
            Debug.Print ("Unapply all aspects from note: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        internal int UnapplyAspectFromAllNotes (ulong aspectID) {
            string sql = "delete from M_Aspects_Notes where fkey_AspectID=" + aspectID;
            Debug.Print ("Unapply aspect from all notes: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
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

        #endregion

        #region << File Bookmarking Operations >>

        internal bool BookmarkFile (ulong fileID) {
            string sql = "insert into L_FileBookmarks (fkey_FileID) values (" + fileID + ")";
            Debug.Print ("Bookmark file: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal List<ulong> GetAllBookmarkedFiles () {
            string sql = "select fkey_FileID from L_FileBookmarks";
            Debug.Print ("Get all file bookmarks: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool IsFileBookmarked (ulong fileID) {
            string sql = "select fkey_FileID from L_FileBookmarks where fkey_FileID=" + fileID;
            Debug.Print ("Is file bookmarked: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int DeleteFileBookmark (ulong fileID) {
            string sql = "delete from L_FileBookmarks where fkey_FileID=" + fileID;
            Debug.Print ("Delete file bookmark: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        internal int DeleteAllFileBookmarks () {
            string sql = "delete from L_FileBookmarks";
            Debug.Print ("Delete all file bookmarks: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        #endregion

        #region << Note Bookmarking Operations >>

        internal bool BookmarkNote (ulong noteID) {
            string sql = "insert into L_NoteBookmarks (fkey_NoteID) values (" + noteID + ")";
            Debug.Print ("Bookmark note: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal List<ulong> GetAllBookmarkedNotes () {
            string sql = "select fkey_NoteID from L_NoteBookmarks";
            Debug.Print ("Get all note bookmarks: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool IsNoteBookmarked (ulong noteID) {
            string sql = "select fkey_NoteID from L_NoteBookmarks where fkey_NoteID=" + noteID;
            Debug.Print ("Is note bookmarked: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int DeleteNoteBookmark (ulong noteID) {
            string sql = "delete from L_NoteBookmarks where fkey_NoteID=" + noteID;
            Debug.Print ("Delete note bookmark: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        internal int DeleteAllNoteBookmarks () {
            string sql = "delete from L_NoteBookmarks";
            Debug.Print ("Delete all note bookmarks: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        #endregion

        #region << Url Bookmarking Operations >>

        internal bool BookmarkUrl (ulong urlID) {
            string sql = "insert into L_UrlBookmarks (fkey_UrlID) values (" + urlID + ")";
            Debug.Print ("Bookmark url: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                return (myCommand.ExecuteNonQuery () > 0);
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

        internal List<ulong> GetAllBookmarkedUrls () {
            string sql = "select fkey_UrlID from L_UrlBookmarks";
            Debug.Print ("Get all url bookmarks: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                List<ulong> allFiles = new List<ulong> ();
                foreach (DataRow row in dt.Rows) {
                    ulong fileID = ulong.Parse (row[0].ToString ());
                    allFiles.Add (fileID);
                }

                return allFiles;
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal bool IsUrlBookmarked (ulong urlID) {
            string sql = "select fkey_UrlID from L_UrlBookmarks where fkey_UrlID=" + urlID;
            Debug.Print ("Is url bookmarked: " + sql);

            SQLiteConnection cnn = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_READING);
                cnn.Open ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);
                SQLiteDataReader reader = myCommand.ExecuteReader ();

                DataTable dt = new DataTable ();
                dt.Load (reader);

                return (dt.Rows.Count > 0);
            } catch (Exception e) {
                Trace.TraceError (e.Message);
                throw new MfsDBException (e.Message);
            } finally {
                if (cnn != null) {
                    cnn.Close ();
                }
            }
        }

        internal int DeleteUrlBookmark (ulong urlID) {
            string sql = "delete from L_UrlBookmarks where fkey_UrlID=" + urlID;
            Debug.Print ("Delete url bookmark: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        internal int DeleteAllUrlBookmarks () {
            string sql = "delete from L_UrlBookmarks";
            Debug.Print ("Delete all url bookmarks: " + sql);

            SQLiteConnection cnn = null;
            SQLiteTransaction transaction = null;
            try {
                cnn = new SQLiteConnection (USERDB_CONN_STR_FOR_WRITING);
                cnn.Open ();
                transaction = cnn.BeginTransaction ();

                SQLiteCommand myCommand = new SQLiteCommand (sql, cnn);

                int updatedRows = myCommand.ExecuteNonQuery ();
                return updatedRows;
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

        #endregion
    }
}
