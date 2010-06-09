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
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Returned zero as file id.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_PositiveIntegerReturnValue_SanityTest () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Returned zero as file id.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_CurrentDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (when.Year == dateTime.Year &&
                         when.Month == dateTime.Month &&
                         when.Day == dateTime.Day &&
                         when.Hour == dateTime.Hour &&
                         when.Minute == dateTime.Minute &&
                         when.Second == dateTime.Second,
                         "Incorrect timestamp applied to saved file.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_BackDate1Year_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_BackDate1Month_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddMonths (-1);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_BackDate1Day_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddDays (-1);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_BackDateTime1Hour_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddHours (-1);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Hour == when.Hour, "Hour saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Minute == when.Minute, "Minute saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Second == when.Second, "Second saved for file during backdate operation was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_BackDateTime1Minute_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddMinutes (-1);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Hour == when.Hour, "Hour saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Minute == when.Minute, "Minute saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Second == when.Second, "Second saved for file during backdate operation was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_BackDateTime1Second_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddSeconds (-1);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (dateTime.Year == when.Year, "Year saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Month == when.Month, "Month saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Day == when.Day, "Day saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Hour == when.Hour, "Hour saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Minute == when.Minute, "Minute saved for file during backdate operation was incorrect.");
            Assert.That (dateTime.Second == when.Second, "Second saved for file during backdate operation was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string nullFileName = null;

            _mfsOperations.SaveFile (nullFileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string emptyFileName = string.Empty;

            _mfsOperations.SaveFile (emptyFileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        public void Test_FileNameWithMaxSizeAllowed_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileName = TestUtils.GetAWord (MfsOperations.MaxFileNameLength);

            ulong fileID = _mfsOperations.SaveFile (longFileName, _fileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Failed to save file though file name is exactly of system-defined size.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNameGreaterThanMaxSizeAllowed_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileName = TestUtils.GetAWord (MfsOperations.MaxFileNameLength + 1);

            _mfsOperations.SaveFile (longFileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullFileNarration_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string nullFileDesc = null;

            _mfsOperations.SaveFile (_fileName, nullFileDesc, _fileData, when, false);
        }

        [Test]
        public void Test_EmptyFileNarration_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string emptyFileNarration = string.Empty;

            ulong fileID = _mfsOperations.SaveFile (_fileName, emptyFileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Failed to save file though empty file narration is allowed.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_FileNarrationWithMaxSizeAllowed_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileNarration = TestUtils.GetAWord (MfsOperations.MaxFileNarrationLength);

            ulong fileID = _mfsOperations.SaveFile (_fileName, longFileNarration, _fileData, when, false);
            Assert.That (fileID > 0, "Failed to save file though file narration is exactly of defined size.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNarrationGreaterThanMaxSizeAllowed_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string longFileNarration = TestUtils.GetAWord (MfsOperations.MaxFileNarrationLength + 1);

            _mfsOperations.SaveFile (_fileName, longFileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_NullFileData_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            byte[] nullFileData = null;

            _mfsOperations.SaveFile (_fileName, _fileNarration, nullFileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsFileDataException))]
        public void Test_ZeroSizeFileData_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.ZERO_FILE_SIZE);
            DateTime when = DateTime.Now;

            _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileDataGreaterThanMaxSizeAllowed_Illegal () {
            Assert.That (true, "This test will have to be taken on faith, since it may not be possible to create file data as large as the limit defined.");

            throw new MfsIllegalArgumentException ("Deliberate throw to pass this test.");
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DeleteFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            bool doesFileExist = _mfsOperations.DoesFileExist (fileID);
            Assert.IsTrue (doesFileExist, "Showing a saved file as not existing.");

            _mfsOperations.DeleteFile (fileID);

            doesFileExist = _mfsOperations.DoesFileExist (fileID);
            Assert.IsFalse (doesFileExist, "Showing a non-existent file as existing.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.DeleteFile (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.DeleteFile (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DoesFileExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            bool doesFileExist = _mfsOperations.DoesFileExist (fileID);
            Assert.IsTrue (doesFileExist, "Returned file status as does not exist even though it does.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_NonExistentFileID_SanityCheck_NotExists () {
            ulong veryLargeFileID = UInt64.MaxValue;

            bool fileExists = _mfsOperations.DoesFileExist (veryLargeFileID);
            Assert.IsFalse (fileExists, "Returned file status as exists even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.DoesFileExist (0);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string fileName = _mfsOperations.GetFileName (fileID);
            Assert.AreEqual (_fileName, fileName, "Failed to get actual file name.");
            
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileName (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileName (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileNarration : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string fileNarration = _mfsOperations.GetFileNarration (fileID);
            Assert.AreEqual (_fileNarration, fileNarration, "Failed to get actual file narration.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileNarration (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileNarration (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileSize : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            int fileSize = _mfsOperations.GetFileSize (fileID);
            Assert.AreEqual (_fileData.Length, fileSize, "Failed to get actual file size.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileSize (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileSize (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileSaveDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime dateTime = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (when.Year == dateTime.Year &&
                         when.Month == dateTime.Month &&
                         when.Day == dateTime.Day &&
                         when.Hour == dateTime.Hour &&
                         when.Minute == dateTime.Minute &&
                         when.Second == dateTime.Second,
                         "Datetime stamp returned was incorrect.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileSaveDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileSaveDateTime (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetFileDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDateTime = _mfsOperations.GetFileDeletionDateTime (fileID);
            Assert.AreEqual (DateTime.MaxValue, deletionDateTime, "Deletion date-time do not default to max deletion time.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetFileDeletionDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetFileDeletionDateTime (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_RetrieveFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            byte[] fileData = _mfsOperations.RetrieveOriginalFile (fileID);
            Assert.AreEqual (_fileData, fileData, "File data returned after saving is not as expected.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.RetrieveOriginalFile (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.RetrieveOriginalFile (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_SetFileDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityTest () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDate = DateTime.Now.AddYears (TYPICAL_MULTI_TIME_UNIT);
            _mfsOperations.SetFileDeletionDateTime (fileID, deletionDate);

            DateTime deletionDateRetrieved = _mfsOperations.GetFileDeletionDateTime (fileID);
            Assert.That (deletionDateRetrieved.Year == deletionDate.Year &&
                         deletionDateRetrieved.Month == deletionDate.Month &&
                         deletionDateRetrieved.Day == deletionDate.Day,
                         "Incorrect deletion date-time retrieved.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime anyDeletionDateInTheFuture = new DateTime (DateTime.Now.Year + TYPICAL_MULTI_TIME_UNIT, DateTime.Now.Month, DateTime.Now.Day);

            _mfsOperations.SetFileDeletionDateTime (0, anyDeletionDateInTheFuture);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime anyDeletionDateTimeInTheFuture = DateTime.Now.AddYears (TYPICAL_MULTI_TIME_UNIT);

            _mfsOperations.SetFileDeletionDateTime (veryLargeFileID, anyDeletionDateTimeInTheFuture);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_DeletionDateTimeLesserThanFileSaveDate_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (TYPICAL_MULTI_TIME_UNIT);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDate = DateTime.Now.AddYears (3);

            try {
                _mfsOperations.SetFileDeletionDateTime (fileID, deletionDate);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_DeletionDateTimeLesserThanCurrentDateTime_FileSaveDateInPastFromCurrentDateTime_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-TYPICAL_MULTI_TIME_UNIT);
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime deletionDate = DateTime.Now.AddYears (-TYPICAL_MULTI_TIME_UNIT);

            try {
                _mfsOperations.SetFileDeletionDateTime (fileID, deletionDate);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string newName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            bool isUpdated = _mfsOperations.UpdateFileName (fileID, newName);
            Assert.IsTrue (isUpdated, "File narration was not updated successfully.");

            string fileName = _mfsOperations.GetFileName (fileID);
            Assert.AreEqual (newName, fileName, "File name was not updated successfully.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateFileName (0, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateFileName (veryLargeFileID, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string nullNewName = null;

            try {
                _mfsOperations.UpdateFileName (fileID, nullNewName);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNameGreaterThanSystemDefined_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string longFileName = TestUtils.GetAWord (MfsOperations.MaxFileNameLength + 1);

            try {
                _mfsOperations.UpdateFileName (fileID, longFileName);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewFileName_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string emptyFileName = string.Empty;

            try {
                _mfsOperations.UpdateFileName (fileID, emptyFileName);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileNarration : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string newNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            bool isUpdated = _mfsOperations.UpdateFileNarration (fileID, newNarration);
            Assert.IsTrue (isUpdated, "File narration was not updated successfully.");

            string fileNarration = _mfsOperations.GetFileNarration (fileID);
            Assert.AreEqual (newNarration, fileNarration, "File narration was not updated successfully.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            string anyNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateFileNarration (0, anyNarration);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            string anyNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateFileNarration (veryLargeFileID, anyNarration);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewFileNarration_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string nullNewNarration = null;

            try {
                _mfsOperations.UpdateFileNarration (fileID, nullNewNarration);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        public void Test_EmptyNewFileNarration_Legal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string emptyFileNarration = string.Empty;
            _mfsOperations.UpdateFileNarration (fileID, emptyFileNarration);

            string fileNarration = _mfsOperations.GetFileNarration (fileID);
            Assert.AreEqual (emptyFileNarration, fileNarration, "File narration was not updated to an empty string.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileNarrationGreaterThanSystemDefined_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            string longFileDesc = TestUtils.GetAWord (MfsOperations.MaxFileNarrationLength + 1);

            try {
                _mfsOperations.UpdateFileNarration (fileID, longFileDesc);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileSaveDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime newWhen = new DateTime (when.Year - 1, when.Month, when.Day);
            bool isUpdated = _mfsOperations.UpdateFileSaveDateTime (fileID, newWhen);
            Assert.IsTrue (isUpdated, "File save date-time was not updated successfully.");

            DateTime whenRetr = _mfsOperations.GetFileSaveDateTime (fileID);
            Assert.That (newWhen.Year == whenRetr.Year &&
                         newWhen.Month == whenRetr.Month &&
                         newWhen.Day == whenRetr.Day &&
                         newWhen.Hour == whenRetr.Hour &&
                         newWhen.Minute == whenRetr.Minute &&
                         newWhen.Second == whenRetr.Second,
                         "File save date-time was not updated successfully.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime newWhen = DateTime.Now;

            _mfsOperations.UpdateFileSaveDateTime (0, newWhen);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime newWhen = DateTime.Now;

            _mfsOperations.UpdateFileSaveDateTime (veryLargeFileID, newWhen);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_UpdateFileDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime newDeletionDateTime = DateTime.Now;
            bool isUpdated = _mfsOperations.UpdateFileDeletionDateTime (fileID, newDeletionDateTime);
            Assert.IsTrue (isUpdated, "File deletion date-time was not updated successfully.");

            DateTime deletionRetr = _mfsOperations.GetFileDeletionDateTime (fileID);
            Assert.That (newDeletionDateTime.Year == deletionRetr.Year &&
                         newDeletionDateTime.Month == deletionRetr.Month &&
                         newDeletionDateTime.Day == deletionRetr.Day &&
                         newDeletionDateTime.Hour == deletionRetr.Hour &&
                         newDeletionDateTime.Minute == deletionRetr.Minute &&
                         newDeletionDateTime.Second == deletionRetr.Second,
                         "File deletion date-time was not updated successfully.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime newWhen = DateTime.Now;

            _mfsOperations.UpdateFileDeletionDateTime (0, newWhen);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime newWhen = DateTime.Now;

            _mfsOperations.UpdateFileDeletionDateTime (veryLargeFileID, newWhen);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_ResetDeletionDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, when, false);

            DateTime newDeletionDateTime = DateTime.Now;
            bool isUpdated = _mfsOperations.UpdateFileDeletionDateTime (fileID, newDeletionDateTime);
            Assert.IsTrue (isUpdated, "File deletion date-time was not updated successfully.");

            _mfsOperations.ResetDeletionDateTime (fileID);

            DateTime deletionRetr = _mfsOperations.GetFileDeletionDateTime (fileID);
            Assert.That (DateTime.MaxValue.Year == deletionRetr.Year &&
                         DateTime.MaxValue.Month == deletionRetr.Month &&
                         DateTime.MaxValue.Day == deletionRetr.Day &&
                         DateTime.MaxValue.Hour == deletionRetr.Hour &&
                         DateTime.MaxValue.Minute == deletionRetr.Minute &&
                         DateTime.MaxValue.Second == deletionRetr.Second,
                         "File deletion date-time was not been reset successfully.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            DateTime newWhen = DateTime.Now;

            _mfsOperations.ResetDeletionDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            DateTime newWhen = DateTime.Now;

            _mfsOperations.ResetDeletionDateTime (veryLargeFileID);
        }
    }

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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            List<ulong> allFileIDs = _mfsOperations.GetAllFiles ();
            Assert.AreEqual (TYPICAL_MULTI_VALUE, allFileIDs.Count, "Incorrect number of files retrieved.");

            fileIDs.Sort ();
            allFileIDs.Sort ();

            for (int i = 0; i < fileIDs.Count; ++i) {
                Assert.AreEqual (fileIDs[i], allFileIDs[i], "Incorrect file retrieved.");
            }

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            DateTime dt1 = DateTime.Now.AddYears (-1).AddDays (-1);
            DateTime dt2 = DateTime.Now.AddYears (1).AddDays (1);
            List<ulong> filesInDateRange = _mfsOperations.GetFilesInDateRange (dt1, dt2);

            Assert.AreEqual (3, filesInDateRange.Count, "Number of files returned in date range are not as expected.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            const int BUFFER_SECONDS_FOR_TEST_TO_RUN = TYPICAL_MULTI_TIME_UNIT; // seconds
            DateTime dt1 = DateTime.Now.AddHours (-1).AddSeconds (-BUFFER_SECONDS_FOR_TEST_TO_RUN);
            DateTime dt2 = DateTime.Now.AddHours (1).AddSeconds (BUFFER_SECONDS_FOR_TEST_TO_RUN);

            List<ulong> filesInDateTimeRange = _mfsOperations.GetFilesInDateTimeRange (dt1, dt2);

            Assert.AreEqual (3, filesInDateTimeRange.Count, "Number of files returned in date-time range are not as expected.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        public void Test_GetFilesOnDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime now = DateTime.Now;

            DateTime whenBefore = DateTime.Now.AddDays (-1);
            ulong fileIDBefore = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, whenBefore, false);

            DateTime whenOn = DateTime.Now;
            ulong fileIDOn = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, whenOn, false);

            DateTime whenAfter = DateTime.Now.AddDays (1);
            ulong fileIDAfter = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, whenAfter, false);

            List<ulong> filesOnDate = _mfsOperations.GetFilesOnDate (whenOn);
            Assert.AreEqual (1, filesOnDate.Count, "Number of files returned on date are not as expected.");

            _mfsOperations.DeleteFile (fileIDBefore);
            _mfsOperations.DeleteFile (fileIDOn);
            _mfsOperations.DeleteFile (fileIDAfter);
        }

        [Test]
        public void Test_GetFilesOnDateTime_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime now = DateTime.Now;

            DateTime whenBefore = DateTime.Now.AddSeconds (-TYPICAL_MULTI_TIME_UNIT);
            ulong fileIDBefore = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, whenBefore, false);

            DateTime whenOn = DateTime.Now;
            ulong fileIDOn = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, whenOn, false);

            DateTime whenAfter = DateTime.Now.AddSeconds (TYPICAL_MULTI_TIME_UNIT);
            ulong fileIDAfter = _mfsOperations.SaveFile (_fileName, _fileNarration, _fileData, whenAfter, false);

            List<ulong> filesOnDate = _mfsOperations.GetFilesAtDateTime (whenOn);
            Assert.AreEqual (1, filesOnDate.Count, "Number of files returned on date-time are not as expected.");

            _mfsOperations.DeleteFile (fileIDBefore);
            _mfsOperations.DeleteFile (fileIDOn);
            _mfsOperations.DeleteFile (fileIDAfter);
        }

        [Test]
        public void Test_GetFilesBeforeDate_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);

            int numFilesToSave = TYPICAL_MULTI_VALUE;

            List<ulong> fileIDsBefore = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (-i - 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDate = _mfsOperations.GetFilesBeforeDate (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDate.Count, "Number of files returned before date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDateTime = _mfsOperations.GetFilesBeforeDateTime (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDateTime.Count, "Number of files returned before date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDate = _mfsOperations.GetFilesBeforeAndOnDate (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDate.Count, "Number of files returned before and on date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesBeforeDateTime = _mfsOperations.GetFilesBeforeAndAtDateTime (now);
            Assert.AreEqual (fileIDsBefore.Count, filesBeforeDateTime.Count, "Number of files returned before and on date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDate = _mfsOperations.GetFilesAfterDate (now);
            Assert.AreEqual (fileIDsAfter.Count, filesAfterDate.Count, "Number of files returned after date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDateTime = _mfsOperations.GetFilesAfterDateTime (now);
            Assert.AreEqual (fileIDsBefore.Count, filesAfterDateTime.Count, "Number of files returned after date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }
            
            DateTime now = DateTime.Now;

            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddYears (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }
            
            List<ulong> filesAfterDate = _mfsOperations.GetFilesAfterAndOnDate (now);
            Assert.AreEqual (fileIDsAfter.Count, filesAfterDate.Count, "Number of files returned after and on date are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
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
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsBefore.Add (fileID);
            }

            DateTime now = DateTime.Now;
            
            List<ulong> fileIDsAfter = new List<ulong> ();
            for (int i = 0; i < numFilesToSave; ++i) {
                string fileName = TestUtils.GetAnyFileName ();
                DateTime when = DateTime.Now.AddHours (i + 1);
                ulong fileID = _mfsOperations.SaveFile (fileName, _fileNarration, _fileData, when, false);

                fileIDsAfter.Add (fileID);
            }

            List<ulong> filesAfterDateTime = _mfsOperations.GetFilesAfterAndAtDateTime (now);
            Assert.AreEqual (fileIDsAfter.Count, filesAfterDateTime.Count, "Number of files returned after and on date-time are not as expected.");

            for (int i = 0; i < numFilesToSave; ++i) {
                _mfsOperations.DeleteFile (fileIDsBefore[i]);
                _mfsOperations.DeleteFile (fileIDsAfter[i]);
            }
        }
    }
    
    [TestFixture]
    public class Tests_MfsOperations_StoreByteStream : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
            Assert.That (byteStreamID > 0, "Returned byte stream id is not valid.");

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullByteArray_Illegal () {
            _fileData = null;
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyByteArray_Illegal () {
            _fileData = new byte[0];
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = null;
            int refNo = 1;

            MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = string.Empty;
            int refNo = 1;

            MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
        }

        [Test]
        public void Test_AnyReferenceNumber_Legal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
            Assert.That (byteStreamID > 0, "Byte stream not stored even though any reference number is allowed.");

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_RetrieveByteStream : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            int returnedRefNo;
            byte[] returnedStream = MfsOperations.RetrieveByteStream (byteStreamID, passphrase, out returnedRefNo);
            Assert.AreEqual (_fileData.Length, returnedStream.Length, "Size of data returned is not correct.");
            Assert.AreEqual (refNo, returnedRefNo, "Reference number returned is not correct.");

            for (int i = 0; i < _fileData.Length; ++i) {
                Assert.AreEqual (_fileData[i], returnedStream[i], "Returned stream is not identical to the saved stream.");
            }

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int returnedRefNo;
            MfsOperations.RetrieveByteStream (0, passphrase, out returnedRefNo);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentByteStreamID_Illegal () {
            ulong veryLargeByteStreamID = UInt64.MaxValue;
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int returnedRefNo;
            MfsOperations.RetrieveByteStream (veryLargeByteStreamID, passphrase, out returnedRefNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            int returnedRefNo;
            string nullPassphrase = null;
            try {
                MfsOperations.RetrieveByteStream (byteStreamID, nullPassphrase, out returnedRefNo);
            } finally {
                MfsOperations.DeleteByteStream (byteStreamID, passphrase);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            int returnedRefNo;
            string emptyPassphrase = string.Empty;
            try {
                MfsOperations.RetrieveByteStream (byteStreamID, emptyPassphrase, out returnedRefNo);
            } finally {
                MfsOperations.DeleteByteStream (byteStreamID, passphrase);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_GetByteStreamReferenceNumber : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            int returnedRefNo = MfsOperations.GetByteStreamReferenceNumber (byteStreamID);
            Assert.AreEqual (refNo, returnedRefNo, "Reference number returned is not correct.");

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            MfsOperations.GetByteStreamReferenceNumber (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentByteStreamID_Illegal () {
            ulong veryLargeByteStreamID = UInt64.MaxValue;
            MfsOperations.GetByteStreamReferenceNumber (veryLargeByteStreamID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DeleteByteStream : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);

            bool byteStreamExists = MfsOperations.DoesByteStreamExist (byteStreamID);
            Assert.IsFalse (byteStreamExists, "Byte stream not deleted properly.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            MfsOperations.DeleteByteStream (0, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentByteStreamID_Illegal () {
            ulong veryLargeByteStreamID = UInt64.MaxValue;
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            MfsOperations.DeleteByteStream (veryLargeByteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = null;
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);

            bool byteStreamExists = MfsOperations.DoesByteStreamExist (byteStreamID);
            Assert.IsFalse (byteStreamExists, "Byte stream not deleted properly even though null passphrase is allowed.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = null;
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            string emptyPassphrase = string.Empty;
            try {
                MfsOperations.DeleteByteStream (byteStreamID, emptyPassphrase);
            } finally {
                MfsOperations.DeleteByteStream (byteStreamID, passphrase);
            }
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DoesByteStreamExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            bool byteStreamExists = MfsOperations.DoesByteStreamExist (byteStreamID);
            Assert.IsTrue (byteStreamExists, "Shows byte stream does not exist even though it does.");

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);
        }

        [Test]
        public void Test_TrueWhenByteStreamExists () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);

            bool byteStreamExists = MfsOperations.DoesByteStreamExist (byteStreamID);
            Assert.IsTrue (byteStreamExists, "Shows byte stream does not exist even though it does.");

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);
        }

        [Test]
        public void Test_FalseWhenByteStreamDoesNotExist () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.StoreByteStream (_fileData, passphrase, refNo);
            bool byteStreamExists = MfsOperations.DoesByteStreamExist (byteStreamID);
            Assert.IsTrue (byteStreamExists, "Shows byte stream does not exist even though it does.");

            MfsOperations.DeleteByteStream (byteStreamID, passphrase);

            byteStreamExists = MfsOperations.DoesByteStreamExist (byteStreamID);
            Assert.IsFalse (byteStreamExists, "Shows byte stream exists even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            MfsOperations.DoesByteStreamExist (0);
        }
    }
}
