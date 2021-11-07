
public enum Direction { NE, E, SE, SW, W, NW }

public static class DirectionExtentions
{
	public static Direction Opposite(this Direction direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static Direction Previous(this Direction direction)
	{
		return direction == Direction.NE ? Direction.NW : (direction - 1);
	}

	public static Direction Next(this Direction direction)
	{
		return direction == Direction.NW ? Direction.NE : (direction + 1);
	}

	public static Direction Previous2(this Direction direction)
	{
		direction -= 2;
		return direction <= Direction.NE ? direction : (direction + 6);
	}

	public static Direction Next2(this Direction direction)
	{
		direction += 2;
		return direction >= Direction.NW ? direction : (direction - 6);
	}
}