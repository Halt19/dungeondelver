using System;

public class DNA<T>
{
	public T[] Genes { get; private set; }
	public float Fitness { get; private set; }

	private Random random;
    private String[][] data;
    private LevelBuilder builder;
    private Room[][] roomData;

	public DNA(Random random, LevelBuilder b)
	{
        builder = b;
        data = builder.GetDataString();
        data = builder.PerformMutation(data, 100);
		this.random = random;
		roomData = builder.MainStringToRoom(data);
        roomData = builder.HandleConflictingDoors(roomData);
        roomData = builder.CloseOffEdgeDoors(roomData); // closes off edges so you can't walk out the side of the dungeon into the void
        data = builder.RoomToMainString(roomData, data);
	}

    public void SetData(string[][] inc)
    {
        data = inc;
        roomData = builder.MainStringToRoom(inc);
        roomData = builder.HandleConflictingDoors(roomData);
        roomData = builder.CloseOffEdgeDoors(roomData); // closes off edges so you can't walk out the side of the dungeon into the void
        data = builder.RoomToMainString(roomData, data);
    }

    public String[][] GetData()
    {
        return data;
    }

    public Room[][] GetRoomData()
    {
        return roomData;
    }

	public float CalculateFitness()
	{
        if (roomData != null)
        {
        } else 
        {
            roomData = builder.MainStringToRoom(data);
        }
        Fitness = builder.CalculateFitness(data, roomData);
		return Fitness;
	}

	public DNA<T> Crossover(DNA<T> otherParent)
	{
		DNA<T> child = new DNA<T>(random, builder);
        child.SetData(builder.PerformCrossOver(data, otherParent.GetData()));
		return child;
	}

	public void Mutate(float mutationRate)
	{
		{
			data = builder.PerformMutation(data);
			roomData = builder.MainStringToRoom(data);
        	roomData = builder.HandleConflictingDoors(roomData);
        	roomData = builder.CloseOffEdgeDoors(roomData); // closes off edges so you can't walk out the side of the dungeon into the void
        	data = builder.RoomToMainString(roomData, data);
		}
	}
}