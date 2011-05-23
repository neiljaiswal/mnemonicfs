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
using MnemonicFS.MfsExceptions;
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Base;

namespace MnemonicFS.Tests.Briefcases {
    [TestFixture]
    public class Tests_BriefcasesMethod_CreateBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);
            Assert.That (briefcaseID > 0, "Briefcase not created successfully: Invalid briefcase id returned.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateBriefcaseName_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            try {
                _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }

        [Test]
        public void Test_BriefcaseNameWithMaxSizeAllowed_SanityCheck () {
            string briefcaseName = TestUtils.GetAWord (MfsOperations.MaxBriefcaseNameLength);

            ulong briefcaseID = _mfsOperations.Briefcase.New (briefcaseName, _briefcaseDesc);

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        public void Test_BriefcaseDescWithMaxSizeAllowed_SanityCheck () {
            string briefcaseDesc = TestUtils.GetAWord (MfsOperations.MaxBriefcaseDescLength);

            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, briefcaseDesc);

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullBriefcaseName_Illegal () {
            string nullBriefcaseName = null;

            _mfsOperations.Briefcase.New (nullBriefcaseName, _briefcaseDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullBriefcaseDesc_Illegal () {
            string nullBriefcaseDesc = null;

            _mfsOperations.Briefcase.New (_briefcaseName, nullBriefcaseDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyBriefcaseName_Illegal () {
            string emptyBriefcaseName = string.Empty;

            _mfsOperations.Briefcase.New (emptyBriefcaseName, _briefcaseDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseNameGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInBriefcaseName = MfsOperations.MaxBriefcaseNameLength;

            string longBriefcaseName = TestUtils.GetAWord (maxCharsInBriefcaseName + 1);
            _mfsOperations.Briefcase.New (longBriefcaseName, _briefcaseDesc);
        }

        [Test]
        public void Test_EmptyBriefcaseDesc_Legal () {
            string emptyBriefcaseDesc = string.Empty;

            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, emptyBriefcaseDesc);
            Assert.That (briefcaseID > 0, "Failed to save briefcase though empty briefcase description is allowed.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseDescGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInBriefcaseDesc = MfsOperations.MaxBriefcaseDescLength;

            string longBriefcaseDesc = TestUtils.GetAWord (maxCharsInBriefcaseDesc + 1);
            _mfsOperations.Briefcase.New (_briefcaseName, longBriefcaseDesc);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_DoesBriefcaseExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            bool briefcaseExists = _mfsOperations.Briefcase.Exists (briefcaseID);
            Assert.IsTrue (briefcaseExists, "Briefcase was shown as not existing, even though it does.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargeBriefcaseID = ulong.MaxValue;

            bool briefcaseExists = _mfsOperations.Briefcase.Exists (veryLargeBriefcaseID);
            Assert.IsFalse (briefcaseExists, "Briefcase was shown as existing, even though it does not.");
        }

        [Test]
        public void Test_SanityCheck_BriefcaseName_Exists () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            bool briefcaseExists = _mfsOperations.Briefcase.Exists (_briefcaseName);
            Assert.IsTrue (briefcaseExists, "Briefcase was shown as not existing, even though it does.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        public void Test_SanityCheck_BriefcaseName_NotExists () {
            string someBriefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);

            bool briefcaseExists = _mfsOperations.Briefcase.Exists (someBriefcaseName);
            Assert.IsFalse (briefcaseExists, "Briefcase was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            _mfsOperations.Briefcase.Exists (0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseNameNull_Illegal () {
            _mfsOperations.Briefcase.Exists (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseNameSizeZero_Illegal () {
            _mfsOperations.Briefcase.Exists (string.Empty);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_DeleteBriefcase : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            int numBriefcasesDeleted = _mfsOperations.Briefcase.Delete (briefcaseID);
            Assert.AreEqual (1, numBriefcasesDeleted, "Briefcase was not deleted.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            _mfsOperations.Briefcase.Delete (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            _mfsOperations.Briefcase.Delete (veryLargeBriefcaseID);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_GetBriefcaseName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (briefcaseID, out briefcaseName, out briefcaseDesc);
            Assert.AreEqual (_briefcaseName, briefcaseName, "Did not retrieve briefcase's name.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (0, out briefcaseName, out briefcaseDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (veryLargeBriefcaseID, out briefcaseName, out briefcaseDesc);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_GetBriefcaseDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (briefcaseID, out briefcaseName, out briefcaseDesc);
            Assert.AreEqual (_briefcaseDesc, briefcaseDesc, "Did not retrieve briefcase's description.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (0, out briefcaseName, out briefcaseDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (veryLargeBriefcaseID, out briefcaseName, out briefcaseDesc);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_GetBriefcaseIDFromName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = CreateUniqueBriefcase (ref _mfsOperations, out _briefcaseName, out _briefcaseDesc);

            ulong retrBriefcaseID = _mfsOperations.Briefcase.IDFromName (_briefcaseName);
            Assert.AreEqual (briefcaseID, retrBriefcaseID, "Wrong briefcase id retrieved.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseName_Illegal () {
            string nonExistentBriefcaseName = null;

            do {
                nonExistentBriefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.Briefcase.Exists (nonExistentBriefcaseName));

            _mfsOperations.Briefcase.IDFromName (nonExistentBriefcaseName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullBriefcaseName_Illegal () {
            string nullBriefcaseName = null;

            _mfsOperations.Briefcase.IDFromName (nullBriefcaseName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyBriefcaseName_Illegal () {
            string emptyBriefcaseName = string.Empty;

            _mfsOperations.Briefcase.IDFromName (emptyBriefcaseName);
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_GetAllBriefcases : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> briefcasesList = new List<ulong> ();
            // Add the global briefcase id too:
            briefcasesList.Add (MfsOperations.GlobalBriefcase);

            for (int i = 0; i < TYPICAL_MULTI_VALUE; ++i) {
                string briefcaseName, briefcaseDesc;
                ulong briefcaseID = CreateUniqueBriefcase (ref _mfsOperations, out briefcaseName, out briefcaseDesc);
                briefcasesList.Add (briefcaseID);
            }

            List<ulong> retrBriefcaseIDs = _mfsOperations.Briefcase.All ();

            briefcasesList.Sort ();
            retrBriefcaseIDs.Sort ();

            Assert.AreEqual (briefcasesList.Count, retrBriefcaseIDs.Count, "Wrong number of briefcase ids retrieved.");
            Assert.AreEqual (briefcasesList, retrBriefcaseIDs, "Wrong briefcase ids recovered.");

            foreach (ulong briefcaseID in briefcasesList) {
                if (briefcaseID == MfsOperations.GlobalBriefcase) {
                    continue;
                }
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_UpdateBriefcaseName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string newBriefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            bool briefcaseNameUpdated = _mfsOperations.Briefcase.UpdateName (briefcaseID, newBriefcaseName);
            Assert.IsTrue (briefcaseNameUpdated, "Briefcase name was not updated successfully.");

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (briefcaseID, out briefcaseName, out briefcaseDesc);
            Assert.AreEqual (newBriefcaseName, briefcaseName, "Briefcase name was not updated successfully.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        public void Test_BriefcaseNameWithMaxSizeAllowed_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string newBriefcaseName = TestUtils.GetAWord (MfsOperations.MaxBriefcaseNameLength);
            _mfsOperations.Briefcase.UpdateName (briefcaseID, newBriefcaseName);

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (briefcaseID, out briefcaseName, out briefcaseDesc);
            Assert.AreEqual (newBriefcaseName, briefcaseName, "Briefcase name was not updated successfully.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.Briefcase.UpdateName (0, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            _mfsOperations.Briefcase.UpdateName (veryLargeBriefcaseID, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewBriefcaseName_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string nullBriefcaseName = null;

            try {
                _mfsOperations.Briefcase.UpdateName (briefcaseID, nullBriefcaseName);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewBriefcaseName_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string emptyBriefcaseName = string.Empty;

            try {
                _mfsOperations.Briefcase.UpdateName (briefcaseID, emptyBriefcaseName);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseNameGreaterThanSystemDefined_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string veryLongBriefcaseName = TestUtils.GetAWord (MfsOperations.MaxBriefcaseNameLength + 1);

            try {
                _mfsOperations.Briefcase.UpdateName (briefcaseID, veryLongBriefcaseName);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_UpdateBriefcaseDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string newBriefcaseDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            bool briefcaseDescUpdated = _mfsOperations.Briefcase.UpdateDesc (briefcaseID, newBriefcaseDesc);
            Assert.IsTrue (briefcaseDescUpdated, "Briefcase description was not updated successfully.");

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (briefcaseID, out briefcaseName, out briefcaseDesc);
            Assert.AreEqual (newBriefcaseDesc, briefcaseDesc, "Briefcase description was not updated successfully.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        public void Test_BriefcaseDescWithMaxSizeAllowed_SanityCheck () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string newBriefcaseDesc = TestUtils.GetAWord (MfsOperations.MaxBriefcaseDescLength);
            _mfsOperations.Briefcase.UpdateDesc (briefcaseID, newBriefcaseDesc);

            string briefcaseName, briefcaseDesc;
            _mfsOperations.Briefcase.Get (briefcaseID, out briefcaseName, out briefcaseDesc);
            Assert.AreEqual (newBriefcaseDesc, briefcaseDesc, "Briefcase description was not updated successfully.");

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseIDZero_Illegal () {
            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.Briefcase.UpdateDesc (0, anyDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentBriefcaseID_Illegal () {
            ulong veryLargeBriefcaseID = UInt64.MaxValue;

            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            _mfsOperations.Briefcase.UpdateDesc (veryLargeBriefcaseID, anyDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewBriefcaseDesc_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string nullBriefcaseDesc = null;

            try {
                _mfsOperations.Briefcase.UpdateDesc (briefcaseID, nullBriefcaseDesc);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }

        [Test]
        public void Test_EmptyNewBriefcaseDesc_Legal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string emptyBriefcaseDesc = string.Empty;
            _mfsOperations.Briefcase.UpdateDesc (briefcaseID, emptyBriefcaseDesc);

            _mfsOperations.Briefcase.Delete (briefcaseID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_BriefcaseDescGreaterThanSystemDefined_Illegal () {
            ulong briefcaseID = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            string veryLongBriefcaseDesc = TestUtils.GetAWord (MfsOperations.MaxBriefcaseDescLength + 1);

            try {
                _mfsOperations.Briefcase.UpdateDesc (briefcaseID, veryLongBriefcaseDesc);
            } finally {
                _mfsOperations.Briefcase.Delete (briefcaseID);
            }
        }
    }

    [TestFixture]
    public class Tests_BriefcasesMethod_DeleteAllBriefcasesInSystem : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numBriefcasesToCreate = TYPICAL_MULTI_VALUE;

            List<ulong> listBriefcases = CreateUniqueNBriefcases (ref _mfsOperations, numBriefcasesToCreate);

            int numBriefcasesDeleted = _mfsOperations.Briefcase.DeleteAll ();
            Assert.AreEqual (listBriefcases.Count, numBriefcasesDeleted, "Did not delete the same number of briefcases as were created.");
        }

        [Test]
        public void Test_GlobalBriefcaseRetained_SanityCheck () {
            _mfsOperations.Briefcase.DeleteAll ();

            bool globalBriefcaseExists = _mfsOperations.Briefcase.Exists (MfsOperations.GlobalBriefcase);
            Assert.IsTrue (globalBriefcaseExists, "Deleted global briefcase.");
        }

        [Test]
        public void Test_AllFilesReturnedToGlobalBriefcase_SanityCheck () {
            ulong briefcaseID1 = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);
            _briefcaseName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            ulong briefcaseID2 = _mfsOperations.Briefcase.New (_briefcaseName, _briefcaseDesc);

            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, _fileName, _fileNarration, _fileData, when, false);

            _mfsOperations.Briefcase.MoveTo (fileID1, briefcaseID1);
            _mfsOperations.Briefcase.MoveTo (fileID2, briefcaseID2);

            ulong containingBriefcaseID1 = _mfsOperations.Briefcase.GetContaining (fileID1);
            Assert.AreEqual (briefcaseID1, containingBriefcaseID1, "File move operation to briefcase failed.");

            ulong containingBriefcaseID2 = _mfsOperations.Briefcase.GetContaining (fileID2);
            Assert.AreEqual (briefcaseID2, containingBriefcaseID2, "File move operation to briefcase failed.");

            _mfsOperations.Briefcase.DeleteAll ();

            containingBriefcaseID1 = _mfsOperations.Briefcase.GetContaining (fileID1);
            Assert.AreEqual (MfsOperations.GlobalBriefcase, containingBriefcaseID1,
                "File is not moved to global briefcase on deletion of all briefcases within system."
                );

            containingBriefcaseID2 = _mfsOperations.Briefcase.GetContaining (fileID2);
            Assert.AreEqual (MfsOperations.GlobalBriefcase, containingBriefcaseID2,
                "File is not moved to global briefcase on deletion of all briefcases within system."
                );

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
        }
    }
}
