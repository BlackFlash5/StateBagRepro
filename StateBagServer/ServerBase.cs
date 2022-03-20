using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace StateBagServer
{
    public class ServerBase : BaseScript
    {
        public ServerBase()
        {
            API.AddStateBagChangeHandler("TestBag", null, new Action<string, string, dynamic, int, bool>(this.StateBagChangeHandler));
        }

        private void StateBagChangeHandler(string bagName, string key, dynamic value, int reserved, bool replicated)
        {
            IDictionary<string, object> bagValue = value;

            Debug.WriteLine($"BagName: {bagName} | Key: {key} | Replicated: {replicated}");

            if (bagValue == null)
            {
                Debug.WriteLine($"Bag is empty!");
                return;
            }

            foreach (KeyValuePair<string, object> kvp in bagValue)
            {
                Debug.WriteLine($"Key: {kvp.Key} | Value: {kvp.Value}");
            }
        }

        [Command("bagserver")]
        private void OnBagServer([FromSource] Player player)
        {
            Debug.WriteLine($"OnBagServer | Player: {player.Name}");

            int vehicleHandle = API.GetVehiclePedIsIn(player.Character.Handle, false);

            if (!API.DoesEntityExist(vehicleHandle))
            {
                Debug.WriteLine($"Player {player.Name} is in no vehicle");
                return;
            }

            Vehicle vehicle = (Vehicle)Vehicle.FromHandle(vehicleHandle);

            IDictionary<string, object> bagValue = vehicle.State["TestBag"];

            if (bagValue == null)
            {
                Debug.WriteLine($"Bag is empty!");
                return;
            }

            foreach (KeyValuePair<string, object> kvp in bagValue)
            {
                Debug.WriteLine($"Key: {kvp.Key} | Value: {kvp.Value}");
            }
        }

        [Command("setbagserver")]
        private void OnSetBagServer([FromSource] Player player, string[] args)
        {
            Debug.WriteLine($"OnSetBagServer | Player: {player.Name} | Args: {(String.Join(", ", args))}");

            if (args.Length < 2)
            {
                Debug.WriteLine($"We need at least 2 args");
                return;
            }

            int vehicleHandle = API.GetVehiclePedIsIn(player.Character.Handle, false);

            if (!API.DoesEntityExist(vehicleHandle))
            {
                Debug.WriteLine($"Player {player.Name} is in no vehicle");
                return;
            }

            Vehicle vehicle = (Vehicle)Vehicle.FromHandle(vehicleHandle);

            if (vehicle == null)
            {
                Debug.WriteLine($"We are in no vehicle");
                return;
            }

            IDictionary<string, object> bagValue = vehicle.State["TestBag"];
            Dictionary<string, object> dict;

            if (bagValue == null)
                dict = new Dictionary<string, object>();
            else
                dict = new Dictionary<string, object>(bagValue);

            if (dict.ContainsKey(args[0]))
                dict[args[0]] = args[1];
            else
                dict.Add(args[0], args[1]);

            vehicle.State.Set("TestBag", dict, true);
        }
    }
}
