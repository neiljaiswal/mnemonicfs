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
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);
            Assert.That (predicateID > 0, "Predicate id is not valid.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicatePredicate_SanityCheck () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.NewPredicate (_predicate);
            } finally {
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        public void Test_PredicateWithMaxSizeAllowed_SanityCheck () {
            string predicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (predicate);

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateGreaterThanMaxSizeAllowed_Illegal () {
            string predicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength + 1);

            _mfsOperations.Relation.NewPredicate (predicate);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPredicate_Illegal () {
            string nullPredicate = null;

            _mfsOperations.Relation.NewPredicate (nullPredicate);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPredicate_Illegal () {
            string emptyPredicate = string.Empty;

            _mfsOperations.Relation.NewPredicate (emptyPredicate);
        }
    }

    [TestFixture]
    public class TestRelations_DoesPredicateExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            bool predicateExists = _mfsOperations.Relation.DoesPredicateExist (predicateID);
            Assert.IsTrue (predicateExists, "Predicate was shown as not existing, even though it does.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargePredicateID = ulong.MaxValue;

            bool predicateExists = _mfsOperations.Relation.DoesPredicateExist (veryLargePredicateID);
            Assert.IsFalse (predicateExists, "Predicate was shown as existing, even though it does not.");
        }

        [Test]
        public void Test_SanityCheck_WithPredicate_Exists () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            bool predicateExists = _mfsOperations.Relation.DoesPredicateExist (_predicate);
            Assert.IsTrue (predicateExists, "Predicate was shown as not existing, even though it does.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_WithPredicate_NotExists () {
            string somePredicateName = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);

            bool predicateExists = _mfsOperations.Relation.DoesPredicateExist (somePredicateName);
            ;
            Assert.IsFalse (predicateExists, "Predicate was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            _mfsOperations.Relation.DoesPredicateExist (0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateNull_Illegal () {
            _mfsOperations.Relation.DoesPredicateExist (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateSizeZero_Illegal () {
            _mfsOperations.Relation.DoesPredicateExist (string.Empty);
        }
    }

    [TestFixture]
    public class TestRelations_DeletePredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            int numPredicatesDeleted = _mfsOperations.Relation.DeletePredicate (predicateID);
            Assert.AreEqual (1, numPredicatesDeleted, "Predicate was not deleted.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            _mfsOperations.Relation.DeletePredicate (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            ulong veryLargePredicateID = UInt64.MaxValue;

            _mfsOperations.Relation.DeletePredicate (veryLargePredicateID);
        }
    }

    [TestFixture]
    public class TestRelations_GetPredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            string retrPredicate = _mfsOperations.Relation.GetPredicate (predicateID);
            Assert.AreEqual (_predicate, retrPredicate, "Did not retrieve existing predicate's name.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            _mfsOperations.Relation.GetPredicate (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            ulong veryLargePredicateID = UInt64.MaxValue;

            _mfsOperations.Relation.GetPredicate (veryLargePredicateID);
        }
    }

    [TestFixture]
    public class TestRelations_GetPredicateID : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            ulong retrPredicateID = _mfsOperations.Relation.GetPredicateID (_predicate);
            Assert.AreEqual (predicateID, retrPredicateID, "Wrong predicate id retrieved.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateName_Illegal () {
            string nonExistentPredicateName = null;

            do {
                nonExistentPredicateName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Relation.DoesPredicateExist (nonExistentPredicateName));

            _mfsOperations.Relation.GetPredicateID (nonExistentPredicateName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPredicateName_Illegal () {
            string nullPredicateName = null;

            _mfsOperations.Relation.GetPredicateID (nullPredicateName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPredicateName_Illegal () {
            string emptyPredicateName = string.Empty;

            _mfsOperations.Relation.GetPredicateID (emptyPredicateName);
        }
    }

    [TestFixture]
    public class TestRelations_GetAllPredicates : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> predicateIDs = CreateUniqueNPredicates (ref _mfsOperations, TYPICAL_MULTI_VALUE);

            List<ulong> retrPredicateIDs = _mfsOperations.Relation.GetAllPredicates ();

            predicateIDs.Sort ();
            retrPredicateIDs.Sort ();

            Assert.AreEqual (predicateIDs.Count, retrPredicateIDs.Count, "Wrong number of predicate ids retrieved.");
            Assert.AreEqual (predicateIDs, retrPredicateIDs, "Wrong predicate ids recovered.");

            foreach (ulong predicateID in predicateIDs) {
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_UpdatePredicate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            string newPredicateName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            bool predicateUpdated = _mfsOperations.Relation.UpdatePredicate (predicateID, newPredicateName);
            Assert.IsTrue (predicateUpdated, "Predicate was not updated successfully.");

            string predicate = _mfsOperations.Relation.GetPredicate (predicateID);
            Assert.AreEqual (newPredicateName, predicate, "Predicate was not updated successfully.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_PredicateWithMaxSizeAllowed_SanityCheck () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            string newPredicate = TestUtils.GetAWord (MfsOperations.MaxPredicateLength);
            _mfsOperations.Relation.UpdatePredicate (predicateID, newPredicate);

            string predicate = _mfsOperations.Relation.GetPredicate (predicateID);
            Assert.AreEqual (newPredicate, predicate, "Wrong predicate returned after predicate updation.");

            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            string any = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.Relation.UpdatePredicate (0, any);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            ulong veryLargePredicateID = UInt64.MaxValue;

            string any = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.Relation.UpdatePredicate (veryLargePredicateID, any);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewPredicate_Illegal () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            string nullPredicateName = null;

            try {
                _mfsOperations.Relation.UpdatePredicate (predicateID, nullPredicateName);
            } finally {
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewPredicate_Illegal () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            string emptyPredicateName = string.Empty;

            try {
                _mfsOperations.Relation.UpdatePredicate (predicateID, emptyPredicateName);
            } finally {
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateGreaterThanSystemDefined_Illegal () {
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            string veryLongPredicateName = TestUtils.GetAWord (MfsOperations.MaxPredicateLength + 1);

            try {
                _mfsOperations.Relation.UpdatePredicate (predicateID, veryLongPredicateName);
            } finally {
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }
    }

    [TestFixture]
    public class TestRelations_DeleteAllPredicates : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numPredicatesToCreate = TYPICAL_MULTI_VALUE;

            List<ulong> predicateIDs = CreateUniqueNPredicates (ref _mfsOperations, numPredicatesToCreate);

            int numPredicatesDeleted = _mfsOperations.Relation.DeleteAllPredicates ();
            Assert.AreEqual (predicateIDs.Count, numPredicatesDeleted, "Did not delete the same number of predicates as were created.");
        }
    }

    [TestFixture]
    public class TestRelations_CreateRelation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            // Save a note:
            ulong noteID = CreateNote (ref _mfsOperations);

            // And save a url:
            ulong urlID = CreateUrl (ref _mfsOperations);

            // And also create a predicate:
            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            // Now create a relation between them:
            bool relationCreated = _mfsOperations.Relation.New (noteID, urlID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_DocSelfRelation_Legal () {
            ulong noteID = CreateNote (ref _mfsOperations);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            bool relationCreated = _mfsOperations.Relation.New (noteID, noteID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.New (veryLargeDocID, noteID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.New (noteID, veryLargeDocID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong veryLargePredicateID = UInt64.MaxValue;

            try {
                _mfsOperations.Relation.New (noteID, urlID, veryLargePredicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Document.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.New (0, noteID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.New (noteID, 0, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Relation.New (noteID, urlID, 0);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Document.Delete (urlID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            _mfsOperations.Relation.New (noteID, urlID, predicateID);

            bool doesRelationExist = _mfsOperations.Relation.Exists (noteID, urlID);
            Assert.IsTrue (doesRelationExist, "Shows relation as not existing even though it does.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            bool doesRelationExist = _mfsOperations.Relation.Exists (noteID, urlID);
            Assert.IsFalse (doesRelationExist, "Shows relation as existing even though it doesn't.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Exists (veryLargeDocID, noteID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            try {
                _mfsOperations.Relation.Exists (noteID, veryLargeDocID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Exists (0, noteID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Exists (noteID, 0);
            } finally {
                _mfsOperations.Document.Delete (noteID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            _mfsOperations.Relation.New (noteID, urlID, predicateID);

            bool doesRelationExist = _mfsOperations.Relation.SpecificExists (noteID, urlID, predicateID);
            Assert.IsTrue (doesRelationExist, "Shows relation as not existing even though it does.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            bool doesRelationExist = _mfsOperations.Relation.SpecificExists (noteID, urlID, predicateID);
            Assert.IsFalse (doesRelationExist, "Shows relation as existing even though it doesn't.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.SpecificExists (veryLargeDocID, noteID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.SpecificExists (noteID, veryLargeDocID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong veryLargePredicateID = UInt64.MaxValue;

            try {
                _mfsOperations.Relation.SpecificExists (noteID, urlID, veryLargePredicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Document.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.SpecificExists (0, noteID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.SpecificExists (noteID, 0, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Relation.SpecificExists (noteID, urlID, 0);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Document.Delete (urlID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            List<ulong> predicates = new List<ulong> ();
            char ch = 'a';
            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string predicate = _predicate + ch++;
                ulong predicateID = _mfsOperations.Relation.NewPredicate (predicate);
                _mfsOperations.Relation.New (noteID, urlID, predicateID);
                predicates.Add (predicateID);
            }

            List<ulong> relationsList = _mfsOperations.Relation.Get (noteID, urlID);
            Assert.AreEqual (predicates.Count, relationsList.Count, "Shows incorrect number of relations.");

            predicates.Sort ();
            relationsList.Sort ();
            Assert.AreEqual (predicates, relationsList, "Wrong relations returned.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            foreach (ulong predicateID in predicates) {
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Get (veryLargeDocID, noteID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Get (noteID, veryLargeDocID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Get (0, noteID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.Get (noteID, 0);
            } finally {
                _mfsOperations.Document.Delete (noteID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            bool relationCreated = _mfsOperations.Relation.New (noteID, urlID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created successfully.");

            bool relationRemoved = _mfsOperations.Relation.RemoveSpecific (noteID, urlID, predicateID);
            Assert.IsTrue (relationRemoved, "Specific relation not removed successfully.");

            bool relationExists = _mfsOperations.Relation.SpecificExists (noteID, urlID, predicateID);
            Assert.IsFalse (relationExists, "Shows specific relation as existing even though it doesn't.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.RemoveSpecific (veryLargeDocID, noteID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.RemoveSpecific (noteID, veryLargeDocID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentPredicateID_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong veryLargePredicateID = UInt64.MaxValue;

            try {
                _mfsOperations.Relation.RemoveSpecific (noteID, urlID, veryLargePredicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Document.Delete (urlID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.RemoveSpecific (0, noteID, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            try {
                _mfsOperations.Relation.SpecificExists (noteID, 0, predicateID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Relation.DeletePredicate (predicateID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_PredicateIDZero_Illegal () {
            DateTime when = DateTime.Now;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            try {
                _mfsOperations.Relation.SpecificExists (noteID, urlID, 0);
            } finally {
                _mfsOperations.Document.Delete (noteID);
                _mfsOperations.Document.Delete (urlID);
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
            ulong noteID = _mfsOperations.Note.New (note);

            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);

            ulong predicateID = _mfsOperations.Relation.NewPredicate (_predicate);

            bool relationCreated = _mfsOperations.Relation.New (noteID, urlID, predicateID);
            Assert.IsTrue (relationCreated, "Relation not created successfully.");

            bool relationRemoved = _mfsOperations.Relation.RemoveAll (noteID, urlID);
            Assert.IsTrue (relationRemoved, "Relation not removed successfully.");

            bool relationExists = _mfsOperations.Relation.Exists (noteID, urlID);
            Assert.IsFalse (relationExists, "Shows relation as existing even though none does.");

            _mfsOperations.Document.Delete (noteID);
            _mfsOperations.Document.Delete (urlID);
            _mfsOperations.Relation.DeletePredicate (predicateID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSubjectDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.RemoveAll (veryLargeDocID, noteID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentObjectDocID_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            ulong veryLargeDocID = UInt64.MaxValue;

            try {
                _mfsOperations.Relation.RemoveAll (noteID, veryLargeDocID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SubjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.RemoveAll (0, noteID);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ObjectDocIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            try {
                _mfsOperations.Relation.RemoveAll (noteID, 0);
            } finally {
                _mfsOperations.Document.Delete (noteID);
            }
        }
    }
}
