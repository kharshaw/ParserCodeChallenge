using System;
using System.Collections.Generic;
using System.Collections;

namespace CodeChallengeLib
{
    public class Parser
    {
        private List<Token> _tokens = new List<Token>();
        private string _input = string.Empty;
        private char _groupStartCharacter;
        private char _groupEndCharacter;
        private char _delimiterCharacter;

        private ParseState _currentState;
        private int _depth = 0;
        private int _order = 0;
        private int _currentIndex = 0;

        private Dictionary<ParseState, List<ParseState>>  _validTransitions;

        public Parser (string input = "", char groupStartCharacter = '(', char groupEndCharacter = ')', char delimiterCharacter = ',')
        {
            _input = input;
            _groupStartCharacter = groupStartCharacter;
            _groupEndCharacter = groupEndCharacter;
            _delimiterCharacter = delimiterCharacter;

            _currentState = ParseState.Initialized;

            _validTransitions = new Dictionary<ParseState, List<ParseState>>()
            {
                { ParseState.Uninitialized, new List<ParseState> { ParseState.Initialized }},
                { ParseState.Initialized, new List<ParseState> { ParseState.GroupStarted }},
                { ParseState.GroupStarted, new List<ParseState> { ParseState.GroupStarted, ParseState.TokenDiscovery }},
                { ParseState.GroupEnded, new List<ParseState> { ParseState.GroupEnded, ParseState.Delimiter }},
                { ParseState.Delimiter, new List<ParseState> { ParseState.TokenDiscovery }},
                { ParseState.TokenDiscovery, new List<ParseState> { ParseState.TokenDiscovery, ParseState.Delimiter, ParseState.GroupEnded, ParseState.GroupStarted }}
            };
            
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

        private bool TransitionStateTo(ParseState transitionToState)
        {
            if (_validTransitions[_currentState].Exists(s => s == transitionToState))
            {
                _currentState = transitionToState;
                return true;
            }
            return false;
        }

        public void Parse()
        {
            

            Token currentToken = null;
            Token parentToken = null;

            string content = string.Empty;

            foreach (char c in _input.ToCharArray())
            {

                if (c == _groupStartCharacter)
                {
                    if (!TransitionStateTo(ParseState.GroupStarted))
                    {
                        throw new UnexpectedOpeningGroupException($"Found unexpected '{c.ToString()}' at postion {_currentIndex}");
                    }

                    currentToken = new Token(_order, _depth, content, parentToken);
                    
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
                    
                    _depth++;
                    _order++;
                    content = string.Empty;
                }
                else if (c == _groupEndCharacter)
                {       
                    
                    if (!TransitionStateTo(ParseState.GroupEnded) || _depth == 0)
                    {
                        throw new UnexpectedOpeningGroupException($"Found unexpected '{c.ToString()}' at postion {_currentIndex}");
                    }


                    currentToken = new Token(_order, _depth, content, parentToken);
                    
                    parentToken.AddSubToken(currentToken);
                    
                    // walk back up the Token hierarchy
                    parentToken = parentToken.Parent;

                    _depth--;
                    _order++;
                    content = string.Empty;

                }
                else if (c == _delimiterCharacter)
                {
                    if (!TransitionStateTo(ParseState.Delimiter))
                    {
                        throw new UnexpectedDelimiterException($"Found unexpected '{c.ToString()}' at postion {_currentIndex}");
                    }

                    // this bypasses creating a token when a comma follows a closing paren.
                    if (content.Length > 0)
                    {
                        currentToken = new Token(_order, _depth, content, parentToken);
                        
                        parentToken.AddSubToken(currentToken);
                    }

                    _order++;
                    content = string.Empty;
                }
                else
                {
                    if (!TransitionStateTo(ParseState.TokenDiscovery))
                    {
                        throw new UnexpectedTokenException($"Found unexpected '{c.ToString()}' at postion {_currentIndex}");
                    }
                    content += c.ToString();
                }

                _currentIndex++;

            }

            if (_currentState != ParseState.GroupEnded)
            {
                throw new UnexpectedEndOfInputException($"Found unexpected end of input at postion {_currentIndex}");
            }
        }
    }
}