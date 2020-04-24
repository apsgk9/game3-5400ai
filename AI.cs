// This is where you build your AI for the Chess game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add additional using(s) here
using Joueur.cs.Games.Chess.Logic;
using System.Diagnostics;
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Chess
{
    /// <summary>
    /// This is where you build your AI for Chess.
    /// </summary>
    public class AI : BaseAI
    {
        #region Properties
        #pragma warning disable 0169 // the never assigned warnings between here are incorrect. We set it for you via reflection. So these will remove it from the Error List.
        #pragma warning disable 0649
        /// <summary>
        /// This is the Game object itself. It contains all the information about the current game.
        /// </summary>
        public readonly Game Game;
        /// <summary>
        /// This is your AI's player. It contains all the information about your player's state.
        /// </summary>
        public readonly Player Player;
        #pragma warning restore 0169
        #pragma warning restore 0649

        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional properties here for your AI to use

        static Board b; //BOARD STATIC INSTANCE-------------------------------
        bool firstTurn=true;
        Stopwatch stopWatch;
        const double timeGiven=900000000000;

        // <<-- /Creer-Merge: properties -->>
        #endregion


        #region Methods
        /// <summary>
        /// This returns your AI's name to the game server. Just replace the string.
        /// </summary>
        /// <returns>Your AI's name</returns>
        public override string GetName()
        {
            // <<-- Creer-Merge: get-name -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            return "Chess C# Player"; // REPLACE THIS WITH YOUR TEAM NAME!
            // <<-- /Creer-Merge: get-name -->>
        }

        /// <summary>
        /// This is automatically called when the game first starts, once the Game and all GameObjects have been initialized, but before any players do anything.
        /// </summary>
        /// <remarks>
        /// This is a good place to initialize any variables you add to your AI or start tracking game objects.
        /// </remarks>
        public override void Start()
        {
            // <<-- Creer-Merge: start -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            b=new Board(8);
            stopWatch = new Stopwatch();
            base.Start();
            // <<-- /Creer-Merge: start -->>
        }

        /// <summary>
        /// This is automatically called every time the game (or anything in it) updates.
        /// </summary>
        /// <remarks>
        /// If a function you call triggers an update, this will be called before that function returns.
        /// </remarks>
        public override void GameUpdated()
        {
            // <<-- Creer-Merge: game-updated -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.GameUpdated();
            // <<-- /Creer-Merge: game-updated -->>
        }

        /// <summary>
        /// This is automatically called when the game ends.
        /// </summary>
        /// <remarks>
        /// You can do any cleanup of you AI here, or do custom logging. After this function returns, the application will close.
        /// </remarks>
        /// <param name="won">True if your player won, false otherwise</param>
        /// <param name="reason">A string explaining why you won or lost</param>
        public override void Ended(bool won, string reason)
        {
            // <<-- Creer-Merge: ended -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Ended(won, reason);
            // <<-- /Creer-Merge: ended -->>
        }


        /// <summary>
        /// This is called every time it is this AI.player's turn to make a move.
        /// </summary>
        /// <returns>A string in Universal Chess Inferface (UCI) or Standard Algebraic Notation (SAN) formatting for the move you want to make. If the move is invalid or not properly formatted you will lose the game.</returns>
        public string MakeMove()
        {
            // <<-- Creer-Merge: makeMove -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            // Put your game logic here for makeMove
            Console.WriteLine("----------------STARTTURN:{0}------------------",Game.History.Count());
            Stopwatch stopWatch_move = new Stopwatch();
            stopWatch.Reset();

            string fen_string = Game.Fen;
            //limit max moves to 20seconds
            b.updateBoard(fen_string);
            Console.WriteLine(b);
            //List<string> Moves_Chosen= new List<string>();
            string selectedMove;
            double timepredict, timeLimit,timeratio;
            double timespan_previous,timespan_next;
            int depth=1;

            //double t0 = stopWatch.ElapsedMilliseconds * 1000000;
            //t0= stopWatch.ElapsedMilliseconds * 1000000;

            timeLimit = Player.TimeRemaining*0.015;
            stopWatch_move.Start();
            
            //Console.WriteLine("timeLimit: {0}",timeLimit);
            //Console.WriteLine("Player.TimeRemaining: {0}",Player.TimeRemaining);
            
            stopWatch.Start();
            selectedMove = algo.AlphaBetaSearch(ref b,depth);
            stopWatch.Stop();
            timespan_previous=stopWatch.ElapsedMilliseconds * 1000000;
            //Console.WriteLine("timespan_previous: {0}",timespan_previous);
            stopWatch.Reset();
            
            depth++;
            double timecurrently;
            bool should_loop;
            if(firstTurn)
            {
                timeLimit=0;
                firstTurn=!firstTurn;
            }
            /*
            do
            {
                
                b.updateBoard(fen_string);
                stopWatch.Start();
                selectedMove = algo.AlphaBetaSearch(ref b,depth);
                stopWatch.Stop();
                timespan_next=stopWatch.ElapsedMilliseconds * 1000000;
                //Console.WriteLine("timespan_next: {0}",timespan_next);
                stopWatch.Reset();

                timeratio= (timespan_previous)/(timespan_next); //previous elapsed time/this elapsed time
                timepredict= (timespan_next)*timeratio;
                //Console.WriteLine("depth: {0} ",depth);
                //Console.WriteLine("timeratio: {0} || timepredict: {1}\n",timeratio,timepredict);
                
                depth++;
                timespan_previous=timespan_next;
                timecurrently=stopWatch_move.ElapsedMilliseconds *1000000;
                //Console.WriteLine("timecurrently+timepredict: {0}\n",(timecurrently+timepredict));
                //Console.WriteLine("timeLimit: {0}\n",timeLimit);
                should_loop =(timecurrently+timepredict)<timeLimit;
                //Console.WriteLine("should_loop: {0}\n",should_loop);

                if(depth==6)
                {
                    should_loop=false;
                }
            }while(should_loop);*/

            //for(int i=0;i<Moves_Chosen.Count();i++)
            //{
            //    Console.WriteLine(Moves_Chosen[i]);
            //} 

            stopWatch_move.Stop();
            timecurrently=stopWatch_move.ElapsedMilliseconds *1000000;


            Console.WriteLine("Selected: "+selectedMove);
            Console.WriteLine("Time For this Move: {0}\n",(timecurrently));
            Console.WriteLine("----------------ENDTURN:{0}------------------",Game.History.Count());
            stopWatch.Stop();
            return selectedMove;
            // <<-- /Creer-Merge: makeMove -->>
        }

        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional methods here for your AI to call
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
