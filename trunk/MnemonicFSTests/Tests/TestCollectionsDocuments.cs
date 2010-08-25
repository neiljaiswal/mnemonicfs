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
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Base;

namespace MnemonicFS.Tests.CollectionsDocuments {
    [TestFixture]
    public class Tests_CollectionsMethod_AddDocumentToCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            bool addSuccess = _mfsOperations.AddDocumentToCollection (urlID, collectionID);
            Assert.IsTrue (addSuccess, "Document was not successfully added to collection.");

            _mfsOperations.DeleteCollection (collectionID);
            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.AddDocumentToCollection (0, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.AddDocumentToCollection (fileID, 0);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.AddDocumentToCollection (veryLargeDocumentID, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeCollectionID = UInt64.MaxValue;

            try {
                _mfsOperations.AddDocumentToCollection (fileID, veryLargeCollectionID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_IsDocumentInCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_InCollection () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            _mfsOperations.AddDocumentToCollection (noteID, collectionID);

            bool isAdded = _mfsOperations.IsDocumentInCollection (noteID, collectionID);
            Assert.IsTrue (isAdded, "Indicated that document is not in collection, even though it is.");

            _mfsOperations.DeleteCollection (collectionID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        public void Test_SanityCheck_NotInCollection () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            bool isAdded = _mfsOperations.IsDocumentInCollection (fileID, collectionID);
            Assert.IsFalse (isAdded, "Indicated that document has been added to collection, even though it isn't.");

            _mfsOperations.DeleteCollection (collectionID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.IsDocumentInCollection (0, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.IsDocumentInCollection (urlID, 0);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.IsDocumentInCollection (veryLargeDocumentID, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeCollectionID = UInt64.MaxValue;

            try {
                _mfsOperations.IsDocumentInCollection (fileID, veryLargeCollectionID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_RemoveDocumentFromCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            _mfsOperations.AddDocumentToCollection (noteID, collectionID);

            bool isRemoved = _mfsOperations.RemoveDocumentFromCollection (noteID, collectionID);
            Assert.IsTrue (isRemoved, "Document was not successfully removed from collection.");

            bool isInCollection = _mfsOperations.IsDocumentInCollection (noteID, collectionID);
            Assert.IsFalse (isInCollection, "Attempt to remove document from collection was not successful.");

            _mfsOperations.DeleteCollection (collectionID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.RemoveDocumentFromCollection (0, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.RemoveDocumentFromCollection (urlID, 0);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.RemoveDocumentFromCollection (veryLargeDocumentID, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeCollectionID = UInt64.MaxValue;

            try {
                _mfsOperations.RemoveDocumentFromCollection (fileID, veryLargeCollectionID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetCollectionsWithDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numCollectionsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.AddDocumentToCollections (fileID, collectionIDs);

            List<ulong> retrievedCollectionIDs = _mfsOperations.GetCollectionsWithDocument (fileID);

            Assert.AreEqual (collectionIDs.Count, retrievedCollectionIDs.Count, "Did not retrieve exact number of collections that a document belongs to.");

            retrievedCollectionIDs.Sort ();
            collectionIDs.Sort ();

            for (int i = 0; i < collectionIDs.Count; ++i) {
                Assert.AreEqual (collectionIDs[i], retrievedCollectionIDs[i], "Got invalid collection id.");
            }

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.GetCollectionsWithDocument (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.GetCollectionsWithDocument (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetDocumentsInCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                _mfsOperations.AddDocumentToCollection (fileID, collectionID);

                docIDs.Add (fileID);
            }

            List<ulong> retrievedDocIDs = _mfsOperations.GetDocumentsInCollection (collectionID);

            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents within a collection.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.DeleteCollection (collectionID);

            foreach (ulong fileID in docIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _mfsOperations.GetDocumentsInCollection (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            _mfsOperations.GetDocumentsInCollection (veryLargeCollectionID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_AddDocumentToCollections : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            int numCollectionsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.AddDocumentToCollections (urlID, collectionIDs);

            List<ulong> retrievedCollectionIDs = _mfsOperations.GetCollectionsWithDocument (urlID);

            Assert.AreEqual (collectionIDs.Count, retrievedCollectionIDs.Count, "Did not retrieve exact number of collections.");

            retrievedCollectionIDs.Sort ();
            collectionIDs.Sort ();

            for (int i = 0; i < collectionIDs.Count; ++i) {
                Assert.AreEqual (collectionIDs[i], retrievedCollectionIDs[i], "Got invalid collection id.");
            }

            _mfsOperations.DeleteUrl (urlID);

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullCollectionList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.AddDocumentToCollections (fileID, null);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyCollectionList_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.AddDocumentToCollections (noteID, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            List<ulong> collectionIDs = new List<ulong> ();
            collectionIDs.Add (collectionID);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.AddDocumentToCollections (veryLargeDocumentID, collectionIDs);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_AddDocumentsToCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                docIDs.Add (urlID);
            }

            _mfsOperations.AddDocumentsToCollection (docIDs, collectionID);

            List<ulong> retrievedDocIDs = _mfsOperations.GetDocumentsInCollection (collectionID);
            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents added to a collection.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.DeleteCollection (collectionID);

            foreach (ulong urlID in docIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.AddDocumentsToCollection (null, collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.AddDocumentsToCollection (new List<ulong> (), collectionID);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_AddDocumentsToCollections : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numDocsToCreate = 3;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                docIDs.Add (urlID);
            }

            int numCollectionsToCreate = 3;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.AddDocumentsToCollections (docIDs, collectionIDs);

            foreach (ulong fileID in docIDs) {
                List<ulong> retrCollections = _mfsOperations.GetCollectionsWithDocument (fileID);
                Assert.AreEqual (collectionIDs.Count, retrCollections.Count, "Collection count for document does not match.");
            }

            foreach (ulong collectionID in collectionIDs) {
                List<ulong> retrFiles = _mfsOperations.GetDocumentsInCollection (collectionID);
                Assert.AreEqual (docIDs.Count, retrFiles.Count, "Document count for collection does not match.");
            }

            foreach (ulong urlID in docIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            List<ulong> collectionIDs = new List<ulong> ();
            collectionIDs.Add (collectionID);

            try {
                _mfsOperations.AddDocumentsToCollections (null, collectionIDs);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            List<ulong> collectionIDs = new List<ulong> ();
            collectionIDs.Add (collectionID);

            try {
                _mfsOperations.AddDocumentsToCollections (new List<ulong> (), collectionIDs);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullCollectionList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<ulong> docIDs = new List<ulong> ();
            docIDs.Add (fileID);

            try {
                _mfsOperations.AddDocumentsToCollections (docIDs, null);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyCollectionList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<ulong> docIDs = new List<ulong> ();
            docIDs.Add (fileID);

            try {
                _mfsOperations.AddDocumentsToCollections (docIDs, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_RemoveDocumentFromAllCollections : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            int numCollectionsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.AddDocumentToCollections (noteID, collectionIDs);

            int numCollectionsRemovedFrom = _mfsOperations.RemoveDocumentFromAllCollections (noteID);
            Assert.AreEqual (numCollectionsToCreate, numCollectionsRemovedFrom, "Attempt to remove document from all collections was unsuccessful.");

            List<ulong> allCollections = _mfsOperations.GetCollectionsWithDocument (noteID);
            Assert.AreEqual (0, allCollections.Count, "Incorrect number of collections from which document was removed.");

            _mfsOperations.DeleteNote (noteID);

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.RemoveDocumentFromAllCollections (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.RemoveDocumentFromAllCollections (veryLargeDocID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_RemoveAllDocumentsFromCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                docIDs.Add (fileID);
            }

            _mfsOperations.AddDocumentsToCollection (docIDs, collectionID);

            int numFilesUnappliedFrom = _mfsOperations.RemoveAllDocumentsFromCollection (collectionID);
            Assert.AreEqual (numDocsToCreate, numFilesUnappliedFrom, "Attempt to remove all documents from collection was unsuccessful.");

            List<ulong> allDocs = _mfsOperations.GetDocumentsInCollection (collectionID);
            Assert.AreEqual (0, allDocs.Count, "Incorrect number of documents that were removed from collection.");

            foreach (ulong fileID in docIDs) {
                _mfsOperations.DeleteFile (fileID);
            }

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _mfsOperations.RemoveAllDocumentsFromCollection (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            _mfsOperations.RemoveAllDocumentsFromCollection (veryLargeCollectionID);
        }
    }
}
