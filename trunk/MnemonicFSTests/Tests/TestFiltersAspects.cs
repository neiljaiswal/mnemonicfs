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

namespace MnemonicFS.Tests.Filters.Aspects {
    [TestFixture]
    public class Tests_AspectsFilters_ANDOperation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply all aspects to first file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID1);
            _mfsOperations.Aspect.Apply (aspectID2, fileID1);
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);

            // Apply only two aspects to second file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID2);
            _mfsOperations.Aspect.Apply (aspectID3, fileID2);

            // Apply only one aspect to third file:
            _mfsOperations.Aspect.Apply (aspectID2, fileID3);

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply AND filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.AND);
            Assert.AreEqual (1, filteredFileIDs.Count, "Filter returned incorrect number of files.");

            ulong retrFileID = filteredFileIDs[0];
            Assert.AreEqual (fileID1, retrFileID, "Filter returned wrong file.");

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }

        [Test]
        public void Test_SanityCheck_WithTwoOutputFiles () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply all aspects to first file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID1);
            _mfsOperations.Aspect.Apply (aspectID2, fileID1);
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);

            // Apply only two aspects to second file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID2);
            _mfsOperations.Aspect.Apply (aspectID3, fileID2);

            // Apply all aspects to third file too:
            _mfsOperations.Aspect.Apply (aspectID1, fileID3);
            _mfsOperations.Aspect.Apply (aspectID2, fileID3);
            _mfsOperations.Aspect.Apply (aspectID3, fileID3);

            // At this point, fileID1 and fileID2 have been applied all three aspects.

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply AND filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.AND);
            Assert.AreEqual (2, filteredFileIDs.Count, "Filter returned incorrect number of files.");

            foreach (ulong retrFileID in filteredFileIDs) {
                if (!(retrFileID == fileID1 || retrFileID == fileID3)) {
                    Assert.Fail ("Filter returned wrong file.");
                }
            }

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }

        [Test]
        public void Test_SanityCheck_WithThreeOutputFiles () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply all aspects to all three files:
            _mfsOperations.Aspect.Apply (aspectID1, fileID1);
            _mfsOperations.Aspect.Apply (aspectID2, fileID1);
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);

            _mfsOperations.Aspect.Apply (aspectID1, fileID2);
            _mfsOperations.Aspect.Apply (aspectID2, fileID2);
            _mfsOperations.Aspect.Apply (aspectID3, fileID2);

            _mfsOperations.Aspect.Apply (aspectID1, fileID3);
            _mfsOperations.Aspect.Apply (aspectID2, fileID3);
            _mfsOperations.Aspect.Apply (aspectID3, fileID3);

            // At this point, all files have been applied all three aspects.

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply AND filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.AND);
            Assert.AreEqual (3, filteredFileIDs.Count, "Filter returned incorrect number of files.");

            foreach (ulong retrFileID in filteredFileIDs) {
                if (!(retrFileID == fileID1 || retrFileID == fileID2 || retrFileID == fileID3)) {
                    Assert.Fail ("Filter returned wrong file.");
                }
            }

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }

        [Test]
        public void Test_SanityCheck_WithNoOutputFiles () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply only one aspect to each of the files:
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);
            _mfsOperations.Aspect.Apply (aspectID2, fileID2);
            _mfsOperations.Aspect.Apply (aspectID1, fileID3);

            // At this point, all files have been applied all three aspects.

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply AND filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.AND);
            Assert.IsEmpty (filteredFileIDs, "Filter returned incorrect number of files.");

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }
    }

    [TestFixture]
    public class Tests_AspectsFilters_OROperation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply two aspects to first file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID1);
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);

            // Apply only one aspect to second file:
            _mfsOperations.Aspect.Apply (aspectID3, fileID2);

            // Apply zero aspects to third file:

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply OR filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.OR);
            Assert.AreEqual (2, filteredFileIDs.Count, "Filter returned incorrect number of files.");

            foreach (ulong retrFileID in filteredFileIDs) {
                if (!(retrFileID == fileID1 || retrFileID == fileID2)) {
                    Assert.Fail ("Filter returned wrong file.");
                }
            }

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }
    }

    [TestFixture]
    public class Tests_AspectsFilters_NANDOperation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply all aspects to first file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID1);
            _mfsOperations.Aspect.Apply (aspectID2, fileID1);
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);

            // Apply only two aspects to second file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID2);
            _mfsOperations.Aspect.Apply (aspectID3, fileID2);

            // Apply only one aspect to third file:
            _mfsOperations.Aspect.Apply (aspectID2, fileID3);

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply AND filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.AND);

            // And invert this o/p to get NAND filter:
            List<ulong> nandFilteredFileIDs = MfsOperations.Filter.Invert (fileIDs, filteredFileIDs);

            Assert.AreEqual (2, nandFilteredFileIDs.Count, "Filter returned incorrect number of files.");

            foreach (ulong retrFileID in nandFilteredFileIDs) {
                if (!(retrFileID == fileID2 || retrFileID == fileID3)) {
                    Assert.Fail ("Filter returned wrong file.");
                }
            }

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }
    }

    [TestFixture]
    public class Tests_AspectsFilters_NOROperation : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            DateTime when = DateTime.Now;

            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            ulong fileID1 = SaveFileToMfs (ref _mfsOperations, fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            ulong fileID2 = SaveFileToMfs (ref _mfsOperations, fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            ulong fileID3 = SaveFileToMfs (ref _mfsOperations, fileName3, fileNarration3, fileData3, when, false);

            // Apply two aspects to first file:
            _mfsOperations.Aspect.Apply (aspectID1, fileID1);
            _mfsOperations.Aspect.Apply (aspectID3, fileID1);

            // Apply only one aspect to second file:
            _mfsOperations.Aspect.Apply (aspectID3, fileID2);

            // Apply zero aspects to third file:

            // Put all aspects in a list:
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID1);
            aspectIDs.Add (aspectID2);
            aspectIDs.Add (aspectID3);

            // And also put all files within a list:
            List<ulong> fileIDs = new List<ulong> ();
            fileIDs.Add (fileID1);
            fileIDs.Add (fileID2);
            fileIDs.Add (fileID3);

            // Now apply OR filter:
            List<ulong> filteredFileIDs = _mfsOperations.Aspect.FilterFilesWithin (aspectIDs, fileIDs, FilterType.OR);

            // And invert this o/p to get NOR filter:
            List<ulong> norFilteredFileIDs = MfsOperations.Filter.Invert (fileIDs, filteredFileIDs);
            Assert.AreEqual (1, norFilteredFileIDs.Count, "Filter returned incorrect number of files.");
            Assert.AreEqual (fileID3, norFilteredFileIDs[0], "Filter returned wrong file.");

            _mfsOperations.Aspect.Delete (aspectID1);
            _mfsOperations.Aspect.Delete (aspectID2);
            _mfsOperations.Aspect.Delete (aspectID3);

            _mfsOperations.File.Delete (fileID1);
            _mfsOperations.File.Delete (fileID2);
            _mfsOperations.File.Delete (fileID3);
        }
    }
}
