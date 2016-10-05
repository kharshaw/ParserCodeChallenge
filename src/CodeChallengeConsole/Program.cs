using System;
using System.Collections.Generic;

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
            string input = "(id,created,employee(id,firstname,employeeType(id), lastname),location)";

            Parser p = new Parser(input);

            Console.WriteLine(p.ToString());

            Console.WriteLine(p.ToStringSorted());
        }
    }

    public class Parser
    {
        private List<Token> _tokens = new List<Token>();
        private string _input = string.Empty;

        public Parser (string input)
        {
            _input = input;
            Parse();
        }

        public override string ToString()
        {
            string s = string.Empty;

            foreach (Token t in _tokens)
            {
                s += t.ToString();
            }

            return s;
        }

        public string ToStringSorted()
        {
            SortTokensByContent();
            return this.ToString();
        }

        private void SortTokensByContent()
        {
            // sort the top level tokens
            this._tokens.Sort();
            
            // then tell each token to sort the sub tokens.  Maybe use events instead??
            foreach (Token token in _tokens)
            {
                token.SortSubTokensByContent();
            }
        }

        private void Parse()
        {
            int depth = 0;
            int order = 0;
            Token currentToken = null;
            Token parentToken = null;

            string content = string.Empty;

            foreach (char c in _input.ToCharArray())
            {
                switch (c)
                {
                    case '(':
                        
                        currentToken = new Token(order, depth, content, parentToken);
                        
                        if (parentToken == null)
                        {
                            // no parent, we have a root token
                            _tokens.Add(currentToken);
                        }
                        else
                        {
                            // this is child token
                            parentToken.AddSubToken(currentToken);
                        }

                        // thte open paren indicates we're starting a new parent Token
                        parentToken = currentToken;
                        
                        depth++;
                        order++;
                        content = string.Empty;

                        break;

                    case ')':
                        
                        currentToken = new Token(order, depth, content, parentToken);
                        
                        parentToken.AddSubToken(currentToken);
                        
                        // walk back up the Token hierarchy
                        parentToken = parentToken.Parent;

                        depth--;
                        order++;
                        content = string.Empty;

                        break;

                    case ',':
                        // this bypasses creating a token when a comma follows a closing paren.
                        if (content.Length > 0)
                        {
                            currentToken = new Token(order, depth, content, parentToken);
                            
                            parentToken.AddSubToken(currentToken);
                        }

                        order++;
                        content = string.Empty;

                        break;
                    default:
                        content += c.ToString();
                        break;
                        
                }

            }
        }
    }

    public class Token : IEquatable<Token>, IComparable<Token>
    {
        
        public string Content { get; set; }
        public Token Parent { get; set; }
        private int _order;
        private int _depth;
        

        private List<Token> _tokens = new List<Token>();

        public Token (int order, int depth, string content, Token parent)
        {
          _order = order;
          _depth = depth;
          Content = content.Trim();
          Parent = parent;
        }

        public void AddSubToken(Token token)
        {
                            
            _tokens.Add(token);
        }

        public void SortSubTokensByContent()
        {
            // sort the children
            _tokens.Sort();

            // tell the children to sort the grandchildren
            foreach (Token token in _tokens)
            {
                token.SortSubTokensByContent();
            }
        }

        public override string ToString()
        {
            string s = string.Empty;

            if (Content.Length > 0)
            {
                for (int i = 0; i < _depth - 1; i++)
                {
                    s += "-";
                }

                if (s.Length > 0)
                {
                    s += " ";
                }
                s += Content + "\r\n";
            }

            foreach (Token subToken in _tokens)
            {
                s += subToken.ToString();
            }

            return s; 
        }

        /*
            IEquatable
        */
        public override bool Equals(object token)
        {
            if (token == null)
            {
                return false;
            }

            Token t = token as Token;
            
            if (t == null) 
            {
                return false;
            }
            else
            {
                    return Equals(t);
            }
        }

        /*

            IComparable
        */
        public int CompareTo(Token token)
        {
            if (token == null) 
            {
                return 1;
            }
            else
            {
                return this.Content.CompareTo(token.Content);
            }
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }

        public bool Equals(Token token)
        {
            if (token == null)
            {
                return false;
            }

            return token.Content == this.Content;
        }
    }
}
