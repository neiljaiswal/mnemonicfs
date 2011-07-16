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
using MnemonicFS.MfsCore;
using MnemonicFS.MfsUtils;
using MnemonicFS.MfsExceptions;
using MnemonicFS.Tests.Utils;
using MnemonicFS.Tests.Base;
using MnemonicFS.MfsUtils.MfsCrypto;

namespace MnemonicFS.Tests.DocumentUUID {
    [TestFixture]
    public class Tests_MfsOperations_GetUUID : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // Create any document:
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            // And request this document's UUID:
            string urlUUID = _mfsOperations.Document.UUID (urlID);
            Assert.IsNotNull (urlUUID, "Document UUID returned is null.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        public void Test_Unique () {
            // Create any document:
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            // And one more document:
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            // And request both document's UUIDs:
            string urlUUID = _mfsOperations.Document.UUID (urlID);
            string noteUUID = _mfsOperations.Document.UUID (noteID);
            Assert.AreNotEqual (urlUUID, noteUUID, "Different documents have same UUIDs.");

            _mfsOperations.Note.Delete (noteID);
            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        public void Test_SameDoc_SameID () {
            // Create any document:
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string noteUUID1 = _mfsOperations.Document.UUID (noteID);
            string noteUUID2 = _mfsOperations.Document.UUID (noteID);
            Assert.AreEqual (noteUUID1, noteUUID2, "UUID returned for same document differs.");

            _mfsOperations.Note.Delete (noteID);
        }

        [Test]
        public void Test_SameDoc_TwoUsers_DifferentIDs () {
            // Create a single document:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            // Save this using current user:
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Create another user account:
            string user2ID = GetANonExistentUserID ();
            string password = TestUtils.GetAWord (TYPICAL_PASSWORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);
            MfsOperations.User.New (user2ID, passwordHash);
            MfsOperations mfsOps = new MfsOperations (user2ID, passwordHash);

            // Save the same file using new user's account:
            ulong fileID2 = SaveFileToMfs (ref mfsOps, _fileName, _fileNarration, _fileData, when, false);

            // Next, retrieve both files' UUIDs:
            string uuid1 = _mfsOperations.Document.UUID (fileID1);
            string uuid2 = mfsOps.Document.UUID (fileID2);
            Assert.AreNotEqual (uuid1, uuid2, "Same document saved across two accounts returns same UUID.");

            _mfsOperations.Document.Delete (fileID1);
            mfsOps.Document.Delete (fileID2);
            MfsOperations.User.Delete (user2ID, true, true);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            _mfsOperations.Document.UUID (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.Document.UUID (veryLargeFileID);
        }
    }
}
