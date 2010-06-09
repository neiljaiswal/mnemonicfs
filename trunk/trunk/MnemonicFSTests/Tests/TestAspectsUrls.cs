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

namespace MnemonicFS.Tests.AspectsUrls {
    [TestFixture]
    public class TestUrls_AspectsMethod_ApplyAspectToUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.ApplyAspectToUrl (aspectID, urlID);
            Assert.IsTrue (applySuccess, "Aspect failed to be applied to url.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToUrl (aspectID, 0);
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
                _mfsOperations.ApplyAspectToUrl (0, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeUrlID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToUrl (aspectID, veryLargeUrlID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToUrl (veryLargeAspectID, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_IsAspectAppliedToUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.ApplyAspectToUrl (aspectID, urlID);

            bool isApplied = _mfsOperations.IsAspectAppliedToUrl (aspectID, urlID);
            Assert.IsTrue (isApplied, "Indicated that aspect has not been applied to url, even though it is.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        public void Test_SanityCheck_NotApplied () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool isApplied = _mfsOperations.IsAspectAppliedToUrl (aspectID, urlID);
            Assert.IsFalse (isApplied, "Indicated that aspect has been applied to url, even though it isn't.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.IsAspectAppliedToUrl (aspectID, 0);
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
                _mfsOperations.IsAspectAppliedToUrl (0, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeUrlID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToUrl (aspectID, veryLargeUrlID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToUrl (veryLargeAspectID, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            _mfsOperations.ApplyAspectToUrl (aspectID, urlID);

            bool isUnapplied = _mfsOperations.UnapplyAspectFromUrl (aspectID, urlID);
            Assert.IsTrue (isUnapplied, "Aspect was not successfully unapplied from url.");

            bool isApplied = _mfsOperations.IsAspectAppliedToUrl (aspectID, urlID);
            Assert.IsFalse (isApplied, "Attempt to unapply aspect from url was not successful.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteUrl (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.UnapplyAspectFromUrl (aspectID, 0);
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
                _mfsOperations.UnapplyAspectFromUrl (0, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeUrlID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromUrl (aspectID, veryLargeUrlID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromUrl (veryLargeAspectID, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectsAppliedOnUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.ApplyAspectToUrl (aspectID, urlID);
            }

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnUrl (urlID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects applied to url.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteUrl (urlID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.GetAspectsAppliedOnUrl (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            _mfsOperations.GetAspectsAppliedOnUrl (veryLargeUrlID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetUrlsAppliedWithAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                _mfsOperations.ApplyAspectToUrl (aspectID, urlID);

                urlIDs.Add (urlID);
            }

            List<ulong> retrievedUrlIDs = _mfsOperations.GetUrlsAppliedWithAspect (aspectID);
            Assert.AreEqual (urlIDs.Count, retrievedUrlIDs.Count, "Did not retrieve exact number of urls aspect has been applied to.");

            retrievedUrlIDs.Sort ();
            urlIDs.Sort ();

            for (int i = 0; i < urlIDs.Count; ++i) {
                Assert.AreEqual (urlIDs[i], retrievedUrlIDs[i], "Got invalid url id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong urlID in urlIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.GetUrlsAppliedWithAspect (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.GetUrlsAppliedWithAspect (veryLargeAspectID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToUrl (aspectIDs, urlID);

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnUrl (urlID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteUrl (urlID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.ApplyAspectsToUrl (null, urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.ApplyAspectsToUrl (new List<ulong> (), urlID);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            ulong veryLargeUrlID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectsToUrl (aspectIDs, veryLargeUrlID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToUrls : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                urlIDs.Add (urlID);
            }

            _mfsOperations.ApplyAspectToUrls (aspectID, urlIDs);

            List<ulong> retrievedUrlIDs = _mfsOperations.GetUrlsAppliedWithAspect (aspectID);
            Assert.AreEqual (urlIDs.Count, retrievedUrlIDs.Count, "Did not retrieve exact number of urls aspect has been applied to.");

            retrievedUrlIDs.Sort ();
            urlIDs.Sort ();

            for (int i = 0; i < urlIDs.Count; ++i) {
                Assert.AreEqual (urlIDs[i], retrievedUrlIDs[i], "Got invalid url id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong urlID in urlIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullUrlList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToUrls (aspectID, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyUrlList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToUrls (aspectID, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToUrls : TestMfsOperationsBase {
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
            }

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToUrls (aspectIDs, urlIDs);

            foreach (ulong urlID in urlIDs) {
                List<ulong> retrAspects = _mfsOperations.GetAspectsAppliedOnUrl (urlID);
                Assert.AreEqual (aspectIDs.Count, retrAspects.Count, "Aspect count for url does not match.");
            }

            foreach (ulong aspectID in aspectIDs) {
                List<ulong> retrUrls = _mfsOperations.GetUrlsAppliedWithAspect (aspectID);
                Assert.AreEqual (urlIDs.Count, retrUrls.Count, "Url count for aspect does not match.");
            }

            foreach (ulong urlID in urlIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullUrlList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToUrls (aspectIDs, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyUrlList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToUrls (aspectIDs, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            List<ulong> urlIDs = new List<ulong> ();
            urlIDs.Add (urlID);

            try {
                _mfsOperations.ApplyAspectsToUrls (null, urlIDs);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            List<ulong> urlIDs = new List<ulong> ();
            urlIDs.Add (urlID);

            try {
                _mfsOperations.ApplyAspectsToUrls (new List<ulong> (), urlIDs);
            } finally {
                _mfsOperations.DeleteUrl (urlID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAllAspectsFromUrl : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToUrl (aspectIDs, urlID);

            int numAspectsUnapplied = _mfsOperations.UnapplyAllAspectsFromUrl (urlID);
            Assert.AreEqual (numAspectsToCreate, numAspectsUnapplied, "Attempt to unapply aspects from url was unsuccessful.");

            List<ulong> allAspects = _mfsOperations.GetAspectsAppliedOnUrl (urlID);
            Assert.AreEqual (0, allAspects.Count, "Incorrect number of aspects unapplied for url.");

            _mfsOperations.DeleteUrl (urlID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UrlIDZero_Illegal () {
            _mfsOperations.UnapplyAllAspectsFromUrl (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentUrlID_Illegal () {
            ulong veryLargeUrlID = UInt64.MaxValue;

            _mfsOperations.UnapplyAllAspectsFromUrl (veryLargeUrlID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromAllUrls : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numUrlsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> urlIDs = new List<ulong> (numUrlsToCreate);

            for (int i = 0; i < numUrlsToCreate; ++i) {
                string url = TestUtils.GetAnyUrl ();
                string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                ulong urlID = _mfsOperations.AddUrl (url, description, when);

                urlIDs.Add (urlID);
            }

            _mfsOperations.ApplyAspectToUrls (aspectID, urlIDs);

            int numUrlsUnappliedFrom = _mfsOperations.UnapplyAspectFromAllUrls (aspectID);
            Assert.AreEqual (numUrlsToCreate, numUrlsUnappliedFrom, "Attempt to unapply aspect from urls was unsuccessful.");

            List<ulong> allUrls = _mfsOperations.GetUrlsAppliedWithAspect (aspectID);
            Assert.AreEqual (0, allUrls.Count, "Number of urls that aspect was unapplied from was incorrect.");

            foreach (ulong urlID in urlIDs) {
                _mfsOperations.DeleteUrl (urlID);
            }

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.UnapplyAspectFromAllUrls (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.UnapplyAspectFromAllUrls (veryLargeAspectID);
        }
    }
}
