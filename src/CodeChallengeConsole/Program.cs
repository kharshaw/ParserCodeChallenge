using System;
using System.Collections.Generic;
using CodeChallengeLib;

namespace ConsoleApplication
{
    /*
        Keith Harshaw, October 5, 2016

        Frontline Education coding challenge

        Parameters
        Can be solved in any language
        Deliver a working runnable solution and include a copy of the source code
        Style and approach is completely up to the candidate to be as creative or simple as they choose. 
        You may have to drop it into a Google Doc or GitHub because of our FireWall.
        Problem to Solve
        Convert the string: 
        "(id,created,employee(id,firstname,employeeType(id), lastname),location)" 
        to the following output 
        id
        created
        employee
        - id
        - firstname
        - employeeType
        -- id
        - lastname
        location
        
        Bonus (output in alphabetical order):
        
        created
        employee
        - employeeType
        -- id
        - firstname
        - id
        - lastname
        id
        location
    */


    /*
        Theory of Operation

        Application performs simple string processing to tokenize a string into a collection of Tokens.
        Main() new's up a Parser object passing in the input string to the ctor.  The ctor kicks off 
        the parsing process, creating Tokens along the way.  Finally, we print out the Tokens in the 
        desired/example format including sorted dislay.

        Token creation is trigged by observing a '(', ')' or ',' character.  Each Token keeps track of 
        the order in which it's found, the depth (or nestedness) within the graph, the content, and the 
        parent Token.  Tokens contain it's children Tokens.  The parser keeps a List<Token> which is not
        strictly required by this problem but I used it to test more interesting scenarios.  The list holds
        what onemight consider the root Tokens in the input. 

        Token implements IComparable and IEquatable so that we can Sort the token collections by the 
        Content property t osatisfy the print in alpha order requirement.

        This simple app is highly optimistic.  THere are no input or bounds checking, no exception
        handling.  There also have been no attempt to optimize the solution in any way.  There also remains
        some extraneous variable from earlier efforts (i.e. _order member of Token to keep track of order 
        discoverd, helpful to get back to the original ordering after the alpha sort.)

    */
    public class Program
    {
        public static void Main(string[] args)
        {
            string input = "(a,(b))";//(id,created,employee(id,firstname,employeeType(id), lastname),location)";

            Parser p = new Parser(input);

            try 
            {
                p.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Encountered parse error:{ e.Message }");
            }
            
            Console.WriteLine(p.ToString());

            Console.WriteLine(p.ToStringSorted());
        }
    }
}
