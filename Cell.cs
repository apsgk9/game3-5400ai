using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Chess.Logic {
    class Cell {
        public struct Piece {
            public char name { get; set; }
            public int colour { get; set; } // black : -1||white : 1||n/a: -1
            public int value { get; set; } // black : -1||white : 1||n/a: -1
            public Piece (char input_name, int input_colour, int input_value) {
                name = input_name;
                colour = input_colour;
                value = input_value;
            }
            public Piece (Piece p) {
                name = p.name;
                colour = p.colour;
                value = p.value;
            }
            
            public static bool operator == (Piece lhs, Piece rhs) {                
                return (lhs.name == rhs.name && lhs.colour == rhs.colour) ? true :false;
            }
            public static bool operator != (Piece lhs, Piece rhs) {
                return !(lhs == rhs);
            }

            public override string ToString () {
                return name.ToString ();
                //return "Piece: " + name + " " + colour;
            }
        }
        public int r_N { get; set; }
        public int c_N { get; set; }
        public bool CurrentlyOccupied { get; set; }
        //public bool LegalNextMove { get; set; }
        public string location { get; set; }
        public Piece piece { get; set; }

        public Cell () {
            c_N = -1;
            r_N = -1;
            CalculateLocation (-1, -1);
            CurrentlyOccupied=false;
            piece = new Piece (' ', -1, 0);
        }

        public Cell (int x, int y) { //col,row
            c_N = x;
            r_N = y;
            CalculateLocation (x, y);
            CurrentlyOccupied=false;
            piece = new Piece (' ', -1, 0);
        }
        static public Cell emptyCell (int x, int y) {
            return new Cell (x, y);
        }

        public Cell (Cell c) { 
            c_N = c.c_N;
            r_N = c.r_N;
            CurrentlyOccupied = c.CurrentlyOccupied;
            CalculateLocation (c.c_N,c.r_N);
            piece = new Piece (c.piece);
        }
        public Cell (Cell c,int x,int y) { //new location
            c_N = x;
            r_N = y;
            CurrentlyOccupied = true;
            CalculateLocation (c_N, r_N);
            piece = new Piece (c.piece);
        }


        public void clear () {
            CurrentlyOccupied = false;
            //LegalNextMove = false;
            piece = new Piece (' ', -1, 0);
        }
        
        public bool isEmpty () {
            return (piece.name==' ') ? true :false;
        }

        //returns if white or not
        public bool placePiece (char input_piece) {
            bool isWhite = false;
            if (char.IsUpper (input_piece)) //white pieces
            {
                int piece_value = EvaluateWhitePiece (input_piece);
                piece = new Piece (input_piece, 1,piece_value);
                isWhite = true;
            } else //black pieces
            {
                int piece_value = EvaluateBlackPiece (input_piece);
                piece = new Piece (input_piece, -1,piece_value);
            }
            CurrentlyOccupied = true;
            return isWhite;
        }

        public int EvaluateWhitePiece (char piece_name) {
            switch (piece_name) {
                case 'P':
                    return (int)PIECEVALUE.PAWN;
                case 'N':
                    return (int)PIECEVALUE.KNIGHT;
                case 'B':
                    return (int)PIECEVALUE.BISHOP;
                case 'R':
                    return (int)PIECEVALUE.ROOK;
                case 'Q':
                    return (int)PIECEVALUE.QUEEN;
                case 'K':
                    return (int)PIECEVALUE.KING;
            }
            return 0;
        }
        
        public int EvaluateBlackPiece (char piece_name) {
            switch (piece_name) {
                case 'p':
                    return (int)PIECEVALUE.PAWN;
                case 'n':
                    return (int)PIECEVALUE.KNIGHT;
                case 'b':
                    return (int)PIECEVALUE.BISHOP;
                case 'r':
                    return (int)PIECEVALUE.ROOK;
                case 'q':
                    return (int)PIECEVALUE.QUEEN;
                case 'k':
                    return (int)PIECEVALUE.KING;
            }
            return 0;
        }
        private void CalculateLocation (int x, int y) {
            location = "";
            switch (x) {    //col
                case 0:
                    location += "a";
                    break;
                case 1:
                    location += "b";
                    break;
                case 2:
                    location += "c";
                    break;
                case 3:
                    location += "d";
                    break;
                case 4:
                    location += "e";
                    break;
                case 5:
                    location += "f";
                    break;
                case 6:
                    location += "g";
                    break;
                case 7:
                    location += "h";
                    break;
            }
            switch (y) {    //row
                case 0:
                    location += "8";
                    break;
                case 1:
                    location += "7";
                    break;
                case 2:
                    location += "6";
                    break;
                case 3:
                    location += "5";
                    break;
                case 4:
                    location += "4";
                    break;
                case 5:
                    location += "3";
                    break;
                case 6:
                    location += "2";
                    break;
                case 7:
                    location += "1";
                    break;
            }
        }
        public static List<int> convertLocation (string input_location) {
            char[] chars = input_location.ToCharArray ();
            List<int> toReturn = new List<int> ();
            //returns col,row*****************
            switch (chars[0]) { //col
                case 'a':
                    toReturn.Add (0);
                    break;
                case 'b':
                    toReturn.Add (1);
                    break;
                case 'c':
                    toReturn.Add (2);
                    break;
                case 'd':
                    toReturn.Add (3);
                    break;
                case 'e':
                    toReturn.Add (4);
                    break;
                case 'f':
                    toReturn.Add (5);
                    break;
                case 'g':
                    toReturn.Add (6);
                    break;
                case 'h':
                    toReturn.Add (7);
                    break;
            }

            switch (chars[1]) { //row
                case '8':
                    toReturn.Add (0);
                    break;
                case '7':
                    toReturn.Add (1);
                    break;
                case '6':
                    toReturn.Add (2);
                    break;
                case '5':
                    toReturn.Add (3);
                    break;
                case '4':
                    toReturn.Add (4);
                    break;
                case '3':
                    toReturn.Add (5);
                    break;
                case '2':
                    toReturn.Add (6);
                    break;
                case '1':
                    toReturn.Add (7);
                    break;
            }
            return toReturn;
        }

        public override string ToString () {
            return location;
        }
    }
}