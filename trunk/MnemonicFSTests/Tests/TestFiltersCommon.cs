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

namespace MnemonicFS.Tests.Filters.Common {
    [TestFixture]
    public class Tests_Filters_Invert : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> listOdd = new List<ulong> ();
            for (ulong i = 1; i < 10; i += 2) {
                listOdd.Add (i);
            }

            List<ulong> listConsecutive = new List<ulong> ();
            for (ulong i = 0; i < 10; ++i) {
                listConsecutive.Add (i);
            }

            List<ulong> listInverted = MfsOperations.Filter.Invert (listConsecutive, listOdd);
            Assert.AreEqual (listConsecutive.Count - listOdd.Count, listInverted.Count, "Wrong number of values returned.");

            listInverted.Sort ();
            int v = 0;
            foreach (ulong invertedVal in listInverted) {
                Assert.AreEqual (v, invertedVal, "Wrong value returned.");
                v += 2;
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_IpListNull_Illegal () {
            List<ulong> ipList = null;

            List<ulong> opList = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                opList.Add (i);
            }

            MfsOperations.Filter.Invert (ipList, opList);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_OpListNull_Illegal () {
            List<ulong> ipList = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                ipList.Add (i);
            }

            List<ulong> opList = null;

            MfsOperations.Filter.Invert (ipList, opList);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_IpListEmpty_Illegal () {
            List<ulong> ipList = new List<ulong> ();

            List<ulong> opList = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                opList.Add (i);
            }

            MfsOperations.Filter.Invert (ipList, opList);
        }

        [Test]
        public void Test_OpListEmpty_Legal () {
            List<ulong> ipList = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                ipList.Add (i);
            }

            List<ulong> opList = new List<ulong> ();

            MfsOperations.Filter.Invert (ipList, opList);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_IpListNotSupersetOfOpList_Illegal () {
            // I/p list *must* necessarily be a superset of the o/p list; or to
            // paraphrase, o/p list *must* necessarily be a subset of i/p list.
            // This test demonstrates this.

            // I/p list contains consecutive numbers from 1 through 10:
            List<ulong> ipList = new List<ulong> ();
            for (ulong i = 1; i <= 10; ++i) {
                ipList.Add (i);
            }

            // O/p list contains odd numbers from 1 through 9, *and* 11,
            // which is clearly illegal:
            List<ulong> opList = new List<ulong> ();
            for (ulong i = 1; i < 10; i += 2) {
                opList.Add (i);
            }
            opList.Add (11);

            MfsOperations.Filter.Invert (ipList, opList);
        }

        [Test]
        public void Test_OpListEmpty_ReturnsIpList () {
            List<ulong> ipList = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                ipList.Add (i);
            }

            List<ulong> opList = new List<ulong> ();

            List<ulong> invertedList = MfsOperations.Filter.Invert (ipList, opList);
            Assert.AreEqual (ipList, invertedList, "Wrong values returned.");
        }
    }

    [TestFixture]
    public class Tests_Filters_CombineOR : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> listOdd = new List<ulong> ();
            for (ulong i = 1; i < 10; i += 2) {
                listOdd.Add (i);
            }

            List<ulong> listEven = new List<ulong> ();
            for (ulong i = 0; i < 10; i += 2) {
                listEven.Add (i);
            }

            List<ulong> oredList = MfsOperations.Filter.CombineOR (listOdd, listEven);
            Assert.AreEqual (listOdd.Count + listEven.Count, oredList.Count, "Wrong number of values returned.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_List1Null_Illegal () {
            List<ulong> list1 = null;

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            MfsOperations.Filter.CombineOR (list1, list2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_List2Null_Illegal () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = null;

            MfsOperations.Filter.CombineOR (list1, list2);
        }

        [Test]
        public void Test_List1Empty_Legal () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = new List<ulong> ();

            MfsOperations.Filter.CombineOR (list1, list2);
        }

        [Test]
        public void Test_List2Empty_Legal () {
            List<ulong> list1 = new List<ulong> ();

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            MfsOperations.Filter.CombineOR (list1, list2);
        }

        [Test]
        public void Test_List1Empty_ReturnsList2 () {
            List<ulong> list1 = new List<ulong> ();

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            List<ulong> oredList = MfsOperations.Filter.CombineOR (list1, list2);
            Assert.AreEqual (list2, oredList, "Wrong values returned.");
        }

        [Test]
        public void Test_List2Empty_ReturnsList1 () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = new List<ulong> ();

            List<ulong> oredList = MfsOperations.Filter.CombineOR (list1, list2);
            Assert.AreEqual (list1, oredList, "Wrong values returned.");
        }
    }

    [TestFixture]
    public class Tests_Filters_CombineAND : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> listOdd = new List<ulong> ();
            for (ulong i = 1; i < 10; i += 2) {
                listOdd.Add (i);
            }

            List<ulong> listConsecutive = new List<ulong> ();
            for (ulong i = 0; i < 10; ++i) {
                listConsecutive.Add (i);
            }

            List<ulong> andedList = MfsOperations.Filter.CombineAND (listOdd, listConsecutive);
            Assert.AreEqual (listOdd.Count, andedList.Count, "Wrong number of values returned.");
            Assert.AreEqual (listOdd, andedList, "Wrong values returned.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_List1Null_Illegal () {
            List<ulong> list1 = null;

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            MfsOperations.Filter.CombineAND (list1, list2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_List2Null_Illegal () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = null;

            MfsOperations.Filter.CombineAND (list1, list2);
        }

        [Test]
        public void Test_List1Empty_ReturnsEmptyList () {
            List<ulong> list1 = new List<ulong> ();

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            List<ulong> andedList = MfsOperations.Filter.CombineAND (list1, list2);
            Assert.AreEqual (0, andedList.Count, "Non-empty list returned.");
        }

        [Test]
        public void Test_List2Empty_ReturnsEmptyList () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = new List<ulong> ();

            List<ulong> andedList = MfsOperations.Filter.CombineAND (list1, list2);
            Assert.AreEqual (0, andedList.Count, "Non-empty list returned.");
        }
    }

    [TestFixture]
    public class Tests_Filters_DeMorgansTheorem : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck1 () {
            // NOT (P OR Q) = (NOT P) AND (NOT Q)

            // First, create a list of 10 numbers:
            List<ulong> superList = new List<ulong> ();
            for (ulong i = 0; i <= 9; ++i) {
                superList.Add (i);
            }

            List<ulong> pList = new List<ulong> ();
            for (ulong i = 1; i < 6; i += 2) {
                pList.Add (i);
            }

            List<ulong> qList = new List<ulong> ();
            for (ulong i = 5; i <= 9; ++i) {
                qList.Add (i);
            }

            // Evaluate the LHS first:
            List<ulong> pORqList = MfsOperations.Filter.CombineOR (pList, qList);
            List<ulong> LHS = MfsOperations.Filter.Invert (superList, pORqList);

            // Evaluate the RHS next:
            List<ulong> NOTpList = MfsOperations.Filter.Invert (superList, pList);
            List<ulong> NOTqList = MfsOperations.Filter.Invert (superList, qList);
            List<ulong> RHS = MfsOperations.Filter.CombineAND (NOTpList, NOTqList);

            // Assert thy are equal:
            LHS.Sort ();
            RHS.Sort ();
            Assert.AreEqual (LHS, RHS, "DeMorgan's test failed: LHS != RHS.");
        }

        [Test]
        public void Test_SanityCheck2 () {
            // NOT (P AND Q) = (NOT P) OR (NOT Q)

            // First, create a list of 10 numbers:
            List<ulong> superList = new List<ulong> ();
            for (ulong i = 0; i <= 9; ++i) {
                superList.Add (i);
            }

            List<ulong> pList = new List<ulong> ();
            for (ulong i = 1; i < 6; i += 2) {
                pList.Add (i);
            }

            List<ulong> qList = new List<ulong> ();
            for (ulong i = 5; i <= 9; ++i) {
                qList.Add (i);
            }

            // Evaluate the LHS first:
            List<ulong> pANDqList = MfsOperations.Filter.CombineAND (pList, qList);
            List<ulong> LHS = MfsOperations.Filter.Invert (superList, pANDqList);

            // Evaluate the RHS next:
            List<ulong> NOTpList = MfsOperations.Filter.Invert (superList, pList);
            List<ulong> NOTqList = MfsOperations.Filter.Invert (superList, qList);
            List<ulong> RHS = MfsOperations.Filter.CombineOR (NOTpList, NOTqList);

            // Assert thy are equal:
            LHS.Sort ();
            RHS.Sort ();
            Assert.AreEqual (LHS, RHS, "DeMorgan's test failed: LHS != RHS.");
        }
    }

    [TestFixture]
    public class Tests_Filters_CombineEXOR : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            List<ulong> list1 = new List<ulong> ();
            list1.Add (1);
            list1.Add (3);
            list1.Add (5);
            list1.Add (7);
            list1.Add (9);

            List<ulong> list2 = new List<ulong> ();
            list2.Add (1);
            list2.Add (2);
            list2.Add (3);
            list2.Add (4);
            list2.Add (5);

            List<ulong> exoredList = MfsOperations.Filter.CombineEXOR (list1, list2);
            // O/p list should now contain four elements: 2, 4, 7, & 9:
            Assert.AreEqual (4, exoredList.Count, "Wrong number of values returned.");
            Assert.IsTrue (exoredList.Contains (2), "Wrong value returned.");
            Assert.IsTrue (exoredList.Contains (4), "Wrong value returned.");
            Assert.IsTrue (exoredList.Contains (7), "Wrong value returned.");
            Assert.IsTrue (exoredList.Contains (9), "Wrong value returned.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_List1Null_Illegal () {
            List<ulong> list1 = null;

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            MfsOperations.Filter.CombineEXOR (list1, list2);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_List2Null_Illegal () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = null;

            MfsOperations.Filter.CombineEXOR (list1, list2);
        }

        [Test]
        public void Test_List1Empty_ReturnsList2 () {
            List<ulong> list1 = new List<ulong> ();

            List<ulong> list2 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list2.Add (i);
            }

            List<ulong> exoredList = MfsOperations.Filter.CombineEXOR (list1, list2);
            Assert.AreEqual (list2, exoredList, "Wrong values returned.");
        }

        [Test]
        public void Test_List2Empty_ReturnsList1 () {
            List<ulong> list1 = new List<ulong> ();
            for (ulong i = 1; i < 10; ++i) {
                list1.Add (i);
            }

            List<ulong> list2 = new List<ulong> ();

            List<ulong> exoredList = MfsOperations.Filter.CombineEXOR (list1, list2);
            Assert.AreEqual (list1, exoredList, "Wrong values returned.");
        }
    }
}
