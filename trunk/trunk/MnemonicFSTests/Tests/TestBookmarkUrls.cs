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

namespace MnemonicFS.Tests.Bookmarks.Urls {
    [TestFixture]
    public class TestBookmarks_BookmarkUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);
            List<ulong> bookmarkedUrlIDs = new List<ulong> ();

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;

                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                // Of these urls, we add bookmarks to only a few, for example all even-numbered url IDs:
                if (urlID % 2 == 0) {
                    _mfsOperations.BookmarkUrl (urlID);
                    bookmarkedUrlIDs.Add (urlID);
                }
            }

            List<ulong> allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();

            Assert.AreEqual (bookmarkedUrlIDs, allBookmarkedUrlIDs, "Expected bookmarked urls not returned.");

            foreach (ulong urlID in urlIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.BookmarkUrl (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            _mfsOperations.BookmarkUrl (veryLargeUrlID);
        }
    }

    [TestFixture]
    public class TestBookmarks_GetBookmarkedUrls : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);
            List<ulong> bookmarkedUrlIDs = new List<ulong> ();

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;

                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                // Of these urls, we add bookmarks to only a few, for example all even-numbered url IDs:
                if (urlID % 2 == 0) {
                    _mfsOperations.BookmarkUrl (urlID);
                    bookmarkedUrlIDs.Add (urlID);
                }
            }

            List<ulong> allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();

            Assert.AreEqual (bookmarkedUrlIDs, allBookmarkedUrlIDs, "Expected bookmarked urls not returned.");

            foreach (ulong urlID in urlIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        public void Test_UrlDeleteShouldDeleteBookmark () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            _mfsOperations.BookmarkUrl (urlID);

            List<ulong> allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();
            Assert.Contains (urlID, allBookmarkedUrlIDs, "Bookmarked url list does not contain bookmarked url.");

            _mfsOperations.DeleteUrl (urlID);

            allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();
            Assert.IsFalse (allBookmarkedUrlIDs.Contains (urlID), "Bookmarked url list contains url reference after url deletion.");
        }
    }

    [TestFixture]
    public class TestBookmarks_IsUrlBookmarked : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            bool isBookmarked = _mfsOperations.IsUrlBookmarked (urlID);
            Assert.IsFalse (isBookmarked, "Url is shown as bookmarked even though it isn't.");

            _mfsOperations.BookmarkUrl (urlID);

            isBookmarked = _mfsOperations.IsUrlBookmarked (urlID);
            Assert.IsTrue (isBookmarked, "Url is not shown as bookmarked even though it is.");

            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.IsUrlBookmarked (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            _mfsOperations.IsUrlBookmarked (veryLargeUrlID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteUrlBookmark : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);
            List<ulong> bookmarkedUrlIDs = new List<ulong> ();
            ulong urlIDToBeUnbookmarked = 0;

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                // Bookmark each url:
                _mfsOperations.BookmarkUrl (urlID);
                bookmarkedUrlIDs.Add (urlID);

                // We will delete the bookmark of one of the urls bookmarked, say the zeroeth one:
                if (i == 0) {
                    urlIDToBeUnbookmarked = urlID;
                }
            }

            _mfsOperations.DeleteUrlBookmark (urlIDToBeUnbookmarked);

            List<ulong> allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();
            Assert.AreEqual (bookmarkedUrlIDs.Count - 1, allBookmarkedUrlIDs.Count, "Url bookmark not deleted properly.");
            Assert.IsFalse (allBookmarkedUrlIDs.Contains (urlIDToBeUnbookmarked), "Url bookmark still in bookmarked list after deletion.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.DeleteUrlBookmark (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            _mfsOperations.DeleteUrlBookmark (veryLargeUrlID);
        }
    }

    [TestFixture]
    public class TestBookmarks_DeleteAllUrlBookmarks : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);
                urlIDs.Add (urlID);

                // Bookmark each url:
                _mfsOperations.BookmarkUrl (urlID);
            }

            List<ulong> allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();
            Assert.AreEqual (urlIDs.Count, allBookmarkedUrlIDs.Count, "Bookmarked url list contains incorrect number of entries.");

            _mfsOperations.DeleteAllUrlBookmarks ();

            allBookmarkedUrlIDs = _mfsOperations.GetAllBookmarkedUrls ();
            Assert.AreEqual (0, allBookmarkedUrlIDs.Count, "Bookmarked url list contains entries after deletion of all url bookmarks.");
        }
    }
}
