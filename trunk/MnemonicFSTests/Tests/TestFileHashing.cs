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
using MnemonicFS.MfsUtils;
using MnemonicFS.MfsExceptions;
using MnemonicFS.Tests.Utils;
using MnemonicFS.Tests.Base;

namespace MnemonicFS.Tests.FileHashing {
    [TestFixture]
    public class Tests_MfsOperations_GetFileHash : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            // First calculate the file's hash:
            string fileHash = TestUtils.GetFileHash (_fileData);

            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string fileHashReturned = _mfsOperations.GetFileHash (fileID);
            Assert.AreEqual (fileHash, fileHashReturned, "File hash returned is incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileHash (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileHash (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileVersionHash : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            byte[] fileDataNewVersion = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string someComment = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // Get the file data hash:
            string newVersionHash = TestUtils.GetFileHash (fileDataNewVersion);

            int versionNumber = _mfsOperations.SaveAsNextVersion (fileID, fileDataNewVersion, someComment, 0);

            string fileHashReturned = _mfsOperations.GetFileVersionHash (fileID, versionNumber);
            Assert.AreEqual (newVersionHash, fileHashReturned, "Versioned file hash returned is incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileVersionHash (0, 0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileVersionHash (veryLargeFileID, 0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NegativeVersionID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            int negativeFileVersionID = Int32.MinValue;

            try {
                _mfsOperations.GetFileVersionHash (fileID, negativeFileVersionID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentVersionID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            int veryLargeFileVersionID = Int32.MaxValue;

            try {
                _mfsOperations.GetFileVersionHash (fileID, veryLargeFileVersionID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetDuplicateFiles : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            // We first save three different files to the system. Notice that they are guaranteed to
            // be different since they are each of a different size:
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            _fileName = TestUtils.GetAnyFileName ();
            _fileNarration = TestUtils.GetASentence (TYPICAL_WORD_SIZE, TYPICAL_SENTENCE_SIZE);
            ulong fileID1 = _mfsOperations.SaveFile (_fileName, _fileNarration, fileData1, when, false);

            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            _fileName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            _fileNarration = TestUtils.GetASentence (TYPICAL_WORD_SIZE, TYPICAL_SENTENCE_SIZE);
            ulong fileID2 = _mfsOperations.SaveFile (_fileName, _fileNarration, fileData2, when, false);

            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            _fileName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            _fileNarration = TestUtils.GetASentence (TYPICAL_WORD_SIZE, TYPICAL_SENTENCE_SIZE);
            ulong fileID3 = _mfsOperations.SaveFile (_fileName, _fileNarration, fileData3, when, false);

            // We will next save fileData2 with a different file name and narration, and the system should
            // be able to detect that the files are the same. It will return a heuristics score indicating
            // the extent to which the files are similar:
            _fileName = TestUtils.GetAnyFileName () + SINGLE_CHAR_STR;
            _fileNarration = TestUtils.GetASentence (TYPICAL_WORD_SIZE + SINGLE_VALUE, TYPICAL_SENTENCE_SIZE);
            ulong fileID4 = _mfsOperations.SaveFile (_fileName, _fileNarration, fileData2, when, false);

            Dictionary<ulong, double> filesScores = null;

            filesScores = _mfsOperations.GetDuplicateFiles (fileID1);
            Assert.IsEmpty (filesScores, "Returned non-empty collection of duplicate files, though none expected.");

            filesScores = _mfsOperations.GetDuplicateFiles (fileID3);
            Assert.IsEmpty (filesScores, "Returned non-empty collection of duplicate files, though none expected.");

            // Recall that fileID2 and fileID4 are the same file with different names and narrations:
            filesScores = _mfsOperations.GetDuplicateFiles (fileID2);
            Assert.IsNotEmpty (filesScores, "Returned empty collection of duplicate files, though exactly one file was expected.");
            Assert.AreEqual (SINGLE_VALUE, filesScores.Count, "Returned more than one duplicate file, though exactly one was expected.");

            Dictionary<ulong, double>.KeyCollection keys = filesScores.Keys;
            double score = 0;
            foreach (ulong fileID in keys) {
                Assert.AreEqual (fileID4, fileID, "Returned duplicate file id is not the same as expected.");
                score = filesScores[fileID];
            }
            Assert.AreEqual (0.75, score, "Heuristic score is not as expected.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
            _mfsOperations.DeleteFile (fileID3);
            _mfsOperations.DeleteFile (fileID4);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetDuplicateFiles (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetDuplicateFiles (veryLargeFileID);
        }

        [Test]
        public void Test_DifferentFile_SameName_SameNarration () {
            DateTime when = DateTime.Now;

            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = _mfsOperations.SaveFile (_fileName, _fileNarration, fileData1, when, false);

            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = _mfsOperations.SaveFile (_fileName, _fileNarration, fileData2, when, false);

            Dictionary<ulong, double> filesScores = null;

            filesScores = _mfsOperations.GetDuplicateFiles (fileID1);
            Assert.IsEmpty (filesScores, "Returned non-empty collection of files, even though none expected.");

            filesScores = _mfsOperations.GetDuplicateFiles (fileID2);
            Assert.IsEmpty (filesScores, "Returned non-empty collection of files, even though none expected.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
        }

        [Test]
        public void Test_SameFile_DifferentNames_SameNarration () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID1 = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            // To guarantee that file name is different, we append a character to it:
            _fileName += SINGLE_CHAR_STR;
            ulong fileID2 = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            Dictionary<ulong, double> filesScores = null;

            filesScores = _mfsOperations.GetDuplicateFiles (fileID1);
            Dictionary<ulong, double>.KeyCollection keys = filesScores.Keys;
            double score = 0;
            foreach (ulong fileID in keys) {
                score = filesScores[fileID];
            }
            Assert.AreEqual (0.85, score, "Heuristic score is not as expected.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
        }

        [Test]
        public void Test_SameFile_SameName_DifferentNarrations () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID1 = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            // To guarantee that file narration is different, we append a character to it:
            _fileNarration += SINGLE_CHAR_STR;
            ulong fileID2 = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            Dictionary<ulong, double> filesScores = null;

            filesScores = _mfsOperations.GetDuplicateFiles (fileID1);
            Dictionary<ulong, double>.KeyCollection keys = filesScores.Keys;
            double score = 0;
            foreach (ulong fileID in keys) {
                score = filesScores[fileID];
            }
            Assert.AreEqual (0.9, score, "Heuristic score is not as expected.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
        }

        [Test]
        public void Test_SameFile_SameName_SameNarration () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID1 = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
            ulong fileID2 = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            Dictionary<ulong, double> filesScores = null;

            filesScores = _mfsOperations.GetDuplicateFiles (fileID1);
            Dictionary<ulong, double>.KeyCollection keys = filesScores.Keys;
            double score = 0;
            foreach (ulong fileID in keys) {
                score = filesScores[fileID];
            }
            Assert.AreEqual (1.0, score, "Heuristic score is not as expected.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
        }
    }
}
