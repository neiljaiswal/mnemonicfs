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

namespace MnemonicFS.Tests.Replication {
    [TestFixture]
    public class Tests_ReplicationMethod_StorageDiff : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
        }
    }

    [TestFixture]
    public class Tests_ReplicationMethod_ReplicateStorage : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // First create a few users:
            int numUsersToCreate = TYPICAL_MULTI_VALUE;
            List<string> userIDs = new List<string> (numUsersToCreate);

            // We can have a common password for all users.
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            for (int i = 0; i < numUsersToCreate; ++i) {
                string userID = TestUtils.GetAnyEmailID ();
                if (MfsOperations.DoesUserExist (userID)) {
                    --i;
                    continue;
                }

                MfsOperations.CreateNewUser (userID, passwordHash);
                userIDs.Add (userID);
            }

            // Add a few files each:
            Dictionary<string, List<ulong>> userFileIDs = new Dictionary<string, List<ulong>> (numUsersToCreate);
            int numFilesPerUser = TYPICAL_MULTI_VALUE;

            foreach (string userID in userIDs) {
                MfsOperations mfsOps = new MfsOperations (userID, passwordHash);
                List<ulong> fileIDs = new List<ulong> (numFilesPerUser);

                for (int i = 0; i < numFilesPerUser; ++i) {
                    _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                    DateTime when = DateTime.Now;
                    ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
                    fileIDs.Add (fileID);
                }

                userFileIDs.Add (userID, fileIDs);
            }

            // Next, replicate the entire storage to the location specified:

            // And check if the files are the same:

            // Finally, delete all users as well as their resources:

            // And also delete storage at location:
        }
    }
}
