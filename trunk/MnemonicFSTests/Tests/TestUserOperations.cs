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
using MnemonicFS.MfsCore;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsUtils.MfsCrypto;

namespace MnemonicFS.Tests.UserOperations {
    [TestFixture]
    public class Tests_CreateMfsUser : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            ulong userID = MfsOperations.User.New (userIDStr, passwordHash);

            Assert.That (userID > 0, "User id returned was less than zero.");

            MfsOperations.User.Delete (userIDStr, true, true);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateUser_Illegal () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            try {
                MfsOperations.User.New (userIDStr, passwordHash);
            } finally {
                MfsOperations.User.Delete (userIDStr, true, true);
            }
        }

        [Test]
        public void Test_NewUserRegexCompliance () {
            // The client can get the regex compliance string as defined by the app admin:
            string regexComplianceString = MfsOperations.RegexString;

            string anyUserID = GetANonExistentUserID ();

            // Check to see if your user id complies with it:
            bool doesComply = MfsOperations.User.IsNameCompliant (anyUserID);
            Assert.IsTrue (doesComply, "User id was shown as non-compliant even though it is.");

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            // The system should accept this string as a valid user id:
            ulong userID = MfsOperations.User.New (anyUserID, passwordHash);
            Assert.That (userID > 0, "A valid user id was not returned.");

            MfsOperations.User.Delete (anyUserID, true, true);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NewUserRegexNonCompliance_Illegal () {
            string wrongFormatUserID = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            // Check to see if your user id conplies with it:
            bool doesComply = MfsOperations.User.IsNameCompliant (wrongFormatUserID);
            Assert.IsFalse (doesComply, "User id was shown as compliant even though it isn't.");

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            // The system should not accept this string as a valid user id:
            MfsOperations.User.New (wrongFormatUserID, passwordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDNull_Illegal () {
            string nullUserIDStr = null;
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (nullUserIDStr, passwordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyUserID_Illegal () {
            string emptyUserIDStr = string.Empty;
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (emptyUserIDStr, passwordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PasswordHashNull_Illegal () {
            string userIDStr = GetANonExistentUserID ();

            string passwordHash = null;

            MfsOperations.User.New (userIDStr, passwordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PasswordHashIncorrectSize_Illegal () {
            string userIDStr = GetANonExistentUserID ();

            MfsOperations.User.New (userIDStr, ILLEGAL_HASH);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPasswordHash_Illegal () {
            string userIDStr = GetANonExistentUserID ();

            string passwordHash = string.Empty;

            MfsOperations.User.New (userIDStr, passwordHash);
        }
    }

    [TestFixture]
    public class Tests_GetMfsUser : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            MfsOperations mfsOperations = new MfsOperations (userIDStr, passwordHash);
            Assert.IsNotNull (mfsOperations, "MfsOperations object was shown as null even though it isn't.");

            MfsOperations.User.Delete (userIDStr, true, true);
        }
    }

    [TestFixture]
    public class Tests_GetMfsUser_PasswordTests : TestMfsOperationsBase {
        [Test]
        public void Test_PasswordCorrect () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            MfsOperations mfsOperations = new MfsOperations (userIDStr, passwordHash);
            Assert.IsNotNull (mfsOperations, "MfsOperations object was not created even though password was correct.");

            MfsOperations.User.Delete (userIDStr, true, true);
        }

        [Test]
        [ExpectedException (typeof (MfsAuthenticationException))]
        public void Test_PasswordIncorrect () {
            string userIDStr = GetANonExistentUserID ();

            // First create the user:
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);
            MfsOperations.User.New (userIDStr, passwordHash);

            string wrongPassword = Hasher.GetSHA1 (password + password);
            string wrongPasswordHash = Hasher.GetSHA1 (wrongPassword);

            try {
                new MfsOperations (userIDStr, wrongPasswordHash);
            } finally {
                MfsOperations.User.Delete (userIDStr, true, true);
            }
        }
    }

    [TestFixture]
    public class Tests_GetMfsUsers : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numUsersToCreate = 3;
            List<string> userIDStrs = new List<string> ();

            for (int i = 0; i < numUsersToCreate; ++i) {
                string userIDStr = null;

                userIDStr = GetANonExistentUserID ();

                string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                string passwordHash = Hasher.GetSHA1 (password);

                MfsOperations.User.New (userIDStr, passwordHash);
                userIDStrs.Add (userIDStr);
            }

            List<string> users = MfsOperations.User.All ();
            // Why users.Count - 1? Because the SetUp() method also creates a user.
            // We do not want to count that here.
            Assert.AreEqual (userIDStrs.Count, users.Count - 1, "User count not as expected.");

            foreach (string userIDStr in userIDStrs) {
                int index = users.IndexOf (userIDStr);
                if (index < 0) {
                    Assert.Fail ("User added was not present in returned collection.");
                }
            }

            foreach (string userIDStr in userIDStrs) {
                if (userIDStr.Equals (_userID)) {
                    // Let the test fixture delete its own user:
                    continue;
                }

                MfsOperations.User.Delete (userIDStr, true, true);
            }
        }
    }

    [TestFixture]
    public class Tests_DoesUserExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            bool doesUserExist = MfsOperations.User.Exists (userIDStr);
            Assert.IsTrue (doesUserExist, "Shows user as not existing even though user does.");

            MfsOperations.User.Delete (userIDStr, true, true);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            string userIDStr = GetANonExistentUserID ();

            bool doesUserExist = MfsOperations.User.Exists (userIDStr);
            Assert.IsFalse (doesUserExist, "Shows user as existing even though user does not.");
        }
    }

    [TestFixture]
    public class Tests_DeleteMfsUser : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            int numDeleted = MfsOperations.User.Delete (userIDStr, true, true);
            Assert.AreEqual (1, numDeleted, "Did not delete user as expected.");
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentUserException))]
        public void Test_SanityCheck_NotExists () {
            string userIDStr = GetANonExistentUserID ();

            MfsOperations.User.Delete (userIDStr, true, true);
        }

        [Test]
        public void Test_DeletionOfOneUserShouldNotAffectAnother () {
            // First create a large number of users; this value is as specified by
            // the global variable, LARGE_NUMBER_OF_USERS. Hopefully, this should result
            // in a collision in the top-level directories.
            // Bear in mind that there is one extra user that has been created by SetUp();
            // we therefore specify the list capacity to be one extra.
            List<string> userIDs = new List<string> (LARGE_NUMBER_OF_USERS + 1);

            // Create as many users:
            for (int i = 0; i < LARGE_NUMBER_OF_USERS; ++i) {
                string userIDStr = GetANonExistentUserID ();

                string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                string passwordHash = Hasher.GetSHA1 (password);

                MfsOperations.User.New (userIDStr, passwordHash);
                userIDs.Add (userIDStr);
            }

            // Next, get a list of all users in the system:
            List<string> returnedMfsUsers = MfsOperations.User.All ();
            int numUsers = returnedMfsUsers.Count;

            // The first user in the list is the one created by SetUp(). But just to be safe, we check to see
            // if this is so, though, strictly speaking, this assert is not within the domain of this test:
            Assert.AreEqual (_userID, returnedMfsUsers[0], "First user is not the one created by SetUp().");

            // We also assert to check the total number of users returned:
            Assert.AreEqual (LARGE_NUMBER_OF_USERS + 1, numUsers, "Number of users is not as expected.");

            // Now start deleting each user one-by-one. Deletion of one user should not affect another user
            // (which is what we are testing), and the user count returned should be as expected. We start by
            // deleting the *last* user first, because recall that the first user is the one created by SetUp():
            int count = returnedMfsUsers.Count;
            while (numUsers > 1) {
                string userID = returnedMfsUsers[--count];

                if (userID.Equals (_userID)) {
                    // Just to be safe:
                    continue;
                }

                MfsOperations.User.Delete (userID, true, true);
                numUsers = MfsOperations.User.GetCount ();
                Assert.AreEqual (count, numUsers, "Number of users is not the as expected.");
            }
        }
    }

    [TestFixture]
    public class Tests_UpdateMfsUserPassword : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            MfsOperations mfsOperations = new MfsOperations (userIDStr, passwordHash);
            Assert.IsNotNull (mfsOperations);

            // Update user password; just get a hash of last password's hash:
            string newPasswordHash = Hasher.GetSHA1 (passwordHash);
            MfsOperations.User.UpdatePassword (userIDStr, newPasswordHash);

            // Try creating an object with the old password:
            try {
                new MfsOperations (userIDStr, passwordHash);
                Assert.Fail ("User was authenticated with wrong password.");
            } catch (MfsAuthenticationException) {
            }

            // Try opening with correct password:
            new MfsOperations (userIDStr, newPasswordHash);

            MfsOperations.User.Delete (userIDStr, true, true);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentUserException))]
        public void Test_NonExistentUser_Illegal () {
            string userIDStr = GetANonExistentUserID ();
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.UpdatePassword (userIDStr, passwordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullUserString_Illegal () {
            string nullUserIDStr = null;
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.UpdatePassword (nullUserIDStr, passwordHash);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_IncorrectSizePasswordHashString_Illegal () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            string wrongPasswordHash = "NOT_A_LEGAL_HASH";

            try {
                MfsOperations.User.UpdatePassword (userIDStr, wrongPasswordHash);
            } finally {
                MfsOperations.User.Delete (userIDStr, true, true);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPasswordHashString_Illegal () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            string nullPasswordHash = null;

            try {
                MfsOperations.User.UpdatePassword (userIDStr, nullPasswordHash);
            } finally {
                MfsOperations.User.Delete (userIDStr, true, true);
            }
        }
    }

    [TestFixture]
    public class Tests_GetMfsUserName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string userIDStr = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (userIDStr, passwordHash);

            MfsOperations mfsOperations = new MfsOperations (userIDStr, passwordHash);

            string fName, lName;
            mfsOperations.GetUserName (out fName, out lName);

            Assert.IsEmpty (fName);
            Assert.IsEmpty (lName);

            MfsOperations.User.Delete (userIDStr, true, true);
        }
    }

    [TestFixture]
    public class Tests_UpdateMfsUserName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // TODO
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserFNameNull_Illegal () {
            // TODO
            throw new MfsIllegalArgumentException ();
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserLNameNull_Illegal () {
            // TODO
            throw new MfsIllegalArgumentException ();
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserFNameLargerThanAllowedSize_Illegal () {
            throw new MfsIllegalArgumentException ();
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserLNameLargerThanAllowedSize_Illegal () {
            // TODO
            throw new MfsIllegalArgumentException ();
        }
    }
}
