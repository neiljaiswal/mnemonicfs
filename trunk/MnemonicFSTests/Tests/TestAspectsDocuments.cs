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

namespace MnemonicFS.Tests.AspectsDocuments {
    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.Aspect.Apply (aspectID, fileID);
            Assert.IsTrue (applySuccess, "Aspect failed to be applied to document.");

            _mfsOperations.Aspect.Delete (aspectID);
            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            try {
                _mfsOperations.Aspect.Apply (aspectID, 0);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Aspect.Apply (0, urlID);
            } finally {
                _mfsOperations.Url.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.Apply (aspectID, veryLargeDocumentID);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.Apply (veryLargeAspectID, noteID);
            } finally {
                _mfsOperations.Note.Delete (noteID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_IsAspectAppliedToDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Applied () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            _mfsOperations.Aspect.Apply (aspectID, fileID);

            bool isApplied = _mfsOperations.Aspect.IsApplied (aspectID, fileID);
            Assert.IsTrue (isApplied, "Indicated that aspect has not been applied to document, even though it is.");

            _mfsOperations.Aspect.Delete (aspectID);
            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_SanityCheck_NotApplied () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            Assert.That (noteID > 0, "Note not added successfully: Invalid note id returned.");

            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            bool isApplied = _mfsOperations.Aspect.IsApplied (aspectID, noteID);
            Assert.IsFalse (isApplied, "Indicated that aspect has been applied to document, even though it isn't.");

            _mfsOperations.Aspect.Delete (aspectID);
            _mfsOperations.Note.Delete (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            try {
                _mfsOperations.Aspect.IsApplied (aspectID, 0);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Aspect.IsApplied (0, urlID);
            } finally {
                _mfsOperations.Url.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.IsApplied (aspectID, veryLargeDocumentID);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.IsApplied (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            _mfsOperations.Aspect.Apply (aspectID, noteID);

            bool isUnapplied = _mfsOperations.Aspect.Unapply (aspectID, noteID);
            Assert.IsTrue (isUnapplied, "Aspect was not successfully unapplied from document.");

            bool isApplied = _mfsOperations.Aspect.IsApplied (aspectID, noteID);
            Assert.IsFalse (isApplied, "Attempt to unapply aspect from document was not successful.");

            _mfsOperations.Aspect.Delete (aspectID);
            _mfsOperations.Note.Delete (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            try {
                _mfsOperations.Aspect.Unapply (aspectID, 0);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Aspect.Unapply (0, noteID);
            } finally {
                _mfsOperations.Note.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.Unapply (aspectID, veryLargeDocumentID);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.Unapply (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectsAppliedOnDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.Aspect.Apply (aspectID, fileID);
            }

            List<ulong> retrievedAspectIDs = _mfsOperations.Aspect.Applied (fileID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects applied to document.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.File.Delete (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.Aspect.Applied (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.Aspect.Applied (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetDocumentsAppliedWithAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.Url.New (url, description, when);

                _mfsOperations.Aspect.Apply (aspectID, urlID);

                docIDs.Add (urlID);
            }

            List<ulong> retrievedDocIDs = _mfsOperations.Aspect.Documents (aspectID);
            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents aspect has been applied to.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.Aspect.Delete (aspectID);

            foreach (ulong urlID in docIDs) {
                _mfsOperations.Url.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.Aspect.Documents (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.Aspect.Documents (veryLargeAspectID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.Aspect.Apply (aspectIDs, fileID);

            List<ulong> retrievedAspectIDs = _mfsOperations.Aspect.Applied (fileID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.File.Delete (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Aspect.Apply (null, noteID);
            } finally {
                _mfsOperations.Note.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.Aspect.Apply (new List<ulong> (), fileID);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Aspect.Apply (aspectIDs, veryLargeDocumentID);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToDocuments : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                docIDs.Add (fileID);
            }

            _mfsOperations.Aspect.Apply (aspectID, docIDs);

            List<ulong> retrievedDocIDs = _mfsOperations.Aspect.Documents (aspectID);
            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents aspect has been applied to.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.Aspect.Delete (aspectID);

            foreach (ulong fileID in docIDs) {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            try {
                _mfsOperations.Aspect.Apply (aspectID, null);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            try {
                _mfsOperations.Aspect.Apply (aspectID, new List<ulong> ());
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToDocuments : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numFilesToCreate);

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                docIDs.Add (fileID);
            }

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.Aspect.Apply (aspectIDs, docIDs);

            foreach (ulong docID in docIDs) {
                List<ulong> retrAspects = _mfsOperations.Aspect.Applied (docID);
                Assert.AreEqual (aspectIDs.Count, retrAspects.Count, "Aspect count for document does not match.");
            }

            foreach (ulong aspectID in aspectIDs) {
                List<ulong> retrDocs = _mfsOperations.Aspect.Documents (aspectID);
                Assert.AreEqual (docIDs.Count, retrDocs.Count, "Document count for aspect does not match.");
            }

            foreach (ulong fileID in docIDs) {
                _mfsOperations.File.Delete (fileID);
            }

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.Aspect.Apply (aspectIDs, null);
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.Aspect.Apply (aspectIDs, new List<ulong> ());
            } finally {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID);

            try {
                _mfsOperations.Aspect.Apply (null, fileIDs);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            List<ulong> docIDs = new List<ulong> ();
            docIDs.Add (noteID);

            try {
                _mfsOperations.Aspect.Apply (new List<ulong> (), docIDs);
            } finally {
                _mfsOperations.Note.Delete (noteID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAllAspectsFromDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.Aspect.Apply (aspectIDs, fileID);

            int numAspectsUnapplied = _mfsOperations.Aspect.UnapplyAll (fileID);
            Assert.AreEqual (numAspectsToCreate, numAspectsUnapplied, "Attempt to unapply aspects from document was unsuccessful.");

            List<ulong> allAspects = _mfsOperations.Aspect.Applied (fileID);
            Assert.AreEqual (0, allAspects.Count, "Incorrect number of aspects unapplied from document.");

            _mfsOperations.File.Delete (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.Aspect.Delete (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.Aspect.UnapplyAll (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.Aspect.UnapplyAll (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromAllDocuments : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.Aspect.New (_aspectName, _aspectDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.Url.New (url, description, when);

                docIDs.Add (urlID);
            }

            _mfsOperations.Aspect.Apply (aspectID, docIDs);

            int numFilesUnappliedFrom = _mfsOperations.Aspect.UnapplyFromAll (aspectID);
            Assert.AreEqual (numDocsToCreate, numFilesUnappliedFrom, "Attempt to unapply aspect from documents was unsuccessful.");

            List<ulong> allFiles = _mfsOperations.Aspect.Documents (aspectID);
            Assert.AreEqual (0, allFiles.Count, "Number of documents that aspect was unapplied from was incorrect.");

            foreach (ulong urlID in docIDs) {
                _mfsOperations.Url.Delete (urlID);
            }

            _mfsOperations.Aspect.Delete (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.Aspect.UnapplyFromAll (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.Aspect.UnapplyFromAll (veryLargeAspectID);
        }
    }
}
