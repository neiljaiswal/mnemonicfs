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
using System.Threading;

namespace MnemonicFS.Tests.FileSaveRetrieval {
    [TestFixture]
    public class Tests_MfsOperations_ObjectConstruction : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // _mfsOperations is initialized with instantiated class object during Setup(). So no need
            // to re-create the object again.
            Assert.IsNotNull (_mfsOperations, "MfsOperations object was not created, was null.");
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_SaveFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            Assert.That (fileID > 0, "Returned zero as file id.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_SanityTest_PositiveIntegerReturnValue () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Returned zero as file id.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_CurrentDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (when.Year == dateTime.Year &&
                         when.Month == dateTime.Month &&
                         when.Day == dateTime.Day &&
                         when.Hour == dateTime.Hour &&
                         when.Minute == dateTime.Minute &&
                         when.Second == dateTime.Second,
                         "Incorrect timestamp applied to saved file.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_BackDate1Year_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_BackDate1Month_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddMonths (-1);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_BackDate1Day_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddDays (-1);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_BackDateTime1Hour_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddHours (-1);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Hour == when.Hour, "Hour saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Minute == when.Minute, "Minute saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Second == when.Second, "Second saved for file during backdate operation was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_BackDateTime1Minute_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddMinutes (-1);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Hour == when.Hour, "Hour saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Minute == when.Minute, "Minute saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Second == when.Second, "Second saved for file during backdate operation was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_BackDateTime1Second_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddSeconds (-1);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Hour == when.Hour, "Hour saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Minute == when.Minute, "Minute saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Second == when.Second, "Second saved for file during backdate operation was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string nullFileName = null;

            SaveFileToMfs (ref _mfsOperations, nullFileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string emptyFileName = string.Empty;

            SaveFileToMfs (ref _mfsOperations, emptyFileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        public void Test_FileNameWithMaxSizeAllowed_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileName = TestUtils.GetAWord (MfsOperations.MaxFileNameLength);

            ulong fileID = SaveFileToMfs (ref _mfsOperations, longFileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Failed to save file though file name is exactly of system-defined size.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNameGreaterThanMaxSizeAllowed_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileName = TestUtils.GetAWord (MfsOperations.MaxFileNameLength + 1);

            SaveFileToMfs (ref _mfsOperations, longFileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullFileNarration_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string nullFileNarration = null;

            SaveFileToMfs (ref _mfsOperations, _fileName, nullFileNarration, _fileData, when, false);
        }

        [Test]
        public void Test_EmptyFileNarration_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string emptyFileNarration = string.Empty;

            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, emptyFileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Failed to save file though empty file narration is allowed.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_FileNarrationWithMaxSizeAllowed_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileNarration = TestUtils.GetAWord (MfsOperations.MaxFileNarrationLength);

            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, longFileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Failed to save file though file narration is exactly of defined size.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNarrationGreaterThanMaxSizeAllowed_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileNarration = TestUtils.GetAWord (MfsOperations.MaxFileNarrationLength + 1);

            SaveFileToMfs (ref _mfsOperations, _fileName, longFileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_NullFileData_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            byte[] nullFileData = null;

            SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, nullFileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_ZeroSizeFileData_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.ZERO_FILE_SIZE);
            DateTime when = DateTime.Now;

            SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileDataGreaterThanMaxSizeAllowed_Illegal () {
            Assert.That (true, "This test will have to be taken on faith, since it may not be possible to create file data as large as the limit defined.");

            throw new MfsIllegalArgumentException ("Deliberate throw to pass this test.");
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_SaveFile_FileNameWithIllegalChars : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            Assert.That (fileID > 0, "File not saved even though name is legal.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_SanityCheck_IllegalChar () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string fileNameWithIllegalChar = TestUtils.GetAnyFileName () + "?";

            SaveFileToMfs (ref _mfsOperations, fileNameWithIllegalChar, _fileNarration, _fileData, when, false);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DeleteFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            bool doesFileExist = _mfsOperations.File.Exists (fileID);
            Assert.IsTrue (doesFileExist, "Showing a saved file as not existing.");

            _mfsOperations.File.Delete (fileID);

            doesFileExist = _mfsOperations.File.Exists (fileID);
            Assert.IsFalse (doesFileExist, "Showing a non-existent file as existing.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.Delete (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.Delete (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DoesFileExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            bool doesFileExist = _mfsOperations.File.Exists (fileID);
            Assert.IsTrue (doesFileExist, "Returned file status as does not exist even though it does.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        public void Test_NonExistentFileID_SanityCheck_NotExists () {
            ulong veryLargeFileID = UInt64.MaxValue;

            bool fileExists = _mfsOperations.File.Exists (veryLargeFileID);
            Assert.IsFalse (fileExists, "Returned file status as exists even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.Exists (0);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string fileName = _mfsOperations.File.GetName (fileID);
            Assert.AreEqual (_fileName, fileName, "Failed to get actual file name.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.GetName (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetName (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileNarration : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string fileNarration = _mfsOperations.File.GetNarration (fileID);
            Assert.AreEqual (_fileNarration, fileNarration, "Failed to get actual file narration.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.GetNarration (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetNarration (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileSize : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int fileSize = _mfsOperations.File.GetSize (fileID);
            Assert.AreEqual (_fileData.Length, fileSize, "Failed to get actual file size.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.GetSize (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetSize (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileSaveDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (when.Year == dateTime.Year &&
                         when.Month == dateTime.Month &&
                         when.Day == dateTime.Day &&
                         when.Hour == dateTime.Hour &&
                         when.Minute == dateTime.Minute &&
                         when.Second == dateTime.Second,
                         "Datetime stamp returned was incorrect.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.GetSaveDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetSaveDateTime (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDateTime = _mfsOperations.File.GetDeletionDateTime (fileID);
            Assert.AreEqual (DateTime.MaxValue, deletionDateTime, "Deletion date-time do not default to max deletion time.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.GetDeletionDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.GetDeletionDateTime (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_RetrieveFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            byte[] fileData = _mfsOperations.File.RetrieveOriginal (fileID);
            Assert.AreEqual (_fileData, fileData, "File data returned after saving is not as expected.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.File.RetrieveOriginal (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.File.RetrieveOriginal (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_SetFileDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityTest () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDate = DateTime.Now.AddYears (TYPICAL_MULTI_TIME_UNIT);
            _mfsOperations.File.SetDeletionDateTime (fileID, deletionDate);

            DateTime deletionDateRetrieved = _mfsOperations.File.GetDeletionDateTime (fileID);
            Assert.That (deletionDateRetrieved.Year == deletionDate.Year &&
                         deletionDateRetrieved.Month == deletionDate.Month &&
                         deletionDateRetrieved.Day == deletionDate.Day,
                         "Incorrect deletion date-time retrieved.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime anyDeletionDateInTheFuture = new DateTime (DateTime.Now.Year + TYPICAL_MULTI_TIME_UNIT, DateTime.Now.Month, DateTime.Now.Day);

            _mfsOperations.File.SetDeletionDateTime (0, anyDeletionDateInTheFuture);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime anyDeletionDateTimeInTheFuture = DateTime.Now.AddYears (TYPICAL_MULTI_TIME_UNIT);

            _mfsOperations.File.SetDeletionDateTime (veryLargeFileID, anyDeletionDateTimeInTheFuture);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_DeletionDateTimeLesserThanFileSaveDate_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (TYPICAL_MULTI_TIME_UNIT);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDate = DateTime.Now.AddYears (3);

            try {
                _mfsOperations.File.SetDeletionDateTime (fileID, deletionDate);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_DeletionDateTimeLesserThanCurrentDateTime_FileSaveDateInPastFromCurrentDateTime_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-TYPICAL_MULTI_TIME_UNIT);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDate = DateTime.Now.AddYears (-TYPICAL_MULTI_TIME_UNIT);

            try {
                _mfsOperations.File.SetDeletionDateTime (fileID, deletionDate);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string newName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            bool isUpdated = _mfsOperations.File.UpdateName (fileID, newName);
            Assert.IsTrue (isUpdated, "File narration was not updated successfully.");

            string fileName = _mfsOperations.File.GetName (fileID);
            Assert.AreEqual (newName, fileName, "File name was not updated successfully.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.File.UpdateName (0, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.File.UpdateName (veryLargeFileID, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string nullNewName = null;

            try {
                _mfsOperations.File.UpdateName (fileID, nullNewName);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNameGreaterThanSystemDefined_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string longFileName = TestUtils.GetAWord (MfsOperations.MaxFileNameLength + 1);

            try {
                _mfsOperations.File.UpdateName (fileID, longFileName);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string emptyFileName = string.Empty;

            try {
                _mfsOperations.File.UpdateName (fileID, emptyFileName);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileNarration : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string newNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            bool isUpdated = _mfsOperations.File.UpdateNarration (fileID, newNarration);
            Assert.IsTrue (isUpdated, "File narration was not updated successfully.");

            string fileNarration = _mfsOperations.File.GetNarration (fileID);
            Assert.AreEqual (newNarration, fileNarration, "File narration was not updated successfully.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            string anyNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.File.UpdateNarration (0, anyNarration);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            string anyNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.File.UpdateNarration (veryLargeFileID, anyNarration);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewFileNarration_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string nullNewNarration = null;

            try {
                _mfsOperations.File.UpdateNarration (fileID, nullNewNarration);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }

        [Test]
        public void Test_EmptyNewFileNarration_Legal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string emptyFileNarration = string.Empty;
            _mfsOperations.File.UpdateNarration (fileID, emptyFileNarration);

            string fileNarration = _mfsOperations.File.GetNarration (fileID);
            Assert.AreEqual (emptyFileNarration, fileNarration, "File narration was not updated to an empty string.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNarrationGreaterThanSystemDefined_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            string longFileDesc = TestUtils.GetAWord (MfsOperations.MaxFileNarrationLength + 1);

            try {
                _mfsOperations.File.UpdateNarration (fileID, longFileDesc);
            } finally {
                _mfsOperations.File.Delete (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileSaveDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime newWhen = new DateTime (when.Year - 1, when.Month, when.Day);
            bool isUpdated = _mfsOperations.File.UpdateSaveDateTime (fileID, newWhen);
            Assert.IsTrue (isUpdated, "File save date-time was not updated successfully.");

            DateTime whenRetr = _mfsOperations.File.GetSaveDateTime (fileID);
            Assert.That (newWhen.Year == whenRetr.Year &&
                         newWhen.Month == whenRetr.Month &&
                         newWhen.Day == whenRetr.Day &&
                         newWhen.Hour == whenRetr.Hour &&
                         newWhen.Minute == whenRetr.Minute &&
                         newWhen.Second == whenRetr.Second,
                         "File save date-time was not updated successfully.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime newWhen = DateTime.Now;

            _mfsOperations.File.UpdateSaveDateTime (0, newWhen);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime newWhen = DateTime.Now;

            _mfsOperations.File.UpdateSaveDateTime (veryLargeFileID, newWhen);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime newDeletionDateTime = DateTime.Now;
            bool isUpdated = _mfsOperations.File.UpdateDeletionDateTime (fileID, newDeletionDateTime);
            Assert.IsTrue (isUpdated, "File deletion date-time was not updated successfully.");

            DateTime deletionRetr = _mfsOperations.File.GetDeletionDateTime (fileID);
            Assert.That (newDeletionDateTime.Year == deletionRetr.Year &&
                         newDeletionDateTime.Month == deletionRetr.Month &&
                         newDeletionDateTime.Day == deletionRetr.Day &&
                         newDeletionDateTime.Hour == deletionRetr.Hour &&
                         newDeletionDateTime.Minute == deletionRetr.Minute &&
                         newDeletionDateTime.Second == deletionRetr.Second,
                         "File deletion date-time was not updated successfully.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime newWhen = DateTime.Now;

            _mfsOperations.File.UpdateDeletionDateTime (0, newWhen);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime newWhen = DateTime.Now;

            _mfsOperations.File.UpdateDeletionDateTime (veryLargeFileID, newWhen);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_ResetDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            DateTime newDeletionDateTime = DateTime.Now;
            bool isUpdated = _mfsOperations.File.UpdateDeletionDateTime (fileID, newDeletionDateTime);
            Assert.IsTrue (isUpdated, "File deletion date-time was not updated successfully.");

            _mfsOperations.File.ResetDeletionDateTime (fileID);

            DateTime deletionRetr = _mfsOperations.File.GetDeletionDateTime (fileID);
            Assert.That (DateTime.MaxValue.Year == deletionRetr.Year &&
                         DateTime.MaxValue.Month == deletionRetr.Month &&
                         DateTime.MaxValue.Day == deletionRetr.Day &&
                         DateTime.MaxValue.Hour == deletionRetr.Hour &&
                         DateTime.MaxValue.Minute == deletionRetr.Minute &&
                         DateTime.MaxValue.Second == deletionRetr.Second,
                         "File deletion date-time was not been reset successfully.");

            _mfsOperations.File.Delete (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime newWhen = DateTime.Now;

            _mfsOperations.File.ResetDeletionDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime newWhen = DateTime.Now;

            _mfsOperations.File.ResetDeletionDateTime (veryLargeFileID);
        }
    }
}
