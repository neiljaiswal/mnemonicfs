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

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.ApplyAspectToDocument (aspectID, fileID);
            Assert.IsTrue (applySuccess, "Aspect failed to be applied to document.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToDocument (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.ApplyAspectToDocument (0, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToDocument (aspectID, veryLargeDocumentID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToDocument (veryLargeAspectID, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
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

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            _mfsOperations.ApplyAspectToDocument (aspectID, fileID);

            bool isApplied = _mfsOperations.IsAspectAppliedToDocument (aspectID, fileID);
            Assert.IsTrue (isApplied, "Indicated that aspect has not been applied to document, even though it is.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_SanityCheck_NotApplied () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            Assert.That (noteID > 0, "Note not added successfully: Invalid note id returned.");

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool isApplied = _mfsOperations.IsAspectAppliedToDocument (aspectID, noteID);
            Assert.IsFalse (isApplied, "Indicated that aspect has been applied to document, even though it isn't.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.IsAspectAppliedToDocument (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.IsAspectAppliedToDocument (0, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToDocument (aspectID, veryLargeDocumentID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
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
                _mfsOperations.IsAspectAppliedToDocument (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
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
            ulong noteID = _mfsOperations.AddNote (note);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            _mfsOperations.ApplyAspectToDocument (aspectID, noteID);

            bool isUnapplied = _mfsOperations.UnapplyAspectFromDocument (aspectID, noteID);
            Assert.IsTrue (isUnapplied, "Aspect was not successfully unapplied from document.");

            bool isApplied = _mfsOperations.IsAspectAppliedToDocument (aspectID, noteID);
            Assert.IsFalse (isApplied, "Attempt to unapply aspect from document was not successful.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.UnapplyAspectFromDocument (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.UnapplyAspectFromDocument (0, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromDocument (aspectID, veryLargeDocumentID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
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
                _mfsOperations.UnapplyAspectFromDocument (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
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
                _mfsOperations.ApplyAspectToDocument (aspectID, fileID);
            }

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnDocument (fileID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects applied to document.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.GetAspectsAppliedOnDocument (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.GetAspectsAppliedOnDocument (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetDocumentsAppliedWithAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                _mfsOperations.ApplyAspectToDocument (aspectID, urlID);

                docIDs.Add (urlID);
            }

            List<ulong> retrievedDocIDs = _mfsOperations.GetDocumentsAppliedWithAspect (aspectID);
            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents aspect has been applied to.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong urlID in docIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.GetDocumentsAppliedWithAspect (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.GetDocumentsAppliedWithAspect (veryLargeAspectID);
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

            _mfsOperations.ApplyAspectsToDocument (aspectIDs, fileID);

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnDocument (fileID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.ApplyAspectsToDocument (null, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.ApplyAspectsToDocument (new List<ulong> (), fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectsToDocument (aspectIDs, veryLargeDocumentID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToDocuments : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                docIDs.Add (fileID);
            }

            _mfsOperations.ApplyAspectToDocuments (aspectID, docIDs);

            List<ulong> retrievedDocIDs = _mfsOperations.GetDocumentsAppliedWithAspect (aspectID);
            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents aspect has been applied to.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong fileID in docIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToDocuments (aspectID, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToDocuments (aspectID, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
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

            _mfsOperations.ApplyAspectsToDocuments (aspectIDs, docIDs);

            foreach (ulong docID in docIDs) {
                List<ulong> retrAspects = _mfsOperations.GetAspectsAppliedOnDocument (docID);
                Assert.AreEqual (aspectIDs.Count, retrAspects.Count, "Aspect count for document does not match.");
            }

            foreach (ulong aspectID in aspectIDs) {
                List<ulong> retrDocs = _mfsOperations.GetDocumentsAppliedWithAspect (aspectID);
                Assert.AreEqual (docIDs.Count, retrDocs.Count, "Document count for aspect does not match.");
            }

            foreach (ulong fileID in docIDs) {
                _mfsOperations.DeleteFile (fileID);
            }

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToDocuments (aspectIDs, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToDocuments (aspectIDs, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
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
                _mfsOperations.ApplyAspectsToDocuments (null, fileIDs);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            List<ulong> docIDs = new List<ulong> ();
            docIDs.Add (noteID);

            try {
                _mfsOperations.ApplyAspectsToDocuments (new List<ulong> (), docIDs);
            } finally {
                _mfsOperations.DeleteNote (noteID);
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

            _mfsOperations.ApplyAspectsToDocument (aspectIDs, fileID);

            int numAspectsUnapplied = _mfsOperations.UnapplyAllAspectsFromDocument (fileID);
            Assert.AreEqual (numAspectsToCreate, numAspectsUnapplied, "Attempt to unapply aspects from document was unsuccessful.");

            List<ulong> allAspects = _mfsOperations.GetAspectsAppliedOnDocument (fileID);
            Assert.AreEqual (0, allAspects.Count, "Incorrect number of aspects unapplied from document.");

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.UnapplyAllAspectsFromDocument (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.UnapplyAllAspectsFromDocument (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromAllDocuments : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                docIDs.Add (urlID);
            }

            _mfsOperations.ApplyAspectToDocuments (aspectID, docIDs);

            int numFilesUnappliedFrom = _mfsOperations.UnapplyAspectFromAllDocuments (aspectID);
            Assert.AreEqual (numDocsToCreate, numFilesUnappliedFrom, "Attempt to unapply aspect from documents was unsuccessful.");

            List<ulong> allFiles = _mfsOperations.GetDocumentsAppliedWithAspect (aspectID);
            Assert.AreEqual (0, allFiles.Count, "Number of documents that aspect was unapplied from was incorrect.");

            foreach (ulong urlID in docIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.UnapplyAspectFromAllDocuments (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.UnapplyAspectFromAllDocuments (veryLargeAspectID);
        }
    }
}
