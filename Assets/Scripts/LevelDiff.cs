
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
		new LevelConf(2, 2, 2, .2f),
		new LevelConf(1, 2, 2, .2f),
		new LevelConf(1, 2, 3, .3f),
		new LevelConf(1, 3, 3, .3f),
		new LevelConf(1, 3, 4, .4f),
		new LevelConf(1, 4, 4, .5f),
	};
}
