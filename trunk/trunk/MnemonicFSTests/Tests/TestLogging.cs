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
using MnemonicFS.MfsCore;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.Tests.Logging {
    [TestFixture]
    public class Tests_MfsOperations_LogCreation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<MfsFileLogEntry> fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (1, fileLogEntries.Count, "Incorrect number of log entries returned for file.");

            _mfsOperations.DeleteFile (fileID);

            fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (2, fileLogEntries.Count, "Incorrect number of log entries returned for file.");
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_RetrieveFileLogs : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<MfsFileLogEntry> fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (1, fileLogEntries.Count, "Incorrect number of log entries returned for file.");

            _mfsOperations.DeleteFile (fileID);

            fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (2, fileLogEntries.Count, "Incorrect number of log entries returned for file.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDNull_Illegal () {
            string nullUserID = null;

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                MfsOperations.RetrieveFileLogs (nullUserID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDEmpty_Illegal () {
            string emptyUserID = string.Empty;

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                MfsOperations.RetrieveFileLogs (emptyUserID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentUserException))]
        public void Test_NonExistentUserID_Illegal () {
            string nonExistentUserID = GetANonExistentUserID ();

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                MfsOperations.RetrieveFileLogs (nonExistentUserID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            MfsOperations.RetrieveFileLogs (_userID, 0);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_RetrieveUserFileLogs : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<MfsFileLogEntry> allFilesLogEntries = MfsOperations.RetrieveUserFileLogs (_userID);
            Assert.AreEqual (2, allFilesLogEntries.Count, "Incorrect number of file log entries returned for user.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDNull_Illegal () {
            string nullUserID = null;
            MfsOperations.RetrieveUserFileLogs (nullUserID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyUserID_Illegal () {
            string emptyUserID = string.Empty;
            MfsOperations.RetrieveUserFileLogs (emptyUserID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentUserException))]
        public void Test_NonExistentUserID_Illegal () {
            string nonExistentUserID = GetANonExistentUserID ();
            MfsOperations.RetrieveUserFileLogs (nonExistentUserID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DeleteFileLogs : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<MfsFileLogEntry> fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (1, fileLogEntries.Count, "Incorrect number of log entries returned for file.");

            _mfsOperations.DeleteFile (fileID);

            fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (2, fileLogEntries.Count, "Incorrect number of log entries returned for file.");

            int numLogsDeleted = MfsOperations.DeleteFileLogs (_userID, fileID);
            Assert.AreEqual (2, numLogsDeleted, "Number of logs deleted for file are incorrect.");

            fileLogEntries = MfsOperations.RetrieveFileLogs (_userID, fileID);
            Assert.AreEqual (0, fileLogEntries.Count, "Did not delete file logs totally.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDNull_Illegal () {
            string nullUserID = null;

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                MfsOperations.DeleteFileLogs (nullUserID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDEmpty_Illegal () {
            string emptyUserID = string.Empty;

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                MfsOperations.DeleteFileLogs (emptyUserID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentUserException))]
        public void Test_NonExistentUserID_Illegal () {
            string nonExistentUserID = GetANonExistentUserID ();

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                MfsOperations.DeleteFileLogs (nonExistentUserID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            MfsOperations.DeleteFileLogs (_userID, 0);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DeleteUserFileLogs : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<MfsFileLogEntry> allFilesLogEntries = MfsOperations.RetrieveUserFileLogs (_userID);
            Assert.AreEqual (2, allFilesLogEntries.Count, "Incorrect number of file log entries returned for user.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);

            allFilesLogEntries = MfsOperations.RetrieveUserFileLogs (_userID);
            Assert.AreEqual (4, allFilesLogEntries.Count, "Incorrect number of file log entries returned for user.");

            int numLogsDeleted = MfsOperations.DeleteUserFileLogs (_userID);
            Assert.AreEqual (4, numLogsDeleted, "Number of logs deleted for file are incorrect.");

            allFilesLogEntries = MfsOperations.RetrieveUserFileLogs (_userID);
            Assert.AreEqual (0, allFilesLogEntries.Count, "Incorrect number of file log entries returned for user after deletion.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_UserIDNull_Illegal () {
            string nullUserID = null;
            MfsOperations.DeleteUserFileLogs (nullUserID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyUserID_Illegal () {
            string emptyUserID = string.Empty;
            MfsOperations.DeleteUserFileLogs (emptyUserID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentUserException))]
        public void Test_NonExistentUserID_Illegal () {
            string nonExistentUserID = GetANonExistentUserID ();
            MfsOperations.DeleteUserFileLogs (nonExistentUserID);
        }
    }
}
