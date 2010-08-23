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
using Ionic.Zip;
using System.IO;
using MnemonicFS.MfsCore;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.Tests.Archiving {
    [TestFixture]
    public class Tests_ArchivingMethod_ArchiveFilesInGrouping : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID1);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID2);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID3);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, aspectID, outFilePath, archiveName, null);

            string opFileWithPath = outFilePath + archiveName;

            // Check to see if the o/p file exists:
            Assert.IsTrue (File.Exists (opFileWithPath), "File does not exist.");

            // Next check archive contents for file names:
            Assert.IsTrue (ZipFile.IsZipFile (opFileWithPath), "Output file is not a valid archive.");

            byte[] retrFileData1 = TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName1, null);
            Assert.AreEqual (fileData1, retrFileData1, "Retrieved file data is not as accurate.");

            byte[] retrFileData2 = TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName2, null);
            Assert.AreEqual (fileData2, retrFileData2, "Retrieved file data is not as accurate.");

            byte[] retrFileData3 = TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName3, null);
            Assert.AreEqual (fileData3, retrFileData3, "Retrieved file data is not as accurate.");

            File.Delete (opFileWithPath);
            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
            _mfsOperations.DeleteFile (fileID3);
        }

        [Test]
        public void Test_NoFilesWithAspect_NoArchive () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, aspectID, outFilePath, archiveName, null);

            string opFileWithPath = outFilePath + archiveName;

            Assert.IsFalse (File.Exists (opFileWithPath), "File archive created even though no files were applied aspect.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_Aspects_GroupingIDZero_Illegal () {
            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, 0, outFilePath, archiveName, null);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspect_Illegal () {
            ulong veryLargeAspectID = ulong.MaxValue;

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, veryLargeAspectID, outFilePath, archiveName, null);
        }

        [Test]
        public void Test_NoFilesInBriefcase_NoArchive () {
            ulong briefcaseID = _mfsOperations.CreateBriefcase (_briefcaseName, _briefcaseDesc);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.BRIEFCASE, briefcaseID, outFilePath, archiveName, null);

            string opFileWithPath = outFilePath + archiveName;

            Assert.IsFalse (File.Exists (opFileWithPath), "File archive created even though no files in briefcase.");

            _mfsOperations.DeleteBriefcase (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_Briefcases_GroupingIDZero_Illegal () {
            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.BRIEFCASE, 0, outFilePath, archiveName, null);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcase_Illegal () {
            ulong veryLargeBriefcaseID = ulong.MaxValue;

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.BRIEFCASE, veryLargeBriefcaseID, outFilePath, archiveName, null);
        }

        [Test]
        public void Test_NoFilesInCollection_NoArchive () {
            ulong collectionID = _mfsOperations.CreateCollection (_collectionName, _collectionDesc);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.COLLECTION, collectionID, outFilePath, archiveName, null);

            string opFileWithPath = outFilePath + archiveName;

            Assert.IsFalse (File.Exists (opFileWithPath), "File archive created even though no files in collection.");

            _mfsOperations.DeleteCollection (collectionID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_Collections_GroupingIDZero_Illegal () {
            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.COLLECTION, 0, outFilePath, archiveName, null);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentCollection_Illegal () {
            ulong veryLargeCollectionID = ulong.MaxValue;

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.COLLECTION, veryLargeCollectionID, outFilePath, archiveName, null);
        }
    }

    [TestFixture]
    public class Tests_ArchivingMethod_ArchiveFilesInGrouping_WithPassword : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID1);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID2);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID3);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, aspectID, outFilePath, archiveName, password);

            string opFileWithPath = outFilePath + archiveName;

            // Check to see if the o/p file exists:
            Assert.IsTrue (File.Exists (opFileWithPath), "File does not exist.");

            // Next check archive contents for file names:
            Assert.IsTrue (ZipFile.IsZipFile (opFileWithPath), "Output file is not a valid archive.");

            byte[] retrFileData1 = TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName1, password);
            Assert.AreEqual (fileData1, retrFileData1, "Retrieved file data is not as accurate.");

            byte[] retrFileData2 = TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName2, password);
            Assert.AreEqual (fileData2, retrFileData2, "Retrieved file data is not as accurate.");

            byte[] retrFileData3 = TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName3, password);
            Assert.AreEqual (fileData3, retrFileData3, "Retrieved file data is not as accurate.");

            File.Delete (opFileWithPath);
            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
            _mfsOperations.DeleteFile (fileID3);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassword_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            DateTime when = DateTime.Now;
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            string emptyPassword = string.Empty;

            try {
                _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, aspectID, outFilePath, archiveName, emptyPassword);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        public void Test_OpenArchiveWithWrongPassword () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID1);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID2);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);
            _mfsOperations.ApplyAspectToFile (aspectID, fileID3);

            string outFilePath = FILE_SYSTEM_LOCATION;
            string archiveName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + ".zip";
            string password = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            _mfsOperations.ArchiveFilesInGrouping (GroupingType.ASPECT, aspectID, outFilePath, archiveName, password);

            string opFileWithPath = outFilePath + archiveName;

            // Check to see if the o/p file exists:
            Assert.IsTrue (File.Exists (opFileWithPath), "File does not exist.");

            // Next check archive contents for file names:
            Assert.IsTrue (ZipFile.IsZipFile (opFileWithPath), "Output file is not a valid archive.");

            string wrongPassword = TestUtils.GetAWord (TYPICAL_WORD_SIZE + 1);

            try {
                TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName1, wrongPassword);
                TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName2, wrongPassword);
                TestUtils.RetrieveByteArrayFromZippedFile (opFileWithPath, fileName3, wrongPassword);

                Assert.Fail ("Bad password exception not thrown.");
            } catch (Ionic.Zip.BadPasswordException) {
            }

            File.Delete (opFileWithPath);
            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
            _mfsOperations.DeleteFile (fileID3);
        }
    }
}
