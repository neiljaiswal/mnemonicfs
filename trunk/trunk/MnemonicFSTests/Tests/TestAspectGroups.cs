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

namespace MnemonicFS.Tests.AspectGroups {
    [TestFixture]
    public class Tests_AspectGroupsMethod_CreateAspectGroup : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);
            Assert.That (aspectGroupID > 0, "Aspect group not created successfully: Invalid aspect group id returned.");

            _mfsOperations.AspectGroup.Delete (aspectGroupID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_SameParentDuplicateAspectGroupName_Illegal () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);

            try {
                _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);
            } finally {
                _mfsOperations.AspectGroup.Delete (aspectGroupID);
            }
        }

        [Test]
        public void Test_DifferentParentsDuplicateAspectGroupName_Legal () {
            // Two different aspect group names:
            string parent1AspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string parent2AspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE + 1);

            // We'll use the same aspect group description for all aspect groups, doesn't matter:
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            // Create both aspect groups at same (root) level so that they are peers (siblings):
            ulong parent1AspectGroupID = _mfsOperations.AspectGroup.New (0, parent1AspectGroupName, aspectGroupDesc);
            ulong parent2AspectGroupID = _mfsOperations.AspectGroup.New (0, parent2AspectGroupName, aspectGroupDesc);

            // Also get a new aspect group name to be created as a child of both the aspect groups just created above:
            string childAspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE + 2);

            // Create child aspects with the same name for both the peers (siblings); this is a legal operation:
            ulong childGroupID1 = _mfsOperations.AspectGroup.New (parent1AspectGroupID, childAspectGroupName, aspectGroupDesc);
            ulong childGroupID2 = _mfsOperations.AspectGroup.New (parent2AspectGroupID, childAspectGroupName, aspectGroupDesc);

            // If control has reached this far, the test is passing; so delete all the aspect groups created:
            _mfsOperations.AspectGroup.Delete (childGroupID1);
            _mfsOperations.AspectGroup.Delete (childGroupID2);
            _mfsOperations.AspectGroup.Delete (parent1AspectGroupID);
            _mfsOperations.AspectGroup.Delete (parent2AspectGroupID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectGroupName_Illegal () {
            string nullAspectGroupName = null;
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.AspectGroup.New (0, nullAspectGroupName, aspectGroupDesc);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyAspectGroupName_Illegal () {
            string emptyAspectGroupName = string.Empty;
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            _mfsOperations.AspectGroup.New (0, emptyAspectGroupName, aspectGroupDesc);
        }

        [Test]
        public void Test_ParentAspectGroupIDZero_Legal () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);

            _mfsOperations.AspectGroup.Delete (aspectGroupID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentParentAspectGroupID_Illegal () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong veryLargeAspectGroupID = UInt64.MaxValue;

            _mfsOperations.AspectGroup.New (veryLargeAspectGroupID, aspectGroupName, aspectGroupDesc);
        }

        [Test]
        public void Test_EmptyAspectGroupDesc_Legal () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string emptyAspectGroupDesc = string.Empty;

            ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, emptyAspectGroupDesc);

            _mfsOperations.AspectGroup.Delete (aspectGroupID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullAspectGroupDesc_Illegal () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string nullAspectGroupDesc = null;

            _mfsOperations.AspectGroup.New (0, aspectGroupName, nullAspectGroupDesc);
        }
    }

    [TestFixture]
    public class Tests_AspectGroupsMethod_GetChildAspectGroups : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            int numAspectGroupsToCreate = TYPICAL_MULTI_VALUE;
            List<ulong> aspectGroupIDs = new List<ulong> (TYPICAL_MULTI_VALUE);
            List<string> aspectGroupNames = new List<string> (TYPICAL_MULTI_VALUE);
            List<string> aspectGroupDescs = new List<string> (TYPICAL_MULTI_VALUE);

            for (int i = 0; i < numAspectGroupsToCreate; ++i) {
                string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE + i);
                aspectGroupNames.Add (aspectGroupName);
                string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                aspectGroupDescs.Add (aspectGroupDesc);

                ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);
                aspectGroupIDs.Add (aspectGroupID);
            }

            List<ulong> retrievedAspectGroupIDs = _mfsOperations.AspectGroup.ChildAspectGroups (0);
            // Recall that the root aspect group is present in the system by default; hence TYPICAL_MULTI_VALUE + 1.
            Assert.AreEqual (TYPICAL_MULTI_VALUE + 1, retrievedAspectGroupIDs.Count, "Number of aspect groups do not match.");

            retrievedAspectGroupIDs.Sort ();
            int count = 0;
            foreach (ulong aspectGroupID in retrievedAspectGroupIDs) {
                if (count == 0) {
                    // Skip the first element since it's going to be root:
                    continue;
                }

                string aspectGroupName;
                string aspectGroupDesc;
                _mfsOperations.AspectGroup.Get (aspectGroupID, out aspectGroupName, out aspectGroupDesc);
                Assert.AreEqual (aspectGroupNames[count], aspectGroupName, "Retrieved aspect group name does not match with original.");
                Assert.AreEqual (aspectGroupDescs[count], aspectGroupDesc, "Retrieved aspect group description does not match with original.");
                ++count;
            }

            foreach (ulong aspectGroupID in aspectGroupIDs) {
                _mfsOperations.AspectGroup.Delete (aspectGroupID);
            }
        }

        [Test]
        public void Test_AspectGroupIDZero_Legal () {
            List<ulong> aspectGroups = _mfsOperations.AspectGroup.ChildAspectGroups (0);
            Assert.IsNotNull (aspectGroups, "Null list returned.");
            Assert.AreEqual (1, aspectGroups.Count, "List should have precisely one entry for root aspect group.");
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectGroup_Illegal () {
            ulong veryLargeAspectGroupID = UInt64.MaxValue;

            _mfsOperations.AspectGroup.ChildAspectGroups (veryLargeAspectGroupID);
        }
    }

    [TestFixture]
    public class Tests_AspectGroupsMethod_GetAspectGroupNameAndDesc : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);

            string retrievedAspectGroupName;
            string retrievedAspectGroupDesc;

            _mfsOperations.AspectGroup.Get (aspectGroupID, out retrievedAspectGroupName, out retrievedAspectGroupDesc);
            Assert.AreEqual (aspectGroupName, retrievedAspectGroupName, "Retrieved aspect group name does not match with original.");
            Assert.AreEqual (aspectGroupDesc, retrievedAspectGroupDesc, "Retrieved aspect group description does not match with original.");

            _mfsOperations.AspectGroup.Delete (aspectGroupID);
        }

        [Test]
        public void Test_AspectGroupIDZero_Legal () {
            string retrievedAspectGroupName;
            string retrievedAspectGroupDesc;

            _mfsOperations.AspectGroup.Get (0, out retrievedAspectGroupName, out retrievedAspectGroupDesc);
            Assert.IsNotNull (retrievedAspectGroupName, "Name of root aspect element is null.");
            Assert.IsNotNull (retrievedAspectGroupDesc, "Description of root aspect element is null.");
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectGroupID_Illegal () {
            ulong veryLargeAspectGroupID = UInt64.MaxValue;

            string retrievedAspectGroupName;
            string retrievedAspectGroupDesc;

            _mfsOperations.AspectGroup.Get (veryLargeAspectGroupID, out retrievedAspectGroupName, out retrievedAspectGroupDesc);
        }
    }

    [TestFixture]
    public class Tests_AspectGroupsMethod_DeleteAspectGroup : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            string aspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string aspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong aspectGroupID = _mfsOperations.AspectGroup.New (0, aspectGroupName, aspectGroupDesc);
            Assert.That (aspectGroupID > 0, "Aspect group not created successfully: Invalid aspect group id returned.");

            _mfsOperations.AspectGroup.Delete (aspectGroupID);

            bool aspectGroupExists = _mfsOperations.AspectGroup.Exists (aspectGroupID);
            Assert.IsFalse (aspectGroupExists, "Shows aspect group as existing even though it doesn't.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_NonEmpty_Illegal () {
            string parentAspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string parentAspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong parentAspectGroupID = _mfsOperations.AspectGroup.New (0, parentAspectGroupName, parentAspectGroupDesc);

            string childAspectGroupName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string childAspectGroupDesc = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);

            ulong childAspectGroupID = _mfsOperations.AspectGroup.New (parentAspectGroupID, childAspectGroupName, childAspectGroupDesc);

            try {
                _mfsOperations.AspectGroup.Delete (parentAspectGroupID);
            } finally {
                _mfsOperations.AspectGroup.Delete (childAspectGroupID);
                _mfsOperations.AspectGroup.Delete (parentAspectGroupID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentAspectGroup_Illegal () {
            ulong veryLargeAspectGroupID = UInt64.MaxValue;

            _mfsOperations.AspectGroup.Delete (veryLargeAspectGroupID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalOperationException))]
        public void Test_AspectGroupIDZero_Illegal () {
            _mfsOperations.AspectGroup.Delete (0);
        }
    }
}
