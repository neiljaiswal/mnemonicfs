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

namespace MnemonicFS.Tests.Bookmarks.Notes {
    [TestFixture]
    public class TestBookmarks_BookmarkNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);
            List<ulong> bookmarkedNoteIDs = new List<ulong> ();

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                // Of these notes, we add bookmarks to only a few, for example all even-numbered note IDs:
                if (noteID % 2 == 0) {
                    _mfsOperations.BookmarkNote (noteID);
                    bookmarkedNoteIDs.Add (noteID);
                }
            }

            List<ulong> allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();

            Assert.AreEqual (bookmarkedNoteIDs, allBookmarkedNoteIDs, "Expected bookmarked notes not returned.");

            foreach (ulong noteID in noteIDs) {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            _mfsOperations.BookmarkNote (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong veryLargeNoteID = UInt64.MaxValue;

            _mfsOperations.BookmarkNote (veryLargeNoteID);
        }
    }

    [TestFixture]
    public class TestBookmarks_GetBookmarkedNotes : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);
            List<ulong> bookmarkedNoteIDs = new List<ulong> ();

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                // Of these notes, we add bookmarks to only a few, for example all even-numbered note IDs:
                if (noteID % 2 == 0) {
                    _mfsOperations.BookmarkNote (noteID);
                    bookmarkedNoteIDs.Add (noteID);
                }
            }

            List<ulong> allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();

            Assert.AreEqual (bookmarkedNoteIDs, allBookmarkedNoteIDs, "Expected bookmarked notes not returned.");

            foreach (ulong noteID in noteIDs) {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        public void Test_NoteDeleteShouldDeleteBookmark () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            _mfsOperations.BookmarkNote (noteID);

            List<ulong> allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();
            Assert.Contains (noteID, allBookmarkedNoteIDs, "Bookmarked note list does not contain bookmarked note.");

            _mfsOperations.DeleteNote (noteID);

            allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();
            Assert.IsFalse (allBookmarkedNoteIDs.Contains (noteID), "Bookmarked note list contains note reference after note deletion.");

        }
    }

    [TestFixture]
    public class TestBookmarks_IsNoteBookmarked : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            bool isBookmarked = _mfsOperations.IsNoteBookmarked (noteID);
            Assert.IsFalse (isBookmarked, "Note is shown as bookmarked even though it isn't.");

            _mfsOperations.BookmarkNote (noteID);

            isBookmarked = _mfsOperations.IsNoteBookmarked (noteID);
            Assert.IsTrue (isBookmarked, "Note is not shown as bookmarked even though it is.");

            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            _mfsOperations.IsNoteBookmarked (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong veryLargeNoteID = UInt64.MaxValue;

            _mfsOperations.IsNoteBookmarked (veryLargeNoteID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteNoteBookmark : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);
            List<ulong> bookmarkedNoteIDs = new List<ulong> ();
            ulong noteIDToBeUnbookmarked = 0;

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                // Bookmark each note:
                _mfsOperations.BookmarkNote (noteID);
                bookmarkedNoteIDs.Add (noteID);

                // We will delete the bookmark of one of the notes bookmarked, say the zeroeth one:
                if (i == 0) {
                    noteIDToBeUnbookmarked = noteID;
                }
            }

            _mfsOperations.DeleteNoteBookmark (noteIDToBeUnbookmarked);

            List<ulong> allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();
            Assert.AreEqual (bookmarkedNoteIDs.Count - 1, allBookmarkedNoteIDs.Count, "Note bookmark not deleted properly.");
            Assert.IsFalse (allBookmarkedNoteIDs.Contains (noteIDToBeUnbookmarked), "Note bookmark still in bookmarked list after deletion.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            _mfsOperations.DeleteNoteBookmark (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong veryLargeNoteID = UInt64.MaxValue;

            _mfsOperations.DeleteNoteBookmark (veryLargeNoteID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteAllNoteBookmarks : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);
                noteIDs.Add (noteID);

                // Bookmark each note:
                _mfsOperations.BookmarkNote (noteID);
            }

            List<ulong> allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();
            Assert.AreEqual (noteIDs.Count, allBookmarkedNoteIDs.Count, "Bookmarked note list contains incorrect number of entries.");

            _mfsOperations.DeleteAllNoteBookmarks ();

            allBookmarkedNoteIDs = _mfsOperations.GetAllBookmarkedNotes ();
            Assert.AreEqual (0, allBookmarkedNoteIDs.Count, "Bookmarked note list contains entries after deletion of all note bookmarks.");
        }
    }
}
