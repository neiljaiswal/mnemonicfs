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
using MnemonicFS.Tests.Base;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.Tests.FileVersioning {
    [TestFixture]
    public class Tests_SaveAsNextVersion : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Last file version number and its data stream are retrieved:
            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

            // The client now has the responsibilty of saving _both_ the file data as well as the version number
            // in its local cache.

            // User next makes some modifications to the locally cached file.
            // Assuming that the new file data (document) is the modified version of the old file:
            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            // User also adds a comment to the new version:
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // And saves it to the repository. Since no new version has been checked in by any other user,
            // we know for sure that the new version number should be one.
            int versionNumber = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            Assert.AreEqual (1, versionNumber, "First modified version number was not one.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            int lastVersionNumber = 0;

            _mfsOperations.File.SaveAsNextVersion (0, fileDataModified, comments, lastVersionNumber);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            int lastVersionNumber = 0;

            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.SaveAsNextVersion (veryLargeFileID, fileDataModified, comments, lastVersionNumber);
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_NullFileData_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

            byte[] nullFileData = null;
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, nullFileData, comments, lastVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_ZeroSizeFileData_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

            byte[] emptyFileData = TestUtils.GetAnyFileData (FileSize.ZERO_FILE_SIZE);
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, emptyFileData, comments, lastVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_FileDataGreaterThanMaxSizeAllowed_Illegal () {
            Assert.IsTrue (true, "This test will have to be taken on faith, since it may not be possible to create file data as large as the limit defined.");

            throw new MfsFileDataException ("Deliberate throw to pass this test.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullComments_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string nullComments = null;

            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, nullComments, lastVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        public void Test_EmptyComments_Legal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string emptyComments = string.Empty;

            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, emptyComments, lastVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_VersionNumberNegative_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            int someNegativeIllegalVersionNumber = -1;

            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, someNegativeIllegalVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentVersionNumber_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            int veryLargeVersionNumber = Int32.MaxValue;

            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, veryLargeVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_RetrieveLastFileVersion : TestMfsOperationsBase {
        [Test]
        public void Test_VirginFileHasVersionNumberZero_SanityCheck () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Last file version number and its data stream are retrieved:
            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);
            Assert.AreEqual (0, lastVersionNumber, "Last version number is not zero, though there have been no other check-ins.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_NewVersionNumber_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Check out last file version:
            int currentHeadVersionNumber = _mfsOperations.File.GetLastVersionNumber (fileID);

            // Make some modifications to current file.
            // Assuming that the new file data object has the modified version of the old file:
            byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            // Add a comment to the new version:
            string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            int versionNumber = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, currentHeadVersionNumber);
            Assert.AreEqual (1, versionNumber, "First modified version number was not one.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_RetrieveLastFileVersionData_SanityCheck () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Last file version number and its data stream are retrieved:
            int lastVersionNumber;
            byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);
            for (int i = 0; i < fileData.Length; ++i) {
                Assert.AreEqual (_fileData[i], fileData[i], "Did not retrieve the file data correctly.");
            }

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            int lastVersionNumber;

            _mfsOperations.File.RetrieveLastVersion (0, out lastVersionNumber);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            int lastVersionNumber;

            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.RetrieveLastVersion (veryLargeFileID, out lastVersionNumber);
        }
    }

    [TestFixture]
    public class Tests_GetFileVersion : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // We next save some versions of the original file.
            int numVersionsToSave = TYPICAL_MULTI_VALUE;
            int numVersionToCheckFor = numVersionsToSave / 2;
            byte[] dataToCheckFor = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            int lastVersionNumber = 0;

            for (int i = 0; i < numVersionsToSave; ++i) {
                // Last file version number and its data stream are retrieved:
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

                // User next makes some modifications to the locally cached file.
                // Assuming that the new file data (document) is the modified version of the old file:
                byte[] fileDataModified = null;

                if (i != numVersionToCheckFor) {
                    fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                } else {
                    fileDataModified = dataToCheckFor;
                }

                // User also adds a comment to the new version:
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

                // And saves it to the repository:
                lastVersionNumber = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            Assert.AreEqual (
                numVersionsToSave, lastVersionNumber,
                string.Format ("Last file version number retrieved should have been {0}.", numVersionsToSave)
                );

            byte[] fileDataRetr = _mfsOperations.File.RetrieveVersion (fileID, numVersionToCheckFor);

            for (int i = 0; i < fileDataRetr.Length; ++i) {
                Assert.AreEqual (dataToCheckFor[i], fileDataRetr[i], "Did not correctly retrieve file data for version sought.");
            }

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            int versionNumber = 0;

            _mfsOperations.File.RetrieveVersion (0, versionNumber);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            int versionNumber = 0;

            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.RetrieveVersion (veryLargeFileID, versionNumber);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_VersionNumberNegative_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int someNegativeIllegalVersionNumber = -1;

            try {
                _mfsOperations.File.RetrieveVersion (fileID, someNegativeIllegalVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentVersionNumber_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int veryLargeVersionNumber = Int32.MaxValue;

            try {
                _mfsOperations.File.RetrieveVersion (fileID, veryLargeVersionNumber);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_GetFileVersionDetails : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int BUFFER_SECONDS = 3;
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // We next save some versions of the original file.
            int numVersionsToSave = TYPICAL_MULTI_VALUE;
            int atNumber = numVersionsToSave / 2;
            byte[] dataToCheckFor = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string commentToCheckFor = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            DateTime beforeDateTime = DateTime.Now.AddSeconds (-BUFFER_SECONDS);
            DateTime afterDateTime = DateTime.MaxValue;

            int numVersionNumberToCheck = -1;

            for (int i = 0; i < numVersionsToSave; ++i) {
                // Last file version number and its data stream are retrieved:
                int currentHeadVersionNumberInRepo;
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out currentHeadVersionNumberInRepo);

                // User next makes some modifications to the locally cached file.
                // Assuming that the new file data (document) is the modified version of the old file:
                byte[] fileDataModified = null;
                // User also adds a comment to the new version:
                string comments = null;

                if (i == atNumber) {
                    fileDataModified = dataToCheckFor;
                    comments = commentToCheckFor;
                    // And saves it to the repository:
                    int newHeadVersionNumberInRepo = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, currentHeadVersionNumberInRepo);
                    numVersionNumberToCheck = newHeadVersionNumberInRepo;
                } else {
                    fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                    comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                    // And saves it to the repository:
                    int newHeadVersionNumberInRepo = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, currentHeadVersionNumberInRepo);
                }

                if (i != atNumber) {
                    afterDateTime = DateTime.Now.AddSeconds (BUFFER_SECONDS);
                }
            }

            string userComments;
            DateTime whenDateTime;
            _mfsOperations.File.GetVersionDetails (fileID, numVersionNumberToCheck, out userComments, out whenDateTime);
            Assert.AreEqual (commentToCheckFor, userComments, "User comments retrieved are not as expected.");
            Assert.That (whenDateTime >= beforeDateTime && whenDateTime <= afterDateTime, "Timestamp on file version is incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            int versionNumber = 0;

            string comments;
            DateTime whenDateTime;

            _mfsOperations.File.GetVersionDetails (0, versionNumber, out comments, out whenDateTime);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            int versionNumber = 0;

            string comments;
            DateTime whenDateTime;

            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetVersionDetails (veryLargeFileID, versionNumber, out comments, out whenDateTime);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_VersionNumberNegative_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string comments;
            DateTime whenDateTime;

            int someNegativeIllegalVersionNumber = -1;

            try {
                _mfsOperations.File.GetVersionDetails (fileID, someNegativeIllegalVersionNumber, out comments, out whenDateTime);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentVersionNumber_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string comments;
            DateTime whenDateTime;

            int veryLargeVersionNumber = Int32.MaxValue;

            try {
                _mfsOperations.File.GetVersionDetails (fileID, veryLargeVersionNumber, out comments, out whenDateTime);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_GetFileVersionHistoryLog : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int BUFFER_SECONDS = 3;
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime beforeDateTime = DateTime.Now.AddSeconds (-BUFFER_SECONDS);

            // We next save some versions of the original file.
            int numVersionsToSave = TYPICAL_MULTI_VALUE;

            string[] comments = new string[numVersionsToSave];
            DateTime[] dateTimes = new DateTime[numVersionsToSave];

            for (int i = 0; i < numVersionsToSave; ++i) {
                // Last file version number and its data stream are retrieved:
                int currentHeadVersionNumberInRepo;
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out currentHeadVersionNumberInRepo);

                // User next makes some modifications to the locally cached file.
                // Assuming that the new file data (document) is the modified version of the old file:
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                comments[i] = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                dateTimes[i] = DateTime.Now;
                int newHeadVersionNumberInRepo = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments[i], currentHeadVersionNumberInRepo);
            }

            DateTime afterDateTime = DateTime.Now.AddSeconds (BUFFER_SECONDS);

            string[] versionComments;
            DateTime[] versionDateTimes;
            _mfsOperations.File.GetVersionHistoryLog (fileID, out versionComments, out versionDateTimes);

            for (int i = 0; i < numVersionsToSave; ++i) {
                Assert.AreEqual (comments[i], versionComments[i], "Comments in file version history log were not accurate.");
                Assert.That (versionDateTimes[i] >= beforeDateTime && versionDateTimes[i] <= afterDateTime, "Timestamp in file version history log is incorrect.");
            }

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            string[] comments;
            DateTime[] whenDateTime;

            _mfsOperations.File.GetVersionHistoryLog (0, out comments, out whenDateTime);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            string[] comments;
            DateTime[] whenDateTime;

            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetVersionHistoryLog (veryLargeFileID, out comments, out whenDateTime);
        }
    }

    [TestFixture]
    public class Tests_FileConflict : TestMfsOperationsBase {
        [Test]
        [ExpectedException (typeof (MfsFileVersionConflictException))]
        public void Test_Scenario () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Last file version number and its data stream are retrieved by user1:
            int lastVersionNumber_user1;
            byte[] fileData_user1 = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber_user1);

            // Simulateneously, user2 also decides to retrieve the last file version:
            int lastVersionNumber_user2;
            byte[] fileData_user2 = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber_user2);

            // User2 makes changes and commits those changes to the repository first.
            // Assuming that the new file data (document) is the modified version of the old file:
            byte[] fileDataModified_user2 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            // User2 also adds a comment to the new version:
            string comments_user2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // And saves it to the repository. Since no new version has been checked in by any other user,
            // we know for sure that the new version number should be one.
            int nextVersionNumber_user2 = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified_user2, comments_user2, lastVersionNumber_user2);
            Assert.AreEqual (1, nextVersionNumber_user2, "First modified version number was not one.");

            // Next, user1 also tries to save his locally cached (modified) file as next version, but
            // the system should not accept it, since user2 has already checked in his changes.
            // Assuming that the new file data (document) is the modified version of the old file:
            byte[] fileDataModified_user1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            // User1 also adds a comment to the new version:
            string comments_user1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // And tries to save it to the repository. Since a new version has already been checked in by user2,
            // the system should throw an exception.
            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified_user1, comments_user1, lastVersionNumber_user1);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_FileMerge : TestMfsOperationsBase {
        [Test]
        public void Test_UseCase () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // Last file version number and its data stream are retrieved by user1:
            int lastVersionNumber_user1;
            byte[] fileData_user1 = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber_user1);

            // Simulateneously, user2 also decides to retrieve the last file version:
            int lastVersionNumber_user2;
            byte[] fileData_user2 = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber_user2);

            // User2 makes changes and commits those changes to the repository first.
            // Assuming that the new file data (document) is the modified version of the old file:
            byte[] fileDataModified_user2 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            // User2 also adds a comment to the new version:
            string comments_user2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // And saves it to the repository. Since no new version has been checked in by any other user,
            // we know for sure that the new version number should be one.
            int nextVersionNumber_user2 = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified_user2, comments_user2, lastVersionNumber_user2);
            Assert.AreEqual (1, nextVersionNumber_user2, "First modified version number was not one.");

            // Next, user1 also tries to save his locally cached (modified) file as next version, but
            // the system should not accept it, since user2 has already checked in his changes.
            // Assuming that the new file data (document) is the modified version of the old file:
            byte[] fileDataModified_user1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            // User1 also adds a comment to the new version:
            string comments_user1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // And tries to save it to the repository. Since a new version has already been checked in by user2,
            // the system should throw an exception.
            try {
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified_user1, comments_user1, lastVersionNumber_user1);
            } catch (MfsFileVersionConflictException) {
                // User1 is informed that there is a conflict.
            }

            // User1 now checks out the last version:
            fileData_user1 = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber_user1);
            Assert.AreEqual (1, lastVersionNumber_user1, "Checked out file does not have version number as expected.");

            // User1 next merges the changes, and tries to save the file again.
            // Assuming that the new file data (document) is the merged version:
            fileDataModified_user1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int nextVersionNumber_user1 = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified_user2, comments_user2, lastVersionNumber_user1);
            Assert.AreEqual (2, nextVersionNumber_user1, "Second modified version number was not two.");

            // We also check to see if the system returns this as the next (head) version number:
            int lastVersionNumber = _mfsOperations.File.GetLastVersionNumber (fileID);
            Assert.AreEqual (2, lastVersionNumber, "Last version number retrieved was not as expected.");

            _mfsOperations.File.Delete (fileID);
        }
    }

    [TestFixture]
    public class Tests_GetVersionDiff : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // We next save some versions of the original file.
            int numVersionsToSave = TYPICAL_MULTI_VALUE;
            int lastVersionNumber = 0;

            for (int i = 0; i < numVersionsToSave; ++i) {
                // Last file version number and its data stream are retrieved:
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

                // User next makes some modifications to the locally cached file.
                // Assuming that the new file data (document) is the modified version of the old file:
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

                // User also adds a comment to the new version:
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

                // And saves it to the repository:
                lastVersionNumber = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            Assert.AreEqual (
                numVersionsToSave, lastVersionNumber,
                string.Format ("Last file version number retrieved should have been {0}.", numVersionsToSave)
                );

            // We next attempt to get file diff for two versions:
            byte[] fileData1;
            byte[] fileData2;

            int versionNumber1 = numVersionsToSave / 2;
            int versionNumber2 = numVersionsToSave;

            _mfsOperations.File.GetVersionDiff (fileID, versionNumber1, versionNumber2, out fileData1, out fileData2);

            // Now the client has the responsibility to invoke the associated app at its end to get a diff of the two versions.

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            int versionNumber1 = 0;
            int versionNumber2 = 1;
            byte[] fileData1;
            byte[] fileData2;

            _mfsOperations.File.GetVersionDiff (0, versionNumber1, versionNumber2, out fileData1, out fileData2);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            int versionNumber1 = 0;
            int versionNumber2 = 1;
            byte[] fileData1;
            byte[] fileData2;

            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetVersionDiff (veryLargeFileID, versionNumber1, versionNumber2, out fileData1, out fileData2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SameVersionNumbers_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int versionNumber1 = 0;
            int versionNumber2 = versionNumber1;
            byte[] fileData1;
            byte[] fileData2;

            try {
                _mfsOperations.File.GetVersionDiff (fileID, versionNumber1, versionNumber2, out fileData1, out fileData2);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FirstVersionNumberNegative_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numVersionsToSave = TYPICAL_MULTI_VALUE;

            for (int i = 0; i < numVersionsToSave; ++i) {
                int lastVersionNumber;
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            int someNegativeIllegalVersionNumber = -1;
            int versionNumber2 = 1;
            byte[] fileData1;
            byte[] fileData2;

            try {
                _mfsOperations.File.GetVersionDiff (fileID, someNegativeIllegalVersionNumber, versionNumber2, out fileData1, out fileData2);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFirstVersionNumber_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numVersionsToSave = TYPICAL_MULTI_VALUE;

            for (int i = 0; i < numVersionsToSave; ++i) {
                int lastVersionNumber;
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            int veryLargeVersionNumber = Int32.MaxValue;
            int versionNumber2 = 1;
            byte[] fileData1;
            byte[] fileData2;

            try {
                _mfsOperations.File.GetVersionDiff (fileID, veryLargeVersionNumber, versionNumber2, out fileData1, out fileData2);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SecondVersionNumberNegative_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numVersionsToSave = TYPICAL_MULTI_VALUE;

            for (int i = 0; i < numVersionsToSave; ++i) {
                int lastVersionNumber;
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            int versionNumber1 = 1;
            int someNegativeIllegalVersionNumber = -1;
            byte[] fileData1;
            byte[] fileData2;

            try {
                _mfsOperations.File.GetVersionDiff (fileID, versionNumber1, someNegativeIllegalVersionNumber, out fileData1, out fileData2);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentSecondVersionNumber_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numVersionsToSave = TYPICAL_MULTI_VALUE;

            for (int i = 0; i < numVersionsToSave; ++i) {
                int lastVersionNumber;
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            int versionNumber1 = 1;
            int veryLargeVersionNumber = Int32.MaxValue;
            byte[] fileData1;
            byte[] fileData2;

            try {
                _mfsOperations.File.GetVersionDiff (fileID, versionNumber1, veryLargeVersionNumber, out fileData1, out fileData2);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_GetLastFileVersionNumber : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // File is first saved:
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            // We next save some versions of the original file.
            int numVersionsToSave = TYPICAL_MULTI_VALUE;
            int lastVersionNumber = 0;

            for (int i = 0; i < numVersionsToSave; ++i) {
                // Last file version number and its data stream are retrieved:
                byte[] fileData = _mfsOperations.File.RetrieveLastVersion (fileID, out lastVersionNumber);

                // User next makes some modifications to the locally cached file.
                // Assuming that the new file data (document) is the modified version of the old file:
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

                // User also adds a comment to the new version:
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

                // And saves it to the repository:
                lastVersionNumber = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, lastVersionNumber);
            }

            int lastVersion = _mfsOperations.File.GetLastVersionNumber (fileID);
            Assert.AreEqual (lastVersionNumber, lastVersion, "Last file version number is not as expected.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.GetLastVersionNumber (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetLastVersionNumber (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_DeleteFile : TestMfsOperationsBase {
        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_ShouldDeleteAllVersionsOfFile_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numVersionsToSave = 10;
            for (int i = 0; i < numVersionsToSave; ++i) {
                int currentVersionNumber = _mfsOperations.File.GetLastVersionNumber (fileID);

                // Make some modifications to current file.
                // Assuming that the new file dat object has the modified version of the old file:
                byte[] fileDataModified = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                // Add a comment to the new version:
                string comments = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

                int versionNumber = _mfsOperations.File.SaveAsNextVersion (fileID, fileDataModified, comments, currentVersionNumber);
            }

            _mfsOperations.File.Delete (fileID);

            // This line will throw an exception:
            _mfsOperations.File.GetLastVersionNumber (fileID);
        }
    }
}
