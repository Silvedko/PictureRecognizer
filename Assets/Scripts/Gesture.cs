using UnityEngine;
using System.Collections;
using System;

public class Gesture 
{
	/// <summary>
	/// Normalized gesture points.
	/// </summary>
	public Vector2 [] points = null;

	public string Name = "";
	private const int SAMPLING_RESOLUTION = 32;

	public Gesture(Vector2[] points, string gestureName = "")
	{
		this.Name = gestureName;

		// normalizes the array of points with respect to scale, origin, and number of points
		this.points = Scale(points);
		this.points = TranslateTo(points, Centroid(points));
		this.points = Resample(points, SAMPLING_RESOLUTION);
	}

	private Vector2[] Scale(Vector2[] points)
	{
		float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
		for (int i = 0; i < points.Length; i++)
		{
			if (minx > points[i].x) minx = points[i].x;
			if (miny > points[i].y) miny = points[i].y;
			if (maxx < points[i].x) maxx = points[i].x;
			if (maxy < points[i].y) maxy = points[i].y;
		}

		Vector2[] newPoints = new Vector2[points.Length];
		float scale = Math.Max(maxx - minx, maxy - miny);
		for (int i = 0; i < points.Length; i++)
			newPoints[i] = new Vector2((points[i].x - minx) / scale, (points[i].y - miny) / scale);
		return newPoints;
	}

	private Vector2[] TranslateTo(Vector2[] points, Vector2 p)
	{
		Vector2[] newPoints = new Vector2[points.Length];
		for (int i = 0; i < points.Length; i++)
			newPoints[i] = new Vector2(points[i].x - p.x, points[i].y - p.y);
		return newPoints;
	}

	private Vector2 Centroid(Vector2[] points)
	{
		float cx = 0, cy = 0;
		for (int i = 0; i < points.Length; i++)
		{
			cx += points[i].x;
			cy += points[i].y;
		}
		return new Vector2(cx / points.Length, cy / points.Length);
	}

	public Vector2[] Resample(Vector2[] points, int n)
	{
		Vector2[] newPoints = new Vector2[n];
		newPoints [0] = new Vector2 (points [0].x, points [0].y);
		int numPoints = 1;

		float I = PathLength (points) / (n - 1); // computes interval length
		float D = 0;
		for (int i = 1; i < points.Length; i++)
		{
			float d =  Vector2.Distance (points [i - 1], points [i]);

			if (D + d >= I)
			{
				Vector2 firstPoint = points [i - 1];
				while (D + d >= I)
				{
					// add interpolated point
					float t = Math.Min (Math.Max ((I - D) / d, 0.0f), 1.0f);
					if (float.IsNaN (t))
						t = 0.5f;
					newPoints [numPoints++] = new Vector2 (
						(1.0f - t) * firstPoint.x + t * points [i].x,
						(1.0f - t) * firstPoint.y + t * points [i].y
					);

					// update partial length
					d = D + d - I;
					D = 0;
					firstPoint = newPoints [numPoints - 1];
				}
				D = d;
			} else
				D += d;
		}

		if (numPoints == n - 1) // sometimes we fall a rounding-error short of adding the last point, so add it if so
			newPoints [numPoints++] = new Vector2 (points [points.Length - 1].x, points [points.Length - 1].y);
		return newPoints;
	}

	private float PathLength(Vector2[] points)
	{
		float length = 0;
		for (int i = 1; i < points.Length; i++)
			length += Vector2.Distance (points[i - 1], points[i]);
		
		return length;
	}
}
