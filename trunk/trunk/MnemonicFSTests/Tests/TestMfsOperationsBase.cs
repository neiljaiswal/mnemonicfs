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
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsUtils.MfsCrypto;
using System.Threading;

namespace MnemonicFS.Tests.Base {
    public enum FileSize {
        ZERO_FILE_SIZE = 0,
        SMALL_FILE_SIZE = 100,
        MEDIUM_FILE_SIZE = 500,
        LARGE_FILE_SIZE = 1000
    }

    public class TestMfsOperationsBase {
        #region << Typical Values >>

        protected const int TYPICAL_WORD_SIZE = 5; // characters per word;
        protected const int TYPICAL_SENTENCE_SIZE = 5; // words per sentence;

        protected const int TYPICAL_PASSWORD_SIZE = 5; // characters per password string

        protected const int TYPICAL_TELEPHONE_NUMBER_SIZE = 12; // digits;

        protected const int TYPICAL_MULTI_VALUE = 5; // units;

        protected const int TYPICAL_MULTI_TIME_UNIT = 5; // units of time;

        protected const string SINGLE_CHAR_STR = "a";

        protected const string ILLEGAL_HASH = "SHORT_ILLEGAL_HASH";

        protected const int SINGLE_VALUE = 1;
        protected const int LARGE_NUMBER_OF_USERS = 70;

        protected const int NUM_MS_TO_SLEEP = 1; // milli-seconds

        protected const string FILE_SYSTEM_LOCATION = @"C:\";

        protected const string PDF_FILE_EXTENSION = "pdf";
        protected const string MSDOC_FILE_EXTENSION = "doc";
        protected const string MSEXCEL_FILE_EXTENSION = "xls";
        protected const string ARCHIVE_EXTENSION = ".zip";

        #endregion << Typical Values >>

        #region << Instance data used across a test suite >>

        protected MfsOperations _mfsOperations;
        protected string _userID;

        protected string _fileName;
        protected byte[] _fileData;
        protected string _fileNarration;

        protected string _aspectName;
        protected string _aspectDesc;

        protected string _briefcaseName;
        protected string _briefcaseDesc;

        protected string _collectionName;
        protected string _collectionDesc;

        protected string _schemaFreeDocName;

        protected string _predicate;

        protected delegate bool MethodCreateUserBackupArchive (string userID, string backupFileNameWithPath);

        #endregion << Instance data used across a test suite >>

        [SetUp]
        protected void SetUp () {
            // We do *not* initialize _fileData here, since we leave
            // it to each test to decide what file size it needs.

            _fileName = TestUtils.GetAnyFileName ();
            _fileNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            do {
                _aspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                _aspectDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Aspect.Exists (_aspectName));

            do {
                _briefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                _briefcaseDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Briefcase.Exists (_briefcaseName));

            do {
                _collectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                _collectionDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Collection.Exists (_collectionName));

            do {
                _schemaFreeDocName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Sfd.Exists (_schemaFreeDocName));

            do {
                _predicate = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Relation.DoesPredicateExist (_predicate));
        }

        [TearDown]
        protected void TearDown () {
            _fileData = null;

            _fileName = null;
            _fileNarration = null;

            _aspectName = null;
            _aspectDesc = null;

            _briefcaseName = null;
            _briefcaseDesc = null;

            _collectionName = null;
            _collectionDesc = null;

            _schemaFreeDocName = null;

            _predicate = null;
        }

        [TestFixtureSetUp]
        protected void TestFixtureSetUp () {
            _userID = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_PASSWORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.User.New (_userID, passwordHash);

            _mfsOperations = new MfsOperations (_userID, passwordHash);
        }

        [TestFixtureTearDown]
        protected void TestFixtureTearDown () {
            MfsOperations.Log.DeleteUserFileLogs (_userID);
            MfsOperations.User.Delete (_userID, true, true);

            _mfsOperations.Dispose ();
            _mfsOperations = null;
        }

        protected static ulong CreateUniqueAspect (ref MfsOperations mfsOperations, out string aspectName, out string aspectDesc) {
            do {
                aspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                aspectDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (mfsOperations.Aspect.Exists (aspectName));

            return mfsOperations.Aspect.New (aspectName, aspectDesc);
        }

        protected static ulong CreateUniqueBriefcase (ref MfsOperations mfsOperations, out string briefcaseName, out string briefcaseDesc) {
            do {
                briefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                briefcaseDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (mfsOperations.Briefcase.Exists (briefcaseName));

            return mfsOperations.Briefcase.New (briefcaseName, briefcaseDesc);
        }

        protected static ulong CreateUniqueCollection (ref MfsOperations mfsOperations, out string collectionName, out string collectionDesc) {
            do {
                collectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                collectionDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (mfsOperations.Collection.Exists (collectionName));

            return mfsOperations.Collection.New (collectionName, collectionDesc);
        }

        protected static ulong CreateUniqueSchemaFreeDocument (ref MfsOperations mfsOperations, out string schemaFreeDocName, DateTime when) {
            do {
                schemaFreeDocName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (mfsOperations.Sfd.Exists (schemaFreeDocName));

            return mfsOperations.Sfd.New (schemaFreeDocName, when);
        }

        protected static ulong CreateUniquePredicate (ref MfsOperations mfsOperations, out string predicate) {
            do {
                predicate = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (mfsOperations.Relation.DoesPredicateExist (predicate));

            return mfsOperations.Relation.NewPredicate (predicate);
        }

        protected static List<ulong> CreateUniqueNAspects (ref MfsOperations mfsOperations, int numAspects) {
            List<ulong> aspectList = new List<ulong> (numAspects);

            for (int i = 0; i < numAspects; ++i) {
                string aspectName;
                string aspectDesc;
                aspectList.Add (CreateUniqueAspect (ref mfsOperations, out aspectName, out aspectDesc));
            }

            return aspectList;
        }

        protected static List<ulong> CreateUniqueNBriefcases (ref MfsOperations mfsOperations, int numBriefcases) {
            List<ulong> briefcaseList = new List<ulong> ();

            for (int i = 0; i < numBriefcases; ++i) {
                string briefcaseName;
                string briefcaseDesc;
                briefcaseList.Add (CreateUniqueBriefcase (ref mfsOperations, out briefcaseName, out briefcaseDesc));
            }

            return briefcaseList;
        }

        protected static List<ulong> CreateUniqueNCollections (ref MfsOperations mfsOperations, int numCollections) {
            List<ulong> collectionList = new List<ulong> ();

            for (int i = 0; i < numCollections; ++i) {
                string collectionName;
                string collectionDesc;
                collectionList.Add (CreateUniqueCollection (ref mfsOperations, out collectionName, out collectionDesc));
            }

            return collectionList;
        }

        protected static List<ulong> CreateUniqueNSchemaFreeDocuments (ref MfsOperations mfsOperations, int numSchemaFreeDocuments, DateTime when) {
            List<ulong> schemaFreeDocumentList = new List<ulong> ();

            for (int i = 0; i < numSchemaFreeDocuments; ++i) {
                string schemaFreeDocumentName;
                schemaFreeDocumentList.Add (CreateUniqueSchemaFreeDocument (ref mfsOperations, out schemaFreeDocumentName, when));
            }

            return schemaFreeDocumentList;
        }

        protected static List<ulong> CreateUniqueNPredicates (ref MfsOperations mfsOperations, int numPredicates) {
            List<ulong> predicatesList = new List<ulong> ();

            for (int i = 0; i < numPredicates; ++i) {
                string predicate;
                predicatesList.Add (CreateUniquePredicate (ref mfsOperations, out predicate));
            }

            return predicatesList;
        }

        protected static MfsVCard CreateVCard (ref MfsOperations mfsOperations) {
            MfsVCard vCard = new MfsVCard ();
            return vCard;
        }

        protected static ulong CreateNote (ref MfsOperations mfsOperations) {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            return mfsOperations.Note.New (note);
        }

        protected static ulong CreateUrl (ref MfsOperations mfsOperations) {
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            return mfsOperations.Url.New (url, description, when);
        }

        protected static string GetANonExistentUserID () {
            string userID = null;

            do {
                userID = TestUtils.GetAnyEmailID ();
            } while (MfsOperations.User.Exists (userID));

            return userID;
        }

        protected static void GetNonExistingAppUrl (ref MfsOperations mfsOperations, out string appUrl, out string username) {
            username = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            do {
                appUrl = TestUtils.GetAnyUrl ();
            } while (mfsOperations.Credentials.Exists (appUrl, username));
        }

        private delegate ulong MethodSaveFile (string fileName, string fileNarration, byte[] fileData, DateTime when, bool indexFile);

        protected static ulong SaveFileToMfs (ref MfsOperations mfsOps, string fileName, string fileNarration, byte[] fileData, DateTime when, bool indexFile) {
            MethodSaveFile method = mfsOps.File.New;
            IAsyncResult res = method.BeginInvoke (fileName, fileNarration, fileData, when, indexFile, null, null);
            // [delegate].EndInvoke (IAsyncResult) is blocking:
            ulong fileID = method.EndInvoke (res);

            return fileID;
        }
    }
}
