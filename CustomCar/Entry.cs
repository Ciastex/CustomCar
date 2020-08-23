using System;
using System.Collections.Generic;
using Events.MainMenu;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Runtime.Patching;
using Reactor.API.Storage;

namespace CustomCar
{
    [ModEntryPoint(ModID)]
    public class Entry
    {
        public const string ModID = "com.github.larnin.CustomCar";

        private List<Assets> assets = new List<Assets>();

        public void Initialize(IManager manager)
        {
            try
            {
                RuntimePatcher.AutoPatch();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.ToString());
            }

            Initialized.Subscribe(data =>
            {
                try
                {
                    var carInfos = new CarInfos();
                    carInfos.collectInfos();
                    var builder = new CarBuilder();
                    builder.createCars(carInfos);
                }
                catch (Exception e)
                {
                    ErrorList.add("An error occured while trying to load cars assets");
                    Console.Out.WriteLine(e.ToString());
                }
                
                if (ErrorList.haveErrors())
                    ErrorList.show();
            });
        }

        public void Load()
        {


            ModdedCarsColors.LoadAll();
        }
    }
}