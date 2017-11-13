namespace MS.Utility
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;

    [FriendAccessAllowed]
    internal sealed class EventTrace
    {
        internal static readonly TraceProvider EventProvider = null;

        [SecurityCritical, SecurityTreatAsSafe]
        static EventTrace()
        {
            EventProvider = new TraceProvider("WindowsClientProvider", new Guid(0xa42c77db, 0x874f, 0x422e, 0x9b, 0x44, 0x6d, 0x89, 0xfe, 0x2b, 0xd3, 0xe5));
        }

        private EventTrace()
        {
        }

        internal static Guid GuidFromId(EventTraceGuidId guidId)
        {
            switch (guidId)
            {
                case EventTraceGuidId.DISPATCHERDISPATCHGUID:
                    return new Guid(0x2481a374, 0x999f, 0x4ad2, 0x9f, 0x22, 0x6b, 0x7c, 0x8e, 0x2a, 0x5d, 0xb0);

                case EventTraceGuidId.DISPATCHERPOSTGUID:
                    return new Guid(0x76287aef, 0xf674, 0x4061, 0xa6, 10, 0x76, 0xf9, 0x55, 80, 0xef, 0xeb);

                case EventTraceGuidId.DISPATCHERABORTGUID:
                    return new Guid(0x39404da9, 0x413f, 0x4581, 160, 0xa1, 0x47, 0x15, 0x16, 0x8b, 90, 0xd8);

                case EventTraceGuidId.DISPATCHERPROMOTEGUID:
                    return new Guid(0x632d4e9e, 0xb988, 0x4b32, 0xab, 0x2a, 0xb3, 0x7a, 0xa3, 0x49, 0x27, 0xee);

                case EventTraceGuidId.DISPATCHERIDLEGUID:
                    return new Guid(0xc626ebef, 0x780, 0x487f, 0x81, 0xd7, 0x38, 0xd3, 240, 0xa6, 240, 0x5e);

                case EventTraceGuidId.LAYOUTPASSGUID:
                    return new Guid(0xa3edb710, 0x21fc, 0x4f91, 0x97, 0xf4, 0xac, 0x2b, 13, 0xf1, 0xc2, 15);

                case EventTraceGuidId.MEASUREGUID:
                    return new Guid(0x3005e67b, 0x129c, 0x4ced, 0xbc, 170, 0x91, 0xd7, 0xd7, 0x3b, 0x15, 0x44);

                case EventTraceGuidId.ARRANGEGUID:
                    return new Guid(0x4b0ef3d1, 0xcbb, 0x4847, 0xb9, 0x8f, 0x16, 0x40, 0x8e, 0x7e, 0x83, 0xf3);

                case EventTraceGuidId.ONRENDERGUID:
                    return new Guid(0x3a475cef, 0xe2a, 0x449b, 0x98, 110, 0xef, 0xff, 0x5d, 0x62, 0x60, 0xe7);

                case EventTraceGuidId.MEDIACREATEVISUALGUID:
                    return new Guid(0x2dbecf62, 0x51ea, 0x493a, 0x8d, 0xd0, 0x4b, 0xee, 0x1c, 0xcb, 0xe8, 170);

                case EventTraceGuidId.HWNDMESSAGEGUID:
                    return new Guid(0x4ac79bac, 0x7dfb, 0x4402, 0xa9, 0x10, 0xfd, 0xaf, 0xe1, 0x6f, 0x29, 0xb2);

                case EventTraceGuidId.RENDERHANDLERGUID:
                    return new Guid(0x7723d8b7, 0x488b, 0x4f80, 0xb0, 0x89, 70, 0xa4, 0xc6, 0xac, 0xa1, 0xc4);

                case EventTraceGuidId.ANIMRENDERHANDLERGUID:
                    return new Guid(0x521c1c8d, 0xfaaa, 0x435b, 0xad, 140, 0x1d, 100, 0x44, 0x2b, 0xfd, 0x70);

                case EventTraceGuidId.MEDIARENDERGUID:
                    return new Guid(0x6827e447, 0xe0e, 0x4b5e, 0xae, 0x81, 0xb7, 0x9a, 0, 0xec, 0x83, 0x49);

                case EventTraceGuidId.POSTRENDERGUID:
                    return new Guid(0xfb69cd45, 0xc00d, 0x4c23, 0x97, 0x65, 0x69, 0xc0, 3, 0x44, 0xb2, 0xc5);

                case EventTraceGuidId.PERFFREQUENCYGUID:
                    return new Guid(0x30ee0097, 0x84c, 0x408b, 0x90, 0x38, 0x73, 190, 0xd0, 0x47, 0x98, 0x73);

                case EventTraceGuidId.PRECOMPUTESCENEGUID:
                    return new Guid(0x3331420f, 0x7a3b, 0x42b6, 0x8d, 0xfe, 170, 0xbf, 0x47, 40, 1, 0xda);

                case EventTraceGuidId.COMPILESCENEGUID:
                    return new Guid(0xaf36fcb5, 0x58e5, 0x48d0, 0x88, 0xd0, 0xd8, 0xf4, 220, 0xb5, 0x6a, 0x12);

                case EventTraceGuidId.UPDATEREALIZATIONSGUID:
                    return new Guid(0x22272cfc, 0xde04, 0x4900, 0xaf, 0x53, 0x3e, 0xe8, 0xef, 30, 0xf4, 0x26);

                case EventTraceGuidId.UIRESPONSEGUID:
                    return new Guid(0xab29585b, 0x4794, 0x4465, 0x91, 230, 0x9d, 0xf5, 0x86, 0x1c, 0x88, 0xc5);

                case EventTraceGuidId.COMMITCHANNELGUID:
                    return new Guid("f9c0372e-60bd-46c9-bc64-94fe5fd31fe4");

                case EventTraceGuidId.UCENOTIFYPRESENTGUID:
                    return new Guid("24cd1476-e145-4e5a-8bfc-50c36bbdf9cc");

                case EventTraceGuidId.SCHEDULERENDERGUID:
                    return new Guid("6d5aeaf3-a433-4daa-8b31-d8ae49cf6bd1");

                case EventTraceGuidId.PARSEBAMLGUID:
                    return new Guid(0x8a1e3af5, 0x3a6d, 0x4582, 0x86, 0xd1, 0x59, 1, 0x47, 30, 0xbb, 0xde);

                case EventTraceGuidId.PARSEXMLGUID:
                    return new Guid(0xbf86e5bf, 0x3fb4, 0x442f, 0xa3, 0x4a, 0xb2, 7, 0xa3, 0xb1, 0x9c, 0x3b);

                case EventTraceGuidId.PARSEXMLINITIALIZEDGUID:
                    return new Guid(0x80484707, 0x838b, 0x4328, 0x9f, 0x31, 0x5e, 0xcb, 0xaf, 0x25, 0xa5, 0x31);

                case EventTraceGuidId.PARSEFEFCREATEINSTANCEGUID:
                    return new Guid(0xf7555161, 0x6c1a, 0x4a12, 130, 0x8d, 0x84, 0x92, 0xa7, 0x69, 0x9a, 0x49);

                case EventTraceGuidId.PARSESTYLEINSTANTIATEVISUALTREEGUID:
                    return new Guid(0xa8c3b9c0, 0x562b, 0x4509, 190, 0xcb, 160, 0x8e, 0x48, 0x1a, 0x72, 0x73);

                case EventTraceGuidId.PARSEREADERCREATEINSTANCEGUID:
                    return new Guid(0x8ba8f51c, 0x775, 0x4adf, 0x9e, 0xed, 0xb1, 0x65, 0x4c, 160, 0x88, 0xf5);

                case EventTraceGuidId.PARSEREADERCREATEINSTANCEFROMTYPEGUID:
                    return new Guid(0xda15d58, 0xc3a7, 0x40de, 0x91, 0x13, 0x72, 0xdb, 12, 0x4a, 0x93, 0x51);

                case EventTraceGuidId.APPCTORGUID:
                    return new Guid(0xf9f048c6, 0x2011, 0x4d0a, 0x81, 0x2a, 0x23, 0xa4, 0xa4, 0xd8, 1, 0xf5);

                case EventTraceGuidId.APPRUNGUID:
                    return new Guid(0x8a719d6, 0xea79, 0x4abc, 0x97, 0x99, 0x38, 0xed, 0xed, 0x60, 0x21, 0x33);

                case EventTraceGuidId.TIMEMANAGERTICKGUID:
                    return new Guid(0xea3b4b66, 0xb25f, 0x4e5d, 0x8b, 0xd4, 0xec, 0x62, 0xbb, 0x44, 0x58, 0x3e);

                case EventTraceGuidId.GENERICSTRINGGUID:
                    return new Guid(0x6b3c0258, 0x9ddb, 0x4579, 0x86, 0x60, 0x41, 0xc3, 0xad, 0xa2, 0x5c, 0x34);

                case EventTraceGuidId.FONTCACHELIMITGUID:
                    return new Guid(0xf3362106, 0xb861, 0x4980, 0x9a, 0xac, 0xb1, 0xef, 11, 0xab, 0x75, 170);

                case EventTraceGuidId.DRXOPENPACKAGEGUID:
                    return new Guid(0x2b8f75f3, 0xf8f9, 0x4075, 0xb9, 20, 90, 0xe8, 0x53, 0xc7, 0x62, 0x76);

                case EventTraceGuidId.DRXREADSTREAMGUID:
                    return new Guid(0xc2b15025, 0x7812, 0x4e44, 0x8b, 0x68, 0x7d, 0x73, 0x43, 3, 0x43, 0x8a);

                case EventTraceGuidId.DRXGETSTREAMGUID:
                    return new Guid(0x3f4510eb, 0x9ee8, 0x4b80, 0x9e, 0xc7, 0x77, 0x5e, 0xfe, 0xb1, 0xba, 0x72);

                case EventTraceGuidId.DRXPAGEVISIBLEGUID:
                    return new Guid(0x2ae7c601, 0xaec, 0x4c99, 0xba, 0x80, 0x2e, 0xca, 0x71, 0x2d, 0x1b, 0x97);

                case EventTraceGuidId.DRXPAGELOADEDGUID:
                    return new Guid(0x66028645, 0xe022, 0x4d90, 0xa7, 0xbd, 0xa8, 0xcc, 0xda, 0xcd, 0xb2, 0xe1);

                case EventTraceGuidId.DRXINVALIDATEVIEWGUID:
                    return new Guid(0x3be3740f, 0xa31, 0x4d22, 0xa2, 0xa3, 0x4d, 0x4b, 0x6d, 0x3a, 0xb8, 0x99);

                case EventTraceGuidId.DRXLINEDOWNGUID:
                    return new Guid(0xb67ab12c, 0x29bf, 0x4020, 0xb6, 120, 240, 0x43, 0x92, 0x5b, 130, 0x35);

                case EventTraceGuidId.DRXPAGEDOWNGUID:
                    return new Guid(0xd7cdeb52, 0x5ba3, 0x4e02, 0xb1, 20, 0x38, 90, 0x61, 0xe7, 0xba, 0x9d);

                case EventTraceGuidId.DRXPAGEJUMPGUID:
                    return new Guid(0xf068b137, 0x7b09, 0x44a1, 0x84, 0xd0, 0x4f, 0xf1, 0x59, 0x2e, 10, 0xc1);

                case EventTraceGuidId.DRXLAYOUTGUID:
                    return new Guid(0x34fbea40, 0x238, 0x498f, 0xb1, 0x2a, 0x63, 0x1f, 90, 0x8e, 0xf9, 0xa5);

                case EventTraceGuidId.DRXINSTANTIATEDGUID:
                    return new Guid(0x9de677e1, 0x914a, 0x426c, 0xbc, 0xd9, 0x2c, 0xcd, 0xea, 0x36, 0x48, 0xdf);

                case EventTraceGuidId.DRXSTYLECREATEDGUID:
                    return new Guid(0x69737c35, 0x1636, 0x43be, 0xa3, 0x52, 0x42, 140, 0xa3, 0x6d, 0x1b, 0x2c);

                case EventTraceGuidId.DRXFINDGUID:
                    return new Guid(0xff8efb74, 0xefaa, 0x424d, 0x90, 0x22, 0xee, 0x8d, 0x21, 0xad, 0x80, 0x4e);

                case EventTraceGuidId.DRXZOOMGUID:
                    return new Guid(0x2e5045a1, 0x8dac, 0x4c90, 0x99, 0x95, 50, 0x60, 0xde, 0x16, 0x6c, 0x8f);

                case EventTraceGuidId.DRXENSUREOMGUID:
                    return new Guid(0x28e3a8bb, 0xaebb, 0x48e8, 0x86, 0xb6, 50, 0x75, 0x9b, 0x47, 0xfc, 190);

                case EventTraceGuidId.DRXGETPAGEGUID:
                    return new Guid(0xa0c17259, 0xc6b1, 0x4850, 0xa9, 0xab, 0x13, 0x65, 0x9f, 230, 220, 0x58);

                case EventTraceGuidId.DRXTREEFLATTENGUID:
                    return new Guid(0xb4557454, 0x212b, 0x4f57, 0xb9, 0xca, 0x2b, 0xa9, 0xd5, 130, 0x73, 0xb3);

                case EventTraceGuidId.DRXALPHAFLATTENGUID:
                    return new Guid(0x302f02e9, 0xf025, 0x4083, 0xab, 0xd5, 0x2c, 0xe3, 170, 0xa9, 0xa3, 0xcf);

                case EventTraceGuidId.DRXGETDEVMODEGUID:
                    return new Guid(0x573ea8dc, 0xdb6c, 0x42c0, 0x91, 0xf8, 150, 0x4e, 0x39, 0xcb, 0x6a, 0x70);

                case EventTraceGuidId.DRXSTARTDOCGUID:
                    return new Guid(0xf3fba666, 0xfa0f, 0x4487, 0xb8, 70, 0x9f, 0x20, 0x48, 0x11, 0xbf, 0x3d);

                case EventTraceGuidId.DRXENDDOCGUID:
                    return new Guid(0x743dd3cf, 0xbbce, 0x4e69, 0xa4, 0xdb, 0x85, 0x22, 110, 0xc6, 0xa4, 0x45);

                case EventTraceGuidId.DRXSTARTPAGEGUID:
                    return new Guid(0x5303d552, 0x28ab, 0x4dac, 0x8b, 0xcd, 15, 0x7d, 0x56, 0x75, 0xa1, 0x57);

                case EventTraceGuidId.DRXENDPAGEGUID:
                    return new Guid(0xe20fddf4, 0x17a6, 0x4e5f, 0x86, 0x93, 0x3d, 0xd7, 0xcb, 4, 0x94, 0x22);

                case EventTraceGuidId.DRXCOMMITPAGEGUID:
                    return new Guid(0x7d7ee18d, 0xaea5, 0x493f, 0x9e, 0xf2, 0xbb, 0xdb, 0x36, 0xfc, 170, 120);

                case EventTraceGuidId.DRXCONVERTFONTGUID:
                    return new Guid(0x88fc2d42, 0xb1de, 0x4588, 140, 0x3b, 220, 0x5b, 0xec, 3, 0xa9, 0xac);

                case EventTraceGuidId.DRXCONVERTIMAGEGUID:
                    return new Guid(0x17fddfdc, 0xa1be, 0x43b3, 0xb2, 0xee, 0xf5, 0xe8, 0x9b, 0x7b, 0x1b, 0x26);

                case EventTraceGuidId.DRXSAVEXPSGUID:
                    return new Guid(0xba0320d5, 0x2294, 0x4067, 0x8b, 0x19, 0xef, 0x9c, 0xdd, 0xad, 0x4b, 0x1a);

                case EventTraceGuidId.DRXLOADPRIMITIVEGUID:
                    return new Guid(0xd0b70c99, 0x450e, 0x4872, 0xa2, 0xd4, 0xfb, 0xfb, 0x1d, 0xc7, 0x97, 250);

                case EventTraceGuidId.DRXSAVEPAGEGUID:
                    return new Guid(0xb0e3e78b, 0x9ac7, 0x473c, 0x89, 3, 0xb5, 210, 0x12, 0x39, 0x9e, 0x3b);

                case EventTraceGuidId.DRXSERIALIZATIONGUID:
                    return new Guid(0x527276c, 0xd3f4, 0x4293, 0xb8, 140, 0xec, 0xdf, 0x7c, 0xac, 0x44, 0x30);

                case EventTraceGuidId.PROPERTYVALIDATIONGUID:
                    return new Guid(0xe0bb1a36, 0x6dc9, 0x459b, 0xab, 0x81, 0xb5, 0xda, 0x91, 14, 0x5d, 0x37);

                case EventTraceGuidId.PROPERTYINVALIDATIONGUID:
                    return new Guid(0x61159ef2, 0x501f, 0x452f, 0x8d, 0x13, 0x51, 0xcd, 5, 0xf2, 0x3e, 130);

                case EventTraceGuidId.PROPERTYPARENTCHECKGUID:
                    return new Guid(0x831bea07, 0x5a2c, 0x434c, 0x8e, 0xf8, 0x7e, 0xba, 0x41, 200, 0x81, 0xfb);

                case EventTraceGuidId.RESOURCEFINDGUID:
                    return new Guid(0x228d90d5, 0x7e19, 0x4480, 0x9e, 0x56, 0x3a, 0xf2, 0xe9, 15, 0x8d, 0xa6);

                case EventTraceGuidId.RESOURCECACHEVALUEGUID:
                    return new Guid(0x3b253e2d, 0x72a5, 0x489e, 140, 0x65, 0x56, 0xc1, 230, 200, 0x59, 0xb5);

                case EventTraceGuidId.RESOURCECACHENULLGUID:
                    return new Guid(0x7866a65b, 0x2f38, 0x43b6, 0xab, 210, 0xdf, 0x43, 0x3b, 0xbc, 160, 0x73);

                case EventTraceGuidId.RESOURCECACHEMISSGUID:
                    return new Guid(0x420755f, 0xd416, 0x4f15, 0x93, 0x9f, 0x3e, 0x2c, 0xd3, 0xfc, 0xea, 0x23);

                case EventTraceGuidId.RESOURCESTOCKGUID:
                    return new Guid(0x6f0fee4, 0x72dd, 0x4802, 0xbd, 0x3d, 9, 0x85, 0x13, 0x9f, 0xa9, 0x1a);

                case EventTraceGuidId.RESOURCEBAMLASSEMBLYGUID:
                    return new Guid(0x19df4373, 0x6680, 0x4a04, 140, 0x77, 210, 0xf6, 0x80, 0x9c, 0xa7, 3);

                case EventTraceGuidId.CREATESTICKYNOTEGUID:
                    return new Guid(0xe3dbffac, 0x1e92, 0x4f48, 0xa6, 90, 0xc2, 0x90, 0xbd, 0x5f, 0x5f, 0x15);

                case EventTraceGuidId.DELETETEXTNOTEGUID:
                    return new Guid(0x7626a2f9, 0x9a61, 0x43a3, 0xb7, 0xcc, 0xbb, 0x84, 0xc2, 0x49, 0x3a, 0xa7);

                case EventTraceGuidId.DELETEINKNOTEGUID:
                    return new Guid(0xbf7e2a93, 0x9d6a, 0x453e, 0xba, 0xdb, 0x3f, 0x8f, 0x60, 7, 0x5c, 0xf2);

                case EventTraceGuidId.CREATEHIGHLIGHTGUID:
                    return new Guid(0xc2a5edb8, 0xac73, 0x41ef, 0xa9, 0x43, 0xa8, 0xa4, 0x9f, 0xa2, 0x84, 0xb1);

                case EventTraceGuidId.CLEARHIGHLIGHTGUID:
                    return new Guid(0xe1a59147, 0xd28d, 0x4c5f, 0xb9, 0x80, 0x69, 0x1b, 0xe2, 0xfd, 0x42, 8);

                case EventTraceGuidId.LOADANNOTATIONSGUID:
                    return new Guid(0xcf3a283e, 0xc004, 0x4e7d, 0xb3, 0xb9, 0xcc, 0x9b, 0x58, 0x2a, 0x4a, 0x5f);

                case EventTraceGuidId.UNLOADANNOTATIONSGUID:
                    return new Guid(0x1c97b272, 0x9b6e, 0x47b0, 0xa4, 0x70, 0xfc, 0xf4, 0x25, 0x37, 0x7d, 0xcc);

                case EventTraceGuidId.ADDANNOTATIONGUID:
                    return new Guid(0x8f4b2faa, 0x24d6, 0x4ee2, 0x99, 0x35, 0xbb, 0xf8, 0x45, 0xf7, 0x58, 0xa2);

                case EventTraceGuidId.DELETEANNOTATIONGUID:
                    return new Guid(0x4d832230, 0x952a, 0x4464, 0x80, 0xaf, 170, 0xb2, 0xac, 0x86, 0x17, 3);

                case EventTraceGuidId.GETANNOTATIONBYIDGUID:
                    return new Guid(0x3d27753f, 0xeb8a, 0x4e75, 0x9d, 0x5b, 130, 0xfb, 0xa5, 0x5c, 0xde, 0xd1);

                case EventTraceGuidId.GETANNOTATIONBYLOCGUID:
                    return new Guid(0x741a41bc, 0x8ecd, 0x43d1, 0xa7, 0xf1, 210, 250, 0xca, 0x73, 0x62, 0xef);

                case EventTraceGuidId.GETANNOTATIONSGUID:
                    return new Guid(0xcd9f6017, 0x7e64, 0x4c61, 0xb9, 0xed, 0x5c, 0x2f, 200, 0xc4, 0xd8, 0x49);

                case EventTraceGuidId.SERIALIZEANNOTATIONGUID:
                    return new Guid(0x148924b, 0x5bea, 0x43e9, 0xb3, 0xed, 0x39, 0x9c, 0xa1, 0x3b, 0x35, 0xeb);

                case EventTraceGuidId.DESERIALIZEANNOTATIONGUID:
                    return new Guid(0x2e32c255, 0xd6db, 0x4de7, 0x9e, 0x62, 0x95, 0x86, 0x37, 0x77, 120, 0xd5);

                case EventTraceGuidId.UPDATEANNOTATIONWITHSNCGUID:
                    return new Guid(0x205e0a58, 0x3c7d, 0x495d, 0xb3, 0xed, 0x18, 0xc3, 0xfb, 0x38, 0x92, 0x3f);

                case EventTraceGuidId.UPDATESNCWITHANNOTATIONGUID:
                    return new Guid(0x59c337ce, 0x9cc2, 0x4a86, 0x9b, 250, 6, 0x1f, 0xe9, 0x54, 8, 0x6b);

                case EventTraceGuidId.ANNOTATIONTEXTCHANGEDGUID:
                    return new Guid(0x8bb912b9, 0x39dd, 0x4208, 0xad, 0x62, 190, 0x66, 0xfe, 0x5b, 0x7b, 0xa5);

                case EventTraceGuidId.ANNOTATIONINKCHANGEDGUID:
                    return new Guid(0x1228e154, 0xf171, 0x426e, 0xb6, 0x72, 0x5e, 0xe1, 0x9b, 0x75, 0x5e, 0xdf);

                case EventTraceGuidId.ADDATTACHEDSNGUID:
                    return new Guid(0x9ca660f6, 0x8d7c, 0x4a90, 0xa9, 0x2f, 0x74, 0x48, 0x2d, 0x9c, 0xc1, 0xcf);

                case EventTraceGuidId.REMOVEATTACHEDSNGUID:
                    return new Guid(0x8c4c69f7, 0x1185, 0x46df, 0xa5, 0xf5, 0xe3, 0x1a, 0xc7, 0xe9, 0x6c, 7);

                case EventTraceGuidId.ADDATTACHEDHIGHLIGHTGUID:
                    return new Guid(0x56d2cae5, 0x5ec0, 0x44fb, 0x98, 0xc2, 0x45, 0x3e, 0x87, 160, 0x87, 0x7b);

                case EventTraceGuidId.REMOVEATTACHEDHIGHLIGHTGUID:
                    return new Guid(0x4c81d490, 0x9004, 0x49d1, 0x87, 0xd7, 40, 0x9d, 0x53, 0xa3, 20, 0xef);

                case EventTraceGuidId.ADDATTACHEDMHGUID:
                    return new Guid(0x7ea1d548, 0xca17, 0xca17, 0xa1, 0xa8, 0xf1, 0x85, 0x7d, 0xb6, 0x30, 0x2e);

                case EventTraceGuidId.REMOVEATTACHEDMHGUID:
                    return new Guid(0x296c7961, 0xb975, 0x450b, 0x89, 0x75, 0xbf, 0x86, 0x2b, 0x6c, 0x71, 0x59);

                case EventTraceGuidId.PRESENTATIONHOSTGUID:
                    return new Guid(0xed251760, 0x7bbc, 0x4b25, 0x83, 40, 0xcd, 0x7f, 0x27, 0x1f, 0xee, 0x89);

                case EventTraceGuidId.HOSTINGGUID:
                    return new Guid(0x5ff6b585, 0x7fb9, 0x4189, 190, 0xb3, 0x54, 200, 0x2c, 0xe4, 0xd7, 0xd1);

                case EventTraceGuidId.NAVIGATIONGUID:
                    return new Guid(0x6ffb9c25, 0x5c8a, 0x4091, 0x98, 0x9c, 0x5b, 0x59, 0x6a, 0xb2, 0x86, 160);

                case EventTraceGuidId.SPLASHSCREENGUID:
                    return new Guid(0x3256bdd3, 0xa738, 0x4d3a, 0x9f, 0xf5, 0x51, 0x3d, 0x7f, 0x54, 0xfc, 0xa6);
            }
            return new Guid();
        }

        internal static bool IsEnabled(Flags flags) => 
            IsEnabled(flags, Level.normal);

        internal static bool IsEnabled(Flags flag, Level level) => 
            (((EventProvider != null) && (level <= EventProvider.Level)) && (EventProvider.IsEnabled && Convert.ToBoolean((uint) (flag & ((Flags) EventProvider.Flags)))));

        internal static void NormalTraceEvent(EventTraceGuidId guidId, byte evType)
        {
            if (IsEnabled(Flags.performance, Level.normal))
            {
                EventProvider.TraceEvent(GuidFromId(guidId), evType);
            }
        }

        [Flags]
        internal enum Flags
        {
            all = 0x7fffffff,
            debugging = 1,
            performance = 2,
            PerToolSupport = 0x80,
            response = 0x20,
            security = 8,
            stress = 4,
            trace = 0x40,
            uiautomation = 0x10
        }

        internal enum HostingEvent : byte
        {
            AbortingActivation = 0x51,
            AppDomainManagerCctor = 40,
            ApplicationActivatorCreateInstanceEnd = 0x2a,
            ApplicationActivatorCreateInstanceStart = 0x29,
            AppProxyCtor = 30,
            AppProxyRunEnd = 0x22,
            AppProxyRunStart = 0x21,
            AssertAppRequirementsEnd = 0x38,
            AssertAppRequirementsStart = 0x37,
            ClickOnceActivationEnd = 0x12,
            ClickOnceActivationStart = 0x11,
            DetermineApplicationTrustEnd = 0x2c,
            DetermineApplicationTrustStart = 0x2b,
            DocObjHostCreated = 10,
            DocObjHostInitAppProxyEnd = 20,
            DocObjHostInitAppProxyStart = 0x13,
            DocObjHostRunApplicationEnd = 0x10,
            DocObjHostRunApplicationStart = 15,
            DownloadApplicationEnd = 0x3a,
            DownloadApplicationStart = 0x39,
            DownloadDeplManifestEnd = 0x36,
            DownloadDeplManifestStart = 0x35,
            DownloadProgressUpdate = 0x3b,
            FirstTimeActivation = 50,
            GetDownloadPageEnd = 0x34,
            GetDownloadPageStart = 0x33,
            IBHSRunEnd = 12,
            IBHSRunStart = 11,
            PostShutdown = 80,
            RootBrowserWindowSetupEnd = 0x20,
            RootBrowserWindowSetupStart = 0x1f,
            StartingFontCacheServiceEnd = 0x3e,
            StartingFontCacheServiceStart = 0x3d,
            UpdateBrowserCommandsEnd = 0x47,
            UpdateBrowserCommandsStart = 70,
            XappLauncherAppExit = 14,
            XappLauncherAppNavigated = 60,
            XappLauncherAppStartup = 13
        }

        internal enum Level : byte
        {
            error = 2,
            fatal = 1,
            normal = 4,
            verbose = 5,
            warning = 3
        }

        internal enum NavigationEvent : byte
        {
            NavigationAsyncWorkItem = 11,
            NavigationContentRendered = 15,
            NavigationEnd = 14,
            NavigationLaunchBrowser = 13,
            NavigationPageFunctionReturn = 0x10,
            NavigationStart = 10,
            NavigationWebResponseReceived = 12
        }

        internal enum SplashScreenEvent : byte
        {
            CreateWindow = 12,
            DestroyWindow = 13,
            LoadAndDecodeImageEnd = 11,
            LoadAndDecodeImageStart = 10
        }
    }
}

