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
            /*
            b5c3: -13
            c2c3: -9
            c1d2: -13
            d1d2: -25
            */
            /*
            rnbqkbnr/ppppppp1/7p/8/8/5N2/PPPPPPPP/RNBQKB1R w KQkq - 0 2
            */
            //8/5k1q/3P4/6K1/PP6/4P3/4P3/8 b - - 3 43
            //rnb3n1/pppp4/5k2/8/8/P3P3/PBPPP1K1/R7 b - - 2 21
            //r3k2R/pp6/1n6/2p1p3/8/N3K1P1/PPPBPP2/8 b q - 2 20
            string FEN= "r1b1k1nr/pp1p1p1p/2pb4/4p1Bq/1n1P4/2NQ3N/PPP1PPPP/R3KBR1 w Qkq - 3 11";
            b.updateBoard(FEN);
            List<string> moves_chosen=new List<string>();

            string selectedMove = algo.AlphaBetaSearch(ref b,2,ref moves_chosen);


            Console.WriteLine("--------------------------");
            Console.WriteLine(b);
            Console.WriteLine("SCORE: {0},{1}",b.BlackScore,b.WhiteScore);
            Console.WriteLine("**************************");
            for(int i=0;i<moves_chosen.Count();i++) //list out moves
            {
                Console.WriteLine(moves_chosen[i]);
            }   
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
            List<string> moves= new List<string>(b.move_select(Mode.select));
            //List<string> moves= new List<string>(b.move_select(1,2));
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
    }
}
