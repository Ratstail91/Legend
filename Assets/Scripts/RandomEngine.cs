using System;

public class RandomEngine {
	private static Random random = new Random(new System.DateTime().Millisecond);

	public double Rand(double upper) {
		return random.NextDouble () * upper;
	}
}
