using System;
using System.Collections.Generic;

namespace WebExpress.WebIndex.Utility
{
    /// <summary>
    /// Represents a finite state transducer (FST) that transforms input strings into output strings based on defined transitions.
    /// </summary>
    public class IndexFiniteStateTransducer
    {
        private readonly Dictionary<(int, char), (int, char)> transitions;
        private int startState;
        private readonly HashSet<int> acceptStates;

        /// <summary>
        /// Initializes a new instance of the FiniteStateTransducer class.
        /// </summary>
        public IndexFiniteStateTransducer()
        {
            transitions = [];
            acceptStates = [];
        }

        /// <summary>
        /// Adds a transition to the transducer.
        /// </summary>
        /// <param name="fromState">The state from which the transition originates.</param>
        /// <param name="inputChar">The input character that triggers the transition.</param>
        /// <param name="toState">The state to which the transition leads.</param>
        /// <param name="outputChar">The output character produced by the transition.</param>
        public void AddTransition(int fromState, char inputChar, int toState, char outputChar)
        {
            transitions[(fromState, inputChar)] = (toState, outputChar);
        }

        /// <summary>
        /// Sets the start state of the transducer.
        /// </summary>
        /// <param name="state">The start state.</param>
        public void SetStartState(int state)
        {
            startState = state;
        }

        /// <summary>
        /// Adds an accept state to the transducer.
        /// </summary>
        /// <param name="state">The accept state.</param>
        public void AddAcceptState(int state)
        {
            acceptStates.Add(state);
        }

        /// <summary>
        /// Processes the input string and returns the transformed output string.
        /// </summary>
        /// <param name="input">The input string to be processed.</param>
        /// <returns>The output string resulting from the transformation.</returns>
        public string ProcessInput(string input)
        {
            int currentState = startState;
            string output = "";

            foreach (char c in input)
            {
                if (transitions.TryGetValue((currentState, c), out var transition))
                {
                    currentState = transition.Item1;
                    output += transition.Item2;
                }
                else
                {
                    // No valid transition found
                    throw new InvalidOperationException("Invalid input string.");
                }
            }

            if (acceptStates.Contains(currentState))
            {
                return output;
            }
            else
            {
                // Not in an accepting state
                throw new InvalidOperationException("Input string was not accepted.");
            }
        }
    }
}
