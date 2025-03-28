using Database;
using Database.Character;
using Database.Character.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Character
{
    public class CharacterManager : GameplayManager<CharacterManager>
    {
        #region ACTIONS

        public event Action OnPlayerCreated;

        #endregion

        #region VARIABLES

        [SerializeField, ValueDropdown(CharacterDataDatabase.GET_DATA_METHOD)] private int defaultPlayerCharacterDataId;
        [SerializeField] private List<CharacterBase> characters = new();

        private Dictionary<CharacterBase, bool> charactersTmp = new();

        #endregion

        #region PROPERTIES

        public Player Player { get; set; }

        #endregion

        #region UNITY_METHODS

        private void Update()
        {
            foreach (var characterTmp in charactersTmp)
            {
                if (characterTmp.Value)
                    characters.Add(characterTmp.Key);
                else
                    characters.Remove(characterTmp.Key);
            }

            charactersTmp.Clear();

            for (int i = 0; i < characters.Count; i++)
                characters[i].OnUpdate();
        }

        #endregion

        #region METHODS

        public override void LateInitialzie()
        {
            base.LateInitialzie();
            //TODO make spawnpoints etc
            if (Player == null)
                CreatePlayer();
        }

        public override void CleanUp()
        {
            //Only detach events. Dont clean up alive character for serialization
            foreach (var character in characters)
                character.DetachEvents();

            base.CleanUp();
        }

        /// <summary>
        /// Spawn setted character
        /// </summary>
        /// <typeparam name="T">Type of character like player or npc, enemy itc.</typeparam>
        /// <param name="dataId">Id of character data into CharacterDatabase</param>
        /// <returns></returns>
        public T CreateCharacter<T>(int dataId) where T : CharacterBase, new()
        {
            CharacterData data = MainDatabases.Instance.CharacterDataDatabase.GetData(dataId);
            return CreateCharacter<T>(data);
        }

        /// <summary>
        /// Spawn setted character
        /// </summary>
        /// <typeparam name="T">Type of character like player or npc, enemy itc.</typeparam>
        /// <param name="characterPoolId">Id of pooled character</param>
        /// <param name="data">Base data for character</param>
        /// <param name="poolCategoryId">Optional pool category of pooled character id [Optimization]</param>
        /// <returns></returns>
        public T CreateCharacter<T>(CharacterData data) where T : CharacterBase, new()
        {
            T character = new T();
            //T character = ObjectPool.Instance.GetFromPool(data.CharacterPoolId, data.CharacterCategoryPoolId).GetComponent<T>();

            character.SetData(data);
            character.Initialize();
            character.AttachEvents();
            if (character.TryCreateVisualization(transform))
                charactersTmp.Add(character, true);

            return character;
        }

        public void RemoveCharacter<T>(T character) where T : CharacterBase
        {
            character.CleanUp();
            character.DetachEvents();
            charactersTmp.Add(character, false);
        }

        [Button]
        private void CreatePlayer()
        {
            Player = CreateCharacter<Player>(defaultPlayerCharacterDataId);
            OnPlayerCreated?.Invoke();
        }

        #endregion
    }
}
