using System;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
	/*
	Truth be told, I modified a lot of this to make it work right, I don't know how closely it resembles the original.

	Original can be found here: https://bitbucket.org/kryzarel/generic-genetic-algorithm/src/master/
	*/
	public List<DNA<T>> Population { get; private set; }
	public int Generation { get; private set; }
	public float BestFitness { get; private set; }
	public T[] BestGenes { get; private set; }
    public string[][] BestLayout { get; private set;}
    public Room[][] BestLayoutRooms { get; private set;}

	public int Elitism;
	public float MutationRate;
	
	private List<DNA<T>> newPopulation;
	private Random random;
	private float fitnessSum;
	private int dnaSize;
	private Func<T> getRandomGene;
	private Func<int, float> fitnessFunction;
    private String[][] gene;

    private LevelBuilder builder;

	public GeneticAlgorithm(int populationSize, Random random, int elitism, LevelBuilder b, float mutationRate = 0.01f)
	{
        builder = b;
		Generation = 1;
		Elitism = elitism;
		MutationRate = mutationRate;
		Population = new List<DNA<T>>(populationSize);
		newPopulation = new List<DNA<T>>(populationSize);
		this.random = random;
		this.dnaSize = dnaSize;
		this.getRandomGene = getRandomGene;
		this.fitnessFunction = fitnessFunction;

		BestGenes = new T[dnaSize];

		for (int i = 0; i < populationSize; i++)
		{
			Population.Add(new DNA<T>(random, builder));
		}
	}

	public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
	{
		int finalCount = Population.Count + numNewDNA;

		if (finalCount <= 0) {
			return;
		}

		if (Population.Count > 0) {
			CalculateFitness();
			Population.Sort(CompareDNA);
		}
		newPopulation.Clear();

		for (int i = 0; i < Population.Count; i++)
		{
			if (i < Elitism && i < Population.Count)
			{
				newPopulation.Add(Population[i]);
			}
			else if (i < Population.Count || crossoverNewDNA)
			{
				DNA<T> parent1 = ChooseParent();
				DNA<T> parent2 = ChooseParent();

				DNA<T> child = parent1.Crossover(parent2);

				child.Mutate(MutationRate);

				newPopulation.Add(child);
			}
			else
			{
				newPopulation.Add(new DNA<T>(random, builder));
			}
		}

		List<DNA<T>> tmpList = Population;
		Population = newPopulation;
		newPopulation = tmpList;

		Generation++;
	}
	
	private int CompareDNA(DNA<T> a, DNA<T> b)
	{
		if (a.Fitness > b.Fitness) {
			return -1;
		} else if (a.Fitness < b.Fitness) {
			return 1;
		} else {
			return 0;
		}
	}

	private void CalculateFitness()
	{
		fitnessSum = 0;
		DNA<T> best = Population[0];

		for (int i = 0; i < Population.Count; i++)
		{
			fitnessSum += Population[i].CalculateFitness();

			if (Population[i].Fitness > best.Fitness)
			{
				best = Population[i];
			}
		}

		BestFitness = best.Fitness;
        BestLayout = best.GetData();
        BestLayoutRooms = best.GetRoomData();
	}

	private DNA<T> ChooseParent()
	{
		double randomNumber = random.NextDouble() * fitnessSum;

		for (int i = 0; i < Population.Count; i++)
		{
			if (randomNumber < Population[i].Fitness)
			{
				return Population[i];
			}

			randomNumber -= Population[i].Fitness;
		}

		int backupPick = (int)(random.NextDouble() * Population.Count);
		return Population[backupPick];
	}
}