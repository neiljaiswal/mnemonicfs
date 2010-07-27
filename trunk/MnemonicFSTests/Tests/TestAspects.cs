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

namespace MnemonicFS.Tests.Aspects {
    [TestFixture]
    public class Tests_AspectsMethod_CreateAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);
            Assert.That (aspectID > 0, "Aspect not created successfully: Invalid aspect id returned.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateAspectName_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            try {
                _mfsOperations.CreateAspect (_aspectName, _aspectDesc);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        public void Test_AspectNameWithMaxSizeAllowed_SanityCheck () {
            string aspectName = TestUtils.GetAWord (MfsOperations.MaxAspectNameLength);

            ulong aspectID = _mfsOperations.CreateAspect (aspectName, _aspectDesc);

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        public void Test_AspectDescWithMaxSizeAllowed_SanityCheck () {
            string aspectDesc = TestUtils.GetAWord (MfsOperations.MaxAspectDescLength);

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, aspectDesc);

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectName_Illegal () {
            string nullAspectName = null;

            _mfsOperations.CreateAspect (nullAspectName, _aspectDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectDesc_Illegal () {
            string nullAspectDesc = null;

            _mfsOperations.CreateAspect (_aspectName, nullAspectDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectName_Illegal () {
            string emptyAspectName = string.Empty;

            _mfsOperations.CreateAspect (emptyAspectName, _aspectDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectNameGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInAspectName = MfsOperations.MaxAspectNameLength;

            string longAspectName = TestUtils.GetAWord (maxCharsInAspectName + 1);

            _mfsOperations.CreateAspect (longAspectName, _aspectDesc);
        }

        [Test]
        public void Test_EmptyAspectDesc_Legal () {
            string emptyAspectDesc = string.Empty;

            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, emptyAspectDesc);
            Assert.That (aspectID > 0, "Failed to save aspect though empty aspect description is allowed.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectDescGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInAspectDesc = MfsOperations.MaxAspectDescLength;

            string longAspectDesc = TestUtils.GetAWord (maxCharsInAspectDesc + 1);

            _mfsOperations.CreateAspect (_aspectName, longAspectDesc);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_DoesAspectExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool aspectExists = _mfsOperations.DoesAspectExist (aspectID);
            Assert.IsTrue (aspectExists, "Aspect was shown as not existing, even though it does.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargeAspectID = ulong.MaxValue;

            bool aspectExists = _mfsOperations.DoesAspectExist (veryLargeAspectID);
            Assert.IsFalse (aspectExists, "Aspect was shown as existing, even though it does not.");
        }

        [Test]
        public void Test_SanityCheck_WithAspectName_Exists () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            bool aspectExists = _mfsOperations.DoesAspectExist (_aspectName);
            Assert.IsTrue (aspectExists, "Aspect was shown as not existing, even though it does.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        public void Test_SanityCheck_WithAspectName_NotExists () {
            string someAspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);

            bool aspectExists = _mfsOperations.DoesAspectExist (someAspectName);
            Assert.IsFalse (aspectExists, "Aspect was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.DoesAspectExist (0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectNameNull_Illegal () {
            _mfsOperations.DoesAspectExist (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectNameSizeZero_Illegal () {
            _mfsOperations.DoesAspectExist (string.Empty);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_DeleteAspect : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            int numAspectsDeleted = _mfsOperations.DeleteAspect (aspectID);
            Assert.AreEqual (1, numAspectsDeleted, "Aspect was not deleted.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            _mfsOperations.DeleteAspect (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.DeleteAspect (veryLargeAspectID);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (_aspectName, aspectName, "Did not retrieve existing aspect's name.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string aspectName, aspectDesc;

            _mfsOperations.GetAspectNameAndDesc (0, out aspectName, out aspectDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (veryLargeAspectID, out aspectName, out aspectDesc);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (_aspectDesc, aspectDesc, "Did not retrieve existing aspect's description.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (0, out aspectName, out aspectDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (veryLargeAspectID, out aspectName, out aspectDesc);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAspectIDFromName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = CreateUniqueAspect (ref _mfsOperations, out _aspectName, out _aspectDesc);

            ulong retrAspectID = _mfsOperations.GetAspectIDFromName (_aspectName);
            Assert.AreEqual (aspectID, retrAspectID, "Wrong aspect ID retrieved.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectName_Illegal () {
            string nonExistentAspectName = null;

            do {
                nonExistentAspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesAspectExist (nonExistentAspectName));

            _mfsOperations.GetAspectIDFromName (nonExistentAspectName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectName_Illegal () {
            string nullAspectName = null;

            _mfsOperations.GetAspectIDFromName (nullAspectName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectName_Illegal () {
            string emptyAspectName = string.Empty;

            _mfsOperations.GetAspectIDFromName (emptyAspectName);
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_GetAllAspects : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> aspectsList = new List<ulong> ();

            for (int i = 0; i < 10; ++i) {
                string aspectName, aspectDesc;
                ulong aspectID = CreateUniqueAspect (ref _mfsOperations, out aspectName, out aspectDesc);
                aspectsList.Add (aspectID);
            }

            List<ulong> retrAspectIDs = _mfsOperations.GetAllAspects ();

            aspectsList.Sort ();
            retrAspectIDs.Sort ();

            Assert.AreEqual (aspectsList.Count, retrAspectIDs.Count, "Wrong number of aspect ids retrieved.");
            Assert.AreEqual (aspectsList, retrAspectIDs, "Wrong aspect ids recovered.");

            foreach (ulong aspectID in aspectsList) {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UpdateAspectName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string newAspectName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            bool aspectNameUpdated = _mfsOperations.UpdateAspectName (aspectID, newAspectName);
            Assert.IsTrue (aspectNameUpdated, "Aspect name was not updated successfully.");

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (newAspectName, aspectName, "Aspect name was not updated successfully.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        public void Test_AspectNameWithMaxSizeAllowed_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string newAspectName = TestUtils.GetAWord (MfsOperations.MaxAspectNameLength);
            _mfsOperations.UpdateAspectName (aspectID, newAspectName);

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (newAspectName, aspectName, "Wrong aspect name returned after aspect name updation.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateAspectName (0, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            string anyName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateAspectName (veryLargeAspectID, anyName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewAspectName_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string nullAspectName = null;

            try {
                _mfsOperations.UpdateAspectName (aspectID, nullAspectName);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyNewAspectName_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string emptyAspectName = string.Empty;

            try {
                _mfsOperations.UpdateAspectName (aspectID, emptyAspectName);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectNameGreaterThanSystemDefined_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string veryLongAspectName = TestUtils.GetAWord (MfsOperations.MaxAspectNameLength + 1);

            try {
                _mfsOperations.UpdateAspectName (aspectID, veryLongAspectName);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_UpdateAspectDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string newAspectDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            bool aspectDescUpdated = _mfsOperations.UpdateAspectDesc (aspectID, newAspectDesc);
            Assert.IsTrue (aspectDescUpdated, "Aspect description was not updated successfully.");

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (newAspectDesc, aspectDesc, "Aspect description was not updated successfully.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        public void Test_AspectDescWithMaxSizeAllowed_SanityCheck () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string newAspectDesc = TestUtils.GetAWord (MfsOperations.MaxAspectDescLength);
            _mfsOperations.UpdateAspectDesc (aspectID, newAspectDesc);

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (newAspectDesc, aspectDesc, "Wrong aspect description returned after aspect description updation.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectIDZero_Illegal () {
            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateAspectDesc (0, anyDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectID_Illegal () {
            ulong veryLargeAspectID = UInt64.MaxValue;

            string anyDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateAspectDesc (veryLargeAspectID, anyDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullNewAspectDesc_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string nullAspectDesc = null;

            try {
                _mfsOperations.UpdateAspectDesc (aspectID, nullAspectDesc);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }

        [Test]
        public void Test_EmptyNewAspectDesc_Legal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string emptyAspectDesc = string.Empty;
            _mfsOperations.UpdateAspectDesc (aspectID, emptyAspectDesc);

            string aspectName, aspectDesc;
            _mfsOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            Assert.AreEqual (string.Empty, aspectDesc, "Aspect description was not updated to an empty string.");

            _mfsOperations.DeleteAspect (aspectID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_AspectDescGreaterThanSystemDefined_Illegal () {
            ulong aspectID = _mfsOperations.CreateAspect (_aspectName, _aspectDesc);

            string veryLongAspectDesc = TestUtils.GetAWord (MfsOperations.MaxAspectDescLength + 1);

            try {
                _mfsOperations.UpdateAspectDesc (aspectID, veryLongAspectDesc);
            } finally {
                _mfsOperations.DeleteAspect (aspectID);
            }
        }
    }

    [TestFixture]
    public class Tests_AspectsMethod_DeleteAllAspects : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numAspectsToCreate = 3;

            List<ulong> listAspects = CreateUniqueNAspects (ref _mfsOperations, numAspectsToCreate);
            
            int numAspectsDeleted = _mfsOperations.DeleteAllAspectsInSystem ();

            Assert.AreEqual (listAspects.Count, numAspectsDeleted, "Did not delete the same number of aspects as were created.");
        }
    }
}
