using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System.IO;

public static class PokeApiController
{
    /*
    BasePokemon pokemon;
    PokeApiController.GetBasePokemonWithID(1, (tempPoke) => {
            pokemon = tempPoke;
            });*/

private static string apiBaseURL = "https://pokeapi.co/api/v2/";
    public static IEnumerator GetBasePokemonWithID(int id, System.Action<BasePokemon> tempPoke)
   {
        //We get the Sprites of our Pokemon from the Data Path
        //(dataPath is: in Unity => "projectPath/Assets/"
        //in Windows/Linux => "executablename_Data" folder
        //in Android => It points directly to the apk, unless you are running a split binary build, in which case it points to the OBB;
        Sprite frontSprite = LoadNewSprite(Application.dataPath + "/Game/Resources/Pokemons/" + id + ".png");
        Sprite backSprite = LoadNewSprite(Application.dataPath + "/Game/Resources/Pokemons/back/" + id + ".png");

        //We create a UnityWebRequest object with our apiURL, that will contact with the API
        string pokemonURL = apiBaseURL + "pokemon/" + id;
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        //We make the request and check for errors
        yield return pokeInfoRequest.SendWebRequest();
        
        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        //We obtain the info about the pokemon with the given id
        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        //We obtain the name of the pokemon
        string pokeName = pokeInfo["name"];

        //We obtain the types of the pokemon
        JSONNode pokeTypes = pokeInfo["types"];
        PokemonType[] types = new PokemonType[2];
        types[1] = PokemonType.None;

        for (int i = 0; i < pokeTypes.Count; i++)
        {
            string tempType = pokeTypes[i]["type"]["name"];
            switch (tempType)
            {
                case "normal":
                    types[i] = PokemonType.Normal;
                    break;
                case "fighting":
                    types[i] = PokemonType.Fighting;
                    break;
                case "flying":
                    types[i] = PokemonType.Flying;
                    break;
                case "poison":
                    types[i] = PokemonType.Poison;
                    break;
                case "ground":
                    types[i] = PokemonType.Ground;
                    break;
                case "rock":
                    types[i] = PokemonType.Rock;
                    break;
                case "bug":
                    types[i] = PokemonType.Bug;
                    break;
                case "ghost":
                    types[i] = PokemonType.Ghost;
                    break;
                case "steel":
                    types[i] = PokemonType.Steel;
                    break;
                case "fire":
                    types[i] = PokemonType.Fire;
                    break;
                case "water":
                    types[i] = PokemonType.Water;
                    break;
                case "grass":
                    types[i] = PokemonType.Grass;
                    break;
                case "electric":
                    types[i] = PokemonType.Electric;
                    break;
                case "psychic":
                    types[i] = PokemonType.Psychic;
                    break;
                case "ice":
                    types[i] = PokemonType.Ice;
                    break;
                case "dragon":
                    types[i] = PokemonType.Dragon;
                    break;
                case "dark":
                    types[i] = PokemonType.Dark;
                    break;
            }
        }

        //We obtain the base stats of the pokemon
        JSONNode pokeStats = pokeInfo["stats"]; //[0] = baseHP, [1] = baseAttack, [2] = baseDefense, [3] = baseSpAttack, [4] = baseSpDefense, [5] = baseSpeed;
        int baseHP = pokeStats[0]["base_stat"];
        int baseAttack = pokeStats[1]["base_stat"];
        int baseDefense = pokeStats[2]["base_stat"];
        int baseSpAttack = pokeStats[3]["base_stat"];
        int baseSpDefense = pokeStats[4]["base_stat"];
        int baseSpeed = pokeStats[5]["base_stat"];

        //We give the BasePokemon object with the data back to the function
        tempPoke(new BasePokemon(pokeName, id, frontSprite, backSprite, types[0], types[1], baseHP, baseAttack, baseDefense, baseSpAttack, baseSpDefense, baseSpeed));
   }

    public static IEnumerator GetLearnableMoves(int id, System.Action<Dictionary<int, Move>> learnableMoves)
    {
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(apiBaseURL + "pokemon/" + id);

        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        //We obtain the moves of the pokemon with the given id
        JSONNode pokeMoves = pokeInfo["moves"];

        //We create a dictionary, in which we will add all the moves learned by level
        Dictionary<int, Move> moves = new Dictionary<int, Move>();

        JSONNode pokeVersions;
        string pokemonVersion, learnMethod;

        //We go through all the moves, to get its info
        for(int currentPokeMove = 0; currentPokeMove < pokeMoves.Count; currentPokeMove++)
        {
            pokeVersions = pokeMoves[currentPokeMove]["version_group_details"];

            //We go through all the games in which the pokemon with the given id has the move we are currently at, cause we want to check if the move was on the diamond and pearl game
            for(int currentPokeVersion = 0; currentPokeVersion < pokeVersions.Count; currentPokeVersion++)
            {
                pokemonVersion = pokeVersions[currentPokeVersion]["version_group"]["name"];
                //We check if the current game is diamond pearl, since we only want info about that game
                if (pokemonVersion.Equals("diamond-pearl"))
                {
                    learnMethod = pokeVersions[currentPokeVersion]["move_learn_method"]["name"];
                    //We check if the learn method is leveling up, since we wont handle other learning methods
                    if (learnMethod.Equals("level-up"))
                    {
                        Move learnableMove;

                        int learnedAt = pokeVersions[currentPokeVersion]["level_learned_at"];
                        string moveURL = pokeMoves[currentPokeMove]["move"]["url"];

                        //We make a request of the move with the move url, to get all the info about the move
                        UnityWebRequest moveRequest = UnityWebRequest.Get(moveURL);

                        yield return moveRequest.SendWebRequest();

                        if(moveRequest.isNetworkError || moveRequest.isHttpError)
                        {
                            Debug.LogError(moveRequest.error);
                            yield break;
                        }

                        //We get the info about the move
                        JSONNode move = JSON.Parse(moveRequest.downloadHandler.text);

                        int accuracy = move["accuracy"];
                        string damageClass = move["damage_class"]["name"];
                        int power = move["power"];
                        int pp = move["pp"];
                        string tempType = move["type"]["name"];
                        PokemonType type = PokemonType.None;
                        switch (tempType)
                        {
                            case "normal":
                                type = PokemonType.Normal;
                                break;
                            case "fighting":
                                type = PokemonType.Fighting;
                                break;
                            case "flying":
                                type = PokemonType.Flying;
                                break;
                            case "poison":
                                type = PokemonType.Poison;
                                break;
                            case "ground":
                                type = PokemonType.Ground;
                                break;
                            case "rock":
                                type = PokemonType.Rock;
                                break;
                            case "bug":
                                type = PokemonType.Bug;
                                break;
                            case "ghost":
                                type = PokemonType.Ghost;
                                break;
                            case "steel":
                                type = PokemonType.Steel;
                                break;
                            case "fire":
                                type = PokemonType.Fire;
                                break;
                            case "water":
                                type = PokemonType.Water;
                                break;
                            case "grass":
                                type = PokemonType.Grass;
                                break;
                            case "electric":
                                type = PokemonType.Electric;
                                break;
                            case "psychic":
                                type = PokemonType.Psychic;
                                break;
                            case "ice":
                                type = PokemonType.Ice;
                                break;
                            case "dragon":
                                type = PokemonType.Dragon;
                                break;
                            case "dark":
                                type = PokemonType.Dark;
                                break;
                        }

                        //We get all the descriptions and search the spanish one with a loop
                        JSONNode descriptions = move["flavor_text_entries"];
                        string description = "";

                        for (int currentDescription = 0; currentDescription < descriptions.Count; currentDescription++)
                        {
                            string descLang = descriptions[currentDescription]["language"]["name"];
                            if (descLang.Equals("es"))
                            {
                                description = descriptions[currentDescription]["flavor_text"];
                            }
                        }

                        //We get all the names and search the spanish one with a loop
                        JSONNode names = move["names"];
                        string name = "";

                        for (int currentName = 0; currentName < names.Count; currentName++)
                        {
                            string nameLang = names[currentName]["language"]["name"];
                            if (nameLang.Equals("es"))
                            {
                                name = names[currentName]["name"];
                            }
                        }
                        Debug.Log("Learnable Move: " + name);
                        //Finally, we create the move and add it to the temporal dictionary that we will return
                        learnableMove = new Move(name, description, type, power, accuracy, pp, damageClass);

                        moves.Add(learnedAt, learnableMove);
                    }
                    //We break the for loop, since we already found the diamond_pearl game and the info about the move within it.
                    break;
                }
            }
        }
        learnableMoves(moves);
    }

    private static Texture2D LoadTexture(string filePath)
    {
        Texture2D Text2D;
        byte[] FileData;

        if (File.Exists(filePath))
        {
            FileData = File.ReadAllBytes(filePath);
            Text2D = new Texture2D(2, 2);
            if (Text2D.LoadImage(FileData))// Load the imagedata into the texture (size is set automatically)
                return Text2D;
        }
        return null;
    }

    private static Sprite LoadNewSprite(string filePath, float PixelsPerUnit = 100.0f)
    {
        Sprite NewSprite;
        Texture2D SpriteTexture = LoadTexture(filePath);
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),
            new Vector2(0, 0), PixelsPerUnit);
        return NewSprite;
    }
}
