using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Joueur.cs.Games.Chess.Logic {
    public enum Mode {
        random = 0,
        select = 1
    }
    public enum TURN {
        BLACK = -1,
        WHITE = 1
    }

    public enum PIECEVALUE : int {
        PAWN = 2,
        KNIGHT = 6,
        BISHOP = 6,
        ROOK = 10,
        QUEEN = 18,
        KING = 7,
    }
        public enum MoveType : int {
        CAPTURE = 1,
        THREAT = 2,
        FORWARDMOVE = 3,
        BACKWARDMOVE = 4,
        PROMOTION = 5,
        NOTHING = 6
    }
    

    //-------------------------------RNG--------------------
    public class RNG {
        static public int GenerateRandomNumber (int min, int max) {
            return GenerateRandomNumberCRYPTO (min, max);
        }

        static int GenerateRandomNumberCRYPTO (int min, int max) {
            //Sourced from https://stackify.com/csharp-random-numbers/ 
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider ();
            //convert 4 bytes to an integer
            var byteArray = new byte[4];
            provider.GetBytes (byteArray);
            uint randomInteger = BitConverter.ToUInt32 (byteArray, 0);

            var modifiedRandNumber = (randomInteger % max) + min;
            //Console.WriteLine("GENERATED NUMBER" + modifiedRandNumber);
            return Convert.ToInt32 (modifiedRandNumber);
        }
    }
    public class G {

        public static void printlines (List<string> moves) {
            Console.WriteLine ("---------MOVES------------");
            foreach (string s in moves) {
                Console.WriteLine (s);
            }
            Console.WriteLine ("---------END------------");
        }

    }
}