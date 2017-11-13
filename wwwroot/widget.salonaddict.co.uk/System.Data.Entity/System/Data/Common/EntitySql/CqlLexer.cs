namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    internal sealed class CqlLexer
    {
        private bool _aliasIdentifierState;
        private const string _datetimeOffsetValueRegularExpression = @"^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}([ ])+[0-9]{1,2}:[0-9]{1,2}(:[0-9]{1,2}(\.[0-9]{1,7})?)?([ ])*[\+-][0-9]{1,2}:[0-9]{1,2}$";
        private const string _datetimeValueRegularExpression = @"^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}([ ])+[0-9]{1,2}:[0-9]{1,2}(:[0-9]{1,2}(\.[0-9]{1,7})?)?$";
        private static HashSet<string> _functionKeywords = InitializeFunctionKeywords();
        private bool _identifierState;
        private static HashSet<string> _invalidAliasName;
        private int _iPos;
        private static Dictionary<string, short> _keywords;
        private int _lineNumber;
        private static readonly char[] _newLineCharacters = new char[] { '\n', '\x0085', '\v', '\u2028', '\u2029' };
        private static Dictionary<string, short> _operators;
        private ParserOptions _parserOptions;
        private static Dictionary<string, short> _punctuators;
        private string _query;
        private static Regex _reDateTimeOffsetValue;
        private static Regex _reDateTimeValue;
        private static Regex _reTimeValue;
        private static readonly StringComparer _stringComparer = StringComparer.OrdinalIgnoreCase;
        private const string _timeValueRegularExpression = @"^[0-9]{1,2}:[0-9]{1,2}(:[0-9]{1,2}(\.[0-9]{1,7})?)?$";
        private AcceptMethod[] accept_dispatch;
        private static int[] yy_acpt = new int[] { 
            0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 4, 4,
            4, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4,
            4, 0, 4, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4
        };
        private bool yy_at_bol;
        private const int YY_BOL = 0x80;
        private char[] yy_buffer;
        private int yy_buffer_end;
        private int yy_buffer_index;
        private int yy_buffer_read;
        private const int YY_BUFFER_SIZE = 0x200;
        private int yy_buffer_start;
        private static int[] yy_cmap = new int[] { 
            11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 0x1b, 11, 11, 8, 11, 11,
            11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            12, 0x21, 0x1c, 11, 11, 0x27, 0x24, 10, 40, 40, 0x27, 0x26, 40, 0x19, 0x18, 0x27,
            0x16, 0x16, 0x16, 0x16, 0x16, 0x16, 0x16, 0x16, 0x16, 0x16, 40, 40, 0x22, 0x20, 0x23, 40,
            0x1d, 5, 2, 30, 13, 15, 0x12, 20, 30, 3, 30, 30, 0x17, 0x10, 0x1a, 0x11,
            30, 30, 6, 0x13, 14, 0x15, 30, 30, 9, 7, 30, 1, 11, 40, 11, 0x1f,
            11, 5, 2, 30, 13, 15, 0x12, 20, 30, 3, 30, 30, 0x17, 0x10, 4, 0x11,
            30, 30, 6, 0x13, 14, 0x15, 30, 30, 30, 7, 30, 40, 0x25, 40, 11, 11,
            0, 0x29
        };
        private const int YY_E_INTERNAL = 0;
        private const int YY_E_MATCH = 1;
        private const int YY_END = 2;
        private const int YY_EOF = 0x81;
        private static string[] yy_error_string = new string[] { "Error: Internal error.\n", "Error: Unmatched input.\n" };
        private const int YY_F = -1;
        private bool yy_last_was_cr;
        private int yy_lexical_state;
        private const int YY_NO_ANCHOR = 4;
        private const int YY_NO_STATE = -1;
        private const int YY_NOT_ACCEPT = 0;
        private static int[,] yy_nxt = new int[,] { 
            { 
                1, 2, 3, 0x53, 0x53, 0x53, 0x53, 0x53, 4, 20, 0x13, -1, 4, 0x54, 0x40, 0x53,
                0x53, 0x53, 0x47, 0x53, 0x48, 0x53, 5, 0x53, 6, 7, 0x19, 8, 0x18, 0x1d, 0x53, 0x53,
                0x16, 0x17, 0x1c, 0x17, 0x21, 0x24, 0x20, 0x20, 0x1b, 1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x4c, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, 4, -1, -1, -1, 4, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x15, -1, 0x27,
                0x15, -1, 0x15, -1, -1, 0x1a, 5, 0x1f, 40, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, 0x23, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x29, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x13, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x18, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 11, 11, 11, 11, 11, 11, -1, 11, -1, -1, -1, 11, 11, 11,
                11, 11, 11, 11, 11, 11, 11, 11, -1, -1, 11, -1, -1, -1, 11, 11,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 9, 0x13, 0x13, 0x13, 0x13, 0x13,
                0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13,
                0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, 0x26, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                0x20, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18,
                0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 10, 0x18, 0x18, 0x18,
                0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, 0x18, -1
            },
            { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, 0x13, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, 0x18, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, 0x15, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                0x20, -1, -1, 0x20, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 14,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, 0x15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, 0x20, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x2c, 0x53, 0x2d, -1, 0x2c, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x15, -1, 0x27,
                0x15, -1, 0x15, -1, -1, -1, 0x23, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, 0x20, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x15, -1, -1,
                -1, -1, 0x15, -1, -1, -1, 0x25, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, -1, 0x26, 12, 0x26, 0x26, 0x26, 0x26, 0x26,
                0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, -1, -1, 0x26, 0x26, 0x26,
                0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, 0x26, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, 0x25, -1, -1, 0x2a, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, 0x2a, -1, -1, -1
            }, { 
                -1, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x2b, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29,
                0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 13, 0x29, 0x29, 0x29, 0x29,
                0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 0x29, 13
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, 0x25, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, 0x2c, -1, 0x2d, -1, 0x2c, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            },
            { 
                -1, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, -1, 0x2d, 15, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d,
                0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, -1, -1, 0x2d, 0x2d, 0x2d,
                0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, 0x2e, -1, 0x2f, -1, 0x2e, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, -1, 0x2f, 0x10, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f,
                0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, -1, -1, 0x2f, 0x2f, 0x2f,
                0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, 0x2f, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, 0x30, -1, 0x26, -1, 0x30, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, 0x31, -1, 50, -1, 0x31, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, 50, 50, 50, 50, 50, 50, 50, -1, 50, 0x11, 50, 50, 50, 50, 50,
                50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, -1, -1, 50, 50, 50,
                50, 50, 50, 50, 50, 50, 50, 50, 50, -1
            }, { 
                -1, -1, -1, -1, -1, -1, -1, -1, 0x33, -1, 0x34, -1, 0x33, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, -1, 0x34, 0x12, 0x34, 0x34, 0x34, 0x34, 0x34,
                0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, -1, -1, 0x34, 0x34, 0x34,
                0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, 0x34, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 30, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x2e, 0x53, 0x2f, -1, 0x2e, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x22,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x30, 0x53, 0x26, -1, 0x30, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 30, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x31, 0x53, 50, -1, 0x31, 0x53, 0x53, 0x53,
                0x53, 0x51, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x36, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x33, 0x53, 0x34, -1, 0x33, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            },
            { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x38, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x3a,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 60, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x41, 0x53, 0x53, 0x35, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x37, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x39, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x3b, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x3d, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x3e, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x3f,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x42, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x43, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x44, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x45, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 70, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x49, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x49, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            },
            { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x4f, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 80,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x4a, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x52, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x53, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x4b, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }, { 
                -1, -1, 0x53, 0x53, 0x53, 0x4e, 0x53, 0x53, -1, 0x53, -1, -1, -1, 0x53, 0x53, 0x53,
                0x53, 0x53, 0x53, 0x53, 0x53, 0x53, 0x4d, 0x53, -1, -1, 0x53, -1, -1, -1, 0x53, 0x4d,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            }
        };
        private TextReader yy_reader;
        private static int[] yy_rmap = new int[] { 
            0, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 1, 11, 1,
            1, 1, 1, 12, 13, 1, 14, 14, 15, 0x10, 0x11, 1, 0x12, 10, 0x13, 20,
            1, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 5, 0x1c, 0x1d, 30, 0x1f, 0x20, 0x21, 0x22,
            0x23, 0x24, 0x25, 0x26, 0x27, 40, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 50,
            0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60, 0x3d, 0x3e, 0x3f, 11, 0x40, 0x41,
            0x42, 0x43, 0x44, 11, 0x45
        };
        private const int YY_START = 1;
        private static int[] yy_state_dtrans = new int[1];
        private int yychar;
        private const int YYINITIAL = 0;
        private int yyline;

        private CqlLexer()
        {
            this.yy_buffer = new char[0x200];
            this.yy_buffer_read = 0;
            this.yy_buffer_index = 0;
            this.yy_buffer_start = 0;
            this.yy_buffer_end = 0;
            this.yychar = 0;
            this.yyline = 0;
            this.yy_at_bol = true;
            this.yy_lexical_state = 0;
            AcceptMethod[] methodArray = new AcceptMethod[0x55];
            methodArray[2] = new AcceptMethod(this.Accept_2);
            methodArray[3] = new AcceptMethod(this.Accept_3);
            methodArray[4] = new AcceptMethod(this.Accept_4);
            methodArray[5] = new AcceptMethod(this.Accept_5);
            methodArray[6] = new AcceptMethod(this.Accept_6);
            methodArray[7] = new AcceptMethod(this.Accept_7);
            methodArray[8] = new AcceptMethod(this.Accept_8);
            methodArray[9] = new AcceptMethod(this.Accept_9);
            methodArray[10] = new AcceptMethod(this.Accept_10);
            methodArray[11] = new AcceptMethod(this.Accept_11);
            methodArray[12] = new AcceptMethod(this.Accept_12);
            methodArray[13] = new AcceptMethod(this.Accept_13);
            methodArray[14] = new AcceptMethod(this.Accept_14);
            methodArray[15] = new AcceptMethod(this.Accept_15);
            methodArray[0x10] = new AcceptMethod(this.Accept_16);
            methodArray[0x11] = new AcceptMethod(this.Accept_17);
            methodArray[0x12] = new AcceptMethod(this.Accept_18);
            methodArray[20] = new AcceptMethod(this.Accept_20);
            methodArray[0x15] = new AcceptMethod(this.Accept_21);
            methodArray[0x16] = new AcceptMethod(this.Accept_22);
            methodArray[0x17] = new AcceptMethod(this.Accept_23);
            methodArray[0x19] = new AcceptMethod(this.Accept_25);
            methodArray[0x1a] = new AcceptMethod(this.Accept_26);
            methodArray[0x1b] = new AcceptMethod(this.Accept_27);
            methodArray[0x1c] = new AcceptMethod(this.Accept_28);
            methodArray[30] = new AcceptMethod(this.Accept_30);
            methodArray[0x1f] = new AcceptMethod(this.Accept_31);
            methodArray[0x20] = new AcceptMethod(this.Accept_32);
            methodArray[0x22] = new AcceptMethod(this.Accept_34);
            methodArray[0x23] = new AcceptMethod(this.Accept_35);
            methodArray[0x25] = new AcceptMethod(this.Accept_37);
            methodArray[0x35] = new AcceptMethod(this.Accept_53);
            methodArray[0x36] = new AcceptMethod(this.Accept_54);
            methodArray[0x37] = new AcceptMethod(this.Accept_55);
            methodArray[0x38] = new AcceptMethod(this.Accept_56);
            methodArray[0x39] = new AcceptMethod(this.Accept_57);
            methodArray[0x3a] = new AcceptMethod(this.Accept_58);
            methodArray[0x3b] = new AcceptMethod(this.Accept_59);
            methodArray[60] = new AcceptMethod(this.Accept_60);
            methodArray[0x3d] = new AcceptMethod(this.Accept_61);
            methodArray[0x3e] = new AcceptMethod(this.Accept_62);
            methodArray[0x3f] = new AcceptMethod(this.Accept_63);
            methodArray[0x40] = new AcceptMethod(this.Accept_64);
            methodArray[0x41] = new AcceptMethod(this.Accept_65);
            methodArray[0x42] = new AcceptMethod(this.Accept_66);
            methodArray[0x43] = new AcceptMethod(this.Accept_67);
            methodArray[0x44] = new AcceptMethod(this.Accept_68);
            methodArray[0x45] = new AcceptMethod(this.Accept_69);
            methodArray[70] = new AcceptMethod(this.Accept_70);
            methodArray[0x47] = new AcceptMethod(this.Accept_71);
            methodArray[0x48] = new AcceptMethod(this.Accept_72);
            methodArray[0x49] = new AcceptMethod(this.Accept_73);
            methodArray[0x4a] = new AcceptMethod(this.Accept_74);
            methodArray[0x4b] = new AcceptMethod(this.Accept_75);
            methodArray[0x4c] = new AcceptMethod(this.Accept_76);
            methodArray[0x4d] = new AcceptMethod(this.Accept_77);
            methodArray[0x4e] = new AcceptMethod(this.Accept_78);
            methodArray[0x4f] = new AcceptMethod(this.Accept_79);
            methodArray[80] = new AcceptMethod(this.Accept_80);
            methodArray[0x51] = new AcceptMethod(this.Accept_81);
            methodArray[0x52] = new AcceptMethod(this.Accept_82);
            methodArray[0x53] = new AcceptMethod(this.Accept_83);
            methodArray[0x54] = new AcceptMethod(this.Accept_84);
            this.accept_dispatch = methodArray;
        }

        internal CqlLexer(FileStream instream) : this()
        {
            if (instream == null)
            {
                throw new EntitySqlException(EntityRes.GetString("ParserInputError"));
            }
            this.yy_reader = new StreamReader(instream);
        }

        internal CqlLexer(TextReader reader) : this()
        {
            if (reader == null)
            {
                throw new EntitySqlException(EntityRes.GetString("ParserInputError"));
            }
            this.yy_reader = reader;
        }

        internal CqlLexer(string query, ParserOptions parserOptions) : this()
        {
            EntityUtil.CheckArgumentNull<string>(query, "query");
            EntityUtil.CheckArgumentNull<ParserOptions>(parserOptions, "parserOptions");
            this._query = query;
            this._parserOptions = parserOptions;
            this.yy_reader = new StringReader(this._query);
        }

        private Token Accept_10() => 
            this.MapDoubleQuotedString(this.YYText);

        private Token Accept_11() => 
            this.NewParameterToken(this.YYText);

        private Token Accept_12() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Binary);

        private Token Accept_13()
        {
            this._lineNumber++;
            this.AdvanceIPos();
            this.ResetIndetifierState();
            return null;
        }

        private Token Accept_14() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Boolean);

        private Token Accept_15() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Time);

        private Token Accept_16() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Guid);

        private Token Accept_17() => 
            this.NewLiteralToken(this.YYText, LiteralKind.DateTime);

        private Token Accept_18() => 
            this.NewLiteralToken(this.YYText, LiteralKind.DateTimeOffset);

        private Token Accept_2() => 
            this.HandleEscapedIdentifiers();

        private Token Accept_20() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_21() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Number);

        private Token Accept_22() => 
            this.MapPunctuator(this.YYText);

        private Token Accept_23() => 
            this.MapOperator(this.YYText);

        private Token Accept_25() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_26() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Number);

        private Token Accept_27() => 
            this.MapPunctuator(this.YYText);

        private Token Accept_28() => 
            this.MapOperator(this.YYText);

        private Token Accept_3() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_30() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_31() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Number);

        private Token Accept_32() => 
            this.MapOperator(this.YYText);

        private Token Accept_34() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_35() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Number);

        private Token Accept_37() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Number);

        private Token Accept_4()
        {
            this.AdvanceIPos();
            this.ResetIndetifierState();
            return null;
        }

        private Token Accept_5() => 
            this.NewLiteralToken(this.YYText, LiteralKind.Number);

        private Token Accept_53() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_54() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_55() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_56() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_57() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_58() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_59() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_6() => 
            this.MapPunctuator(this.YYText);

        private Token Accept_60() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_61() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_62() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_63() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_64() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_65() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_66() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_67() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_68() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_69() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_7() => 
            this.MapOperator(this.YYText);

        private Token Accept_70() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_71() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_72() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_73() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_74() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_75() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_76() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_77() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_78() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_79() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_8()
        {
            this._lineNumber++;
            this.AdvanceIPos();
            this.ResetIndetifierState();
            return null;
        }

        private Token Accept_80() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_81() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_82() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_83() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_84() => 
            this.MapIdentifierOrKeyword(this.YYText);

        private Token Accept_9() => 
            this.NewLiteralToken(this.YYText, LiteralKind.String);

        internal int AdvanceIPos()
        {
            this._iPos += this.YYText.Length;
            return this._iPos;
        }

        private static string GetLiteralSingleQuotePayload(string literal)
        {
            if (((literal.Split(new char[] { '\'' }).Length != 3) || (-1 == literal.IndexOf('\''))) || (-1 == literal.LastIndexOf('\'')))
            {
                throw EntityUtil.EntitySqlError(Strings.MalformedSingleQuotePayload);
            }
            int index = literal.IndexOf('\'');
            string str = literal.Substring(index + 1, literal.Length - (index + 2));
            if (str.Split(new char[] { '\'' }).Length != 1)
            {
                throw EntityUtil.EntitySqlError(Strings.MalformedSingleQuotePayload);
            }
            return str;
        }

        internal Token HandleEscapedIdentifiers()
        {
            char ch = this.YYText[0];
            int num = -1;
            while (ch != '\x0081')
            {
                if (ch == '\x0081')
                {
                    throw EntityUtil.EntitySqlError(this._query, Strings.InvalidEscapedIdentifierEOF, this._iPos);
                }
                if (ch == ']')
                {
                    num++;
                    this.yy_mark_end();
                    ch = this.yy_advance();
                    if (ch == ']')
                    {
                        num--;
                    }
                    if (ch != ']')
                    {
                        break;
                    }
                }
                ch = this.yy_advance();
            }
            if (num != 0)
            {
                throw EntityUtil.EntitySqlError(this._query, Strings.InvalidEscapedIdentifierUnbalanced(this.YYText), this._iPos);
            }
            this.yy_to_mark();
            this.ResetKeywordAsIdentifierState();
            return this.MapIdentifierOrKeyword(this.YYText.Replace("]]", "]"));
        }

        private static HashSet<string> InitializeFunctionKeywords() => 
            new HashSet<string>(_stringComparer) { 
                "left",
                "right"
            };

        private static bool IsDigit(char c) => 
            ((c >= '0') && (c <= '9'));

        private static bool isHexDigit(char c) => 
            ((IsDigit(c) || ((c >= 'a') && (c <= 'f'))) || ((c >= 'A') && (c <= 'F')));

        private static bool IsLetter(char c) => 
            (((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z')));

        internal static bool IsLetterOrDigitOrUnderscore(string symbol, out bool isIdentifierASCII)
        {
            isIdentifierASCII = true;
            for (int i = 0; i < symbol.Length; i++)
            {
                isIdentifierASCII = isIdentifierASCII && (symbol[i] < '\x0080');
                if ((!isIdentifierASCII && !IsLetter(symbol[i])) && (!IsDigit(symbol[i]) && (symbol[i] != '_')))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsNewLine(char c)
        {
            for (int i = 0; i < _newLineCharacters.Length; i++)
            {
                if (c == _newLineCharacters[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsReservedKeyword(string term) => 
            InternalKeywordDictionary.ContainsKey(term);

        private static bool IsValidBinaryValue(string binaryValue)
        {
            if (string.IsNullOrEmpty(binaryValue))
            {
                return true;
            }
            int num = 0;
            bool flag = binaryValue.Length > 0;
            while (flag && (num < binaryValue.Length))
            {
                flag = isHexDigit(binaryValue[num++]);
            }
            return flag;
        }

        private static bool IsValidDateTimeOffsetValue(string datetimeOffsetValue) => 
            _reDateTimeOffsetValue?.IsMatch(datetimeOffsetValue);

        private static bool IsValidDateTimeValue(string datetimeValue) => 
            _reDateTimeValue?.IsMatch(datetimeValue);

        private static bool IsValidGuidValue(string guidValue)
        {
            int num = 0;
            int num2 = guidValue.Length - 1;
            if (((num2 - num) + 1) != 0x24)
            {
                return false;
            }
            int num3 = 0;
            bool flag = true;
            while (flag && (num3 < 0x24))
            {
                switch (num3)
                {
                    case 8:
                    case 13:
                    case 0x12:
                    case 0x17:
                        flag = guidValue[num + num3] == '-';
                        break;

                    default:
                        flag = isHexDigit(guidValue[num + num3]);
                        break;
                }
                num3++;
            }
            return flag;
        }

        private static bool IsValidTimeValue(string timeValue) => 
            _reTimeValue?.IsMatch(timeValue);

        internal Token MapDoubleQuotedString(string symbol)
        {
            if (this._parserOptions.AllowQuotedIdentifiers)
            {
                return NewToken(CqlParser.ESCAPED_IDENTIFIER, new Identifier(symbol, true, this._query, this._iPos));
            }
            LiteralKind nonUnicodeString = LiteralKind.NonUnicodeString;
            if ('N' == symbol[0])
            {
                nonUnicodeString = LiteralKind.UnicodeString;
            }
            return NewToken(CqlParser.LITERAL, new Literal(symbol, nonUnicodeString, this._query, this._iPos));
        }

        internal Token MapIdentifierOrKeyword(string symbol)
        {
            if ((symbol[0] == '[') && (symbol.Length > 1))
            {
                if (symbol[symbol.Length - 1] != ']')
                {
                    throw EntityUtil.EntitySqlError(this._query, Strings.InvalidEscapedIdentifier(symbol), this._iPos);
                }
                Identifier identifier = new Identifier(symbol, true, this._query, this._iPos) {
                    ErrCtx = { ErrorContextInfo = "CtxEscapedIdentifier" }
                };
                return NewToken(CqlParser.ESCAPED_IDENTIFIER, identifier);
            }
            this.yy_mark_end();
            char c = this.yy_advance();
            while ((c != '\x0081') && (char.IsWhiteSpace(c) || IsNewLine(c)))
            {
                c = this.yy_advance();
            }
            this.yy_to_mark();
            if ((c == '(') && _functionKeywords.Contains(symbol))
            {
                return NewToken(CqlParser.IDENTIFIER, new Identifier(symbol, false, this._query, this._iPos));
            }
            if ((!this._identifierState && !this._aliasIdentifierState) && ((c != '.') && InternalKeywordDictionary.ContainsKey(symbol)))
            {
                this.ResetKeywordAsIdentifierState();
                if (symbol.Equals("AS", StringComparison.OrdinalIgnoreCase))
                {
                    this._aliasIdentifierState = true;
                }
                return NewToken(InternalKeywordDictionary[symbol], new TerminalToken(symbol, this._iPos));
            }
            this.ResetKeywordAsIdentifierState();
            if (InternalInvalidAliasNameDictionary.Contains(symbol))
            {
                throw EntityUtil.EntitySqlError(this._query, Strings.InvalidAliasName(symbol), this._iPos);
            }
            Identifier tokenvalue = new Identifier(symbol, false, this._query, this._iPos) {
                ErrCtx = { ErrorContextInfo = "CtxIdentifier" }
            };
            return NewToken(CqlParser.IDENTIFIER, tokenvalue);
        }

        internal Token MapOperator(string oper)
        {
            if (!InternalOperatorDictionary.ContainsKey(oper))
            {
                throw EntityUtil.EntitySqlError(this._query, Strings.InvalidOperatorSymbol, this._iPos);
            }
            return NewToken(InternalOperatorDictionary[oper], new TerminalToken(oper, this._iPos));
        }

        internal Token MapPunctuator(string punct)
        {
            if (!InternalPunctuatorDictionary.ContainsKey(punct))
            {
                throw EntityUtil.EntitySqlError(this._query, Strings.InvalidPunctuatorSymbol, this._iPos);
            }
            this.ResetKeywordAsIdentifierState();
            if (punct.Equals(".", StringComparison.OrdinalIgnoreCase))
            {
                this._identifierState = true;
            }
            return NewToken(InternalPunctuatorDictionary[punct], new TerminalToken(punct, this._iPos));
        }

        internal Token NewLiteralToken(string literal, LiteralKind literalKind)
        {
            string binaryValue = literal;
            switch (literalKind)
            {
                case LiteralKind.String:
                    literalKind = LiteralKind.NonUnicodeString;
                    if ('N' == literal[0])
                    {
                        literalKind = LiteralKind.UnicodeString;
                    }
                    break;

                case LiteralKind.Binary:
                    binaryValue = GetLiteralSingleQuotePayload(literal);
                    if (!IsValidBinaryValue(binaryValue))
                    {
                        throw EntityUtil.EntitySqlError(this._query, Strings.InvalidLiteralFormat("binary", binaryValue), this._iPos);
                    }
                    break;

                case LiteralKind.DateTime:
                    binaryValue = GetLiteralSingleQuotePayload(literal);
                    if (!IsValidDateTimeValue(binaryValue))
                    {
                        throw EntityUtil.EntitySqlError(this._query, Strings.InvalidLiteralFormat("datetime", binaryValue), this._iPos);
                    }
                    break;

                case LiteralKind.Time:
                    binaryValue = GetLiteralSingleQuotePayload(literal);
                    if (!IsValidTimeValue(binaryValue))
                    {
                        throw EntityUtil.EntitySqlError(this._query, Strings.InvalidLiteralFormat("time", binaryValue), this._iPos);
                    }
                    break;

                case LiteralKind.DateTimeOffset:
                    binaryValue = GetLiteralSingleQuotePayload(literal);
                    if (!IsValidDateTimeOffsetValue(binaryValue))
                    {
                        throw EntityUtil.EntitySqlError(this._query, Strings.InvalidLiteralFormat("datetimeoffset", binaryValue), this._iPos);
                    }
                    break;

                case LiteralKind.Guid:
                    binaryValue = GetLiteralSingleQuotePayload(literal);
                    if (!IsValidGuidValue(binaryValue))
                    {
                        throw EntityUtil.EntitySqlError(this._query, Strings.InvalidLiteralFormat("guid", binaryValue), this._iPos);
                    }
                    break;
            }
            return NewToken(CqlParser.LITERAL, new Literal(binaryValue, literalKind, this._query, this._iPos));
        }

        internal Token NewParameterToken(string param) => 
            NewToken(CqlParser.PARAMETER, new Parameter(param, this._query, this._iPos));

        internal static Token NewToken(short tokenId, AstNode tokenvalue) => 
            new Token(tokenId, tokenvalue);

        internal static Token NewToken(short tokenId, TerminalToken termToken) => 
            new Token(tokenId, termToken);

        private void ResetIndetifierState()
        {
            this._identifierState = false;
        }

        private void ResetKeywordAsIdentifierState()
        {
            this._identifierState = this._aliasIdentifierState = false;
        }

        private char yy_advance()
        {
            if (this.yy_buffer_index >= this.yy_buffer_read)
            {
                int num;
                if (this.yy_buffer_start != 0)
                {
                    int index = this.yy_buffer_start;
                    int num3 = 0;
                    while (index < this.yy_buffer_read)
                    {
                        this.yy_buffer[num3] = this.yy_buffer[index];
                        index++;
                        num3++;
                    }
                    this.yy_buffer_end -= this.yy_buffer_start;
                    this.yy_buffer_start = 0;
                    this.yy_buffer_read = num3;
                    this.yy_buffer_index = num3;
                    num = this.yy_reader.Read(this.yy_buffer, this.yy_buffer_read, this.yy_buffer.Length - this.yy_buffer_read);
                    if (num <= 0)
                    {
                        return '\x0081';
                    }
                    this.yy_buffer_read += num;
                }
                while (this.yy_buffer_index >= this.yy_buffer_read)
                {
                    if (this.yy_buffer_index >= this.yy_buffer.Length)
                    {
                        this.yy_buffer = this.yy_double(this.yy_buffer);
                    }
                    num = this.yy_reader.Read(this.yy_buffer, this.yy_buffer_read, this.yy_buffer.Length - this.yy_buffer_read);
                    if (num <= 0)
                    {
                        return '\x0081';
                    }
                    this.yy_buffer_read += num;
                }
            }
            return yy_translate.translate(this.yy_buffer[this.yy_buffer_index++]);
        }

        internal int yy_char() => 
            this.yychar;

        private char[] yy_double(char[] buf)
        {
            char[] chArray = new char[2 * buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                chArray[i] = buf[i];
            }
            return chArray;
        }

        private void yy_error(int code, bool fatal)
        {
            if (fatal)
            {
                throw new EntitySqlException(EntityRes.GetString("ParserFatalError"));
            }
        }

        private void yy_mark_end()
        {
            this.yy_buffer_end = this.yy_buffer_index;
        }

        private void yy_mark_start()
        {
            for (int i = this.yy_buffer_start; i < this.yy_buffer_index; i++)
            {
                if ((this.yy_buffer[i] == '\n') && !this.yy_last_was_cr)
                {
                    this.yyline++;
                }
                if (this.yy_buffer[i] == '\r')
                {
                    this.yyline++;
                    this.yy_last_was_cr = true;
                }
                else
                {
                    this.yy_last_was_cr = false;
                }
            }
            this.yychar = (this.yychar + this.yy_buffer_index) - this.yy_buffer_start;
            this.yy_buffer_start = this.yy_buffer_index;
        }

        private void yy_move_end()
        {
            if ((this.yy_buffer_end > this.yy_buffer_start) && ('\n' == this.yy_buffer[this.yy_buffer_end - 1]))
            {
                this.yy_buffer_end--;
            }
            if ((this.yy_buffer_end > this.yy_buffer_start) && ('\r' == this.yy_buffer[this.yy_buffer_end - 1]))
            {
                this.yy_buffer_end--;
            }
        }

        private void yy_to_mark()
        {
            this.yy_buffer_index = this.yy_buffer_end;
            this.yy_at_bol = (this.yy_buffer_end > this.yy_buffer_start) && ((this.yy_buffer[this.yy_buffer_end - 1] == '\r') || (this.yy_buffer[this.yy_buffer_end - 1] == '\n'));
        }

        private void yybegin(int state)
        {
            this.yy_lexical_state = state;
        }

        private int yylength() => 
            (this.yy_buffer_end - this.yy_buffer_start);

        internal Token yylex()
        {
            int num = 4;
            int index = yy_state_dtrans[this.yy_lexical_state];
            int num3 = -1;
            int num4 = -1;
            bool flag = true;
            this.yy_mark_start();
            if (yy_acpt[index] != 0)
            {
                num4 = index;
                this.yy_mark_end();
            }
            while (true)
            {
                char ch;
                if (flag && this.yy_at_bol)
                {
                    ch = '\x0080';
                }
                else
                {
                    ch = this.yy_advance();
                }
                num3 = yy_nxt[yy_rmap[index], yy_cmap[ch]];
                if (('\x0081' == ch) && flag)
                {
                    return null;
                }
                if (-1 != num3)
                {
                    index = num3;
                    flag = false;
                    if (yy_acpt[index] != 0)
                    {
                        num4 = index;
                        this.yy_mark_end();
                    }
                }
                else
                {
                    if (-1 == num4)
                    {
                        throw new EntitySqlException(EntityRes.GetString("GenericSyntaxError"));
                    }
                    num = yy_acpt[num4];
                    if ((2 & num) != 0)
                    {
                        this.yy_move_end();
                    }
                    this.yy_to_mark();
                    if (num4 < 0)
                    {
                        if (num4 < 0x55)
                        {
                            this.yy_error(0, false);
                        }
                    }
                    else
                    {
                        AcceptMethod method = this.accept_dispatch[num4];
                        if (method != null)
                        {
                            Token token = method();
                            if (token != null)
                            {
                                return token;
                            }
                        }
                    }
                    flag = true;
                    index = yy_state_dtrans[this.yy_lexical_state];
                    num3 = -1;
                    num4 = -1;
                    this.yy_mark_start();
                    if (yy_acpt[index] != 0)
                    {
                        num4 = index;
                        this.yy_mark_end();
                    }
                }
            }
        }

        internal string yytext() => 
            new string(this.yy_buffer, this.yy_buffer_start, this.yy_buffer_end - this.yy_buffer_start);

        private static HashSet<string> InternalInvalidAliasNameDictionary
        {
            get
            {
                if (_invalidAliasName == null)
                {
                    HashSet<string> set = new HashSet<string>(_stringComparer) { 
                        "all",
                        "and",
                        "apply",
                        "as",
                        "asc",
                        "between",
                        "by",
                        "case",
                        "cast",
                        "collate",
                        "createref",
                        "deref",
                        "desc",
                        "distinct",
                        "element",
                        "else",
                        "end",
                        "escape",
                        "except",
                        "exists",
                        "flatten",
                        "from",
                        "group",
                        "having",
                        "in",
                        "inner",
                        "intersect",
                        "is",
                        "join",
                        "like",
                        "multiset",
                        "navigate",
                        "not",
                        "null",
                        "of",
                        "oftype",
                        "on",
                        "only",
                        "or",
                        "overlaps",
                        "ref",
                        "relationship",
                        "select",
                        "set",
                        "then",
                        "treat",
                        "union",
                        "using",
                        "when",
                        "where",
                        "with"
                    };
                    _invalidAliasName = set;
                }
                return _invalidAliasName;
            }
        }

        private static Dictionary<string, short> InternalKeywordDictionary
        {
            get
            {
                if (_keywords == null)
                {
                    Dictionary<string, short> dictionary = new Dictionary<string, short>(60, _stringComparer) {
                        { 
                            "all",
                            CqlParser.ALL
                        },
                        { 
                            "and",
                            CqlParser.AND
                        },
                        { 
                            "anyelement",
                            CqlParser.ANYELEMENT
                        },
                        { 
                            "apply",
                            CqlParser.APPLY
                        },
                        { 
                            "as",
                            CqlParser.AS
                        },
                        { 
                            "asc",
                            CqlParser.ASC
                        },
                        { 
                            "between",
                            CqlParser.BETWEEN
                        },
                        { 
                            "by",
                            CqlParser.BY
                        },
                        { 
                            "case",
                            CqlParser.CASE
                        },
                        { 
                            "cast",
                            CqlParser.CAST
                        },
                        { 
                            "collate",
                            CqlParser.COLLATE
                        },
                        { 
                            "createref",
                            CqlParser.CREATEREF
                        },
                        { 
                            "cross",
                            CqlParser.CROSS
                        },
                        { 
                            "deref",
                            CqlParser.DEREF
                        },
                        { 
                            "desc",
                            CqlParser.DESC
                        },
                        { 
                            "distinct",
                            CqlParser.DISTINCT
                        },
                        { 
                            "element",
                            CqlParser.ELEMENT
                        },
                        { 
                            "else",
                            CqlParser.ELSE
                        },
                        { 
                            "end",
                            CqlParser.END
                        },
                        { 
                            "escape",
                            CqlParser.ESCAPE
                        },
                        { 
                            "except",
                            CqlParser.EXCEPT
                        },
                        { 
                            "exists",
                            CqlParser.EXISTS
                        },
                        { 
                            "flatten",
                            CqlParser.FLATTEN
                        },
                        { 
                            "from",
                            CqlParser.FROM
                        },
                        { 
                            "full",
                            CqlParser.FULL
                        },
                        { 
                            "group",
                            CqlParser.GROUP
                        },
                        { 
                            "having",
                            CqlParser.HAVING
                        },
                        { 
                            "in",
                            CqlParser.IN
                        },
                        { 
                            "inner",
                            CqlParser.INNER
                        },
                        { 
                            "intersect",
                            CqlParser.INTERSECT
                        },
                        { 
                            "is",
                            CqlParser.IS
                        },
                        { 
                            "join",
                            CqlParser.JOIN
                        },
                        { 
                            "key",
                            CqlParser.KEY
                        },
                        { 
                            "left",
                            CqlParser.LEFT
                        },
                        { 
                            "like",
                            CqlParser.LIKE
                        },
                        { 
                            "limit",
                            CqlParser.LIMIT
                        },
                        { 
                            "multiset",
                            CqlParser.MULTISET
                        },
                        { 
                            "navigate",
                            CqlParser.NAVIGATE
                        },
                        { 
                            "not",
                            CqlParser.NOT
                        },
                        { 
                            "null",
                            CqlParser.NULL
                        },
                        { 
                            "of",
                            CqlParser.OF
                        },
                        { 
                            "oftype",
                            CqlParser.OFTYPE
                        },
                        { 
                            "on",
                            CqlParser.ON
                        },
                        { 
                            "only",
                            CqlParser.ONLY
                        },
                        { 
                            "or",
                            CqlParser.OR
                        },
                        { 
                            "order",
                            CqlParser.ORDER
                        },
                        { 
                            "outer",
                            CqlParser.OUTER
                        },
                        { 
                            "overlaps",
                            CqlParser.OVERLAPS
                        },
                        { 
                            "ref",
                            CqlParser.REF
                        },
                        { 
                            "relationship",
                            CqlParser.RELATIONSHIP
                        },
                        { 
                            "right",
                            CqlParser.RIGHT
                        },
                        { 
                            "row",
                            CqlParser.ROW
                        },
                        { 
                            "select",
                            CqlParser.SELECT
                        },
                        { 
                            "set",
                            CqlParser.SET
                        },
                        { 
                            "skip",
                            CqlParser.SKIP
                        },
                        { 
                            "then",
                            CqlParser.THEN
                        },
                        { 
                            "top",
                            CqlParser.TOP
                        },
                        { 
                            "treat",
                            CqlParser.TREAT
                        },
                        { 
                            "union",
                            CqlParser.UNION
                        },
                        { 
                            "using",
                            CqlParser.USING
                        },
                        { 
                            "value",
                            CqlParser.VALUE
                        },
                        { 
                            "when",
                            CqlParser.WHEN
                        },
                        { 
                            "where",
                            CqlParser.WHERE
                        },
                        { 
                            "with",
                            CqlParser.WITH
                        }
                    };
                    _keywords = dictionary;
                }
                return _keywords;
            }
        }

        private static Dictionary<string, short> InternalOperatorDictionary
        {
            get
            {
                if (_operators == null)
                {
                    Dictionary<string, short> dictionary = new Dictionary<string, short>(0x10, _stringComparer) {
                        { 
                            "==",
                            CqlParser.OP_EQ
                        },
                        { 
                            "!=",
                            CqlParser.OP_NEQ
                        },
                        { 
                            "<>",
                            CqlParser.OP_NEQ
                        },
                        { 
                            "<",
                            CqlParser.OP_LT
                        },
                        { 
                            "<=",
                            CqlParser.OP_LE
                        },
                        { 
                            ">",
                            CqlParser.OP_GT
                        },
                        { 
                            ">=",
                            CqlParser.OP_GE
                        },
                        { 
                            "&&",
                            CqlParser.AND
                        },
                        { 
                            "||",
                            CqlParser.OR
                        },
                        { 
                            "!",
                            CqlParser.NOT
                        },
                        { 
                            "+",
                            CqlParser.PLUS
                        },
                        { 
                            "-",
                            CqlParser.MINUS
                        },
                        { 
                            "*",
                            CqlParser.STAR
                        },
                        { 
                            "/",
                            CqlParser.FSLASH
                        },
                        { 
                            "%",
                            CqlParser.PERCENT
                        }
                    };
                    _operators = dictionary;
                }
                return _operators;
            }
        }

        private static Dictionary<string, short> InternalPunctuatorDictionary
        {
            get
            {
                if (_punctuators == null)
                {
                    Dictionary<string, short> dictionary = new Dictionary<string, short>(0x10, _stringComparer) {
                        { 
                            ",",
                            CqlParser.COMMA
                        },
                        { 
                            ":",
                            CqlParser.COLON
                        },
                        { 
                            ".",
                            CqlParser.DOT
                        },
                        { 
                            "?",
                            CqlParser.QMARK
                        },
                        { 
                            "(",
                            CqlParser.L_PAREN
                        },
                        { 
                            ")",
                            CqlParser.R_PAREN
                        },
                        { 
                            "[",
                            CqlParser.L_BRACE
                        },
                        { 
                            "]",
                            CqlParser.R_BRACE
                        },
                        { 
                            "{",
                            CqlParser.L_CURLY
                        },
                        { 
                            "}",
                            CqlParser.R_CURLY
                        },
                        { 
                            ";",
                            CqlParser.SCOLON
                        },
                        { 
                            "=",
                            CqlParser.EQUAL
                        }
                    };
                    _punctuators = dictionary;
                }
                return _punctuators;
            }
        }

        internal int IPos =>
            this._iPos;

        internal string YYText =>
            this.yytext();

        private delegate CqlLexer.Token AcceptMethod();

        internal class TerminalToken
        {
            private int _iPos;
            private string _token;

            internal TerminalToken(string token, int iPos)
            {
                this._token = token;
                this._iPos = iPos;
            }

            internal int IPos =>
                this._iPos;

            internal string Token =>
                this._token;
        }

        internal class Token
        {
            private short _tokenId;
            private object _tokenValue;

            internal Token(short tokenId, AstNode tokenValue)
            {
                this._tokenId = tokenId;
                this._tokenValue = tokenValue;
            }

            internal Token(short tokenId, CqlLexer.TerminalToken terminal)
            {
                this._tokenId = tokenId;
                this._tokenValue = terminal;
            }

            internal short TokenId =>
                this._tokenId;

            internal object Value =>
                this._tokenValue;
        }

        internal static class yy_translate
        {
            internal static char translate(char c)
            {
                if (char.IsWhiteSpace(c) || char.IsControl(c))
                {
                    if (CqlLexer.IsNewLine(c))
                    {
                        return '\n';
                    }
                    return ' ';
                }
                if (c < '\x007f')
                {
                    return c;
                }
                if ((!char.IsLetter(c) && !char.IsSymbol(c)) && !char.IsNumber(c))
                {
                    return '`';
                }
                return 'a';
            }
        }
    }
}

