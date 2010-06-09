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
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.Tests.Notes {
    [TestFixture]
    public class TestNotes_NotesMethod_Creation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            MfsNote note = new MfsNote (noteContent, when);
            Assert.IsNotNull (note, "Note was not created successfully.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNoteContent_Illegal () {
            string nullNoteContent = null;
            DateTime when = DateTime.Now;

            new MfsNote (nullNoteContent, when);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNoteContent_Illegal () {
            string emptyNoteContent = string.Empty;
            DateTime when = DateTime.Now;

            new MfsNote (emptyNoteContent, when);
        }
    }

    [TestFixture]
    public class TestNotes_NotesMethod_AddNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);

            ulong noteID = _mfsOperations.AddNote (note);
            Assert.That (noteID > 0, "Note not added successfully: Invalid note id returned.");

            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        public void Test_PositiveIntegerReturnValue_SanityTest () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);

            ulong noteID = _mfsOperations.AddNote (note);
            Assert.That (noteID > 0, "Note not added successfully: Invalid note id returned.");

            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        public void Test_CurrentDateTime_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);

            ulong noteID = _mfsOperations.AddNote (note);
            Assert.That (noteID > 0, "Note not added successfully: Invalid note id returned.");

            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        public void Test_BackDate_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now.AddYears (-1);
            MfsNote note = new MfsNote (noteContent, when);

            ulong noteID = _mfsOperations.AddNote (note);
            Assert.That (noteID > 0, "Note not added successfully: Invalid note id returned.");

            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNote_Illegal () {
            _mfsOperations.AddNote (null);
        }
    }

    [TestFixture]
    public class TestNotes_NotesMethod_GetNote : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;

            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.AddNote (note);

            MfsNote retrNote = _mfsOperations.GetNote (noteID);

            Assert.AreEqual (note.NoteContent, retrNote.NoteContent, "Note content not as expected.");

            Assert.AreEqual (note.NoteDateTime.Year, retrNote.NoteDateTime.Year, "Note date-time year not as expected.");
            Assert.AreEqual (note.NoteDateTime.Month, retrNote.NoteDateTime.Month, "Note date-time month not as expected.");
            Assert.AreEqual (note.NoteDateTime.Day, retrNote.NoteDateTime.Day, "Note date-time day not as expected.");

            Assert.AreEqual (note.NoteDateTime.Hour, retrNote.NoteDateTime.Hour, "Note date-time hour not as expected.");
            Assert.AreEqual (note.NoteDateTime.Minute, retrNote.NoteDateTime.Minute, "Note date-time minute not as expected.");
            Assert.AreEqual (note.NoteDateTime.Second, retrNote.NoteDateTime.Second, "Note date-time second not as expected.");

            _mfsOperations.DeleteNote (noteID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentNote_Illegal () {
            ulong veryLargeNoteID = UInt64.MaxValue;

            _mfsOperations.GetNote (veryLargeNoteID);
        }
    }

    [TestFixture]
    public class TestNotes_NotesMethod_GetNoteDate : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);

            ulong noteID = _mfsOperations.AddNote (note);

            DateTime savedDate = _mfsOperations.GetNoteDateTime (noteID);
            Assert.AreEqual (when.Year, savedDate.Year, "Year returned is not the same as saved year.");
            Assert.AreEqual (when.Month, savedDate.Month, "Month returned is not the same as saved month.");
            Assert.AreEqual (when.Day, savedDate.Day, "Day returned is not the same as saved day.");

            _mfsOperations.DeleteNote (noteID);
        }
    }

    [TestFixture]
    public class TestNotes_NotesMethod_GetNoteDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);

            ulong noteID = _mfsOperations.AddNote (note);

            DateTime savedDate = _mfsOperations.GetNoteDateTime (noteID);
            Assert.AreEqual (when.Year, savedDate.Year, "Year returned is not the same as saved year.");
            Assert.AreEqual (when.Month, savedDate.Month, "Month returned is not the same as saved month.");
            Assert.AreEqual (when.Day, savedDate.Day, "Day returned is not the same as saved day.");
            Assert.AreEqual (when.Hour, savedDate.Hour, "Hour returned is not the same as saved hour.");
            Assert.AreEqual (when.Minute, savedDate.Minute, "Minute returned is not the same as saved minute.");
            Assert.AreEqual (when.Second, savedDate.Second, "Second returned is not the same as saved second.");

            _mfsOperations.DeleteNote (noteID);
        }
    }
}
