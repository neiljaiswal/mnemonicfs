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
using MnemonicFS.MfsExceptions;
using MnemonicFS.Tests.Utils;
using MnemonicFS.Tests.Base;
using MnemonicFS.MfsCore;

namespace MnemonicFS.Tests.Urls {
    [TestFixture]
    public class TestUrls_UrlMethod_AddUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);
            Assert.That (urlID > 0, "Url id returned is not a valid value.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        public void Test_UrlWithMaxSize_SanityCheck () {
            string url = TestUtils.GetAWord (MfsOperations.MaxUrlLength);
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);
            Assert.That (urlID > 0, "Url id returned is not a valid value.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlNull_Illegal () {
            string nullUrl = null;
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            _mfsOperations.Url.New (nullUrl, description, when);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInUrl = MfsOperations.MaxUrlLength;

            string longUrl = TestUtils.GetAWord (maxCharsInUrl + 1);
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            _mfsOperations.Url.New (longUrl, description, when);
        }

        [Test]
        public void Test_UrlDescriptionNull_Legal () {
            string url = TestUtils.GetAnyUrl ();
            string description = null;
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);
            Assert.That (urlID > 0, "Url id returned is not a valid value.");

            _mfsOperations.Url.Delete (urlID);
        }
    }

    [TestFixture]
    public class TestUrls_UrlMethod_DoesUrlExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = null;
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);

            bool urlExists = _mfsOperations.Url.Exists (urlID);
            Assert.IsTrue (urlExists, "Shows url as not existing even though it does.");

            int numDeleted = _mfsOperations.Url.Delete (urlID);
            Assert.AreEqual (1, numDeleted, "Did not delete the url.");

            urlExists = _mfsOperations.Url.Exists (urlID);
            Assert.IsFalse (urlExists, "Shows url as existing even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.Url.Exists (0);
        }
    }

    [TestFixture]
    public class TestUrls_UrlMethod_DeleteUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = null;
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);

            int numDeleted = _mfsOperations.Url.Delete (urlID);
            Assert.AreEqual (1, numDeleted, "Did not delete single url.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.Url.Delete (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrl_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            _mfsOperations.Url.Delete (veryLargeUrlID);
        }
    }

    [TestFixture]
    public class TestUrls_UrlMethod_GetUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);

            string retrUrl;
            string retrDescription;
            DateTime retrWhen;
            _mfsOperations.Url.GetDetails (urlID, out retrUrl, out retrDescription, out retrWhen);
            Assert.AreEqual (url, retrUrl, "Retrieved url is not the same as the one saved.");
            Assert.AreEqual (description, retrDescription, "Retrieved url description is not the same as the one saved.");
            Assert.AreEqual (when.Year, retrWhen.Year, "Year for url was incorrect.");
            Assert.AreEqual (when.Month, retrWhen.Month, "Month for url was incorrect.");
            Assert.AreEqual (when.Day, retrWhen.Day, "Day for url was incorrect.");
            Assert.AreEqual (when.Hour, retrWhen.Hour, "Hour for url was incorrect.");
            Assert.AreEqual (when.Minute, retrWhen.Minute, "Minute for url was incorrect.");
            Assert.AreEqual (when.Second, retrWhen.Second, "Second for url was incorrect.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            string retrUrl;
            string retrDescription;
            DateTime retrWhen;
            _mfsOperations.Url.GetDetails (0, out retrUrl, out retrDescription, out retrWhen);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrl_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            string retrUrl;
            string retrDescription;
            DateTime retrWhen;
            _mfsOperations.Url.GetDetails (veryLargeUrlID, out retrUrl, out retrDescription, out retrWhen);
        }
    }

    [TestFixture]
    public class TestUrls_UrlMethod_UpdateUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);

            string newUrl = TestUtils.GetAnyUrl ();
            bool updated = _mfsOperations.Url.Update (urlID, newUrl);

            Assert.IsTrue (updated, "Url was not updated.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            _mfsOperations.Url.Update (0, url);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrl_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            string url = TestUtils.GetAnyUrl ();
            _mfsOperations.Url.Update (veryLargeUrlID, url);
        }
    }

    [TestFixture]
    public class TestUrls_UrlMethod_UpdateUrlDescription : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);

            string newDescription = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            bool updated = _mfsOperations.Url.UpdateDescription (urlID, newDescription);

            Assert.IsTrue (updated, "Url description was not updated.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            _mfsOperations.Url.UpdateDescription (0, description);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrl_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            _mfsOperations.Url.UpdateDescription (veryLargeUrlID, description);
        }
    }

    [TestFixture]
    public class TestUrls_UrlMethod_UpdateUrlDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            ulong urlID = _mfsOperations.Url.New (url, description, when);

            DateTime newDateTime = DateTime.Now.AddDays (-1);
            bool updated = _mfsOperations.Url.UpdateDateTime (urlID, newDateTime);

            Assert.IsTrue (updated, "Url date-time was not updated.");

            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            DateTime when = DateTime.Now;
            _mfsOperations.Url.UpdateDateTime (0, when);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrl_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            DateTime when = DateTime.Now;
            _mfsOperations.Url.UpdateDateTime (veryLargeUrlID, when);
        }
    }
}
