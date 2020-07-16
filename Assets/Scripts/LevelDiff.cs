
public class LevelConf
{
	internal int nState;
	internal int step;
	internal int maxNeighbours;
	internal float edgeRatio;

	public LevelConf(int step, int nState, int maxNeighbours, float edgeRatio)
	{
		this.step = step;
		this.nState = nState;
		this.maxNeighbours = maxNeighbours;
		this.edgeRatio = edgeRatio;
	}
}

public static class LevelDiff
{
	public static LevelConf[] config = {
		new LevelConf(3, 2, 2, .2f),
		new LevelConf(1, 2, 2, .2f),
		new LevelConf(1, 2, 3, .3f),
		new LevelConf(1, 3, 3, .3f),
		new LevelConf(2, 3, 4, .4f),
		new LevelConf(2, 4, 4, .6f),
	};
}

public static class LevelText
{
	public static string[] text = {
		"",
		"Try click one, see what happens.",
		"Now, match the color.",
		"Good. Now, try again...",
		"Nice. You know what to do now.",
		"More colors' coming.",
		"This is close to the end of the journey...",
		"You've done all of them. Thanks for playing.",
	};
}