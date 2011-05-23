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
        public class _Aspect : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Aspect (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Aspect _theObject = null;

            internal static _Aspect GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Aspect (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoAspectChecks (ulong aspectID) {
                if (aspectID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Aspect id")
                    );
                }

                if (!_dbOperations.DoesAspectExist (aspectID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Aspect")
                    );
                }
            }

            internal void DoAspectChecks (string aspectName) {
                if (aspectName == null || aspectName.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Aspect Name")
                    );
                }

                // NO! Don't check for aspect existence here.
            }

            #endregion << Client-input Check Methods >>

            #region << Aspect-related Operations >>

            /// <summary>
            /// This method creates a new aspect within the system.
            /// </summary>
            /// <param name="aspectName">Name of the new aspect.</param>
            /// <param name="aspectDesc">Description of the new aspect.</param>
            /// <returns>An id that uniquely identifies the aspect across the entire system, if the call was successful.</returns>
            public ulong New (string aspectName, string aspectDesc) {
                ValidateString (aspectName, ValidationCheckType.ASPECT_NAME);
                ValidateString (aspectDesc, ValidationCheckType.ASPECT_DESC);
                DoAspectChecks (aspectName);
                if (Exists (aspectName) == true) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Aspect Name")
                    );
                }

                return _dbOperations.CreateAspect (aspectName, aspectDesc);
            }

            /// <summary>
            /// This method gets the aspect name associated with an aspect id.
            /// </summary>
            /// <param name="aspectID">The aspect id for which this information is sought.</param>
            /// <returns>Name of the aspect sought.</returns>
            public void Get (ulong aspectID, out string aspectName, out string aspectDesc) {
                DoAspectChecks (aspectID);

                _dbOperations.GetAspectNameAndDesc (aspectID, out aspectName, out aspectDesc);
            }

            public List<ulong> All () {
                return _dbOperations.GetAllAspects ();
            }

            /// <summary>
            /// This method updates the name of an existing aspect.
            /// </summary>
            /// <param name="aspectID">The aspect id, the name of which has to be updated.</param>
            /// <param name="newAspectName">New aspect name.</param>
            /// <returns>A boolean value indicating whether the call was successful or not.</returns>
            public bool UpdateName (ulong aspectID, string newAspectName) {
                ValidateString (newAspectName, ValidationCheckType.ASPECT_NAME);
                DoAspectChecks (aspectID);

                return _dbOperations.UpdateAspectName (aspectID, newAspectName);
            }

            /// <summary>
            /// This method updates the description of an existing aspect.
            /// </summary>
            /// <param name="aspectID">The aspect id, the description of which has to be updated.</param>
            /// <param name="newAspectDesc">New aspect description.</param>
            /// <returns>A boolean value indicating whether the call was successful or not.</returns>
            public bool UpdateDesc (ulong aspectID, string newAspectDesc) {
                ValidateString (newAspectDesc, ValidationCheckType.ASPECT_DESC);
                DoAspectChecks (aspectID);

                return _dbOperations.UpdateAspectDesc (aspectID, newAspectDesc);
            }

            /// <summary>
            /// This method tells the client whether an aspect exists or not.
            /// </summary>
            /// <param name="aspectID">The aspect id for which this information is sought.</param>
            /// <returns>A boolean value that indicates whether this aspect exists or not.</returns>
            public bool Exists (ulong aspectID) {
                if (aspectID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Aspect id")
                    );
                }

                return _dbOperations.DoesAspectExist (aspectID);
            }

            /// <summary>
            /// This method tells the client whether an aspect exists or not.
            /// </summary>
            /// <param name="aspectName">The aspect name for which this information is sought.</param>
            /// <returns>A boolean value that indicates whether this aspect exists or not.</returns>
            public bool Exists (string aspectName) {
                DoAspectChecks (aspectName);

                return _dbOperations.DoesAspectExist (aspectName);
            }

            /// <summary>
            /// This method deletes an aspect.
            /// </summary>
            /// <param name="aspectID">Id of the aspect that has to be deleted.</param>
            /// <returns>An integer value that indicates how many aspects have been deleted. Is always one.</returns>
            public int Delete (ulong aspectID) {
                DoAspectChecks (aspectID);

                return _dbOperations.DeleteAspect (aspectID);
            }

            public ulong IDFromName (string aspectName) {
                DoAspectChecks (aspectName);
                if (!Exists (aspectName)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Aspect Name")
                    );
                }

                return _dbOperations.GetAspectIDFromName (aspectName);
            }

            /// <summary>
            /// This method deletes all aspects within the system. Needless to add, this should be used very carefully.
            /// </summary>
            /// <returns>An integer value indicating how many aspects have been deleted.</returns>
            public int DeleteAll () {
                return _dbOperations.DeleteAllAspectsInSystem ();
            }

            #endregion << Aspect-related Operations >>

            #region << Aspects-Documents Operations >>

            /// <summary>
            /// This method applies an aspect to a document.
            /// </summary>
            /// <param name="aspectID">Aspect to be applied.</param>
            /// <param name="documentID">Document to be applied aspect to.</param>
            /// <returns>A boolean value indicating if the operation was successful.</returns>
            public bool Apply (ulong aspectID, ulong documentID) {
                DoAspectChecks (aspectID);
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.ApplyAspectToDocument (aspectID, documentID);
            }

            /// <summary>
            /// This method applies a single aspect to multiple documents.
            /// </summary>
            /// <param name="aspectID">Aspect to be applied.</param>
            /// <param name="documentIDs">List of documents to which aspect is to be applied.</param>
            public void Apply (ulong aspectID, List<ulong> documentIDs) {
                _parent.ValidateList (documentIDs, false, "Document");
                DoAspectChecks (aspectID);
                foreach (ulong documentID in documentIDs) {
                    _parent.DocumentObj.DoDocumentChecks (documentID);
                }

                foreach (ulong documentID in documentIDs) {
                    _dbOperations.ApplyAspectToDocument (aspectID, documentID);
                }
            }

            /// <summary>
            /// This method applies multiple aspects to a single document.
            /// </summary>
            /// <param name="aspectIDs">List of aspects to apply to document.</param>
            /// <param name="documentID">Document to which aspects are to be applied.</param>
            public void Apply (List<ulong> aspectIDs, ulong documentID) {
                _parent.ValidateList (aspectIDs, false, "Document");
                _parent.DocumentObj.DoDocumentChecks (documentID);
                foreach (ulong aspectID in aspectIDs) {
                    DoAspectChecks (aspectID);
                }

                foreach (ulong aspectID in aspectIDs) {
                    _dbOperations.ApplyAspectToDocument (aspectID, documentID);
                }
            }

            /// <summary>
            /// Applies multiple aspects to multiple documents. This results in a Cartesian product.
            /// </summary>
            /// <param name="aspectIDs">List of aspects to apply to list of documents.</param>
            /// <param name="documentIDs">List of documents to which list of aspects are to be applied.</param>
            public void Apply (List<ulong> aspectIDs, List<ulong> documentIDs) {
                _parent.ValidateList (aspectIDs, false, "Aspect");
                _parent.ValidateList (documentIDs, false, "Document");
                foreach (ulong aspectID in aspectIDs) {
                    DoAspectChecks (aspectID);
                }
                foreach (ulong documentID in documentIDs) {
                    _parent.DocumentObj.DoDocumentChecks (documentID);
                }

                foreach (ulong aspectID in aspectIDs) {
                    foreach (ulong documentID in documentIDs) {
                        _dbOperations.ApplyAspectToDocument (aspectID, documentID);
                    }
                }
            }

            /// <summary>
            /// This method tells the caller if an aspect has been applied to a document.
            /// </summary>
            /// <param name="aspectID">Aspect to be checked if applied.</param>
            /// <param name="documentID">Document to be checked whether aspect has been applied to or not.</param>
            /// <returns>A boolean value indicating whether or not the aspect has been applied to the document.</returns>
            public bool IsApplied (ulong aspectID, ulong documentID) {
                DoAspectChecks (aspectID);
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.IsAspectAppliedToDocument (aspectID, documentID);
            }

            /// <summary>
            /// This method "unapplies" an aspect from a document.
            /// </summary>
            /// <param name="aspectID">Aspect that is to be "unapplied."</param>
            /// <param name="documentID">Document from which aspect is to be "unapplied."</param>
            /// <returns></returns>
            public bool Unapply (ulong aspectID, ulong documentID) {
                DoAspectChecks (aspectID);
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.UnapplyAspectFromDocument (aspectID, documentID);
            }

            /// <summary>
            /// This method "unapplies" all aspects from a document.
            /// </summary>
            /// <param name="documentID">Document from which all aspects are to be "unapplied."</param>
            /// <returns>Number of aspects that were "unapplied" from the document.</returns>
            public int UnapplyAll (ulong documentID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.UnapplyAllAspectsFromDocument (documentID);
            }

            /// <summary>
            /// This method "unapplies" an aspect from all documents.
            /// </summary>
            /// <param name="aspectID">Aspect that is to be "unapplied."</param>
            /// <returns>Number of documents from which the apsect was "unapplied."</returns>
            public int UnapplyFromAll (ulong aspectID) {
                DoAspectChecks (aspectID);

                return _dbOperations.UnapplyAspectFromAllDocuments (aspectID);
            }

            /// <summary>
            /// This method gets the aspects that have been applied to a document.
            /// </summary>
            /// <param name="documentID">Document for which this information is sought.</param>
            /// <returns>List of aspects which have been applied to this document.</returns>
            public List<ulong> Applied (ulong documentID) {
                _parent.DocumentObj.DoDocumentChecks (documentID);

                return _dbOperations.GetAspectsAppliedOnDocument (documentID);
            }

            /// <summary>
            /// This method gets all the documents that an aspect has been applied to.
            /// </summary>
            /// <param name="aspectID">Aspect for which this information is sought.</param>
            /// <returns>List of documents to which this aspect has been applied.</returns>
            public List<ulong> Documents (ulong aspectID) {
                DoAspectChecks (aspectID);

                return _dbOperations.GetDocumentsAppliedWithAspect (aspectID);
            }

            #endregion << Aspects-Documents Operations >>

            #region << Aspect Filter Operations >>

            public List<ulong> FilterFilesWithin (List<ulong> aspectIDs, List<ulong> fileIDs, FilterType filterType) {
                List<ulong> opFileIDs = new List<ulong> ();

                bool isIn = false;
                switch (filterType) {
                    case FilterType.AND:
                        foreach (ulong fileID in fileIDs) {
                            foreach (ulong aspectID in aspectIDs) {
                                if (IsApplied (aspectID, fileID)) {
                                    isIn = true;
                                } else {
                                    isIn = false;
                                    break;
                                }
                            }
                            if (isIn) {
                                opFileIDs.Add (fileID);
                                isIn = false;
                            }
                        }
                        break;
                    case FilterType.OR:
                        foreach (ulong fileID in fileIDs) {
                            foreach (ulong aspectID in aspectIDs) {
                                if (IsApplied (aspectID, fileID)) {
                                    isIn = true;
                                    break;
                                } else {
                                    isIn = false;
                                }
                            }
                            if (isIn) {
                                opFileIDs.Add (fileID);
                                isIn = false;
                            }
                        }
                        break;
                }

                return opFileIDs;
            }

            #endregion << Aspect Filter Operations >>
        }
    }
}
