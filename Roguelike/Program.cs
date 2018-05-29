﻿using Microsoft.Extensions.Configuration;
using Roguelike.Core;
using Roguelike.Systems;
using System;
using System.IO;

namespace Roguelike
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration configs = new Configuration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build()
                .Bind(configs);

            Game game = new Game(configs, OptionHandler.ParseOptions(args));
            game.Initialize();
        }
    }
}