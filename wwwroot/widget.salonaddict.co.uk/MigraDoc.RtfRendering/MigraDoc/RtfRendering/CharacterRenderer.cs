namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class CharacterRenderer : RendererBase
    {
        private Character character;

        internal CharacterRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.character = domObj as Character;
        }

        internal override void Render()
        {
            if (this.character.Char != '\0')
            {
                base.rtfWriter.WriteHex(this.character.Char);
            }
            else
            {
                int num = this.character.IsNull("Count") ? 1 : this.character.Count;
                SymbolName symbolName = this.character.SymbolName;
                if (symbolName <= ((SymbolName) (-234881023)))
                {
                    switch (symbolName)
                    {
                        case ((SymbolName) (-251658239)):
                            for (int i = 0; i < num; i++)
                            {
                                base.rtfWriter.WriteBlank();
                            }
                            return;

                        case ((SymbolName) (-251658238)):
                            for (int j = 0; j < num; j++)
                            {
                                base.rtfWriter.WriteControl("u", "8194");
                                base.rtfWriter.WriteHex(0x20);
                            }
                            return;

                        case ((SymbolName) (-251658237)):
                            for (int k = 0; k < num; k++)
                            {
                                base.rtfWriter.WriteControl("u", "8195");
                                base.rtfWriter.WriteHex(0x20);
                            }
                            return;

                        case ((SymbolName) (-251658236)):
                            for (int m = 0; m < num; m++)
                            {
                                base.rtfWriter.WriteControl("u", "8197");
                                base.rtfWriter.WriteHex(0x20);
                            }
                            return;

                        case ((SymbolName) (-234881023)):
                            for (int n = 0; n < num; n++)
                            {
                                base.rtfWriter.WriteControl("tab");
                            }
                            return;
                    }
                }
                else if (symbolName == ((SymbolName) (-201326591)))
                {
                    for (int num12 = 0; num12 < num; num12++)
                    {
                        base.rtfWriter.WriteControl("line");
                    }
                }
                else
                {
                    int num17;
                    switch (symbolName)
                    {
                        case ((SymbolName) (-134217727)):
                            for (int num10 = 0; num10 < num; num10++)
                            {
                                base.rtfWriter.WriteHex(0x80);
                            }
                            return;

                        case ((SymbolName) (-134217726)):
                            for (int num4 = 0; num4 < num; num4++)
                            {
                                base.rtfWriter.WriteHex(0xa9);
                            }
                            return;

                        case ((SymbolName) (-134217725)):
                            num17 = 0;
                            break;

                        case ((SymbolName) (-134217724)):
                            for (int num15 = 0; num15 < num; num15++)
                            {
                                base.rtfWriter.WriteHex(0xae);
                            }
                            return;

                        case ((SymbolName) (-134217723)):
                            for (int num3 = 0; num3 < num; num3++)
                            {
                                base.rtfWriter.WriteControl("bullet");
                            }
                            return;

                        case ((SymbolName) (-134217722)):
                            for (int num13 = 0; num13 < num; num13++)
                            {
                                base.rtfWriter.WriteHex(0xac);
                            }
                            return;

                        case ((SymbolName) (-134217721)):
                            for (int num8 = 0; num8 < num; num8++)
                            {
                                base.rtfWriter.WriteControl("emdash");
                            }
                            return;

                        case ((SymbolName) (-134217720)):
                            for (int num9 = 0; num9 < num; num9++)
                            {
                                base.rtfWriter.WriteControl("endash");
                            }
                            return;

                        case ((SymbolName) (-134217719)):
                            for (int num11 = 0; num11 < num; num11++)
                            {
                                base.rtfWriter.WriteControl("~");
                            }
                            return;

                        case ((SymbolName) (-201326585)):
                            for (int num14 = 0; num14 < num; num14++)
                            {
                                base.rtfWriter.WriteControl("par");
                            }
                            return;

                        default:
                            return;
                    }
                    while (num17 < num)
                    {
                        base.rtfWriter.WriteHex(0x99);
                        num17++;
                    }
                }
            }
        }
    }
}

