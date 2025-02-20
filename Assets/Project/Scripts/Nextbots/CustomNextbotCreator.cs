using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CustomNextbotCreator
{
    private static string _extensions = "*.png";

    public static Texture2D[] LoadNextbotsTextures()
    {
        List<Texture2D> _textures = new List<Texture2D>();

        string pathToDirectory = Path.Combine(Application.streamingAssetsPath, "Nextbots");

        if (!Directory.Exists(pathToDirectory))
            Directory.CreateDirectory(pathToDirectory);

        string[] files = Directory.GetFiles(pathToDirectory, _extensions, SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            Texture2D t = new Texture2D(1, 1);
            t.filterMode = FilterMode.Point;
            t.LoadImage(File.ReadAllBytes(file));

            if (t.width == t.height)
                _textures.Add(t);
        }

        return _textures.Count != 0 ? _textures.ToArray() : null;
    }

    public static Nextbot[] LoadAllCustomNextbots()
    {
        Texture2D[] textures = LoadNextbotsTextures();

        if (textures == null || textures.Length == 0)
            return null;

        Nextbot[] bots = new Nextbot[textures.Length];

        for (int i = 0; i < textures.Length; i++)
        {
            Nextbot bot = ScriptableObject.CreateInstance<Nextbot>();
            bot.Texture = textures[i];
            bots[i] = bot;
        }

        return bots;
    }
}