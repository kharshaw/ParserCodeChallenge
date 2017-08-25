using System;

namespace CodeChallengeLib
{
    public class UnexpectedOpeningGroupException : Exception
    {
        public UnexpectedOpeningGroupException(string message) : base(message) { }

    }

    public class UnexpectedClosingGroupException : Exception
    {
        public UnexpectedClosingGroupException(string message) : base(message) { }

    }
    
    public class UnexpectedDelimiterException : Exception
    {
        public UnexpectedDelimiterException(string message) : base(message) { }

    }

    public class UnexpectedEndOfInputException : Exception
    {
        public UnexpectedEndOfInputException(string message) : base(message) { }

    }

    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string message) : base(message) { }

    }
}