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
        public class _AspectGroup : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _AspectGroup (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _AspectGroup _theObject = null;

            internal static _AspectGroup GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _AspectGroup (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Aspect Group-related Operations >>

            public ulong New (ulong parentAspectGroupID, string aspectGroupName, string aspectGroupDesc) {
                if (aspectGroupName == null || aspectGroupName.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Aspect Group Name")
                    );
                }

                if (ExistsAtLevel (parentAspectGroupID, aspectGroupName)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Aspect Group at specified level")
                    );
                }

                if (!Exists (parentAspectGroupID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Parent Aspect Group")
                    );
                }

                if (aspectGroupDesc == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "Aspect Group Description")
                    );
                }

                return _dbOperations.CreateAspectGroup (parentAspectGroupID, aspectGroupName, aspectGroupDesc);
            }

            public bool ExistsAtLevel (ulong parentAspectGroupID, string aspectGroupName) {
                return _dbOperations.DoesAspectGroupExistAtLevel (parentAspectGroupID, aspectGroupName);
            }

            public void Get (ulong aspectGroupID, out string aspectGroupName, out string aspectGroupDesc) {
                if (!Exists (aspectGroupID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Aspect Group")
                    );
                }

                _dbOperations.GetAspectGroupNameAndDesc (aspectGroupID, out aspectGroupName, out aspectGroupDesc);
            }

            public bool Exists (ulong aspectGroupID) {
                return _dbOperations.DoesAspectGroupExist (aspectGroupID);
            }

            public List<ulong> ChildAspectGroups (ulong parentAspectID) {
                if (!Exists (parentAspectID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Aspect id")
                    );
                }

                return _dbOperations.GetChildAspectGroups (parentAspectID);
            }

            public int NumAspects (ulong aspectGroupID) {
                return _dbOperations.GetNumAspectsInAspectGroup (aspectGroupID);
            }

            public int NumAspectGroups (ulong aspectGroupID) {
                return _dbOperations.GetNumAspectGroupsInAspectGroup (aspectGroupID);
            }

            public int Delete (ulong aspectGroupID) {
                if (aspectGroupID == 0) {
                    throw new MfsIllegalOperationException (
                        MfsErrorMessages.GetMessage (MessageType.OP_NOT_ALLOWED, "Delete root aspect group")
                    );
                }

                if (!Exists (aspectGroupID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Aspect Group")
                    );
                }

                if (NumAspects (aspectGroupID) + NumAspectGroups (aspectGroupID) > 0) {
                    throw new MfsIllegalOperationException (
                        MfsErrorMessages.GetMessage (MessageType.OP_NOT_ALLOWED, "Delete non-empty aspect group")
                    );
                }

                return _dbOperations.DeleteAspectGroup (aspectGroupID);
            }

            #endregion << Aspect Group-related Operations >>
        }
    }
}
