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

namespace MnemonicFS.Tests.AspectsNotes {
    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.ApplyAspectToNote (aspectID, noteID);
            Assert.IsTrue (applySuccess, "Aspect failed to be applied to note.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToNote (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.ApplyAspectToNote (0, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeNoteID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToNote (aspectID, veryLargeNoteID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToNote (veryLargeAspectID, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_IsAspectAppliedToNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.ApplyAspectToNote (aspectID, noteID);

            bool isApplied = _mfsOperations.IsAspectAppliedToNote (aspectID, noteID);
            Assert.IsTrue (isApplied, "Indicated that aspect has not been applied to note, even though it is.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        public void Test_SanityCheck_NotApplied () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool isApplied = _mfsOperations.IsAspectAppliedToNote (aspectID, noteID);
            Assert.IsFalse (isApplied, "Indicated that aspect has been applied to note, even though it isn't.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.IsAspectAppliedToNote (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.IsAspectAppliedToNote (0, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeNoteID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToNote (aspectID, veryLargeNoteID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToNote (veryLargeAspectID, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            _mfsOperations.ApplyAspectToNote (aspectID, noteID);

            bool isUnapplied = _mfsOperations.UnapplyAspectFromNote (aspectID, noteID);
            Assert.IsTrue (isUnapplied, "Aspect was not successfully unapplied from note.");

            bool isApplied = _mfsOperations.IsAspectAppliedToNote (aspectID, noteID);
            Assert.IsFalse (isApplied, "Attempt to unapply aspect from note was not successful.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.UnapplyAspectFromNote (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.UnapplyAspectFromNote (0, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeNoteID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromNote (aspectID, veryLargeNoteID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromNote (veryLargeAspectID, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectsAppliedOnNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.ApplyAspectToNote (aspectID, noteID);
            }

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnNote (noteID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects applied to note.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteNote (noteID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            _mfsOperations.GetAspectsAppliedOnNote (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong veryLargeNoteID = UInt64.MaxValue;

            _mfsOperations.GetAspectsAppliedOnNote (veryLargeNoteID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetNotesAppliedWithAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                _mfsOperations.ApplyAspectToNote (aspectID, noteID);

                noteIDs.Add (noteID);
            }

            List<ulong> retrievedNoteIDs = _mfsOperations.GetNotesAppliedWithAspect (aspectID);
            Assert.AreEqual (noteIDs.Count, retrievedNoteIDs.Count, "Did not retrieve exact number of notes aspect has been applied to.");

            retrievedNoteIDs.Sort ();
            noteIDs.Sort ();

            for (int i = 0; i < noteIDs.Count; ++i) {
                Assert.AreEqual (noteIDs[i], retrievedNoteIDs[i], "Got invalid note id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong noteID in noteIDs) {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.GetNotesAppliedWithAspect (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.GetNotesAppliedWithAspect (veryLargeAspectID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToNote (aspectIDs, noteID);

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnNote (noteID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteNote (noteID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.ApplyAspectsToNote (null, noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            try {
                _mfsOperations.ApplyAspectsToNote (new List<ulong> (), noteID);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            ulong veryLargeNoteID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectsToNote (aspectIDs, veryLargeNoteID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToNotes : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                noteIDs.Add (noteID);
            }

            _mfsOperations.ApplyAspectToNotes (aspectID, noteIDs);

            List<ulong> retrievedNoteIDs = _mfsOperations.GetNotesAppliedWithAspect (aspectID);
            Assert.AreEqual (noteIDs.Count, retrievedNoteIDs.Count, "Did not retrieve exact number of notes aspect has been applied to.");

            retrievedNoteIDs.Sort ();
            noteIDs.Sort ();

            for (int i = 0; i < noteIDs.Count; ++i) {
                Assert.AreEqual (noteIDs[i], retrievedNoteIDs[i], "Got invalid note id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong noteID in noteIDs) {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNoteList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToNotes (aspectID, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNoteList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToNotes (aspectID, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToNotes : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                noteIDs.Add (noteID);
            }

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToNotes (aspectIDs, noteIDs);

            foreach (ulong noteID in noteIDs) {
                List<ulong> retrAspects = _mfsOperations.GetAspectsAppliedOnNote (noteID);
                Assert.AreEqual (aspectIDs.Count, retrAspects.Count, "Aspect count for note does not match.");
            }

            foreach (ulong aspectID in aspectIDs) {
                List<ulong> retrNotes = _mfsOperations.GetNotesAppliedWithAspect (aspectID);
                Assert.AreEqual (noteIDs.Count, retrNotes.Count, "Note count for aspect does not match.");
            }

            foreach (ulong noteID in noteIDs) {
                _mfsOperations.DeleteNote (noteID);
            }

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNoteList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToNotes (aspectIDs, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNoteList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToNotes (aspectIDs, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            List<ulong> noteIDs = new List<ulong> ();
            noteIDs.Add (noteID);

            try {
                _mfsOperations.ApplyAspectsToNotes (null, noteIDs);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            List<ulong> noteIDs = new List<ulong> ();
            noteIDs.Add (noteID);

            try {
                _mfsOperations.ApplyAspectsToNotes (new List<ulong> (), noteIDs);
            } finally {
                _mfsOperations.DeleteNote (noteID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAllAspectsFromNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToNote (aspectIDs, noteID);

            int numAspectsUnapplied = _mfsOperations.UnapplyAllAspectsFromNote (noteID);
            Assert.AreEqual (numAspectsToCreate, numAspectsUnapplied, "Attempt to unapply aspects from note was unsuccessful.");

            List<ulong> allAspects = _mfsOperations.GetAspectsAppliedOnNote (noteID);
            Assert.AreEqual (0, allAspects.Count, "Incorrect number of aspects unapplied for note.");

            _mfsOperations.DeleteNote (noteID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NoteIDZero_Illegal () {
            _mfsOperations.UnapplyAllAspectsFromNote (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNoteID_Illegal () {
            ulong veryLargeNoteID = UInt64.MaxValue;

            _mfsOperations.UnapplyAllAspectsFromNote (veryLargeNoteID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromAllNotes : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numNotesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> noteIDs = new List<ulong> (numNotesToCreate);

            for (int i = 0; i < numNotesToCreate; ++i) {
                string noteContent = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                DateTime when = DateTime.Now;
                MfsNote note = new MfsNote (noteContent, when);
                ulong noteID = _mfsOperations.AddNote (note);

                noteIDs.Add (noteID);
            }

            _mfsOperations.ApplyAspectToNotes (aspectID, noteIDs);

            int numNotesUnappliedFrom = _mfsOperations.UnapplyAspectFromAllNotes (aspectID);
            Assert.AreEqual (numNotesToCreate, numNotesUnappliedFrom, "Attempt to unapply aspect from notes was unsuccessful.");

            List<ulong> allNotes = _mfsOperations.GetNotesAppliedWithAspect (aspectID);
            Assert.AreEqual (0, allNotes.Count, "Number of notes that aspect was unapplied from was incorrect.");

            foreach (ulong noteID in noteIDs) {
                _mfsOperations.DeleteNote (noteID);
            }

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.UnapplyAspectFromAllNotes (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.UnapplyAspectFromAllNotes (veryLargeAspectID);
        }
    }
}
