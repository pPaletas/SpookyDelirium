using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    public class ActorsManager : MonoBehaviour
    {
        public List<Actor> Actors { get; private set; }
        public List<GameObject> Players { get; private set; } = new List<GameObject>();

        public void SetPlayer(GameObject player) => Players.Add(player);

        

        void Awake()
        {
            Actors = new List<Actor>();
        }
    }
}