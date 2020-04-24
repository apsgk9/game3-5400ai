using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Joueur.cs.Games.Chess.Logic {

    public struct Action {
        public string move { get; set; }
        public int value { get; set; }
        public Action (Action action) {
            move = action.move;
            value = action.value;
        }
        public Action (string input_move,int input_value) {
            move = input_move;
            value = input_value;
        }
        public static bool operator == (Action lhs, Action rhs) {
            return ( lhs.value == rhs.value) ? true : false;
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return value == ((Action)obj).value;
        }
        
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            //throw new System.NotImplementedException();
            return base.GetHashCode();
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
            return move+","+value.ToString();
        }
    }
    class algo {


        
        public static Func<Action, Action, bool> lesserThan= (A1,A2)=> A1<A2; //formax
        public static Func<Action, Action, bool> greaterThan= (A1,A2)=> A1>A2; //formmin
        
        public static Func<int, int, bool> lesserThanInt= (A1,A2)=> A1<A2; //formax
        public static Func<int, int, bool> greaterThanInt= (A1,A2)=> A1>A2; //formmin
        public static string AlphaBetaSearch (ref Board b, int depth) {
            //generate all valid moves
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            //G.printlines(moves);

            List<Action> Action_list= new List<Action>();
            string test = "test";
            Action Action_Chosen= new Action(test,-1);
            //Console.WriteLine("player: "+b.nextToMove);
            int alpha= int.MinValue;
            int beta= int.MaxValue;

            int toPass_alpha= alpha;
            int toPass_beta= beta;

            int best_valuefound= int.MinValue;
            //int toPass_Value= int.MinValue;
            
            //Calculates at the depth, not internal nodes
            if(moves.Count()>0)
            {
                for(int i=0;i<moves.Count();i++)
                {
                    //make new board from move
                    Board new_board = new Board(b);
                    new_board.MakeMove(moves[i]);
                    int value=minValue(new_board,depth-1,b.nextToMove,toPass_alpha,toPass_beta);//, ref toPass_Value);
                    Action_list.Add(new Action(moves[i],value));
                    if(value>=best_valuefound) 
                    {
                        best_valuefound=value; 
                    } //continue if v is smaller than beta
    
                    //decides if worth to explore next children
                    // if the max value of the node being explored so far happens to be >= to beta
                    //don't look at any further children
                    if(best_valuefound>=toPass_beta)
                    {
                        break;
                    }
                    //α := MAX(α,v)
                    toPass_alpha=best_valuefound; //this is where alpha is changed
                }
                
                for(int i=0;i<Action_list.Count();i++)
                {
                    Console.WriteLine(Action_list[i].move +": "+ Action_list[i].value);
                }   
                Action_Chosen= compare(Action_list,lesserThan);
            }
            else //fortesting
            {
                Console.WriteLine("OHNO");
            }

            return Action_Chosen.move;
        }
        /*  Brief: Finds Minimum Value of its Children (max functions)
        /   keep track of alpha parameter
        *   @param[in] b board to make moves from
        *   @param[in] player player to move (based on enum Turn)
        *   @param[in] given_alpha the value of the best choice along for max
        *   @param[in] given_beta the value of the best choice along for min
        *   @pre player must be range of enum TURN
        *   @post returns max heuristic value of its children or this node
        */
        public static int maxValue (Board b, int depth,int player,int given_alpha,int given_beta){//, ref int given_value) {
            //generate all valid moves
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            List<int> Value_list= new List<int>();
            
            if(moves.Count() == 0 || depth==0)//terminal//dead end
            {
                int scoreCalculated=b.calculateScore(player);
                //given_value=scoreCalculated;
                return scoreCalculated;
            }

            int this_alpha= given_alpha;
            int this_beta= given_beta; //beta doesn't get changed

            //int toPass_alpha= given_alpha;
            //int toPass_beta= given_beta; //doesn't change here
            
            int best_valuefound= int.MinValue;
            //int toPass_Value= int.MinValue;

            int bestAction= new int();

            for(int i=0;i<moves.Count();i++)
            {
                //make new board from move
                Board new_board = new Board(b);
                new_board.MakeMove(moves[i]);
                Value_list.Add(minValue(new_board,depth-1,player,this_alpha,this_beta));//, ref toPass_Value));
                if(Value_list.Last()>=best_valuefound) 
                {
                    best_valuefound=Value_list.Last(); 
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
                this_alpha=best_valuefound; //this is where alpha is changed
            }
            //return best value

            //given_value=best_valuefound;
            bestAction = compare(Value_list,lesserThanInt); //MAX
            return bestAction;
        }



        
        /*  Brief: Finds Minimum Value of its Children (max functions)
        *   keep track of alpha parameter
        *   @param[in] b board to make moves from
        *   @param[in] player player to move (based on enum Turn)
        *   @param[in] given_alpha the value of the best choice along for max
        *   @param[in] given_beta the value of the best choice along for min
        *   @pre player must be range of enum TURN
        *   @post returns mininum heuristic value of its children or this node
        */
        public static int minValue (Board b, int depth,int player,int given_alpha,int given_beta){//, ref int given_value) {
            //Console.WriteLine("MIN:"+depth);

            //generate all valid moves
            List<string> moves= new List<string>(b.selectPiece(Mode.select));
            List<int> Value_list= new List<int>();

            if(moves.Count() == 0 || depth==0)//terminal//dead end
            {
                int scoreCalculated=b.calculateScore(player);
                //given_value=scoreCalculated;
                return scoreCalculated;
            }

            int this_alpha= given_alpha;
            int this_beta= given_beta;
            //int toPass_alpha= given_alpha;
            //int toPass_beta= given_beta;

            int best_valuefound= int.MaxValue;
            //int toPass_Value= int.MaxValue;

            int worstAction= new int();
            
            for(int i=0;i<moves.Count();i++)
            {
                Board new_board = new Board(b);
                new_board.MakeMove(moves[i]);
                Value_list.Add(maxValue(new_board,depth-1,player,this_alpha,this_beta));//, ref toPass_Value));
                if(Value_list.Last()<=best_valuefound)
                {
                    best_valuefound=Value_list.Last(); 
                } //continue if v is smaller than beta
                // if the max value of the node being explored so far happens to be >= to beta
                if(best_valuefound<=this_alpha)
                {
                    //return V
                    break;
                }
                //α := MAX(α,v)
                this_beta=best_valuefound; //this is where alpha is changed
            }

            //given_value=best_valuefound;
            worstAction = compare(Value_list,greaterThanInt);
            return worstAction;
        }

        /*  Brief: Compare List 
        *   @param[in] CompareList list to compare agaisn't itself
        *   @param[in] compareFunc T,T,bool function to compare list
        *   @pre CompareList cannot be empty
        *   @pre T must define .Equals and =
        *   @post  Returns the element that is the best according to compareFunc
        */
        public static T compare<T>(List<T> CompareList,Func<T,T,bool> compareFunc)
        {
            int count=CompareList.Count();
            
            T toReturn=CompareList[0];

            for(int i=1;i<count;i++)
            {
                T comparedTo=CompareList[i];
                if(compareFunc(toReturn,comparedTo)) //left is obselete if true
                {
                    toReturn=CompareList[i];
                }
                else if(toReturn.Equals(comparedTo)) //random 50/50 assingment if the same
                {
                    double rand= RNG.GenerateRandomNumber(0,100);
                    toReturn= (rand<=50)? toReturn : CompareList[i];
                }
            }
            return toReturn;
        }
            

    }
}