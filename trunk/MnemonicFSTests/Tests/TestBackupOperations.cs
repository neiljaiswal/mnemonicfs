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
using NUnit.Framework;
using MnemonicFS.Tests.Base;
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsUtils.MfsCrypto;
using System.IO;
using System.Threading;

namespace MnemonicFS.Tests.Backup {
    [TestFixture]
    public class Tests_BackupMethod_CreateUserBackupArchive : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // Create a few aspects:
            List<ulong> aspectsList = new List<ulong> (TYPICAL_MULTI_VALUE);
            string aspectDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string aspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

                if (_mfsOperations.Aspect.Exists (aspectName)) {
                    --i;
                    continue;
                }

                ulong aspectID = _mfsOperations.Aspect.New (aspectName, aspectDesc);
                aspectsList.Add (aspectID);
            }

            // Add some files:
            List<ulong> filesList = new List<ulong> (TYPICAL_MULTI_VALUE);
            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
                filesList.Add (fileID);
            }

            // Also map some of the files to some of the aspects:
            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                _mfsOperations.Aspect.Apply (aspectsList[i], filesList[i]);
            }

            // Prepare to do a backup:

            // First specify the location where the backup file should be saved:
            string backupFileNameWithPath = null;
            // We don't want to end up over-writing an existing file:
            do {
                backupFileNameWithPath = FILE_SYSTEM_LOCATION + TestUtils.GetAnyFileName ();
            } while (File.Exists (backupFileNameWithPath));

            MethodCreateUserBackupArchive method = MfsBackupManager.CreateUserBackupArchive;
            IAsyncResult res = method.BeginInvoke (_userID, backupFileNameWithPath, null, null);
            // [delegate].EndInvoke (IAsyncResult) is blocking:
            bool taskSuccess = method.EndInvoke (res);

            // Finally check the results:
            Assert.IsTrue (taskSuccess, "Task not completed successfully.");
            Assert.That (File.Exists (backupFileNameWithPath), "Backup file does not exist.");

            // Finally, do the clean up, post-test:

            // Delete user backup file:
            File.Delete (backupFileNameWithPath);

            foreach (ulong aspectID in aspectsList) {
                _mfsOperations.Aspect.Delete (aspectID);
            }

            foreach (ulong fileID in filesList) {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_BackupMethod_UpdateUserArchive : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
        }
    }

    [TestFixture]
    public class Tests_BackupMethod_RestoreFromUserArchive : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
        }
    }
}
