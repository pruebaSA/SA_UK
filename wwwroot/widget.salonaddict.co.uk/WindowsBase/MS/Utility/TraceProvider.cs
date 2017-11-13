namespace MS.Utility
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using MS.Win32;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;

    [Guid("748004CA-4959-409a-887C-6546438CF48E"), FriendAccessAllowed]
    internal class TraceProvider
    {
        private bool _enabled = false;
        private UnsafeNativeMethods.EtwTrace.EtwProc _etwProc;
        private uint _flags = 0;
        private byte _level = 0;
        private SecurityCriticalDataForSet<ulong> _registrationHandle = new SecurityCriticalDataForSet<ulong>(0L);
        private ulong _traceHandle = 0L;
        private const ushort _version = 2;

        [SecurityCritical]
        internal TraceProvider(string _applicationName, Guid controlGuid)
        {
            this.Register(controlGuid);
        }

        [SecurityCritical]
        private unsafe string EncodeObject(object data, MofField* mofField, char* ptr, ref uint offSet, byte* ptrArgInfo)
        {
            if (data == null)
            {
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 0;
                    *((short*) (ptrArgInfo + 1)) = 0;
                }
                mofField.DataLength = 0;
                mofField.DataPointer = null;
                return null;
            }
            Type enumType = data.GetType();
            if (enumType.IsEnum)
            {
                data = Convert.ChangeType(data, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture);
            }
            string str = data as string;
            if (str != null)
            {
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 2;
                    *((short*) (ptrArgInfo + 1)) = (str.Length < 0xffff) ? ((short) ((ushort) str.Length)) : ((short) ((ushort) 0xffff));
                    return str;
                }
                mofField.DataLength = 2;
                ushort* numPtr = (ushort*) ptr;
                numPtr[0] = ((str.Length * 2) < 0xffff) ? ((ushort) (str.Length * 2)) : ((ushort) 0xffff);
                mofField.DataPointer = (void*) numPtr;
                offSet += 2;
                return str;
            }
            if (data is sbyte)
            {
                mofField.DataLength = 1;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 3;
                }
                sbyte* numPtr2 = (sbyte*) ptr;
                numPtr2[0] = (sbyte) data;
                mofField.DataPointer = (void*) numPtr2;
                offSet++;
            }
            else if (data is byte)
            {
                mofField.DataLength = 1;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 4;
                }
                byte* numPtr3 = (byte*) ptr;
                numPtr3[0] = (byte) data;
                mofField.DataPointer = (void*) numPtr3;
                offSet++;
            }
            else if (data is short)
            {
                mofField.DataLength = 2;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 5;
                }
                short* numPtr4 = (short*) ptr;
                numPtr4[0] = (short) data;
                mofField.DataPointer = (void*) numPtr4;
                offSet += 2;
            }
            else if (data is ushort)
            {
                mofField.DataLength = 2;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 6;
                }
                ushort* numPtr5 = (ushort*) ptr;
                numPtr5[0] = (ushort) data;
                mofField.DataPointer = (void*) numPtr5;
                offSet += 2;
            }
            else if (data is int)
            {
                mofField.DataLength = 4;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 7;
                }
                int* numPtr6 = (int*) ptr;
                numPtr6[0] = (int) data;
                mofField.DataPointer = (void*) numPtr6;
                offSet += 4;
            }
            else if (data is uint)
            {
                mofField.DataLength = 4;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 8;
                }
                uint* numPtr7 = (uint*) ptr;
                numPtr7[0] = (uint) data;
                mofField.DataPointer = (void*) numPtr7;
                offSet += 4;
            }
            else if (data is long)
            {
                mofField.DataLength = 8;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 9;
                }
                long* numPtr8 = (long*) ptr;
                numPtr8[0] = (long) data;
                mofField.DataPointer = (void*) numPtr8;
                offSet += 8;
            }
            else if (data is ulong)
            {
                mofField.DataLength = 8;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 10;
                }
                ulong* numPtr9 = (ulong*) ptr;
                numPtr9[0] = (ulong) data;
                mofField.DataPointer = (void*) numPtr9;
                offSet += 8;
            }
            else if (data is char)
            {
                mofField.DataLength = 2;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 11;
                }
                char* chPtr = ptr;
                chPtr[0] = (char) data;
                mofField.DataPointer = (void*) chPtr;
                offSet += 2;
            }
            else if (data is float)
            {
                mofField.DataLength = 4;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 12;
                }
                float* numPtr10 = (float*) ptr;
                numPtr10[0] = (float) data;
                mofField.DataPointer = (void*) numPtr10;
                offSet += 4;
            }
            else if (data is double)
            {
                mofField.DataLength = 8;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 13;
                }
                double* numPtr11 = (double*) ptr;
                numPtr11[0] = (double) data;
                mofField.DataPointer = (void*) numPtr11;
                offSet += 8;
            }
            else if (data is bool)
            {
                mofField.DataLength = 1;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 14;
                }
                bool* flagPtr = (bool*) ptr;
                *((sbyte*) flagPtr) = (bool) data;
                mofField.DataPointer = (void*) flagPtr;
                offSet++;
            }
            else if (data is decimal)
            {
                mofField.DataLength = 0x10;
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 15;
                }
                decimal* numPtr12 = (decimal*) ptr;
                numPtr12[0] = (decimal) data;
                mofField.DataPointer = (void*) numPtr12;
                offSet += 0x10;
            }
            else
            {
                str = data.ToString();
                if (ptrArgInfo != null)
                {
                    ptrArgInfo[0] = 2;
                    *((short*) (ptrArgInfo + 1)) = (str.Length < 0xffff) ? ((short) ((ushort) str.Length)) : ((short) ((ushort) 0xffff));
                    return str;
                }
                mofField.DataLength = 2;
                ushort* numPtr13 = (ushort*) ptr;
                numPtr13[0] = ((str.Length * 2) < 0xffff) ? ((ushort) (str.Length * 2)) : ((ushort) 0xffff);
                mofField.DataPointer = (void*) numPtr13;
                offSet += 2;
                return str;
            }
            if (ptrArgInfo != null)
            {
                *((short*) (ptrArgInfo + 1)) = (ushort) mofField.DataLength;
            }
            return str;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        ~TraceProvider()
        {
            UnsafeNativeMethods.EtwTrace.UnregisterTraceGuids(this._registrationHandle.Value);
            GC.KeepAlive(this._etwProc);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal unsafe uint MyCallback(uint requestCode, IntPtr context, IntPtr bufferSize, byte* byteBuffer)
        {
            try
            {
                BaseEvent* eventPtr = (BaseEvent*) byteBuffer;
                switch (requestCode)
                {
                    case 4:
                        this._traceHandle = eventPtr->HistoricalContext;
                        this._flags = (uint) UnsafeNativeMethods.EtwTrace.GetTraceEnableFlags(eventPtr->HistoricalContext);
                        this._level = UnsafeNativeMethods.EtwTrace.GetTraceEnableLevel(eventPtr->HistoricalContext);
                        this._enabled = true;
                        break;

                    case 5:
                        this._enabled = false;
                        this._traceHandle = 0L;
                        this._level = 0;
                        this._flags = 0;
                        break;

                    default:
                        this._enabled = false;
                        this._traceHandle = 0L;
                        break;
                }
                return 0;
            }
            catch (Exception exception)
            {
                if ((exception is NullReferenceException) || (exception is SEHException))
                {
                    throw;
                }
                return 0;
            }
        }

        [SecurityCritical]
        private unsafe string ProcessOneObject(object data, MofField* mofField, char* ptr, ref uint offSet) => 
            this.EncodeObject(data, mofField, ptr, ref offSet, null);

        [SecurityCritical]
        private unsafe uint Register(Guid controlGuid)
        {
            ulong num2;
            UnsafeNativeMethods.EtwTrace.TraceGuidRegistration guidReg = new UnsafeNativeMethods.EtwTrace.TraceGuidRegistration();
            Guid guid = new Guid(0xb4955bf0, 0x3af1, 0x4740, 180, 0x75, 0x99, 5, 0x5d, 0x3f, 0xe9, 170);
            this._etwProc = new UnsafeNativeMethods.EtwTrace.EtwProc(this.MyCallback);
            guidReg.Guid = &guid;
            guidReg.RegHandle = null;
            uint num = UnsafeNativeMethods.EtwTrace.RegisterTraceGuids(this._etwProc, null, ref controlGuid, 1, ref guidReg, null, null, out num2);
            this._registrationHandle.Value = num2;
            return num;
        }

        internal void TraceEvent(Guid eventGuid, byte eventType)
        {
            this.TraceEvent(MS.Utility.EventTrace.Level.normal, eventGuid, eventType, null, null);
        }

        internal void TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte eventType)
        {
            this.TraceEvent(level, eventGuid, eventType, null, null);
        }

        internal void TraceEvent(Guid eventGuid, byte eventType, object data0)
        {
            this.TraceEvent(MS.Utility.EventTrace.Level.normal, eventGuid, eventType, data0, null);
        }

        internal void TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte eventType, object data0)
        {
            this.TraceEvent(level, eventGuid, eventType, data0, null);
        }

        internal void TraceEvent(Guid eventGuid, byte eventType, object data0, object data1)
        {
            this.TraceEvent(MS.Utility.EventTrace.Level.normal, eventGuid, eventType, data0, data1);
        }

        internal void TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte eventType, object data0, object data1)
        {
            this.TraceEvent(level, eventGuid, eventType, data0, data1, null, null, null, null, null, null, null);
        }

        internal void TraceEvent(Guid eventGuid, byte eventType, object data0, object data1, object data2)
        {
            this.TraceEvent(MS.Utility.EventTrace.Level.normal, eventGuid, eventType, data0, data1, data2);
        }

        internal void TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte eventType, object data0, object data1, object data2)
        {
            this.TraceEvent(level, eventGuid, eventType, data0, data1, data2, null, null, null, null, null, null);
        }

        internal void TraceEvent(Guid eventGuid, byte eventType, object data0, object data1, object data2, object data3)
        {
            this.TraceEvent(MS.Utility.EventTrace.Level.normal, eventGuid, eventType, data0, data1, data2, data3);
        }

        internal void TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte eventType, object data0, object data1, object data2, object data3)
        {
            this.TraceEvent(level, eventGuid, eventType, data0, data1, data2, data3, null, null, null, null, null);
        }

        internal void TraceEvent(Guid eventGuid, byte eventType, object data0, object data1, object data2, object data3, object data4)
        {
            this.TraceEvent(MS.Utility.EventTrace.Level.normal, eventGuid, eventType, data0, data1, data2, data3, data4);
        }

        internal void TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte eventType, object data0, object data1, object data2, object data3, object data4)
        {
            this.TraceEvent(level, eventGuid, eventType, data0, data1, data2, data3, data4, null, null, null, null);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal unsafe uint TraceEvent(MS.Utility.EventTrace.Level level, Guid eventGuid, byte evtype, object data0, object data1, object data2, object data3, object data4, object data5, object data6, object data7, object data8)
        {
            BaseEvent event2;
            string str2;
            string str3;
            string str4;
            string str5;
            string str6;
            string str7;
            string str8;
            string str9;
            uint num = 0;
            char* chPtr = (char*) stackalloc byte[(2 * 0x90)];
            uint offSet = 0;
            char* ptr = chPtr;
            int num3 = 0;
            uint num4 = 0;
            int num5 = 0;
            string str = str2 = str3 = str4 = str5 = str6 = str7 = str8 = str9 = "";
            event2.ClientContext = 0;
            event2.Flags = 0x120000;
            event2.Guid = eventGuid;
            event2.EventType = evtype;
            event2.Level = level;
            event2.Version = 2;
            MofField* mofField = null;
            if (data0 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                str = this.ProcessOneObject(data0, mofField, ptr, ref offSet);
                if (str != null)
                {
                    num3 |= 1;
                    num5++;
                }
            }
            if (data1 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str2 = this.ProcessOneObject(data1, mofField, ptr, ref offSet);
                if (str2 != null)
                {
                    num3 |= 2;
                    num5++;
                }
            }
            if (data2 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str3 = this.ProcessOneObject(data2, mofField, ptr, ref offSet);
                if (str3 != null)
                {
                    num3 |= 4;
                    num5++;
                }
            }
            if (data3 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str4 = this.ProcessOneObject(data3, mofField, ptr, ref offSet);
                if (str4 != null)
                {
                    num3 |= 8;
                    num5++;
                }
            }
            if (data4 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str5 = this.ProcessOneObject(data4, mofField, ptr, ref offSet);
                if (str5 != null)
                {
                    num3 |= 0x10;
                    num5++;
                }
            }
            if (data5 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str6 = this.ProcessOneObject(data5, mofField, ptr, ref offSet);
                if (str6 != null)
                {
                    num3 |= 0x20;
                    num5++;
                }
            }
            if (data6 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str7 = this.ProcessOneObject(data6, mofField, ptr, ref offSet);
                if (str7 != null)
                {
                    num3 |= 0x40;
                    num5++;
                }
            }
            if (data7 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str8 = this.ProcessOneObject(data7, mofField, ptr, ref offSet);
                if (str8 != null)
                {
                    num3 |= 0x80;
                    num5++;
                }
            }
            if (data8 != null)
            {
                num4++;
                mofField = &event2.UserData + num5++;
                ptr = chPtr + ((char*) offSet);
                str9 = this.ProcessOneObject(data8, mofField, ptr, ref offSet);
                if (str9 != null)
                {
                    num3 |= 0x100;
                    num5++;
                }
            }
            Invariant.Assert(((long) ((ptr - chPtr) / 2)) <= 0x90L);
            fixed (char* str10 = ((char*) str))
            {
                char* chPtr3 = str10;
                fixed (char* str11 = ((char*) str2))
                {
                    char* chPtr4 = str11;
                    fixed (char* str12 = ((char*) str3))
                    {
                        char* chPtr5 = str12;
                        fixed (char* str13 = ((char*) str4))
                        {
                            char* chPtr6 = str13;
                            fixed (char* str14 = ((char*) str5))
                            {
                                char* chPtr7 = str14;
                                fixed (char* str15 = ((char*) str6))
                                {
                                    char* chPtr8 = str15;
                                    fixed (char* str16 = ((char*) str7))
                                    {
                                        char* chPtr9 = str16;
                                        fixed (char* str17 = ((char*) str8))
                                        {
                                            char* chPtr10 = str17;
                                            fixed (char* str18 = ((char*) str9))
                                            {
                                                char* chPtr11 = str18;
                                                int index = 0;
                                                if ((num3 & 1) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr3;
                                                }
                                                index++;
                                                if ((num3 & 2) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str2.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr4;
                                                }
                                                index++;
                                                if ((num3 & 4) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str3.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr5;
                                                }
                                                index++;
                                                if ((num3 & 8) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str4.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr6;
                                                }
                                                index++;
                                                if ((num3 & 0x10) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str5.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr7;
                                                }
                                                index++;
                                                if ((num3 & 0x20) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str6.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr8;
                                                }
                                                index++;
                                                if ((num3 & 0x40) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str7.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr9;
                                                }
                                                index++;
                                                if ((num3 & 0x80) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str8.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr10;
                                                }
                                                index++;
                                                if ((num3 & 0x100) != 0)
                                                {
                                                    index++;
                                                    &event2.UserData[index].DataLength = (uint) (str9.Length * 2);
                                                    &event2.UserData[index].DataPointer = (void*) chPtr11;
                                                }
                                                event2.BufferSize = (uint) (0x30 + (num5 * sizeof(MofField)));
                                                num = UnsafeNativeMethods.EtwTrace.TraceEvent(this._traceHandle, (char*) &event2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            str12 = null;
            str13 = null;
            str14 = null;
            str15 = null;
            str16 = null;
            str17 = null;
            str18 = null;
            return num;
        }

        internal uint Flags =>
            this._flags;

        internal bool IsEnabled =>
            this._enabled;

        internal byte Level =>
            this._level;

        [StructLayout(LayoutKind.Explicit, Size=0x130)]
        internal struct BaseEvent
        {
            [FieldOffset(0)]
            internal uint BufferSize;
            [FieldOffset(40)]
            internal uint ClientContext;
            [FieldOffset(4)]
            internal byte EventType;
            [FieldOffset(0x2c)]
            internal uint Flags;
            [FieldOffset(0x18)]
            internal System.Guid Guid;
            [FieldOffset(8)]
            internal ulong HistoricalContext;
            [FieldOffset(5)]
            internal MS.Utility.EventTrace.Level Level;
            [FieldOffset(0x10)]
            internal long TimeStamp;
            [FieldOffset(0x30)]
            internal TraceProvider.MofField UserData;
            [FieldOffset(6)]
            internal ushort Version;
        }

        [StructLayout(LayoutKind.Explicit, Size=0x10)]
        internal struct MofField
        {
            [FieldOffset(8)]
            internal uint DataLength;
            [FieldOffset(0)]
            internal unsafe void* DataPointer;
            [FieldOffset(12)]
            internal uint DataType;
        }

        internal sealed class RequestCodes
        {
            internal const uint DisableCollection = 7;
            internal const uint DisableEvents = 5;
            internal const uint EnableCollection = 6;
            internal const uint EnableEvents = 4;
            internal const uint ExecuteMethod = 9;
            internal const uint GetAllData = 0;
            internal const uint GetSingleInstance = 1;
            internal const uint RegInfo = 8;
            internal const uint SetSingleInstance = 2;
            internal const uint SetSingleItem = 3;

            private RequestCodes()
            {
            }
        }
    }
}

