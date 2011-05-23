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
        public class _Briefcase : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Briefcase (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Briefcase _theObject = null;

            internal static _Briefcase GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Briefcase (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoBriefcaseChecks (ulong briefcaseID) {
                if (briefcaseID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Briefcase id")
                    );
                }

                if (!_dbOperations.DoesBriefcaseExist (briefcaseID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Briefcase")
                    );
                }
            }

            internal void DoBriefcaseChecks (string briefcaseName) {
                if (briefcaseName == null || briefcaseName.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Briefcase name")
                    );
                }

                // NO! Don't check for briefcase existence here.
            }

            #endregion << Client-input Check Methods >>

            #region << Briefcase-related Operations >>

            /// <summary>
            /// This method creates a new briefcase within the system.
            /// </summary>
            /// <param name="briefcaseName">Name of the new briefcase</param>
            /// <param name="briefcaseDesc">Description of the new briefcase</param>
            /// <returns>An id that uniquely identifies the aspect across the entire system, if the call was successful.</returns>
            public ulong New (string briefcaseName, string briefcaseDesc) {
                ValidateString (briefcaseName, ValidationCheckType.BRIEFCASE_NAME);
                ValidateString (briefcaseDesc, ValidationCheckType.BRIEFCASE_DESC);
                DoBriefcaseChecks (briefcaseName);
                if (Exists (briefcaseName)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Briefcase name")
                    );
                }

                return _dbOperations.CreateBriefcase (briefcaseName, briefcaseDesc);
            }

            /// <summary>
            /// This method deletes a briefcase.
            /// </summary>
            /// <param name="briefcaseID">Id of the briefcase that has to be deleted.</param>
            /// <returns>An integer value that indicates how many briefcases have been deleted. Is always one.</returns>
            public int Delete (ulong briefcaseID) {
                DoBriefcaseChecks (briefcaseID);
                if (briefcaseID == MfsOperations.GLOBAL_BRIEFCASE_ID) {
                    throw new MfsIllegalOperationException (
                        MfsErrorMessages.GetMessage (MessageType.OP_NOT_ALLOWED, "Delete global briefcase")
                    );
                }

                return _dbOperations.DeleteBriefcase (briefcaseID);
            }

            /// <summary>
            /// This method tells the client whether an briefcase exists or not.
            /// </summary>
            /// <param name="briefcaseID">The briefcase id for which this information is sought</param>
            /// <returns>A boolean value that indicates whether this briefcase exists or not.</returns>
            public bool Exists (ulong briefcaseID) {
                if (briefcaseID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Briefcase id")
                    );
                }

                return _dbOperations.DoesBriefcaseExist (briefcaseID);
            }

            /// <summary>
            /// This method tells the client whether an briefcase exists or not.
            /// </summary>
            /// <param name="briefcaseName">The briefcase name for which this information is sought</param>
            /// <returns>A boolean value that indicates whether this briefcase exists or not.</returns>
            public bool Exists (string briefcaseName) {
                DoBriefcaseChecks (briefcaseName);

                return _dbOperations.DoesBriefcaseExist (briefcaseName);
            }

            /// <summary>
            /// This method gets the briefcase name associated with a briefcase id.
            /// </summary>
            /// <param name="briefcaseID">The briefcase id for which this information is sought.</param>
            /// <returns>Name of the briefcase sought.</returns>
            public void Get (ulong briefcaseID, out string briefcaseName, out string briefcaseDesc) {
                DoBriefcaseChecks (briefcaseID);

                _dbOperations.GetBriefcaseNameAndDesc (briefcaseID, out briefcaseName, out briefcaseDesc);
            }

            public ulong IDFromName (string briefcaseName) {
                DoBriefcaseChecks (briefcaseName);
                if (!Exists (briefcaseName)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Briefcase name")
                    );
                }

                return _dbOperations.GetBriefcaseIDFromName (briefcaseName);
            }

            public List<ulong> All () {
                return _dbOperations.GetAllBriefcases ();
            }

            /// <summary>
            /// This method updates the name of an existing briefcase.
            /// </summary>
            /// <param name="briefcaseID">The briefcase id, the name of which has to be updated.</param>
            /// <param name="newBriefcaseName">New briefcase name.</param>
            /// <returns>A boolean value indicating whether the call was successful or not.</returns>
            public bool UpdateName (ulong briefcaseID, string newBriefcaseName) {
                DoBriefcaseChecks (briefcaseID);
                ValidateString (newBriefcaseName, ValidationCheckType.BRIEFCASE_NAME);

                return _dbOperations.UpdateBriefcaseName (briefcaseID, newBriefcaseName);
            }

            /// <summary>
            /// This method updates the description of an existing briefcase.
            /// </summary>
            /// <param name="briefcaseID">The briefcase id, the description of which has to be updated.</param>
            /// <param name="newBriefcaseDesc">New briefcase description.</param>
            /// <returns>A boolean value indicating whether the call was successful or not.</returns>
            public bool UpdateDesc (ulong briefcaseID, string newBriefcaseDesc) {
                DoBriefcaseChecks (briefcaseID);
                ValidateString (newBriefcaseDesc, ValidationCheckType.BRIEFCASE_DESC);

                return _dbOperations.UpdateBriefcaseDesc (briefcaseID, newBriefcaseDesc);
            }

            /// <summary>
            /// This method deletes all aspects within the system. Needless to add, this should be used very carefully.
            /// </summary>
            /// <returns>An integer value indicating how many aspects have been deleted.</returns>
            public int DeleteAll () {
                return _dbOperations.DeleteAllBriefcasesInSystem ();
            }

            #endregion << Briefcase-related Operations >>

            #region << Briefcases-Documents Operations >>

            /// <summary>
            /// This method returns the briefcase id that contains a document.
            /// </summary>
            /// <param name="documentID">Document for which this information is sought.</param>
            /// <returns>Id of the briefcase.</returns>
            public ulong GetContaining (ulong documentID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.GetContainingBriefcase (documentID);
            }

            /// <summary>
            /// This method moves a document to a briefcase.
            /// </summary>
            /// <param name="documentID">Id of the document to be moved.</param>
            /// <param name="briefcaseID">Id of the briefcase to be moved to.</param>
            /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
            public bool MoveTo (ulong documentID, ulong briefcaseID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);
                DoBriefcaseChecks (briefcaseID);

                return _dbOperations.MoveDocumentToBriefcase (documentID, briefcaseID);
            }

            /// <summary>
            /// This method removes a document from its containing briefcase to the global briefcase.
            /// </summary>
            /// <param name="documentID">Id of the document to be moved.</param>
            /// <returns>A boolean value indicating whether the operation was successful or not.</returns>
            public bool MoveToGlobal (ulong documentID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.RemoveDocumentFromBriefcase (documentID);
            }

            /// <summary>
            /// This method gets all the documents in a briefcase.
            /// </summary>
            /// <param name="briefcaseID">Id of the briefcase from which its contained documents are sought.</param>
            /// <returns>A list of documents that are contained within the briefcase.</returns>
            public List<ulong> All (ulong briefcaseID) {
                DoBriefcaseChecks (briefcaseID);

                return _dbOperations.GetDocumentsInBriefcase (briefcaseID);
            }

            #endregion << Briefcases-Documents Operations >>
        }
    }
}
