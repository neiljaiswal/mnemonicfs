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
        public static class Filter {
            #region << General Filter Methods >>

            public static List<ulong> Invert (List<ulong> ipList, List<ulong> opList) {
                if (ipList == null || opList == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "One or both lists")
                    );
                }
                if (ipList.Count == 0) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.EMPTY, "I/p list")
                    );
                }
                if (!IsList1Superset (ipList, opList)) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NON_SPECIFIC, "O/p list has to necessarily be a subset of i/p list")
                    );
                }

                List<ulong> invertedList = new List<ulong> ();

                foreach (ulong ipListItem in ipList) {
                    if (!opList.Contains (ipListItem)) {
                        invertedList.Add (ipListItem);
                    }
                }

                return invertedList;
            }

            public static List<ulong> CombineOR (List<ulong> list1, List<ulong> list2) {
                if (list1 == null || list2 == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "One or both lists")
                    );
                }

                List<ulong> oredList = new List<ulong> ();

                foreach (ulong item1 in list1) {
                    oredList.Add (item1);
                }

                foreach (ulong item2 in list2) {
                    if (!oredList.Contains (item2)) {
                        oredList.Add (item2);
                    }
                }

                return oredList;
            }

            public static List<ulong> CombineOR (List<List<ulong>> lists) {
                List<ulong> oredList = new List<ulong> ();

                foreach (List<ulong> list in lists) {
                    foreach (ulong item in list) {
                        if (!oredList.Contains (item)) {
                            oredList.Add (item);
                        }
                    }
                }

                return oredList;
            }

            public static List<ulong> CombineAND (List<ulong> list1, List<ulong> list2) {
                if (list1 == null || list2 == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "One or both lists")
                    );
                }

                List<ulong> andedList = new List<ulong> ();

                foreach (ulong item in list1) {
                    if (list2.Contains (item)) {
                        andedList.Add (item);
                    }
                }

                foreach (ulong item in list2) {
                    if (list1.Contains (item) && !andedList.Contains (item)) {
                        andedList.Add (item);
                    }
                }

                return andedList;
            }

            public static List<ulong> CombineAND (List<List<ulong>> lists) {
                List<ulong> andedList = new List<ulong> ();

                foreach (List<ulong> list in lists) {
                    foreach (ulong item in list) {
                        if (DoAllListsHaveItem (lists, item)) {
                            andedList.Add (item);
                        }
                    }
                    return andedList;
                }

                return andedList;
            }

            public static List<ulong> CombineEXOR (List<ulong> list1, List<ulong> list2) {
                if (list1 == null || list2 == null) {
                    throw new MfsIllegalArgumentException (
                        MfsErrorMessages.GetMessage (MessageType.NULL, "One or both lists")
                    );
                }

                List<ulong> exoredList = new List<ulong> ();

                foreach (ulong item in list1) {
                    if (!list2.Contains (item)) {
                        exoredList.Add (item);
                    }
                }

                foreach (ulong item in list2) {
                    if (!list1.Contains (item) && !exoredList.Contains (item)) {
                        exoredList.Add (item);
                    }
                }

                return exoredList;
            }

            private static bool IsList1Superset (List<ulong> list1, List<ulong> list2) {
                foreach (ulong item in list2) {
                    if (!list1.Contains (item)) {
                        return false;
                    }
                }

                return true;
            }

            private static bool DoAllListsHaveItem (List<List<ulong>> lists, ulong item) {
                foreach (List<ulong> list in lists) {
                    if (!list.Contains (item)) {
                        return false;
                    }
                }
                return true;
            }

            #endregion << General Filter Methods >>
        }
    }
}
