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

namespace MnemonicFS.Tests.ByteStreams {
    [TestFixture]
    public class Tests_MfsOperations_StoreByteStream : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
            Assert.That (byteStreamID > 0, "Returned byte stream id is not valid.");

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullByteArray_Illegal () {
            _fileData = null;
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyByteArray_Illegal () {
            _fileData = new byte[0];
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = null;
            int refNo = 1;

            MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = string.Empty;
            int refNo = 1;

            MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
        }

        [Test]
        public void Test_AnyReferenceNumber_Legal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
            Assert.That (byteStreamID > 0, "Byte stream not stored even though any reference number is allowed.");

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_RetrieveByteStream : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            int returnedRefNo;
            byte[] returnedStream = MfsOperations.Bytestream.Retrieve (byteStreamID, passphrase, out returnedRefNo);
            Assert.AreEqual (_fileData.Length, returnedStream.Length, "Size of data returned is not correct.");
            Assert.AreEqual (refNo, returnedRefNo, "Reference number returned is not correct.");

            for (int i = 0; i < _fileData.Length; ++i) {
                Assert.AreEqual (_fileData[i], returnedStream[i], "Returned stream is not identical to the saved stream.");
            }

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int returnedRefNo;
            MfsOperations.Bytestream.Retrieve (0, passphrase, out returnedRefNo);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentByteStreamID_Illegal () {
            ulong veryLargeByteStreamID = UInt64.MaxValue;
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int returnedRefNo;
            MfsOperations.Bytestream.Retrieve (veryLargeByteStreamID, passphrase, out returnedRefNo);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            int returnedRefNo;
            string nullPassphrase = null;
            try {
                MfsOperations.Bytestream.Retrieve (byteStreamID, nullPassphrase, out returnedRefNo);
            } finally {
                MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            int returnedRefNo;
            string emptyPassphrase = string.Empty;
            try {
                MfsOperations.Bytestream.Retrieve (byteStreamID, emptyPassphrase, out returnedRefNo);
            } finally {
                MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
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

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            int returnedRefNo = MfsOperations.Bytestream.GetReferenceNumber (byteStreamID);
            Assert.AreEqual (refNo, returnedRefNo, "Reference number returned is not correct.");

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            MfsOperations.Bytestream.GetReferenceNumber (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentByteStreamID_Illegal () {
            ulong veryLargeByteStreamID = UInt64.MaxValue;
            MfsOperations.Bytestream.GetReferenceNumber (veryLargeByteStreamID);
        }
    }

    [TestFixture]
    public class Tests_MfsOperations_DeleteByteStream : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);

            bool byteStreamExists = MfsOperations.Bytestream.Exists (byteStreamID);
            Assert.IsFalse (byteStreamExists, "Byte stream not deleted properly.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            MfsOperations.Bytestream.Delete (0, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentByteStreamID_Illegal () {
            ulong veryLargeByteStreamID = UInt64.MaxValue;
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            MfsOperations.Bytestream.Delete (veryLargeByteStreamID, passphrase);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = null;
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);

            bool byteStreamExists = MfsOperations.Bytestream.Exists (byteStreamID);
            Assert.IsFalse (byteStreamExists, "Byte stream not deleted properly even though null passphrase is allowed.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyPassphrase_Illegal () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = null;
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            string emptyPassphrase = string.Empty;
            try {
                MfsOperations.Bytestream.Delete (byteStreamID, emptyPassphrase);
            } finally {
                MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
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

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            bool byteStreamExists = MfsOperations.Bytestream.Exists (byteStreamID);
            Assert.IsTrue (byteStreamExists, "Shows byte stream does not exist even though it does.");

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
        }

        [Test]
        public void Test_TrueWhenByteStreamExists () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);

            bool byteStreamExists = MfsOperations.Bytestream.Exists (byteStreamID);
            Assert.IsTrue (byteStreamExists, "Shows byte stream does not exist even though it does.");

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);
        }

        [Test]
        public void Test_FalseWhenByteStreamDoesNotExist () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            string passphrase = TestUtils.GetRandomString (TYPICAL_WORD_SIZE);
            int refNo = 1;

            ulong byteStreamID = MfsOperations.Bytestream.Store (_fileData, passphrase, refNo);
            bool byteStreamExists = MfsOperations.Bytestream.Exists (byteStreamID);
            Assert.IsTrue (byteStreamExists, "Shows byte stream does not exist even though it does.");

            MfsOperations.Bytestream.Delete (byteStreamID, passphrase);

            byteStreamExists = MfsOperations.Bytestream.Exists (byteStreamID);
            Assert.IsFalse (byteStreamExists, "Shows byte stream exists even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ByteStreamIDZero_Illegal () {
            MfsOperations.Bytestream.Exists (0);
        }
    }
}
