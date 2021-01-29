using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    private BasePokemon basePokemon;

    private int level;
    public bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }
    public BasePokemon BasePokemon { get => basePokemon; set => basePokemon = value; }

    public void Setup()
    {
        basePokemon = PokeApiController.BasePokemon;

        Pokemon = new Pokemon(basePokemon, 5);
        if (isPlayerUnit)
        {
            //Cuando esté hecho, habrá que coger la info de la party y tal, que se guardará en Utils
            List<Tuple<int, Move>> moves = new List<Tuple<int, Move>>();
            moves.Add(new Tuple<int, Move>(1, new Move("Arañazo", "sadasdasddddddd", PokemonType.Normal, 40, 100, 35, "physical")));
            moves.Add(new Tuple<int, Move>(1, new Move("Malicioso", "sadasdasddddddd", PokemonType.Normal, 0, 100, 30, "status")));
            moves.Add(new Tuple<int, Move>(1, new Move("Ascuas", "sadasdasddddddd", PokemonType.Fire, 40, 100, 25, "special")));
            Sprite frontSprite = PokeApiController.LoadNewSprite(Application.dataPath + "/Game/Resources/Pokemons/" + 390 + ".png");
            Sprite backSprite = PokeApiController.LoadNewSprite(Application.dataPath + "/Game/Resources/Pokemons/back/" + 390 + ".png");
            BasePokemon = new BasePokemon("Chimchar", 390, frontSprite, backSprite, PokemonType.Fire, PokemonType.None, 44, 58, 44, 58, 44, 61, moves);
            Pokemon = new Pokemon(BasePokemon, 5);
            GetComponent<Image>().sprite = Pokemon.BasePoke.BackSprite;
        } else
        {
            Pokemon = new Pokemon(PokeApiController.BasePokemon, PokeApiController.PokeLevel);
            GetComponent<Image>().sprite = Pokemon.BasePoke.FrontSprite;
        }
    }
}
