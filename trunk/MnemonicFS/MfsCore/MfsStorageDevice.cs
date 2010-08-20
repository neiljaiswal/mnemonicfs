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
using Ionic.Zip;
using MnemonicFS.MfsUtils;
using MnemonicFS.MfsUtils.MfsSystem;
using MnemonicFS.MfsUtils.MfsCrypto;
using MnemonicFS.MfsUtils.MfsStrings;
using MnemonicFS.MfsUtils.MfsConfig;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.MfsCore {
    /// <summary>
    /// The MfsStorageDevice class embodies a storage device. It is non-instantiable.
    /// </summary>
    internal class MfsStorageDevice {

        #region << Declarations & Initialization >>

        private static string _basePath;

        /// <summary>
        /// Constructor for initializing the storage device.
        /// </summary>
        static MfsStorageDevice () {
            InitDevice ();
        }

        /// <summary>
        /// All device initialization code goes here.
        /// </summary>
        internal static void InitDevice () {
            _basePath = Config.GetStorageBasePath ();

            // Also create the base path:
            Trace.TraceInformation ("Creating base path: " + Config.GetStorageBasePath ());
            Directory.CreateDirectory (Config.GetStorageBasePath ());
        }

        #endregion << Declarations & Initialization >>

        #region << User-creation Operations >>

        internal static string CreateUserPath (string userIDStr) {
            string hash = Hasher.GetSHA1 (userIDStr);
            string sep = BaseSystem.GetFileSystemSeparator ();

            // We only take the first 8 characters of the hash:
            hash = hash.Substring (0, 8);
            hash = StringUtils.GetPureAlphaStr (hash);

            // And break up these characters into a further subset of 2 chars each.
            // This will result in a maximum proliferation of 676 directories at any one level.
            // True, the chances of collisions will increase, but we can live with that.
            List<string> brokenHash = StringUtils.BreakStringIntoTokens (hash, 2);

            string hashPath = "";
            foreach (string s in brokenHash) {
                hashPath += s + sep;
            }

            string userSpecificPath = hashPath;

            // We also need to make sure that there isn't another user at the same path already.
            // We therefore create another directory within this directory for this specific user.
            string userSpecificDir = "";
            do {
                userSpecificDir = RandomStrs.GetRandomString (5);
            } while (Directory.Exists (userSpecificPath + userSpecificDir));
            userSpecificPath = userSpecificPath + userSpecificDir + sep;

            Debug.Print ("Got new user's specific path: " + userSpecificPath);

            string pathToCreate = Config.GetStorageBasePath () + userSpecificPath;
            Directory.CreateDirectory (pathToCreate);

            return userSpecificPath;
        }

        #endregion << User-creation Operations >>

        #region << File Save / Retrieval Operations >>

        private static int lastInt = 0;
        /// <summary>
        /// This method saves the file data passed to it within persistent storage.
        /// </summary>
        /// <param name="fileName">Name of the file to be saved.</param>
        /// <param name="fileData">Actual file data to be saved.</param>
        /// <returns>Exact path on the storage device where the file has been saved.</returns>
        internal static void SaveFile (
            string userSpecificPath, string fileName, byte[] fileData,
            out string assumedFileName, out string filePassword, out string archiveName, out string absPathToContainingDir
            ) {
            string sep = BaseSystem.GetFileSystemSeparator ();
            DateTime now = DateTime.Now;

            // We generate a fresh path with the format:
            // <base_storage_dir>/day/month/year/hour/<some random int>/v0/
            absPathToContainingDir = userSpecificPath +
                                     NumericValuesCustomHashDictionary.GetCustomHashValue (now.Day) + sep +
                                     NumericValuesCustomHashDictionary.GetCustomHashValue (now.Month) + sep +
                                     NumericValuesCustomHashDictionary.GetCustomHashValue (now.Year) + sep +
                                     NumericValuesCustomHashDictionary.GetCustomHashValue (now.Hour);

            // We also need to make sure while generating the random int that the directory does not already exist.
            bool dirExists = true;
            int seedCount = 0;

            assumedFileName = RandomStrs.GetRandomString (5);
            filePassword = Hasher.GetSHA1 (RandomStrs.GetRandomString (5)).Substring (0, 20);
            archiveName = RandomStrs.GetRandomFileName ();

            do {
                Random random = new Random (
                    now.Year + now.Month + now.Day + now.Hour +
                    now.Minute + now.Second + now.Millisecond +
                    ++seedCount
                    );
                
                int randIntVal = random.Next ();
                
                if (randIntVal == lastInt) {
                    Debug.Print ("Collision of random ints: " + randIntVal);
                    continue;
                } else {
                    lastInt = randIntVal;
                }

                string prospectiveDir = absPathToContainingDir + sep + StringUtils.GetPureAlphaStr (lastInt.ToString ()) + sep + "v0" + sep;
                if (Directory.Exists (prospectiveDir)) {
                    continue;
                }

                dirExists = false;

                absPathToContainingDir += sep + StringUtils.GetPureAlphaStr (lastInt.ToString ()) + sep + "v0" + sep;

                Debug.Print ("Creating fresh directory: " + absPathToContainingDir);
                Directory.CreateDirectory (absPathToContainingDir);
            } while (dirExists == true);

            string fileWithPath = absPathToContainingDir + archiveName;

            SaveByteArrayToZippedFile (fileWithPath, fileData, assumedFileName, filePassword);
        }

        internal static void SaveByteArrayToZippedFile (string destZipFileWithPath, byte[] bytesToSave, string contentNameInZip, string password) {
            Stream StreamToRead = new MemoryStream (bytesToSave);
            using (ZipFile zip = new ZipFile ()) {
                zip.UseUnicodeAsNecessary = true;
                zip.ForceNoCompression = true;
                ZipEntry e = zip.AddEntry (contentNameInZip, "/", StreamToRead);
                e.Password = password;
                zip.Save (destZipFileWithPath);
            }
        }

        internal static void SaveByteArraysToZippedFile (string destZipFileWithPath, List<byte[]> byteArraysToSave, List<string> contentNamesInZip, string password) {
            int count = byteArraysToSave.Count;

            using (ZipFile zip = new ZipFile ()) {
                for (int i = 0; i < count; ++i) {
                    byte[] bytesToSave = byteArraysToSave[i];
                    string contentNameInZip = contentNamesInZip[i];
                    Stream StreamToRead = new MemoryStream (bytesToSave);
                    zip.UseUnicodeAsNecessary = true;
                    //zip.ForceNoCompression = true;
                    ZipEntry e = zip.AddEntry (contentNameInZip, "/", StreamToRead);
                    if (password != null) {
                        e.Password = password;
                    }
                    zip.Save (destZipFileWithPath);
                }
            }
        }

        internal static byte[] RetrieveByteArrayFromZippedFile (string srcZipFileWithPath, string contentNameInZip, string password) {
            Stream stream = null;
            using (ZipFile zip = ZipFile.Read (srcZipFileWithPath)) {
                ZipEntry e = zip[contentNameInZip];
                if (e == null) {
                    throw new BadContentException ("Illegal content!");
                }
                stream = new MemoryStream ((int) e.UncompressedSize);
                e.ExtractWithPassword (stream, password);
            }

            byte[] bytes = new byte[stream.Length];
            stream.Seek (0, SeekOrigin.Begin);
            for (int i = 0; i < stream.Length; ++i) {
                int val = stream.ReadByte ();
                byte b = (byte) val;
                bytes[i] = b;
            }

            return bytes;
        }

        /// <summary>
        /// This method tells the caller if the file exists in storage.
        /// </summary>
        /// <param name="fileWithPath">Filename to check for along with its fully qualified path.</param>
        /// <returns>A boolean value indicating if the file exists in storage or not.</returns>
        internal static bool DoesFileExist (string fileWithPath) {
            return File.Exists (fileWithPath);
        }

        /// <summary>
        /// This method returns the file sought by the client as a byte array.
        /// </summary>
        /// <param name="filePath">The exact FQ path of the file.</param>
        /// <returns>File content as a byte array.</returns>
        internal static byte[] RetrieveFile (string filePath) {
            return File.ReadAllBytes (filePath);
        }

        /// <summary>
        /// This method deletes the file sought to be deleted by the client. Please note that this
        /// method will recursively keep deleting non-empty parent directories, till it arrives at
        /// the root storage directory, and if this directory's empty, will delete that too.
        /// </summary>
        /// <param name="containingDir">The containing directory of the file.</param>
        /// <param name="fileName">The file name of the file to be deleted.</param>
        internal static void DeleteFile (string containingDir, string fileName) {
            Debug.Print ("Got file name: " + fileName + ", at path: " + containingDir);
            string fileWithPath = containingDir + fileName;

            Debug.Print ("Deleting: " + fileWithPath);
            File.Delete (fileWithPath);
            //Debug.Print ("About to start deleting: " + containingDir);

            DeleteEmptyDir (containingDir);
        }

        internal static void DeleteDirectoryIfEmpty (string directory) {
            Debug.Print ("Deleting directory (IF empty): " + directory);
            DeleteEmptyDir (directory);
        }

        internal static void DeleteUserDB (string dbPath) {
            string absDbPath = Config.GetStorageBasePath () + dbPath + BaseSystem.GetUserDBFileName ();
            if (File.Exists (absDbPath)) {
                Debug.Print ("Deleting user db: " + absDbPath);
                File.Delete (absDbPath);
            } else {
                Debug.Print ("User db does NOT exist: " + absDbPath);
            }
        }

        internal static void DeleteUserFileSystemObjects (string path) {
            string absPath = Config.GetStorageBasePath () + path;
            string[] allFiles = Directory.GetFiles (absPath);
            foreach (string file in allFiles) {
                File.Delete (file);
            }
            string[] allDirs = Directory.GetDirectories (absPath);
            foreach (string dir in allDirs) {
                Directory.Delete (dir, true);
            }
        }

        internal static void SaveByteArray (byte[] byteStream, string passphrase, out string assumedFileName, out string archiveName, out string destDir) {
            assumedFileName = RandomStrs.GetRandomString (5);
            archiveName = RandomStrs.GetRandomFileName ();

            DateTime now = DateTime.Now;
            string sep = BaseSystem.GetFileSystemSeparator ();
            destDir = Config.GetStorageBasePath () + sep +
                        NumericValuesCustomHashDictionary.GetCustomHashValue (now.Day).Substring (0, 2) + sep +
                        NumericValuesCustomHashDictionary.GetCustomHashValue (now.Month).Substring (0, 2) + sep +
                        NumericValuesCustomHashDictionary.GetCustomHashValue (now.Year).Substring (0, 2) + sep +
                        NumericValuesCustomHashDictionary.GetCustomHashValue (now.Hour).Substring (0, 2);

            // We also need to make sure while generating the random int that the directory does not already exist.
            bool dirExists = true;
            int seedCount = 0;
            do {
                Random random = new Random (
                    now.Year + now.Month + now.Day + now.Hour +
                    now.Minute + now.Second + now.Millisecond +
                    ++seedCount
                    );

                int randIntVal = random.Next ();

                if (randIntVal == lastInt) {
                    Debug.Print ("Collision of random ints: " + randIntVal);
                    continue;
                } else {
                    lastInt = randIntVal;
                }

                string prospectiveDir = destDir + sep + StringUtils.GetPureAlphaStr (lastInt.ToString ()) + sep;
                if (Directory.Exists (prospectiveDir)) {
                    continue;
                }

                dirExists = false;

                destDir += sep + StringUtils.GetPureAlphaStr (lastInt.ToString ()) + sep;

                Debug.Print ("Creating fresh directory: " + destDir);
                Directory.CreateDirectory (destDir);
            } while (dirExists == true);

            string fileWithPath = destDir + archiveName;

            SaveByteArrayToZippedFile (fileWithPath, byteStream, assumedFileName, passphrase);
        }

        #endregion << File Save / Retrieval Operations >>

        #region << File Deletion Operations >>

        /// <summary>
        /// This method recursively keeps deleting directories till it arrives at the root directory.
        /// </summary>
        /// <param name="path">The directory to be deleted.</param>
        private static void DeleteEmptyDir (string path) {
            if (GetNumChildren (path) < 1) {
                string parent = GetParentAsString (path);
                
                Directory.Delete (path);

                if (parent == null) {
                    return;
                }

                DeleteEmptyDir (parent);
            } else {
                return;
            }
        }

        /// <summary>
        /// This method returns the number of children (including files and
        /// immediate sub-directories) that a directory has.
        /// </summary>
        /// <param name="path">Directory path for which this information is sought.</param>
        /// <returns>An integer indicating the total number of children that this directory has.</returns>
        private static int GetNumChildren (string path) {
            int numChildren = GetNumChildrenFiles (path) + GetNumChildrenDirs (path);
            
            return numChildren;
        }

        /// <summary>
        /// This method returns the number of children files that a directory has.
        /// </summary>
        /// <param name="path">Directory path for which this information is sought.</param>
        /// <returns>An integer indicating the total number of children files that this directory has.</returns>
        private static int GetNumChildrenFiles (string path) {
            string[] files = Directory.GetFiles (path);

            return files.Length;
        }

        /// <summary>
        /// This method returns the number of sub-directories that a directory has.
        /// </summary>
        /// <param name="path">Directory path for which this information is sought.</param>
        /// <returns>An integer indicating the total number of sub-directories that this directory has.</returns>
        private static int GetNumChildrenDirs (string path) {
            string[] dirs = Directory.GetDirectories (path);

            return dirs.Length;
        }

        /// <summary>
        /// This method returns the parent directory of a sub-directory.
        /// </summary>
        /// <param name="path">Directory path for which this information is sought.</param>
        /// <returns>The path of the parent directory.</returns>
        private static string GetParentAsString (string path) {
            path = RemoveTrailingSlash (path);
            DirectoryInfo parentDir = Directory.GetParent (path);

            if (parentDir == null) {
                return null;
            }

            return parentDir.FullName;
        }

        /// <summary>
        /// This helper method removes the trailing slashes from a directory path.
        /// </summary>
        /// <param name="path">Directory path for which this is sought.</param>
        /// <returns>The same directory path without any trailing slashes.</returns>
        private static string RemoveTrailingSlash (string path) {
            if (path.EndsWith ("/") || path.EndsWith ("\\")) {
                int length = path.Length;
                path = path.Substring (0, length - 1);
            }

            return path;
        }

        #endregion << File Deletion Operations >>

        #region << File Version Operations >>

        internal static void SaveFileAsNewVersion (
            byte[] fileData, string freshPath,
            string fileAssumedName, string filePassword, string archiveName
            ) {
            Directory.CreateDirectory (freshPath);

            string fileWithPath = freshPath + archiveName;

            SaveByteArrayToZippedFile (fileWithPath, fileData, fileAssumedName, filePassword);
            
            Debug.Print ("Writing (new version) file to path: " + fileWithPath);
            File.WriteAllBytes (fileWithPath, fileData);
        }

        #endregion << File Version Operations >>

        #region << Archiving Operations >>

        internal static void ArchiveFiles (List<byte[]> filesData, List<string> fileNames, string opDirPath, string opArchiveName, string password) {
            if (!Directory.Exists (opDirPath)) {
                Directory.CreateDirectory (opDirPath);
            }

            string fileNameWithPath = opDirPath + BaseSystem.GetFileSystemSeparator () + opArchiveName;

            SaveByteArraysToZippedFile (fileNameWithPath, filesData, fileNames, password);
        }

        #endregion << Archiving Operations >>
    }
}
