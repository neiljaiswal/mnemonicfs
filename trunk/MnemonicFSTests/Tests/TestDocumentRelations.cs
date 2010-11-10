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

namespace MnemonicFS.Tests.Relations {
    [TestFixture]
    public class TestRelations_CreatePredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);
            Assert.That (predicateID > 0, "Predicate ID is not valid.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicatePredicate_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.CreatePredicate (_predicate);
            } finally {
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        public void Test_PredicateWithMaxSizeAllowed_SanityCheck () {
            string predicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength);

            ulong predicateID = _mfsOperations.CreatePredicate (predicate);

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateGreaterThanMaxSizeAllowed_Illegal () {
            string predicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength + 1);

            _mfsOperations.CreatePredicate (predicate);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPredicate_Illegal () {
            string nullPredicate = null;

            _mfsOperations.CreatePredicate (nullPredicate);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPredicate_Illegal () {
            string emptyPredicate = string.Empty;

            _mfsOperations.CreatePredicate (emptyPredicate);
        }
    }

    [TestFixture]
    public class TestRelations_DoesPredicateExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            bool predicateExists = _mfsOperations.DoesPredicateExist (predicateID);
            Assert.IsTrue (predicateExists, "Predicate was shown as not existing, even though it does.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargePredicateID = ulong.MaxValue;

            bool predicateExists = _mfsOperations.DoesPredicateExist (veryLargePredicateID);
            Assert.IsFalse (predicateExists, "Predicate was shown as existing, even though it does not.");
        }

        [Test]
        public void Test_SanityCheck_WithPredicate_Exists () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            bool predicateExists = _mfsOperations.DoesPredicateExist (_predicate);
            Assert.IsTrue (predicateExists, "Predicate was shown as not existing, even though it does.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_WithPredicate_NotExists () {
            string somePredicateName = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);

            bool predicateExists = _mfsOperations.DoesPredicateExist (somePredicateName);
            ;
            Assert.IsFalse (predicateExists, "Predicate was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            _mfsOperations.DoesPredicateExist (0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateNull_Illegal () {
            _mfsOperations.DoesPredicateExist (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateSizeZero_Illegal () {
            _mfsOperations.DoesPredicateExist (string.Empty);
        }
    }

    [TestFixture]
    public class TestRelations_DeletePredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            int numPredicatesDeleted = _mfsOperations.DeletePredicate (predicateID);
            Assert.AreEqual (1, numPredicatesDeleted, "Predicate was not deleted.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            _mfsOperations.DeletePredicate (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            ulong veryLargePredicateID = UInt64.MaxValue;

            _mfsOperations.DeletePredicate (veryLargePredicateID);
        }
    }

    [TestFixture]
    public class TestRelations_GetPredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            string retrPredicate = _mfsOperations.GetPredicate (predicateID);
            Assert.AreEqual (_predicate, retrPredicate, "Did not retrieve existing predicate's name.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            _mfsOperations.GetPredicate (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            ulong veryLargePredicateID = UInt64.MaxValue;

            _mfsOperations.GetPredicate (veryLargePredicateID);
        }
    }

    [TestFixture]
    public class TestRelations_GetPredicateID : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            ulong retrPredicateID = _mfsOperations.GetPredicateID (_predicate);
            Assert.AreEqual (predicateID, retrPredicateID, "Wrong predicate ID retrieved.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateName_Illegal () {
            string nonExistentPredicateName = null;

            do {
                nonExistentPredicateName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesPredicateExist (nonExistentPredicateName));

            _mfsOperations.GetPredicateID (nonExistentPredicateName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPredicateName_Illegal () {
            string nullPredicateName = null;

            _mfsOperations.GetPredicateID (nullPredicateName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPredicateName_Illegal () {
            string emptyPredicateName = string.Empty;

            _mfsOperations.GetPredicateID (emptyPredicateName);
        }
    }

    [TestFixture]
    public class TestRelations_GetAllPredicates : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> predicatesList = new List<ulong> ();

            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string predicate;
                ulong predicateID = CreateUniquePredicate (ref _mfsOperations, out predicate);
                predicatesList.Add (predicateID);
            }

            List<ulong> retrPredicateIDs = _mfsOperations.GetAllPredicates ();

            predicatesList.Sort ();
            retrPredicateIDs.Sort ();

            Assert.AreEqual (predicatesList.Count, retrPredicateIDs.Count, "Wrong number of predicate ids retrieved.");
            Assert.AreEqual (predicatesList, retrPredicateIDs, "Wrong predicate ids recovered.");

            foreach (ulong predicateID in predicatesList) {
                _mfsOperations.DeletePredicate (predicateID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_UpdatePredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            string newPredicateName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            bool predicateUpdated = _mfsOperations.UpdatePredicate (predicateID, newPredicateName);
            Assert.IsTrue (predicateUpdated, "Predicate was not updated successfully.");

            string predicate = _mfsOperations.GetPredicate (predicateID);
            Assert.AreEqual (newPredicateName, predicate, "Predicate was not updated successfully.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_PredicateWithMaxSizeAllowed_SanityCheck () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            string newPredicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength);
            _mfsOperations.UpdatePredicate (predicateID, newPredicate);

            string predicate = _mfsOperations.GetPredicate (predicateID);
            Assert.AreEqual (newPredicate, predicate, "Wrong predicate returned after predicate updation.");

            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            string any = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdatePredicate (0, any);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            ulong veryLargePredicateID = UInt64.MaxValue;

            string any = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdatePredicate (veryLargePredicateID, any);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewPredicate_Illegal () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            string nullPredicateName = null;

            try {
                _mfsOperations.UpdatePredicate (predicateID, nullPredicateName);
            } finally {
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewPredicate_Illegal () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            string emptyPredicateName = string.Empty;

            try {
                _mfsOperations.UpdatePredicate (predicateID, emptyPredicateName);
            } finally {
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateGreaterThanSystemDefined_Illegal () {
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            string veryLongPredicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength + 1);

            try {
                _mfsOperations.UpdatePredicate (predicateID, veryLongPredicate);
            } finally {
                _mfsOperations.DeletePredicate (predicateID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_DeleteAllPredicates : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numPredicatesToCreate = TYPICAL_MULTI_VALUE;

            List<ulong> listPredicates = CreateUniqueNPredicates (ref _mfsOperations, numPredicatesToCreate);

            int numPredicatesDeleted = _mfsOperations.DeleteAllPredicates ();
            Assert.AreEqual (listPredicates.Count, numPredicatesDeleted, "Did not delete the same number of predicates as were created.");
        }
    }

    [TestFixture]
    public class TestRelations_CreateRelation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            // Save a note:
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            // And save a url:
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            // And also create a predicate:
            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            // Now create a relation between them:
            bool relationCreated = _mfsOperations.CreateRelation (noteID, urlID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_DocSelfRelation_Legal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            bool relationCreated = _mfsOperations.CreateRelation (noteID, noteID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.CreateRelation (veryLargeDocID, noteID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.CreateRelation (noteID, veryLargeDocID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong veryLargePredicateID = UInt64.MaxValue;

            try {
                _mfsOperations.CreateRelation (noteID, urlID, veryLargePredicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeleteDocument (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.CreateRelation (0, noteID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.CreateRelation (noteID, 0, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.CreateRelation (noteID, urlID, 0);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeleteDocument (urlID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_DoesRelationExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            _mfsOperations.CreateRelation (noteID, urlID, predicateID);

            bool doesRelationExist = _mfsOperations.DoesRelationExist (noteID, urlID);
            Assert.IsTrue (doesRelationExist, "Shows relation as not existing even though it does.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            bool doesRelationExist = _mfsOperations.DoesRelationExist (noteID, urlID);
            Assert.IsFalse (doesRelationExist, "Shows relation as existing even though it doesn't.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.DoesRelationExist (veryLargeDocID, noteID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            try {
                _mfsOperations.DoesRelationExist (noteID, veryLargeDocID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.DoesRelationExist (0, noteID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.DoesRelationExist (noteID, 0);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_DoesSpecificRelationExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            _mfsOperations.CreateRelation (noteID, urlID, predicateID);

            bool doesRelationExist = _mfsOperations.DoesSpecificRelationExist (noteID, urlID, predicateID);
            Assert.IsTrue (doesRelationExist, "Shows relation as not existing even though it does.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            bool doesRelationExist = _mfsOperations.DoesSpecificRelationExist (noteID, urlID, predicateID);
            Assert.IsFalse (doesRelationExist, "Shows relation as existing even though it doesn't.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.DoesSpecificRelationExist (veryLargeDocID, noteID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.DoesSpecificRelationExist (noteID, veryLargeDocID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong veryLargePredicateID = UInt64.MaxValue;

            try {
                _mfsOperations.DoesSpecificRelationExist (noteID, urlID, veryLargePredicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeleteDocument (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.DoesSpecificRelationExist (0, noteID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.DoesSpecificRelationExist (noteID, 0, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.DoesSpecificRelationExist (noteID, urlID, 0);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeleteDocument (urlID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_GetRelations : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            List<ulong> predicates = new List<ulong> ();
            char ch = 'a';
            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string predicate = _predicate + ch++;
                ulong predicateID = _mfsOperations.CreatePredicate (predicate);
                _mfsOperations.CreateRelation (noteID, urlID, predicateID);
                predicates.Add (predicateID);
            }

            List<ulong> relationsList = _mfsOperations.GetRelations (noteID, urlID);
            Assert.AreEqual (predicates.Count, relationsList.Count, "Shows incorrect number of relations.");

            predicates.Sort ();
            relationsList.Sort ();
            Assert.AreEqual (predicates, relationsList, "Wrong relations returned.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            foreach (ulong predicateID in predicates) {
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.GetRelations (veryLargeDocID, noteID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.GetRelations (noteID, veryLargeDocID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.GetRelations (0, noteID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.GetRelations (noteID, 0);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_RemoveSpecificRelation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            bool relationCreated = _mfsOperations.CreateRelation (noteID, urlID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created successfully.");

            bool relationRemoved = _mfsOperations.RemoveSpecificRelation (noteID, urlID, predicateID);
            Assert.IsTrue (relationRemoved, "Specific relation not removed successfully.");

            bool relationExists = _mfsOperations.DoesSpecificRelationExist (noteID, urlID, predicateID);
            Assert.IsFalse (relationExists, "Shows specific relation as existing even though it doesn't.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.RemoveSpecificRelation (veryLargeDocID, noteID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.RemoveSpecificRelation (noteID, veryLargeDocID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong veryLargePredicateID = UInt64.MaxValue;

            try {
                _mfsOperations.RemoveSpecificRelation (noteID, urlID, veryLargePredicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeleteDocument (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.RemoveSpecificRelation (0, noteID, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            try {
                _mfsOperations.DoesSpecificRelationExist (noteID, 0, predicateID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            try {
                _mfsOperations.DoesSpecificRelationExist (noteID, urlID, 0);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
                _mfsOperations.DeleteDocument (urlID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_RemoveAllRelations : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.AddUrl (url, description, when);

            ulong predicateID = _mfsOperations.CreatePredicate (_predicate);

            bool relationCreated = _mfsOperations.CreateRelation (noteID, urlID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created successfully.");

            bool relationRemoved = _mfsOperations.RemoveAllRelations (noteID, urlID);
            Assert.IsTrue (relationRemoved, "Relation not removed successfully.");

            bool relationExists = _mfsOperations.DoesRelationExist (noteID, urlID);
            Assert.IsFalse (relationExists, "Shows relation as existing even though none does.");

            _mfsOperations.DeleteDocument (noteID);
            _mfsOperations.DeleteDocument (urlID);
            _mfsOperations.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.RemoveAllRelations (veryLargeDocID, noteID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            try {
                _mfsOperations.RemoveAllRelations (noteID, veryLargeDocID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.RemoveAllRelations (0, noteID);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.RemoveAllRelations (noteID, 0);
            } finally {
                _mfsOperations.DeleteDocument (noteID);
            }
        }
    }
}
