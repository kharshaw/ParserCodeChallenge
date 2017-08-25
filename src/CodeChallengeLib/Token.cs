using System;
using System.Collections.Generic;

namespace CodeChallengeLib
{
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