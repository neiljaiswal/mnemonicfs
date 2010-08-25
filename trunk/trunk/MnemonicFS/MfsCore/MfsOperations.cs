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
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsUtils.MfsCrypto;
using MnemonicFS.MfsUtils.MfsConfig;
using MnemonicFS.MfsUtils.MfsSystem;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsUtils.MfsLogging;
using MnemonicFS.MfsUtils.MfsIndexing;

namespace MnemonicFS.MfsCore {
    internal enum ValidationCheckType {
        FILE_NAME = 1,
        FILE_NARRATION,
        ASPECT_NAME,
        ASPECT_DESC,
        BRIEFCASE_NAME,
        BRIEFCASE_DESC,
        COLLECTION_NAME,
        COLLECTION_DESC,
        VERSION_COMMENT,
        SCHEMA_FREE_DOC_NAME
    };

    public enum DocumentType {
        FILE = 1,
        NOTE,
        URL,
        VCARD
    };

    public enum GroupingType {
        ASPECT = 1,
        BRIEFCASE,
        COLLECTION
    };

    public enum FilterType {
        AND = 1,
        OR
    };

    public class MfsOperations {
        #region << Constants Declarations >>

        private static int HASH_SIZE = 40;

        private static int MAX_FILENAME_LENGTH = Config.GetMaxFileNameLength ();
        private static int MAX_FILENARRATION_LENGTH = Config.GetMaxFileNarrationLength ();
        private static int MAX_FILE_SIZE = Config.GetMaxFileSize ();

        private static int MAX_ASPECTNAME_LENGTH = Config.GetMaxAspectNameLength ();
        private static int MAX_ASPECTDESC_LENGTH = Config.GetMaxAspectDescLength ();

        private static int MAX_BRIEFCASENAME_LENGTH = Config.GetMaxBriefcaseNameLength ();
        private static int MAX_BRIEFCASEDESC_LENGTH = Config.GetMaxBriefcaseDescLength ();

        private static int MAX_COLLECTIONNAME_LENGTH = Config.GetMaxCollectionNameLength ();
        private static int MAX_COLLECTIONDESC_LENGTH = Config.GetMaxCollectionDescLength ();

        private static int MAX_URL_LENGTH = Config.GetMaxUrlLength ();

        private static int MAX_FILEVERSIONCOMMENT_LENGTH = Config.GetMaxFileVersionCommentLength ();

        private static int MAX_SCHEMA_FREE_DOC_NAME_LENGTH = Config.GetMaxSchemaFreeDocNameLength ();

        private static string REGEX_STRING = Config.GetRegexString ();

        private static ulong GLOBAL_BRIEFCASE_ID = 1;

        #endregion << Constants Declarations >>

        #region << Property Getters >>

        public static int MaxFileNameLength {
            get {
                return MAX_FILENAME_LENGTH;
            }
        }

        public static int MaxFileNarrationLength {
            get {
                return MAX_FILENARRATION_LENGTH;
            }
        }

        public static int MaxFileSize {
            get {
                return MAX_FILE_SIZE;
            }
        }

        public static int MaxAspectNameLength {
            get {
                return MAX_ASPECTNAME_LENGTH;
            }
        }

        public static int MaxAspectDescLength {
            get {
                return MAX_ASPECTDESC_LENGTH;
            }
        }

        public static int MaxBriefcaseNameLength {
            get {
                return MAX_BRIEFCASENAME_LENGTH;
            }
        }

        public static int MaxBriefcaseDescLength {
            get {
                return MAX_BRIEFCASEDESC_LENGTH;
            }
        }

        public static int MaxCollectionNameLength {
            get {
                return MAX_COLLECTIONNAME_LENGTH;
            }
        }

        public static int MaxCollectionDescLength {
            get {
                return MAX_COLLECTIONDESC_LENGTH;
            }
        }

        public static int MaxUrlLength {
            get {
                return MAX_URL_LENGTH;
            }
        }

        public static ulong GlobalBriefcase {
            get {
                return GLOBAL_BRIEFCASE_ID;
            }
        }

        public static string RegexString {
            get {
                return REGEX_STRING;
            }
        }

        public static int MaxSchemaFreeDocNameLength {
            get {
                return MAX_SCHEMA_FREE_DOC_NAME_LENGTH;
            }
        }

        internal MfsDBOperations DBOperationsObject {
            get {
                return _dbOperations;
            }
        }

        #endregion << Property Getters >>

        #region << Bare Bytestream Storage / Retrieval Operations >>

        /// <summary>
        /// This is a static method for user-independent operation that can done by the client utility
        /// for saving a byte stream to the storage. This method is especially useful if the client would
        /// want to, say, stripe a file across multiple clients across a network.
        /// </summary>
        /// <param name="byteStream">The byte stream to be saved.</param>
        /// <param name="password">Password based on which the byte stream can be recovered.</param>
        /// <param name="referenceNumber">A reference number that could be used by the client to
        /// specify some custom value.
        /// </param>
        /// <returns>A unique id that identifies the byte stream on the storage.</returns>
        public static ulong StoreByteStream (byte[] byteStream, string passphrase, int referenceNumber) {
            if (byteStream == null || byteStream.Length == 0) {
                throw new MfsIllegalArgumentException ("Byte stream cannot be null or empty.");
            }

            if (passphrase == null || passphrase.Length == 0) {
                throw new MfsIllegalArgumentException ("Passphrase cannot be be null or empty.");
            }

            string assumedFileName;
            string archiveName;
            string destDir;

            MfsStorageDevice.SaveByteArray (byteStream, passphrase, out assumedFileName, out archiveName, out destDir);

            return MfsDBOperations.SaveByteArrayMetaData (assumedFileName, archiveName, destDir, referenceNumber);
        }

        public static byte[] RetrieveByteStream (ulong byteStreamID, string passphrase, out int referenceNumber) {
            if (byteStreamID == 0) {
                throw new MfsIllegalArgumentException ("Byte Stream ID cannot be zero.");
            }

            if (passphrase == null || passphrase.Length == 0) {
                throw new MfsIllegalArgumentException ("Passphrase cannot be null or empty.");
            }

            if (!DoesByteStreamExist (byteStreamID)) {
                throw new MfsNonExistentResourceException ("Byte stream does not exist.");
            }

            string assumedFileName = null;
            string archiveName = null;
            string archivePath = null;

            MfsDBOperations.GetByteStreamMetaData (byteStreamID, out assumedFileName, out archiveName, out archivePath, out referenceNumber);

            return MfsStorageDevice.RetrieveByteArrayFromZippedFile (archivePath + archiveName, assumedFileName, passphrase);
        }

        public static int GetByteStreamReferenceNumber (ulong byteStreamID) {
            if (byteStreamID == 0) {
                throw new MfsIllegalArgumentException ("Byte Stream ID cannot be zero.");
            }

            if (!DoesByteStreamExist (byteStreamID)) {
                throw new MfsNonExistentResourceException ("Byte stream does not exist.");
            }

            return MfsDBOperations.GetByteStreamReferenceNumber (byteStreamID);
        }

        public static void DeleteByteStream (ulong byteStreamID, string passphrase) {
            if (byteStreamID == 0) {
                throw new MfsIllegalArgumentException ("Byte Stream ID cannot be zero.");
            }

            if (passphrase == null || passphrase.Length == 0) {
                throw new MfsIllegalArgumentException ("Passphrase cannot be null or empty.");
            }

            if (!DoesByteStreamExist (byteStreamID)) {
                throw new MfsNonExistentResourceException ("Byte stream does not exist.");
            }

            string assumedFileName = null;
            string archiveName = null;
            string archivePath = null;
            int referenceNumber;

            MfsDBOperations.GetByteStreamMetaData (byteStreamID, out assumedFileName, out archiveName, out archivePath, out referenceNumber);
            MfsDBOperations.DeleteByteStreamMetaData (byteStreamID);

            MfsStorageDevice.DeleteFile (archivePath, archiveName);
        }

        public static bool DoesByteStreamExist (ulong byteStreamID) {
            if (byteStreamID == 0) {
                throw new MfsIllegalArgumentException ("Byte Stream ID cannot be zero.");
            }

            return MfsDBOperations.DoesByteStreamExist (byteStreamID);
        }

        #endregion << Bare Bytestream Storage / Retrieval Operations >>

        #region << Static Constructor & User-related Operations >>

        static MfsOperations () {
            MfsStorageDevice.InitDevice ();
        }

        private static string ProcessUserIDStr (string userID) {
            return userID.Trim ().ToLower ();
        }

        public static ulong CreateNewUser (string userID, string passwordHash) {
            if (userID == null || userID.Length == 0) {
                throw new MfsIllegalArgumentException ("UserIDStr cannot be null or empty.");
            }

            userID = ProcessUserIDStr (userID);

            if (DoesUserExist (userID)) {
                throw new MfsDuplicateNameException ("User already exists.");
            }

            if (passwordHash == null || passwordHash.Length != HASH_SIZE) {
                throw new MfsIllegalArgumentException (
                    string.Format ("PasswordHash cannot be null or of size not equal to {0}", HASH_SIZE)
                    );
            }

            Regex regex = new Regex (REGEX_STRING);
            if (!regex.IsMatch (userID)) {
                throw new MfsIllegalArgumentException ("UserIDStr does not comply with user id format requirements.");
            }

            return CreateUserSpecificPaths (userID, passwordHash);
        }

        private static ulong CreateUserSpecificPaths (string userID, string passwordHash) {
            string userSpecificPath = MfsStorageDevice.CreateUserPath (userID);

            ulong uid = MfsDBOperations.CreateUser (userID, passwordHash, userSpecificPath);
            Debug.Print ("Done creating user with userID: " + uid);

            return uid;
        }

        public static int UpdateUserPassword (string userID, string newPasswordHash) {
            if (userID == null) {
                throw new MfsIllegalArgumentException ("UserIDStr may not be null.");
            }

            userID = ProcessUserIDStr (userID);

            if (MfsDBOperations.DoesUserExist (userID) == 0) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            if (newPasswordHash == null || newPasswordHash.Length != HASH_SIZE) {
                throw new MfsIllegalArgumentException ("Password hash may not be null or an invalid hash.");
            }

            return MfsDBOperations.UpdateUserPassword (userID, newPasswordHash);
        }

        public static List<string> GetMfsUsers () {
            return MfsDBOperations.GetMfsUsers ();
        }

        public static int GetUserCount () {
            return MfsDBOperations.GetUserCount ();
        }

        public static int DeleteUser (string userID, bool deleteUserStorage, bool deleteUserLogs) {
            userID = ProcessUserIDStr (userID);

            if (MfsDBOperations.DoesUserExist (userID) == 0) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            if (deleteUserStorage) {
                DeleteUserStorage (userID);
            }

            if (deleteUserLogs) {
                DeleteUserFileLogs (userID);
            }

            return MfsDBOperations.DeleteUser (userID);
        }

        private static void DeleteUserStorage (string userID) {
            userID = ProcessUserIDStr (userID);

            string path = MfsDBOperations.GetUserSpecificPath (userID);
            Debug.Print ("Got user specific path: " + path);

            // First delete user db:
            MfsStorageDevice.DeleteUserDB (path);
            // And also all user file system data:
            MfsStorageDevice.DeleteUserFileSystemObjects (path);

            string dirToDelete = Config.GetStorageBasePath () + path;
            MfsStorageDevice.DeleteDirectoryIfEmpty (dirToDelete);
        }

        public static bool DoesUserExist (string userID) {
            userID = ProcessUserIDStr (userID);

            return (MfsDBOperations.DoesUserExist (userID) != 0);
        }

        public static bool IsUserNameCompliant (string userID) {
            userID = ProcessUserIDStr (userID);

            Regex regex = new Regex (REGEX_STRING);
            return regex.IsMatch (userID);
        }

        #endregion << Static Constructor & User-related Operations >>

        #region << Instance-specific Variable Declarations >>

        private string _userID;
        private string _userSpecificPath;
        private string _userFQPath;
        private MfsDBOperations _dbOperations;
        private LuceneIndexer _indexer;

        #endregion << Instance-specific Variable Declarations >>

        #region << Object Construction >>

        public MfsOperations (string userID, string passwordHash) {
            userID = ProcessUserIDStr (userID);

            ulong uid = MfsDBOperations.DoesUserExist (userID);

            if (uid == 0) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            // If control has reached here, it means that the user exists, so check for password:
            bool authenticated = MfsDBOperations.AuthenticateUser (userID, passwordHash);
            if (!authenticated) {
                throw new MfsAuthenticationException ("Authentication failed.");
            }

            LoadUserValues (userID, passwordHash);
        }

        internal MfsOperations (string userID) {
            userID = ProcessUserIDStr (userID);

            ulong uid = MfsDBOperations.DoesUserExist (userID);
            if (uid == 0) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            string passwordHash = MfsDBOperations.GetUserPasswordHash (userID);

            LoadUserValues (userID, passwordHash);
        }

        private void LoadUserValues (string userID, string passwordHash) {
            Debug.Print ("Loading values for user: " + userID);

            _userID = userID;
            _userSpecificPath = MfsDBOperations.GetUserSpecificPath (userID);

            _userFQPath = Config.GetStorageBasePath () + _userSpecificPath;
            Debug.Print ("Got FQ path for user: " + _userFQPath);

            Debug.Print ("Creating DBOperations object.");
            _dbOperations = new MfsDBOperations (userID, _userSpecificPath);

            _indexer = new LuceneIndexer (_userSpecificPath + @"\LuceneIndex\");
        }

        public void GetUserName (out string fName, out string lName) {
            _dbOperations.GetUserName (out fName, out lName);
        }

        #endregion << Object Construction >>

        #region << Client-input Check Methods >>

        private void DoDocumentChecks (ulong documentID) {
            if (documentID == 0) {
                throw new MfsIllegalArgumentException ("Document id cannot be zero.");
            }

            if (!(_dbOperations.DoesFileExist (documentID) || _dbOperations.DoesNoteExist (documentID) || _dbOperations.DoesUrlExist (documentID))) {
                throw new MfsNonExistentResourceException ("Non-existent document.");
            }
        }

        private void DoFileChecks (ulong fileID) {
            if (fileID == 0) {
                throw new MfsIllegalArgumentException ("File id cannot be zero.");
            }

            if (!_dbOperations.DoesFileExist (fileID)) {
                throw new MfsNonExistentResourceException ("Non-existent file.");
            }
        }

        private void DoAspectChecks (ulong aspectID) {
            if (aspectID == 0) {
                throw new MfsIllegalArgumentException ("Aspect id cannot be zero.");
            }

            if (!_dbOperations.DoesAspectExist (aspectID)) {
                throw new MfsNonExistentResourceException ("Non-existent aspect.");
            }
        }

        private void DoAspectChecks (string aspectName) {
            if (aspectName == null || aspectName.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Aspect name cannot be null or empty.");
            }

            // NO! Don't check for aspect existence here.
        }

        private void DoBriefcaseChecks (ulong briefcaseID) {
            if (briefcaseID == 0) {
                throw new MfsIllegalArgumentException ("Briefcase id cannot be zero.");
            }

            if (!_dbOperations.DoesBriefcaseExist (briefcaseID)) {
                throw new MfsNonExistentResourceException ("Non-existent briefcase.");
            }
        }

        private void DoBriefcaseChecks (string briefcaseName) {
            if (briefcaseName == null || briefcaseName.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Briefcase name cannot be null or empty.");
            }

            // NO! Don't check for briefcase existence here.
        }

        private void DoCollectionChecks (ulong collectionID) {
            if (collectionID == 0) {
                throw new MfsIllegalArgumentException ("Collection id cannot be zero.");
            }

            if (!_dbOperations.DoesCollectionExist (collectionID)) {
                throw new MfsNonExistentResourceException ("Non-existent collection.");
            }
        }

        private void DoCollectionChecks (string collectionName) {
            if (collectionName == null || collectionName.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Collection name cannot be null or empty.");
            }

            // NO! Don't check for collection existence here.
        }

        private void DoNoteChecks (ulong noteID) {
            if (noteID == 0) {
                throw new MfsIllegalArgumentException ("Note id cannot be zero.");
            }

            if (!_dbOperations.DoesNoteExist (noteID)) {
                throw new MfsNonExistentResourceException ("Non-existent note.");
            }
        }

        private void DoUrlChecks (ulong urlID) {
            if (urlID == 0) {
                throw new MfsIllegalArgumentException ("Url id cannot be zero.");
            }

            if (!_dbOperations.DoesUrlExist (urlID)) {
                throw new MfsNonExistentResourceException ("Non-existent url.");
            }
        }

        private void DoVersionChecks (ulong fileID, int versionNumber) {
            if (versionNumber < 0) {
                throw new MfsIllegalArgumentException ("Version number cannot less than be zero.");
            }

            if (!_dbOperations.DoesFileVersionExist (fileID, versionNumber)) {
                throw new MfsNonExistentResourceException ("Non-existent version of file.");
            }
        }

        private void DoSFDChecks (ulong docID) {
            if (docID == 0) {
                throw new MfsIllegalArgumentException ("Schema-free document id cannot be zero.");
            }

            if (!_dbOperations.DoesSfdExist (docID)) {
                throw new MfsNonExistentResourceException ("Non-existent schema-free document.");
            }
        }

        private void DoSchemaFreeDocChecks (string docName) {
            if (docName == null || docName.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Schema-free document name cannot be null or empty.");
            }

            // NO! Don't check for schema-free document existence here.
        }

        private static void ValidateString (string str, ValidationCheckType checkType) {
            switch (checkType) {
                case ValidationCheckType.FILE_NAME:
                    if (str == null || str.Equals (string.Empty) || str.Length > MAX_FILENAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("File name cannot be null, empty or greater than {0} chars.", MAX_FILENAME_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.FILE_NARRATION:
                    if (str == null || str.Length > MAX_FILENARRATION_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("File narration cannot be null or greater than {0} chars.", MAX_FILENARRATION_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.ASPECT_NAME:
                    if (str == null || str.Equals (string.Empty) || str.Length > MAX_ASPECTNAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Aspect name cannot be null, empty or greater than {0} chars.", MAX_ASPECTNAME_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.ASPECT_DESC:
                    if (str == null || str.Length > MAX_ASPECTDESC_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Aspect description cannot be null or greater than {0} chars.", MAX_ASPECTDESC_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.BRIEFCASE_NAME:
                    if (str == null || str.Equals (string.Empty) || str.Length > MAX_BRIEFCASENAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Briefcase name cannot be null, empty or greater than {0} chars.", MAX_BRIEFCASENAME_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.BRIEFCASE_DESC:
                    if (str == null || str.Length > MAX_BRIEFCASEDESC_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Briefcase description cannot be null or greater than {0} chars.", MAX_BRIEFCASEDESC_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.COLLECTION_NAME:
                    if (str == null || str.Equals (string.Empty) || str.Length > MAX_COLLECTIONNAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Collection name cannot be null, empty or greater than {0} chars.", MAX_COLLECTIONNAME_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.COLLECTION_DESC:
                    if (str == null || str.Length > MAX_COLLECTIONDESC_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Collection description cannot be null or greater than {0} chars.", MAX_COLLECTIONDESC_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.VERSION_COMMENT:
                    if (str == null || str.Length > MAX_FILEVERSIONCOMMENT_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("File version comment cannot be null or greater than {0} chars.", MAX_FILEVERSIONCOMMENT_LENGTH)
                            );
                    }
                    break;

                case ValidationCheckType.SCHEMA_FREE_DOC_NAME:
                    if (str == null || str.Length > MAX_SCHEMA_FREE_DOC_NAME_LENGTH) {
                        throw new MfsIllegalArgumentException (
                                string.Format ("Schema-free document name cannot be null or greater than {0} chars.", MAX_SCHEMA_FREE_DOC_NAME_LENGTH)
                            );
                    }
                    break;
            }
        }

        private void ValidateFileData (byte[] bytes) {
            if (bytes == null || bytes.Length == 0 || bytes.Length > MAX_FILE_SIZE) {
                throw new MfsFileDataException (
                        string.Format ("File data cannot be null, empty or greater than {0} chars.", MAX_FILE_SIZE)
                    );
            }
        }

        private void ValidateList (List<ulong> list, bool allowEmpty, string listName) {
            if (list == null) {
                throw new MfsIllegalArgumentException (
                    string.Format ("{0} list cannot be null.", listName)
                    );
            }

            if (!allowEmpty && list.Count == 0) {
                throw new MfsIllegalArgumentException (
                    string.Format ("{0} list cannot be empty.", listName)
                    );
            }
        }

        private static bool IsList1Superset (List<ulong> list1, List<ulong> list2) {
            foreach (ulong item in list2) {
                if (!list1.Contains (item)) {
                    return false;
                }
            }

            return true;
        }

        private static bool DoAllListsHaveItem (List<List<ulong>> lists, ulong item) {
            foreach (List<ulong> list in lists) {
                if (!list.Contains (item)) {
                    return false;
                }
            }
            return true;
        }

        #endregion << Client-input Check Methods >>

        #region << File Save / Retrieval / Deletion Instance Operations >>

        public ulong SaveFile (string fileName, string fileNarration, byte[] fileData, DateTime when, bool indexFile) {
            ValidateString (fileName, ValidationCheckType.FILE_NAME);
            ValidateString (fileNarration, ValidationCheckType.FILE_NARRATION);
            ValidateFileData (fileData);

            string assumedFileName;
            string filePassword;
            string archiveName;
            string filePath;

            MfsStorageDevice.SaveFile (
                _userFQPath, fileName, fileData, out assumedFileName, out filePassword, out archiveName, out filePath
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
                _dbOperations.IndexFileExtension (fileID, fileExtension);
            }

            if (indexFile) {
                bool fileIndexed = IndexFile (fileName, fileNarration, fileData, when, fileID, 0);
            }

            Debug.Print ("Returning new file id: " + fileID);
            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.CREATED, DateTime.Now, fileName, fileNarration);

            return fileID;
        }

        /// <summary>
        /// This method returns the file for the passed file id, as a byte array.
        /// </summary>
        /// <param name="fileID">Id of the file, the size of which is sought.</param>
        /// <returns>The file sought, as a byte array.</returns>
        public byte[] RetrieveOriginalFile (ulong fileID) {
            DoFileChecks (fileID);

            string fileWithPath = _dbOperations.GetFileContainingDirPath (fileID) + _dbOperations.GetFileArchiveName (fileID);
            string assumedFileName = _dbOperations.GetFileAssumedName (fileID);
            string password = _dbOperations.GetFilePassword (fileID);

            // Now check to see if the file exists in storage:
            bool doesFileExistInStorage = MfsStorageDevice.DoesFileExist (fileWithPath);
            if (!doesFileExistInStorage) {
                throw new MfsStorageCorruptedException ("File storage is corrupted.");
            }

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.ACCESSED_ORIGINAL, DateTime.Now, null, null);

            return MfsStorageDevice.RetrieveByteArrayFromZippedFile (fileWithPath, assumedFileName, password);
        }

        /// <summary>
        /// This method allows the calling client to delete a file that is already present within the system.
        /// </summary>
        /// <param name="fileID">Id of the file that the client wants to delete.</param>
        public int DeleteFile (ulong fileID) {
            DoFileChecks (fileID);

            string fileName = _dbOperations.GetFileName (fileID);
            string filePath = _dbOperations.GetFileContainingDirPath (fileID);
            Debug.Print ("Got file path: " + filePath + ", file name: " + fileName);

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
                Debug.Print ("Got index: " + index + ", str len: " + fileWithPath.Length);

                string[] fileNameAndPath = new string[2];
                fileNameAndPath[0] = fileWithPath.Substring (0, index + 1);
                Debug.Print ("File path: " + fileNameAndPath[0]);

                fileNameAndPath[1] = fileWithPath.Substring (index + 1, fileWithPath.Length - index - 1);
                Debug.Print ("File name: " + fileNameAndPath[1]);

                MfsStorageDevice.DeleteFile (fileNameAndPath[0], fileNameAndPath[1]);
            }

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.DELETED, DateTime.Now, null, null);

            return deletionCount;
        }

        /// <summary>
        /// This method tells the client if a file exists within the system.
        /// </summary>
        /// <param name="fileID">Id of the file for which this information is sought.</param>
        /// <returns>A boolean value indicating whether the file exists or not.</returns>
        public bool DoesFileExist (ulong fileID) {
            if (fileID == 0) {
                throw new MfsIllegalArgumentException ("File id cannot be zero.");
            }

            return _dbOperations.DoesFileExist (fileID);
        }

        /// <summary>
        /// This method returns the file name of the file for the passed file id.
        /// </summary>
        /// <param name="fileID">Id of the file, the name of which is sought.</param>
        /// <returns>Name of the file sought.</returns>
        public string GetFileName (ulong fileID) {
            DoFileChecks (fileID);

            return this._dbOperations.GetFileName (fileID);
        }

        /// <summary>
        /// This method returns the file size of the file for the passed file id.
        /// </summary>
        /// <param name="fileID">Id of the file, the size of which is sought.</param>
        /// <returns>Size of the file sought.</returns>
        public int GetFileSize (ulong fileID) {
            DoFileChecks (fileID);

            return this._dbOperations.GetFileSize (fileID);
        }

        /// <summary>
        /// This method returns the (MD5) file hash for the passed file id.
        /// </summary>
        /// <param name="fileID">Id of the file, the hash value of which is sought.</param>
        /// <returns>Hash of the file sought.</returns>
        public string GetFileHash (ulong fileID) {
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
        public Dictionary<ulong, double> GetDuplicateFiles (ulong fileID) {
            DoFileChecks (fileID);

            return _dbOperations.GetDuplicateFiles (fileID);
        }

        /// <summary>
        /// This method returns the file narration of the file for the passed file id.
        /// </summary>
        /// <param name="fileID">Id of the file, the narration of which is sought.</param>
        /// <returns>Narration of the file sought.</returns>
        public string GetFileNarration (ulong fileID) {
            DoFileChecks (fileID);

            return this._dbOperations.GetFileNarration (fileID);
        }

        /// <summary>
        /// This method returns the file save date-time for the passed file id.
        /// </summary>
        /// <param name="fileID">Id of the file, the save date-time stamp of which is sought.</param>
        /// <returns>Date-time when the file was saved.</returns>
        public DateTime GetFileSaveDateTime (ulong fileID) {
            DoFileChecks (fileID);

            return this._dbOperations.GetFileSaveDateTime (fileID);
        }

        /// <summary>
        /// This method sets a file's deletion date-time.
        /// </summary>
        /// <param name="fileID">Id of the file, the deletion date-time stamp of which is to be set.</param>
        /// <param name="deletionDateTime">The deletion date-time to be set</param>
        public void SetFileDeletionDateTime (ulong fileID, DateTime deletionDateTime) {
            DoFileChecks (fileID);

            DateTime fileSaveDateTime = _dbOperations.GetFileSaveDateTime (fileID);

            Debug.Print ("Save DateTime: " + fileSaveDateTime + "; Deletion DateTime: " + deletionDateTime);

            if (deletionDateTime <= fileSaveDateTime) {
                throw new MfsIllegalOperationException ("Deletion date should not occur before file save date.");
            }

            if (deletionDateTime <= DateTime.Now) {
                throw new MfsIllegalOperationException ("Deletion date should occur after current date.");
            }

            _dbOperations.SetDeletionDateTime (fileID, deletionDateTime);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.DELETION_DATETIME_SET, DateTime.Now, deletionDateTime.ToString (), null);
        }

        /// <summary>
        /// This method returns a file's deletion date-time.
        /// </summary>
        /// <param name="fileID">Id of the file, the save date-time stamp of which is sought</param>
        /// <returns>A DateTime object that indicates the deletion date-time.</returns>
        public DateTime GetFileDeletionDateTime (ulong fileID) {
            DoFileChecks (fileID);

            return _dbOperations.GetDeletionDateTime (fileID);
        }

        /// <summary>
        /// This method updates the name of a file already saved on the system.
        /// </summary>
        /// <param name="fileID">Id of the file that has to be renamed.</param>
        /// <param name="newName">New name of the file.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        public bool UpdateFileName (ulong fileID, string newName) {
            DoFileChecks (fileID);
            ValidateString (newName, ValidationCheckType.FILE_NAME);

            bool updated = _dbOperations.UpdateFileName (fileID, newName);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.FILENAME_UPDATED, DateTime.Now, newName, null);

            return updated;
        }

        /// <summary>
        /// This method updates the narration of a file already saved on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the narration of which has to be updated.</param>
        /// <param name="newNarration">New narration of the file.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        public bool UpdateFileNarration (ulong fileID, string newNarration) {
            DoFileChecks (fileID);
            ValidateString (newNarration, ValidationCheckType.FILE_NARRATION);

            bool updated = _dbOperations.UpdateFileNarration (fileID, newNarration);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.FILE_NARRATION_UPDATED, DateTime.Now, newNarration, null);

            return updated;
        }

        /// <summary>
        /// This method updates the file save date-time of a file already saved on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the save date-time of which has to be updated.</param>
        /// <param name="newWhen">New save date-time of the file.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        public bool UpdateFileSaveDateTime (ulong fileID, DateTime newWhen) {
            DoFileChecks (fileID);

            bool updated = _dbOperations.UpdateFileSaveDateTime (fileID, newWhen);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.FILE_SAVEDATETIME_UPDATED, DateTime.Now, newWhen.ToString (), null);

            return updated;
        }

        /// <summary>
        /// This method updates the file deletion date-time of a file already saved on the system.
        /// </summary>
        /// <param name="fileID">Id of the file, the deletion date-time of which has to be updated.</param>
        /// <param name="newDeletionDateTime">New deletion date-time of the file.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        public bool UpdateFileDeletionDateTime (ulong fileID, DateTime newDeletionDateTime) {
            DoFileChecks (fileID);

            bool updated = _dbOperations.UpdateFileDeletionDateTime (fileID, newDeletionDateTime);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.DELETION_DATETIME_UPDATED, DateTime.Now, newDeletionDateTime.ToString (), null);

            return updated;
        }

        /// <summary>
        /// This method resets a file's deletion date-time.
        /// </summary>
        /// <param name="fileID">Id of the file the deletion date-time of which has to be reset.</param>
        public void ResetDeletionDateTime (ulong fileID) {
            DoFileChecks (fileID);

            _dbOperations.ResetDeletionDateTime (fileID);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.DELETION_DATETIME_RESET, DateTime.Now, null, null);
        }

        private bool IndexFile (string fileName, string fileNarration, byte[] fileData, DateTime when, ulong fileID, int versionNumber) {
            Debug.Print ("Indexing file in indexer.");

            // TODO: Remove this and add indexing for byte[] array:
            string fData = StringUtils.ConvertToString (fileData);

            Dictionary<ulong, string> dictionary = new Dictionary<ulong, string> ();
            dictionary.Add (fileID, fData);
            _indexer.Index (dictionary, IndexContentType.FILE_CONTENT);

            dictionary = new Dictionary<ulong, string> ();
            dictionary.Add (fileID, fileNarration);
            _indexer.Index (dictionary, IndexContentType.FILE_NARRRATION);

            return true;
        }

        #endregion << File Save / Retrieval / Deletion Instance Operations >>

        #region << File-logging Operations >>

        public static List<MfsFileLogEntry> RetrieveFileLogs (string userID, ulong fileID) {
            if (userID == null || userID == string.Empty) {
                throw new MfsIllegalArgumentException ("User id may not be null or empty.");
            }

            userID = ProcessUserIDStr (userID);

            if (!DoesUserExist (userID)) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            if (fileID == 0) {
                throw new MfsIllegalArgumentException ("File id cannot be zero.");
            }

            return FileLogger.RetrieveFileLogs (userID, fileID);
        }

        public static List<MfsFileLogEntry> RetrieveUserFileLogs (string userID) {
            if (userID == null || userID.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("User id string cannot be null or empty.");
            }

            userID = ProcessUserIDStr (userID);

            if (!DoesUserExist (userID)) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            return FileLogger.RetrieveAllFilesLogs (userID);
        }

        public static int DeleteFileLogs (string userID, ulong fileID) {
            if (userID == null || userID.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("User id string cannot be null or empty.");
            }

            userID = ProcessUserIDStr (userID);

            if (!DoesUserExist (userID)) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            if (fileID == 0) {
                throw new MfsIllegalArgumentException ("File id cannot be zero.");
            }

            return FileLogger.DeleteFileLogs (userID, fileID);
        }

        public static int DeleteUserFileLogs (string userID) {
            if (userID == null || userID.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("User id string cannot be null or empty.");
            }

            userID = ProcessUserIDStr (userID);

            if (!DoesUserExist (userID)) {
                throw new MfsNonExistentUserException ("User does not exist.");
            }

            return FileLogger.DeleteUserLogs (userID);
        }

        #endregion << File-logging Operations >>

        #region << File Retrieval in Date/Time Ranges >>

        public List<ulong> GetAllFiles () {
            return _dbOperations.GetAllFiles ();
        }

        public List<ulong> GetFilesInDateRange (DateTime startDate, DateTime endDate) {
            return _dbOperations.GetFilesInDateRange (startDate, endDate);
        }

        public List<ulong> GetFilesInDateTimeRange (DateTime startDateTime, DateTime endDateTime) {
            return _dbOperations.GetFilesInDateTimeRange (startDateTime, endDateTime);
        }

        public List<ulong> GetFilesOnDate (DateTime onDate) {
            return _dbOperations.GetFilesOnDate (onDate);
        }

        public List<ulong> GetFilesAtDateTime (DateTime onDateTime) {
            return _dbOperations.GetFilesOnDateTime (onDateTime);
        }

        public List<ulong> GetFilesBeforeDate (DateTime beforeDate) {
            return _dbOperations.GetFilesBeforeDate (beforeDate);
        }

        public List<ulong> GetFilesBeforeDateTime (DateTime beforeDateTime) {
            return _dbOperations.GetFilesBeforeDateTime (beforeDateTime);
        }

        public List<ulong> GetFilesBeforeAndOnDate (DateTime beforeOnDate) {
            return _dbOperations.GetFilesBeforeAndOnDate (beforeOnDate);
        }

        public List<ulong> GetFilesBeforeAndAtDateTime (DateTime beforeOnDateTime) {
            return _dbOperations.GetFilesBeforeAndOnDateTime (beforeOnDateTime);
        }

        public List<ulong> GetFilesAfterDate (DateTime afterDate) {
            return _dbOperations.GetFilesAfterDate (afterDate);
        }

        public List<ulong> GetFilesAfterDateTime (DateTime afterDateTime) {
            return _dbOperations.GetFilesAfterDateTime (afterDateTime);
        }

        public List<ulong> GetFilesAfterAndOnDate (DateTime afterOnDate) {
            return _dbOperations.GetFilesAfterAndOnDate (afterOnDate);
        }

        public List<ulong> GetFilesAfterAndAtDateTime (DateTime afterOnDateTime) {
            return _dbOperations.GetFilesAfterAndOnDateTime (afterOnDateTime);
        }

        #endregion << File Retrieval in Date/Time Ranges >>

        #region << Document Operations >>

        public DocumentType GetDocumentType (ulong documentID) {
            if (documentID == 0) {
                throw new MfsIllegalArgumentException ("Document id may not be zero.");
            }

            if (_dbOperations.DoesFileExist (documentID)) {
                return DocumentType.FILE;
            }
            if (_dbOperations.DoesNoteExist (documentID)) {
                return DocumentType.NOTE;
            }
            if (_dbOperations.DoesUrlExist (documentID)) {
                return DocumentType.URL;
            }
            if (_dbOperations.DoesVCardExist (documentID)) {
                return DocumentType.VCARD;
            }

            throw new MfsNonExistentResourceException ("Non-existent document.");
        }

        public bool DeleteDocument (ulong docID) {
            DoDocumentChecks (docID);

            if (_dbOperations.DoesFileExist (docID)) {
                return _dbOperations.DeleteFile (docID) > 0;
            }
            if (_dbOperations.DoesNoteExist (docID)) {
                return _dbOperations.DeleteNote (docID) > 0;
            }
            if (_dbOperations.DoesUrlExist (docID)) {
                return _dbOperations.DeleteUrl (docID) > 0;
            }
            if (_dbOperations.DoesVCardExist (docID)) {
                return _dbOperations.DeleteVCard (docID) > 0;
            }

            throw new MfsNonExistentResourceException ("Non-existent document.");
        }

        #endregion << Document Operations >>

        #region << Aspect-related Operations >>

        /// <summary>
        /// This method creates a new aspect within the system.
        /// </summary>
        /// <param name="aspectName">Name of the new aspect.</param>
        /// <param name="aspectDesc">Description of the new aspect.</param>
        /// <returns>An id that uniquely identifies the aspect across the entire system, if the call was successful.</returns>
        public ulong CreateAspect (string aspectName, string aspectDesc) {
            ValidateString (aspectName, ValidationCheckType.ASPECT_NAME);
            ValidateString (aspectDesc, ValidationCheckType.ASPECT_DESC);
            DoAspectChecks (aspectName);
            if (DoesAspectExist (aspectName) == true) {
                throw new MfsDuplicateNameException ("Aspect name already exists.");
            }

            return _dbOperations.CreateAspect (aspectName, aspectDesc);
        }

        /// <summary>
        /// This method gets the aspect name associated with an aspect id.
        /// </summary>
        /// <param name="aspectID">The aspect id for which this information is sought.</param>
        /// <returns>Name of the aspect sought.</returns>
        public void GetAspectNameAndDesc (ulong aspectID, out string aspectName, out string aspectDesc) {
            DoAspectChecks (aspectID);

            _dbOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
        }

        public List<ulong> GetAllAspects () {
            return _dbOperations.GetAllAspects ();
        }

        /// <summary>
        /// This method updates the name of an existing aspect.
        /// </summary>
        /// <param name="aspectID">The aspect id, the name of which has to be updated.</param>
        /// <param name="newAspectName">New aspect name.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        public bool UpdateAspectName (ulong aspectID, string newAspectName) {
            ValidateString (newAspectName, ValidationCheckType.ASPECT_NAME);
            DoAspectChecks (aspectID);

            return _dbOperations.UpdateAspectName (aspectID, newAspectName);
        }

        /// <summary>
        /// This method updates the description of an existing aspect.
        /// </summary>
        /// <param name="aspectID">The aspect id, the description of which has to be updated.</param>
        /// <param name="newAspectDesc">New aspect description.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        public bool UpdateAspectDesc (ulong aspectID, string newAspectDesc) {
            ValidateString (newAspectDesc, ValidationCheckType.ASPECT_DESC);
            DoAspectChecks (aspectID);

            return _dbOperations.UpdateAspectDesc (aspectID, newAspectDesc);
        }

        /// <summary>
        /// This method tells the client whether an aspect exists or not.
        /// </summary>
        /// <param name="aspectID">The aspect id for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this aspect exists or not.</returns>
        public bool DoesAspectExist (ulong aspectID) {
            if (aspectID == 0) {
                throw new MfsIllegalArgumentException ("Aspect id cannot be zero.");
            }

            return _dbOperations.DoesAspectExist (aspectID);
        }

        /// <summary>
        /// This method tells the client whether an aspect exists or not.
        /// </summary>
        /// <param name="aspectName">The aspect name for which this information is sought.</param>
        /// <returns>A boolean value that indicates whether this aspect exists or not.</returns>
        public bool DoesAspectExist (string aspectName) {
            DoAspectChecks (aspectName);

            return _dbOperations.DoesAspectExist (aspectName);
        }

        /// <summary>
        /// This method deletes an aspect.
        /// </summary>
        /// <param name="aspectID">Id of the aspect that has to be deleted.</param>
        /// <returns>An integer value that indicates how many aspects have been deleted. Is always one.</returns>
        public int DeleteAspect (ulong aspectID) {
            DoAspectChecks (aspectID);

            return _dbOperations.DeleteAspect (aspectID);
        }

        public ulong GetAspectIDFromName (string aspectName) {
            DoAspectChecks (aspectName);
            if (!DoesAspectExist (aspectName)) {
                throw new MfsNonExistentResourceException ("Aspect name does not exist.");
            }

            return _dbOperations.GetAspectIDFromName (aspectName);
        }

        /// <summary>
        /// This method deletes all aspects within the system. Needless to add, this should be used very carefully.
        /// </summary>
        /// <returns>An integer value indicating how many aspects have been deleted.</returns>
        public int DeleteAllAspectsInSystem () {
            return _dbOperations.DeleteAllAspectsInSystem ();
        }

        #endregion << Aspect-related Operations >>

        #region << Aspect Group-related Operations >>

        public ulong CreateAspectGroup (ulong parentAspectGroupID, string aspectGroupName, string aspectGroupDesc) {
            if (aspectGroupName == null || aspectGroupName.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Aspect group name may not be null or empty.");
            }

            if (DoesAspectGroupExistAtLevel (parentAspectGroupID, aspectGroupName)) {
                throw new MfsDuplicateNameException ("Aspect group already exists at the specified level.");
            }

            if (!DoesAspectGroupExist (parentAspectGroupID)) {
                throw new MfsNonExistentResourceException ("Parent aspect group does not exist.");
            }

            if (aspectGroupDesc == null) {
                throw new MfsIllegalArgumentException ("Aspect group description may not be null.");
            }

            return _dbOperations.CreateAspectGroup (parentAspectGroupID, aspectGroupName, aspectGroupDesc);
        }

        public bool DoesAspectGroupExistAtLevel (ulong parentAspectGroupID, string aspectGroupName) {
            return _dbOperations.DoesAspectGroupExistAtLevel (parentAspectGroupID, aspectGroupName);
        }

        public void GetAspectGroupNameAndDesc (ulong aspectGroupID, out string aspectGroupName, out string aspectGroupDesc) {
            if (!DoesAspectGroupExist (aspectGroupID)) {
                throw new MfsNonExistentResourceException ("Aspect group does not exist.");
            }

            _dbOperations.GetAspectGroupNameAndDesc (aspectGroupID, out aspectGroupName, out aspectGroupDesc);
        }

        public bool DoesAspectGroupExist (ulong aspectGroupID) {
            return _dbOperations.DoesAspectGroupExist (aspectGroupID);
        }

        public List<ulong> GetChildAspectGroups (ulong parentAspectID) {
            if (!DoesAspectGroupExist (parentAspectID)) {
                throw new MfsNonExistentResourceException ("Aspect id does not exist.");
            }

            return _dbOperations.GetChildAspectGroups (parentAspectID);
        }

        public int GetNumAspectsInAspectGroup (ulong aspectGroupID) {
            return _dbOperations.GetNumAspectsInAspectGroup (aspectGroupID);
        }

        public int GetNumAspectGroupsInAspectGroup (ulong aspectGroupID) {
            return _dbOperations.GetNumAspectGroupsInAspectGroup (aspectGroupID);
        }

        public int DeleteAspectGroup (ulong aspectGroupID) {
            if (aspectGroupID == 0) {
                throw new MfsIllegalOperationException ("Cannot delete root aspect group.");
            }

            if (!DoesAspectGroupExist (aspectGroupID)) {
                throw new MfsNonExistentResourceException ("Aspect group does not exist.");
            }

            if (GetNumAspectsInAspectGroup (aspectGroupID) + GetNumAspectGroupsInAspectGroup (aspectGroupID) > 0) {
                throw new MfsIllegalOperationException ("Cannot delete non-empty aspect group.");
            }

            return _dbOperations.DeleteAspectGroup (aspectGroupID);
        }

        public List<ulong> GetFilesWithExtension (string fileExtension) {
            if (fileExtension == null || fileExtension.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("File extension cannot be null or empty.");
            }

            return _dbOperations.GetFilesWithExtension (fileExtension.ToLower ());
        }

        #endregion << Aspect Group-related Operations >>

        #region << Aspects-Documents Operations >>

        /// <summary>
        /// This method applies an aspect to a document.
        /// </summary>
        /// <param name="aspectID">Aspect to be applied.</param>
        /// <param name="documentID">Document to be applied aspect to.</param>
        /// <returns>A boolean value indicating if the operation was successful.</returns>
        public bool ApplyAspectToDocument (ulong aspectID, ulong documentID) {
            DoAspectChecks (aspectID);
            DoDocumentChecks (documentID);

            return _dbOperations.ApplyAspectToDocument (aspectID, documentID);
        }

        /// <summary>
        /// This method applies a single aspect to multiple documents.
        /// </summary>
        /// <param name="aspectID">Aspect to be applied.</param>
        /// <param name="documentIDs">List of documents to which aspect is to be applied.</param>
        public void ApplyAspectToDocuments (ulong aspectID, List<ulong> documentIDs) {
            ValidateList (documentIDs, false, "Document");
            DoAspectChecks (aspectID);
            foreach (ulong documentID in documentIDs) {
                DoDocumentChecks (documentID);
            }

            foreach (ulong documentID in documentIDs) {
                _dbOperations.ApplyAspectToDocument (aspectID, documentID);
            }
        }

        /// <summary>
        /// This method applies multiple aspects to a single document.
        /// </summary>
        /// <param name="aspectIDs">List of aspects to apply to document.</param>
        /// <param name="documentID">Document to which aspects are to be applied.</param>
        public void ApplyAspectsToDocument (List<ulong> aspectIDs, ulong documentID) {
            ValidateList (aspectIDs, false, "Document");
            DoDocumentChecks (documentID);
            foreach (ulong aspectID in aspectIDs) {
                DoAspectChecks (aspectID);
            }

            foreach (ulong aspectID in aspectIDs) {
                _dbOperations.ApplyAspectToDocument (aspectID, documentID);
            }
        }

        /// <summary>
        /// Applikes multiple aspects to multiple documents. This results in a Cartesian product.
        /// </summary>
        /// <param name="aspectIDs">List of aspects to apply to list of documents.</param>
        /// <param name="documentIDs">List of documents to which list of aspects are to be applied.</param>
        public void ApplyAspectsToDocuments (List<ulong> aspectIDs, List<ulong> documentIDs) {
            ValidateList (aspectIDs, false, "Aspect");
            ValidateList (documentIDs, false, "Document");
            foreach (ulong aspectID in aspectIDs) {
                DoAspectChecks (aspectID);
            }
            foreach (ulong documentID in documentIDs) {
                DoDocumentChecks (documentID);
            }

            foreach (ulong aspectID in aspectIDs) {
                foreach (ulong documentID in documentIDs) {
                    _dbOperations.ApplyAspectToDocument (aspectID, documentID);
                }
            }
        }

        /// <summary>
        /// This method tells the caller if an aspect has been applied to a document.
        /// </summary>
        /// <param name="aspectID">Aspect to be checked if applied.</param>
        /// <param name="documentID">Document to be checked whether aspect has been applied to or not.</param>
        /// <returns>A boolean value indicating whether or not the aspect has been applied to the document.</returns>
        public bool IsAspectAppliedToDocument (ulong aspectID, ulong documentID) {
            DoAspectChecks (aspectID);
            DoDocumentChecks (documentID);

            return _dbOperations.IsAspectAppliedToDocument (aspectID, documentID);
        }

        /// <summary>
        /// This method "unapplies" an aspect from a document.
        /// </summary>
        /// <param name="aspectID">Aspect that is to be "unapplied."</param>
        /// <param name="documentID">Document from which aspect is to be "unapplied."</param>
        /// <returns></returns>
        public bool UnapplyAspectFromDocument (ulong aspectID, ulong documentID) {
            DoAspectChecks (aspectID);
            DoDocumentChecks (documentID);

            return _dbOperations.UnapplyAspectFromDocument (aspectID, documentID);
        }

        /// <summary>
        /// This method "unapplies" all aspects from a document.
        /// </summary>
        /// <param name="documentID">Document from which all aspects are to be "unapplied."</param>
        /// <returns>Number of aspects that were "unapplied" from the document.</returns>
        public int UnapplyAllAspectsFromDocument (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.UnapplyAllAspectsFromDocument (documentID);
        }

        /// <summary>
        /// This method "unapplies" an aspect from all documents.
        /// </summary>
        /// <param name="aspectID">Aspect that is to be "unapplied."</param>
        /// <returns>Number of documents from which the apsect was "unapplied."</returns>
        public int UnapplyAspectFromAllDocuments (ulong aspectID) {
            DoAspectChecks (aspectID);

            return _dbOperations.UnapplyAspectFromAllDocuments (aspectID);
        }

        /// <summary>
        /// This method gets the aspects that have been applied to a document.
        /// </summary>
        /// <param name="documentID">Document for which this information is sought.</param>
        /// <returns>List of aspects which have been applied to this document.</returns>
        public List<ulong> GetAspectsAppliedOnDocument (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.GetAspectsAppliedOnDocument (documentID);
        }

        /// <summary>
        /// This method gets all the documents that an aspect has been applied to.
        /// </summary>
        /// <param name="aspectID">Aspect for which this information is sought.</param>
        /// <returns>List of documents to which this aspect has been applied.</returns>
        public List<ulong> GetDocumentsAppliedWithAspect (ulong aspectID) {
            DoAspectChecks (aspectID);

            return _dbOperations.GetDocumentsAppliedWithAspect (aspectID);
        }

        #endregion << Aspects-Documents Operations >>

        #region << Briefcase-related Operations >>

        /// <summary>
        /// This method creates a new briefcase within the system.
        /// </summary>
        /// <param name="briefcaseName">Name of the new briefcase</param>
        /// <param name="briefcaseDesc">Description of the new briefcase</param>
        /// <returns>An id that uniquely identifies the aspect across the entire system, if the call was successful.</returns>
        public ulong CreateBriefcase (string briefcaseName, string briefcaseDesc) {
            ValidateString (briefcaseName, ValidationCheckType.BRIEFCASE_NAME);
            ValidateString (briefcaseDesc, ValidationCheckType.BRIEFCASE_DESC);
            DoBriefcaseChecks (briefcaseName);
            if (DoesBriefcaseExist (briefcaseName)) {
                throw new MfsDuplicateNameException ("Briefcase name already exists.");
            }

            return _dbOperations.CreateBriefcase (briefcaseName, briefcaseDesc);
        }

        /// <summary>
        /// This method deletes a briefcase.
        /// </summary>
        /// <param name="briefcaseID">Id of the briefcase that has to be deleted.</param>
        /// <returns>An integer value that indicates how many briefcases have been deleted. Is always one.</returns>
        public int DeleteBriefcase (ulong briefcaseID) {
            DoBriefcaseChecks (briefcaseID);
            if (briefcaseID == MfsOperations.GLOBAL_BRIEFCASE_ID) {
                throw new MfsIllegalOperationException ("Global briefcase cannot be deleted.");
            }

            return _dbOperations.DeleteBriefcase (briefcaseID);
        }

        /// <summary>
        /// This method tells the client whether an briefcase exists or not.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id for which this information is sought</param>
        /// <returns>A boolean value that indicates whether this briefcase exists or not.</returns>
        public bool DoesBriefcaseExist (ulong briefcaseID) {
            if (briefcaseID == 0) {
                throw new MfsIllegalArgumentException ("Briefcase id cannot be zero.");
            }

            return _dbOperations.DoesBriefcaseExist (briefcaseID);
        }

        /// <summary>
        /// This method tells the client whether an briefcase exists or not.
        /// </summary>
        /// <param name="briefcaseName">The briefcase name for which this information is sought</param>
        /// <returns>A boolean value that indicates whether this briefcase exists or not.</returns>
        public bool DoesBriefcaseExist (string briefcaseName) {
            DoBriefcaseChecks (briefcaseName);

            return _dbOperations.DoesBriefcaseExist (briefcaseName);
        }

        /// <summary>
        /// This method gets the briefcase name associated with a briefcase id.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id for which this information is sought.</param>
        /// <returns>Name of the briefcase sought.</returns>
        public void GetBriefcaseNameAndDesc (ulong briefcaseID, out string briefcaseName, out string briefcaseDesc) {
            DoBriefcaseChecks (briefcaseID);

            _dbOperations.GetBriefcaseNameAndDesc (briefcaseID, out briefcaseName, out briefcaseDesc);
        }

        public ulong GetBriefcaseIDFromName (string briefcaseName) {
            DoBriefcaseChecks (briefcaseName);
            if (!DoesBriefcaseExist (briefcaseName)) {
                throw new MfsNonExistentResourceException ("Briefcase name does not exist.");
            }

            return _dbOperations.GetBriefcaseIDFromName (briefcaseName);
        }

        public List<ulong> GetAllBriefcases () {
            return _dbOperations.GetAllBriefcases ();
        }

        /// <summary>
        /// This method updates the name of an existing briefcase.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id, the name of which has to be updated.</param>
        /// <param name="newBriefcaseName">New briefcase name.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        public bool UpdateBriefcaseName (ulong briefcaseID, string newBriefcaseName) {
            DoBriefcaseChecks (briefcaseID);
            ValidateString (newBriefcaseName, ValidationCheckType.BRIEFCASE_NAME);

            return _dbOperations.UpdateBriefcaseName (briefcaseID, newBriefcaseName);
        }

        /// <summary>
        /// This method updates the description of an existing briefcase.
        /// </summary>
        /// <param name="briefcaseID">The briefcase id, the description of which has to be updated.</param>
        /// <param name="newBriefcaseDesc">New briefcase description.</param>
        /// <returns>A boolean value indicating whether the call was successful or not.</returns>
        public bool UpdateBriefcaseDesc (ulong briefcaseID, string newBriefcaseDesc) {
            DoBriefcaseChecks (briefcaseID);
            ValidateString (newBriefcaseDesc, ValidationCheckType.BRIEFCASE_DESC);

            return _dbOperations.UpdateBriefcaseDesc (briefcaseID, newBriefcaseDesc);
        }

        /// <summary>
        /// This method deletes all aspects within the system. Needless to add, this should be used very carefully.
        /// </summary>
        /// <returns>An integer value indicating how many aspects have been deleted.</returns>
        public int DeleteAllBriefcasesInSystem () {
            return _dbOperations.DeleteAllBriefcasesInSystem ();
        }

        #endregion << Briefcase-related Operations >>

        #region << Briefcases-Documents Operations >>

        /// <summary>
        /// This method returns the briefcase id that contains a document.
        /// </summary>
        /// <param name="documentID">Document for which this information is sought.</param>
        /// <returns>Id of the briefcase.</returns>
        public ulong GetContainingBriefcase (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.GetContainingBriefcase (documentID);
        }

        /// <summary>
        /// This method moves a document to a briefcase.
        /// </summary>
        /// <param name="documentID">Id of the document to be moved.</param>
        /// <param name="briefcaseID">Id of the briefcase to be moved to.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        public bool MoveDocumentToBriefcase (ulong documentID, ulong briefcaseID) {
            DoDocumentChecks (documentID);
            DoBriefcaseChecks (briefcaseID);

            return _dbOperations.MoveDocumentToBriefcase (documentID, briefcaseID);
        }

        /// <summary>
        /// This method removes a document from its containing briefcase to the global briefcase.
        /// </summary>
        /// <param name="documentID">Id of the document to be moved.</param>
        /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
        public bool RemoveDocumentFromBriefcase (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.RemoveDocumentFromBriefcase (documentID);
        }

        /// <summary>
        /// This method gets all the documents in a briefcase.
        /// </summary>
        /// <param name="briefcaseID">Id of the briefcase from which its contained documents are sought.</param>
        /// <returns>A list of documents that are contained within the briefcase.</returns>
        public List<ulong> GetDocumentsInBriefcase (ulong briefcaseID) {
            DoBriefcaseChecks (briefcaseID);

            return _dbOperations.GetDocumentsInBriefcase (briefcaseID);
        }

        #endregion << Briefcases-Documents Operations >>

        #region << Collection-related Operations >>

        /// <summary>
        /// This method creates a new collection.
        /// </summary>
        /// <param name="collectionName">Name of the new collection.</param>
        /// <param name="collectionDesc">Description of the new collection.</param>
        /// <returns>A unique integer that identifies the collection across the entire system.</returns>
        public ulong CreateCollection (string collectionName, string collectionDesc) {
            ValidateString (collectionName, ValidationCheckType.COLLECTION_NAME);
            ValidateString (collectionDesc, ValidationCheckType.COLLECTION_DESC);
            DoCollectionChecks (collectionName);
            if (DoesCollectionExist (collectionName)) {
                throw new MfsDuplicateNameException ("Collection name already exists.");
            }

            return _dbOperations.CreateCollection (collectionName, collectionDesc);
        }

        /// <summary>
        /// This method deletes a collection.
        /// </summary>
        /// <param name="collectionID">The collection to be deleted.</param>
        /// <returns>Number of collections deleted. Always one.</returns>
        public int DeleteCollection (ulong collectionID) {
            DoCollectionChecks (collectionID);

            return _dbOperations.DeleteCollection (collectionID);
        }

        /// <summary>
        /// This method tells the caller if a collection exists.
        /// </summary>
        /// <param name="collectionID">The collection name for which this information is sought.</param>
        /// <returns>A boolean value indicating whether the collection exists or not.</returns>
        public bool DoesCollectionExist (ulong collectionID) {
            if (collectionID == 0) {
                throw new MfsIllegalArgumentException ("Collection id cannot be zero.");
            }

            return _dbOperations.DoesCollectionExist (collectionID);
        }

        /// <summary>
        /// This method tells the caller if a collection exists.
        /// </summary>
        /// <param name="collectionName">The collection name for which this information is sought.</param>
        /// <returns>A boolean value indicating whether the collection exists or not.</returns>
        public bool DoesCollectionExist (string collectionName) {
            DoCollectionChecks (collectionName);

            return _dbOperations.DoesCollectionExist (collectionName);
        }

        /// <summary>
        /// This method retrieves a collection's name.
        /// </summary>
        /// <param name="collectionID">Id of the collection the name of which is sought.</param>
        /// <returns>Name of the collection.</returns>
        public void GetCollectionNameAndDesc (ulong collectionID, out string collectionName, out string collectionDesc) {
            DoCollectionChecks (collectionID);

            _dbOperations.GetCollectionNameAndDesc (collectionID, out collectionName, out collectionDesc);
        }

        public ulong GetCollectionIDFromName (string collectionName) {
            DoCollectionChecks (collectionName);
            if (!DoesCollectionExist (collectionName)) {
                throw new MfsNonExistentResourceException ("Collection name does not exist.");
            }

            return _dbOperations.GetCollectionIDFromName (collectionName);
        }

        public List<ulong> GetAllCollections () {
            return _dbOperations.GetAllCollections ();
        }

        /// <summary>
        /// This method updates a collection's name.
        /// </summary>
        /// <param name="collectionID">Id of the collection the name of which is to be updated.</param>
        /// <param name="newCollectionName">New name for the collection.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        public bool UpdateCollectionName (ulong collectionID, string newCollectionName) {
            DoCollectionChecks (collectionID);
            ValidateString (newCollectionName, ValidationCheckType.COLLECTION_NAME);

            return _dbOperations.UpdateCollectionName (collectionID, newCollectionName);
        }

        /// <summary>
        /// This method updates the description of a collection.
        /// </summary>
        /// <param name="collectionID">Id of the collection the description of which is to be updated.</param>
        /// <param name="newCollectionDesc">New description for the collection.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        public bool UpdateCollectionDesc (ulong collectionID, string newCollectionDesc) {
            DoCollectionChecks (collectionID);
            ValidateString (newCollectionDesc, ValidationCheckType.COLLECTION_DESC);

            return _dbOperations.UpdateCollectionDesc (collectionID, newCollectionDesc);
        }

        /// <summary>
        /// This method deletes all collections within the system. Needless to add, this should be used very carefully.
        /// </summary>
        /// <returns>An integer value indicating how many collections have been deleted.</returns>
        public int DeleteAllCollectionsInSystem () {
            return _dbOperations.DeleteAllCollectionsInSystem ();
        }

        #endregion << Collection-related Operations >>

        #region << Collections-Documents Operations >>

        public bool AddDocumentToCollection (ulong documentID, ulong collectionID) {
            DoDocumentChecks (documentID);
            DoCollectionChecks (collectionID);

            return _dbOperations.AddDocumentToCollection (documentID, collectionID);
        }

        public bool RemoveDocumentFromCollection (ulong documentID, ulong collectionID) {
            DoDocumentChecks (documentID);
            DoCollectionChecks (collectionID);

            return _dbOperations.RemoveDocumentFromCollection (documentID, collectionID);
        }

        public List<ulong> GetCollectionsWithDocument (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.GetCollectionsWithDocument (documentID);
        }

        public List<ulong> GetDocumentsInCollection (ulong collectionID) {
            DoCollectionChecks (collectionID);

            return _dbOperations.GetDocumentsInCollection (collectionID);
        }

        public void AddDocumentToCollections (ulong documentID, List<ulong> collectionIDs) {
            DoDocumentChecks (documentID);
            ValidateList (collectionIDs, false, "Collection");

            foreach (ulong collectionID in collectionIDs) {
                DoCollectionChecks (collectionID);
            }

            foreach (ulong collectionID in collectionIDs) {
                _dbOperations.AddDocumentToCollection (documentID, collectionID);
            }
        }

        public void AddDocumentsToCollection (List<ulong> documentIDs, ulong collectionID) {
            ValidateList (documentIDs, false, "Document");
            DoCollectionChecks (collectionID);

            foreach (ulong fileID in documentIDs) {
                DoDocumentChecks (fileID);
            }

            foreach (ulong documentID in documentIDs) {
                _dbOperations.AddDocumentToCollection (documentID, collectionID);
            }
        }

        public void AddDocumentsToCollections (List<ulong> documentIDs, List<ulong> collectionIDs) {
            ValidateList (documentIDs, false, "Document");
            ValidateList (collectionIDs, false, "Collection");

            foreach (ulong documentID in documentIDs) {
                DoDocumentChecks (documentID);
            }

            foreach (ulong collectionID in collectionIDs) {
                DoCollectionChecks (collectionID);
            }

            foreach (ulong documentID in documentIDs) {
                foreach (ulong collectionID in collectionIDs) {
                    _dbOperations.AddDocumentToCollection (documentID, collectionID);
                }
            }
        }

        public bool IsDocumentInCollection (ulong documentID, ulong collectionID) {
            DoDocumentChecks (documentID);
            DoCollectionChecks (collectionID);

            return _dbOperations.IsDocumentInCollection (documentID, collectionID);
        }

        public int RemoveDocumentFromAllCollections (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.RemoveDocumentFromAllCollections (documentID);
        }

        public int RemoveAllDocumentsFromCollection (ulong collectionID) {
            DoCollectionChecks (collectionID);

            return _dbOperations.RemoveAllDocumentsFromCollection (collectionID);
        }

        #endregion << Collections-Documents Operations >>

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
                throw new MfsFileVersionConflictException ("Conflict: another version of this file has already been checked in.");
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
            string lastPartOfFreshPath = "v" + (lastVersionNumber + 1).ToString ();

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
            MfsStorageDevice.SaveFileAsNewVersion (fileData, freshPath, fileAssumedName, password, archiveName);

            string fileFQPath = null;
            fileFQPath = freshPath + archiveName;

            int nextVersionNumber = lastVersionNumber + 1;

            // Calculate its hash before saving its meta-data:
            string fileHash = Hasher.GetFileHash (fileData);

            _dbOperations.SaveAsNextVersion (fileID, fileHash, comments, fileFQPath, nextVersionNumber);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.NEW_VERSION_CREATED, DateTime.Now, nextVersionNumber.ToString (), null);

            return nextVersionNumber;
        }

        public byte[] RetrieveLastFileVersion (ulong fileID, out int currentVersionNumber) {
            DoFileChecks (fileID);

            currentVersionNumber = _dbOperations.GetLastFileVersionNumber (fileID);

            if (currentVersionNumber == 0) {
                return RetrieveOriginalFile (fileID);
            }

            string fileNameWithPath;
            _dbOperations.GetFileVersionPath (fileID, currentVersionNumber, out fileNameWithPath);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.ACCESSED_VERSION, DateTime.Now, currentVersionNumber.ToString (), null);

            return MfsStorageDevice.RetrieveFile (fileNameWithPath);
        }

        // TODO: To add tests for this.
        public byte[] RetrieveFileVersion (ulong fileID, int versionNumber) {
            DoFileChecks (fileID);

            if (versionNumber == 0) {
                return RetrieveOriginalFile (fileID);
            }

            int lastVersionNumber = _dbOperations.GetLastFileVersionNumber (fileID);
            if (versionNumber == lastVersionNumber) {
                return RetrieveLastFileVersion (fileID, out versionNumber);
            }

            if (versionNumber < 0 || versionNumber > lastVersionNumber) {
                throw new MfsFileVersionException ("Illegal file version specified.");
            }

            string fileNameWithPath;
            _dbOperations.GetFileVersionPath (fileID, versionNumber, out fileNameWithPath);

            FileLogger.AddLogEntry (_userID, fileID, FileLogEntryType.ACCESSED_VERSION, DateTime.Now, versionNumber.ToString (), null);

            return MfsStorageDevice.RetrieveFile (fileNameWithPath);
        }

        public int GetLastFileVersionNumber (ulong fileID) {
            DoFileChecks (fileID);

            return _dbOperations.GetLastFileVersionNumber (fileID);
        }

        public string GetFileVersionHash (ulong fileID, int versionNumber) {
            DoFileChecks (fileID);
            DoVersionChecks (fileID, versionNumber);

            return _dbOperations.GetFileVersionHash (fileID, versionNumber);
        }

        public byte[] GetFileVersion (ulong fileID, int versionNumber) {
            DoFileChecks (fileID);
            DoVersionChecks (fileID, versionNumber);

            string fileNameWithPath;
            _dbOperations.GetFileVersionPath (fileID, versionNumber, out fileNameWithPath);

            return MfsStorageDevice.RetrieveFile (fileNameWithPath);
        }

        public void GetFileVersionDetails (ulong fileID, int versionNumber, out string comments, out DateTime whenDateTime) {
            DoFileChecks (fileID);
            DoVersionChecks (fileID, versionNumber);

            _dbOperations.GetFileVersionDetails (fileID, versionNumber, out comments, out whenDateTime);
        }

        public void GetFileVersionHistoryLog (ulong fileID, out string[] versionComments, out DateTime[] versionDateTimes) {
            DoFileChecks (fileID);

            _dbOperations.GetFileVersionHistoryLog (fileID, out versionComments, out versionDateTimes);
        }

        public void GetVersionDiff (ulong fileID, int versionNumber1, int versionNumber2, out byte[] fileData1, out byte[] fileData2) {
            // Validations _are_ happening in GetFileVersion () method. No fun in repeating them here and slowing down the application 
            // unnecessarily. The only other validation we do here is to see if the file version numbers are not the same.
            if (versionNumber1 == versionNumber2) {
                throw new MfsIllegalArgumentException ("Version numbers for diffing cannot be the same.");
            }

            fileData1 = GetFileVersion (fileID, versionNumber1);
            fileData2 = GetFileVersion (fileID, versionNumber2);
        }

        #endregion << File Version-related Operations >>

        #region << Url-related Operations >>

        public ulong AddUrl (string url, string description, DateTime when) {
            if (url == null || url.Length > MAX_URL_LENGTH) {
                throw new MfsIllegalArgumentException (
                                string.Format ("Url cannot be null, empty or greater than {0} chars.", MAX_FILENAME_LENGTH)
                            );
            }

            return _dbOperations.AddUrl (url, description, when);
        }

        public void GetUrlDetails (ulong urlID, out string url, out string description, out DateTime when) {
            DoUrlChecks (urlID);

            _dbOperations.GetUrlDetails (urlID, out url, out description, out when);
        }

        public bool UpdateUrl (ulong urlID, string newUrl) {
            DoUrlChecks (urlID);

            return _dbOperations.UpdateUrl (urlID, newUrl);
        }

        public bool UpdateUrlDescription (ulong urlID, string newDescription) {
            DoUrlChecks (urlID);

            return _dbOperations.UpdateUrlDescription (urlID, newDescription);
        }

        public bool UpdateUrlDateTime (ulong urlID, DateTime newWhen) {
            DoUrlChecks (urlID);

            return _dbOperations.UpdateUrlDateTime (urlID, newWhen);
        }

        public int DeleteUrl (ulong urlID) {
            DoUrlChecks (urlID);

            return _dbOperations.DeleteUrl (urlID);
        }

        public bool DoesUrlExist (ulong urlID) {
            if (urlID == 0) {
                throw new MfsIllegalArgumentException ("Url id cannot be zero.");
            }

            return _dbOperations.DoesUrlExist (urlID);
        }

        #endregion << Url-related Operations >>

        #region << Note-related Operations >>

        public ulong AddNote (MfsNote note) {
            if (note == null) {
                throw new MfsIllegalArgumentException ("Note object cannot be null.");
            }

            return _dbOperations.AddNote (note);
        }

        public int DeleteNote (ulong noteID) {
            DoNoteChecks (noteID);

            return _dbOperations.DeleteNote (noteID);
        }

        public MfsNote GetNote (ulong noteID) {
            DoNoteChecks (noteID);

            return _dbOperations.GetNote (noteID);
        }

        public DateTime GetNoteDateTime (ulong noteID) {
            return _dbOperations.GetNoteDateTime (noteID);
        }

        #endregion << Note-related Operations >>

        #region << Archiving Operations >>

        public void ArchiveFilesInGrouping (GroupingType groupingType, ulong groupingID, string opDirPath, string opArchiveName, string password) {
            if (password != null && password.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Password may not be empty.");
            }

            List<ulong> filesInGrouping = null;

            switch (groupingType) {
                case GroupingType.ASPECT:
                    DoAspectChecks (groupingID);
                    filesInGrouping = _dbOperations.GetDocumentsAppliedWithAspect (groupingID);
                    break;
                case GroupingType.BRIEFCASE:
                    DoBriefcaseChecks (groupingID);
                    filesInGrouping = _dbOperations.GetDocumentsInBriefcase (groupingID);
                    break;
                case GroupingType.COLLECTION:
                    DoCollectionChecks (groupingID);
                    filesInGrouping = _dbOperations.GetDocumentsInCollection (groupingID);
                    break;
            }

            List<byte[]> filesData = new List<byte[]> (filesInGrouping.Count);
            List<string> fileNames = new List<string> (filesInGrouping.Count);
            foreach (ulong fileID in filesInGrouping) {
                byte[] fileData = RetrieveOriginalFile (fileID);
                filesData.Add (fileData);
                string fileName = GetFileName (fileID);
                fileNames.Add (fileName);
            }

            MfsStorageDevice.ArchiveFiles (filesData, fileNames, opDirPath, opArchiveName, password);
        }

        #endregion << Archiving Operations >>

        #region << General Filter Methods >>

        public static List<ulong> FilterInvert (List<ulong> ipList, List<ulong> opList) {
            if (ipList == null || opList == null) {
                throw new MfsIllegalArgumentException ("Neither of the two list arguments may be null.");
            }
            if (ipList.Count == 0) {
                throw new MfsIllegalArgumentException ("I/p list may not be empty.");
            }
            if (!IsList1Superset (ipList, opList)) {
                throw new MfsIllegalArgumentException ("O/p list has to necessarily be a subset of i/p list.");
            }

            List<ulong> invertedList = new List<ulong> ();

            foreach (ulong ipListItem in ipList) {
                if (!opList.Contains (ipListItem)) {
                    invertedList.Add (ipListItem);
                }
            }

            return invertedList;
        }

        public static List<ulong> FilterCombineOR (List<ulong> list1, List<ulong> list2) {
            if (list1 == null || list2 == null) {
                throw new MfsIllegalArgumentException ("Neither of the two list arguments may be null.");
            }

            List<ulong> oredList = new List<ulong> ();

            foreach (ulong item1 in list1) {
                oredList.Add (item1);
            }

            foreach (ulong item2 in list2) {
                if (!oredList.Contains (item2)) {
                    oredList.Add (item2);
                }
            }

            return oredList;
        }

        public static List<ulong> FilterCombineOR (List<List<ulong>> lists) {
            List<ulong> oredList = new List<ulong> ();

            foreach (List<ulong> list in lists) {
                foreach (ulong item in list) {
                    if (!oredList.Contains (item)) {
                        oredList.Add (item);
                    }
                }
            }

            return oredList;
        }

        public static List<ulong> FilterCombineAND (List<ulong> list1, List<ulong> list2) {
            if (list1 == null || list2 == null) {
                throw new MfsIllegalArgumentException ("Neither of the two list arguments may be null.");
            }

            List<ulong> andedList = new List<ulong> ();

            foreach (ulong item in list1) {
                if (list2.Contains (item)) {
                    andedList.Add (item);
                }
            }

            foreach (ulong item in list2) {
                if (list1.Contains (item) && !andedList.Contains (item)) {
                    andedList.Add (item);
                }
            }

            return andedList;
        }

        public static List<ulong> FilterCombineAND (List<List<ulong>> lists) {
            List<ulong> andedList = new List<ulong> ();

            foreach (List<ulong> list in lists) {
                foreach (ulong item in list) {
                    if (DoAllListsHaveItem (lists, item)) {
                        andedList.Add (item);
                    }
                }
                return andedList;
            }

            return andedList;
        }

        public static List<ulong> FilterCombineEXOR (List<ulong> list1, List<ulong> list2) {
            if (list1 == null || list2 == null) {
                throw new MfsIllegalArgumentException ("Neither of the two list arguments may be null.");
            }

            List<ulong> exoredList = new List<ulong> ();

            foreach (ulong item in list1) {
                if (!list2.Contains (item)) {
                    exoredList.Add (item);
                }
            }

            foreach (ulong item in list2) {
                if (!list1.Contains (item) && !exoredList.Contains (item)) {
                    exoredList.Add (item);
                }
            }

            return exoredList;
        }

        #endregion << General Filter Methods >>

        #region << Aspect Filter Operations >>

        public List<ulong> FilterFilesWithinAspects (List<ulong> aspectIDs, List<ulong> fileIDs, FilterType filterType) {
            List<ulong> opFileIDs = new List<ulong> ();

            bool isIn = false;
            switch (filterType) {
                case FilterType.AND:
                    foreach (ulong fileID in fileIDs) {
                        foreach (ulong aspectID in aspectIDs) {
                            if (IsAspectAppliedToDocument (aspectID, fileID)) {
                                isIn = true;
                            } else {
                                isIn = false;
                                break;
                            }
                        }
                        if (isIn) {
                            opFileIDs.Add (fileID);
                            isIn = false;
                        }
                    }
                    break;
                case FilterType.OR:
                    foreach (ulong fileID in fileIDs) {
                        foreach (ulong aspectID in aspectIDs) {
                            if (IsAspectAppliedToDocument (aspectID, fileID)) {
                                isIn = true;
                                break;
                            } else {
                                isIn = false;
                            }
                        }
                        if (isIn) {
                            opFileIDs.Add (fileID);
                            isIn = false;
                        }
                    }
                    break;
            }

            return opFileIDs;
        }

        #endregion << Aspect Filter Operations >>

        #region << Document Bookmarking Operations >>

        public bool BookmarkDocument (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.BookmarkDocument (documentID);
        }

        public List<ulong> GetAllBookmarkedDocuments () {
            return _dbOperations.GetAllBookmarkedDocuments ();
        }

        public bool IsDocumentBookmarked (ulong fileID) {
            DoDocumentChecks (fileID);

            return _dbOperations.IsDocumentBookmarked (fileID);
        }

        public int DeleteDocumentBookmark (ulong documentID) {
            DoDocumentChecks (documentID);

            return _dbOperations.DeleteDocumentBookmark (documentID);
        }

        public int DeleteAllDocumentBookmarks () {
            return _dbOperations.DeleteAllDocumentBookmarks ();
        }

        #endregion << Document Bookmarking Operations >>

        #region << Schema Free Document-related Operations >>

        public ulong CreateSfd (string docName, DateTime when) {
            ValidateString (docName, ValidationCheckType.SCHEMA_FREE_DOC_NAME);
            DoSchemaFreeDocChecks (docName);
            if (DoesSfdExist (docName) == true) {
                throw new MfsDuplicateNameException ("Schema-free document name already exists.");
            }

            return _dbOperations.CreateSfd (docName, when);
        }

        public int DeleteSfd (ulong docID) {
            DoSFDChecks (docID);

            return _dbOperations.DeleteSfd (docID);
        }

        public int DeleteAllSfds () {
            return _dbOperations.DeleteAllSfds ();
        }

        public bool DoesSfdExist (ulong docID) {
            if (docID == 0) {
                throw new MfsIllegalArgumentException ("Schema-free document id cannot be zero.");
            }

            return _dbOperations.DoesSfdExist (docID);
        }

        public bool DoesSfdExist (string docName) {
            DoSchemaFreeDocChecks (docName);

            return _dbOperations.DoesSfdExist (docName);
        }

        public void GetSfdName (ulong docID, out string docName) {
            DoSFDChecks (docID);

            _dbOperations.GetSfdName (docID, out docName);
        }

        public ulong GetSfdIDFromName (string docName) {
            DoSchemaFreeDocChecks (docName);
            if (!DoesSfdExist (docName)) {
                throw new MfsNonExistentResourceException ("Schema-free document name does not exist.");
            }

            return _dbOperations.GetSfdIDFromName (docName);
        }

        public DateTime GetSfdSaveDateTime (ulong docID) {
            DoSFDChecks (docID);

            return _dbOperations.GetSfdSaveDateTime (docID);
        }

        public List<ulong> GetAllSfds () {
            return _dbOperations.GetAllSfds ();
        }

        public void AddPropertyToSfd (ulong docID, string key, string value) {
            DoSFDChecks (docID);
            if (key == null || key.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Key cannot be null or empty.");
            }
            if (value == null || value.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Value cannot be null or empty.");
            }
            if (_dbOperations.DoesSfdHaveKey (docID, key)) {
                throw new MfsDuplicateNameException ("Document already has a key by that name.");
            }

            _dbOperations.AddPropertyToSfd (docID, key, value);
        }

        public void AddPropertiesToSfd (ulong docID, Dictionary<string, string> properties) {
            DoSFDChecks (docID);
            HashSet<string> allKeys = new HashSet<string> (properties.Keys);
            foreach (string key in allKeys) {
                if (key == null || key.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException ("Key cannot be null or empty.");
                }
                string value = properties[key];
                if (value == null || value.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException ("Value cannot be null or empty.");
                }
                if (_dbOperations.DoesSfdHaveKey (docID, key)) {
                    throw new MfsDuplicateNameException ("Document already has a key by that name.");
                }
            }

            foreach (string key in allKeys) {
                string value = properties[key];
                _dbOperations.AddPropertyToSfd (docID, key, value);
            }
        }

        public bool DoesSfdHaveKey (ulong docID, string key) {
            DoSFDChecks (docID);
            if (key == null || key.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Key cannot be null or empty.");
            }

            return _dbOperations.DoesSfdHaveKey (docID, key);
        }

        public string GetValueForKeyInSfd (ulong docID, string key) {
            DoSFDChecks (docID);
            if (key == null || key.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Key cannot be null or empty.");
            }
            if (!_dbOperations.DoesSfdHaveKey (docID, key)) {
                throw new MfsNonExistentResourceException ("Key does not exist in schema-free document.");
            }

            return _dbOperations.GetValueForKeyInSfd (docID, key);
        }

        public bool UpdateValueForKeyInSfd (ulong docID, string key, string newValue) {
            DoSFDChecks (docID);
            if (key == null || key.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Key cannot be null or empty.");
            }
            if (newValue == null || newValue.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Value cannot be null or empty.");
            }
            if (!_dbOperations.DoesSfdHaveKey (docID, key)) {
                throw new MfsNonExistentResourceException ("Key does not exist in schema-free document.");
            }

            return _dbOperations.UpdateValueForKeyInSfd (docID, key, newValue);
        }

        public int DeleteKeyInSfd (ulong docID, string key) {
            DoSFDChecks (docID);
            if (key == null || key.Equals (string.Empty)) {
                throw new MfsIllegalArgumentException ("Key cannot be null or empty.");
            }
            if (!_dbOperations.DoesSfdHaveKey (docID, key)) {
                throw new MfsNonExistentResourceException ("Key does not exist in schema-free document.");
            }

            return _dbOperations.DeleteKeyInSfd (docID, key);
        }

        public List<string> GetAllKeysInSfd (ulong docID) {
            DoSFDChecks (docID);

            return _dbOperations.GetAllKeysInSfd (docID);
        }

        #endregion << Schema Free Document-related Operations >>

        #region << Schema Free Document Version-related Operations >>

        // TODO

        #endregion << Schema Free Document Version-related Operations >>
    }
}
