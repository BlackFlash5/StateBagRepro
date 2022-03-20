using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace StateBagClient
{
    public class ClientBase : BaseScript
    {
        public ClientBase()
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

        [Command("bagclient")]
        private void OnBagClient()
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle == null)
            {
                Debug.WriteLine($"We are in no vehicle");
                return;
            }

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

        [Command("setbagclient")]
        private void OnSetBagClient(string[] args)
        {
            Debug.WriteLine($"OnSetBagClient | Args: {(String.Join(", ", args))}");

            if (args.Length < 2)
            {
                Debug.WriteLine($"We need at least 2 args");
                return;
            }

            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

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
