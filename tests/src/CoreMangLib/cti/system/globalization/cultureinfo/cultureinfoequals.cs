using System;
using System.Globalization;
using TestLibrary;

/// <summary>
///Equals
/// </summary>
public class CultureInfoEquals
{
    public static int Main()
    {
        CultureInfoEquals CultureInfoEquals = new CultureInfoEquals();

        TestLibrary.TestFramework.BeginTestCase("CultureInfoEquals");
        if (CultureInfoEquals.RunTests())
        {
            TestLibrary.TestFramework.EndTestCase();
            TestLibrary.TestFramework.LogInformation("PASS");
            return 100;
        }
        else
        {
            TestLibrary.TestFramework.EndTestCase();
            TestLibrary.TestFramework.LogInformation("FAIL");
            return 0;
        }
    }

    public bool RunTests()
    {
        bool retVal = true;
        TestLibrary.TestFramework.LogInformation("[Positive]");
        retVal = PosTest1() && retVal;
        if (!Utilities.IsWindows)
        {
            // Neutral cultures not supported on Windows
            retVal = PosTest2() && retVal;
            retVal = PosTest3() && retVal;
        }
        retVal = PosTest4() && retVal;
        if (!Utilities.IsWindows)
        {
            // Neutral cultures not supported on Windows
            retVal = PosTest5() && retVal;
        }
        retVal = PosTest6() && retVal;
        retVal = PosTest7() && retVal;
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest1()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest1: Other CultureInfo comes from Clone method is the same CultureInfo as the original");
        try
        {

            CultureInfo myCultureInfo = new CultureInfo("fr-FR");
            CultureInfo myClone = myCultureInfo.Clone() as CultureInfo;
            if (!myClone.Equals(myCultureInfo))
            {
                TestLibrary.TestFramework.LogError("001", "Other CultureInfo comes from Clone method should be the same CultureInfo as the original.");
                retVal = false;
            }
        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("002", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest2()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest2: Other (CultureTypes.NeutralCultures)  CultureInfo comes from GetCultureInfo method with the same paramter is the same CultureInfo as the original");
        try
        {

            CultureInfo myCultureInfo = new CultureInfo("en");
            CultureInfo myCultureInfo1 = new CultureInfo("en");
            if (!myCultureInfo1.Equals(myCultureInfo))
            {
                TestLibrary.TestFramework.LogError("003", "Other CultureInfo comes from GetCultureInfo method with the same paramter should be the same CultureInfo as the original.");
                retVal = false;
            }

        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("004", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest3()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest3: Other (CultureTypes.NeutralCultures)  CultureInfo comes from constroctor with the same paramter is  the same CultureInfo as the original");
        try
        {

            CultureInfo myCultureInfo = new CultureInfo("en");
            CultureInfo myCultureInfo1 = new CultureInfo("en");
            if (!myCultureInfo1.Equals(myCultureInfo))
            {
                TestLibrary.TestFramework.LogError("005", "Other CultureInfo comes from GetCultureInfo method with the same paramter should be the same CultureInfo as the original.");
                retVal = false;
            }

        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("006", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest4()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest4:  Other (CultureInfo.InvariantCulture)  CultureInfo comes from InvariantCulture Property  is the same CultureInfo as the original");
        try
        {

            CultureInfo myCultureInfo = CultureInfo.InvariantCulture;
            CultureInfo myCultureInfo1 = CultureInfo.InvariantCulture;
            if (!myCultureInfo.Equals(myCultureInfo1))
            {
                TestLibrary.TestFramework.LogError("007", "Should return true.");
                retVal = false;
            }

        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("008", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest5()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest5: Other (CultureTypes.NeutralCultures)  CultureInfo comes from constroctor with the different paramter is not the same CultureInfo as the original( CultureTypes.SpecificCultures)");
        try
        {

            CultureInfo myCultureInfo = new CultureInfo("en");
            CultureInfo myCultureInfo1 = new CultureInfo("en-US");
            if (myCultureInfo1.Equals(myCultureInfo))
            {
                TestLibrary.TestFramework.LogError("009", "Should not be  the same Culture.");
                retVal = false;
            }

        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("010", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest6()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest6:compare to itself ");
        try
        {

            CultureInfo myCultureInfo = new CultureInfo("en-US");
            if (!myCultureInfo.Equals(myCultureInfo))
            {
                TestLibrary.TestFramework.LogError("011", "Should not be  the same Culture.");
                retVal = false;
            }

        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("012", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
    // Returns true if the expected result is right
    // Returns false if the expected result is wrong
    public bool PosTest7()
    {
        bool retVal = true;

        TestLibrary.TestFramework.BeginScenario("PosTest7: Compare to null");
        try
        {

            CultureInfo myCultureInfo = new CultureInfo("en-US");

            if (myCultureInfo.Equals(null))
            {
                TestLibrary.TestFramework.LogError("013", "Should not be  the same Culture.");
                retVal = false;
            }

        }
        catch (Exception e)
        {
            TestLibrary.TestFramework.LogError("014", "Unexpected exception: " + e);
            retVal = false;
        }
        return retVal;
    }
}

