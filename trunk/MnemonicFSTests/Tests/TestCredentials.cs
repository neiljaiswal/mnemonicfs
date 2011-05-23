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

namespace MnemonicFS.Tests.Credentials {
    [TestFixture]
    public class Tests_AddCredentials : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            ulong credID = _mfsOperations.Credentials.New (appUrl, username, accessKey);
            Assert.IsTrue (credID > 0, "Credentials not added successfully.");

            _mfsOperations.Credentials.Delete (credID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AppUrlNull_Illegal () {
            string nullAppUrl = null;
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            _mfsOperations.Credentials.New (nullAppUrl, username, accessKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AppUrlEmpty_Illegal () {
            string emptyAppUrl = string.Empty;
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            _mfsOperations.Credentials.New (emptyAppUrl, username, accessKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UsernameNull_Illegal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string nullUsername = null;
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            _mfsOperations.Credentials.New (appUrl, nullUsername, accessKey);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UsernameEmpty_Illegal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string emptyUsername = string.Empty;
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            _mfsOperations.Credentials.New (appUrl, emptyUsername, accessKey);
        }

        [Test]
        public void Test_NotAllowedIfMasterPasswordEnforcedAndNotSet () {
        }

        [Test]
        public void Test_DuplicateAppUrl_Legal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username1 = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            // To ensure that the usernames are different, we use different size usernames:
            string username2 = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            ulong credID1 = _mfsOperations.Credentials.New (appUrl, username1, accessKey);
            ulong credID2 = _mfsOperations.Credentials.New (appUrl, username2, accessKey);

            Assert.IsTrue (credID1 > 0, "Did not create a legal credential record.");
            Assert.IsTrue (credID2 > 0, "Did not create a legal credential record.");
            Assert.AreNotEqual (credID1, credID2, "Created duplicate ids for credentials with same url.");

            _mfsOperations.Credentials.Delete (credID1);
            _mfsOperations.Credentials.Delete (credID2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_DuplicateAppUrl_And_DuplicateUsername_Illegal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            ulong credID = _mfsOperations.Credentials.New (appUrl, username, accessKey);

            try {
                _mfsOperations.Credentials.New (appUrl, username, accessKey);
            } finally {
                _mfsOperations.Credentials.Delete (credID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AccessKeyNull_Illegal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = null;

            _mfsOperations.Credentials.New (appUrl, username, accessKey);
        }

        [Test]
        public void Test_AccessKeyEmpty_Legal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = string.Empty;

            ulong credID = _mfsOperations.Credentials.New (appUrl, username, accessKey);
            Assert.IsTrue (credID > 0, "Credentials not added successfully.");

            _mfsOperations.Credentials.Delete (credID);
        }
    }

    [TestFixture]
    public class Tests_DoesCredentialExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            ulong credID = _mfsOperations.Credentials.New (appUrl, username, accessKey);

            bool credExists = _mfsOperations.Credentials.Exists (credID);
            Assert.IsTrue (credExists, "Credential was shown as not existing even though it does.");

            _mfsOperations.Credentials.Delete (credID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargeCredID = ulong.MaxValue;

            bool credExists = _mfsOperations.Credentials.Exists (veryLargeCredID);
            Assert.IsFalse (credExists, "Credential was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CredentialIDZero_Illegal () {
            _mfsOperations.Credentials.Exists (0);
        }

        [Test]
        public void Test_SanityCheck_WithAppUrl_And_Username_Exists () {
            string appUrl = TestUtils.GetAnyUrl ();
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string accessKey = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);

            ulong credID = _mfsOperations.Credentials.New (appUrl, username, accessKey);

            bool credExists = _mfsOperations.Credentials.Exists (appUrl, username);
            Assert.IsTrue (credExists, "Credential was shown as not existing even though it does.");

            _mfsOperations.Credentials.Delete (credID);
        }

        [Test]
        public void Test_SanityCheck_WithAppUrl_And_Username_NotExists () {
            string appUrl;
            string username;

            GetNonExistingAppUrl (ref _mfsOperations, out appUrl, out username);

            bool credExists = _mfsOperations.Credentials.Exists (appUrl, username);
            Assert.IsFalse (credExists, "Credential was shown as not existing even though it does.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AppUrlNull_Illegal () {
            string nullAppUrl = null;
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.Credentials.Exists (nullAppUrl, username);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AppUrlEmpty_Illegal () {
            string emptyAppUrl = string.Empty;
            string username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.Credentials.Exists (emptyAppUrl, username);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UsernameNull_Illegal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string nullUsername = null;

            _mfsOperations.Credentials.Exists (appUrl, nullUsername);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UsernameEmpty_Illegal () {
            string appUrl = TestUtils.GetAnyUrl ();
            string emptyUsername = string.Empty;

            _mfsOperations.Credentials.Exists (appUrl, emptyUsername);
        }
    }

    [TestFixture]
    public class Tests_GetAccessKey : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
        }
    }
}
