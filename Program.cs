using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Joueur.cs.Games.Chess.Logic;

namespace Joueur.cs.Games.Chess
{
    class Program
    {
        static Board b;
        static void Main(string[] args)
        {
            //CastlingTest1();
            //DoNotBecauseCheckMate();
            //SomeError();
            //SomeError2();
            //fixcheckmatemove2();
            movetest();        
        }
        static void movetest()
        {
            Console.WriteLine("moveTest");
            b=new Board(8);
            //string FEN= "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            //8/r1P1nr2/2k2P1p/p6P/P1P5/5R2/4K3/1bq3R1 w - - 0 70
            //r1b1kN2/pp6/5P2/2p5/8/N4P2/PPPP1PPp/R1BQKBR1 b Qq - 0 16 player disconnects
            //5N2/1Bp5/8/1k2N3/8/6PP/PPPPPP1R/1RBQK3 b - - 2 26 disconnect
            //8/k7/7Q/7P/1P1PP3/N6B/PP1P3P/R1B1K3 b - e3 0 43 disconnect
            //1nbqkb1N/p7/8/8/3NB3/2p1P1PP/PPP1P2R/R1BQK3 w - - 0 18 disconnect
            //rn2k2r/p2N4/8/8/7p/N2PP2P/PP2PPP1/R1BQKB1R b Qkq - 0 15
            //rnbqkbnr/ppppN3/8/8/6p1/N7/PPPPPPPP/1RBQKB1R b Kkq - 0 6
            //rnbqk3/1n6/2p1p3/3p1pbp/pp1P3p/3BP3/PPPNKPPP/RQ1R4 b q - 43 55 tries to casle
            //http://vis.siggame.io/?log=http%3A%2F%2Fcashdomain.tk%3A3080%2Fgamelog%2FChess-MyOwnGamesession-2020.04.07.02.40.35.806
            //r1b1kbnr/pppp2pp/4pq2/8/3n1B2/2N2N2/PPPQPPPP/R3KBR1 w Qkq - 0 7 ohno
            //r1b1k1nr/pp1p1p1p/2pb4/4p1Bq/1n1P4/2NQ3N/PPP1PPPP/R3KBR1 w Qkq - 3 11
            //http://vis.siggame.io/?log=http%3A%2F%2Fcashdomain.tk%3A3080%2Fgamelog%2FChess-MyOwnGamesession-2020.04.21.04.38.34.523
            //rnbqk1nr/pppp1p2/6pp/1N2p3/1b5P/3P4/PPP1PPP1/R1BQKBNR w KQkq - 1 5
            string FEN= "rnbqk1nr/pppp1p2/6pp/1N2p3/1b5P/3P4/PPP1PPP1/R1BQKBNR w KQkq - 1 5";
            b.updateBoard(FEN);

            string selectedMove = algo.AlphaBetaSearch(ref b,2);


            Console.WriteLine("--------------------------");
            Console.WriteLine(b);
            Console.WriteLine("SCORE: {0},{1}",b.BlackScore,b.WhiteScore);
            Console.WriteLine("**************************");
            Console.WriteLine(selectedMove);
            b.MakeMove(selectedMove);
            //Console.WriteLine(b.fenstring);
            Console.WriteLine("**************************");
            Console.WriteLine("--------------------------");
            Console.WriteLine(b);
            Console.WriteLine("SCORE: {0},{1}",b.BlackScore,b.WhiteScore);


        }
        static void fixcheckmatemove2()
        {
            Console.WriteLine("fixcheckmatemove");
            b=new Board(8);
            //2N4/p7/1p1p4/8/2Bk1n2/6p1/5P2/2r3K1 w - - 2 77
            //4r1nr/p1nQ1k2/3P1p1p/1p2p3/1P1R3P/B3PP2/P3BPR1/4K1N1 b - - 0 29
            //4k2n/3b4/p2p2nP/3p1p1p/p4PpP/4P1N1/1rK3R1/2N1B3 w - - 0 61

            //8/5k2/6nq/1P3b2/P5R1/2r3K1/3q4/7Q w - - 0 108
            //8/r1P1nr2/2k2P1p/p6P/P1P5/5R2/4K3/1bq3R1 w - - 0 70
            //Nnbq1bnN/p2k4/8/8/4p2P/3P1p2/PP1PPPP1/R1BQKBR1 w - - 0 16
            //r1bqkbnr/ppppN3/2n5/8/6p1/N7/PPPPPPPP/1RBQKB1R w Kkq - 1 7 //produces no moves
            //rnbqkbnr/ppppN3/8/8/8/N5p1/PPPPPPPP/1RBQKB1R w Kkq - 0 6 own
            //rnb1kbnr/ppppq3/8/8/8/N6p/PPPPPPP1/1RBQKB1R w Kkq - 0 8 check numbers
            string FEN= "rnb1kbnr/ppppq3/8/8/8/N6p/PPPPPPP1/1RBQKB1R w Kkq - 0 8";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            //List<string> moves= new List<string>(b.selectPiece(1,2));
            Console.WriteLine(b);
            G.printlines(moves);
            string selectedMove;
            if(moves.Count>1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else if(moves.Count==1)
            {
                selectedMove=moves[0];
            }
            else
            {
                selectedMove="fail";
            }
            Console.WriteLine("Selected: "+selectedMove);
            Console.WriteLine("SCORE: {0},{1}",b.BlackScore,b.WhiteScore);
        }
        
        static void fixcheckmatemove()
        {
            Console.WriteLine("fixcheckmatemove");
            b=new Board(8);

            string FEN= "1nr5/1B2nb2/k3p1p1/p7/p2p2P1/1Q6/1K6/3R4 b - - 1 48";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            Console.WriteLine(b);
            G.printlines(moves);
            string selectedMove;
            if(moves.Count>1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else if(moves.Count==1)
            {
                selectedMove=moves[0];
            }
            else
            {
                selectedMove="fail";
            }
            Console.WriteLine("Selected: "+selectedMove);
        }

        static void SomeError2()
        {
            Console.WriteLine("Weird error");
            b=new Board(8);

            string FEN= "rnbqkbnr/1pp1pppp/3p4/pB6/8/2P1P3/PP1P1PPP/RNBQK1NR b KQkq - 1 3";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            Console.WriteLine(b);
            G.printlines(moves);
            string selectedMove;
            if(moves.Count>1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else if(moves.Count==1)
            {
                selectedMove=moves[0];
            }
            else
            {
                selectedMove="fail";
            }
            Console.WriteLine("Selected: "+selectedMove);
        }

        static void DoNotBecauseCheckMate()
        {
            Console.WriteLine("Castling Failure Test");
            b=new Board(8);

            string FEN= "2b5/1pN1k2p/r1pr1ppP/p2pPP2/P3p3/2B1P3/2n4Q/R1K2BqR w - - 1 24";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(7,5)); //results in 0 moves
            Console.WriteLine(b);
            G.printlines(moves);
            string selectedMove;
            if(moves.Count!=1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else
            {
                selectedMove=moves[0];
            }
            Console.WriteLine("Selected: "+selectedMove);
        }
        static void SomeError()
        {
            Console.WriteLine("Weird error");
            b=new Board(8);

            string FEN= "4r2B/1k4PN/pp6/P2p4/1p2pP1R/2b3P1/7R/7K w - - 4 62";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            Console.WriteLine(b);
            G.printlines(moves);
            string selectedMove;
            if(moves.Count!=1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else
            {
                selectedMove=moves[0];
            }
            Console.WriteLine("Selected: "+selectedMove);
        }
        static void CastlingTest1()
        {
            Console.WriteLine("Castling Failure Test");
            b=new Board(8);

            string FEN= "r2qkbr1/p3p2p/1p5n/2pp1pp1/8/B1P3PN/P1bNPP1P/R3KB1R w Kq f6 0 14";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(7,4));
            Console.WriteLine(b);
            G.printlines(moves);
            string selectedMove;
            if(moves.Count!=1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else
            {
                selectedMove=moves[0];
            }
            Console.WriteLine("Selected: "+selectedMove);
        }

        static void test()
        {
            Console.WriteLine("Chess Test");
            b=new Board(8);
            //Console.WriteLine(b);
            //string FEN="rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";
            //string FEN="rnbqk2r/ppp1ppbp/3p1np1/8/2PPPP2/2N5/PP4PP/R1BQKBNR b KQkq f3 1 5";
            //string FEN="5n2/8/8/8/8/8/8/5N2 w - - 0 1";

            //Cell currentCell = setCurrentCell();
            //currentCell.CurrentlyOccupied=true;

            //rook
            //b.grid[currentCell.RowNumber-2,currentCell.ColoumnNumber].CurrentlyOccupied=true;
            //b.grid[currentCell.RowNumber+2,currentCell.ColoumnNumber].CurrentlyOccupied=true;
            //b.grid[currentCell.RowNumber,currentCell.ColoumnNumber-2].CurrentlyOccupied=true;
            //b.grid[currentCell.RowNumber,currentCell.ColoumnNumber+2].CurrentlyOccupied=true;
            
            //b.grid[currentCell.RowNumber-1,currentCell.ColoumnNumber-1].CurrentlyOccupied=true;
            //
            //Console.WriteLine(b);
            //b.MarkNextLegalMoves(currentCell,"Bishop");
            //Console.WriteLine(b);

            //TEST PIECE standard Board
            //string FEN="rn1q1rk1/pb4p1/1p2p3/2pp1pPp/P1P1nP2/3P4/1B1NBP1P/R2Q1RK1 b - h6 0 16";
            //string FEN="rnbqk2r/ppp3p1/3b1n2/5p1p/3Pp2P/2N2P2/PPP1N1B1/R1BQK2R b KQkq - 0 10"; //castle king black
            //string FEN="3rqb2/p1pb2pr/Bp3p1p/1PQ1p2k/2P3nP/P3PP2/1B1P2P1/1R3KNR b - - 10 20"; //king is h5,check is checkmate
            //List<string> moves= new List<string>(b.selectPiece(3,7));//SELECT KING BLACK

            //string FEN="rn3b1r/pp2pk1p/B1pp1p2/4P1p1/8/NP3q1N/P1PP1PPP/1RBbK2R w K g6 0 13";//check h2pawnmoves
            //List<string> moves= new List<string>(b.selectPiece(6,7));//selectPawn

            string FEN= "r3kb1r/pp2pppp/1q1p2B1/2p3Pn/P2P4/4Pn1b/1PP1NP2/R1BQK1NR w KQ - 3 15";
            b.updateBoard(FEN);
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            Console.WriteLine(b);
            //List<string> moves= new List<string>(b.selectPiece(Mode.random));
            //List<string> moves= new List<string>(b.selectPiece(Mode.select));
            G.printlines(moves);
            string selectedMove;
            if(moves.Count!=1)
            {
                selectedMove=moves[RNG.GenerateRandomNumber(0,moves.Count()-1)];                
            }
            else
            {
                selectedMove=moves[0];
            }
            Console.WriteLine("Selected: "+selectedMove);

            //string FEN="3rqb2/p1pb2pr/Bp3p1p/1PQ1p2k/2P3nP/P3PP2/1B1P2P1/1R3KNR b - - 10 20"; //king is h5,check is checkmate
            
            //List<string> moves= new List<string>(b.selectPiece(0,4));//king move

            //Rook
            //List<string> moves= new List<string>(b.selectPiece(0,5));
            //Queen
            //List<string> moves= new List<string>(b.selectPiece(0,3));
            //Knight
            //List<string> moves= new List<string>(b.selectPiece(4,4));
            //List<string> moves= new List<string>(b.selectPiece(6,3)); //white
            //Console.WriteLine("MOVES:");
            //foreach(string s in moves)
            //{
            //    Console.WriteLine(s);
            //}
            //Console.WriteLine("-----------\n");
            //Console.WriteLine(b);
        }
    }
}
