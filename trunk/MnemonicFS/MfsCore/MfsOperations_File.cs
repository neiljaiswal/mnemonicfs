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
using System.Diagnostics;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsUtils.MfsLogging;
using MnemonicFS.MfsUtils.MfsCrypto;
using MnemonicFS.MfsUtils.MfsConfig;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsUtils.MfsSystem;

namespace MnemonicFS.MfsCore {
    public partial class MfsOperations {
        [Serializable]
        public class _File : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _File (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _File _theObject = null;

            internal static _File GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _File (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoFileChecks (ulong fileID) {
                if (fileID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "File id")
                    );
                }

                if (!_dbOperations.DoesFileExist (fileID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "File")
                    );
                }
            }

            internal void DoVersionChecks (ulong fileID, int versionNumber) {
                if (versionNumber < 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NEGATIVE, "Version number")
                    );
                }

                if (!_dbOperations.DoesFileVersionExist (fileID, versionNumber)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "File version")
                    );
                }
            }

            internal void ValidateFileData (byte[] bytes) {
                if (bytes == null || bytes.Length == 0 || bytes.Length > MAX_FILE_SIZE) {
                    throw new MfsFileDataException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "File data")
                    );
                }
                if (bytes.Length > MAX_FILE_SIZE) {
                    throw new MfsFileDataException (
                        MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "File data", MAX_FILE_SIZE)
                    );
                }
            }

            #endregion << Client-input Check Methods >>

            #region << File Save / Retrieval / Deletion Instance Operations >>

            public ulong New (string fileName, string fileNarration, byte[] fileData, DateTime when, bool indexFile) {
                ValidateString (fileName, ValidationCheckType.FILE_NAME);
                ValidateString (fileNarration, ValidationCheckType.FILE_NARRATION);
                ValidateFileData (fileData);

                string assumedFileName;
                string filePassword;
                string archiveName;
                string filePath;

                MfsStorageDevice.SaveFile (
                    _parent._userAbsPath, fileName, fileData, out assumedFileName, out filePassword, out archiveName, out filePath
                    );

                Debug.Print ("Got path info from storage device: " + filePath);

                // Also get the file size:
                int fileSize = fileData.Length;
                Debug.Print ("File size: " + fileSize);

                // And the file hash:
                string fileHash = Hasher.GetFileHash (fileData);

                // Next, save file meta info to db:
                Debug.Print ("Saving file meta data to db.");
                ulong fileID = _dbOperations.SaveFileMetaData (
                    fileName, fileNarration, fileSize, fileHash,
                    archiveName, filePath, when,
                    assumedFileName, filePassword
                    );

                // Also index file extension (always lower-case):
                string fileExtension = StringUtils.GetFileExtension (fileName).ToLower ();
                if (!fileExtension.Equals (string.Empty)) {
                    _parent._dbOperations.IndexFileExtension (fileID, fileExtension);
                }

                if (indexFile) {
                    bool fileIndexed = _parent.IndexObj.IndexFile (fileName, fileNarration, fileData, when, fileID, 0);
                }

                Debug.Print ("Returning new file id: " + fileID);
                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.CREATED, DateTime.Now, fileName, fileNarration);

                return fileID;
            }

            /// <summary>
            /// This method returns the file for the passed file id, as a byte array.
            /// </summary>
            /// <param name="fileID">Id of the file, the size of which is sought.</param>
            /// <returns>The file sought, as a byte array.</returns>
            public byte[] RetrieveOriginal (ulong fileID) {
                DoFileChecks (fileID);

                string fileWithPath = Config.GetStorageBasePath () + _parent._userSpecificPath + _parent._dbOperations.GetFileContainingDirPath (fileID) + _parent._dbOperations.GetFileArchiveName (fileID);

                // Now check to see if the file exists in storage:
                bool doesFileExistInStorage = MfsStorageDevice.DoesFileExist (fileWithPath);
                if (!doesFileExistInStorage) {
                    throw new MfsStorageCorruptedException (
                        MfsErrorMessages.GetMessage (MessageType.CORRUPTED, "File storage")
                    );
                }

                string assumedFileName = _dbOperations.GetFileAssumedName (fileID);
                string password = _dbOperations.GetFilePassword (fileID);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.ACCESSED_ORIGINAL, DateTime.Now, null, null);

                return MfsStorageDevice.RetrieveByteArrayFromZippedFile (fileWithPath, assumedFileName, password);
            }

            /// <summary>
            /// This method allows the calling client to delete a file that is already present within the system.
            /// </summary>
            /// <param name="fileID">Id of the file that the client wants to delete.</param>
            public int Delete (ulong fileID) {
                DoFileChecks (fileID);

                string fileName = _dbOperations.GetFileName (fileID);
                string filePath = Config.GetStorageBasePath () + _parent._userSpecificPath + _dbOperations.GetFileContainingDirPath (fileID);

                List<string> allVersionsOfFile = _dbOperations.GetAllVersionsPathsForFile (fileID);

                int deletionCount = allVersionsOfFile.Count;
                // Delete the file's meta-data entries from the db:
                deletionCount += _dbOperations.DeleteFile (fileID);

                // Finally, ask the storage device to delete the file:
                MfsStorageDevice.DeleteFile (filePath, fileName);

                string sep = BaseSystem.GetFileSystemSeparator ();

                // Also delete all the versions of this file:
                foreach (string fileWithPath in allVersionsOfFile) {
                    int index = fileWithPath.LastIndexOf (sep[0]);
                    string[] fileNameAndPath = new string[2];
                    fileNameAndPath[0] = fileWithPath.Substring (0, index + 1);
                    fileNameAndPath[1] = fileWithPath.Substring (index + 1, fileWithPath.Length - index - 1);
                    MfsStorageDevice.DeleteFile (_parent._userAbsPath + fileNameAndPath[0], fileNameAndPath[1]);
                }

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.DELETED, DateTime.Now, null, null);

                return deletionCount;
            }

            /// <summary>
            /// This method tells the client if a file exists within the system.
            /// </summary>
            /// <param name="fileID">Id of the file for which this information is sought.</param>
            /// <returns>A boolean value indicating whether the file exists or not.</returns>
            public bool Exists (ulong fileID) {
                if (fileID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "File id")
                    );
                }

                return _dbOperations.DoesFileExist (fileID);
            }

            /// <summary>
            /// This method returns the file name of the file for the passed file id.
            /// </summary>
            /// <param name="fileID">Id of the file, the name of which is sought.</param>
            /// <returns>Name of the file sought.</returns>
            public string GetName (ulong fileID) {
                DoFileChecks (fileID);

                return this._dbOperations.GetFileName (fileID);
            }

            /// <summary>
            /// This method returns the file size of the file for the passed file id.
            /// </summary>
            /// <param name="fileID">Id of the file, the size of which is sought.</param>
            /// <returns>Size of the file sought.</returns>
            public int GetSize (ulong fileID) {
                DoFileChecks (fileID);

                return this._dbOperations.GetFileSize (fileID);
            }

            /// <summary>
            /// This method returns the (MD5) file hash for the passed file id.
            /// </summary>
            /// <param name="fileID">Id of the file, the hash value of which is sought.</param>
            /// <returns>Hash of the file sought.</returns>
            public string GetHash (ulong fileID) {
                DoFileChecks (fileID);

                return this._dbOperations.GetFileHash (fileID);
            }

            /// <summary>
            /// This method returns a dictionary of file ids. Against each file id is a real value that
            /// indicates to what extent the duplicate file matches the passed file. The heuristic score
            /// is calculated as follows:
            /// a) If the file content is the same, but the file names and narrations are different, the
            /// score is 0.75;
            /// b) If the file content and the names are the same, but the narrations are different, the score
            /// is 0.90;
            /// c) If the file content and the narrations are the same, but the names are different, the score
            /// is 0.85;
            /// d) If the file content, the name, and the naration are the same, the score is 1.0;
            /// e) Else it's a no-hit.
            /// </summary>
            /// <param name="fileID">Id of the file for which duplicate files are sought.</param>
            /// <returns>A dictionary of file ids along with their heuristic hit score.</returns>
            public Dictionary<ulong, double> GetDuplicates (ulong fileID) {
                DoFileChecks (fileID);

                return _dbOperations.GetDuplicateFiles (fileID);
            }

            /// <summary>
            /// This method returns the file narration of the file for the passed file id.
            /// </summary>
            /// <param name="fileID">Id of the file, the narration of which is sought.</param>
            /// <returns>Narration of the file sought.</returns>
            public string GetNarration (ulong fileID) {
                DoFileChecks (fileID);

                return this._dbOperations.GetFileNarration (fileID);
            }

            /// <summary>
            /// This method returns the file save date-time for the passed file id.
            /// </summary>
            /// <param name="fileID">Id of the file, the save date-time stamp of which is sought.</param>
            /// <returns>Date-time when the file was saved.</returns>
            public DateTime GetSaveDateTime (ulong fileID) {
                DoFileChecks (fileID);

                return this._dbOperations.GetFileSaveDateTime (fileID);
            }

            /// <summary>
            /// This method sets a file's deletion date-time.
            /// </summary>
            /// <param name="fileID">Id of the file, the deletion date-time stamp of which is to be set.</param>
            /// <param name="deletionDateTime">The deletion date-time to be set</param>
            public void SetDeletionDateTime (ulong fileID, DateTime deletionDateTime) {
                DoFileChecks (fileID);

                DateTime fileSaveDateTime = _dbOperations.GetFileSaveDateTime (fileID);

                Debug.Print ("Save DateTime: " + fileSaveDateTime + "; Deletion DateTime: " + deletionDateTime);

                if (deletionDateTime <= fileSaveDateTime) {
                    throw new MfsIllegalOperationException (
                        MfsErrorMessages.GetMessage (MessageType.DATE_DISCREPANCY, "Deletion date before file save date")
                    );
                }

                if (deletionDateTime <= DateTime.Now) {
                    throw new MfsIllegalOperationException (
                        MfsErrorMessages.GetMessage (MessageType.DATE_DISCREPANCY, "Deletion date before current date")
                    );
                }

                _dbOperations.SetDeletionDateTime (fileID, deletionDateTime);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.DELETION_DATETIME_SET, DateTime.Now, deletionDateTime.ToString (), null);
            }

            /// <summary>
            /// This method returns a file's deletion date-time.
            /// </summary>
            /// <param name="fileID">Id of the file, the save date-time stamp of which is sought</param>
            /// <returns>A DateTime object that indicates the deletion date-time.</returns>
            public DateTime GetDeletionDateTime (ulong fileID) {
                DoFileChecks (fileID);

                return _dbOperations.GetDeletionDateTime (fileID);
            }

            /// <summary>
            /// This method updates the name of a file already saved on the system.
            /// </summary>
            /// <param name="fileID">Id of the file that has to be renamed.</param>
            /// <param name="newName">New name of the file.</param>
            /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
            public bool UpdateName (ulong fileID, string newName) {
                DoFileChecks (fileID);
                ValidateString (newName, ValidationCheckType.FILE_NAME);

                bool updated = _dbOperations.UpdateFileName (fileID, newName);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.FILENAME_UPDATED, DateTime.Now, newName, null);

                return updated;
            }

            /// <summary>
            /// This method updates the narration of a file already saved on the system.
            /// </summary>
            /// <param name="fileID">Id of the file, the narration of which has to be updated.</param>
            /// <param name="newNarration">New narration of the file.</param>
            /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
            public bool UpdateNarration (ulong fileID, string newNarration) {
                DoFileChecks (fileID);
                ValidateString (newNarration, ValidationCheckType.FILE_NARRATION);

                bool updated = _dbOperations.UpdateFileNarration (fileID, newNarration);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.FILE_NARRATION_UPDATED, DateTime.Now, newNarration, null);

                return updated;
            }

            /// <summary>
            /// This method updates the file save date-time of a file already saved on the system.
            /// </summary>
            /// <param name="fileID">Id of the file, the save date-time of which has to be updated.</param>
            /// <param name="newWhen">New save date-time of the file.</param>
            /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
            public bool UpdateSaveDateTime (ulong fileID, DateTime newWhen) {
                DoFileChecks (fileID);

                bool updated = _dbOperations.UpdateFileSaveDateTime (fileID, newWhen);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.FILE_SAVEDATETIME_UPDATED, DateTime.Now, newWhen.ToString (), null);

                return updated;
            }

            /// <summary>
            /// This method updates the file deletion date-time of a file already saved on the system.
            /// </summary>
            /// <param name="fileID">Id of the file, the deletion date-time of which has to be updated.</param>
            /// <param name="newDeletionDateTime">New deletion date-time of the file.</param>
            /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
            public bool UpdateDeletionDateTime (ulong fileID, DateTime newDeletionDateTime) {
                DoFileChecks (fileID);

                bool updated = _dbOperations.UpdateFileDeletionDateTime (fileID, newDeletionDateTime);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.DELETION_DATETIME_UPDATED, DateTime.Now, newDeletionDateTime.ToString (), null);

                return updated;
            }

            /// <summary>
            /// This method resets a file's deletion date-time.
            /// </summary>
            /// <param name="fileID">Id of the file the deletion date-time of which has to be reset.</param>
            public void ResetDeletionDateTime (ulong fileID) {
                DoFileChecks (fileID);

                _dbOperations.ResetDeletionDateTime (fileID);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.DELETION_DATETIME_RESET, DateTime.Now, null, null);
            }

            #endregion << File Save / Retrieval / Deletion Instance Operations >>

            #region << File Version-related Operations >>

            public int SaveAsNextVersion (ulong fileID, byte[] fileData, string comments, int currentVersionNumber) {
                DoFileChecks (fileID);
                ValidateFileData (fileData);
                ValidateString (comments, ValidationCheckType.VERSION_COMMENT);
                DoVersionChecks (fileID, currentVersionNumber);

                // Bear in mind that the narration property will be inherited by all subsequent versions
                // of the old file.
                // One thing the client will have no control over is that the newer versions do not allow
                // the client to specify the save date-time.

                int lastVersionNumber = _dbOperations.GetLastFileVersionNumber (fileID);
                if (lastVersionNumber - currentVersionNumber > 0) {
                    throw new MfsFileVersionConflictException (
                        MfsErrorMessages.GetMessage (MessageType.VERSION_CONFLICT, "Another version of file checked in")
                    );
                }

                // Also, get its name and path in the file storage:
                string fileName = _dbOperations.GetFileName (fileID);
                string fileAssumedName = _dbOperations.GetFileAssumedName (fileID);
                string password = _dbOperations.GetFilePassword (fileID);
                string archiveName = _dbOperations.GetFileArchiveName (fileID);
                string containingDirPath = _dbOperations.GetFileContainingDirPath (fileID);

                // Recall that the path is of the form: <base_dir>/<date-time>/<some_int>/<last_version_number>/
                // The thingamajig code below merely gets the complete path of the last version, and adds a new
                // version path to it. Thus, if the last file version path was, say,
                // <base_dir>/abc/pqr/xyz/v41/, the fresh path will be: <base_dir>/abc/pqr/xyz/v42/
                // Read the code below to find out how: it's just a manpulation of strings.
                int nextVersionNumber = lastVersionNumber + 1;
                string lastPartOfFreshPath = "v" + (nextVersionNumber).ToString ();

                Debug.Print ("Got original file's path: " + containingDirPath);
                string freshPath = null;
                char systemSep = (BaseSystem.GetFileSystemSeparator ())[0];

                if (containingDirPath.EndsWith (systemSep.ToString ())) {
                    string tmpStr = containingDirPath.Substring (0, containingDirPath.Length - 1);
                    int indexOfSeparator = tmpStr.LastIndexOf (systemSep);
                    freshPath = containingDirPath.Substring (0, indexOfSeparator) + systemSep + lastPartOfFreshPath + systemSep;
                } else {
                    int indexOfSeparator = containingDirPath.LastIndexOf (systemSep);
                    freshPath = containingDirPath.Substring (0, indexOfSeparator) + systemSep + lastPartOfFreshPath + systemSep;
                }
                Debug.Print ("Got fresh path: " + freshPath);

                // Now ask the MfsStorageDevice to save this file:
                MfsStorageDevice.SaveFileAsNewVersion (_parent._userAbsPath, fileData, freshPath, fileAssumedName, password, archiveName);

                string fileAbsPath = null;
                fileAbsPath = freshPath + archiveName;

                // Calculate its hash before saving its meta-data:
                string fileHash = Hasher.GetFileHash (fileData);

                _dbOperations.SaveAsNextVersion (fileID, fileHash, comments, fileAbsPath, nextVersionNumber);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.NEW_VERSION_CREATED, DateTime.Now, nextVersionNumber.ToString (), null);

                return nextVersionNumber;
            }

            public byte[] RetrieveLastVersion (ulong fileID, out int currentVersionNumber) {
                DoFileChecks (fileID);

                currentVersionNumber = _dbOperations.GetLastFileVersionNumber (fileID);

                if (currentVersionNumber == 0) {
                    return RetrieveOriginal (fileID);
                }

                string fileNameWithPath = _dbOperations.GetFileVersionPath (fileID, currentVersionNumber);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.ACCESSED_VERSION, DateTime.Now, currentVersionNumber.ToString (), null);

                return MfsStorageDevice.RetrieveFile (_parent._userAbsPath + fileNameWithPath);
            }

            public int GetLastVersionNumber (ulong fileID) {
                DoFileChecks (fileID);

                return _dbOperations.GetLastFileVersionNumber (fileID);
            }

            public string GetFileVersionHash (ulong fileID, int versionNumber) {
                DoFileChecks (fileID);
                DoVersionChecks (fileID, versionNumber);

                return _dbOperations.GetFileVersionHash (fileID, versionNumber);
            }

            public byte[] RetrieveVersion (ulong fileID, int versionNumber) {
                DoFileChecks (fileID);
                DoVersionChecks (fileID, versionNumber);

                string fileNameWithPath = _dbOperations.GetFileVersionPath (fileID, versionNumber);

                return MfsStorageDevice.RetrieveFile (_parent._userAbsPath + fileNameWithPath);
            }

            // TODO: To add tests for this.
            /*public byte[] RetrieveVersion (ulong fileID, int versionNumber) {
                _parent.DoFileChecks (fileID);

                if (versionNumber == 0) {
                    return RetrieveOriginal (fileID);
                }

                int lastVersionNumber = _dbOperations.GetLastFileVersionNumber (fileID);
                if (versionNumber == lastVersionNumber) {
                    return RetrieveLastVersion (fileID, out versionNumber);
                }

                if (versionNumber < 0 || versionNumber > lastVersionNumber) {
                    throw new MfsFileVersionException ("Illegal file version specified.");
                }

                string fileNameWithPath = _dbOperations.GetFileVersionPath (fileID, versionNumber);

                FileLogger.AddLogEntry (_parent._userID, fileID, FileLogEntryType.ACCESSED_VERSION, DateTime.Now, versionNumber.ToString (), null);

                return MfsStorageDevice.RetrieveFile (fileNameWithPath);
            }*/

            public void GetVersionDetails (ulong fileID, int versionNumber, out string comments, out DateTime whenDateTime) {
                DoFileChecks (fileID);
                DoVersionChecks (fileID, versionNumber);

                _dbOperations.GetFileVersionDetails (fileID, versionNumber, out comments, out whenDateTime);
            }

            public void GetVersionHistoryLog (ulong fileID, out string[] versionComments, out DateTime[] versionDateTimes) {
                DoFileChecks (fileID);

                _dbOperations.GetFileVersionHistoryLog (fileID, out versionComments, out versionDateTimes);
            }

            public void GetVersionDiff (ulong fileID, int versionNumber1, int versionNumber2, out byte[] fileData1, out byte[] fileData2) {
                // Validations _are_ happening in GetFileVersion () method. No fun in repeating them here and slowing down the application 
                // unnecessarily. The only other validation we do here is to see if the file version numbers are not the same.
                if (versionNumber1 == versionNumber2) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.VERSION_CONFLICT, "Same version numbers for diffing")
                    );
                }

                fileData1 = RetrieveVersion (fileID, versionNumber1);
                fileData2 = RetrieveVersion (fileID, versionNumber2);
            }

            #endregion << File Version-related Operations >>

            #region << File Retrieval in Date/Time Ranges >>

            public List<ulong> GetAll () {
                return _dbOperations.GetAllFiles ();
            }

            public List<ulong> GetInDateRange (DateTime startDate, DateTime endDate) {
                return _dbOperations.GetFilesInDateRange (startDate, endDate);
            }

            public List<ulong> GetInDateTimeRange (DateTime startDateTime, DateTime endDateTime) {
                return _dbOperations.GetFilesInDateTimeRange (startDateTime, endDateTime);
            }

            public List<ulong> GetOnDate (DateTime onDate) {
                return _dbOperations.GetFilesOnDate (onDate);
            }

            public List<ulong> GetAtDateTime (DateTime onDateTime) {
                return _dbOperations.GetFilesOnDateTime (onDateTime);
            }

            public List<ulong> GetBeforeDate (DateTime beforeDate) {
                return _dbOperations.GetFilesBeforeDate (beforeDate);
            }

            public List<ulong> GetBeforeDateTime (DateTime beforeDateTime) {
                return _dbOperations.GetFilesBeforeDateTime (beforeDateTime);
            }

            public List<ulong> GetBeforeAndOnDate (DateTime beforeOnDate) {
                return _dbOperations.GetFilesBeforeAndOnDate (beforeOnDate);
            }

            public List<ulong> GetBeforeAndAtDateTime (DateTime beforeOnDateTime) {
                return _dbOperations.GetFilesBeforeAndOnDateTime (beforeOnDateTime);
            }

            public List<ulong> GetAfterDate (DateTime afterDate) {
                return _dbOperations.GetFilesAfterDate (afterDate);
            }

            public List<ulong> GetAfterDateTime (DateTime afterDateTime) {
                return _dbOperations.GetFilesAfterDateTime (afterDateTime);
            }

            public List<ulong> GetAfterAndOnDate (DateTime afterOnDate) {
                return _dbOperations.GetFilesAfterAndOnDate (afterOnDate);
            }

            public List<ulong> GetAfterAndAtDateTime (DateTime afterOnDateTime) {
                return _dbOperations.GetFilesAfterAndOnDateTime (afterOnDateTime);
            }

            #endregion << File Retrieval in Date/Time Ranges >>
        }
    }
}
