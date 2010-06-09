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

namespace MnemonicFS.Tests.Filters.Pipes {
    [TestFixture]
    public class Tests_Pipes : TestMfsOperationsBase {
        [Test]
        public void Test_NotInDateRange () {
            // Create three files:
            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1); // one year in the past
            ulong fileIDPast = _mfsOperations.SaveFile (fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            when = DateTime.Now;
            ulong fileIDCurrent = _mfsOperations.SaveFile (fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            when = DateTime.Now.AddYears (+1); // one year in the future
            ulong fileIDFuture = _mfsOperations.SaveFile (fileName3, fileNarration3, fileData3, when, false);

            // Presume we are looking for all files *not* within a 6 month time radius from now.

            // First retrieve all files within date time range that you *don't* need:
            List<ulong> unwantedFileIDs = _mfsOperations.GetFilesInDateRange (DateTime.Now.AddMonths (-6), DateTime.Now.AddMonths (6));

            // Then retrieve *all* files:
            List<ulong> allUserFileIDs = _mfsOperations.GetAllFiles ();

            // And "pipe" it to the invert filter to invert the selection:
            List<ulong> reqdFileIDs = MfsOperations.FilterInvert (allUserFileIDs, unwantedFileIDs);
            Assert.AreEqual (2, reqdFileIDs.Count, "Incorrect number of files retrieved.");

            foreach (ulong reqdFileID in reqdFileIDs) {
                if (!(reqdFileID == fileIDPast || reqdFileID == fileIDFuture)) {
                    Assert.Fail ("Incorrect file retrieved.");
                }
            }

            _mfsOperations.DeleteFile (fileIDPast);
            _mfsOperations.DeleteFile (fileIDCurrent);
            _mfsOperations.DeleteFile (fileIDFuture);
        }
        
        [Test]
        public void Test_InDateRange_AND_InAspects () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1); // one year in the past
            ulong fileIDPast = _mfsOperations.SaveFile (fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            when = DateTime.Now;
            ulong fileIDCurrent = _mfsOperations.SaveFile (fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            when = DateTime.Now.AddYears (+1); // one year in the future
            ulong fileIDFuture = _mfsOperations.SaveFile (fileName3, fileNarration3, fileData3, when, false);

            // Apply aspects to files:
            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDPast);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDPast);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDCurrent);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDFuture);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDFuture);

            // Presume we are looking for all files within a 6 month time radius from now,
            // AND
            // Files within aspectID2.

            // First retrieve all files within date range that you need:
            List<ulong> retrFileIDs = _mfsOperations.GetFilesInDateRange (DateTime.Now.AddMonths (-6), DateTime.Now.AddMonths (6));

            List<ulong> listFilterAspects = new List<ulong> ();
            listFilterAspects.Add (aspectID2);
            List<ulong> reqdFileIDs = _mfsOperations.FilterFilesWithinAspects (listFilterAspects, retrFileIDs, FilterType.AND);

            Assert.AreEqual (1, reqdFileIDs.Count, "Incorrect number of files retrieved.");
            Assert.AreEqual (fileIDCurrent, reqdFileIDs[0], "Incorrect file retrieved.");

            _mfsOperations.DeleteAspect (aspectID1);
            _mfsOperations.DeleteAspect (aspectID2);
            _mfsOperations.DeleteAspect (aspectID3);

            _mfsOperations.DeleteFile (fileIDPast);
            _mfsOperations.DeleteFile (fileIDCurrent);
            _mfsOperations.DeleteFile (fileIDFuture);
        }

        [Test]
        public void Test_InDateRange_AND_NotInAspects () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1); // one year in the past
            ulong fileIDPast = _mfsOperations.SaveFile (fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            when = DateTime.Now;
            ulong fileIDCurrent = _mfsOperations.SaveFile (fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            when = DateTime.Now.AddYears (+1); // one year in the future
            ulong fileIDFuture = _mfsOperations.SaveFile (fileName3, fileNarration3, fileData3, when, false);

            // Apply aspects to files:
            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDPast);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDPast);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDCurrent);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDFuture);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDFuture);

            // Presume we are looking for all files within a 6 month time radius from now,
            // AND
            // Files *not* within aspectID2.

            // First retrieve all files within date range that you need:
            List<ulong> retrFileIDs = _mfsOperations.GetFilesInDateRange (DateTime.Now.AddMonths (-6), DateTime.Now.AddMonths (6));

            // Get files within aspect that you *don't* need:
            List<ulong> listFilterAspects = new List<ulong> ();
            listFilterAspects.Add (aspectID2);
            List<ulong> unwantedFileIDs = _mfsOperations.FilterFilesWithinAspects (listFilterAspects, retrFileIDs, FilterType.AND);

            // And invert the selection:
            List<ulong> reqdFileIDs = MfsOperations.FilterInvert (retrFileIDs, unwantedFileIDs);
            Assert.AreEqual (0, reqdFileIDs.Count, "Files retrieved even though none expected in set.");

            _mfsOperations.DeleteAspect (aspectID1);
            _mfsOperations.DeleteAspect (aspectID2);
            _mfsOperations.DeleteAspect (aspectID3);

            _mfsOperations.DeleteFile (fileIDPast);
            _mfsOperations.DeleteFile (fileIDCurrent);
            _mfsOperations.DeleteFile (fileIDFuture);
        }

        [Test]
        public void Test_NotBeforeDate_OR_InAspects () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1); // one year in the past
            ulong fileIDPast = _mfsOperations.SaveFile (fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            when = DateTime.Now;
            ulong fileIDCurrent = _mfsOperations.SaveFile (fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            when = DateTime.Now.AddYears (+1); // one year in the future
            ulong fileIDFuture = _mfsOperations.SaveFile (fileName3, fileNarration3, fileData3, when, false);

            // Apply aspects to files:
            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDPast);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDPast);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDCurrent);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDFuture);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDFuture);

            // Presume we are looking for all files *not* older than 6 months,
            // OR
            // Files within aspectID2.

            List<ulong> unwantedFileIDs = _mfsOperations.GetFilesBeforeDate (DateTime.Now.AddMonths (-6));
            List<ulong> allFileIDs = _mfsOperations.GetAllFiles ();
            List<ulong> retrFileIDs1 = MfsOperations.FilterInvert (allFileIDs, unwantedFileIDs);

            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID2);
            List<ulong> retrFileIDs2 = _mfsOperations.FilterFilesWithinAspects (aspectIDs, allFileIDs, FilterType.AND);

            List<ulong> reqdFileIDs = MfsOperations.FilterCombineOR (retrFileIDs1, retrFileIDs2);

            Assert.AreEqual (3, reqdFileIDs.Count, "Incorrect number of files returned.");

            Assert.IsTrue (reqdFileIDs.Contains (fileIDPast), "Expected file not returned.");
            Assert.IsTrue (reqdFileIDs.Contains (fileIDCurrent), "Expected file not returned.");
            Assert.IsTrue (reqdFileIDs.Contains (fileIDFuture), "Expected file not returned.");

            _mfsOperations.DeleteAspect (aspectID1);
            _mfsOperations.DeleteAspect (aspectID2);
            _mfsOperations.DeleteAspect (aspectID3);

            _mfsOperations.DeleteFile (fileIDPast);
            _mfsOperations.DeleteFile (fileIDCurrent);
            _mfsOperations.DeleteFile (fileIDFuture);
        }

        [Test]
        public void Test_AfterDateTime_OR_InAspects () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1); // one year in the past
            ulong fileIDPast = _mfsOperations.SaveFile (fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            when = DateTime.Now;
            ulong fileIDCurrent = _mfsOperations.SaveFile (fileName2, fileNarration2, fileData2, when, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            when = DateTime.Now.AddYears (+1); // one year in the future
            ulong fileIDFuture = _mfsOperations.SaveFile (fileName3, fileNarration3, fileData3, when, false);

            // Apply aspects to files:
            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDPast);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDPast);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDCurrent);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDFuture);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDFuture);

            // Presume we are looking for all files 6 months after current date and time,
            // OR
            // Files within aspectID2.

            List<ulong> retrFileIDs1 = _mfsOperations.GetFilesAfterDateTime (DateTime.Now.AddMonths (6));
            List<ulong> allFileIDs = _mfsOperations.GetAllFiles ();
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID2);
            List<ulong> retrFileIDs2 = _mfsOperations.FilterFilesWithinAspects (aspectIDs, allFileIDs, FilterType.AND);

            List<ulong> reqdFileIDs = MfsOperations.FilterCombineOR (retrFileIDs1, retrFileIDs2);
            Assert.AreEqual (2, reqdFileIDs.Count, "Incorrect number of files returned.");

            Assert.IsTrue (reqdFileIDs.Contains (fileIDPast), "Expected file not returned.");
            Assert.IsTrue (reqdFileIDs.Contains (fileIDFuture), "Expected file not returned.");

            _mfsOperations.DeleteAspect (aspectID1);
            _mfsOperations.DeleteAspect (aspectID2);
            _mfsOperations.DeleteAspect (aspectID3);

            _mfsOperations.DeleteFile (fileIDPast);
            _mfsOperations.DeleteFile (fileIDCurrent);
            _mfsOperations.DeleteFile (fileIDFuture);
        }

        [Test]
        public void Test_AtDateTime_AND_InAspects () {
            // Create three aspects:
            string aspectName1, aspectDesc1;
            ulong aspectID1 = CreateUniqueAspect (ref _mfsOperations, out aspectName1, out aspectDesc1);
            string aspectName2, aspectDesc2;
            ulong aspectID2 = CreateUniqueAspect (ref _mfsOperations, out aspectName2, out aspectDesc2);
            string aspectName3, aspectDesc3;
            ulong aspectID3 = CreateUniqueAspect (ref _mfsOperations, out aspectName3, out aspectDesc3);

            // Create three files:
            string fileName1 = TestUtils.GetAnyFileName ();
            string fileNarration1 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData1 = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
            DateTime when = DateTime.Now.AddYears (-1); // one year in the past
            ulong fileIDPast = _mfsOperations.SaveFile (fileName1, fileNarration1, fileData1, when, false);

            string fileName2 = TestUtils.GetAnyFileName ();
            string fileNarration2 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData2 = TestUtils.GetAnyFileData (FileSize.MEDIUM_FILE_SIZE);
            DateTime whenNow = DateTime.Now;
            ulong fileIDCurrent = _mfsOperations.SaveFile (fileName2, fileNarration2, fileData2, whenNow, false);

            string fileName3 = TestUtils.GetAnyFileName ();
            string fileNarration3 = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
            byte[] fileData3 = TestUtils.GetAnyFileData (FileSize.LARGE_FILE_SIZE);
            when = DateTime.Now.AddYears (+1); // one year in the future
            ulong fileIDFuture = _mfsOperations.SaveFile (fileName3, fileNarration3, fileData3, when, false);

            // Apply aspects to files:
            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDPast);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDPast);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDCurrent);
            _mfsOperations.ApplyAspectToFile (aspectID3, fileIDCurrent);

            _mfsOperations.ApplyAspectToFile (aspectID1, fileIDFuture);
            _mfsOperations.ApplyAspectToFile (aspectID2, fileIDFuture);

            // Presume we are looking for all files at current date and time,
            // AND
            // Files within aspectID2.

            List<ulong> retrFileIDs1 = _mfsOperations.GetFilesAtDateTime (whenNow);
            List<ulong> aspectIDs = new List<ulong> ();
            aspectIDs.Add (aspectID2);
            List<ulong> reqdFileIDs = _mfsOperations.FilterFilesWithinAspects (aspectIDs, retrFileIDs1, FilterType.AND);

            Assert.AreEqual (0, reqdFileIDs.Count, "Incorrect number of files returned.");

            _mfsOperations.DeleteAspect (aspectID1);
            _mfsOperations.DeleteAspect (aspectID2);
            _mfsOperations.DeleteAspect (aspectID3);

            _mfsOperations.DeleteFile (fileIDPast);
            _mfsOperations.DeleteFile (fileIDCurrent);
            _mfsOperations.DeleteFile (fileIDFuture);
        }

        [Test]
        public void Test_InDateRange_AND_NotInYear () {
            // First add some files in years from 2001 through 2010:
            List<ulong> fileIDs = new List<ulong> ();
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            for (int year = 2001; year <= 2010; ++year) {
                DateTime when = new DateTime (year, month, day);
                string fileName = TestUtils.GetAnyFileName ();
                string fileNarration = TestUtils.GetASentence (TYPICAL_SENTENCE_SIZE, TYPICAL_WORD_SIZE);
                byte[] fileData = TestUtils.GetAnyFileData (FileSize.SMALL_FILE_SIZE);
                ulong fileID = _mfsOperations.SaveFile (fileName, fileNarration, fileData, when, false);
                fileIDs.Add (fileID);
            }
            
            // Presume that we are looking for all files between the years 2001 and 2010 (including both)
            // but *excluding* year 2005:

            // First, retrieve all the files between 2001 and 2010:
            DateTime startDate = new DateTime (2001, 1, 1);
            DateTime endDate = new DateTime (2010, 12, 31);
            // The o/p list below should have 10 files:
            List<ulong> allFilesInRange = _mfsOperations.GetFilesInDateRange (startDate, endDate);

            // Next, get all files in the year 2005:
            DateTime unwantedStartDate = new DateTime (2005, 1, 1);
            DateTime unwantedEndDate = new DateTime (2005, 12, 31);
            // The o/p list below should have 1 file:
            List<ulong> unwantedFiles = _mfsOperations.GetFilesInDateRange (unwantedStartDate, unwantedEndDate);

            // Now EXOR both the lists to get the required files:
            List<ulong> reqdFiles = MfsOperations.FilterCombineEXOR (allFilesInRange, unwantedFiles);
            Assert.AreEqual (9, reqdFiles.Count, "Wrong number of files returned.");

            foreach (ulong fileID in fileIDs) {
                _mfsOperations.DeleteFile (fileID);
            }
        }
    }
}
