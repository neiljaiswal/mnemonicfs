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
using MnemonicFS.MfsUtils.MfsCrypto;

namespace MnemonicFS.Tests.MasterPassword {
    [TestFixture]
    public class Tests_IsMasterPasswordSet : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_NotSet () {
            string userIDStr = GetANonExistentUserID ();
            string password = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            ulong userID = MfsOperations.User.New (userIDStr, passwordHash);

            bool hasMasterPassword = _mfsOperations.MasterPassword.IsSet ();

            Assert.IsFalse (hasMasterPassword, "Newly created user should not have a master password.");

            MfsOperations.User.Delete (userIDStr, true, true);
        }

        [Test]
        public void Test_SanityCheck_Set () {
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            _mfsOperations.MasterPassword.Set (passwordHash);

            bool hasMasterPassword = _mfsOperations.MasterPassword.IsSet ();
            Assert.IsTrue (hasMasterPassword, "Master passsword not set.");

            _mfsOperations.MasterPassword.Reset ();
        }
    }

    [TestFixture]
    public class Tests_SetMasterPassword : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            _mfsOperations.MasterPassword.Set (passwordHash);
            bool hasMasterPassword = _mfsOperations.MasterPassword.IsSet ();

            Assert.IsTrue (hasMasterPassword, "Master password was not set successfully.");

            _mfsOperations.MasterPassword.Reset ();
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PasswordHashNull_Illegal () {
            string nullPasswordHash = null;

            _mfsOperations.MasterPassword.Set (nullPasswordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PasswordHashIncorrectSize_Illegal () {
            _mfsOperations.MasterPassword.Set (ILLEGAL_HASH);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPasswordHash_Illegal () {
            string emptyPasswordHash = string.Empty;

            _mfsOperations.MasterPassword.Set (emptyPasswordHash);
        }
    }

    [TestFixture]
    public class Tests_ResetMasterPassword : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            _mfsOperations.MasterPassword.Set (passwordHash);

            string retrMasterPasswordHash = _mfsOperations.MasterPassword.GetHash ();
            Assert.AreEqual (passwordHash, retrMasterPasswordHash, "Retrieved master password hash is not as expected.");

            _mfsOperations.MasterPassword.Reset ();

            retrMasterPasswordHash = _mfsOperations.MasterPassword.GetHash ();
            Assert.IsNull (retrMasterPasswordHash, "Master password reset did not reset it to null.");
        }
    }

    [TestFixture]
    public class Tests_GetMasterPasswordHash : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            _mfsOperations.MasterPassword.Set (passwordHash);

            string retrMasterPasswordHash = _mfsOperations.MasterPassword.GetHash ();
            Assert.AreEqual (passwordHash, retrMasterPasswordHash, "Retrieved master password hash is not as expected.");

            _mfsOperations.MasterPassword.Reset ();
        }

        [Test]
        public void Test_ReturnNullWhenNotSet () {
            string masterPasswordHash = _mfsOperations.MasterPassword.GetHash ();
            Assert.IsNull (masterPasswordHash, "Master password of new user not null.");
        }
    }

    [TestFixture]
    public class Tests_AuthenticateMasterPasswordHash : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            _mfsOperations.MasterPassword.Set (passwordHash);

            bool authenticated = _mfsOperations.MasterPassword.Authenticate (passwordHash);
            Assert.IsTrue (authenticated, "Did not authenticate master password even when correct.");

            _mfsOperations.MasterPassword.Reset ();
        }

        [Test]
        public void Test_SanityCheck_WrongPasswordHash () {
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            _mfsOperations.MasterPassword.Set (passwordHash);

            string wrongPasswordHash = Hasher.GetSHA1 (passwordHash);
            bool authenticated = _mfsOperations.MasterPassword.Authenticate (wrongPasswordHash);
            Assert.IsFalse (authenticated, "Authenticated wrong master password.");

            _mfsOperations.MasterPassword.Reset ();
        }
        
        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PasswordHashNull_Illegal () {
            string nullPasswordHash = null;

            _mfsOperations.MasterPassword.Authenticate (nullPasswordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PasswordHashIncorrectSize_Illegal () {
            _mfsOperations.MasterPassword.Authenticate (ILLEGAL_HASH);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPasswordHash () {
            string emptyPasswordHash = string.Empty;

            _mfsOperations.MasterPassword.Authenticate (emptyPasswordHash);
        }
    }
}
