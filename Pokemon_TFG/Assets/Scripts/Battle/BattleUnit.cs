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
        textureFront.LoadImage(File.ReadAllBytes(Application.dataPath + "/Game/Resources/Pokemons/390.png"));
        Texture2D textureBack = new Texture2D(2, 2);
        textureBack.LoadImage(File.ReadAllBytes(Application.dataPath + "/Game/Resources/Pokemons/back/390.png"));

        Sprite frontSprite = Sprite.Create(textureFront, new Rect(0, 0, textureFront.width, textureFront.height), new Vector2(0, 0), 100.0f);
        Sprite backSprite = Sprite.Create(textureBack, new Rect(0, 0, textureBack.width, textureBack.height), new Vector2(0, 0), 100.0f);

        BasePokemon basePokemon = new BasePokemon("Chimchar", 390, frontSprite, backSprite, PokemonType.Fire, PokemonType.None, 44, 58, 44, 58, 44, 61);
        PokeApiController.GetBasePokemonWithID(390, (tempPoke) =>
        {
            basePokemon = tempPoke;
        });
        PokeApiController.GetLearnableMoves(basePokemon.Id, (moves) =>
        {
            basePokemon.LearnableMoves = moves;
        });
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
