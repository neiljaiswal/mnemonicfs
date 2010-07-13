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

namespace MnemonicFS.Tests.Bookmarks.Files {
    [TestFixture]
    public class TestBookmarks_BookmarkFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);
            List<ulong> bookmarkedFileIDs = new List<ulong> ();

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
                fileIDs.Add (fileID);

                // Of these files, we add bookmarks to only a few, for example all even-numbered file IDs:
                if (fileID % 2 == 0) {
                    _mfsOperations.BookmarkFile (fileID);
                    bookmarkedFileIDs.Add (fileID);
                }
            }

            List<ulong> allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();

            Assert.AreEqual (bookmarkedFileIDs, allBookmarkedFileIDs, "Expected bookmarked files not returned.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.BookmarkFile (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.BookmarkFile (veryLargeFileID);
        }
    }

    [TestFixture]
    public class TestBookmarks_GetBookmarkedFiles : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);
            List<ulong> bookmarkedFileIDs = new List<ulong> ();

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
                fileIDs.Add (fileID);

                if (fileID % 2 == 0) {
                    _mfsOperations.BookmarkFile (fileID);
                    bookmarkedFileIDs.Add (fileID);
                }
            }

            List<ulong> allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();

            Assert.AreEqual (bookmarkedFileIDs, allBookmarkedFileIDs, "Expected bookmarked files not returned.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        public void Test_FileDeleteShouldDeleteBookmark () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            _mfsOperations.BookmarkFile (fileID);

            List<ulong> allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();
            Assert.Contains (fileID, allBookmarkedFileIDs, "Bookmarked file list does not contain bookmarked file.");

            _mfsOperations.DeleteFile (fileID);

            allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();
            Assert.IsFalse (allBookmarkedFileIDs.Contains (fileID), "Bookmarked file list contains file reference after file deletion.");

        }
    }

    [TestFixture]
    public class TestBookmarks_IsFileBookmarked : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            bool isBookmarked = _mfsOperations.IsFileBookmarked (fileID);
            Assert.IsFalse (isBookmarked, "File is shown as bookmarked even though it isn't.");

            _mfsOperations.BookmarkFile (fileID);

            isBookmarked = _mfsOperations.IsFileBookmarked (fileID);
            Assert.IsTrue (isBookmarked, "File is not shown as bookmarked even though it is.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.IsFileBookmarked (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.IsFileBookmarked (veryLargeFileID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteFileBookmark : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);
            List<ulong> bookmarkedFileIDs = new List<ulong> ();

            ulong fileIDToBeUnbookmarked = 0;

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
                fileIDs.Add (fileID);
                
                // Bookmark each file:
                _mfsOperations.BookmarkFile (fileID);
                bookmarkedFileIDs.Add (fileID);

                // We will delete the bookmark of one of the files bookmarked, say the zeroeth one:
                if (i == 0) {
                    fileIDToBeUnbookmarked = fileID;
                }
            }

            _mfsOperations.DeleteFileBookmark (fileIDToBeUnbookmarked);

            List<ulong> allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();
            Assert.AreEqual (bookmarkedFileIDs.Count - 1, allBookmarkedFileIDs.Count, "File bookmark not deleted properly.");
            Assert.IsFalse (allBookmarkedFileIDs.Contains (fileIDToBeUnbookmarked), "File bookmark still in bookmarked list after deletion.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.DeleteFileBookmark (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.DeleteFileBookmark (veryLargeFileID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteAllFileBookmarks : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
                fileIDs.Add (fileID);

                _mfsOperations.BookmarkFile (fileID);
            }

            List<ulong> allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();
            Assert.AreEqual (fileIDs.Count, allBookmarkedFileIDs.Count, "Bookmarked file list contains incorrect number of entries.");

            _mfsOperations.DeleteAllFileBookmarks ();

            allBookmarkedFileIDs = _mfsOperations.GetAllBookmarkedFiles ();
            Assert.AreEqual (0, allBookmarkedFileIDs.Count, "Bookmarked file list contains entries after deletion of all file bookmarks.");
        }
    }
}
