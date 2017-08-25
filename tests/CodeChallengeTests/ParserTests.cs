using System;
using Xunit;
using CodeChallengeLib;

namespace CodeChallengeTests
{
    public class UnitTest1
    {
        [Fact]
        public void Parse_ChallengeString_Unsorted_ProducesChallengeResult()
        {
            // arrange
            string input = "(id,created,employee(id,firstname,employeeType(id), lastname),location)";
            string expectedOutput = "id\r\ncreated\r\nemployee\r\n- id\r\n- firstname\r\n- employeeType\r\n-- id\r\n- lastname\r\nlocation\r\n";

            Parser p = new Parser(input);

            // act            
            p.Parse();

            // assert
            Assert.Equal(expectedOutput, p.ToString());
            
        }

        [Fact]
        public void Parse_ChallengeString_Sorted_ProducesChallengeResult()
        {
            // arrange
            string input = "(id,created,employee(id,firstname,employeeType(id), lastname),location)";
            string expectedOutput = "created\r\nemployee\r\n- employeeType\r\n-- id\r\n- firstname\r\n- id\r\n- lastname\r\nid\r\nlocation\r\n";

            Parser p = new Parser(input);
            
            // act            
            p.Parse();

            // assert
            Assert.Equal(expectedOutput, p.ToStringSorted());
            
        }

        [Fact]
        public void Parse_EmptyString_UnexpectedEndOfInputException()
        {
            // arrange
            string input = string.Empty;
            string expectedOutput = string.Empty;
            Parser p = new Parser(input);

            // act
            var exception = Record.Exception(() => p.Parse());

            // assert
            Assert.IsType<UnexpectedEndOfInputException>(exception);
        }

        [Fact]
        public void Parse_MissingCloseGroup_UnexpectedEndOfInputException()
        {
            // arrange
            string input = "(a";
            string expectedOutput = string.Empty;

            Parser p = new Parser(input);
            
            // act            
            var exception = Record.Exception(() => p.Parse());

            // assert
            Assert.IsType<UnexpectedEndOfInputException>(exception);
            
        }

        [Fact]
        public void Parse_MisplacedOpeningGroup_UnexpectedOpeningGroupException()
        {
            string input = "(a,()";

            var p = new Parser(input);

            // act
            var exception = Record.Exception(() => p.Parse());

            // assert
            Assert.IsType<UnexpectedOpeningGroupException>(exception);
        }

        [Fact]
        public void Parse_NoGroup_UnexpectedTokenGroupException()
        {
            string input = "a,b";

            var p = new Parser(input);

            // act
            var exception = Record.Exception(() => p.Parse());

            // assert
            Assert.IsType<UnexpectedTokenException>(exception);
        }
        
        [Fact]
        public void Parse_DelimiterFollowingOpenGroup_UnexpectedDelimiterException()
        {
            string input = "(,b)";

            var p = new Parser(input);

            // act
            var exception = Record.Exception(() => p.Parse());

            // assert
            Assert.IsType<UnexpectedDelimiterException>(exception);
        }
    }
}
