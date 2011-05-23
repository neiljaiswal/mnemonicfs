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
        public class _Relation : IDisposable {
            private MfsOperations _parent;
            private MfsDBOperations _dbOperations;

            private _Relation (MfsOperations parent) {
                _parent = parent;
                _dbOperations = new MfsDBOperations (_parent._userID, _parent._userSpecificPath);
            }

            private static _Relation _theObject = null;

            internal static _Relation GetObject (MfsOperations parent) {
                if (_theObject == null) {
                    _theObject = new _Relation (parent);
                }

                return _theObject;
            }

            #region << IDisposable Members >>

            public void Dispose () {
                _theObject = null;
            }

            #endregion << IDisposable Members >>

            #region << Client-input Check Methods >>

            internal void DoPredicateChecks (ulong predicateID) {
                if (predicateID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Predicate id")
                    );
                }

                if (!_dbOperations.DoesPredicateExist (predicateID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Predicate")
                    );
                }
            }

            internal void DoPredicateChecks (string predicate) {
                if (predicate == null || predicate.Equals (string.Empty)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL_OR_EMPTY, "Predicate")
                    );
                }

                // NO! Don't check for predicate existence here.
            }

            #endregion << Client-input Check Methods >>

            #region << Relationship Operations >>

            public ulong NewPredicate (string predicate) {
                ValidateString (predicate, ValidationCheckType.PREDICATE);
                DoPredicateChecks (predicate);

                if (_dbOperations.DoesPredicateExist (predicate)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Predicate")
                    );
                }

                return _dbOperations.CreatePredicate (predicate);
            }

            public int DeletePredicate (ulong predicateID) {
                if (!DoesPredicateExist (predicateID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Predicate")
                    );
                }

                return _dbOperations.DeletePredicate (predicateID);
            }

            public int DeletePredicate (string predicate) {
                DoPredicateChecks (predicate);
                if (!DoesPredicateExist (predicate)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Predicate")
                    );
                }

                return _dbOperations.DeletePredicate (predicate);
            }

            public bool DoesPredicateExist (ulong predicateID) {
                if (predicateID == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.ZERO, "Predicate id")
                    );
                }

                return _dbOperations.DoesPredicateExist (predicateID);
            }

            public bool DoesPredicateExist (string predicate) {
                DoPredicateChecks (predicate);

                return _dbOperations.DoesPredicateExist (predicate);
            }

            public string GetPredicate (ulong predicateID) {
                if (!DoesPredicateExist (predicateID)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Predicate")
                    );
                }

                return _dbOperations.GetPredicate (predicateID);
            }

            public ulong GetPredicateID (string predicate) {
                DoPredicateChecks (predicate);
                if (!DoesPredicateExist (predicate)) {
                    throw new MfsNonExistentResourceException (
                        MfsErrorMessages.GetMessage (MessageType.NON_EXISTENT_RES, "Predicate")
                    );
                }

                return _dbOperations.GetPredicateID (predicate);
            }

            public List<ulong> GetAllPredicates () {
                return _dbOperations.GetAllPredicates ();
            }

            public bool UpdatePredicate (ulong predicateID, string newPredicate) {
                DoPredicateChecks (predicateID);
                ValidateString (newPredicate, ValidationCheckType.PREDICATE);

                return _dbOperations.UpdatePredicate (predicateID, newPredicate);
            }

            public int DeleteAllPredicates () {
                return _dbOperations.DeleteAllPredicates ();
            }

            public bool New (ulong subjectDocID, ulong objectDocID, ulong predicateID) {
                _parent.DocumentObj.DoDocumentChecks (subjectDocID);
                _parent.DocumentObj.DoDocumentChecks (objectDocID);
                DoPredicateChecks (predicateID);

                if (_dbOperations.DoesSpecificRelationExist (subjectDocID, objectDocID, predicateID)) {
                    throw new MfsDuplicateNameException (
                        MfsErrorMessages.GetMessage (MessageType.ALREADY_EXISTS, "Specific relation")
                    );
                }

                return _dbOperations.CreateRelation (subjectDocID, objectDocID, predicateID);
            }

            public bool Exists (ulong subjectDocID, ulong objectDocID) {
                _parent.DocumentObj.DoDocumentChecks (subjectDocID);
                _parent.DocumentObj.DoDocumentChecks (objectDocID);

                return _dbOperations.DoesRelationExist (subjectDocID, objectDocID);
            }

            public bool SpecificExists (ulong subjectDocID, ulong objectDocID, ulong predicateID) {
                _parent.DocumentObj.DoDocumentChecks (subjectDocID);
                _parent.DocumentObj.DoDocumentChecks (objectDocID);
                DoPredicateChecks (predicateID);

                return _dbOperations.DoesSpecificRelationExist (subjectDocID, objectDocID, predicateID);
            }

            public List<ulong> Get (ulong subjectDocID, ulong objectDocID) {
                _parent.DocumentObj.DoDocumentChecks (subjectDocID);
                _parent.DocumentObj.DoDocumentChecks (objectDocID);

                return _dbOperations.GetRelations (subjectDocID, objectDocID);
            }

            public bool RemoveSpecific (ulong subjectDocID, ulong objectDocID, ulong predicateID) {
                _parent.DocumentObj.DoDocumentChecks (subjectDocID);
                _parent.DocumentObj.DoDocumentChecks (objectDocID);
                DoPredicateChecks (predicateID);

                return _dbOperations.RemoveSpecificRelation (subjectDocID, objectDocID, predicateID);
            }

            public bool RemoveAll (ulong subjectDocID, ulong objectDocID) {
                _parent.DocumentObj.DoDocumentChecks (subjectDocID);
                _parent.DocumentObj.DoDocumentChecks (objectDocID);

                return _dbOperations.RemoveAllRelations (subjectDocID, objectDocID);
            }

            #endregion << Relationship Operations >>
        }
    }
}
