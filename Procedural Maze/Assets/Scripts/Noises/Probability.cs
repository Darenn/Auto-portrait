using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Probability {

    /// <summary>
    /// Create a probability array using the given probabilities.
    /// </summary>
    /// <param name="caseToProb">
    /// Each key is the identifier of a case, the value is the probability. 
    /// The sum of values should be equal to 100.
    /// </param>
    /// <returns>The probability array.</returns>
	public static int[] CreateProbabilityArray(Dictionary<int, int> caseToProb)
    {
        int[] probArray = new int[100];
        int index = 0;
        foreach (var pair in caseToProb)
        {
            int lastIndex = index;
            while (index < lastIndex + pair.Value) probArray[index++] = pair.Key;
        }
        return probArray;
    }
}
