using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    //location.GetLength(0) to obtain the number of different combinations of pokemon, level and rarity.
    //For example, puebloHojaVerde.GetLength(0) will return 4

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

    public static Dictionary<string, int[][]> locations = new Dictionary<string, int[][]>
    {
        {"puebloHojaVerde", puebloHojaVerde}
    };

    private static List<Pokemon> partyPokemons = new List<Pokemon>();
    private static List<Pokemon> enemyPartyPokemons = new List<Pokemon>();

    public static int[] GetEncounters(string locationString, int rarityLvl)
    {
        int[][] location = locations[locationString];
        int rarity = rarityLvl;
        int arraySize = 0;
        List<int[]> encounters = new List<int[]>();
        int[][] encountersWithRarity = null;

        //I check how many encounters there are with the rarity given and I save them in an arraylist, while I increase the array size.
        do
        {
            for (int i = 0; i < location.Length; i++)
            {
                if (location[i][0] == rarity)
                {
                    arraySize++;
                    encounters.Add(location[i]);
                }
            }
            rarity--;
        } while (arraySize == 0);
        encountersWithRarity = new int[arraySize][];
        //I save into the array all the encounters with the given rarity
        for (int i = 0; i < arraySize; i++)
        {
            encountersWithRarity[i] = encounters[i];
        }
        if(encountersWithRarity.Length == 1)
        {
            return encountersWithRarity[0];
        } else
        {
            int length = encountersWithRarity.Length;
            int selectedIndex = Random.Range(0, length);
            return encountersWithRarity[selectedIndex];
        }
    }
    public static List<Pokemon> PartyPokemons { get => partyPokemons; set => partyPokemons = value; }
    public static List<Pokemon> EnemyPartyPokemons { get => enemyPartyPokemons; set => enemyPartyPokemons = value; }
}
