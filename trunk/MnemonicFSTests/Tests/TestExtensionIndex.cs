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
using MnemonicFS.MfsCore;
using MnemonicFS.Tests.Utils;
using MnemonicFS.MfsUtils.MfsCrypto;
using System.IO;
using System.Threading;
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.Tests.ExtensionIndex {
    [TestFixture]
    public class TestExtensionIndex_MethodName_GetFilesWithExtension : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            // Add three different file with various extensions:
            string fileName1 = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + "." + PDF_FILE_EXTENSION;
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, _fileNarration, _fileData, when, false);

            string fileName2 = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + "." + MSDOC_FILE_EXTENSION;
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, _fileNarration, _fileData, when, false);

            string fileName3 = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + "." + PDF_FILE_EXTENSION;
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, _fileNarration, _fileData, when, false);

            List<ulong> fileIDs1 = _mfsOperations.GetFilesWithExtension (PDF_FILE_EXTENSION);
            Assert.AreEqual (2, fileIDs1.Count, "Did not return the actual number of files with extension.");
            Assert.That (fileIDs1.Contains (fileID1), "Returned file list does not contain expected file.");
            Assert.That (fileIDs1.Contains (fileID3), "Returned file list does not contain expected file.");

            List<ulong> fileIDs2 = _mfsOperations.GetFilesWithExtension (MSDOC_FILE_EXTENSION);
            Assert.AreEqual (1, fileIDs2.Count, "Did not return the actual number of files with extension.");
            Assert.That (fileIDs2.Contains (fileID2), "Returned file list does not contain expected file.");

            List<ulong> fileIDs3 = _mfsOperations.GetFilesWithExtension (MSEXCEL_FILE_EXTENSION);
            Assert.AreEqual (0, fileIDs3.Count, "Returned file/s with extension though none exist.");

            _mfsOperations.DeleteFile (fileID1);
            _mfsOperations.DeleteFile (fileID2);
            _mfsOperations.DeleteFile (fileID3);
        }

        [Test]
        public void Test_CaseInsensitiveIndexing () {
            _fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now;

            string fileName = TestUtils.GetAWord (TYPICAL_WORD_SIZE) + "." + PDF_FILE_EXTENSION;
            ulong fileID = SaveFileToMfs (ref _mfsOperations, fileName, _fileNarration, _fileData, when, false);

            List<ulong> fileIDs = _mfsOperations.GetFilesWithExtension (PDF_FILE_EXTENSION);
            Assert.That (fileIDs.Contains (fileID), "Returned file list does not contain expected file.");

            // Search with upper-case extension:
            fileIDs = _mfsOperations.GetFilesWithExtension (PDF_FILE_EXTENSION.ToUpper ());
            Assert.That (fileIDs.Contains (fileID), "Returned file list does not contain expected file.");

            _mfsOperations.DeleteFile (fileID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullString_Illegal () {
            _mfsOperations.GetFilesWithExtension (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyString_Illegal () {
            _mfsOperations.GetFilesWithExtension (string.Empty);
        }
    }
}
