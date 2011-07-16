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
using MnemonicFS.MfsUtils.MfsCrypto;

namespace MnemonicFS.MfsCore {
    public partial class MfsOperations {
        [Serializable]
        public class _Document : IDisposable {
            MfsOperations _parent;
            MfsDBOperations _dbOperations;

            private _Document (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Document _theObject = null;

            internal static _Document GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Document (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoDocumentChecks (ulong documentID) {
                if (documentID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Document id")
                    );
                }

                if (!(_dbOperations.DoesFileExist (documentID)
                    || _dbOperations.DoesNoteExist (documentID)
                    || _dbOperations.DoesUrlExist (documentID)
                    || _dbOperations.DoesSfdExist (documentID)
                    || _dbOperations.DoesVCardExist (documentID))) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Document")
                    );
                }
            }

            #endregion << Client-input Check Methods >>

            #region << Document Operations >>

            public DocumentType Type (ulong documentID) {
                if (documentID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Document id")
                    );
                }

                if (_dbOperations.DoesFileExist (documentID)) {
                    return DocumentType.FILE;
                }
                if (_dbOperations.DoesNoteExist (documentID)) {
                    return DocumentType.NOTE;
                }
                if (_dbOperations.DoesUrlExist (documentID)) {
                    return DocumentType.URL;
                }
                if (_dbOperations.DoesVCardExist (documentID)) {
                    return DocumentType.VCARD;
                }
                if (_dbOperations.DoesSfdExist (documentID)) {
                    return DocumentType.SFD;
                }

                throw new MfsNonExistentResourceException (
                    MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Document")
                );
            }

            public bool Delete (ulong docID) {
                _parent.DocumentObj.DoDocumentChecks (docID);

                if (_dbOperations.DoesFileExist (docID)) {
                    return _dbOperations.DeleteFile (docID) > 0;
                }
                if (_dbOperations.DoesNoteExist (docID)) {
                    return _dbOperations.DeleteNote (docID) > 0;
                }
                if (_dbOperations.DoesUrlExist (docID)) {
                    return _dbOperations.DeleteUrl (docID) > 0;
                }
                if (_dbOperations.DoesVCardExist (docID)) {
                    return _dbOperations.DeleteVCard (docID) > 0;
                }
                if (_dbOperations.DoesSfdExist (docID)) {
                    return _dbOperations.DeleteSfd (docID) > 0;
                }

                throw new MfsNonExistentResourceException (
                    MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Document")
                );
            }

            #endregion << Document Operations >>

            #region << Document Utility Operations >>

            private DateTime GetDocumentDateTimeStamp (ulong documentID) {
                DocumentType docType = Type (documentID);

                switch (docType) {
                    case DocumentType.FILE:
                        return _dbOperations.GetFileTimeStamp (documentID);
                    case DocumentType.NOTE:
                        return _dbOperations.GetNoteTimeStamp (documentID);
                    case DocumentType.SFD:
                        return _dbOperations.GetSfdTimeStamp (documentID);
                    case DocumentType.URL:
                        return _dbOperations.GetUrlTimeStamp (documentID);
                    case DocumentType.VCARD:
                        return _dbOperations.GetVCardTimeStamp (documentID);
                    case DocumentType.NONE:
                        break;
                }

                throw new MfsNonExistentResourceException (
                    MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Document")
                );
            }

            #endregion << Document Utility Operations >>

            #region << Universally Unique Document ID Operations >>

            private string GetUniqueIPStrForDocument (ulong documentID) {
                DateTime when = GetDocumentDateTimeStamp (documentID);
                return _parent._userID + "-" + documentID.ToString () + "-" + when.ToString ();
            }

            public string UUID (ulong documentID) {
                DoDocumentChecks (documentID);

                string ipStr = GetUniqueIPStrForDocument (documentID);

                return Hasher.GetSHA256 (ipStr);
            }

            #endregion << Universally Unique Document ID Operations >>
        }
    }
}
