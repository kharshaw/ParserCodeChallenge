using System;

namespace CodeChallengeLib
{
    public enum ParseState
    {
        Uninitialized,
        Initialized,
        GroupStarted,
        GroupEnded,
        TokenDiscovery,
        Delimiter,
        Terminated
    }
}