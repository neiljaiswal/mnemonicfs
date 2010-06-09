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

namespace MnemonicFS.Tests.Base {
    public enum FileSize {
        ZERO_FILE_SIZE = 0,
        SMALL_FILE_SIZE = 100,
        MEDIUM_FILE_SIZE = 500,
        LARGE_FILE_SIZE = 1000
    }

    public class TestMfsOperationsBase {
        protected const int TYPICAL_WORD_SIZE = 5; // characters per word;
        protected const int TYPICAL_SENTENCE_SIZE = 5; // words per sentence;

        protected const int TYPICAL_TELEPHONE_NUMBER_SIZE = 12; // digits;

        protected const int TYPICAL_MULTI_VALUE = 5; // units;

        protected const int TYPICAL_MULTI_TIME_UNIT = 5; // units of time;

        protected const string SINGLE_CHAR_STR = "a";

        protected const int SINGLE_VALUE = 1;
        protected const int LARGE_NUMBER_OF_USERS = 70;

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

        [SetUp]
        protected void SetUp () {
            // We do *not* initialize _fileData here, since we leave
            // it to each test to decide what file size it needs.

            _fileName = TestUtils.GetAnyFileName ();
            _fileNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            do {
                _aspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                _aspectDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesAspectExist (_aspectName));

            do {
                _briefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                _briefcaseDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesBriefcaseExist (_briefcaseName));

            do {
                _collectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                _collectionDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesCollectionExist (_collectionName));
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
        }

        [TestFixtureSetUp]
        protected void TestFixtureSetUp () {
            _userID = GetANonExistentUserID ();

            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string passwordHash = Hasher.GetSHA1 (password);

            MfsOperations.CreateNewUser (_userID, passwordHash);

            _mfsOperations = new MfsOperations (_userID, passwordHash);
        }

        [TestFixtureTearDown]
        protected void TestFixtureTearDown () {
            MfsOperations.DeleteUser (_userID, true, true);

            _mfsOperations = null;
        }

        protected static ulong CreateUniqueAspect (ref MfsOperations mfsOperations, out string aspectName, out string aspectDesc) {
            ulong aspectID = 0;
            
            do {
                aspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                aspectDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (mfsOperations.DoesAspectExist (aspectName));

            aspectID = mfsOperations.CreateAspect (aspectName, aspectDesc);

            return aspectID;
        }

        protected static ulong CreateUniqueBriefcase (ref MfsOperations mfsOperations, out string briefcaseName, out string briefcaseDesc) {
            ulong briefcaseID = 0;
            
            do {
                briefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                briefcaseDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (mfsOperations.DoesBriefcaseExist (briefcaseName));

            briefcaseID = mfsOperations.CreateBriefcase (briefcaseName, briefcaseDesc);

            return briefcaseID;
        }

        protected static ulong CreateUniqueCollection (ref MfsOperations mfsOperations, out string collectionName, out string collectionDesc) {
            ulong collectionID = 0;
            
            do {
                collectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
                collectionDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            } while (mfsOperations.DoesCollectionExist (collectionName));

            collectionID = mfsOperations.CreateCollection (collectionName, collectionDesc);

            return collectionID;
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

        protected static MfsVCard CreateVCard () {
            MfsVCard vCard = new MfsVCard ();
            return vCard;
        }

        protected static string GetANonExistentUserID () {
            string userID = null;

            do {
                userID = TestUtils.GetAnyEmailID ();
            } while (MfsOperations.DoesUserExist (userID));

            return userID;
        }
    }
}
