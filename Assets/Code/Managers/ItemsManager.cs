using Gameplay.Character;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Items
{
    public class ItemsManager : MonoSingleton<ItemsManager>
    {

        #region VARIABLES

        private GameServerMock gameServerMock;

        #endregion

        #region PROPERTIES

        private CancellationTokenSource TokenSource { get; set; }
        public GameServerMock GameServerMock
        {
            get
            {
                if (gameServerMock == null)
                    gameServerMock = new();
                return gameServerMock;
            }
        }

        #endregion


        #region UNITY_METHODS

        private void OnDestroy()
        {
            TokenSource?.Cancel();
            TokenSource?.Dispose();
        }

        #endregion

        #region METHODS

        public async Task<List<Item>> GetRandomItems()
        {
            List<Item> items = new();
            TokenSource = new();
            string itemsData = await GameServerMock.GetItemsAsync(TokenSource.Token);

            if (TokenSource.IsCancellationRequested == true)
                return items;

            JObject jObjectOfItems = JObject.Parse(itemsData);
            if (jObjectOfItems.TryGetValue("Items", out var jToken))
            {
                var children = jToken.Children<JObject>();
                foreach (var child in children)
                {
                    Item item = new(child);
                    if (item.Data == null)
                    {
                        Debug.LogError($"Missing data in database for {item.ItemName}, check settings of database");
                        continue;
                    }
                    items.Add(item);
                }
            }

            return items;
        }

        [Button]
        private async void TestRandomItems()
        {
            if (CharacterManager.Instance.Player == null)
                return;

            List<Item> itemsToAdd = await GetRandomItems();
            itemsToAdd.ForEach(item => CharacterManager.Instance.Player.EquipmentController.CollectItem(item));
            Debug.Log("Done");
        }

        #endregion
    }
}