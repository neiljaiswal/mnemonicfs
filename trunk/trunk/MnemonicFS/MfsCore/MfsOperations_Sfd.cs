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
using MnemonicFS.MfsExceptions;

namespace MnemonicFS.MfsCore {
    public partial class MfsOperations {
        [Serializable]
        public class _Sfd : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Sfd (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Sfd _theObject = null;

            internal static _Sfd GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Sfd (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoSfdChecks (ulong docID) {
                if (docID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Schema-free document id")
                    );
                }

                if (!_dbOperations.DoesSfdExist (docID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "Schema-free document")
                    );
                }
            }

            internal void DoSfdChecks (string docName) {
                if (docName == null || docName.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Schema-free document name")
                    );
                }

                // NO! Don't check for schema-free document existence here.
            }

            #endregion << Client-input Check Methods >>

            #region << Schema Free Document-related Operations >>

            public ulong New (string docName, DateTime when) {
                ValidateString (docName, ValidationCheckType.SCHEMA_FREE_DOC_NAME);
                DoSfdChecks (docName);
                if (Exists (docName) == true) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Schema-free document name")
                    );
                }

                return _dbOperations.CreateSfd (docName, when);
            }

            public int Delete (ulong docID) {
                DoSfdChecks (docID);

                return _dbOperations.DeleteSfd (docID);
            }

            public int DeleteAll () {
                return _dbOperations.DeleteAllSfds ();
            }

            public bool Exists (ulong docID) {
                if (docID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Schema-free document id")
                    );
                }

                return _dbOperations.DoesSfdExist (docID);
            }

            public bool Exists (string docName) {
                DoSfdChecks (docName);

                return _dbOperations.DoesSfdExist (docName);
            }

            public void GetName (ulong docID, out string docName) {
                DoSfdChecks (docID);

                _dbOperations.GetSfdName (docID, out docName);
            }

            public ulong IDFromName (string docName) {
                DoSfdChecks (docName);
                if (!Exists (docName)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Schema-free document name")
                    );
                }

                return _dbOperations.GetSfdIDFromName (docName);
            }

            public DateTime GetSaveDateTime (ulong docID) {
                DoSfdChecks (docID);

                return _dbOperations.GetSfdSaveDateTime (docID);
            }

            public List<ulong> All () {
                return _dbOperations.GetAllSfds ();
            }

            public void AddPropertyTo (ulong docID, string key, string value) {
                DoSfdChecks (docID);
                if (key == null || key.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Key")
                    );
                }
                if (value == null || value.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Value")
                    );
                }
                if (_dbOperations.DoesSfdHaveKey (docID, key)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Document key")
                    );
                }

                _dbOperations.AddPropertyToSfd (docID, key, value);
            }

            public void AddPropertiesTo (ulong docID, Dictionary<string, string> properties) {
                DoSfdChecks (docID);
                HashSet<string> allKeys = new HashSet<string> (properties.Keys);
                foreach (string key in allKeys) {
                    if (key == null || key.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Key")
                        );
                    }
                    string value = properties[key];
                    if (value == null || value.Equals (string.Empty)) {
                        throw new MfsIllegalArgumentException (
                            MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Value")
                        );
                    }
                    if (_dbOperations.DoesSfdHaveKey (docID, key)) {
                        throw new MfsDuplicateNameException (
                            MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Document key")
                        );
                    }
                }

                foreach (string key in allKeys) {
                    string value = properties[key];
                    _dbOperations.AddPropertyToSfd (docID, key, value);
                }
            }

            public bool HasKey (ulong docID, string key) {
                DoSfdChecks (docID);
                if (key == null || key.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Key")
                    );
                }

                return _dbOperations.DoesSfdHaveKey (docID, key);
            }

            public string GetValue (ulong docID, string key) {
                DoSfdChecks (docID);
                if (key == null || key.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Key")
                    );
                }
                if (!_dbOperations.DoesSfdHaveKey (docID, key)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Key")
                    );
                }

                return _dbOperations.GetValueForKeyInSfd (docID, key);
            }

            public bool UpdateValue (ulong docID, string key, string newValue) {
                DoSfdChecks (docID);
                if (key == null || key.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Key")
                    );
                }
                if (newValue == null || newValue.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Value")
                    );
                }
                if (!_dbOperations.DoesSfdHaveKey (docID, key)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Key")
                    );
                }

                return _dbOperations.UpdateValueForKeyInSfd (docID, key, newValue);
            }

            public int DeleteKey (ulong docID, string key) {
                DoSfdChecks (docID);
                if (key == null || key.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Key")
                    );
                }
                if (!_dbOperations.DoesSfdHaveKey (docID, key)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Key")
                    );
                }

                return _dbOperations.DeleteKeyInSfd (docID, key);
            }

            public List<string> AllKeys (ulong docID) {
                DoSfdChecks (docID);

                return _dbOperations.GetAllKeysInSfd (docID);
            }

            #endregion << Schema Free Document-related Operations >>

            #region << Schema Free Document Version-related Operations >>

            // TODO

            #endregion << Schema Free Document Version-related Operations >>
        }
    }
}
