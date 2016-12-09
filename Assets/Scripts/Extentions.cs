using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extentions
{
	private static System.Random rng = new System.Random ();  

	public static void Shuffle(this IList<Gesture> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next (n + 1);  
			Gesture value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
