namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp;
    using System;

    internal class HorizontalMetricsTable : OpenTypeFontTable
    {
        public short[] leftSideBearing;
        public HorizontalMetrics[] metrics;
        public const string Tag = "hmtx";

        public HorizontalMetricsTable(FontData fontData) : base(fontData, "hmtx")
        {
            this.Read();
        }

        public void Read()
        {
            try
            {
                HorizontalHeaderTable hhea = base.fontData.hhea;
                MaximumProfileTable maxp = base.fontData.maxp;
                if ((hhea != null) && (maxp != null))
                {
                    int numberOfHMetrics = hhea.numberOfHMetrics;
                    int num2 = maxp.numGlyphs - numberOfHMetrics;
                    this.metrics = new HorizontalMetrics[numberOfHMetrics];
                    for (int i = 0; i < numberOfHMetrics; i++)
                    {
                        this.metrics[i] = new HorizontalMetrics(base.fontData);
                    }
                    if (num2 > 0)
                    {
                        this.leftSideBearing = new short[num2];
                        for (int j = 0; j < num2; j++)
                        {
                            this.leftSideBearing[j] = base.fontData.ReadFWord();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new PdfSharpException(PSSR.ErrorReadingFontData, exception);
            }
        }
    }
}

