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

namespace MnemonicFS.Tests.SchemaFreeDocs {
    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_CreateSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            Assert.That (docID > 0, "Schema-free document not created successfully: Invalid document id returned.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateDocName_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            try {
                _mfsOperations.CreateSfd (_schemaFreeDocName, when);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        public void Test_DocNameWithMaxSizeAllowed_SanityCheck () {
            string docName = TestUtils.GetAWord (MfsOperations.MaxSchemaFreeDocNameLength);
            DateTime when = DateTime.Now;

            ulong docID = _mfsOperations.CreateSfd (docName, when);

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocName_Illegal () {
            string nullDocName = null;
            DateTime when = DateTime.Now;

            _mfsOperations.CreateSfd (nullDocName, when);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocName_Illegal () {
            string emptyDocName = string.Empty;
            DateTime when = DateTime.Now;

            _mfsOperations.CreateSfd (emptyDocName, when);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocNameGreaterThanMaxSizeAllowed_Illegal () {
            int maxCharsInDocName = MfsOperations.MaxSchemaFreeDocNameLength;

            string longDocName = TestUtils.GetAWord (maxCharsInDocName + 1);
            DateTime when = DateTime.Now;

            _mfsOperations.CreateSfd (longDocName, when);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_DoesSfdExist : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck_Exists () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            bool docExists = _mfsOperations.DoesSfdExist (docID);
            Assert.IsTrue (docExists, "Schema-free document was shown as not existing, even though it does.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        public void Test_SanityCheck_NotExists () {
            ulong veryLargeDocID = ulong.MaxValue;

            bool docExists = _mfsOperations.DoesSfdExist (veryLargeDocID);
            Assert.IsFalse (docExists, "Schema-free document was shown as existing, even though it does not.");
        }

        [Test]
        public void Test_SanityCheck_WithDocName_Exists () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            bool docExists = _mfsOperations.DoesSfdExist (_schemaFreeDocName);
            Assert.IsTrue (docExists, "Schema-free document was shown as not existing, even though it does.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        public void Test_SanityCheck_WithDocName_NotExists () {
            string someDocName = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);

            bool docExists = _mfsOperations.DoesSfdExist (someDocName);
            Assert.IsFalse (docExists, "Schema-free document was shown as existing, even though it does not.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            _mfsOperations.DoesSfdExist (0);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocNameNull_Illegal () {
            _mfsOperations.DoesSfdExist (null);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocNameSizeZero_Illegal () {
            _mfsOperations.DoesSfdExist (string.Empty);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_DeleteSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            int docsDeleted = _mfsOperations.DeleteSfd (docID);
            Assert.AreEqual (1, docsDeleted, "Schema-free document was not deleted.");
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            _mfsOperations.DeleteSfd (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.DeleteSfd (veryLargeDocID);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_GetSfdName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string docName;
            _mfsOperations.GetSfdName (docID, out docName);
            Assert.AreEqual (_schemaFreeDocName, docName, "Did not retrieve existing schema-free document's name.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string docName;

            _mfsOperations.GetSfdName (0, out docName);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            string docName;
            _mfsOperations.GetSfdName (veryLargeDocID, out docName);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_GetSfdIDFromName : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = CreateUniqueSchemaFreeDocument (ref _mfsOperations, out _schemaFreeDocName, when);

            ulong retrDocID = _mfsOperations.GetSfdIDFromName (_schemaFreeDocName);
            Assert.AreEqual (docID, retrDocID, "Wrong schema-free document ID retrieved.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocName_Illegal () {
            string nonExistentDocName = null;

            do {
                nonExistentDocName = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesSfdExist (nonExistentDocName));

            _mfsOperations.GetSfdIDFromName (nonExistentDocName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NullDocName_Illegal () {
            string nullDocName = null;

            _mfsOperations.GetSfdIDFromName (nullDocName);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_EmptyDocName_Illegal () {
            string emptyDocName = string.Empty;

            _mfsOperations.GetSfdIDFromName (emptyDocName);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_GetSfdSaveDateTime : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            DateTime retrWhen = _mfsOperations.GetSfdSaveDateTime (docID);
            Assert.AreEqual (when.Year, retrWhen.Year, "Retrieved date/time year for schema-free document does not match the saved date/time year.");
            Assert.AreEqual (when.Month, retrWhen.Month, "Retrieved date/time month for schema-free document does not match the saved date/time month.");
            Assert.AreEqual (when.Day, retrWhen.Day, "Retrieved date/time day for schema-free document does not match the saved date/time day.");
            Assert.AreEqual (when.Hour, retrWhen.Hour, "Retrieved date/time hour for schema-free document does not match the saved date/time hour.");
            Assert.AreEqual (when.Minute, retrWhen.Minute, "Retrieved date/time minute for schema-free document does not match the saved date/time minute.");
            Assert.AreEqual (when.Second, retrWhen.Second, "Retrieved date/time second for schema-free document does not match the saved date/time second.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            _mfsOperations.GetSfdSaveDateTime (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.GetSfdSaveDateTime (veryLargeDocID);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_GetAllSfds : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            List<ulong> listSchemaFreeDocs = CreateUniqueNSchemaFreeDocuments (ref _mfsOperations, TYPICAL_MULTI_VALUE, when);

            List<ulong> retrSchemaFreeDocIDs = _mfsOperations.GetAllSfds ();

            listSchemaFreeDocs.Sort ();
            retrSchemaFreeDocIDs.Sort ();

            Assert.AreEqual (listSchemaFreeDocs.Count, retrSchemaFreeDocIDs.Count, "Wrong number of schema-free document ids retrieved.");
            Assert.AreEqual (listSchemaFreeDocs, retrSchemaFreeDocIDs, "Wrong schema-free document ids recovered.");

            foreach (ulong docID in listSchemaFreeDocs) {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_DeleteAllSfds : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            List<ulong> listSchemaFreeDocs = CreateUniqueNSchemaFreeDocuments (ref _mfsOperations, TYPICAL_MULTI_VALUE, when);

            List<ulong> retrSchemaFreeDocIDs = _mfsOperations.GetAllSfds ();
            Assert.AreEqual (listSchemaFreeDocs.Count, retrSchemaFreeDocIDs.Count, "Number of schema-free documents created is not the same as expected.");

            _mfsOperations.DeleteAllSfds ();

            retrSchemaFreeDocIDs = _mfsOperations.GetAllSfds ();
            Assert.AreEqual (0, retrSchemaFreeDocIDs.Count, "Did not delete all schema-free documents.");
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_AddPropertyToSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = null;

            do {
                key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesSfdHaveKey (docID, key));

            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateKey_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = null;
            do {
                key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesSfdHaveKey (docID, key));
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            try {
                _mfsOperations.AddPropertyToSfd (docID, key, value);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (0, key, value);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.AddPropertyToSfd (veryLargeDocID, key, value);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = null;
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.AddPropertyToSfd (docID, key, value);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = string.Empty;
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.AddPropertyToSfd (docID, key, value);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ValueNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = null;

            try {
                _mfsOperations.AddPropertyToSfd (docID, key, value);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ValueEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = string.Empty;

            try {
                _mfsOperations.AddPropertyToSfd (docID, key, value);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_AddPropertiesToSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            List<string> listUniqueKeys = TestUtils.GetUniqueNStrings (TYPICAL_MULTI_VALUE, TYPICAL_WORD_SIZE);
            // We are not really going to bother about the value of, well, value, here, since it need not be unique.
            // So we will use the same value across all keys.
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            Dictionary<string, string> properties = new Dictionary<string, string> (TYPICAL_MULTI_VALUE);
            foreach (string key in listUniqueKeys) {
                properties.Add (key, value);
            }

            _mfsOperations.AddPropertiesToSfd (docID, properties);

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_DuplicateKeysInArgument_Illegal () {
            // We do not want to bother about this test for the simple reason that a Dictionary does not
            // accept duplicate keys. So we throw an exception right away to pass this test:
            throw new MfsDuplicateNameException ();
        }

        [Test]
        [ExpectedException (typeof (MfsDuplicateNameException))]
        public void Test_AddPreExistingKey_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = null;
            do {
                key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            } while (_mfsOperations.DoesSfdHaveKey (docID, key));
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            Dictionary<string, string> properties = new Dictionary<string, string> (TYPICAL_MULTI_VALUE);
            properties.Add (key, value);

            try {
                _mfsOperations.AddPropertiesToSfd (docID, properties);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            Dictionary<string, string> properties = new Dictionary<string, string> ();
            properties.Add (key, value);

            _mfsOperations.AddPropertiesToSfd (0, properties);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            Dictionary<string, string> properties = new Dictionary<string, string> ();
            properties.Add (key, value);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.AddPropertiesToSfd (veryLargeDocID, properties);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyNull_Illegal () {
            // We do not want to bother about this test for the simple reason that a Dictionary does not
            // accept null keys. So we throw an exception right away to pass this test:
            throw new MfsIllegalArgumentException ();
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = string.Empty;
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            Dictionary<string, string> properties = new Dictionary<string, string> ();
            properties.Add (key, value);

            try {
                _mfsOperations.AddPropertiesToSfd (docID, properties);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ValueNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = null;
            Dictionary<string, string> properties = new Dictionary<string, string> ();
            properties.Add (key, value);

            try {
                _mfsOperations.AddPropertiesToSfd (docID, properties);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_ValueEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = string.Empty;
            Dictionary<string, string> properties = new Dictionary<string, string> ();
            properties.Add (key, value);

            try {
                _mfsOperations.AddPropertiesToSfd (docID, properties);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_DoesSfdHaveKey : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            bool hasKey = _mfsOperations.DoesSfdHaveKey (docID, key);
            Assert.IsTrue (hasKey, "Schema-free document was shown as not having key even though it does.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        public void Test_SanityCheck_NoKeyFalse () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            
            bool hasKey = _mfsOperations.DoesSfdHaveKey (docID, key);
            Assert.IsFalse (hasKey, "Schema-free document was shown as having key even though it doesn't.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.DoesSfdHaveKey (0, key);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.DoesSfdHaveKey (veryLargeDocID, key);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nullKey = null;
            try {
                _mfsOperations.DoesSfdHaveKey (docID, nullKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string emptyKey = string.Empty;
            try {
                _mfsOperations.DoesSfdHaveKey (docID, emptyKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_GetValueForKeyInSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            string retrValue = _mfsOperations.GetValueForKeyInSfd (docID, key);
            Assert.AreEqual (value, retrValue, "Retrieved value of key in schema-free document is not as expected.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.GetValueForKeyInSfd (0, key);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.GetValueForKeyInSfd (veryLargeDocID, key);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentKey_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nonExistentKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.GetValueForKeyInSfd (docID, nonExistentKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nullKey = null;
            try {
                _mfsOperations.GetValueForKeyInSfd (docID, nullKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string emptyKey = string.Empty;
            try {
                _mfsOperations.GetValueForKeyInSfd (docID, emptyKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_UpdateValueForKeyInSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            string newValue = TestUtils.GetAWord (TYPICAL_WORD_SIZE * 2);
            _mfsOperations.UpdateValueForKeyInSfd (docID, key, newValue);

            string retrValue = _mfsOperations.GetValueForKeyInSfd (docID, key);
            Assert.AreEqual (newValue, retrValue, "Value of key in schema-free document not updated to new value.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string newValue = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.UpdateValueForKeyInSfd (0, key, newValue);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string newValue = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.UpdateValueForKeyInSfd (veryLargeDocID, key, newValue);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentKey_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nonExistentKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string newValue = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.UpdateValueForKeyInSfd (docID, nonExistentKey, newValue);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nullKey = null;
            string newValue = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            try {
                _mfsOperations.UpdateValueForKeyInSfd (docID, nullKey, newValue);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string emptyKey = string.Empty;
            string newValue = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            try {
                _mfsOperations.UpdateValueForKeyInSfd (docID, emptyKey, newValue);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NewValueNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            string nullNewValue = null;
            try {
                _mfsOperations.UpdateValueForKeyInSfd (docID, key, nullNewValue);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_NewValueEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            string emptyNewValue = string.Empty;
            try {
                _mfsOperations.UpdateValueForKeyInSfd (docID, key, emptyNewValue);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_DeleteKeyInSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.AddPropertyToSfd (docID, key, value);

            bool hasKey = _mfsOperations.DoesSfdHaveKey (docID, key);
            Assert.IsTrue (hasKey, "Schema-free document was shown as not having key even though it does.");

            _mfsOperations.DeleteKeyInSfd (docID, key);
            hasKey = _mfsOperations.DoesSfdHaveKey (docID, key);
            Assert.IsFalse (hasKey, "Schema-free document was shown as having key even though it doesn't.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.DeleteKeyInSfd (0, key);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.DeleteKeyInSfd (veryLargeDocID, key);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentKey_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nonExistentKey = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            try {
                _mfsOperations.DeleteKeyInSfd (docID, nonExistentKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyNull_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string nullKey = null;
            try {
                _mfsOperations.DeleteKeyInSfd (docID, nullKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_KeyEmpty_Illegal () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            string emptyKey = string.Empty;
            try {
                _mfsOperations.DeleteKeyInSfd (docID, emptyKey);
            } finally {
                _mfsOperations.DeleteSfd (docID);
            }
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_GetAllKeysInSfd : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
            DateTime when = DateTime.Now;
            ulong docID = _mfsOperations.CreateSfd (_schemaFreeDocName, when);

            List<string> listUniqueKeys = TestUtils.GetUniqueNStrings (TYPICAL_MULTI_VALUE, TYPICAL_WORD_SIZE);
            string value = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            Dictionary<string, string> properties = new Dictionary<string, string> (TYPICAL_MULTI_VALUE);
            foreach (string key in listUniqueKeys) {
                properties.Add (key, value);
            }

            _mfsOperations.AddPropertiesToSfd (docID, properties);

            List<string> retrKeys = _mfsOperations.GetAllKeysInSfd (docID);
            Assert.AreEqual (listUniqueKeys, retrKeys, "Retrieved keys in schema-free document are not the same as expected.");

            _mfsOperations.DeleteSfd (docID);
        }

        [Test]
        [ExpectedException (typeof (MfsIllegalArgumentException))]
        public void Test_DocIDZero_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            _mfsOperations.GetAllKeysInSfd (0);
        }

        [Test]
        [ExpectedException (typeof (MfsNonExistentResourceException))]
        public void Test_NonExistentDocID_Illegal () {
            string key = TestUtils.GetAWord (TYPICAL_WORD_SIZE);

            ulong veryLargeDocID = UInt64.MaxValue;

            _mfsOperations.GetAllKeysInSfd (veryLargeDocID);
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_ExportSfdToJSON : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
        }
    }

    [TestFixture]
    public class Tests_SchemaFreeDocumentsMethod_CreateSfdNextVersion : TestMfsOperationsBase {
        [Test]
        public void Test_SanityCheck () {
        }
    }
}
