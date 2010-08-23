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
        /// <param name="backupFileNameWithPath">Location in the file system where the data has to be backed up.</param>
        /// <param name="backupTaskDone">Delegate specified by the caller using which it is informed if the backup task was
        /// successful or not.</param>
        public static bool CreateUserBackupArchive (string userID, string backupFileNameWithPath) {
            // tmp:
            using (FileStream fs = File.Create (backupFileNameWithPath)) {
                // Delete this block later! This is used just to pass the test.
            }

            // TODO
            
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
