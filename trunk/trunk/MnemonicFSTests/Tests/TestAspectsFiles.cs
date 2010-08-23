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

namespace MnemonicFS.Tests.AspectsFiles {
    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool applySuccess = _mfsOperations.ApplyAspectToFile (aspectID, fileID);
            Assert.IsTrue (applySuccess, "Aspect failed to be applied to file.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToFile (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.ApplyAspectToFile (0, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeFileID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToFile (aspectID, veryLargeFileID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectToFile (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_IsAspectAppliedToFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Applied () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            _mfsOperations.ApplyAspectToFile (aspectID, fileID);

            bool isApplied = _mfsOperations.IsAspectAppliedToFile (aspectID, fileID);
            Assert.IsTrue (isApplied, "Indicated that aspect has not been applied to file, even though it is.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        public void Test_SanityCheck_NotApplied () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool isApplied = _mfsOperations.IsAspectAppliedToFile (aspectID, fileID);
            Assert.IsFalse (isApplied, "Indicated that aspect has been applied to file, even though it isn't.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.IsAspectAppliedToFile (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.IsAspectAppliedToFile (0, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeFileID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToFile (aspectID, veryLargeFileID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.IsAspectAppliedToFile (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            _mfsOperations.ApplyAspectToFile (aspectID, fileID);

            bool isUnapplied = _mfsOperations.UnapplyAspectFromFile (aspectID, fileID);
            Assert.IsTrue (isUnapplied, "Aspect was not successfully unapplied from file.");

            bool isApplied = _mfsOperations.IsAspectAppliedToFile (aspectID, fileID);
            Assert.IsFalse (isApplied, "Attempt to unapply aspect from file was not successful.");

            _mfsOperations.DeleteAspect (aspectID);
            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.UnapplyAspectFromFile (aspectID, 0);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.UnapplyAspectFromFile (0, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            ulong veryLargeFileID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromFile (aspectID, veryLargeFileID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            ulong veryLargeAspectID = UInt64.MaxValue;

            try {
                _mfsOperations.UnapplyAspectFromFile (veryLargeAspectID, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectsAppliedOnFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.ApplyAspectToFile (aspectID, fileID);
            }

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnFile (fileID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects applied to file.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.GetAspectsAppliedOnFile (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.GetAspectsAppliedOnFile (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetFilesAppliedWithAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                _mfsOperations.ApplyAspectToFile (aspectID, fileID);

                fileIDs.Add (fileID);
            }

            List<ulong> retrievedFileIDs = _mfsOperations.GetFilesAppliedWithAspect (aspectID);
            Assert.AreEqual (fileIDs.Count, retrievedFileIDs.Count, "Did not retrieve exact number of files aspect has been applied to.");

            retrievedFileIDs.Sort ();
            fileIDs.Sort ();

            for (int i = 0; i < fileIDs.Count; ++i) {
                Assert.AreEqual (fileIDs[i], retrievedFileIDs[i], "Got invalid file id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.GetFilesAppliedWithAspect (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.GetFilesAppliedWithAspect (veryLargeAspectID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToFile (aspectIDs, fileID);

            List<ulong> retrievedAspectIDs = _mfsOperations.GetAspectsAppliedOnFile (fileID);
            Assert.AreEqual (aspectIDs.Count, retrievedAspectIDs.Count, "Did not retrieve exact number of aspects.");

            retrievedAspectIDs.Sort ();
            aspectIDs.Sort ();

            for (int i = 0; i < aspectIDs.Count; ++i) {
                Assert.AreEqual (aspectIDs[i], retrievedAspectIDs[i], "Got invalid aspect id.");
            }

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.ApplyAspectsToFile (null, fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            try {
                _mfsOperations.ApplyAspectsToFile (new List<ulong> (), fileID);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            ulong veryLargeFileID = UInt64.MaxValue;

            try {
                _mfsOperations.ApplyAspectsToFile (aspectIDs, veryLargeFileID);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectToFiles : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            _mfsOperations.ApplyAspectToFiles (aspectID, fileIDs);

            List<ulong> retrievedFileIDs = _mfsOperations.GetFilesAppliedWithAspect (aspectID);
            Assert.AreEqual (fileIDs.Count, retrievedFileIDs.Count, "Did not retrieve exact number of files aspect has been applied to.");

            retrievedFileIDs.Sort ();
            fileIDs.Sort ();

            for (int i = 0; i < fileIDs.Count; ++i) {
                Assert.AreEqual (fileIDs[i], retrievedFileIDs[i], "Got invalid file id.");
            }

            _mfsOperations.DeleteAspect (aspectID);

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullFileList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToFiles (aspectID, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyFileList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.ApplyAspectToFiles (aspectID, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_ApplyAspectsToFiles : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToFiles (aspectIDs, fileIDs);

            foreach (ulong fileID in fileIDs) {
                List<ulong> retrAspects = _mfsOperations.GetAspectsAppliedOnFile (fileID);
                Assert.AreEqual (aspectIDs.Count, retrAspects.Count, "Aspect count for file does not match.");
            }

            foreach (ulong aspectID in aspectIDs) {
                List<ulong> retrFiles = _mfsOperations.GetFilesAppliedWithAspect (aspectID);
                Assert.AreEqual (fileIDs.Count, retrFiles.Count, "File count for aspect does not match.");
            }

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullFileList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToFiles (aspectIDs, null);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyFileList_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID);

            try {
                _mfsOperations.ApplyAspectsToFiles (aspectIDs, new List<ulong> ());
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID);

            try {
                _mfsOperations.ApplyAspectsToFiles (null, fileIDs);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectList_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID);

            try {
                _mfsOperations.ApplyAspectsToFiles (new List<ulong> (), fileIDs);
            } finally {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAllAspectsFromFile : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            int numAspectsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectIDs = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);

            _mfsOperations.ApplyAspectsToFile (aspectIDs, fileID);

            int numAspectsUnapplied = _mfsOperations.UnapplyAllAspectsFromFile (fileID);
            Assert.AreEqual (numAspectsToCreate, numAspectsUnapplied, "Attempt to unapply aspects from file was unsuccessful.");

            List<ulong> allAspects = _mfsOperations.GetAspectsAppliedOnFile (fileID);
            Assert.AreEqual (0, allAspects.Count, "Incorrect number of aspects unapplied from file.");

            _mfsOperations.DeleteFile (fileID);

            foreach (ulong aspectID in aspectIDs) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_FileIDZero_Illegal () {
            _mfsOperations.UnapplyAllAspectsFromFile (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentFileID_Illegal () {
            ulong veryLargeFileID = UInt64.MaxValue;

            _mfsOperations.UnapplyAllAspectsFromFile (veryLargeFileID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UnapplyAspectFromAllFiles : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numFilesToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> fileIDs = new List<ulong> (numFilesToCreate);

            for (int i = 0; i < numFilesToCreate; ++i) {
                _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                DateTime when = DateTime.Now;
                ulong fileID = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

                fileIDs.Add (fileID);
            }

            _mfsOperations.ApplyAspectToFiles (aspectID, fileIDs);

            int numFilesUnappliedFrom = _mfsOperations.UnapplyAspectFromAllFiles (aspectID);
            Assert.AreEqual (numFilesToCreate, numFilesUnappliedFrom, "Attempt to unapply aspect from files was unsuccessful.");

            List<ulong> allFiles = _mfsOperations.GetFilesAppliedWithAspect (aspectID);
            Assert.AreEqual (0, allFiles.Count, "Number of files that aspect was unapplied from was incorrect.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.UnapplyAspectFromAllFiles (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            _mfsOperations.UnapplyAspectFromAllFiles (veryLargeAspectID);
        }
    }
}
