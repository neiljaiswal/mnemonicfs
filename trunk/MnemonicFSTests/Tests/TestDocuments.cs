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

namespace MnemonicFS.Tests.Documents {
    [TestFixture]
    public class Tests_AspectsMethod_UniqueDocumentID : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            // First save a file:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "File id returned is not a valid value.");

            // Next, save a note:
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);
            Assert.That (noteID > 0, "Note id returned is not a valid value.");

            // And also save a url:
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);
            Assert.That (urlID > 0, "Url id returned is not a valid value.");

            // Next, confirm that all document ids are different:
            Assert.AreNotEqual (fileID, noteID, "File id is the same as note id.");
            Assert.AreNotEqual (fileID, urlID, "File id is the same as url id.");
            Assert.AreNotEqual (noteID, urlID, "Note id is the same as url id.");

            _mfsOperations.File.Delete (fileID);
            _mfsOperations.Note.Delete (noteID);
            _mfsOperations.Url.Delete (urlID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetDocumentType : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            // First save a file:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "File id returned is not a valid value.");

            // Next, save a note:
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);
            Assert.That (noteID > 0, "Note id returned is not a valid value.");

            // And also save a url:
            string url = TestUtils.GetAnyUrl ();
            string description = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            ulong urlID = _mfsOperations.Url.New (url, description, when);
            Assert.That (urlID > 0, "Url id returned is not a valid value.");

            DocumentType docType1 = _mfsOperations.Document.Type (fileID);
            Assert.That (docType1 == DocumentType.FILE, "Indicated that document type is not a file even though it is.");

            DocumentType docType2 = _mfsOperations.Document.Type (noteID);
            Assert.That (docType2 == DocumentType.NOTE, "Indicated that document type is not a note even though it is.");

            DocumentType docType3 = _mfsOperations.Document.Type (urlID);
            Assert.That (docType3 == DocumentType.URL, "Indicated that document type is not a url even though it is.");

            _mfsOperations.File.Delete (fileID);
            _mfsOperations.Note.Delete (noteID);
            _mfsOperations.Url.Delete (urlID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            _mfsOperations.Document.Type (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void NonExistentDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.Document.Type (veryLargeDocID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_DeleteDocument : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string noteContent = TestUtils.GetASentence (TestMfsOperationsBase.TYPICAL_SENTENCE_SIZE, TestMfsOperationsBase.TYPICAL_WORD_SIZE);
            DateTime when = DateTime.Now;
            MfsNote note = new MfsNote (noteContent, when);
            ulong noteID = _mfsOperations.Note.New (note);

            bool deleted = _mfsOperations.Document.Delete (noteID);
            Assert.IsTrue (deleted, "Document not deleted though it should have been.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            _mfsOperations.Document.Delete (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void NonExistentDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.Document.Delete (veryLargeDocID);
        }
    }
}
