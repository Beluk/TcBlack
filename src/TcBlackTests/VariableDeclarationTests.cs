using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class VariableDeclarationTests
    {
        [Theory]
        [InlineData("variable1", "BOOL")]
        [InlineData("variable_1 ", "BOOL")]
        [InlineData("String1 ", " STRING")]
        [InlineData("var ", " LREAL ")]
        [InlineData(" number ", "    DINT")]
        [InlineData("   _internalVar4", "DWORD ")]
        [InlineData("   SHOUTING ", " int ")]
        [InlineData("aSample1 ", "ARRAY[1..5] OF INT")]
        [InlineData("pSample ", "POINTER TO INT")]
        [InlineData("refInt ", "REFERENCE TO INT")]
        [InlineData("aSample ", "ARRAY[*] OF INT")]
        [InlineData("typeclass ", "__SYSTEM.TYPE_CLASS")]
        [InlineData("fbSample ", "FB_Sample(nId_Init := 11, fIn_Init := 33.44)")]
        [InlineData(
            "afbSample2  ", 
            "ARRAY[0..1, 0..1] OF FB_Sample[(nId_Init:= 100, fIn_Init:= 123.456)]"
        )]
        [InlineData(
            "afbSample1",
            "ARRAY[0..1, 0..1] OF FB_Sample[" 
                + "(nId_Init:= 12, fIn_Init:= 11.22),"
                + "(nId_Init:= 13, fIn_Init:= 22.33)," 
                + "(nId_Init:= 14, fIn_Init:= 33.44)," 
                + "(nId_Init:= 15, fIn_Init:= 44.55)]"
        )]
        public void VarAndTypeVariousWhitespaceArrangements(
            string variable, string type
        )
        {
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable}:{type};", singleIndent:"    ", lineEnding: "\n"
            );
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(), "", varDecl.RemoveWhiteSpaceIfPossible(type), "", ""
            );
            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        [Theory]
        [InlineData("voltage ", "%I*", "INT")]
        [InlineData("Curr_2 ", "%I*", "BYTE")]
        [InlineData("   _nonSense  ", "%I* ", " BOOL  ")]
        [InlineData("  Bl12K  ", "      %I* ", " BOOL  ")]
        [InlineData("Uint", " %I* ", " UINT  ")]
        [InlineData(" RFRW_3 ", "  %I*", "REAL")]
        [InlineData(" __Asdf ", "  %I* ", " STRING  ")]
        [InlineData("voltage ", "%Q*", "UDINT")]
        [InlineData("voltage", " %Q* ", "T_MaxString")]
        [InlineData("_V", " %Q*    ", "    DWORD ")]
        [InlineData("I ", " %Q* ", "BOOL   ")]
        [InlineData("fResistance     ", "   %Q* ", "LREAL")]
        [InlineData("fResistance  ", "   %Q* ", "    UINT ")]
        [InlineData("bCurrent_3    ", " %M*     ", "   LREAL   ")]
        [InlineData("fInt", "%QX7.5", "    INT ")]
        [InlineData("sString", " %IW215     ", "  STRING")]
        [InlineData("_bBool  ", " %QB7 ", " BOOL  ")]
        [InlineData("bCurrent_3", " %MD48 ", "LREAL ")]
        [InlineData("Current", "%IW2.5.7.1 ", "DINT")]
        public void VarAllocationAndTypeVariousWhitespaceArangements
        (
            string variable, string allocation, string type)
        {
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable} AT {allocation}:{type};", 
                singleIndent: "    ", 
                lineEnding: "\n"
            );
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(), allocation.Trim(), type.Trim(), "", ""
            );
            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        [Theory]
        [InlineData("voltage ", "AT %I*", "INT", "3")]
        [InlineData("weight ", "", "LREAL", "3.14159")]
        [InlineData("weight ", "", "LREAL", "1E-5")]
        [InlineData("pid_controller ", "", "ST_Struct  ", " (nVar1:=1, nVar2:=4)")]
        [InlineData("Light", "", "photons  ", "2.4 ")]
        [InlineData("SomeWords ", "", "T_MaxString ", " 'Black quartz watch my vow.'")]
        [InlineData("aSample_3  ", "", "ARRAY[1..2, 2..3, 3..4] OF INT ", "[2(0),4(4),2,3]")]
        [InlineData(
            "aSample4", " AT %Q*", " ARRAY[1..3] OF ST_STRUCT1 ", 
            "[(n1:= 1, n2:= 10, n3:= 4723),\n"
            + "(n1:= 2, n2:= 0, n3:= 299),\n" 
            + "(n1:= 14, n2:= 5, n3:= 112)]"
        )]
        [InlineData("wsWSTRING ", "", "WSTRING", "\"abc\"")]
        [InlineData("dtDATEANDTIME ", "", "DATE_AND_TIME    ", "DT#2017-02-20-11:07:00  ")]
        [InlineData("   tdTIMEOFDAY ", "", "  TIME_OF_DAY ", "    TOD#11:07:00")]
        [InlineData("nDINT", "", "DINT", "-12345")]
        [InlineData(" nDWORD", "", "DWORD", "16#6789ABCD")]
        [InlineData("sVar  ", "", "STRING(35)", "'This is a String'")]
        [InlineData(
            "stPoly1  ", "", "ST_Polygonline", 
            "(aStartPoint:=[3,3],aPoint1:=[5,2], aPoint2:=[7,3],aPoint3:=[8,5],aPoint4:=[5,7],aEndPoint:=[3,5])"
        )]
        public void VarAllocationTypeAndInitializationVariousWhitespaceArangements(
            string variable, string allocation, string type, string initialization
        )
        {
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable}{allocation}:{type}:={initialization};",
                singleIndent: "    ",
                lineEnding: "\n"
            );
            string _allocation = allocation.Replace("AT", "");
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(), 
                _allocation.Trim(),
                varDecl.RemoveWhiteSpaceIfPossible(type.Trim()), 
                varDecl.RemoveWhiteSpaceIfPossible(initialization), ""
            );

            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        [Theory]
        [InlineData(
            "Boolean  ", "AT %Q*", "BOOL", "TRUE", "// Very important comment"
        )]
        [InlineData("weight  ", "", "LREAL", "3.124", "// Comment with numbers 123  ")]
        [InlineData("  weight ", "", "  LREAL", "3.124", "  // $p�(|^[ characters")]
        [InlineData(" Pressure ", "", "  LREAL", "0.04", "  (* Chamber 1 pressure *)")]
        [InlineData("Name ", "", "STRING   ", "", "  (* Multi \n line \n comment *) ")]
        public void AllDeclarationsVariousWhitespaceArangements(
            string variable, 
            string allocation, 
            string type, 
            string initialization, 
            string comment
        )
        {
            VariableDeclaration varDecl = new VariableDeclaration(
                $"{variable}{allocation}:{type}:={initialization};{comment}",
                singleIndent: "    ",
                lineEnding: "\n"
            );
            string _allocation = allocation.Replace("AT", "");
            TcDeclaration expectedDecl = new TcDeclaration(
                variable.Trim(),
                _allocation.Trim(),
                varDecl.RemoveWhiteSpaceIfPossible(type.Trim()),
                varDecl.RemoveWhiteSpaceIfPossible(initialization),
                comment.Trim()
            );

            AssertEquals(expectedDecl, varDecl.Tokenize());
        }

        //TODO: add tests and code to recognize "nVar : INT; {info 'TODO: should get another name'}"

        private void AssertEquals(TcDeclaration expected, TcDeclaration actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Allocation, actual.Allocation);
            Assert.Equal(expected.DataType, actual.DataType);
            Assert.Equal(expected.Initialization, actual.Initialization);
            Assert.Equal(expected.Comment, actual.Comment);
        }

        [Theory]
        [InlineData("   variable1  : BOOL      ;", "variable1 : BOOL;")]
        [InlineData(
            "deviceDown      AT     %QX0.2  :        BOOL ; ", 
            "deviceDown AT %QX0.2 : BOOL;"
        )]
        [InlineData("devSpeed:TIME:=T#10ms;", "devSpeed : TIME := T#10ms;")]
        [InlineData(
            "fbSample   :   FB_Sample(nId_Init := 11, fIn_Init := 33.44)   ;",
            "fbSample : FB_Sample(nId_Init:=11, fIn_Init:=33.44);"
        )]
        [InlineData(
            "var1   :   REAL := 8   ; // Comment",
            "var1 : REAL := 8; // Comment"
        )]
        public void FormatVariableDeclaration(string unformattedCode, string expected)
        {
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode:unformattedCode, singleIndent:"    ", lineEnding: "\n"
            );
            uint indents = 0;
            Assert.Equal(expected, variable.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "var1   :   REAL := 8   ; // Comment",
            "    var1 : REAL := 8; // Comment",
            1
        )]
        [InlineData(
            "deviceDown      AT     %QX0.2  :        BOOL ; ",
            "                    deviceDown AT %QX0.2 : BOOL;",
            5
        )]
        public void FormatVariableDeclarationWithIndentation(
            string unformattedCode, string expected, uint indents)
        {
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode: unformattedCode, singleIndent: "    ", lineEnding: "\n"
            );
            Assert.Equal(expected, variable.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "FB_Sample( nId_Init := 11, fIn_Init := 33.44 )",
            "FB_Sample(nId_Init:=11,fIn_Init:=33.44)"
        )]
        [InlineData(
            "(aStartPoint:=[3, 3] ,aPoint1:=[    5,2], aPoint2:=[7,3],   aPoint3:=[8,5],aPoint4 := [5,7],aEndPoint   := [3,5]\n)",
            "(aStartPoint:=[3,3],aPoint1:=[5,2],aPoint2:=[7,3],aPoint3:=[8,5],aPoint4:=[5,7],aEndPoint:=[3,5])"
        )]
        [InlineData(
            "ARRAY[1..2,    2..3, 3..4] OF UINT",
            "ARRAY[1..2,2..3,3..4] OF UINT"
        )]
        [InlineData(
            "[ (n1:= 1, n2 := 10, n3:= 4723   ),\n"
            + " (n1:= 2, n2 := 0, n3:= 299) ,\n"
            + "( n1:=14, n2:= 5,  n3:=112)];",
            "[(n1:=1,n2:=10,n3:=4723),(n1:=2,n2:=0,n3:=299),(n1:=14,n2:=5,n3:=112)];"
        )]
        public void RemoveWhitespace(string input, string expected)
        {
            VariableDeclaration variable = new VariableDeclaration(
                unformattedCode:"", singleIndent:"    ", lineEnding: "\n"
            );
            string actual = variable.RemoveWhiteSpaceIfPossible(input);

            Assert.Equal(expected, actual);
        }
    }
}