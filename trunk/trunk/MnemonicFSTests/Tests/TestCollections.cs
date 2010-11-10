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
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Base;

namespace MnemonicFS.Tests.Collections {
    [TestFixture]
    public class Tests_CollectionsMethod_CreateCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            Assert.That (collectionID > 0, "Collection not created successfully: Invalid collection id returned.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateCollectionName_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            try {
                _mfsOperations.CreateCollection (_collectionName, _collectionDesc);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        public void Test_CollectionNameWithMaxSizeAllowed_SanityCheck () {
            string collectionName = TestUtils.GetAWord (MfsOperations.MaxCollectionNameLength);

            ulong collectionID = _mfsOperations.CreateCollection (collectionName, _collectionDesc);

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        public void Test_CollectionDescWithMaxSizeAllowed_SanityCheck () {
            string collectionDesc = TestUtils.GetAWord (MfsOperations.MaxCollectionNameLength);

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, collectionDesc);

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullCollectionName_Illegal () {
            string nullCollectionName = null;

            _mfsOperations.CreateCollection (nullCollectionName, _collectionDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullCollectionDesc_Illegal () {
            string nullCollectionDesc = null;

            _mfsOperations.CreateCollection (_collectionName, nullCollectionDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyCollectionName_Illegal () {
            string emptyCollectionName = string.Empty;

            _mfsOperations.CreateCollection (emptyCollectionName, _collectionDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionNameGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInCollectionName = MfsOperations.MaxCollectionNameLength;

            string longCollectionName = TestUtils.GetAWord (maxCharsInCollectionName + 1);

            _mfsOperations.CreateCollection (longCollectionName, _collectionDesc);
        }

        [Test]
        public void Test_EmptyCollectionDesc_Legal () {
            string emptyCollectionDesc = string.Empty;

            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, emptyCollectionDesc);

            Assert.That (collectionID > 0, "Failed to save collection though empty collection description is allowed.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionDescGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInCollectionDesc = MfsOperations.MaxCollectionDescLength;

            string longCollectionDesc = TestUtils.GetAWord (maxCharsInCollectionDesc + 1);

            _mfsOperations.CreateCollection (_collectionName, longCollectionDesc);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_DoesCollectionExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            bool collectionExists = _mfsOperations.DoesCollectionExist (collectionID);

            Assert.IsTrue (collectionExists, "Collection was shown as not existing, even though it does.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargeCollectionID = ulong.MaxValue;

            bool collectionExists = _mfsOperations.DoesCollectionExist (veryLargeCollectionID);

            Assert.IsFalse (collectionExists, "Collection was shown as existing, even though it does not.");
        }

        [Test]
        public void Test_SanityCheck_CollectionName_Exists () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            bool collectionExists = _mfsOperations.DoesCollectionExist (_collectionName);

            Assert.IsTrue (collectionExists, "Collection was shown as not existing, even though it does.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        public void Test_SanityCheck_CollectionName_NotExists () {
            string someCollectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);

            bool collectionExists = _mfsOperations.DoesCollectionExist (someCollectionName);

            Assert.IsFalse (collectionExists, "Collection was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _mfsOperations.DoesCollectionExist (0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionNameNull_Illegal () {
            _mfsOperations.DoesCollectionExist (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionNameSizeZero_Illegal () {
            _mfsOperations.DoesCollectionExist (string.Empty);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_DeleteCollection : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            int numCollectionsDeleted = _mfsOperations.DeleteCollection (collectionID);

            Assert.AreEqual (1, numCollectionsDeleted, "Collection was not deleted.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            _mfsOperations.DeleteCollection (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.DeleteCollection (veryLargeCollectionID);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetCollectionName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (collectionID, out collectionName, out collectionDesc);

            Assert.AreEqual (_collectionName, collectionName, "Did not retrieve collection's name.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (0, out collectionName, out collectionDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (veryLargeCollectionID, out collectionName, out collectionDesc);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetCollectionDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (collectionID, out collectionName, out collectionDesc);

            Assert.AreEqual (_collectionDesc, collectionDesc, "Did not retrieve collection's description.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (0, out collectionName, out collectionDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (veryLargeCollectionID, out collectionName, out collectionDesc);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetCollectionIDFromName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = CreateUniqueCollection (ref _mfsOperations, out _collectionName, out _collectionDesc);

            ulong retrCollectionID = _mfsOperations.GetCollectionIDFromName (_collectionName);
            Assert.AreEqual (collectionID, retrCollectionID, "Wrong collection ID retrieved.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionName_Illegal () {
            string nonExistentCollectionName = null;

            do {
                nonExistentCollectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesCollectionExist (nonExistentCollectionName));

            _mfsOperations.GetCollectionIDFromName (nonExistentCollectionName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullCollectionName_Illegal () {
            string nullCollectionName = null;

            _mfsOperations.GetCollectionIDFromName (nullCollectionName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyCollectionName_Illegal () {
            string emptyCollectionName = string.Empty;

            _mfsOperations.GetCollectionIDFromName (emptyCollectionName);
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_GetAllCollections : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> collectionsList = new List<ulong> ();

            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string collectionName, collectionDesc;
                ulong collectionID = CreateUniqueCollection (ref _mfsOperations, out collectionName, out collectionDesc);
                collectionsList.Add (collectionID);
            }

            List<ulong> retrCollectionIDs = _mfsOperations.GetAllCollections ();

            collectionsList.Sort ();
            retrCollectionIDs.Sort ();

            Assert.AreEqual (collectionsList.Count, retrCollectionIDs.Count, "Wrong number of collection ids retrieved.");
            Assert.AreEqual (collectionsList, retrCollectionIDs, "Wrong collection ids recovered.");

            foreach (ulong collectionID in collectionsList) {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_UpdateCollectionName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string newCollectionName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            bool collectionNameUpdated = _mfsOperations.UpdateCollectionName (collectionID, newCollectionName);

            Assert.IsTrue (collectionNameUpdated, "Collection name was not updated successfully.");

            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (collectionID, out collectionName, out collectionDesc);

            Assert.AreEqual (newCollectionName, collectionName, "Collection name was not updated successfully.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        public void Test_CollectionNameWithMaxSizeAllowed_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string newCollectionName = TestUtils.GetAWord (MfsOperations.MaxCollectionNameLength);

            _mfsOperations.UpdateCollectionName (collectionID, newCollectionName);

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateCollectionName (0, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateCollectionName (veryLargeCollectionID, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewCollectionName_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string nullCollectionName = null;

            try {
                _mfsOperations.UpdateCollectionName (collectionID, nullCollectionName);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewCollectionName_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string emptyCollectionName = string.Empty;

            try {
                _mfsOperations.UpdateCollectionName (collectionID, emptyCollectionName);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionNameGreaterThanSystemDefined_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string veryLongCollectionName = TestUtils.GetAWord (MfsOperations.MaxCollectionNameLength + 1);

            try {
                _mfsOperations.UpdateCollectionName (collectionID, veryLongCollectionName);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_UpdateCollectionDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string newCollectionDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            bool collectionDescUpdated = _mfsOperations.UpdateCollectionDesc (collectionID, newCollectionDesc);

            Assert.IsTrue (collectionDescUpdated, "Collection description was not updated successfully.");

            string collectionName, collectionDesc;
            _mfsOperations.GetCollectionNameAndDesc (collectionID, out collectionName, out collectionDesc);

            Assert.AreEqual (newCollectionDesc, collectionDesc, "Collection description was not updated successfully.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        public void Test_CollectionDescWithMaxSizeAllowed_SanityCheck () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string newCollectionDesc = TestUtils.GetAWord (MfsOperations.MaxCollectionDescLength);

            _mfsOperations.UpdateCollectionDesc (collectionID, newCollectionDesc);

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionIDZero_Illegal () {
            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateCollectionDesc (0, anyDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollectionID_Illegal () {
            ulong veryLargeCollectionID = UInt64.MaxValue;

            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateCollectionDesc (veryLargeCollectionID, anyDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewCollectionDesc_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string nullCollectionDesc = null;

            try {
                _mfsOperations.UpdateCollectionDesc (collectionID, nullCollectionDesc);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }

        [Test]
        public void Test_EmptyNewCollectionDesc_Legal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string emptyCollectionDesc = string.Empty;

            _mfsOperations.UpdateCollectionDesc (collectionID, emptyCollectionDesc);

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_CollectionDescGreaterThanSystemDefined_Illegal () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string veryLongCollectionDesc = TestUtils.GetAWord (MfsOperations.MaxCollectionDescLength + 1);

            try {
                _mfsOperations.UpdateCollectionDesc (collectionID, veryLongCollectionDesc);
            } finally {
                _mfsOperations.DeleteCollection (collectionID);
            }
        }
    }

    [TestFixture]
    public class Tests_CollectionsMethod_DeleteAllCollections : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numCollectionsToCreate = TYPICAL_MULTI_VALUE;

            List<ulong> listCollections = CreateUniqueNCollections (ref _mfsOperations, numCollectionsToCreate);

            int numCollectionsDeleted = _mfsOperations.DeleteAllCollectionsInSystem ();
            Assert.AreEqual (listCollections.Count, numCollectionsDeleted, "Did not delete the same number of collections as were created.");

            List<ulong> collectionsList = _mfsOperations.GetAllCollections ();
            Assert.AreEqual (0, collectionsList.Count, "Did not delete all collections in the system.");
        }
    }
}
