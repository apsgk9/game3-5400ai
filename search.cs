using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Joueur.cs.Games.Chess.Logic {

    public struct Action {
        public List<string> movelist { get; set; }
        public int value { get; set; }
        public Action (Action action) {
            movelist = action.movelist;
            value = action.value;
        }
        public Action (List<string> input_movelist,int input_value) {
            movelist = input_movelist;
            value = input_value;
        }
        public static bool operator == (Action lhs, Action rhs) {
            return ( lhs.value == rhs.value) ? true : false;
        }
        public static bool operator != (Action lhs, Action rhs) {
            return ( lhs.value != rhs.value) ? true : false;
        }
        public static bool operator < (Action lhs, Action rhs) {
            return (lhs.value < rhs.value) ? true : false;
        }
        public static bool operator > (Action lhs, Action rhs) {
            return (lhs.value > rhs.value) ? true : false;
        }

        public override string ToString () {
            return movelist[0].ToString()+","+value.ToString();
        }
    }
    class algo {


        
        public static Func<Action, Action, bool> lesserThan= (A1,A2)=> A1<A2; //formax
        public static Func<Action, Action, bool> greaterThan= (A1,A2)=> A1>A2; //formmin
        public static string AlphaBetaSearch (ref Board b, int depth) {
            //generate all valid moves
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            //G.printlines(moves);

            List<Action> Action_list= new List<Action>();
            List<string> test = new List<string>{"test"};
            Action Action_Chosen= new Action(test,-1);
            //Console.WriteLine("player: "+b.nextToMove);
            int alpha= int.MinValue;
            int beta= int.MaxValue;

            int toPass_alpha= alpha;
            int toPass_beta= beta;

            int best_valuefound= int.MinValue;
            int toPass_Value= int.MinValue;
            
            //Calculates at the depth, not internal nodes
            if(moves.Count()>0)
            {
                for(int i=0;i<moves.Count();i++)
                {
                    //make new board from move
                    Board new_board = new Board(b);
                    new_board.MakeMove(moves[i]);
                    Action_list.AddRange(minValue(new_board,depth-1,b.nextToMove,toPass_alpha,toPass_beta, ref toPass_Value));
                    if(toPass_Value>=best_valuefound) 
                    {
                        best_valuefound=toPass_Value; 
                    } //continue if v is smaller than beta
    
                    //decides if worth to explore next children
                    // if the max value of the node being explored so far happens to be >= to beta
                    if(best_valuefound>=toPass_beta)
                    {
                        //don't look at any further children
                        //return V
                        break;
                    }
                    //α := MAX(α,v)
                    toPass_alpha=best_valuefound; //this is where alpha is changed
                }
                
                for(int i=0;i<Action_list.Count();i++)
                {
                    Console.WriteLine(Action_list[i].movelist[0] +": "+ Action_list[i].value);
                }   
                Action_Chosen= compare(Action_list,lesserThan);
            }
            else //fortesting
            {
                Console.WriteLine("OHNO");
            }

            return Action_Chosen.movelist[0];
        }
        //keep track of alpha parameter
        public static List<Action> maxValue (Board b, int depth,int player,int given_alpha,int given_beta, ref int given_value) {
            //generate all valid moves
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            List<Action> Action_list= new List<Action>();
            
            if(moves.Count() == 0 || depth==0)//terminal//dead end
            {
                int scoreCalculated=b.calculateScore(player);
                given_value=scoreCalculated;
                Action_list.Add(new Action(b.moveHistory,scoreCalculated));
                return Action_list;
            }


            int this_alpha= given_alpha;
            int this_beta= given_beta; //beta doesn't get changed

            int toPass_alpha= given_alpha;
            int toPass_beta= given_beta; //doesn't change here
            
            int best_valuefound= int.MinValue;
            int toPass_Value= int.MinValue;

            Action bestAction= new Action();

            for(int i=0;i<moves.Count();i++)
            {
                //make new board from move
                Board new_board = new Board(b);
                new_board.MakeMove(moves[i]);
                Action_list.AddRange(minValue(new_board,depth-1,player,toPass_alpha,toPass_beta, ref toPass_Value));
                if(toPass_Value>=best_valuefound) 
                {
                    best_valuefound=toPass_Value; 
                } //continue if v is smaller than beta

                //decides if worth to explore next children
                // if the max value of the node being explored so far happens to be >= to beta
                if(best_valuefound>=this_beta)
                {
                    //don't look at any further children
                    //return V
                    break;
                }
                //α := MAX(α,v)
                toPass_alpha=best_valuefound; //this is where alpha is changed
            }
            //return best value

            given_value=best_valuefound;
            bestAction = compare(Action_list,lesserThan); //MAX
            return new List<Action>(){bestAction};
        }



        //keep track of beta parameter
        public static List<Action> minValue (Board b, int depth,int player,int given_alpha,int given_beta, ref int given_value) {
            //Console.WriteLine("MIN:"+depth);

            //generate all valid moves
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            List<Action> Action_list= new List<Action>();

            if(moves.Count() == 0 || depth==0)//terminal//dead end
            {
                int scoreCalculated=b.calculateScore(player);
                given_value=scoreCalculated;
                Action_list.Add(new Action(b.moveHistory,b.calculateScore(player)));
                return Action_list;
            }

            int this_alpha= given_alpha;
            int this_beta= given_beta;
            int toPass_alpha= given_alpha;
            int toPass_beta= given_beta;

            int best_valuefound= int.MaxValue;
            int toPass_Value= int.MaxValue;

            Action worstAction= new Action();
            
            for(int i=0;i<moves.Count();i++)
            {
                Board new_board = new Board(b);
                new_board.MakeMove(moves[i]);
                Action_list.AddRange(maxValue(new_board,depth-1,player,toPass_alpha,toPass_beta, ref toPass_Value));
                if(toPass_Value<=best_valuefound)
                {
                    best_valuefound=toPass_Value; 
                } //continue if v is smaller than beta
                // if the max value of the node being explored so far happens to be >= to beta
                if(best_valuefound<=this_alpha)
                {
                    //return V
                    break;
                }
                //α := MAX(α,v)
                toPass_beta=best_valuefound; //this is where alpha is changed
            }

            given_value=best_valuefound;
            worstAction = compare(Action_list,greaterThan);
            return new List<Action>(){worstAction};
        }


        public static Action compare(List<Action> listofActions,Func<Action,Action,bool> compareFunc)
        {
            int count=listofActions.Count();
            
            Action toReturn=listofActions[0];

            for(int i=1;i<count;i++)
            {
                Action comparedTo=listofActions[i];
                if(compareFunc(toReturn,comparedTo)) //left is obselete if true
                {
                    toReturn=listofActions[i];
                }
                else if(toReturn == comparedTo)
                {
                    double rand= RNG.GenerateRandomNumber(0,100);
                    toReturn = (rand<=50)? toReturn : listofActions[i];
                }
            }
            return toReturn;
        }
            

    }
}