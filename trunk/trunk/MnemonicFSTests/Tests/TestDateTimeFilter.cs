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

namespace MnemonicFS.Tests.DateTimeFilter {
    [TestFixture]
    public class Tests_MfsOperations_DateTimeRetrievals : TestMfsOperationsBase {
        [Test]
        public void Test_GetAllFiles_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDs = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i - numFilesToSave / 2);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            List<ulong> allFileIDs = _mfsOperations.File.GetAll ();
            Assert.AreEqual (TYPICAL_MULTI_VALUE, allFileIDs.Count, "Incorrect number of files retrieved.");

            fileIDs.Sort ();
            allFileIDs.Sort ();

            for (int i = 0; i < fileIDs.Count; ++i) {
                Assert.AreEqual (fileIDs[i], allFileIDs[i], "Incorrect file retrieved.");
            }

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        public void Test_GetFilesInDateRange_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDs = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i - numFilesToSave / 2);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            DateTime dt1 = DateTime.Now.AddYears (-1).AddDays (-1);
            DateTime dt2 = DateTime.Now.AddYears (1).AddDays (1);
            List<ulong> filesInDateRange = _mfsOperations.File.GetInDateRange (dt1, dt2);

            Assert.AreEqual (3, filesInDateRange.Count, "Number of files returned in date range are not as expected.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        public void Test_GetFilesInDateTimeRange_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDs = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i - numFilesToSave / 2);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            const int BUFFER_SECONDS_FOR_TEST_TO_RUN = TYPICAL_MULTI_TIME_UNIT; // seconds
            DateTime dt1 = DateTime.Now.AddHours (-1).AddSeconds (-BUFFER_SECONDS_FOR_TEST_TO_RUN);
            DateTime dt2 = DateTime.Now.AddHours (1).AddSeconds (BUFFER_SECONDS_FOR_TEST_TO_RUN);

            List<ulong> filesInDateTimeRange = _mfsOperations.File.GetInDateTimeRange (dt1, dt2);

            Assert.AreEqual (3, filesInDateTimeRange.Count, "Number of files returned in date-time range are not as expected.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        public void Test_GetFilesOnDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime now = DateTime.Now;

            DateTime whenBefore = DateTime.Now.AddDays (-1);
            ulong fileIDBefore = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, whenBefore, false);

            DateTime whenOn = DateTime.Now;
            ulong fileIDOn = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, whenOn, false);

            DateTime whenAfter = DateTime.Now.AddDays (1);
            ulong fileIDAfter = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, whenAfter, false);

            List<ulong> filesOnDate = _mfsOperations.File.GetOnDate (whenOn);
            Assert.AreEqual (1, filesOnDate.Count, "Number of files returned on date are not as expected.");

            _mfsOperations.File.Delete (fileIDBefore);
            _mfsOperations.File.Delete (fileIDOn);
            _mfsOperations.File.Delete (fileIDAfter);
        }

        [Test]
        public void Test_GetFilesOnDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime now = DateTime.Now;

            DateTime whenBefore = DateTime.Now.AddSeconds (-TYPICAL_MULTI_TIME_UNIT);
            ulong fileIDBefore = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, whenBefore, false);

            DateTime whenOn = DateTime.Now;
            ulong fileIDOn = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, whenOn, false);

            DateTime whenAfter = DateTime.Now.AddSeconds (TYPICAL_MULTI_TIME_UNIT);
            ulong fileIDAfter = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, whenAfter, false);

            List<ulong> filesOnDate = _mfsOperations.File.GetAtDateTime (whenOn);
            Assert.AreEqual (1, filesOnDate.Count, "Number of files returned on date-time are not as expected.");

            _mfsOperations.File.Delete (fileIDBefore);
            _mfsOperations.File.Delete (fileIDOn);
            _mfsOperations.File.Delete (fileIDAfter);
        }

        [Test]
        public void Test_GetFilesBeforeDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDate = _mfsOperations.File.GetBeforeDate (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDate.Count, "Number of files returned before date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesBeforeDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDateTime = _mfsOperations.File.GetBeforeDateTime (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDateTime.Count, "Number of files returned before date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesBeforeAndOnDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDate = _mfsOperations.File.GetBeforeAndOnDate (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDate.Count, "Number of files returned before and on date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesBeforeAndOnDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDateTime = _mfsOperations.File.GetBeforeAndAtDateTime (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDateTime.Count, "Number of files returned before and on date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesAfterDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDate = _mfsOperations.File.GetAfterDate (now);
            Assert.AreEqual (fileIDsAfter.Count, filesAfterDate.Count, "Number of files returned after date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesAfterDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDateTime = _mfsOperations.File.GetAfterDateTime (now);
            Assert.AreEqual (fileIDsBefore.Count, filesAfterDateTime.Count, "Number of files returned after date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesAfterAndOnDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDate = _mfsOperations.File.GetAfterAndOnDate (now);
            Assert.AreEqual (fileIDsAfter.Count, filesAfterDate.Count, "Number of files returned after and on date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }

        [Test]
        public void Test_GetFilesAfterAndOnDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (-i - 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDateTime = _mfsOperations.File.GetAfterAndAtDateTime (now);
            Assert.AreEqual (fileIDsAfter.Count, filesAfterDateTime.Count, "Number of files returned after and on date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.File.Delete (fileIDsBefore[i]);
                _mfsOperations.File.Delete (fileIDsAfter[i]);
            }
        }
    }
}
