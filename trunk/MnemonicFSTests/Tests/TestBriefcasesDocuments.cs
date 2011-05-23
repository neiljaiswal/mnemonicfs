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
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsCore;

namespace MnemonicFS.Tests.BriefcasesDocuments {
    [TestFixture]
    public class Tests_BriefcasesMethod_GlobalBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_NewDocumentShouldBeInGlobalBriefcaseByDefault_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.Sfd.New (_schemaFreeDocName, when);

            ulong briefcaseID = _mfsOperations.Briefcase.GetContaining (docID);
            Assert.AreEqual (MfsOperations.GlobalBriefcase, briefcaseID, "Freshly added document is not in global briefcase by default.");

            _mfsOperations.Sfd.Delete (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_GlobalBriefcaseShouldNotBeDeletable_SanityCheck () {
            _mfsOperations.Briefcase.Delete (MfsOperations.GlobalBriefcase);
        }

        [Test]
        public void Test_DocumentShouldBeMovedBackToGlobalBriefcase_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            _mfsOperations.Briefcase.MoveTo (urlID, briefcaseID);

            bool removedFromBriefcase = _mfsOperations.Briefcase.MoveToGlobal (urlID);

            ulong retrievedBriefcaseID = _mfsOperations.Briefcase.GetContaining (urlID);
            Assert.AreEqual (MfsOperations.GlobalBriefcase, retrievedBriefcaseID, "Document moved out of briefcase is not put back in global briefcase.");

            _mfsOperations.Url.Delete (urlID);
            _mfsOperations.Briefcase.Delete (briefcaseID);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_GetContainingBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            bool fileMoved = _mfsOperations.Briefcase.MoveTo (noteID, briefcaseID);
            Assert.IsTrue (fileMoved, "Document was not moved successfully to briefcase.");

            ulong retrievedBriefcaseID = _mfsOperations.Briefcase.GetContaining (noteID);
            Assert.AreEqual (briefcaseID, retrievedBriefcaseID, "Document's retrieved briefcase is not the same as the one moved to.");

            _mfsOperations.Note.Delete (noteID);
            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.Briefcase.GetContaining (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.Briefcase.GetContaining (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_MoveDocumentToBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong briefcaseID1 = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            _mfsOperations.Briefcase.MoveTo (noteID, briefcaseID1);

            ulong retrievedBriefcaseID1 = _mfsOperations.Briefcase.GetContaining (noteID);
            Assert.AreEqual (briefcaseID1, retrievedBriefcaseID1, "Document's retrieved briefcase is not the same as the one moved to.");

            string briefcaseName2 = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            ulong briefcaseID2 = _mfsOperations.Briefcase.New (briefcaseName2, _briefcaseDesc);

            _mfsOperations.Briefcase.MoveTo (noteID, briefcaseID2);

            ulong retrievedBriefcaseID2 = _mfsOperations.Briefcase.GetContaining (noteID);
            Assert.AreEqual (briefcaseID2, retrievedBriefcaseID2, "Document's retrieved briefcase is not the same as the one moved to.");

            _mfsOperations.Note.Delete (noteID);
            _mfsOperations.Briefcase.Delete (briefcaseID1);
            _mfsOperations.Briefcase.Delete (briefcaseID2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            try {
                _mfsOperations.Briefcase.MoveTo (0, briefcaseID);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Briefcase.MoveTo (veryLargeDocumentID, briefcaseID);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.Briefcase.MoveTo (fileID, 0);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            try {
                _mfsOperations.Briefcase.MoveTo (urlID, veryLargeBriefcaseID);
            } finally {
                _mfsOperations.Url.Delete (urlID);
            }
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_GetDocumentsInBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();
            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.Url.New (url, description, when);

                _mfsOperations.Briefcase.MoveTo (urlID, briefcaseID);
                docIDs.Add (urlID);
            }

            List<ulong> docIDsInBriefcase = _mfsOperations.Briefcase.All (briefcaseID);
            Assert.AreEqual (numDocsToCreate, docIDsInBriefcase.Count, "Number of documents returned do not match.");

            docIDs.Sort ();
            docIDsInBriefcase.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], docIDsInBriefcase[i], "Briefcase ids returned do not match.");
            }

            foreach (ulong urlID in docIDs) {
                _mfsOperations.Url.Delete (urlID);
            }

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            _mfsOperations.Briefcase.All (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            _mfsOperations.Briefcase.All (veryLargeBriefcaseID);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_RemoveDocumentFromBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            _mfsOperations.Briefcase.MoveTo (noteID, briefcaseID);

            bool removedFromBriefcase = _mfsOperations.Briefcase.MoveToGlobal (noteID);

            ulong retrievedBriefcaseID = _mfsOperations.Briefcase.GetContaining (noteID);
            Assert.AreEqual (MfsOperations.GlobalBriefcase, retrievedBriefcaseID, "Removing document from briefcase does not delete its reference.");

            _mfsOperations.Note.Delete (noteID);
            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.Briefcase.MoveToGlobal (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.Briefcase.MoveToGlobal (veryLargeDocumentID);
        }
    }
}
