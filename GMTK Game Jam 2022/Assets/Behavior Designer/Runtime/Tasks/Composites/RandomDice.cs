using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class RandomDice : Composite
    {
        public SharedInt _maxDice;

        private SharedInt _currentChildIndex;
        
        public override int CurrentChildIndex()
        {
            // Peek will return the index at the top of the stack.
            return _currentChildIndex.Value;
        }
    }

    public static class Extentions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}