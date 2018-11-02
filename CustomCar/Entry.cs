﻿using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spectrum.API.Configuration;
using System.IO;
using Harmony;
using System.Reflection;

namespace CustomCar
{
    public static class Constants
    {
        public const int carNb = 10;
    }

    public class Configs
    {
        public string carName;
        public Configs()
        {
            var settings = new Settings("CustomCar");

            var entries = new Dictionary<string, string>
            {
                {"CarName", "Random" },
            };

            foreach (var s in entries)
                if (!settings.ContainsKey(s.Key))
                    settings.Add(s.Key, s.Value);

            settings.Save();

            carName = (string)settings["CarName"];
        }
    }

    public class Entry : IPlugin
    {
        public void Initialize(IManager manager, string ipcIdentifier)
        {
            //LogCarPrefabs.logCars();

            var profileManager = G.Sys.ProfileManager_;
            var oldCars = profileManager.carInfos_.ToArray();
            profileManager.carInfos_ = new CarInfo[Constants.carNb];

            var unlocked = (Dictionary<string, int>)profileManager.GetType().GetField("unlockedCars_", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profileManager);

            for (int i = 0; i < profileManager.carInfos_.Length; i++)
            {
                if (i < oldCars.Length)
                {
                    profileManager.carInfos_[i] = oldCars[i];
                    continue;
                }
                var car = new CarInfo();
                car.name_ = "Wtf is that " + i + " ?";
                car.prefabs_ = oldCars[0].prefabs_;
                car.colors_ = oldCars[0].colors_;
                profileManager.carInfos_[i] = car;
                unlocked.Add(car.name_, i);
            }

            var harmony = HarmonyInstance.Create("com.Larnin.CustomCar");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    //[HarmonyPatch(typeof(Profile), "GetColorsForIndex")]
    //internal class ProfileGetColorsForIndex
    //{
    //    static bool Prefix(Profile __instance, ref int index)
    //    {
    //        try
    //        {
    //            var carColorsList = (CarColors[])__instance.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

    //            if (index >= carColorsList.Length)
    //                index = 0;
    //        }
    //        catch(Exception e)
    //        {
    //            Console.Out.WriteLine(e.ToString());
    //        }

    //        return true;
    //    }
    //}

    [HarmonyPatch(typeof(Profile), "Awake")]
    internal class ProfileAwake
    {
        static void Postfix(Profile __instance)
        {
            var carColors = new CarColors[Constants.carNb];
            for(int i = 0; i < carColors.Length; i++)
                carColors[i] = G.Sys.ProfileManager_.carInfos_[i].colors_;

            var field = __instance.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(__instance, carColors);
        }
    }
}
