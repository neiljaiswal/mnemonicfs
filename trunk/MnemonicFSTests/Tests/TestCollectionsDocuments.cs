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
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            bool addSuccess = _mfsOperations.Collection.AddDocument (urlID, collectionID);
            Assert.IsTrue (addSuccess, "Document was not successfully added to collection.");

            _mfsOperations.Collection.Delete (collectionID);
            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            try {
                _mfsOperations.Collection.AddDocument (0, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.Collection.AddDocument (fileID, 0);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            try {
                _mfsOperations.Collection.AddDocument (veryLargeDocumentID, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
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
                _mfsOperations.Collection.AddDocument (fileID, veryLargeCollectionID);
            } finally {
                _mfsOperations.File.Delete (fileID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            _mfsOperations.Collection.AddDocument (noteID, collectionID);

            bool isAdded = _mfsOperations.Collection.IsDocumentIn (noteID, collectionID);
            Assert.IsTrue (isAdded, "Indicated that document is not in collection, even though it is.");

            _mfsOperations.Collection.Delete (collectionID);
            _mfsOperations.Note.Delete (noteID);
        }

        [Test]
        public void Test_SanityCheck_NotInCollection () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            bool isAdded = _mfsOperations.Collection.IsDocumentIn (fileID, collectionID);
            Assert.IsFalse (isAdded, "Indicated that document has been added to collection, even though it isn't.");

            _mfsOperations.Collection.Delete (collectionID);
            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            try {
                _mfsOperations.Collection.IsDocumentIn (0, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Collection.IsDocumentIn (urlID, 0);
            } finally {
                _mfsOperations.Url.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Collection.IsDocumentIn (veryLargeDocumentID, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
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
                _mfsOperations.Collection.IsDocumentIn (fileID, veryLargeCollectionID);
            } finally {
                _mfsOperations.File.Delete (fileID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            _mfsOperations.Collection.AddDocument (noteID, collectionID);

            bool isRemoved = _mfsOperations.Collection.RemoveFrom (noteID, collectionID);
            Assert.IsTrue (isRemoved, "Document was not successfully removed from collection.");

            bool isInCollection = _mfsOperations.Collection.IsDocumentIn (noteID, collectionID);
            Assert.IsFalse (isInCollection, "Attempt to remove document from collection was not successful.");

            _mfsOperations.Collection.Delete (collectionID);
            _mfsOperations.Note.Delete (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            try {
                _mfsOperations.Collection.RemoveFrom (0, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Collection.RemoveFrom (urlID, 0);
            } finally {
                _mfsOperations.Url.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Collection.RemoveFrom (veryLargeDocumentID, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
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
                _mfsOperations.Collection.RemoveFrom (fileID, veryLargeCollectionID);
            } finally {
                _mfsOperations.File.Delete (fileID);
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

            _mfsOperations.Collection.AddToMultiple (fileID, collectionIDs);

            List<ulong> retrievedCollectionIDs = _mfsOperations.Collection.CollectionsWith (fileID);

            Assert.AreEqual (collectionIDs.Count, retrievedCollectionIDs.Count, "Did not retrieve exact number of collections that a document belongs to.");

            retrievedCollectionIDs.Sort ();
            collectionIDs.Sort ();

            for (int i = 0; i < collectionIDs.Count; ++i) {
                Assert.AreEqual (collectionIDs[i], retrievedCollectionIDs[i], "Got invalid collection id.");
            }

            _mfsOperations.File.Delete (fileID);

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.Collection.CollectionsWith (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.Collection.CollectionsWith (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetDocumentsInCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                _mfsOperations.Collection.AddDocument (fileID, collectionID);

                docIDs.Add (fileID);
            }

            List<ulong> retrievedDocIDs = _mfsOperations.Collection.GetDocuments (collectionID);

            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents within a collection.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.Collection.Delete (collectionID);

            foreach (ulong fileID in docIDs) {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _mfsOperations.Collection.GetDocuments (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            _mfsOperations.Collection.GetDocuments (veryLargeCollectionID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_AddDocumentToCollections : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            int numCollectionsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.Collection.AddToMultiple (urlID, collectionIDs);

            List<ulong> retrievedCollectionIDs = _mfsOperations.Collection.CollectionsWith (urlID);

            Assert.AreEqual (collectionIDs.Count, retrievedCollectionIDs.Count, "Did not retrieve exact number of collections.");

            retrievedCollectionIDs.Sort ();
            collectionIDs.Sort ();

            for (int i = 0; i < collectionIDs.Count; ++i) {
                Assert.AreEqual (collectionIDs[i], retrievedCollectionIDs[i], "Got invalid collection id.");
            }

            _mfsOperations.Url.Delete (urlID);

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullCollectionList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.Collection.AddToMultiple (fileID, null);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyCollectionList_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Collection.AddToMultiple (noteID, new List<ulong> ());
            } finally {
                _mfsOperations.Note.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            List<ulong> collectionIDs = new List<ulong> ();
            collectionIDs.Add (collectionID);

            ulong veryLargeDocumentID = UInt64.MaxValue;

            try {
                _mfsOperations.Collection.AddToMultiple (veryLargeDocumentID, collectionIDs);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_AddDocumentsToCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.Url.New (url, description, when);

                docIDs.Add (urlID);
            }

            _mfsOperations.Collection.AddTo (docIDs, collectionID);

            List<ulong> retrievedDocIDs = _mfsOperations.Collection.GetDocuments (collectionID);
            Assert.AreEqual (docIDs.Count, retrievedDocIDs.Count, "Did not retrieve exact number of documents added to a collection.");

            retrievedDocIDs.Sort ();
            docIDs.Sort ();

            for (int i = 0; i < docIDs.Count; ++i) {
                Assert.AreEqual (docIDs[i], retrievedDocIDs[i], "Got invalid document id.");
            }

            _mfsOperations.Collection.Delete (collectionID);

            foreach (ulong urlID in docIDs) {
                _mfsOperations.Url.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            try {
                _mfsOperations.Collection.AddTo (null, collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            try {
                _mfsOperations.Collection.AddTo (new List<ulong> (), collectionID);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
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
                ulong urlID = _mfsOperations.Url.New (url, description, when);

                docIDs.Add (urlID);
            }

            int numCollectionsToCreate = 3;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.Collection.AddToMultiple (docIDs, collectionIDs);

            foreach (ulong fileID in docIDs) {
                List<ulong> retrCollections = _mfsOperations.Collection.CollectionsWith (fileID);
                Assert.AreEqual (collectionIDs.Count, retrCollections.Count, "Collection count for document does not match.");
            }

            foreach (ulong collectionID in collectionIDs) {
                List<ulong> retrFiles = _mfsOperations.Collection.GetDocuments (collectionID);
                Assert.AreEqual (docIDs.Count, retrFiles.Count, "Document count for collection does not match.");
            }

            foreach (ulong urlID in docIDs) {
                _mfsOperations.Url.Delete (urlID);
            }

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            List<ulong> collectionIDs = new List<ulong> ();
            collectionIDs.Add (collectionID);

            try {
                _mfsOperations.Collection.AddToMultiple (null, collectionIDs);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocumentList_Illegal () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            List<ulong> collectionIDs = new List<ulong> ();
            collectionIDs.Add (collectionID);

            try {
                _mfsOperations.Collection.AddToMultiple (new List<ulong> (), collectionIDs);
            } finally {
                _mfsOperations.Collection.Delete (collectionID);
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
                _mfsOperations.Collection.AddToMultiple (docIDs, null);
            } finally {
                _mfsOperations.File.Delete (fileID);
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
                _mfsOperations.Collection.AddToMultiple (docIDs, new List<ulong> ());
            } finally {
                _mfsOperations.File.Delete (fileID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            int numCollectionsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> collectionIDs = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            _mfsOperations.Collection.AddToMultiple (noteID, collectionIDs);

            int numCollectionsRemovedFrom = _mfsOperations.Collection.RemoveDocumentFromAll (noteID);
            Assert.AreEqual (numCollectionsToCreate, numCollectionsRemovedFrom, "Attempt to remove document from all collections was unsuccessful.");

            List<ulong> allCollections = _mfsOperations.Collection.CollectionsWith (noteID);
            Assert.AreEqual (0, allCollections.Count, "Incorrect number of collections from which document was removed.");

            _mfsOperations.Note.Delete (noteID);

            foreach (ulong collectionID in collectionIDs) {
                _mfsOperations.Collection.Delete (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.Collection.RemoveDocumentFromAll (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.Collection.RemoveDocumentFromAll (veryLargeDocID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_RemoveAllDocumentsFromCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.Collection.New (_collectionName, _collectionDesc);

            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                docIDs.Add (fileID);
            }

            _mfsOperations.Collection.AddTo (docIDs, collectionID);

            int numFilesUnappliedFrom = _mfsOperations.Collection.RemoveAllDocuments (collectionID);
            Assert.AreEqual (numDocsToCreate, numFilesUnappliedFrom, "Attempt to remove all documents from collection was unsuccessful.");

            List<ulong> allDocs = _mfsOperations.Collection.GetDocuments (collectionID);
            Assert.AreEqual (0, allDocs.Count, "Incorrect number of documents that were removed from collection.");

            foreach (ulong fileID in docIDs) {
                _mfsOperations.File.Delete (fileID);
            }

            _mfsOperations.Collection.Delete (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _mfsOperations.Collection.RemoveAllDocuments (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            _mfsOperations.Collection.RemoveAllDocuments (veryLargeCollectionID);
        }
    }
}
