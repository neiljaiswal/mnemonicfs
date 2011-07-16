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
        public class _Url : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Url (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Url _theObject = null;

            internal static _Url GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Url (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoUrlChecks (ulong urlID) {
                if (urlID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Url id")
                    );
                }

                if (!_dbOperations.DoesUrlExist (urlID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Url")
                    );
                }
            }

            #endregion << Client-input Check Methods >>

            #region << Url-related Operations >>

            public ulong New (string url, string description, DateTime when) {
                if (url == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "Url")
                    );
                }
                if (url.Length > MAX_URL_LENGTH) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.SIZE_OVERFLOW, "Url", MAX_URL_LENGTH)
                    );
                }

                return _dbOperations.AddUrl (url, description, when);
            }

            public void GetDetails (ulong urlID, out string url, out string description, out DateTime when) {
                DoUrlChecks (urlID);

                _dbOperations.GetUrlDetails (urlID, out url, out description, out when);
            }

            public bool Update (ulong urlID, string newUrl) {
                DoUrlChecks (urlID);

                return _dbOperations.UpdateUrl (urlID, newUrl);
            }

            public bool UpdateDescription (ulong urlID, string newDescription) {
                DoUrlChecks (urlID);

                return _dbOperations.UpdateUrlDescription (urlID, newDescription);
            }

            public DateTime GetSaveDateTime (ulong docID) {
                DoUrlChecks (docID);

                return _dbOperations.GetUrlSaveDateTime (docID);
            }

            public bool UpdateDateTime (ulong urlID, DateTime newWhen) {
                DoUrlChecks (urlID);

                return _dbOperations.UpdateUrlDateTime (urlID, newWhen);
            }

            public int Delete (ulong urlID) {
                DoUrlChecks (urlID);

                return _dbOperations.DeleteUrl (urlID);
            }

            public bool Exists (ulong urlID) {
                if (urlID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Url id")
                    );
                }

                return _dbOperations.DoesUrlExist (urlID);
            }

            #endregion << Url-related Operations >>
        }
    }
}
