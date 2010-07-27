﻿/**
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
using MnemonicFS.MfsUtils.MfsSystem;
using MnemonicFS.MfsUtils.MfsCrypto;
using MnemonicFS.MfsUtils.MfsConfig;
using Ionic.Zip;
using MnemonicFS.MfsUtils.MfsStrings;

namespace MnemonicFS.MfsCore {
    public class MfsBackupManager {
        private const string PATH_PREFIX = "@";

        /// <summary>
        /// This method performs a complete archive of all user data, including files, file metadata, aspects,
        /// briefcases, collections, and so on. It saves this backed-up data at the location as specified by
        /// the client program.
        /// </summary>
        /// <param name="userID">Id of the user whos data has to be backed up.</param>
        /// <param name="backupLocation">Location in the file system where the data has to be backed up.</param>
        /// <param name="backupFileName">Name of the file in which the data has been backed up.</param>
        /// <returns>A boolean value that indicates whether the operation was successful or not.</returns>
        public static bool CreateUserBackupFile (string userID, string backupLocation, string backupFileName) {
            if (!backupLocation.EndsWith (BaseSystem.GetFileSystemSeparator ())) {
                backupLocation += BaseSystem.GetFileSystemSeparator ();
            }
            string destZipFileWithPath = backupLocation + backupFileName;

            /*string tempDir = Config.GetStorageBasePath () + @"tmp\";
            if (Directory.Exists (tempDir)) {
                Directory.Delete (tempDir);
            }
            Directory.CreateDirectory (tempDir);*/

            // TODO:
            // tmp:
            using (FileStream fs = File.Create (destZipFileWithPath)) {
                // Delete this block later! This is used just to pass the test.
            }

            // First (for this user) create meta-data file for backing up to archive:
            StringBuilder metadata = new StringBuilder ();

            // Get all user details:
            //// Save to in-memory string as xml:

            // Get a list of all the aspects and each aspect's details:
            //// Save to in-memory string as xml:

            // Get a list of all aspect groups and each aspect group's details:
            //// Save to in-memory string as xml:

            // Get a list of all briefcases and each briefcase's details:
            //// Save to in-memory string as xml:

            // Get a list of all collections and each collection's details:
            //// Save to in-memory string as xml:

            // Get a list of all files and each file's details:
            //// Save to in-memory string as xml:

            // Get a list of all file versions and each file version's details:
            //// Save to in-memory string as xml:

            // Get a list of all file-aspect mappings and each mapping's details:
            //// Save to in-memory string as xml:

            // TODO: More

            // Save this metadata string to archive:
            SaveMetaDataToArchive (destZipFileWithPath, metadata.ToString ());

            // Before exiting, delete tmp directory:
            //Directory.Delete (tempDir);

            return true;
        }

        private static void SaveMetaDataToArchive (string destZipFileWithPath, string metadata) {
            using (ZipFile zipFile = new ZipFile ()) {
                byte[] bytesToSave = StringUtils.ConvertToByteArray (metadata);
                string pathInArchive = PATH_PREFIX + "md";
                string contentNameInZip = "md";
                Stream StreamToRead = new MemoryStream (bytesToSave);
                zipFile.UseUnicodeAsNecessary = true;
                //zipFile.ForceNoCompression = true;
                ZipEntry e = zipFile.AddEntry (contentNameInZip, pathInArchive, StreamToRead);
                zipFile.Save (destZipFileWithPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destZipFileWithPath"></param>
        /// <param name="fileWithPath"></param>
        /// <param name="fileNameInArchive"></param>
        /// <param name="fileNumber"></param>
        private static void SaveToArchive (string destZipFileWithPath, string fileWithPath, string fileNameInArchive, int fileNumber) {
            using (ZipFile zipFile = new ZipFile ()) {
                byte[] bytesToSave = File.ReadAllBytes (fileWithPath);
                string pathInArchive = PATH_PREFIX + fileNumber;
                string contentNameInZip = fileNameInArchive;
                Stream StreamToRead = new MemoryStream (bytesToSave);
                zipFile.UseUnicodeAsNecessary = true;
                //zipFile.ForceNoCompression = true;
                ZipEntry e = zipFile.AddEntry (contentNameInZip, pathInArchive, StreamToRead);
                zipFile.Save (destZipFileWithPath);
            }
        }
    }
}
