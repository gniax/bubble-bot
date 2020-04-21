using System;

namespace BubbleBot.Utility.Security
{
    public static class YeastAPI
    {
        private static readonly char[] _Alphabet =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_".ToCharArray();

        private static string _PreviousKey = string.Empty;
        private static int _Seed;

        public static string Encode(long number)
        {
            var encoded = string.Empty;

            do
            {
                encoded = _Alphabet[number % _Alphabet.Length] + encoded;
                number = (long) Math.Floor((double) number / _Alphabet.Length);
            } while (number > 0);

            return encoded;
        }

        public static long Decode(string value)
        {
            long decoded = 0;

            for (var i = 0; i < value.Length; i++)
                decoded = decoded * _Alphabet.Length + Array.IndexOf(_Alphabet, value[i]);

            return decoded;
        }

        public static string GenerateKey()
        {
            var now = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var key = Encode(now);

            if (key != _PreviousKey)
            {
                _Seed = 0;
                _PreviousKey = key;
                return key;
            }

            return $"{key}.{Encode(_Seed++)}";
        }
    }
}