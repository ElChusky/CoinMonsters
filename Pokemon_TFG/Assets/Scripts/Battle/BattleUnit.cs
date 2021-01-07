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

    public void Setup()
    {
        Texture2D textureFront = new Texture2D(2, 2);
        textureFront.LoadImage(File.ReadAllBytes(Application.dataPath + "/Game/Resources/Pokemons/391.png"));
        Texture2D textureBack = new Texture2D(2, 2);
        textureBack.LoadImage(File.ReadAllBytes(Application.dataPath + "/Game/Resources/Pokemons/back/391.png"));

        Sprite frontSprite = Sprite.Create(textureFront, new Rect(0, 0, textureFront.width, textureFront.height), new Vector2(0, 0), 100.0f);
        Sprite backSprite = Sprite.Create(textureBack, new Rect(0, 0, textureBack.width, textureBack.height), new Vector2(0, 0), 100.0f);

        BasePokemon basePokemon = new BasePokemon("No Funciona", 4, frontSprite, backSprite, PokemonType.Fire, PokemonType.None, 44, 58, 44, 58, 44, 61);
        StartCoroutine(PokeApiController.GetBasePokemonWithID(390, (tempPoke) =>
        {
            basePokemon = tempPoke;
        }));
        Dictionary<int, Move> learnableMoves = new Dictionary<int, Move>();
        learnableMoves.Add(0, new Move("no funciona", "", PokemonType.None, 0, 0, 0, "status"));
        PokeApiController.GetLearnableMoves(basePokemon.Id, (moves) =>
        {
            learnableMoves = moves;
        });
        basePokemon.LearnableMoves = learnableMoves;
        foreach (KeyValuePair<int, Move> moves in learnableMoves)
        {
            Debug.Log(moves.Key + " = " + moves.Value.Name);
        }
        Pokemon = new Pokemon(basePokemon, 5);
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = Pokemon.BasePoke.BackSprite;
        } else
        {
            GetComponent<Image>().sprite = Pokemon.BasePoke.FrontSprite;
        }
    }
}
