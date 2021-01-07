using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    //location.GetLength(0) to obtain the number of different combinations of pokemon, level and rarity.
    //For example, puebloHojaVerde.GetLength(0) will return 4.

    public static Dictionary<string, int[][]> locations = new Dictionary<string, int[][]>
    {
        {"puebloHojaVerde", puebloHojaVerde}
    };

    #region Locations
    //Bidimensional Arrays of size [numberOfEncounters][3] ({rarityLevel(1-5), PokemonID, level})
    private static int[][] puebloHojaVerde = new int[][]{
        new int[]{1, 396, 3},
        new int[]{2, 396, 4},
        new int[]{4, 387, 3},
        new int[]{5, 387, 4}
    };
    private static int[] route101_0 = {1, 2};
    #endregion

    private static List<Pokemon> partyPokemons = new List<Pokemon>();
    private static List<Pokemon> enemyPartyPokemons = new List<Pokemon>();
    private static Pokemon wildPokemon;

    public static int[][] GetEncounters(string locationString, int rarityLvl)
    {
        int[][] location = locations[locationString];
        int arraySize = 0;
        List<int[]> encounters = new List<int[]>();
        int[][] encountersWithRarity = null;

        //I check how many encounters there are with the rarity given and I save them in an arraylist, while I increase the array size.
        for (int i = 0; i < location[0].Length; i++)
        {
            if (location[i][0] == rarityLvl) arraySize++;
            encounters.Add(location[i]);
        }
        encountersWithRarity = new int[arraySize][];
        //I save into the array all the encounters with the given rarity
        for (int i = 0; i < arraySize; i++)
        {
            encountersWithRarity[i] = encounters[0];
        }
        return encountersWithRarity;
    }
    public static List<Pokemon> PartyPokemons { get => partyPokemons; set => partyPokemons = value; }
    public static Pokemon WildPokemon { get => wildPokemon; set => wildPokemon = value; }
    public static List<Pokemon> EnemyPartyPokemons { get => enemyPartyPokemons; set => enemyPartyPokemons = value; }
}
