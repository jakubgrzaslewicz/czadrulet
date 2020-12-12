using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CzadRuletAPI.Hubs
{
    public static class ChatMemoryStorage
    {
        /**
         * Słownik przechowujący aktualnie otwarte pokoje
         * Klucz = RoomId (GUID)
         * Wartość = Lista identyfikatorów połączeń
         */
        public static Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();
    }
}