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

namespace MnemonicFS.Tests.Bookmarks {
    [TestFixture]
    public class TestBookmarks_BookmarkDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);
            List<ulong> bookmarkedDocIDs = new List<ulong> ();

            for (int i = 0; i < numDocsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);
                docIDs.Add (urlID);

                // Of these documents, we add bookmarks to only a few, for example all even-numbered document-IDs:
                if (urlID % 2 == 0) {
                    _mfsOperations.BookmarkDocument (urlID);
                    bookmarkedDocIDs.Add (urlID);
                }
            }

            List<ulong> allBookmarkedDocIDs = _mfsOperations.GetAllBookmarkedDocuments ();

            Assert.AreEqual (bookmarkedDocIDs, allBookmarkedDocIDs, "Expected bookmarked documents not returned.");

            foreach (ulong urlID in docIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.BookmarkDocument (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.BookmarkDocument (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class TestBookmarks_GetBookmarkedDocuments : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numDocumentsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> documentIDs = new List<ulong> (numDocumentsToCreate);
            List<ulong> bookmarkedDocumentIDs = new List<ulong> ();

            for (int i = 0; i < numDocumentsToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);
                documentIDs.Add (noteID);

                if (noteID % 2 == 0) {
                    _mfsOperations.BookmarkDocument (noteID);
                    bookmarkedDocumentIDs.Add (noteID);
                }
            }

            List<ulong> allBookmarkedDocIDs = _mfsOperations.GetAllBookmarkedDocuments ();

            Assert.AreEqual (bookmarkedDocumentIDs, allBookmarkedDocIDs, "Expected bookmarked documents not returned.");

            foreach (ulong noteID in documentIDs) {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        public void Test_DocumentDeleteShouldDeleteBookmark () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            _mfsOperations.BookmarkDocument (fileID);

            List<ulong> allBookmarkedDocIDs = _mfsOperations.GetAllBookmarkedDocuments ();
            Assert.Contains (fileID, allBookmarkedDocIDs, "Bookmarked file list does not contain bookmarked document.");

            _mfsOperations.DeleteFile (fileID);

            allBookmarkedDocIDs = _mfsOperations.GetAllBookmarkedDocuments ();
            Assert.IsFalse (allBookmarkedDocIDs.Contains (fileID), "Bookmarked document list contains document reference after its deletion.");
        }
    }

    [TestFixture]
    public class TestBookmarks_IsDocumentBookmarked : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            bool isBookmarked = _mfsOperations.IsDocumentBookmarked (urlID);
            Assert.IsFalse (isBookmarked, "Document is shown as bookmarked even though it isn't.");

            _mfsOperations.BookmarkDocument (urlID);

            isBookmarked = _mfsOperations.IsDocumentBookmarked (urlID);
            Assert.IsTrue (isBookmarked, "Document is not shown as bookmarked even though it is.");

            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.IsDocumentBookmarked (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.IsDocumentBookmarked (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteDocumentBookmark : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numDocumentsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> documentIDs = new List<ulong> (numDocumentsToCreate);
            List<ulong> bookmarkedDocIDs = new List<ulong> ();

            ulong docIDToBeUnbookmarked = 0;

            for (int i = 0; i < numDocumentsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);
                documentIDs.Add (urlID);

                // Bookmark each document:
                _mfsOperations.BookmarkDocument (urlID);
                bookmarkedDocIDs.Add (urlID);

                // We will delete the bookmark of one of the documents bookmarked, say the zeroeth one:
                if (i == 0) {
                    docIDToBeUnbookmarked = urlID;
                }
            }

            _mfsOperations.DeleteDocumentBookmark (docIDToBeUnbookmarked);

            List<ulong> allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedDocuments ();
            Assert.AreEqual (bookmarkedDocIDs.Count - 1, allBookmarkedFileIDs.Count, "Document bookmark not deleted properly.");
            Assert.IsFalse (allBookmarkedFileIDs.Contains (docIDToBeUnbookmarked), "Document bookmark still in bookmarked list after deletion.");

            foreach (ulong urlID in documentIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocumentIDZero_Illegal () {
            _mfsOperations.DeleteDocumentBookmark (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocumentID_Illegal () {
            ulong veryLargeDocumentID = UInt64.MaxValue;

            _mfsOperations.DeleteDocumentBookmark (veryLargeDocumentID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteAllDocumentBookmarks : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numDocsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> docIDs = new List<ulong> (numDocsToCreate);

            for (int i = 0; i < numDocsToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);
                docIDs.Add (noteID);

                _mfsOperations.BookmarkDocument (noteID);
            }

            List<ulong> allBookmarkedDocIDs = _mfsOperations.GetAllBookmarkedDocuments ();
            Assert.AreEqual (docIDs.Count, allBookmarkedDocIDs.Count, "Bookmarked document list contains incorrect number of entries.");

            _mfsOperations.DeleteAllDocumentBookmarks ();

            allBookmarkedDocIDs = _mfsOperations.GetAllBookmarkedDocuments ();
            Assert.AreEqual (0, allBookmarkedDocIDs.Count, "Bookmarked document list contains entries after deletion of all document bookmarks.");

            foreach (ulong noteID in docIDs) {
                _mfsOperations.DeleteNote (noteID);
            }
        }
    }
}
