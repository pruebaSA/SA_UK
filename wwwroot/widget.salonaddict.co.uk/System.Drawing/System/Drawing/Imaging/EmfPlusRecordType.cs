﻿namespace System.Drawing.Imaging
{
    using System;

    public enum EmfPlusRecordType
    {
        BeginContainer = 0x4027,
        BeginContainerNoParams = 0x4028,
        Clear = 0x4009,
        Comment = 0x4003,
        DrawArc = 0x4012,
        DrawBeziers = 0x4019,
        DrawClosedCurve = 0x4017,
        DrawCurve = 0x4018,
        DrawDriverString = 0x4036,
        DrawEllipse = 0x400f,
        DrawImage = 0x401a,
        DrawImagePoints = 0x401b,
        DrawLines = 0x400d,
        DrawPath = 0x4015,
        DrawPie = 0x4011,
        DrawRects = 0x400b,
        DrawString = 0x401c,
        EmfAbortPath = 0x44,
        EmfAlphaBlend = 0x72,
        EmfAngleArc = 0x29,
        EmfArcTo = 0x37,
        EmfBeginPath = 0x3b,
        EmfBitBlt = 0x4c,
        EmfChord = 0x2e,
        EmfCloseFigure = 0x3d,
        EmfColorCorrectPalette = 0x6f,
        EmfColorMatchToTargetW = 0x79,
        EmfCreateBrushIndirect = 0x27,
        EmfCreateColorSpace = 0x63,
        EmfCreateColorSpaceW = 0x7a,
        EmfCreateDibPatternBrushPt = 0x5e,
        EmfCreateMonoBrush = 0x5d,
        EmfCreatePalette = 0x31,
        EmfCreatePen = 0x26,
        EmfDeleteColorSpace = 0x65,
        EmfDeleteObject = 40,
        EmfDrawEscape = 0x69,
        EmfEllipse = 0x2a,
        EmfEndPath = 60,
        EmfEof = 14,
        EmfExcludeClipRect = 0x1d,
        EmfExtCreateFontIndirect = 0x52,
        EmfExtCreatePen = 0x5f,
        EmfExtEscape = 0x6a,
        EmfExtFloodFill = 0x35,
        EmfExtSelectClipRgn = 0x4b,
        EmfExtTextOutA = 0x53,
        EmfExtTextOutW = 0x54,
        EmfFillPath = 0x3e,
        EmfFillRgn = 0x47,
        EmfFlattenPath = 0x41,
        EmfForceUfiMapping = 0x6d,
        EmfFrameRgn = 0x48,
        EmfGdiComment = 70,
        EmfGlsBoundedRecord = 0x67,
        EmfGlsRecord = 0x66,
        EmfGradientFill = 0x76,
        EmfHeader = 1,
        EmfIntersectClipRect = 30,
        EmfInvertRgn = 0x49,
        EmfLineTo = 0x36,
        EmfMaskBlt = 0x4e,
        EmfMax = 0x7a,
        EmfMin = 1,
        EmfModifyWorldTransform = 0x24,
        EmfMoveToEx = 0x1b,
        EmfNamedEscpae = 110,
        EmfOffsetClipRgn = 0x1a,
        EmfPaintRgn = 0x4a,
        EmfPie = 0x2f,
        EmfPixelFormat = 0x68,
        EmfPlgBlt = 0x4f,
        EmfPlusRecordBase = 0x4000,
        EmfPolyBezier = 2,
        EmfPolyBezier16 = 0x55,
        EmfPolyBezierTo = 5,
        EmfPolyBezierTo16 = 0x58,
        EmfPolyDraw = 0x38,
        EmfPolyDraw16 = 0x5c,
        EmfPolygon = 3,
        EmfPolygon16 = 0x56,
        EmfPolyline = 4,
        EmfPolyline16 = 0x57,
        EmfPolyLineTo = 6,
        EmfPolylineTo16 = 0x59,
        EmfPolyPolygon = 8,
        EmfPolyPolygon16 = 0x5b,
        EmfPolyPolyline = 7,
        EmfPolyPolyline16 = 90,
        EmfPolyTextOutA = 0x60,
        EmfPolyTextOutW = 0x61,
        EmfRealizePalette = 0x34,
        EmfRectangle = 0x2b,
        EmfReserved069 = 0x45,
        EmfReserved117 = 0x75,
        EmfResizePalette = 0x33,
        EmfRestoreDC = 0x22,
        EmfRoundArc = 0x2d,
        EmfRoundRect = 0x2c,
        EmfSaveDC = 0x21,
        EmfScaleViewportExtEx = 0x1f,
        EmfScaleWindowExtEx = 0x20,
        EmfSelectClipPath = 0x43,
        EmfSelectObject = 0x25,
        EmfSelectPalette = 0x30,
        EmfSetArcDirection = 0x39,
        EmfSetBkColor = 0x19,
        EmfSetBkMode = 0x12,
        EmfSetBrushOrgEx = 13,
        EmfSetColorAdjustment = 0x17,
        EmfSetColorSpace = 100,
        EmfSetDIBitsToDevice = 80,
        EmfSetIcmMode = 0x62,
        EmfSetIcmProfileA = 0x70,
        EmfSetIcmProfileW = 0x71,
        EmfSetLayout = 0x73,
        EmfSetLinkedUfis = 0x77,
        EmfSetMapMode = 0x11,
        EmfSetMapperFlags = 0x10,
        EmfSetMetaRgn = 0x1c,
        EmfSetMiterLimit = 0x3a,
        EmfSetPaletteEntries = 50,
        EmfSetPixelV = 15,
        EmfSetPolyFillMode = 0x13,
        EmfSetROP2 = 20,
        EmfSetStretchBltMode = 0x15,
        EmfSetTextAlign = 0x16,
        EmfSetTextColor = 0x18,
        EmfSetTextJustification = 120,
        EmfSetViewportExtEx = 11,
        EmfSetViewportOrgEx = 12,
        EmfSetWindowExtEx = 9,
        EmfSetWindowOrgEx = 10,
        EmfSetWorldTransform = 0x23,
        EmfSmallTextOut = 0x6c,
        EmfStartDoc = 0x6b,
        EmfStretchBlt = 0x4d,
        EmfStretchDIBits = 0x51,
        EmfStrokeAndFillPath = 0x3f,
        EmfStrokePath = 0x40,
        EmfTransparentBlt = 0x74,
        EmfWidenPath = 0x42,
        EndContainer = 0x4029,
        EndOfFile = 0x4002,
        FillClosedCurve = 0x4016,
        FillEllipse = 0x400e,
        FillPath = 0x4014,
        FillPie = 0x4010,
        FillPolygon = 0x400c,
        FillRects = 0x400a,
        FillRegion = 0x4013,
        GetDC = 0x4004,
        Header = 0x4001,
        Invalid = 0x4000,
        Max = 0x4036,
        Min = 0x4001,
        MultiFormatEnd = 0x4007,
        MultiFormatSection = 0x4006,
        MultiFormatStart = 0x4005,
        MultiplyWorldTransform = 0x402c,
        Object = 0x4008,
        OffsetClip = 0x4035,
        ResetClip = 0x4031,
        ResetWorldTransform = 0x402b,
        Restore = 0x4026,
        RotateWorldTransform = 0x402f,
        Save = 0x4025,
        ScaleWorldTransform = 0x402e,
        SetAntiAliasMode = 0x401e,
        SetClipPath = 0x4033,
        SetClipRect = 0x4032,
        SetClipRegion = 0x4034,
        SetCompositingMode = 0x4023,
        SetCompositingQuality = 0x4024,
        SetInterpolationMode = 0x4021,
        SetPageTransform = 0x4030,
        SetPixelOffsetMode = 0x4022,
        SetRenderingOrigin = 0x401d,
        SetTextContrast = 0x4020,
        SetTextRenderingHint = 0x401f,
        SetWorldTransform = 0x402a,
        Total = 0x4037,
        TranslateWorldTransform = 0x402d,
        WmfAnimatePalette = 0x10436,
        WmfArc = 0x10817,
        WmfBitBlt = 0x10922,
        WmfChord = 0x10830,
        WmfCreateBrushIndirect = 0x102fc,
        WmfCreateFontIndirect = 0x102fb,
        WmfCreatePalette = 0x100f7,
        WmfCreatePatternBrush = 0x101f9,
        WmfCreatePenIndirect = 0x102fa,
        WmfCreateRegion = 0x106ff,
        WmfDeleteObject = 0x101f0,
        WmfDibBitBlt = 0x10940,
        WmfDibCreatePatternBrush = 0x10142,
        WmfDibStretchBlt = 0x10b41,
        WmfEllipse = 0x10418,
        WmfEscape = 0x10626,
        WmfExcludeClipRect = 0x10415,
        WmfExtFloodFill = 0x10548,
        WmfExtTextOut = 0x10a32,
        WmfFillRegion = 0x10228,
        WmfFloodFill = 0x10419,
        WmfFrameRegion = 0x10429,
        WmfIntersectClipRect = 0x10416,
        WmfInvertRegion = 0x1012a,
        WmfLineTo = 0x10213,
        WmfMoveTo = 0x10214,
        WmfOffsetCilpRgn = 0x10220,
        WmfOffsetViewportOrg = 0x10211,
        WmfOffsetWindowOrg = 0x1020f,
        WmfPaintRegion = 0x1012b,
        WmfPatBlt = 0x1061d,
        WmfPie = 0x1081a,
        WmfPolygon = 0x10324,
        WmfPolyline = 0x10325,
        WmfPolyPolygon = 0x10538,
        WmfRealizePalette = 0x10035,
        WmfRecordBase = 0x10000,
        WmfRectangle = 0x1041b,
        WmfResizePalette = 0x10139,
        WmfRestoreDC = 0x10127,
        WmfRoundRect = 0x1061c,
        WmfSaveDC = 0x1001e,
        WmfScaleViewportExt = 0x10412,
        WmfScaleWindowExt = 0x10410,
        WmfSelectClipRegion = 0x1012c,
        WmfSelectObject = 0x1012d,
        WmfSelectPalette = 0x10234,
        WmfSetBkColor = 0x10201,
        WmfSetBkMode = 0x10102,
        WmfSetDibToDev = 0x10d33,
        WmfSetLayout = 0x10149,
        WmfSetMapMode = 0x10103,
        WmfSetMapperFlags = 0x10231,
        WmfSetPalEntries = 0x10037,
        WmfSetPixel = 0x1041f,
        WmfSetPolyFillMode = 0x10106,
        WmfSetRelAbs = 0x10105,
        WmfSetROP2 = 0x10104,
        WmfSetStretchBltMode = 0x10107,
        WmfSetTextAlign = 0x1012e,
        WmfSetTextCharExtra = 0x10108,
        WmfSetTextColor = 0x10209,
        WmfSetTextJustification = 0x1020a,
        WmfSetViewportExt = 0x1020e,
        WmfSetViewportOrg = 0x1020d,
        WmfSetWindowExt = 0x1020c,
        WmfSetWindowOrg = 0x1020b,
        WmfStretchBlt = 0x10b23,
        WmfStretchDib = 0x10f43,
        WmfTextOut = 0x10521
    }
}

