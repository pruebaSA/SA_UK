namespace PdfSharp.Pdf.Content.Objects
{
    using System;
    using System.Collections.Generic;

    internal sealed class OpCodes
    {
        private static OpCode b = new OpCode("b", OpCodeName.b, 0, "closepath, fill, stroke", OpCodeFlags.None, "Close, fill, and stroke path using nonzero winding number");
        private static OpCode B = new OpCode("B", OpCodeName.B, 0, "fill, stroke", OpCodeFlags.None, "Fill and stroke path using nonzero winding number rule");
        private static OpCode BDC = new OpCode("BDC", OpCodeName.BDC, 2, null, OpCodeFlags.None, "(PDF 1.2) Begin marked-content sequence with property list");
        private static OpCode BI = new OpCode("BI", OpCodeName.BI, 0, null, OpCodeFlags.None, "Begin inline image object");
        private static OpCode BMC = new OpCode("BMC", OpCodeName.BMC, 1, null, OpCodeFlags.None, "(PDF 1.2) Begin marked-content sequence");
        private static OpCode BT = new OpCode("BT", OpCodeName.BT, 0, null, OpCodeFlags.None, "Begin text object");
        private static OpCode bx = new OpCode("b*", OpCodeName.bx, 0, "closepath, eofill, stroke", OpCodeFlags.None, "Close, fill, and stroke path using even-odd rule");
        private static OpCode Bx = new OpCode("B*", OpCodeName.Bx, 0, "eofill, stroke", OpCodeFlags.None, "Fill and stroke path using even-odd rule");
        private static OpCode BX = new OpCode("BX", OpCodeName.BX, 0, null, OpCodeFlags.None, "(PDF 1.1) Begin compatibility section");
        private static OpCode c = new OpCode("c", OpCodeName.c, 6, "curveto", OpCodeFlags.None, "Append curved segment to path (three control points)");
        private static OpCode cm = new OpCode("cm", OpCodeName.cm, 6, "concat", OpCodeFlags.None, "Concatenate matrix to current transformation matrix");
        private static OpCode cs = new OpCode("cs", OpCodeName.cs, 1, "setcolorspace", OpCodeFlags.None, "(PDF 1.1) Set color space for nonstroking operations");
        private static OpCode CS = new OpCode("CS", OpCodeName.CS, 1, "setcolorspace", OpCodeFlags.None, "(PDF 1.1) Set color space for stroking operations");
        private static OpCode d = new OpCode("d", OpCodeName.d, 2, "setdash", OpCodeFlags.None, "Set line dash pattern");
        private static OpCode d0 = new OpCode("d0", OpCodeName.d0, 2, "setcharwidth", OpCodeFlags.None, "Set glyph width in Type 3 font");
        private static OpCode d1 = new OpCode("d1", OpCodeName.d1, 6, "setcachedevice", OpCodeFlags.None, "Set glyph width and bounding box in Type 3 font");
        private static OpCode Do = new OpCode("Do", OpCodeName.Do, 1, null, OpCodeFlags.None, "Invoke named XObject");
        private static OpCode DP = new OpCode("DP", OpCodeName.DP, 2, null, OpCodeFlags.None, "(PDF 1.2) Define marked-content point with property list");
        private static OpCode EI = new OpCode("EI", OpCodeName.EI, 0, null, OpCodeFlags.None, "End inline image object");
        private static OpCode EMC = new OpCode("EMC", OpCodeName.EMC, 0, null, OpCodeFlags.None, "(PDF 1.2) End marked-content sequence");
        private static OpCode ET = new OpCode("ET", OpCodeName.ET, 0, null, OpCodeFlags.None, "End text object");
        private static OpCode EX = new OpCode("EX", OpCodeName.EX, 0, null, OpCodeFlags.None, "(PDF 1.1) End compatibility section");
        private static OpCode f = new OpCode("f", OpCodeName.f, 0, "fill", OpCodeFlags.None, "Fill path using nonzero winding number rule");
        private static OpCode F = new OpCode("F", OpCodeName.F, 0, "fill", OpCodeFlags.None, "Fill path using nonzero winding number rule (obsolete)");
        private static OpCode fx = new OpCode("f*", OpCodeName.fx, 0, "eofill", OpCodeFlags.None, "Fill path using even-odd rule");
        private static OpCode g = new OpCode("g", OpCodeName.g, 1, "setgray", OpCodeFlags.None, "Set gray level for nonstroking operations");
        private static OpCode G = new OpCode("G", OpCodeName.G, 1, "setgray", OpCodeFlags.None, "Set gray level for stroking operations");
        private static OpCode gs = new OpCode("gs", OpCodeName.gs, 1, null, OpCodeFlags.None, "(PDF 1.2) Set parameters from graphics state parameter dictionary");
        private static OpCode h = new OpCode("h", OpCodeName.h, 0, "closepath", OpCodeFlags.None, "Close subpath");
        private static OpCode i = new OpCode("i", OpCodeName.i, 1, "setflat", OpCodeFlags.None, "Set flatness tolerance");
        private static OpCode ID = new OpCode("ID", OpCodeName.ID, 0, null, OpCodeFlags.None, "Begin inline image data");
        private static OpCode j = new OpCode("j", OpCodeName.j, 1, "setlinejoin", OpCodeFlags.None, "Set line join style");
        private static OpCode J = new OpCode("J", OpCodeName.J, 1, "setlinecap", OpCodeFlags.None, "Set line cap style");
        private static OpCode k = new OpCode("k", OpCodeName.k, 4, "setcmykcolor", OpCodeFlags.None, "Set CMYK color for nonstroking operations");
        private static OpCode K = new OpCode("K", OpCodeName.K, 4, "setcmykcolor", OpCodeFlags.None, "Set CMYK color for stroking operations");
        private static OpCode l = new OpCode("l", OpCodeName.l, 2, "lineto", OpCodeFlags.None, "Append straight line segment to path");
        private static OpCode m = new OpCode("m", OpCodeName.m, 2, "moveto", OpCodeFlags.None, "Begin new subpath");
        private static OpCode M = new OpCode("M", OpCodeName.M, 1, "setmiterlimit", OpCodeFlags.None, "Set miter limit");
        private static OpCode MP = new OpCode("MP", OpCodeName.MP, 1, null, OpCodeFlags.None, "(PDF 1.2) Define marked-content point");
        private static OpCode n = new OpCode("n", OpCodeName.n, 0, null, OpCodeFlags.None, "End path without filling or stroking");
        private static OpCode[] ops = new OpCode[] { 
            b, B, bx, Bx, BDC, BI, BMC, BT, BX, c, cm, CS, cs, d, d0, d1,
            Do, DP, EI, EMC, ET, EX, f, F, fx, G, g, gs, h, i, ID, j,
            J, K, k, l, m, M, MP, n, q, Q, re, RG, rg, ri, s, S,
            SC, sc, SCN, scn, sh, Tx, Tc, Td, TD, Tf, Tj, TJ, TL, Tm, Tr, Ts,
            Tw, Tz, v, w, W, Wx, y, QuoteSingle, QuoteDbl
        };
        private static OpCode q = new OpCode("q", OpCodeName.q, 0, "gsave", OpCodeFlags.None, "Save graphics state");
        private static OpCode Q = new OpCode("Q", OpCodeName.Q, 0, "grestore", OpCodeFlags.None, "Restore graphics state");
        private static OpCode QuoteDbl = new OpCode("\"", OpCodeName.QuoteDbl, 3, null, OpCodeFlags.TextOut, "Set word and character spacing, move to next line, and show text");
        private static OpCode QuoteSingle = new OpCode("'", OpCodeName.QuoteSingle, 1, null, OpCodeFlags.TextOut, "Move to next line and show text");
        private static OpCode re = new OpCode("re", OpCodeName.re, 4, null, OpCodeFlags.None, "Append rectangle to path");
        private static OpCode rg = new OpCode("rg", OpCodeName.rg, 3, "setrgbcolor", OpCodeFlags.None, "Set RGB color for nonstroking operations");
        private static OpCode RG = new OpCode("RG", OpCodeName.RG, 3, "setrgbcolor", OpCodeFlags.None, "Set RGB color for stroking operations");
        private static OpCode ri = new OpCode("ri", OpCodeName.ri, 1, null, OpCodeFlags.None, "Set color rendering intent");
        private static OpCode s = new OpCode("s", OpCodeName.s, 0, "closepath,stroke", OpCodeFlags.None, "Close and stroke path");
        private static OpCode S = new OpCode("S", OpCodeName.S, 0, "stroke", OpCodeFlags.None, "Stroke path");
        private static OpCode sc = new OpCode("sc", OpCodeName.sc, -1, "setcolor", OpCodeFlags.None, "(PDF 1.1) Set color for nonstroking operations");
        private static OpCode SC = new OpCode("SC", OpCodeName.SC, -1, "setcolor", OpCodeFlags.None, "(PDF 1.1) Set color for stroking operations");
        private static OpCode scn = new OpCode("scn", OpCodeName.scn, -1, "setcolor", OpCodeFlags.None, "(PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)");
        private static OpCode SCN = new OpCode("SCN", OpCodeName.SCN, -1, "setcolor", OpCodeFlags.None, "(PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)");
        private static OpCode sh = new OpCode("sh", OpCodeName.sh, 1, "shfill", OpCodeFlags.None, "(PDF 1.3) Paint area defined by shading pattern");
        private static readonly Dictionary<string, OpCode> stringToOpCode = new Dictionary<string, OpCode>();
        private static OpCode Tc = new OpCode("Tc", OpCodeName.Tc, 1, null, OpCodeFlags.None, "Set character spacing");
        private static OpCode Td = new OpCode("Td", OpCodeName.Td, 2, null, OpCodeFlags.None, "Move text position");
        private static OpCode TD = new OpCode("TD", OpCodeName.TD, 2, null, OpCodeFlags.None, "Move text position and set leading");
        private static OpCode Tf = new OpCode("Tf", OpCodeName.Tf, 2, "selectfont", OpCodeFlags.None, "Set text font and size");
        private static OpCode Tj = new OpCode("Tj", OpCodeName.Tj, 1, "show", OpCodeFlags.TextOut, "Show text");
        private static OpCode TJ = new OpCode("TJ", OpCodeName.TJ, 1, null, OpCodeFlags.TextOut, "Show text, allowing individual glyph positioning");
        private static OpCode TL = new OpCode("TL", OpCodeName.TL, 1, null, OpCodeFlags.None, "Set text leading");
        private static OpCode Tm = new OpCode("Tm", OpCodeName.Tm, 6, null, OpCodeFlags.None, "Set text matrix and text line matrix");
        private static OpCode Tr = new OpCode("Tr", OpCodeName.Tr, 1, null, OpCodeFlags.None, "Set text rendering mode");
        private static OpCode Ts = new OpCode("Ts", OpCodeName.Ts, 1, null, OpCodeFlags.None, "Set text rise");
        private static OpCode Tw = new OpCode("Tw", OpCodeName.Tw, 1, null, OpCodeFlags.None, "Set word spacing");
        private static OpCode Tx = new OpCode("T*", OpCodeName.Tx, 0, null, OpCodeFlags.None, "Move to start of next text line");
        private static OpCode Tz = new OpCode("Tz", OpCodeName.Tz, 1, null, OpCodeFlags.None, "Set horizontal text scaling");
        private static OpCode v = new OpCode("v", OpCodeName.v, 4, "curveto", OpCodeFlags.None, "Append curved segment to path (initial point replicated)");
        private static OpCode w = new OpCode("w", OpCodeName.w, 1, "setlinewidth", OpCodeFlags.None, "Set line width");
        private static OpCode W = new OpCode("W", OpCodeName.W, 0, "clip", OpCodeFlags.None, "Set clipping path using nonzero winding number rule");
        private static OpCode Wx = new OpCode("W*", OpCodeName.Wx, 0, "eoclip", OpCodeFlags.None, "Set clipping path using even-odd rule");
        private static OpCode y = new OpCode("y", OpCodeName.y, 4, "curveto", OpCodeFlags.None, "Append curved segment to path (final point replicated)");

        static OpCodes()
        {
            for (int i = 0; i < ops.Length; i++)
            {
                OpCode code = ops[i];
                stringToOpCode.Add(code.Name, code);
            }
        }

        private OpCodes()
        {
        }

        public static COperator OperatorFromName(string name)
        {
            COperator @operator = null;
            OpCode opcode = stringToOpCode[name];
            if (opcode != null)
            {
                @operator = new COperator(opcode);
            }
            return @operator;
        }
    }
}

