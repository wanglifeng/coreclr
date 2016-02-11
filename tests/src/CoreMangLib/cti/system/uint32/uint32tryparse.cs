using System;
using System.Globalization;

// CoreCLR Port from Co7528TryParse_all.cs
// Tests UInt32.TryParse(String), UInt32.TryParse(String, NumberStyles, IFormatProvider, ref UInt32)
// 2003/04/01  KatyK
// 2007/06/28  adapted by MarielY

public class Co7528TryParse
{
    static bool verbose = false;

    public static int Main()
    {
        bool passed = true;

        try
        {
            // Make the test culture independent
            TestLibrary.Utilities.CurrentCulture = CultureInfo.InvariantCulture;

            // Set up NFIs to use
            NumberFormatInfo goodNFI = new NumberFormatInfo();

            NumberFormatInfo corruptNFI = new NumberFormatInfo(); // DecimalSeparator == GroupSeparator 
            corruptNFI.NumberDecimalSeparator = ".";
            corruptNFI.NumberGroupSeparator = ".";
            corruptNFI.CurrencyDecimalSeparator = ".";
            corruptNFI.CurrencyGroupSeparator = ".";
            corruptNFI.CurrencySymbol = "$";

            NumberFormatInfo swappedNFI = new NumberFormatInfo(); // DecimalSeparator & GroupSeparator swapped
            swappedNFI.NumberDecimalSeparator = ".";
            swappedNFI.NumberGroupSeparator = ",";
            swappedNFI.CurrencyDecimalSeparator = ",";
            swappedNFI.CurrencyGroupSeparator = ".";
            swappedNFI.CurrencySymbol = "$";

            NumberFormatInfo distinctNFI = new NumberFormatInfo(); // DecimalSeparator & GroupSeparator distinct
            distinctNFI.NumberDecimalSeparator = ".";
            distinctNFI.NumberGroupSeparator = ",";
            distinctNFI.CurrencyDecimalSeparator = ":";
            distinctNFI.CurrencyGroupSeparator = ";";
            distinctNFI.CurrencySymbol = "$";

            NumberFormatInfo customNFI = new NumberFormatInfo();
            customNFI.NegativeSign = "^";

            NumberFormatInfo ambigNFI = new NumberFormatInfo();
            ambigNFI.NegativeSign = "^";
            ambigNFI.CurrencySymbol = "^";

            CultureInfo germanCulture = new CultureInfo("de-DE");
			CultureInfo japaneseCulture;

			try
			{
				japaneseCulture = new CultureInfo("ja-JP");
			}
			catch (Exception)
			{
				TestLibrary.Logging.WriteLine("East Asian Languages are not installed. Skiping Japanese culture tests.");
				japaneseCulture = null;
			}

            // Parse tests included for comparison/regression
            passed &= VerifyUInt32Parse("5", 5);
            passed &= VerifyUInt32Parse("5  ", 5);
            passed &= VerifyUInt32Parse("5\0", 5);
            passed &= VerifyUInt32Parse("893382737", 893382737);
            passed &= VerifyUInt32Parse("1234567891", 1234567891);
            passed &= VerifyUInt32Parse("3123456789", 3123456789u);
            passed &= VerifyUInt32Parse("5", NumberStyles.Integer, CultureInfo.InvariantCulture, 5);
            passed &= VerifyUInt32Parse("5  \0", NumberStyles.Integer, CultureInfo.InvariantCulture, 5);
            passed &= VerifyUInt32Parse("5\0\0\0", NumberStyles.Integer, CultureInfo.InvariantCulture, 5);
            passed &= VerifyUInt32Parse("12", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0x12);
            passed &= VerifyUInt32Parse("fF", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0xFF);
            passed &= VerifyUInt32Parse("5", NumberStyles.Integer, goodNFI, 5);
            passed &= VerifyUInt32Parse("5.0", NumberStyles.AllowDecimalPoint, goodNFI, 5);
            passed &= VerifyUInt32Parse("123", NumberStyles.Integer, germanCulture, 123);
            passed &= VerifyUInt32Parse("123", NumberStyles.Integer, japaneseCulture, 123);
            passed &= VerifyUInt32Parse("123.000", NumberStyles.Any, germanCulture, 123000);
            passed &= VerifyUInt32Parse("123,000", NumberStyles.Any, japaneseCulture, 123000);
            passed &= VerifyUInt32Parse("123,000", NumberStyles.AllowDecimalPoint, germanCulture, 123);
            passed &= VerifyUInt32Parse("123.000", NumberStyles.AllowDecimalPoint, japaneseCulture, 123);
            passed &= VerifyUInt32Parse("5,00 " + germanCulture.NumberFormat.CurrencySymbol, NumberStyles.Any, germanCulture, 5); // currency
            //
            passed &= VerifyUInt32Parse("5", NumberStyles.Integer, corruptNFI, 5);
            passed &= VerifyUInt32Parse("5", NumberStyles.Number, corruptNFI, 5);
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Number, corruptNFI, 5);
            passed &= VerifyUInt32ParseException("5,0", NumberStyles.Number, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5.0.0", NumberStyles.Number, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("$5.0", NumberStyles.Currency, corruptNFI, 5);
            passed &= VerifyUInt32ParseException("$5,0", NumberStyles.Currency, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("$5.0.0", NumberStyles.Currency, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Currency, corruptNFI, 5);
            passed &= VerifyUInt32ParseException("5,0", NumberStyles.Currency, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5.0.0", NumberStyles.Currency, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Any, corruptNFI, 5);
            passed &= VerifyUInt32ParseException("5,0", NumberStyles.Any, corruptNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5.0.0", NumberStyles.Any, corruptNFI, typeof(FormatException));
            //
            passed &= VerifyUInt32Parse("5", NumberStyles.Integer, swappedNFI, 5);
            passed &= VerifyUInt32ParseException("1,234", NumberStyles.Integer, swappedNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("5", NumberStyles.Number, swappedNFI, 5);
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Number, swappedNFI, 5);
            passed &= VerifyUInt32Parse("1,234", NumberStyles.Number, swappedNFI, 1234);
            passed &= VerifyUInt32Parse("1,234.0", NumberStyles.Number, swappedNFI, 1234);
            passed &= VerifyUInt32ParseException("5.000.000", NumberStyles.Number, swappedNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5.000,00", NumberStyles.Number, swappedNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("5.000", NumberStyles.Currency, swappedNFI, 5); //???
            passed &= VerifyUInt32ParseException("5.000,00", NumberStyles.Currency, swappedNFI, typeof(FormatException)); //???
            passed &= VerifyUInt32Parse("$5.000", NumberStyles.Currency, swappedNFI, 5000);
            passed &= VerifyUInt32Parse("$5.000,00", NumberStyles.Currency, swappedNFI, 5000);
            passed &= VerifyUInt32Parse("5.000", NumberStyles.Any, swappedNFI, 5);  //?
            passed &= VerifyUInt32ParseException("5.000,00", NumberStyles.Any, swappedNFI, typeof(FormatException));  //?
            passed &= VerifyUInt32Parse("$5.000", NumberStyles.Any, swappedNFI, 5000);
            passed &= VerifyUInt32Parse("$5.000,00", NumberStyles.Any, swappedNFI, 5000);
            passed &= VerifyUInt32Parse("5,0", NumberStyles.Currency, swappedNFI, 5);
            passed &= VerifyUInt32Parse("$5,0", NumberStyles.Currency, swappedNFI, 5);
            passed &= VerifyUInt32Parse("5,000", NumberStyles.Currency, swappedNFI, 5);
            passed &= VerifyUInt32Parse("$5,000", NumberStyles.Currency, swappedNFI, 5);
            passed &= VerifyUInt32ParseException("5,000.0", NumberStyles.Currency, swappedNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("$5,000.0", NumberStyles.Currency, swappedNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("5,000", NumberStyles.Any, swappedNFI, 5);
            passed &= VerifyUInt32Parse("$5,000", NumberStyles.Any, swappedNFI, 5);
            passed &= VerifyUInt32ParseException("5,000.0", NumberStyles.Any, swappedNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("$5,000.0", NumberStyles.Any, swappedNFI, typeof(FormatException));
            //
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Number, distinctNFI, 5);
            passed &= VerifyUInt32Parse("1,234.0", NumberStyles.Number, distinctNFI, 1234);
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Currency, distinctNFI, 5);
            passed &= VerifyUInt32Parse("1,234.0", NumberStyles.Currency, distinctNFI, 1234);
            passed &= VerifyUInt32Parse("5.0", NumberStyles.Any, distinctNFI, 5);
            passed &= VerifyUInt32Parse("1,234.0", NumberStyles.Any, distinctNFI, 1234);
            passed &= VerifyUInt32ParseException("$5.0", NumberStyles.Currency, distinctNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("$5.0", NumberStyles.Any, distinctNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5:0", NumberStyles.Number, distinctNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5;0", NumberStyles.Number, distinctNFI, typeof(FormatException));
            passed &= VerifyUInt32ParseException("$5:0", NumberStyles.Number, distinctNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("5:0", NumberStyles.Currency, distinctNFI, 5);
            passed &= VerifyUInt32Parse("5:000", NumberStyles.Currency, distinctNFI, 5);
            passed &= VerifyUInt32Parse("5;000", NumberStyles.Currency, distinctNFI, 5000);
            passed &= VerifyUInt32Parse("$5:0", NumberStyles.Currency, distinctNFI, 5);
            passed &= VerifyUInt32Parse("$5;0", NumberStyles.Currency, distinctNFI, 50);
            passed &= VerifyUInt32Parse("5:0", NumberStyles.Any, distinctNFI, 5);
            passed &= VerifyUInt32Parse("5;0", NumberStyles.Any, distinctNFI, 50);
            passed &= VerifyUInt32Parse("$5:0", NumberStyles.Any, distinctNFI, 5);
            passed &= VerifyUInt32Parse("$5;0", NumberStyles.Any, distinctNFI, 50);
            passed &= VerifyUInt32ParseException("123,456;789.0", NumberStyles.Number, distinctNFI, typeof(FormatException));
            passed &= VerifyUInt32Parse("123,456;789.0", NumberStyles.Currency, distinctNFI, 123456789);
            passed &= VerifyUInt32Parse("123,456;789.0", NumberStyles.Any, distinctNFI, 123456789);
            passed &= VerifyUInt32ParseException("$123,456;789.0", NumberStyles.Any, distinctNFI, typeof(FormatException));
            //
            passed &= VerifyUInt32ParseException("12345678912", typeof(OverflowException));
            passed &= VerifyUInt32ParseException("5.3", NumberStyles.AllowDecimalPoint, goodNFI, typeof(OverflowException));  //weird that it's Overflow, but consistent with v1
            passed &= VerifyUInt32ParseException("Garbage", typeof(FormatException));
            passed &= VerifyUInt32ParseException("5\0Garbage", typeof(FormatException));
            passed &= VerifyUInt32ParseException(null, typeof(ArgumentNullException));
            passed &= VerifyUInt32ParseException("893382737", NumberStyles.HexNumber, CultureInfo.InvariantCulture, typeof(OverflowException));
            passed &= VerifyUInt32ParseException("5", NumberStyles.AllowHexSpecifier | NumberStyles.AllowParentheses, null, typeof(ArgumentException));
            passed &= VerifyUInt32ParseException("4", (NumberStyles)(-1), typeof(ArgumentException));
            passed &= VerifyUInt32ParseException("4", (NumberStyles)0x10000, typeof(ArgumentException));
            passed &= VerifyUInt32ParseException("4", (NumberStyles)(-1), CultureInfo.InvariantCulture, typeof(ArgumentException));
            passed &= VerifyUInt32ParseException("4", (NumberStyles)0x10000, CultureInfo.InvariantCulture, typeof(ArgumentException));
            passed &= VerifyUInt32ParseException("123.000.000.000", NumberStyles.Any, germanCulture, typeof(OverflowException));
            passed &= VerifyUInt32ParseException("123,000,000,000", NumberStyles.Any, japaneseCulture, typeof(OverflowException));
            passed &= VerifyUInt32ParseException("123,000,000,000", NumberStyles.Integer, germanCulture, typeof(FormatException));
            passed &= VerifyUInt32ParseException("123.000.000.000", NumberStyles.Integer, japaneseCulture, typeof(FormatException));
            passed &= VerifyUInt32ParseException("5,00 " + germanCulture.NumberFormat.CurrencySymbol, NumberStyles.Integer, germanCulture, typeof(FormatException)); // currency

            /////////// TryParse(String)
            //// Pass cases
            passed &= VerifyUInt32TryParse("5", 5, true);
            passed &= VerifyUInt32TryParse(" 5 ", 5, true);
            passed &= VerifyUInt32TryParse("5\0", 5, true);
            passed &= VerifyUInt32TryParse("5  \0", 5, true);
            passed &= VerifyUInt32TryParse("5\0\0\0", 5, true);
            passed &= VerifyUInt32TryParse("10000", 10000, true);
            passed &= VerifyUInt32TryParse("893382737", 893382737, true);
            passed &= VerifyUInt32TryParse("1234567891", 1234567891, true);
            passed &= VerifyUInt32TryParse("3123456789", 3123456789u, true);
            passed &= VerifyUInt32TryParse(UInt32.MaxValue.ToString(), UInt32.MaxValue, true);
            passed &= VerifyUInt32TryParse(UInt32.MinValue.ToString(), UInt32.MinValue, true);

            //// Fail cases
            passed &= VerifyUInt32TryParse(null, 0, false);
            passed &= VerifyUInt32TryParse("", 0, false);
            passed &= VerifyUInt32TryParse("Garbage", 0, false);
            passed &= VerifyUInt32TryParse("5\0Garbage", 0, false);
            passed &= VerifyUInt32TryParse("-5", 0, false);
            passed &= VerifyUInt32TryParse("12345678912", 0, false);
            passed &= VerifyUInt32TryParse("FF", 0, false);
            passed &= VerifyUInt32TryParse("27.3", 0, false);
            passed &= VerifyUInt32TryParse("23 5", 0, false);


            /////////// TryParse(TryParse(String, NumberStyles, IFormatProvider, ref UInt32)
            //// Pass cases
            passed &= VerifyUInt32TryParse("5", NumberStyles.Integer, CultureInfo.InvariantCulture, 5, true);
            // Variations on NumberStyles
            passed &= VerifyUInt32TryParse("12", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0x12, true);
            passed &= VerifyUInt32TryParse("FF", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0xFF, true);
            passed &= VerifyUInt32TryParse("fF", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0xFF, true);
            passed &= VerifyUInt32TryParse("fFFFFFFF", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0xFFFFFFFF, true);
            passed &= VerifyUInt32TryParse("   5", NumberStyles.AllowLeadingWhite, goodNFI, 5, true);
            passed &= VerifyUInt32TryParse("5", NumberStyles.Number, goodNFI, 5, true);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.AllowDecimalPoint, goodNFI, 5, true);
            // Variations on IFP
            passed &= VerifyUInt32TryParse("5", NumberStyles.Integer, goodNFI, 5, true);
            passed &= VerifyUInt32TryParse("5", NumberStyles.Integer, null, 5, true);
            passed &= VerifyUInt32TryParse("5", NumberStyles.Integer, new DateTimeFormatInfo(), 5, true);
            passed &= VerifyUInt32TryParse("123", NumberStyles.Integer, germanCulture, 123, true);
            passed &= VerifyUInt32TryParse("123", NumberStyles.Integer, japaneseCulture, 123, true);
            passed &= VerifyUInt32TryParse("123.000", NumberStyles.Any, germanCulture, 123000, true);
            passed &= VerifyUInt32TryParse("123,000", NumberStyles.Any, japaneseCulture, 123000, true);
            passed &= VerifyUInt32TryParse("123,000", NumberStyles.AllowDecimalPoint, germanCulture, 123, true);
            passed &= VerifyUInt32TryParse("123.000", NumberStyles.AllowDecimalPoint, japaneseCulture, 123, true);
            passed &= VerifyUInt32TryParse("5,00 " + germanCulture.NumberFormat.CurrencySymbol, NumberStyles.Any, germanCulture, 5, true); // currency

            //// Fail cases
            passed &= VerifyUInt32TryParse("-5", NumberStyles.Integer, CultureInfo.InvariantCulture, 0, false);
            passed &= VerifyUInt32TryParse("FF", NumberStyles.Integer, CultureInfo.InvariantCulture, 0, false);
            passed &= VerifyUInt32TryParse("893382737", NumberStyles.HexNumber, CultureInfo.InvariantCulture, 0, false);
            passed &= VerifyUInt32TryParse("^42", NumberStyles.Any, customNFI, 0, false);
            passed &= VerifyUInt32TryParse("-42", NumberStyles.Any, customNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.3", NumberStyles.AllowDecimalPoint, goodNFI, 0, false);
            passed &= VerifyUInt32TryParse("5   ", NumberStyles.AllowLeadingWhite, goodNFI, 0, false);
            passed &= VerifyUInt32TryParse("123.000.000.000", NumberStyles.Any, germanCulture, 0, false);
            passed &= VerifyUInt32TryParse("123,000,000,000", NumberStyles.Any, japaneseCulture, 0, false);
            passed &= VerifyUInt32TryParse("123,000,000,000", NumberStyles.Integer, germanCulture, 0, false);
            passed &= VerifyUInt32TryParse("123.000.000.000", NumberStyles.Integer, japaneseCulture, 0, false);
            passed &= VerifyUInt32TryParse("5,00 \u20AC", NumberStyles.Integer, germanCulture, 0, false); // currency

            //// Exception cases
            passed &= VerifyUInt32TryParseException("5", NumberStyles.AllowHexSpecifier | NumberStyles.AllowParentheses, null, typeof(ArgumentException));
            passed &= VerifyUInt32TryParseException("4", (NumberStyles)(-1), CultureInfo.InvariantCulture, typeof(ArgumentException));
            passed &= VerifyUInt32TryParseException("4", (NumberStyles)0x10000, CultureInfo.InvariantCulture, typeof(ArgumentException));

            // NumberStyles/NFI variations
            //
            passed &= VerifyUInt32TryParse("5", NumberStyles.Integer, corruptNFI, 5, true);
            passed &= VerifyUInt32TryParse("5", NumberStyles.Number, corruptNFI, 5, true);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Number, corruptNFI, 5, true);
            passed &= VerifyUInt32TryParse("5,0", NumberStyles.Number, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.0.0", NumberStyles.Number, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("$5.0", NumberStyles.Currency, corruptNFI, 5, true);
            passed &= VerifyUInt32TryParse("$5,0", NumberStyles.Currency, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("$5.0.0", NumberStyles.Currency, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Currency, corruptNFI, 5, true);
            passed &= VerifyUInt32TryParse("5,0", NumberStyles.Currency, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.0.0", NumberStyles.Currency, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Any, corruptNFI, 5, true);
            passed &= VerifyUInt32TryParse("5,0", NumberStyles.Any, corruptNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.0.0", NumberStyles.Any, corruptNFI, 0, false);
            //
            passed &= VerifyUInt32TryParse("5", NumberStyles.Integer, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("1,234", NumberStyles.Integer, swappedNFI, 0, false);
            passed &= VerifyUInt32TryParse("5", NumberStyles.Number, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Number, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("1,234", NumberStyles.Number, swappedNFI, 1234, true);
            passed &= VerifyUInt32TryParse("1,234.0", NumberStyles.Number, swappedNFI, 1234, true);
            passed &= VerifyUInt32TryParse("5.000.000", NumberStyles.Number, swappedNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.000,00", NumberStyles.Number, swappedNFI, 0, false);
            passed &= VerifyUInt32TryParse("5.000", NumberStyles.Currency, swappedNFI, 5, true); //???
            passed &= VerifyUInt32TryParse("5.000,00", NumberStyles.Currency, swappedNFI, 0, false); //???
            passed &= VerifyUInt32TryParse("$5.000", NumberStyles.Currency, swappedNFI, 5000, true);
            passed &= VerifyUInt32TryParse("$5.000,00", NumberStyles.Currency, swappedNFI, 5000, true);
            passed &= VerifyUInt32TryParse("5.000", NumberStyles.Any, swappedNFI, 5, true);  //?
            passed &= VerifyUInt32TryParse("5.000,00", NumberStyles.Any, swappedNFI, 0, false);  //?
            passed &= VerifyUInt32TryParse("$5.000", NumberStyles.Any, swappedNFI, 5000, true);
            passed &= VerifyUInt32TryParse("$5.000,00", NumberStyles.Any, swappedNFI, 5000, true);
            passed &= VerifyUInt32TryParse("5,0", NumberStyles.Currency, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("$5,0", NumberStyles.Currency, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("5,000", NumberStyles.Currency, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("$5,000", NumberStyles.Currency, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("5,000.0", NumberStyles.Currency, swappedNFI, 0, false);
            passed &= VerifyUInt32TryParse("$5,000.0", NumberStyles.Currency, swappedNFI, 0, false);
            passed &= VerifyUInt32TryParse("5,000", NumberStyles.Any, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("$5,000", NumberStyles.Any, swappedNFI, 5, true);
            passed &= VerifyUInt32TryParse("5,000.0", NumberStyles.Any, swappedNFI, 0, false);
            passed &= VerifyUInt32TryParse("$5,000.0", NumberStyles.Any, swappedNFI, 0, false);
            //
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Number, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("1,234.0", NumberStyles.Number, distinctNFI, 1234, true);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Currency, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("1,234.0", NumberStyles.Currency, distinctNFI, 1234, true);
            passed &= VerifyUInt32TryParse("5.0", NumberStyles.Any, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("1,234.0", NumberStyles.Any, distinctNFI, 1234, true);
            passed &= VerifyUInt32TryParse("$5.0", NumberStyles.Currency, distinctNFI, 0, false);
            passed &= VerifyUInt32TryParse("$5.0", NumberStyles.Any, distinctNFI, 0, false);
            passed &= VerifyUInt32TryParse("5:0", NumberStyles.Number, distinctNFI, 0, false);
            passed &= VerifyUInt32TryParse("5;0", NumberStyles.Number, distinctNFI, 0, false);
            passed &= VerifyUInt32TryParse("$5:0", NumberStyles.Number, distinctNFI, 0, false);
            passed &= VerifyUInt32TryParse("5:0", NumberStyles.Currency, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("5:000", NumberStyles.Currency, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("5;000", NumberStyles.Currency, distinctNFI, 5000, true);
            passed &= VerifyUInt32TryParse("$5:0", NumberStyles.Currency, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("$5;0", NumberStyles.Currency, distinctNFI, 50, true);
            passed &= VerifyUInt32TryParse("5:0", NumberStyles.Any, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("5;0", NumberStyles.Any, distinctNFI, 50, true);
            passed &= VerifyUInt32TryParse("$5:0", NumberStyles.Any, distinctNFI, 5, true);
            passed &= VerifyUInt32TryParse("$5;0", NumberStyles.Any, distinctNFI, 50, true);
            passed &= VerifyUInt32TryParse("123,456;789.0", NumberStyles.Number, distinctNFI, 0, false);
            passed &= VerifyUInt32TryParse("123,456;789.0", NumberStyles.Currency, distinctNFI, 123456789, true);
            passed &= VerifyUInt32TryParse("123,456;789.0", NumberStyles.Any, distinctNFI, 123456789, true);
            passed &= VerifyUInt32TryParse("$123,456;789.0", NumberStyles.Any, distinctNFI, 0, false);


            // Should these pass or fail?  Current parse behavior is to pass, so they might be
            // parse bugs, but they're not tryparse bugs.
            passed &= VerifyUInt32Parse("5", NumberStyles.Float, goodNFI, 5);
            passed &= VerifyUInt32TryParse("5", NumberStyles.Float, goodNFI, 5, true);
            passed &= VerifyUInt32Parse("5", NumberStyles.AllowDecimalPoint, goodNFI, 5);
            passed &= VerifyUInt32TryParse("5", NumberStyles.AllowDecimalPoint, goodNFI, 5, true);
            // I expect ArgumentException with an ambiguous NFI
            passed &= VerifyUInt32ParseException("^42", NumberStyles.Any, ambigNFI, typeof(OverflowException));
            passed &= VerifyUInt32TryParse("^42", NumberStyles.Any, ambigNFI, 0, false);

            ///  END TEST CASES
        }
        catch (Exception e)
        {
            TestLibrary.Logging.WriteLine("Unexpected exception!!  " + e.ToString());
            passed = false;
        }

        if (passed)
        {
            TestLibrary.Logging.WriteLine("paSs");
            return 100;
        }
        else
        {
            TestLibrary.Logging.WriteLine("FAiL");
            return 1;
        }
    }

    public static bool VerifyUInt32TryParse(string value, UInt32 expectedResult, bool expectedReturn)
    {
        if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.TryParse, Value = '{0}', Expected Result = {1}, Expected Return = {2}",
                value, expectedResult, expectedReturn);
        }
        UInt32 result = 0;
        try
        {
            bool returnValue = UInt32.TryParse(value, out result);
            if (returnValue != expectedReturn)
            {
                TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Return: {1}, Actual Return: {2}", value, expectedReturn, returnValue);
                return false;
            }
            if (result != expectedResult)
            {
                TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Return: {1}, Actual Return: {2}", value, expectedResult, result);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            TestLibrary.Logging.WriteLine("FAILURE: Unexpected Exception, Value = '{0}', Exception: {1}", value, ex);
            return false;
        }
    }

    public static bool VerifyUInt32TryParse(string value, NumberStyles style, IFormatProvider provider, UInt32 expectedResult, bool expectedReturn)
    {
		if (provider == null) return true;

		if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.TryParse, Value = '{0}', Style = {1}, Provider = {2}, Expected Result = {3}, Expected Return = {4}",
                  value, style, provider, expectedResult, expectedReturn);
        }
        UInt32 result = 0;
        try
        {
            bool returnValue = UInt32.TryParse(value, style, provider, out result);
            if (returnValue != expectedReturn)
            {
                TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Style = {1}, Provider = {2}, Expected Return = {3}, Actual Return = {4}",
                  value, style, provider, expectedReturn, returnValue);
                return false;
            }
            if (result != expectedResult)
            {
                TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Return: {1}, Actual Return: {2}", value, expectedResult, result);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            TestLibrary.Logging.WriteLine("FAILURE: Unexpected Exception, Value = '{0}', Exception: {1}", value, ex);
            return false;
        }
    }

    public static bool VerifyUInt32TryParseException(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
    {
		if (provider == null) return true;

		if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.TryParse, Value = '{0}', Style = {1}, Provider = {2}, Expected Exception = {3}",
                value, style, provider, exceptionType);
        }
        try
        {
            UInt32 result = 0;
            Boolean returnValue = UInt32.TryParse(value, style, provider, out result);
            TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Exception: {1}", value, exceptionType);
            return false;
        }
        catch (Exception ex)
        {
            if (!ex.GetType().IsAssignableFrom(exceptionType))
            {
                TestLibrary.Logging.WriteLine("FAILURE: Wrong Exception Type, Value = '{0}', Exception Type: {1} Expected Type: {2}", value, ex.GetType(), exceptionType);
                return false;
            }
            return true;
        }
    }

    public static bool VerifyUInt32Parse(string value, UInt32 expectedResult)
    {
        if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.Parse, Value = '{0}', Expected Result, {1}",
                value, expectedResult);
        }
        try
        {
            UInt32 returnValue = UInt32.Parse(value);
            if (returnValue != expectedResult)
            {
                TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Return: {1}, Actual Return: {2}", value, expectedResult, returnValue);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            TestLibrary.Logging.WriteLine("FAILURE: Unexpected Exception, Value = '{0}', Exception: {1}", value, ex);
            return false;
        }
    }

    public static bool VerifyUInt32Parse(string value, NumberStyles style, IFormatProvider provider, UInt32 expectedResult)
    {
		if (provider == null) return true;

		if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.Parse, Value = '{0}', Style = {1}, provider = {2}, Expected Result = {3}",
                value, style, provider, expectedResult);
        }
        try
        {
            UInt32 returnValue = UInt32.Parse(value, style, provider);
            if (returnValue != expectedResult)
            {
                TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Return: {1}, Actual Return: {2}", value, expectedResult, returnValue);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            TestLibrary.Logging.WriteLine("FAILURE: Unexpected Exception, Value = '{0}', Exception: {1}", value, ex);
            return false;
        }
    }

    public static bool VerifyUInt32ParseException(string value, Type exceptionType)
    {
        if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.Parse, Value = '{0}', Expected Exception, {1}",
                value, exceptionType);
        }
        try
        {
            UInt32 returnValue = UInt32.Parse(value);
            TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Exception: {1}", value, exceptionType);
            return false;
        }
        catch (Exception ex)
        {
            if (!ex.GetType().IsAssignableFrom(exceptionType))
            {
                TestLibrary.Logging.WriteLine("FAILURE: Wrong Exception Type, Value = '{0}', Exception Type: {1} Expected Type: {2}", value, ex.GetType(), exceptionType);
                return false;
            }
            return true;
        }
    }

    public static bool VerifyUInt32ParseException(string value, NumberStyles style, Type exceptionType)
    {
        if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.Parse, Value = '{0}', Style = {1}, Expected Exception = {3}",
                value, style, exceptionType);
        }
        try
        {
            UInt32 returnValue = UInt32.Parse(value, style);
            TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Exception: {1}", value, exceptionType);
            return false;
        }
        catch (Exception ex)
        {
            if (!ex.GetType().IsAssignableFrom(exceptionType))
            {
                TestLibrary.Logging.WriteLine("FAILURE: Wrong Exception Type, Value = '{0}', Exception Type: {1} Expected Type: {2}", value, ex.GetType(), exceptionType);
                return false;
            }
            return true;
        }
    }

    public static bool VerifyUInt32ParseException(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
    {
		if (provider == null) return true;

		if (verbose)
        {
            TestLibrary.Logging.WriteLine("Test: UInt32.Parse, Value = '{0}', Style = {1}, Provider = {2}, Expected Exception = {3}",
                value, style, provider, exceptionType);
        }
        try
        {
            UInt32 returnValue = UInt32.Parse(value, style, provider);
            TestLibrary.Logging.WriteLine("FAILURE: Value = '{0}', Expected Exception: {1}", value, exceptionType);
            return false;
        }
        catch (Exception ex)
        {
            if (!ex.GetType().IsAssignableFrom(exceptionType))
            {
                TestLibrary.Logging.WriteLine("FAILURE: Wrong Exception Type, Value = '{0}', Exception Type: {1} Expected Type: {2}", value, ex.GetType(), exceptionType);
                return false;
            }
            return true;
        }
    }
}
