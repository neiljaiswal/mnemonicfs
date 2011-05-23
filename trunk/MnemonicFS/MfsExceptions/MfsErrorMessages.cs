using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MnemonicFS.MfsExceptions {
    internal enum MessageType {
        NON_SPECIFIC = 0,
        AUTH_FAILURE,
        NON_EXISTENT_USER,
        NON_EXISTENT_RES,
        BAD_ARG,
        BAD_FORMAT,
        ALREADY_EXISTS,
        NULL,
        EMPTY,
        NULL_OR_EMPTY,
        ZERO,
        INADEQUATE,
        NEGATIVE,
        SIZE_OVERFLOW,
        BAD_LENGTH,
        OP_NOT_ALLOWED,
        CORRUPTED,
        DATE_DISCREPANCY,
        VERSION_CONFLICT,
        DB_EXC,
        STORAGE_EXC,
    }

    internal static class MfsErrorMessages {
        public static string GetMessage (MessageType msgType, string arg1, int arg2 = 0) {
            const string PREFIX = "ERROR! ";
            switch (msgType) {
                case MessageType.NON_SPECIFIC:
                    return PREFIX + " " + arg1;
                case MessageType.AUTH_FAILURE:
                    return PREFIX + "Authentication failure: " + arg1;
                case MessageType.NON_EXISTENT_USER:
                    return PREFIX + "Non-existent user: " + arg1;
                case MessageType.NON_EXISTENT_RES:
                    return PREFIX + "Non-existent resource: " + arg1;
                case MessageType.BAD_ARG:
                    return PREFIX + "Bad argument: " + arg1;
                case MessageType.BAD_FORMAT:
                    return PREFIX + "Bad format: " + arg1;
                case MessageType.ALREADY_EXISTS:
                    return PREFIX + "Already exists: " + arg1;
                case MessageType.NULL:
                    return PREFIX + "Null: " + arg1;
                case MessageType.EMPTY:
                    return PREFIX + "Empty: " + arg1;
                case MessageType.NULL_OR_EMPTY:
                    return PREFIX + "Null or empty: " + arg1;
                case MessageType.ZERO:
                    return PREFIX + "Zero: " + arg1;
                case MessageType.NEGATIVE:
                    return PREFIX + "Negative: " + arg1;
                case MessageType.SIZE_OVERFLOW:
                    return PREFIX + "Too large: " + arg1 + "; Max size: " + arg2;
                case MessageType.BAD_LENGTH:
                    return PREFIX + "Wrong length: " + arg1 + "; Expected size: " + arg2;
                case MessageType.OP_NOT_ALLOWED:
                    return PREFIX + "Operation not allowed: " + arg1;
                case MessageType.CORRUPTED:
                    return PREFIX + "Corrupted: " + arg1;
                case MessageType.DATE_DISCREPANCY:
                    return PREFIX + "Date discrepancy: " + arg1;
                case MessageType.VERSION_CONFLICT:
                    return PREFIX + "Version conflict: " + arg1;
                case MessageType.DB_EXC:
                    return PREFIX + "Database threw an exception: " + arg1;
                case MessageType.STORAGE_EXC:
                    return PREFIX + "Storage threw an exception: " + arg1;
            }

            return arg1;
        }
    }
}
