using UnityEngine;

namespace DaftAppleGames.Common.Weather
{
    public interface ITimeProvider
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}